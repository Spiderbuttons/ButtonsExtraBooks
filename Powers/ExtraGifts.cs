using System;
using System.Linq;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using ButtonsExtraBooks.Helpers;

namespace ButtonsExtraBooks.Powers
{
    [HarmonyPatch]
    static class ExtraGifts
    {
        [HarmonyPatch(typeof(Farmer), nameof(Farmer.updateFriendshipGifts))]
        private static void Postfix(Farmer __instance)
        {
            if (!ModEntry.Config.EnableExtraGifts) return;
            try
            {
                if (__instance.stats.Get("Spiderbuttons.ButtonsExtraBooks_Book_ExtraGifts") == 0) return;
                foreach (var name in __instance.friendshipData.Keys.Where(name => __instance.friendshipData[name].GiftsThisWeek == 2))
                {
                    __instance.friendshipData[name].GiftsThisWeek--;
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error in ButtonsExtraBooks_ExtraGifts.updateFriendshipGifts_Prefix: \n" + ex);
            }
        }
    }
}