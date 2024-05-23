using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using ButtonsExtraBooks.Helpers;
using Microsoft.Xna.Framework;
using StardewValley.Minigames;
using StardewValley.Monsters;

namespace ButtonsExtraBooks.Powers
{
    [HarmonyPatch]
    static class CheatCodes
    {
        [HarmonyPatch(typeof(MineCart), "restartLevel")]
        private static void Postfix(MineCart __instance, ref bool new_game, ref int ___livesLeft, ref HashSet<MineCart.CollectableFruits> ____collectedFruit, ref int ___score, ref int ___levelsBeat)
        {
            if (!ModEntry.Config.EnableCheatCodes || Game1.player.stats.Get("Spiderbuttons.ButtonsExtraBooks_Book_CheatCodes") == 0) return;
            try
            {
                if (new_game)
                {
                    ___livesLeft = 3 + ModEntry.Config.CheatCodesLives;
                    ____collectedFruit.Clear();
                    ___score = 0;
                    ___levelsBeat = 0;
                    __instance.coinCount = 0;
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error in ButtonsExtraBooks_CheatCodes.restartLevel_Postfix: \n" + ex);
            }
        }
        
        [HarmonyPatch(typeof(MineCart), nameof(MineCart.Die))]
        private static void Postfix()
        {
            if (!ModEntry.Config.EnableCheatCodes) return;
            try
            {
                Game1.player.stats.Increment("Spiderbuttons.ButtonsExtraBooks_JunimoDeaths");
            }
            catch (Exception ex)
            {
                Log.Error("Error in ButtonsExtraBooks_CheatCodes.Die_Postfix: \n" + ex);
            }
        }
    }
}