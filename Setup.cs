using Kitchen;
using KitchenMods;
using UnityEngine;

namespace AutoRestart {
    internal class Setup : RestaurantInitialisationSystem, IModSystem {
		protected override void Initialise() {
			base.Initialise();

			if (Utility.IsCustomDifficultyInstalled()) {
				Debug.Log(typeof(AutoRestart).Name + ": Custom Difficulty is present, disabling");

				return;
			}

			Debug.Log(typeof(AutoRestart).Name + ": Disabling CheckGameOverFromLife");
			World.GetExistingSystem(typeof(CheckGameOverFromLife)).Enabled = false;
			Debug.Log(typeof(AutoRestart).Name + ": Disabled CheckGameOverFromLife");
		}

		protected override void OnUpdate() {
		}
	}
}
