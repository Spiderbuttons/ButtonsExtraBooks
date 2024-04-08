using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using ButtonsExtraBooks.Helpers;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;

namespace ButtonsExtraBooks.Patches
{
    [HarmonyPatch]
    static class ArtisanGrangeDisplay
    {
        [HarmonyPatch(typeof(Event), "judgeGrange")]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            try
            {
                var code = new List<CodeInstruction>(instructions);
                
                var artisanPoints = il.DeclareLocal(typeof(int));
                code.Insert(0, new CodeInstruction(OpCodes.Ldc_I4_0));
                code.Insert(1, new CodeInstruction(OpCodes.Stloc, artisanPoints));

                int artisanCheckIndex;
                bool foundCategoryCheck = false;
                int direction = -1;
                for (artisanCheckIndex = code.Count - 1; artisanCheckIndex >= 0; artisanCheckIndex += direction)
                {
                    if (code[artisanCheckIndex].opcode == OpCodes.Ldc_I4_S && (sbyte)code[artisanCheckIndex].operand == -26)
                    {
                        direction = 1;
                        foundCategoryCheck = true;
                    }
                    if (foundCategoryCheck && code[artisanCheckIndex].opcode == OpCodes.Callvirt)
                    {
                        artisanCheckIndex++;
                        break;
                    }
                }

                var instructionsToAdd = new List<CodeInstruction>();
                
                instructionsToAdd.Add(new CodeInstruction(OpCodes.Ldloc, artisanPoints));
                instructionsToAdd.Add(new CodeInstruction(OpCodes.Ldc_I4_1));
                instructionsToAdd.Add(new CodeInstruction(OpCodes.Add));
                instructionsToAdd.Add(new CodeInstruction(OpCodes.Stloc, artisanPoints));
                
                code.InsertRange(artisanCheckIndex, instructionsToAdd);
                instructionsToAdd.Clear();
                
                // add label to the final instruction since that's our return
                var returnLabel = il.DefineLabel();
                code[^1].labels.Add(returnLabel);
                
                var ifLabel = il.DefineLabel();
                instructionsToAdd.Add(new CodeInstruction(OpCodes.Ldloc, artisanPoints));
                instructionsToAdd[^1].labels.Add(ifLabel);
                instructionsToAdd.Add(new CodeInstruction(OpCodes.Ldc_I4_6));
                instructionsToAdd.Add(new CodeInstruction(OpCodes.Blt, returnLabel));
                instructionsToAdd.Add(new CodeInstruction(OpCodes.Ldstr, "Spiderbuttons.ButtonsExtraBooks_Mail_ArtisanGrange"));
                instructionsToAdd.Add(new CodeInstruction(OpCodes.Ldc_I4_0));
                instructionsToAdd.Add(new CodeInstruction(OpCodes.Ldc_I4_1));
                instructionsToAdd.Add(new CodeInstruction(OpCodes.Call,
                    AccessTools.Method(typeof(Game1), nameof(Game1.addMailForTomorrow))));
                
                // insert those instructions before the return statement which is last in our list
                code.InsertRange(code.Count - 1, instructionsToAdd);
                
                // searching backwards through our code list, find the first brfalse.s instruction and change the operand to our ifLabel
                for (var branchIndex = code.Count - 1; branchIndex >= 0; branchIndex--)
                {
                    if (code[branchIndex].opcode != OpCodes.Brfalse_S) continue;
                    code[branchIndex].operand = ifLabel;
                    break;
                }
                
                return code;
            }
            catch (Exception ex)
            {
                Loggers.Log("Error in ButtonsExtraBooks_Artisan.judgeGrange_Transpiler: \n" + ex, LogLevel.Error);
                return instructions;
            }
        }
    }
}