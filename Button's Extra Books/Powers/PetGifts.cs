using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using StardewValley;
using ButtonsExtraBooks.Helpers;
using StardewValley.Characters;
using System.Reflection;

namespace ButtonsExtraBooks.Powers
{
    [HarmonyPatch]
    static class PetGifts
    {
        [HarmonyPatch(typeof(Pet), nameof(Pet.TryGetGiftItem))]
        private static IEnumerable<CodeInstruction> Transpiler(this IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            var code = instructions.ToList();
            try
            {
                return code.MethodReplacer(RuntimeReflectionExtensions.GetMethodInfo(Utility.RandomFloat), RuntimeReflectionExtensions.GetMethodInfo(AdjustedFloat));
            }
            catch (Exception ex)
            {
                Log.Error("Error in ButtonsExtraBooks_PetGifts.TryGetGiftItem_Transpiler: \n" + ex);
                return code;
            }
        }

        private static float AdjustedFloat(float min, float max, Random random = null)
        {
            return Utility.RandomFloat(Utils.AnyoneHasPower("PetGifts") ? Utility.RandomFloat(min, max) : min, max);
        }
    }
}