using ButtonsExtraBooks.Config;
using ButtonsExtraBooks.Helpers;
using ContentPatcher;
using GenericModConfigMenu;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace ButtonsExtraBooks
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
        internal static IMonitor ModMonitor { get; private set; } = null!;
        internal static Harmony Harmony { get; private set; } = null!;
        internal static ModConfig Config { get; private set; } = null!;
        internal static IContentPatcherAPI ContentPatcher { get; private set; } = null!;
        public override void Entry(IModHelper helper)
        {
            i18n.Init(helper.Translation);
            Config = helper.ReadConfig<ModConfig>();
            ModMonitor = this.Monitor;
            var harmony = new Harmony(base.ModManifest.UniqueID);
            Harmony = harmony;
            harmony.PatchAll();

            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        }
        
        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady) return;
            if (Game1.player.stats.Get("Spiderbuttons.ButtonsExtraBooks_Debug_RemoveAll") != 0) RemovePowers.RemoveAll();
        }
        
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            ContentPatcher = Helper.ModRegistry.GetApi<IContentPatcherAPI>("Pathoschild.ContentPatcher");
            if (ContentPatcher == null)
            {
                ModMonitor.Log("ContentPatcher not found. This mod requires ContentPatcher to function.", LogLevel.Error);
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
                name: "ConfigDebugBook",
                getValue: () =>
                {
                    return new[]
                    {
                        Config.DebugBook.ToString()
                    };
                }
            );
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu != null) Config.SetupConfig(configMenu, ModManifest, Helper, Harmony);
        }
    }
}