using System;
using System.Globalization;
using System.Linq;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using ButtonsExtraBooks.Helpers;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;

namespace ButtonsExtraBooks.Powers
{
    [HarmonyPatch]
    static class QiNotebook
    {
        [HarmonyPatch(typeof(GameLocation), nameof(GameLocation.monsterDrop))]
        private static void Postfix(GameLocation __instance, Monster monster, int x, int y, Farmer who)
        {
            if (!ModEntry.Config.EnableQiNotebook || !Utils.PlayerHasPower(who, "QiNotebook")) return;
            try
            {
                
                if (Game1.netWorldState.Value.GoldenWalnutsFound >= 100)
                {
                    if (Game1.random.NextDouble() < (ModEntry.Config.QiNotebookPercent / 1000) + (double)((float)who.LuckLevel * 0.002f))
                    {
                        monster.ModifyMonsterLoot(Game1.createItemDebris(ItemRegistry.Create("(O)858"), new Vector2(x, y), -1, __instance));
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error in ButtonsExtraBooks_QiNotebook.monsterDrop_Postfix: \n" + ex);
            }
        }
        
        // TODO.... someday.
        // BreakableContainer.releaseContents(Farmer who) 
        // BigSlime public BigSlime(Vector2 position, int mineArea)
    }
}