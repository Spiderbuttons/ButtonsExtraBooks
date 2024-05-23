using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using StardewValley;
using StardewModdingAPI;
using StardewValley.TerrainFeatures;
using ButtonsExtraBooks.Helpers;
using Force.DeepCloner;
using StardewValley.GameData.WildTrees;

namespace ButtonsExtraBooks.Powers
{
    [HarmonyPatch]
    static class TreesIgnoreSeason
    {
        [HarmonyPatch(typeof(Tree), nameof(Tree.GetData))]
        private static void Postfix(Tree __instance, ref WildTreeData __result)
        {
            if (!ModEntry.Config.EnableTreesIgnoreSeason || __result == null) return;
            try
            {
                if (!Utils.AnyoneHasPower("TreesIgnoreSeason"))
                {
                    if (!__instance.modData.ContainsKey("Spiderbuttons.ButtonsExtraBooks_Book_TreesIgnoreSeason"))
                        return;
                    __instance.modData.Remove("Spiderbuttons.ButtonsExtraBooks_Book_TreesIgnoreSeason");
                    ModEntry.ModHelper.Reflection.GetMethod(typeof(Tree), "ClearCache").Invoke();
                }
                else
                {
                    __result.GrowsInWinter = true;
                    __instance.modData["Spiderbuttons.ButtonsExtraBooks_Book_TreesIgnoreSeason"] = "1";
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error in ButtonsExtraBooks_TreesIgnoreSeason.GetData_Postfix: \n" + ex);
            }
        }
    }
}