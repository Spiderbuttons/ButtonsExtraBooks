using System;
using HarmonyLib;
using StardewValley;
using StardewModdingAPI;
using ButtonsExtraBooks.Helpers;
using StardewValley.Extensions;

namespace ButtonsExtraBooks.Powers
{
    [HarmonyPatch]
    static class Popularity
    {
        [HarmonyPatch(typeof(Game1), nameof(Game1.newDayAfterFade))]
        static void Postfix()
        {
            if (!ModEntry.Config.EnablePopularity) return;
            try
            {
                if (Game1.player.stats.Get("Spiderbuttons.ButtonsExtraBooks_Book_Popularity") == 0) return;
                if (Utility.TryGetRandom(Game1.player.friendshipData, out var whichFriend, out var friendship) && Game1.random.NextBool((double)(friendship.Points / 250) * 0.1) && Game1.player.spouse != whichFriend && DataLoader.Mail(Game1.content).ContainsKey(whichFriend))
                {
                    Game1.mailbox.Add(whichFriend);
                }
            }
            catch (Exception ex)
            {
                Loggers.Log("Error in ButtonsExtraBooks_Luck.DailyLuck_Postfix: \n" + ex, LogLevel.Error);
            }
        }
    }
}