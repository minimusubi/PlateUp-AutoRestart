using KitchenMods;

namespace AutoRestart {
    internal class Utility {
        internal static bool IsCustomDifficultyInstalled() {
            return ModPreload.Mods.Exists(mod => mod.Name == "Custom Difficulty");
        }
    }
}
