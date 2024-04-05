using System;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;

namespace ButtonsExtraBooks
{
    public static class ButtonsExtraBooks_Luck
    {
        private static bool Applied { get; set; }
        private static IMonitor Monitor { get; set; }
        
        public static void Apply(Harmony harmony, IMonitor monitor)
        {
            if (Applied)
                throw new InvalidOperationException("ButtonsExtraBooks_Luck patch is already applied.");
            Monitor = monitor;
            Monitor.Log("Patching in Luck Book...", LogLevel.Trace);
            try
            {
                harmony.Patch(
                    original: AccessTools.PropertyGetter(typeof(Farmer), nameof(Farmer.DailyLuck)),
                    postfix: new HarmonyMethod(typeof(ButtonsExtraBooks_Luck), nameof(DailyLuck_Postfix))
                );
            }
            catch (Exception ex)
            {
                Monitor.Log("Error patching ButtonsExtraBooks_Luck: \n" + ex, LogLevel.Error);
            }
            Applied = true;
        }
        
        private static void DailyLuck_Postfix(Farmer __instance, ref double __result)
        {
            try
            {
                __result += __instance.stats.Get("Spiderbuttons.ButtonsExtraBooks_Book_Luck") != 0 ? 0.025f : 0f;
            }
            catch (Exception ex)
            {
                Monitor.Log("Error in ButtonsExtraBooks_Luck.DailyLuck_Postfix: \n" + ex, LogLevel.Error);
            }
        }
    }
}