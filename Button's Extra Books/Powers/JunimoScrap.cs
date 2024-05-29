using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using ButtonsExtraBooks.Config;
using HarmonyLib;
using StardewValley;
using StardewModdingAPI;
using ButtonsExtraBooks.Helpers;
using StardewModdingAPI.Events;
using StardewValley.Buildings;
using StardewValley.Characters;
using StardewValley.Extensions;
using StardewValley.GameData.Objects;
using StardewValley.Menus;
using StardewValley.TerrainFeatures;
using StardewValley.TokenizableStrings;

namespace ButtonsExtraBooks.Powers
{
    [HarmonyPatch]
    static class JunimoScrap
    {
        private static readonly Dictionary<Season, List<string>> seasonalCrops = new();
        
        public static void OnDayEnding(object sender, DayEndingEventArgs e)
        {
            if (!ModEntry.Config.EnableJunimoScrap || !Utils.PlayerHasPower("JunimoScrap")) return;
            Random rng = new Random();
            if (rng.Next(20) != 0) return;
            List<string> letterOptions = ["Crop", "Gem"];
            if (ModEntry.Config.JunimoRandomItems) letterOptions.Add("Item");
            Game1.mailbox.Add($"Spiderbuttons.ButtonsExtraBooks_Mail_JunimoScrap_{rng.ChooseFrom(letterOptions)}");
        }
        
        public static void showJunimoText(JunimoHarvester junimo, string text, int delay = 0)
        {
            if (ModEntry.Config.JunimoChatter == 0) return;
            if (!Utils.PlayerHasPower("JunimoScrap") || new Random().Next(100 - ModEntry.Config.JunimoChatter) != 0) return;
            // string currentlyShownText = ModEntry.ModHelper.Reflection
            //     .GetField<string>(junimo, "textAboveHead").GetValue();
            //if (currentlyShownText != null) return;
            junimo.showTextAboveHead(text, null, 2, 3000, delay);
        }
        
        public static Item randomCropInSeason()
        {
            if (seasonalCrops.Count == 0)
            {
                foreach (var crop in Game1.cropData)
                {
                    foreach (var season in crop.Value.Seasons)
                    {
                        if (!seasonalCrops.ContainsKey(season)) seasonalCrops[season] = new List<string>();
                        seasonalCrops[season].Add(crop.Value.HarvestItemId);
                    }
                }
            }
            
            if (seasonalCrops.TryGetValue(Game1.season, out var crops))
            {
                string cropId = crops[new Random().Next(crops.Count)];
                Item randomCrop = ItemRegistry.Create(cropId);
                randomCrop.Quality = new Random().Choose(1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 4);
                randomCrop.Stack = new Random().Next(3, 8);
                return randomCrop;
            }
            return null;
        }

        public static Item randomGem()
        {
            List<string> gems = Game1.objectData.Keys
                .Where(key => Game1.objectData[key].Category == -2)
                .ToList();
            string gemId = gems[new Random().Next(gems.Count)];
            Item randomGem = ItemRegistry.Create(gemId);
            randomGem.Stack = new Random().Next(1, 4);
            return randomGem;
        }

        // This will likely give some strange items sometimes that you're not supposed to be able to get. That's why it's fun! (And locked behind an opt-in config setting.)
        public static Item randomItem()
        {
            List<string> items = Game1.objectData.Keys.ToList();
            string itemId = items[new Random().Next(items.Count)];
            Item randomItem = ItemRegistry.Create(itemId);
            return randomItem;
        }

        public static string randomDroppedText()
        {
            Random rng = Utility.CreateRandom(Game1.currentGameTime.TotalGameTime.TotalMilliseconds);
            List<string> options = new List<string>();
            for (int i = 1; i < 6; i++)
            {
                var opt = Utils.TryGetI18n("Dialogue.JunimoScrap.DroppedItem." + i)();
                if (opt != "Missing translation key!")
                {
                    options.Add(opt);   
                }
            }
            if (options.Count == 0)
            {
                options.Add("...");
            }
            return rng.Choose(options.ToArray());
        }

        public static string randomExitHutText()
        {
            Random rng = Utility.CreateRandom(Game1.currentGameTime.TotalGameTime.TotalMilliseconds);
            List<string> options = new List<string>();
            for (int i = 1; i < 10; i++)
            {
                var opt = Utils.TryGetI18n("Dialogue.JunimoScrap.ExitHut." + i)();
                if (opt != "Missing translation key!")
                {
                    options.Add(opt);   
                }
            }
            if (options.Count == 0)
            {
                options.Add("...");
            }
            return rng.Choose(options.ToArray());
        }

        public static string randomCropAdjective(string cropName)
        {
            Random rng = Utility.CreateRandom(Game1.currentGameTime.TotalGameTime.TotalMilliseconds);
            List<string> options = new List<string>();
            for (int i = 1; i < 9; i++)
            {
                var opt = Utils.TryGetI18n("Dialogue.JunimoScrap.CropAdjective." + i)();
                if (opt != "Missing translation key!")
                {
                    options.Add(string.Format(opt, cropName));   
                }
            }
            if (options.Count == 0)
            {
                options.Add("...");
            }
            return rng.Choose(options.ToArray());
        }

