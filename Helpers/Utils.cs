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
}