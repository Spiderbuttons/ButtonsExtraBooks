using System;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.WildTrees;
using StardewValley.TerrainFeatures;

namespace ButtonsExtraBooks
{
    public static class ButtonsExtraBooks_ExtraGifts
    {
        private static bool Applied { get; set; }
        private static IMonitor Monitor { get; set; }
        
        public static void Apply(Harmony harmony, IMonitor monitor)
        {
            if (Applied)
                throw new InvalidOperationException("ButtonsExtraBooks_ExtraGifts patch is already applied.");
            Monitor = monitor;
            Monitor.Log("Patching in Extra Gifts Book...", LogLevel.Trace);
            try
            {
                // harmony.Patch(
                //     original: AccessTools.Method(typeof(Farmer), nameof(Farmer.updateFriendshipGifts)),
                //     postfix: new HarmonyMethod(typeof(ButtonsExtraBooks_ExtraGift), nameof(updateFriendshipGifts_Postfix))
                // );
                harmony.Patch(
                    original: AccessTools.Method(typeof(Farmer), nameof(Farmer.updateFriendshipGifts)),
                    postfix: new HarmonyMethod(typeof(ButtonsExtraBooks_ExtraGifts), nameof(updateFriendshipGifts_Postfix))
                );
            }
            catch (Exception ex)
            {
                Monitor.Log("Error patching ButtonsExtraBooks_ExtraGifts: \n" + ex, LogLevel.Error);
            }
            Applied = true;
        }
        
        // This allows infinite gifts per week (but still only one per day).
        private static void updateFriendshipGifts_Postfix(Farmer __instance)
        {
            try
            {
                if (__instance.stats.Get("Spiderbuttons.ButtonsExtraBooks_Book_ExtraGifts") != 0)
                {
                    foreach (string name in __instance.friendshipData.Keys)
                    {
                        if (__instance.friendshipData[name].GiftsThisWeek == 2) __instance.friendshipData[name].GiftsThisWeek--;
                    }
                }
            }
            catch (Exception ex)
            {
                Monitor.Log("Error in ButtonsExtraBooks_ExtraGifts.updateFriendshipGifts_Prefix: \n" + ex, LogLevel.Error);
            }
        }
        
        // This allows for one extra gift per week.
        // private static void updateFriendshipGifts_Postfix(Farmer __instance, WorldDate date)
        // {
        //     try
        //     {
        //         if (__instance.stats.Get("ButtonsExtraBooks_ExtraGifts") == 0) return;
        //         foreach (string name in __instance.friendshipData.Keys)
        //         {
        //             if (__instance.friendshipData[name].LastGiftDate == null || date.TotalSundayWeeks != __instance.friendshipData[name].LastGiftDate.TotalSundayWeeks)
        //             {
        //                 __instance.friendshipData[name].GiftsThisWeek--;
        //             }
        //             
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         Monitor.Log("Error in ButtonsExtraBooks_TreesIgnoreSeason.GetMaxSizeHere_Prefix: \n" + ex, LogLevel.Error);
        //     }
        // }
    }
}