        public static string randomHarvestText()
        {
            Random rng = Utility.CreateRandom(Game1.currentGameTime.TotalGameTime.TotalMilliseconds);
            List<string> options = new List<string>();
            for (int i = 1; i < 6; i++)
            {
                var opt = Utils.TryGetI18n("Dialogue.JunimoScrap.Harvest." + i)();
                if (opt != "Missing translation key!")
                {
                    options.Add(opt);   
                }
            }
            if (options.Count == 0)
            {
                options.Add("...");
            }
            return rng.Choose(options.ToArray());
        }

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(JunimoHut), nameof(JunimoHut.updateWhenFarmNotCurrentLocation))]
        static IEnumerable<CodeInstruction> updateWhenFarmNotCurrentLocation_Transpiler(
            IEnumerable<CodeInstruction> instructions,
            ILGenerator il)
        {
            if (!ModEntry.Config.EnableJunimoScrap) return instructions;
            var code = instructions.ToList();
            try
            {
                var matcher = new CodeMatcher(code, il);
                matcher.MatchStartForward(
                    new CodeMatch(OpCodes.Ldloc_0),
                    new CodeMatch(OpCodes.Ldstr, "junimoMeep1")
                ).ThrowIfNotMatch("Could not find proper entry point for updateWhenFarmNotCurrentLocation_Transpiler");

                matcher.Insert(
                    new CodeInstruction(OpCodes.Ldloc_S, 6),
                    new CodeInstruction(OpCodes.Call,
                        AccessTools.Method(typeof(JunimoScrap), nameof(JunimoScrap.randomExitHutText))),
                    new CodeInstruction(OpCodes.Ldc_I4, 1000),
                    new CodeInstruction(OpCodes.Call,
                        AccessTools.Method(typeof(JunimoScrap), nameof(JunimoScrap.showJunimoText)))
                );

                return matcher.InstructionEnumeration();
            }
            catch (Exception ex)
            {
                Log.Error("Error in ButtonsExtraBooks_JunimoScrap.updateWhenFarmNotCurrentLocation_Transpiler: \n" + ex);
                return code;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(JunimoHarvester), "isHarvestable")]
        static void isHarvestable_Postfix(JunimoHarvester __instance, bool __result)
        {
            if (!ModEntry.Config.EnableJunimoScrap || !__result) return;
            try
            {
                Random rng = Utility.CreateRandom(Game1.currentGameTime.TotalGameTime.TotalMilliseconds);
                if (!__instance.currentLocation.terrainFeatures.TryGetValue(__instance.Tile, out var feature) ||
                    feature is not HoeDirt dirt || !dirt.readyForHarvest()) return;
                if (!Game1.objectData.TryGetValue(dirt.crop.indexOfHarvest.Value, out var data)) return;
                string itemName = TokenParser.ParseText(data.DisplayName);
                string cropAdjective = randomCropAdjective(itemName);
                string harvestText = string.Format(randomHarvestText(), cropAdjective);
                showJunimoText(__instance, harvestText);
            }
            catch (Exception ex)
            {
                Log.Error("Error in ButtonsExtraBooks_JunimoScrap.isHarvestable_Postfix: \n" + ex);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(JunimoHarvester), nameof(JunimoHarvester.tryToAddItemToHut))]
        static void tryToAddItemToHut_Postfix(JunimoHarvester __instance)
        {
            if (!ModEntry.Config.EnableJunimoScrap) return;
            try
            {
                int random = new Random().Next(1, 251);
                if (random != 1) return;
                Item scrapbook = __instance.home?.GetOutputChest()
                    .addItem(ItemRegistry.Create("(O)Spiderbuttons.ButtonsExtraBooks_Book_JunimoScrap"));
                if (scrapbook != null)
                {
                    Game1.createObjectDebris(scrapbook.QualifiedItemId, __instance.TilePoint.X,
                        __instance.TilePoint.Y, -1, scrapbook.Quality, 1f, __instance.currentLocation);
                    showJunimoText(__instance, randomDroppedText());
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error in ButtonsExtraBooks_JunimoScrap.tryToAddItemToHut_Postfix: \n" + ex);
            }
        }

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(JunimoHarvester), nameof(JunimoHarvester.tryToAddItemToHut))]
        static IEnumerable<CodeInstruction> tryToAddItemToHut_Transpiler(IEnumerable<CodeInstruction> instructions,
            ILGenerator il)
        {
            if (!ModEntry.Config.EnableJunimoScrap) return instructions;
            var code = instructions.ToList();
            try
            {
                var matcher = new CodeMatcher(code, il);
                matcher.MatchStartForward(
                        new CodeMatch(OpCodes.Brfalse_S),
                        new CodeMatch(OpCodes.Ldc_I4_0),
                        new CodeMatch(OpCodes.Stloc_1))
                    .ThrowIfNotMatch("Could not find proper entry point for tryToAddItemToHut_Transpiler");

                matcher.Advance(1);

                matcher.Insert(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call,
                        AccessTools.Method(typeof(JunimoScrap), nameof(JunimoScrap.randomDroppedText))),
                    new CodeInstruction(OpCodes.Ldc_I4_0),
                    new CodeInstruction(OpCodes.Call,
                        AccessTools.Method(typeof(JunimoScrap), nameof(JunimoScrap.showJunimoText)))
                );

                return matcher.InstructionEnumeration();
            }
            catch (Exception ex)
            {
                Log.Error("Error in ButtonsExtraBooks_JunimoScrap.tryToAddItemToHut_Transpiler: \n" + ex);
                return code;
            }
        }
    }
}