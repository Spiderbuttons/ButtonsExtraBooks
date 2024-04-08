using System.Net.Mime;
using GenericModConfigMenu;
using HarmonyLib;
using StardewModdingAPI;

namespace ButtonsExtraBooks.Config;

public sealed class ModConfig
{
    public bool AlwaysAvailable { get; set; } = false;
    
    public bool EnableLuck { get; set; } = true;
    public float LuckAmount { get; set; } = 0.025f;
    
    public bool EnableExtraGifts { get; set; } = true;
    
    public bool EnableTreesIgnoreSeason { get; set; } = true;
    
    public bool EnableArtisanMachines { get; set; } = true;
    public int ArtisanMachinesPercentDecrease { get; set; } = 15;
    
    public bool EnableGiantCrops { get; set; } = true;
    public int GiantCropsPercent { get; set; } = 5;
    
    public bool DebugBook { get; set; } = false;

    public ModConfig()
    {
        Init();
    }

    private void Init()
    {
        AlwaysAvailable = false;
        EnableLuck = true;
        LuckAmount = 0.025f;
        EnableExtraGifts = true;
        EnableTreesIgnoreSeason = true;
        EnableArtisanMachines = true;
        ArtisanMachinesPercentDecrease = 15;
        EnableGiantCrops = true;
        GiantCropsPercent = 5;
        DebugBook = false;
    }
    
    public bool GetPowerEnabled(string powerName)
    {
        // return whether the bool property with the same name is enabled in this modconfig class
        return (bool)GetType().GetProperty($"Enable{powerName}")?.GetValue(this)!;
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
            name: i18n.Config_Books_EnableLuck_Name,
            tooltip: i18n.Config_Books_EnableLuck_Description,
            getValue: () => EnableLuck,
            setValue: value => EnableLuck = value
        );
        configMenu.AddNumberOption(
            mod: ModManifest,
            name: i18n.Config_Books_LuckBonus_Name,
            tooltip: i18n.Config_Books_LuckBonus_Description,
            getValue: () => LuckAmount,
            setValue: value => LuckAmount = value
        );
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: i18n.Config_Books_EnableExtraGifts_Name,
            tooltip: i18n.Config_Books_EnableExtraGifts_Description,
            getValue: () => EnableExtraGifts,
            setValue: value => EnableExtraGifts = value
        );
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: i18n.Config_Books_EnableTreesIgnoreSeason_Name,
            tooltip: i18n.Config_Books_EnableTreesIgnoreSeason_Description,
            getValue: () => EnableTreesIgnoreSeason,
            setValue: value => EnableTreesIgnoreSeason = value
        );
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: i18n.Config_Books_EnableArtisanMachines_Name,
            tooltip: i18n.Config_Books_EnableArtisanMachines_Description,
            getValue: () => EnableArtisanMachines,
            setValue: value => EnableArtisanMachines = value
        );
        configMenu.AddNumberOption(
            mod: ModManifest,
            name: i18n.Config_Books_ArtisanMachinesBonus_Name,
            tooltip: i18n.Config_Books_ArtisanMachinesBonus_Description,
            getValue: () => ArtisanMachinesPercentDecrease,
            setValue: (value) =>
            {
                ArtisanMachinesPercentDecrease = value;
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
            name: i18n.Config_Books_EnableGiantCrops_Name,
            tooltip: i18n.Config_Books_EnableGiantCrops_Description,
            getValue: () => EnableGiantCrops,
            setValue: value => EnableGiantCrops = value
        );
        configMenu.AddNumberOption(
            mod: ModManifest,
            name: i18n.Config_Books_GiantCropsBonus_Name,
            tooltip: i18n.Config_Books_GiantCropsBonus_Description,
            getValue: () => GiantCropsPercent,
            setValue: value => GiantCropsPercent = value,
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
            pageTitle: i18n.Config_SectionTitle_Debug
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