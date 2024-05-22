using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using ButtonsExtraBooks.Helpers;
using HarmonyLib;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using StardewModdingAPI;
using StardewValley;
using Object = StardewValley.Object;

namespace ButtonsExtraBooks.Patches
{
    [HarmonyPatch]
    static class ArtisanGrangeDisplay
    {
        [HarmonyPatch(typeof(Event), "judgeGrange")]
        static void Postfix(Event __instance)
        {
            try
            {
                int artisanCount = 0;
                foreach (Item i in Game1.player.team.grangeDisplay)
                {
                    if (i is not Object obj) continue;
                    if (obj.Category == -26) artisanCount++;
                }
                if ((ModEntry.Config.ArtisanMachinesGrangeMustWin && __instance.grangeScore < 90) || artisanCount < 6) return;
                Game1.addMailForTomorrow("Spiderbuttons.ButtonsExtraBooks_Mail_ArtisanGrange", false, true);
            }
            catch (Exception ex)
            {
                Log.Error("Error in ButtonsExtraBooks_ArtisanGrangeDisplay.judgeGrange_Postfix: \n" + ex);
            }
        }
    }
}