using GenericModConfigMenu;
using HarmonyLib;
using StardewModdingAPI;

namespace ButtonsExtraBooks.Config;

public sealed class ModConfig
{
    public bool AlwaysAvailable { get; set; } = false;
    
    public bool EnableBeginnersLuck { get; set; } = true;
    public float BeginnersLuckAmount { get; set; } = 0.025f;
    
    public bool EnableBookOfAltruism { get; set; } = true;
    
    public bool EnableWinterForestry { get; set; } = true;
    
    public bool EnableArtisanManual { get; set; } = true;
    public int ArtisanManualPercentDecrease { get; set; } = 15;
    
    public bool DebugBook { get; set; } = false;

    public ModConfig()
    {
        Init();
    }

    private void Init()
    {
        AlwaysAvailable = false;
        EnableBeginnersLuck = true;
        BeginnersLuckAmount = 0.025f;
        EnableBookOfAltruism = true;
        EnableWinterForestry = true;
        EnableArtisanManual = true;
        ArtisanManualPercentDecrease = 15;
        DebugBook = false;
    }

    public void SetupConfig(IGenericModConfigMenuApi configMenu, IManifest ModManifest, IModHelper Helper, Harmony harmony)
    {
        configMenu.Register(
                    mod: ModManifest,
                    reset: Init,
                    save: () => Helper.WriteConfig(this)
                );
                configMenu.AddSectionTitle(
                    mod: ModManifest,
                    text: () => "General Settings"
                );
                configMenu.AddBoolOption(
                    mod: ModManifest,
                    name: () => "Always Available",
                    tooltip: () => "If enabled, all books will always be available from the bookseller without the need to meet certain requirements.",
                    getValue: () => AlwaysAvailable,
                    setValue: value => AlwaysAvailable = value
                );
                configMenu.AddSectionTitle(
                    mod: ModManifest,
                    text: () => "Book Settings"
                );
                configMenu.AddBoolOption(
                    mod: ModManifest,
                    name: () => "Enable Beginner's Luck",
                    tooltip: () => "If disabled, the Beginner's Luck book will not be available.",
                    getValue: () => EnableBeginnersLuck,
                    setValue: value => EnableBeginnersLuck = value
                );
                configMenu.AddNumberOption(
                    mod: ModManifest,
                    name: () => "Beginner's Luck Bonus",
                    tooltip: () => "How much luck is added after reading the Beginner's Luck book.",
                    getValue: () => BeginnersLuckAmount,
                    setValue: value => BeginnersLuckAmount = value
                );
                configMenu.AddBoolOption(
                    mod: ModManifest,
                    name: () => "Enable Book of Altruism",
                    tooltip: () => "If disabled, the Book of Altruism book will not be available.",
                    getValue: () => EnableBookOfAltruism,
                    setValue: value => EnableBookOfAltruism = value
                );
                configMenu.AddBoolOption(
                    mod: ModManifest,
                    name: () => "Enable Winter Forestry",
                    tooltip: () => "If disabled, the Winter Forestry book will not be available.",
                    getValue: () => EnableWinterForestry,
                    setValue: value => EnableWinterForestry = value
                );
                configMenu.AddBoolOption(
                    mod: ModManifest,
                    name: () => "Enable Artisans Guild Manual",
                    tooltip: () => "If disabled, the Artisans Guild Manual book will not be available.",
                    getValue: () => EnableArtisanManual,
                    setValue: value => EnableArtisanManual = value
                );
                configMenu.AddNumberOption(
                    mod: ModManifest,
                    name: () => "Artisans Guild Manual Bonus",
                    tooltip: () => "Percent decrease to the processing time of machines producing artisan goods.",
                    getValue: () => ArtisanManualPercentDecrease,
                    setValue: (value) =>
                    {
                        ArtisanManualPercentDecrease = value;
                        harmony.UnpatchAll(ModManifest.UniqueID);
                        harmony.PatchAll();
                    },
                    min: 1,
                    max: 50,
                    interval: 1,
                    formatValue: (value) => $"{value}%"
                );
                configMenu.AddSectionTitle(
                    mod: ModManifest,
                    text: () => "Debug Settings"
                );
                configMenu.AddBoolOption(
                    mod: ModManifest,
                    name: () => "Debug Book",
                    tooltip: () => "If enabled, a debug book will be available from Pierre that will remove all of your powers.",
                    getValue: () => DebugBook,
                    setValue: value => DebugBook = value
                );
    }
}