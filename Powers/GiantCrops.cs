using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using StardewValley;
using StardewModdingAPI;
using ButtonsExtraBooks.Helpers;
using StardewValley.GameData.GiantCrops;

namespace ButtonsExtraBooks.Powers
{
    [HarmonyPatch]
    static class GiantCrops
    {
        [HarmonyPatch(typeof(Crop), nameof(Crop.TryGetGiantCrops))]
        static void Postfix(ref IReadOnlyList<KeyValuePair<string, GiantCropData>> giantCrops)
        {
            try
            {
                bool someoneHasBuff = Game1.getAllFarmers().Any(farmer => farmer.stats.Get("Spiderbuttons.ButtonsExtraBooks_Book_GiantCrops") != 0);
                foreach (var crop in giantCrops)
                {
                    crop.Value.Chance = someoneHasBuff ? ModEntry.Config.BigMelonsPercent/100f : 0.01f;
                }
            }
            catch (Exception ex)
            {
                Loggers.Log("Error in ButtonsExtraBooks_Luck.GiantCrops_Postfix: \n" + ex, LogLevel.Error);
            }
        }
    }
}