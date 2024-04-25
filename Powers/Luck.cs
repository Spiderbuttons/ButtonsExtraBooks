using System;
using HarmonyLib;
using StardewValley;
using StardewModdingAPI;
using ButtonsExtraBooks.Helpers;

namespace ButtonsExtraBooks.Powers
{
    [HarmonyPatch]
    static class Luck
    {
        [HarmonyPatch(typeof(Farmer), nameof(Farmer.DailyLuck), MethodType.Getter)]
        static void Postfix(Farmer __instance, ref double __result)
        {
            if (!ModEntry.Config.EnableLuck) return;
            try
            {
                if (__instance.stats.Get("Spiderbuttons.ButtonsExtraBooks_Book_Luck") == 0) return;
                double luckToAdd = ModEntry.Config.LuckAmount;
                while (luckToAdd > 0 && __result + luckToAdd > 0.15)
                {
                    luckToAdd -= 0.01;
                }
                __result += luckToAdd;
            }
            catch (Exception ex)
            {
                Loggers.Log("Error in ButtonsExtraBooks_Luck.DailyLuck_Postfix: \n" + ex, LogLevel.Error);
            }
        }
    }
}