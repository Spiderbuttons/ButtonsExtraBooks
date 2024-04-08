using System;
using System.Linq;
using HarmonyLib;
using StardewValley;
using StardewModdingAPI;
using StardewValley.TerrainFeatures;
using ButtonsExtraBooks.Helpers;

namespace ButtonsExtraBooks.Powers
{
    [HarmonyPatch]
    static class TreesIgnoreSeason
    {
        [HarmonyPatch(typeof(Tree), nameof(Tree.GetMaxSizeHere))]
        static void Prefix(ref bool ignoreSeason)
        {
            if (!ModEntry.Config.EnableTreesIgnoreSeason) return;
            try
            {
                if (Game1.getAllFarmers().All(farmer => farmer.stats.Get("Spiderbuttons.ButtonsExtraBooks_Book_TreesIgnoreSeason") == 0)) return;
                ignoreSeason = true;
                return;
            }
            catch (Exception ex)
            {
                Loggers.Log("Error in ButtonsExtraBooks_TreesIgnoreSeason.GetMaxSizeHere_Prefix: \n" + ex, LogLevel.Error);
            }
        }
        
        [HarmonyPatch(typeof(Tree), nameof(Tree.IsInSeason))]
        private static void Postfix(ref bool __result)
        {
            if (!ModEntry.Config.EnableTreesIgnoreSeason) return;
            try
            {
                foreach (Farmer farmer in Game1.getAllFarmers())
                {
                    if (farmer.stats.Get("Spiderbuttons.ButtonsExtraBooks_Book_TreesIgnoreSeason") == 0) continue;
                    __result = true;
                    return;
                }
            }
            catch (Exception ex)
            {
                Loggers.Log("Error in ButtonsExtraBooks_TreesIgnoreSeason.IsInSeason_Postfix: \n" + ex, LogLevel.Error);
            }
        }
    }
}