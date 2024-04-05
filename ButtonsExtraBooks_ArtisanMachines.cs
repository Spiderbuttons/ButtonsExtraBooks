using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData;

namespace ButtonsExtraBooks
{
    public static class ButtonsExtraBooks_ArtisanMachines
    {
        private static bool Applied { get; set; }
        private static IMonitor Monitor { get; set; }
        
        public static void Apply(Harmony harmony, IMonitor monitor)
        {
            if (Applied)
                throw new InvalidOperationException("ButtonsExtraBooks_ArtisanMachines patch is already applied.");
            Monitor = monitor;
            Monitor.Log("Patching in ArtisanMachines Book...", LogLevel.Trace);
            try
            {
                harmony.Patch(
                    original: AccessTools.Method(typeof(Event), "judgeGrange"),
                    transpiler: new HarmonyMethod(typeof(ButtonsExtraBooks_ArtisanMachines), nameof(judgeGrange_Transpiler))
                );
                // Now patch the "ApplyQuantityModifiers" method in Utility to actually apply the buff. it should be a prefix
                harmony.Patch(
                    original: AccessTools.Method(typeof(Utility), nameof(Utility.ApplyQuantityModifiers)),
                    prefix: new HarmonyMethod(typeof(ButtonsExtraBooks_ArtisanMachines), nameof(ApplyQuantityModifiers_Prefix))
                );
            }
            catch (Exception ex)
            {
                Monitor.Log("Error patching ButtonsExtraBooks_ArtisanMachines: \n" + ex, LogLevel.Error);
            }
            Applied = true;
        }
        
        private static void ApplyQuantityModifiers_Prefix(ref IList<QuantityModifier> modifiers, ref Item targetItem)
        {
            try
            {
                if (Game1.getAllFarmers().Any(farmer => farmer.stats.Get("Spiderbuttons.ButtonsExtraBooks_Book_ArtisanMachines") != 0) && targetItem.Category == -26)
                {
                    Monitor.Log("Applying ArtisanMachines buff to " + targetItem.Name, LogLevel.Trace);
                    QuantityModifier artisanBuff = new QuantityModifier
                    {
                        Id = "Spiderbuttons.ButtonsExtraBooks_ArtisanMachines",
                        Modification = QuantityModifier.ModificationType.Multiply,
                        Amount = 0.75f
                    };
                    if (modifiers == null) modifiers = new List<QuantityModifier>();
                    modifiers.Add(artisanBuff);
                }
            }
            catch (Exception ex)
            {
                Monitor.Log("Error in ButtonsExtraBooks_ArtisanMachines.ApplyQuantityModifiers_Prefix: \n" + ex, LogLevel.Error);
            }
        }

        private static IEnumerable<CodeInstruction> judgeGrange_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            try
            {
                var code = new List<CodeInstruction>(instructions);
                
                // create new localVariable called artisanPoints to add to the original method
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
                Monitor.Log("Error in ButtonsExtraBooks_Artisan.judgeGrange_Transpiler: \n" + ex, LogLevel.Error);
                return instructions;
            }
        }
    }
}