using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using ButtonsExtraBooks.Config;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using ButtonsExtraBooks.Helpers;

namespace ButtonsExtraBooks.Powers
{
    [HarmonyPatch]
    static class ArtisanMachines
    {
        [HarmonyPatch(typeof(StardewValley.Object), nameof(StardewValley.Object.OutputMachine))]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            try
            {
                var code = new List<CodeInstruction>(instructions);
                
                var farmerEnumerator = il.DeclareLocal(typeof(IEnumerator<Farmer>));
                
                int insertionIndex; // This will be just after minutesUntilReady is set and before FixQuality is called
                for (insertionIndex = 0; insertionIndex < code.Count; insertionIndex++)
                {
                    if (code[insertionIndex].opcode == OpCodes.Callvirt && code[insertionIndex].operand.ToString().Contains("FixQuality"))
                    {
                        insertionIndex -= 1;
                        break;
                    }
                }
                
                // get our ArtisanManualPercentDecrease from Config
                var artisanManualPercentDecrease = (100.0f - ModEntry.Config.ArtisanManualPercentDecrease)/100.0f;
                
                var instructionsToAdd = new List<CodeInstruction>();

                var newHeldItemIndex = 1;
                var minutesUntilReadyIndex = 3;
                
                var LoopStart = il.DefineLabel();
                var MoveNext = il.DefineLabel();
                var EndFinally = il.DefineLabel();
                var EndCode = il.DefineLabel();
                code[insertionIndex].labels.Add(EndCode);

                instructionsToAdd.Add(new CodeInstruction(OpCodes.Call,
                    AccessTools.Method(typeof(Game1), nameof(Game1.getAllFarmers))));
                instructionsToAdd.Add(new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(IEnumerable<Farmer>), nameof(IEnumerable<Farmer>.GetEnumerator))));
                instructionsToAdd.Add(new CodeInstruction(OpCodes.Stloc, farmerEnumerator));
                instructionsToAdd.Add(new CodeInstruction(OpCodes.Br_S, MoveNext));
                instructionsToAdd.Add(new CodeInstruction(OpCodes.Ldloc, farmerEnumerator));
                instructionsToAdd[^1].labels.Add(LoopStart);
                instructionsToAdd.Add(new CodeInstruction(OpCodes.Callvirt,
                    AccessTools.PropertyGetter(typeof(IEnumerator<Farmer>), nameof(IEnumerator<Farmer>.Current))));
                instructionsToAdd.Add(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Farmer), nameof(Farmer.stats))));
                instructionsToAdd.Add(new CodeInstruction(OpCodes.Ldstr, "Spiderbuttons.ButtonsExtraBooks_Book_ArtisanMachines"));
                instructionsToAdd.Add(new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(Stats), nameof(Stats.Get))));
                instructionsToAdd.Add(new CodeInstruction(OpCodes.Brfalse_S, MoveNext));
                instructionsToAdd.Add(new CodeInstruction(OpCodes.Ldloc_S, newHeldItemIndex));
                instructionsToAdd.Add(new CodeInstruction(OpCodes.Callvirt,
                    AccessTools.PropertyGetter(typeof(StardewValley.Object), nameof(StardewValley.Object.Category))));
                instructionsToAdd.Add(new CodeInstruction(OpCodes.Ldc_I4, -26));
                instructionsToAdd.Add(new CodeInstruction(OpCodes.Bne_Un_S, MoveNext));
                instructionsToAdd.Add(new CodeInstruction(OpCodes.Ldloc_S, minutesUntilReadyIndex));
                instructionsToAdd.Add(new CodeInstruction(OpCodes.Conv_R4));
                instructionsToAdd.Add(new CodeInstruction(OpCodes.Ldc_R4, artisanManualPercentDecrease));
                instructionsToAdd.Add(new CodeInstruction(OpCodes.Mul));
                instructionsToAdd.Add(new CodeInstruction(OpCodes.Conv_I4));
                instructionsToAdd.Add(new CodeInstruction(OpCodes.Stloc_S, minutesUntilReadyIndex));
                instructionsToAdd.Add(new CodeInstruction(OpCodes.Leave_S, EndCode));
                instructionsToAdd.Add(new CodeInstruction(OpCodes.Ldloc, farmerEnumerator));
                instructionsToAdd[^1].labels.Add(MoveNext);
                instructionsToAdd.Add(new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(IEnumerator), nameof(IEnumerator.MoveNext))));
                instructionsToAdd.Add(new CodeInstruction(OpCodes.Brtrue_S, LoopStart));
                instructionsToAdd.Add(new CodeInstruction(OpCodes.Leave_S, EndCode));
                instructionsToAdd.Add(new CodeInstruction(OpCodes.Ldloc, farmerEnumerator));
                instructionsToAdd.Add(new CodeInstruction(OpCodes.Brfalse_S, EndFinally));
                instructionsToAdd.Add(new CodeInstruction(OpCodes.Ldloc, farmerEnumerator));
                instructionsToAdd.Add(new CodeInstruction(OpCodes.Callvirt,
                    AccessTools.Method(typeof(IDisposable), nameof(IDisposable.Dispose))));
                instructionsToAdd.Add(new CodeInstruction(OpCodes.Endfinally));
                instructionsToAdd[^1].labels.Add(EndFinally);
                
                code.InsertRange(insertionIndex, instructionsToAdd);

                return code;
            }
            catch (Exception ex)
            {
                Loggers.Log("Error in ButtonsExtraBooks_ArtisanMachines.OutputMachine_Postfix: \n" + ex, LogLevel.Error);
                return instructions;
            }
        }
    }
}