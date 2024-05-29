using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using ButtonsExtraBooks.Helpers;

namespace ButtonsExtraBooks.Powers
{
    [HarmonyPatch]
    static class ExtraGifts
    {
        public static int GetGiftLimit()
        {
            return Utils.PlayerHasPower("ExtraGifts") ? ModEntry.Config.ExtraGiftsBonus : 2;
        }
        
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(NPC), nameof(NPC.tryToReceiveActiveObject))]
        static IEnumerable<CodeInstruction> tryToReceiveActiveObject_Transpiler(IEnumerable<CodeInstruction> instructions,
            ILGenerator il)
        {
            if (!ModEntry.Config.EnableExtraGifts) return instructions;
            var code = instructions.ToList();
            try
            {
                var matcher = new CodeMatcher(code, il);
                matcher.MatchEndForward(
                        new CodeMatch(OpCodes.Ldloc_1),
                        new CodeMatch(OpCodes.Call, AccessTools.PropertyGetter(typeof(Friendship), nameof(Friendship.GiftsThisWeek))),
                        new CodeMatch(OpCodes.Ldc_I4_2))
                    .ThrowIfNotMatch("Could not find proper entry point for tryToReceiveActiveObject_Transpiler");

                matcher.Set(OpCodes.Call, AccessTools.Method(typeof(ExtraGifts), nameof(ExtraGifts.GetGiftLimit)));

                return matcher.InstructionEnumeration();
            }
            catch (Exception ex)
            {
                Log.Error("Error in ButtonsExtraBooks_ExtraGifts.tryToReceiveActiveObject_Transpiler: \n" + ex);
                return code;
            }
        }
    }
}