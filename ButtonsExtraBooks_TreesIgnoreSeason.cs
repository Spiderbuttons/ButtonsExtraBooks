using System;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.WildTrees;
using StardewValley.TerrainFeatures;

namespace ButtonsExtraBooks
{
    public static class ButtonsExtraBooks_TreesIgnoreSeason
    {
        private static bool Applied { get; set; }
        private static IMonitor Monitor { get; set; }
        
        public static void Apply(Harmony harmony, IMonitor monitor)
        {
            if (Applied)
                throw new InvalidOperationException("ButtonsExtraBooks_TreesIgnoreSeason patch is already applied.");
            Monitor = monitor;
            Monitor.Log("Patching in Trees Ignore Season Book...", LogLevel.Trace);
            try
            {
                harmony.Patch(
                    original: AccessTools.Method(typeof(Tree), nameof(Tree.GetMaxSizeHere)),
                    prefix: new HarmonyMethod(typeof(ButtonsExtraBooks_TreesIgnoreSeason), nameof(GetMaxSizeHere_Prefix))
                );
                harmony.Patch(
                    original: AccessTools.Method(typeof(Tree), nameof(Tree.IsInSeason)),
                    postfix: new HarmonyMethod(typeof(ButtonsExtraBooks_TreesIgnoreSeason), nameof(IsInSeason_Postfix))
                );
            }
            catch (Exception ex)
            {
                Monitor.Log("Error patching ButtonsExtraBooks_TreesIgnoreSeason: \n" + ex, LogLevel.Error);
            }
            Applied = true;
        }
        
        private static void GetMaxSizeHere_Prefix(ref bool ignoreSeason)
        {
            try
            {
                foreach (Farmer farmer in Game1.getAllFarmers())
                {
                    if (farmer.stats.Get("Spiderbuttons.ButtonsExtraBooks_Book_TreesIgnoreSeason") != 0)
                    {
                        ignoreSeason = true;
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Monitor.Log("Error in ButtonsExtraBooks_TreesIgnoreSeason.GetMaxSizeHere_Prefix: \n" + ex, LogLevel.Error);
            }
        }
        
        private static void IsInSeason_Postfix(ref bool __result)
        {
            try
            {
                foreach (Farmer farmer in Game1.getAllFarmers())
                {
                    if (farmer.stats.Get("Spiderbuttons.ButtonsExtraBooks_Book_TreesIgnoreSeason") != 0)
                    {
                        __result = true;
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Monitor.Log("Error in ButtonsExtraBooks_TreesIgnoreSeason.IsInSeason_Postfix: \n" + ex, LogLevel.Error);
            }
        }
    }
}