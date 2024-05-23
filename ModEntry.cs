using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using ButtonsExtraBooks.Config;
using ButtonsExtraBooks.Helpers;
using ButtonsExtraBooks.Powers;
using ContentPatcher;
using GenericModConfigMenu;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace ButtonsExtraBooks
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
        internal static IMonitor ModMonitor { get; private set; } = null!;

        internal static IModHelper ModHelper { get; private set; } = null!;
        internal static Harmony Harmony { get; private set; } = null!;
        internal static ModConfig Config { get; private set; } = null!;
        internal static IContentPatcherAPI ContentPatcher { get; private set; } = null!;

        public override void Entry(IModHelper helper)
        {
            i18n.Init(helper.Translation);
            Config = helper.ReadConfig<ModConfig>();
            ModMonitor = Monitor;
            ModHelper = helper;
            var harmony = new Harmony(base.ModManifest.UniqueID);
            Harmony = harmony;
            harmony.PatchAll();

            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.GameLoop.DayEnding += JunimoScrap.OnDayEnding;
            helper.Events.Display.MenuChanged += this.OnMenuChange;
        }

        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady) return;
            if (Game1.player.stats.Get("Spiderbuttons.ButtonsExtraBooks_Debug_RemoveAll") != 0)
                RemovePowers.RemoveAll();
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            ContentPatcher = Helper.ModRegistry.GetApi<IContentPatcherAPI>("Pathoschild.ContentPatcher");
            if (ContentPatcher == null)
            {
                ModMonitor.Log("ContentPatcher not found. Button's Extra Books requires ContentPatcher to function.",
                    LogLevel.Error);
                return;
            }

            ContentPatcher.RegisterToken(
                mod: ModManifest,
                name: "ConfigAlwaysAvailable",
                getValue: () =>
                {
                    return new[]
                    {
                        Config.AlwaysAvailable.ToString()
                    };
                }
            );
            ContentPatcher.RegisterToken(
                mod: ModManifest,
                name: "ConfigEnableDebugBook",
                getValue: () =>
                {
                    return new[]
                    {
                        Config.DebugBook.ToString()
                    };
                }
            );

            foreach (var power in Assembly.GetExecutingAssembly().GetTypes().Where(t =>
                         t.Namespace == "ButtonsExtraBooks.Powers" && t.IsClass &&
                         !t.IsDefined(typeof(CompilerGeneratedAttribute), false)))
            {
                ContentPatcher.RegisterToken(
                    mod: ModManifest,
                    name: $"ConfigEnable{power.Name}",
                    getValue: () =>
                    {
                        return new[]
                        {
                            Config.GetPowerEnabled(power.Name).ToString()
                        };
                    }
                );
                ContentPatcher.RegisterToken(
                    mod: ModManifest,
                    name: $"ConfigPrice{power.Name}",
                    getValue: () =>
                    {
                        return new[]
                        {
                            Config.GetBookPrice(power.Name).ToString()
                        };
                    }
                );
            }

            ContentPatcher.RegisterToken(
                mod: ModManifest,
                name: "ConfigCheatCodesRequirement",
                getValue: () =>
                {
                    return new[]
                    {
                        Config.CheatCodesRequirement.ToString()
                    };
                }
            );

            ContentPatcher.RegisterToken(
                mod: ModManifest,
                name: "HasCoffeePower",
                getValue: () =>
                {
                    return Context.IsWorldReady
                        ? new[]
                        {
                            Game1.player.stats.Get("Spiderbuttons.ButtonsExtraBooks_Book_Coffee") > 0 ? "true" : "false"
                        }
                        : null;
                }
            );
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu != null) Config.SetupConfig(configMenu, ModManifest, Helper, Harmony);
        }
        
        private void OnMenuChange(object sender, MenuChangedEventArgs e)
        {
            if (e.NewMenu is not LetterViewerMenu letter)
                return;
            switch (letter.mailTitle)
            {
                case "Spiderbuttons.ButtonsExtraBooks_Mail_JunimoScrap_Crop":
                    letter.itemsToGrab[0].item = JunimoScrap.randomCropInSeason();
                    break;
                case "Spiderbuttons.ButtonsExtraBooks_Mail_JunimoScrap_Gem":
                    letter.itemsToGrab[0].item = JunimoScrap.randomGem();
                    break;
                case "Spiderbuttons.ButtonsExtraBooks_Mail_JunimoScrap_Item":
                    letter.itemsToGrab[0].item = JunimoScrap.randomItem();
                    break;
            }
        }
    }
}