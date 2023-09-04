using Kitchen;
using KitchenMods;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace AutoRestart {
    [UpdateBefore(typeof(CheckGameOverFromLife))]
    public class AutoRestart : CheckGameOverFromLife, IModSystem {
        protected static bool DisabledByCompat = false;
        private EntityQuery Patience;
        [ReadOnly]
        private EntityQuery OriginalKitchenStatus;
        private EntityQuery CurrentKitchenStatus;

        protected override void Initialise() {
            base.Initialise();

            Patience = GetEntityQuery(typeof(CPatience));
            OriginalKitchenStatus = GetEntityQuery(ComponentType.ReadOnly<SKitchenStatus>());
            CurrentKitchenStatus = GetEntityQuery(ComponentType.ReadWrite<SKitchenStatus>());

            if (Utility.IsCustomDifficultyInstalled()) {
                DisabledByCompat = true;
            }
        }

        protected override void OnUpdate() {
            if (DisabledByCompat) {
                return;
            }

            SKitchenStatus singleton = OriginalKitchenStatus.GetSingleton<SKitchenStatus>();

            if (singleton.RemainingLives <= 0 && !Has<SPracticeMode>() && !RescuedByAppliance()) {
                ShowDayRestart();
            }
        }

        private bool RescuedByAppliance() {
            SKitchenStatus singleton = OriginalKitchenStatus.GetSingleton<SKitchenStatus>();
            EntityQuery entityQuery = GetEntityQuery(new QueryHelper().All(typeof(CPreventGameOver)).None(typeof(CPreventGameOverConsumed)));
            if (!entityQuery.IsEmpty) {
                base.EntityManager.AddComponent<CPreventGameOverConsumed>(entityQuery.First());
                CurrentKitchenStatus.SetSingleton(new SKitchenStatus {
                    RemainingLives = 1,
                    TotalLives = singleton.TotalLives
                });
                using NativeArray<Entity> nativeArray = Patience.ToEntityArray(Allocator.Temp);
                foreach (Entity item in nativeArray) {
                    if (Require<CPatience>(item, out CPatience comp)) {
                        comp.ResetTime();
                        Set(item, comp);
                    }
                }
                return true;
            }
            return false;
        }

        private void ShowDayRestart() {
            Debug.Log(typeof(AutoRestart).Name + ": Displaying restart day offer");

            SKitchenStatus singleton = OriginalKitchenStatus.GetSingleton<SKitchenStatus>();
            base.World.Add(new COfferRestartDay {
                Reason = LossReason.Patience
            });
            CurrentKitchenStatus.SetSingleton(new SKitchenStatus {
                RemainingLives = singleton.TotalLives,
                TotalLives = singleton.TotalLives
            });
        }
    }
}