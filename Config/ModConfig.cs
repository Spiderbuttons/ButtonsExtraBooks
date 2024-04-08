using System.Net.Mime;
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
    
    public bool BigMelonsBook { get; set; } = true;
    public int BigMelonsPercent { get; set; } = 5;
    
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
        SetupPages(configMenu, ModManifest, Helper, harmony);
        SetupGeneral(configMenu, ModManifest, Helper, harmony);
        SetupBooks(configMenu, ModManifest, Helper, harmony);
        SetupDebug(configMenu, ModManifest, Helper, harmony);
    }
    
    private static void SetupPages(IGenericModConfigMenuApi configMenu, IManifest ModManifest, IModHelper Helper,
        Harmony harmony)
    {
        configMenu.AddPageLink(
            mod: ModManifest,
            pageId: "Config.Pages.General",
            text: i18n.Config_SectionTitle_General
        );
        configMenu.AddPageLink(
            mod: ModManifest,
            pageId: "Config.Pages.Books",
            text: i18n.Config_SectionTitle_Books
        );
        configMenu.AddPageLink(
            mod: ModManifest,
            pageId: "Config.Pages.Debug",
            text: i18n.Config_SectionTitle_Debug
        );
    }
    
    private void SetupGeneral(IGenericModConfigMenuApi configMenu, IManifest ModManifest, IModHelper Helper,
        Harmony harmony)
    {
        configMenu.AddPage(
            mod: ModManifest,
            pageId: "Config.Pages.General",
            pageTitle: i18n.Config_SectionTitle_General
        );
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: i18n.Config_General_AlwaysAvailable_Name,
            tooltip: i18n.Config_General_AlwaysAvailable_Description,
            getValue: () => AlwaysAvailable,
            setValue: value => AlwaysAvailable = value
        );
    }
    
    private void SetupBooks(IGenericModConfigMenuApi configMenu, IManifest ModManifest, IModHelper Helper,
        Harmony harmony)
    {
        configMenu.AddPage(
            mod: ModManifest,
            pageId: "Config.Pages.Books",
            pageTitle: i18n.Config_SectionTitle_Books
        );
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: i18n.Config_Books_EnableBeginnersLuck_Name,
            tooltip: i18n.Config_Books_EnableBeginnersLuck_Description,
            getValue: () => EnableBeginnersLuck,
            setValue: value => EnableBeginnersLuck = value
        );
        configMenu.AddNumberOption(
            mod: ModManifest,
            name: i18n.Config_Books_BeginnersLuckBonus_Name,
            tooltip: i18n.Config_Books_BeginnersLuckBonus_Description,
            getValue: () => BeginnersLuckAmount,
            setValue: value => BeginnersLuckAmount = value
        );
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: i18n.Config_Books_EnableBookOfAltruism_Name,
            tooltip: i18n.Config_Books_EnableBookOfAltruism_Description,
            getValue: () => EnableBookOfAltruism,
            setValue: value => EnableBookOfAltruism = value
        );
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: i18n.Config_Books_EnableWinterForestry_Name,
            tooltip: i18n.Config_Books_EnableWinterForestry_Description,
            getValue: () => EnableWinterForestry,
            setValue: value => EnableWinterForestry = value
        );
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: i18n.Config_Books_EnableArtisanManual_Name,
            tooltip: i18n.Config_Books_EnableArtisanManual_Description,
            getValue: () => EnableArtisanManual,
            setValue: value => EnableArtisanManual = value
        );
        configMenu.AddNumberOption(
            mod: ModManifest,
            name: i18n.Config_Books_ArtisanManualBonus_Name,
            tooltip: i18n.Config_Books_ArtisanManualBonus_Description,
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
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: i18n.Config_Books_EnableBigMelons_Name,
            tooltip: i18n.Config_Books_EnableBigMelons_Description,
            getValue: () => BigMelonsBook,
            setValue: value => BigMelonsBook = value
        );
        configMenu.AddNumberOption(
            mod: ModManifest,
            name: i18n.Config_Books_BigMelonsBonus_Name,
            tooltip: i18n.Config_Books_BigMelonsBonus_Description,
            getValue: () => BigMelonsPercent,
            setValue: value => BigMelonsPercent = value,
            min: 2,
            max: 100,
            interval: 1,
            formatValue: value => $"{value}%"
        );
    }
    
    private void SetupDebug(IGenericModConfigMenuApi configMenu, IManifest ModManifest, IModHelper Helper,
        Harmony harmony)
    {
        configMenu.AddPage(
            mod: ModManifest,
            pageId: "Config.Pages.Debug",
            pageTitle: i18n.Config_Books_EnableBeginnersLuck_Name
        );
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: i18n.Config_Debug_EnableDebugBook_Name,
            tooltip: i18n.Config_Debug_EnableDebugBook_Description,
            getValue: () => DebugBook,
            setValue: value => DebugBook = value
        );
    }
}