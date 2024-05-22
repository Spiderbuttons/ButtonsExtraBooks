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
        static Dictionary<string, float> giantCropReserve = new();
        
        [HarmonyPatch(typeof(Crop), nameof(Crop.TryGetGiantCrops))]
        static void Postfix(ref IReadOnlyList<KeyValuePair<string, GiantCropData>> giantCrops)
        {
            if (!ModEntry.Config.EnableGiantCrops) return;
            if (giantCropReserve.Count == 0)
            {
                foreach (var crop in giantCrops)
                {
                    giantCropReserve.Add(crop.Key, crop.Value.Chance);
                }
            }
            try
            {
                foreach (var crop in giantCrops)
                {
                    float oldChance = giantCropReserve.GetValueOrDefault(crop.Key, 0.01f);
                    crop.Value.Chance = Utils.AnyoneHasPower("GiantCrops")
                        ? ModEntry.Config.GiantCropsPercent / 100f
                        : oldChance;
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error in ButtonsExtraBooks_Luck.GiantCrops_Postfix: \n" + ex);
            }
        }
    }
}