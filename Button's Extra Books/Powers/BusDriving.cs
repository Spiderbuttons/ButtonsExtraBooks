using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using ButtonsExtraBooks.Config;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using ButtonsExtraBooks.Helpers;
using Netcode;
using StardewValley.Locations;
using StardewValley.Network;

namespace ButtonsExtraBooks.Powers
{
    [HarmonyPatch]
    static class BusDriving
    {
        public static bool CanDriveBus()
        {
            return Utils.PlayerHasPower("BusDriving");
        }
        
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(BusStop), nameof(BusStop.draw))]
        private static IEnumerable<CodeInstruction> BusStop_DrawTranspiler(IEnumerable<CodeInstruction> instructions,
            ILGenerator il)
        {
            if (!ModEntry.Config.EnableBusDriving) return instructions;
            try
            {
                var code = new List<CodeInstruction>(instructions);
                
                int insertionIndex;
                for (insertionIndex = 0; insertionIndex < code.Count; insertionIndex++)
                {
                    if (code[insertionIndex].opcode == OpCodes.Ldsfld && code[insertionIndex].operand.ToString()!.Contains("netWorldState"))
                    {
                        break;
                    }
                }
                
                int bgtIndex;
                var bgtLabel = il.DefineLabel();
                for (bgtIndex = insertionIndex; bgtIndex < code.Count; bgtIndex++)
                {
                    if (code[bgtIndex].opcode == OpCodes.Ldarg_0)
                    {
                        code[bgtIndex].labels.Add(bgtLabel);
                        break;
                    }
                }
                
                var instructionsToAdd = new List<CodeInstruction>();

                instructionsToAdd.Add(new CodeInstruction(OpCodes.Call, AccessTools.PropertyGetter(typeof(Game1), nameof(Game1.player))));
                instructionsToAdd.Add(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Farmer), nameof(Farmer.stats))));
                instructionsToAdd.Add(new CodeInstruction(OpCodes.Ldstr, "Spiderbuttons.ButtonsExtraBooks_Book_BusDriving"));
                instructionsToAdd.Add(new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(Stats), nameof(Stats.Get))));
                instructionsToAdd.Add(new CodeInstruction(OpCodes.Ldc_I4_0));
                instructionsToAdd.Add(new CodeInstruction(OpCodes.Bgt_Un_S, bgtLabel));
                
                code.InsertRange(insertionIndex, instructionsToAdd);

                return code;
            }
            catch (Exception ex)
            {
                Log.Error("Error in ButtonsExtraBooks_BusDriving.draw_Transpiler: \n" + ex);
                return instructions;
            }
        }
        
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(Desert), nameof(Desert.draw))]
        private static IEnumerable<CodeInstruction> Desert_DrawTranspiler(IEnumerable<CodeInstruction> instructions,
            ILGenerator il)
        {
            if (!ModEntry.Config.EnableBusDriving) return instructions;
            var code = instructions.ToList();
            try
            {
                var matcher = new CodeMatcher(code, il);
                matcher.MatchEndForward(
                    new CodeMatch(OpCodes.Ldarg_0),
                    new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(Desert), nameof(Desert.drivingOff))),
                    new CodeMatch(OpCodes.Brfalse),
                    new CodeMatch(OpCodes.Ldsfld, AccessTools.Field(typeof(Desert), nameof(Desert.warpedToDesert))),
                    new CodeMatch(OpCodes.Brfalse))
                    .ThrowIfNotMatch("Could not find proper entry point for Desert_DrawTranspiler");

                matcher.CreateLabelAt(matcher.Pos + 1, out var label);
                matcher.Advance(-4);
                
                matcher.Insert(
                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(BusDriving), nameof(BusDriving.CanDriveBus))),
                    new CodeInstruction(OpCodes.Brtrue_S, label)
                );

                return matcher.InstructionEnumeration();
            }
            catch (Exception ex)
            {
                Log.Error("Error in ButtonsExtraBooks_BusDriving.Desert_DrawTranspiler: \n" + ex);
                return code;
            }
        }

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(BusStop), nameof(BusStop.answerDialogue))]
        private static IEnumerable<CodeInstruction> ticketTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            if (!ModEntry.Config.EnableBusDriving) return instructions;
            try
            {
                var code = new List<CodeInstruction>(instructions);
                
                int insertionIndex;
                for (insertionIndex = 0; insertionIndex < code.Count; insertionIndex++)
                {
                    if (code[insertionIndex].opcode == OpCodes.Ldsfld && code[insertionIndex].operand.ToString()!.Contains("netWorldState"))
                    {
                        break;
                    }
                }
                
                int brTrueIndex;
                var brTrueLabel = il.DefineLabel();
                for (brTrueIndex = insertionIndex; brTrueIndex < code.Count; brTrueIndex++)
                {
                    if (code[brTrueIndex].opcode == OpCodes.Brtrue_S)
                    {
                        brTrueLabel = (Label) code[brTrueIndex].operand;
                        break;
                    }
                }
                
                var instructionsToAdd = new List<CodeInstruction>();

                instructionsToAdd.Add(new CodeInstruction(OpCodes.Call, AccessTools.PropertyGetter(typeof(Game1), nameof(Game1.player))));
                instructionsToAdd.Add(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Farmer), nameof(Farmer.stats))));
                instructionsToAdd.Add(new CodeInstruction(OpCodes.Ldstr, "Spiderbuttons.ButtonsExtraBooks_Book_BusDriving"));
                instructionsToAdd.Add(new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(Stats), nameof(Stats.Get))));
                instructionsToAdd.Add(new CodeInstruction(OpCodes.Brtrue_S, brTrueLabel));
                
                code.InsertRange(insertionIndex, instructionsToAdd);

                return code;
            }
            catch (Exception ex)
            {
                Log.Error("Error in ButtonsExtraBooks_BusDriving.answerDialogue_Transpiler: \n" + ex);
                return instructions;
            }
        }
    }
}