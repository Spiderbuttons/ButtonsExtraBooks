using System;
using HarmonyLib;
using StardewValley;
using StardewModdingAPI;
using ButtonsExtraBooks.Helpers;
using StardewValley.Extensions;

namespace ButtonsExtraBooks.Powers
{
    [HarmonyPatch]
    static class Optimization
    {
        [HarmonyPatch(typeof(Farmer), nameof(Farmer.gainExperience))]
        static void Postfix(ref int howMuch)
        {
            if (!ModEntry.Config.EnableOptimization || !Utils.PlayerHasPower("Optimization")) return;
            try
            {
                Log.Debug($"Before: {howMuch}");
                int pct = ModEntry.Config.OptimizationPercent;
                if (pct <= 0 || pct >= 100) return;
                howMuch = (int)Math.Ceiling(howMuch * (1 + pct / 100f));
                Log.Debug($"After: {howMuch}");
            }
            catch (Exception ex)
            {
                Log.Error("Error in ButtonsExtraBooks_Optimization.gainExperience_Postfix: \n" + ex);
            }
        }
    }
}