using System;
using StardewValley;
using StardewModdingAPI;

namespace ButtonsExtraBooks.Helpers;

public class Utils
{
    static readonly string powerPrefix = "Spiderbuttons.ButtonsExtraBooks_Book_";
    
    public static bool PlayerHasPower(string powerID)
    {
        return Game1.player.stats.Get(powerPrefix + powerID) != 0;
    }

    public static bool PlayerHasPower(Farmer who, string powerID)
    {
        return who.stats.Get(powerPrefix + powerID) != 0;
    }

    public static bool AnyoneHasPower(string powerID)
    {
        foreach (var farmer in Game1.getAllFarmers())
        {
            if (farmer.stats.Get(powerPrefix + powerID) != 0)
            {
                return true;
            }
        }
        return false;
    }
    
    public static string GetLanguageCode()
    {
        return LocalizedContentManager.CurrentLanguageCode.ToString() != "mod" ? LocalizedContentManager.CurrentLanguageCode.ToString() : LocalizedContentManager.CurrentModLanguage.LanguageCode;
    }
    
    public static Func<string> TryGetI18n(string key)
    {
        return () =>
        {
            if (ModEntry.ContentPackI18n.TryGetValue(GetLanguageCode(), out var i18nStrings))
            {
                if (i18nStrings.TryGetValue(key, out var i18nString))
                {
                    return i18nString;
                }
            } else if (ModEntry.ContentPackI18n.TryGetValue("default", out var defaultStrings))
            {
                if (defaultStrings.TryGetValue(key, out var defaultString))
                {
                    return defaultString;
                }
            }
            
            return "Missing translation key!";
        };
    }
}