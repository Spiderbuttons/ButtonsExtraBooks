using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using StardewValley;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using ButtonsExtraBooks.Helpers;

namespace ButtonsExtraBooks.Helpers;

static class RemovePowers
{
    public static void RemoveAll(bool refund = true)
    {
        Game1.player.stats.Set("Spiderbuttons.ButtonsExtraBooks_Debug_RemoveAll", 0);
        foreach (var power in Assembly.GetExecutingAssembly().GetTypes().Where(t => t.Namespace == "ButtonsExtraBooks.Powers" && t.IsClass && !t.IsDefined(typeof(CompilerGeneratedAttribute), false)))
        {
            if (Game1.player.stats.Get($"Spiderbuttons.ButtonsExtraBooks_Book_{power.Name}") != 0)
            {
                Game1.player.stats.Set($"Spiderbuttons.ButtonsExtraBooks_Book_{power.Name}", 0);
                if (refund) Game1.player.dropItem(ItemRegistry.Create($"(O)Spiderbuttons.ButtonsExtraBooks_Book_{power.Name}"));
            }
        }
    }
    
    public static void RemovePower(string powerName, bool refund = true)
    {
        Game1.player.stats.Set($"Spiderbuttons.ButtonsExtraBooks_Book_{powerName}", 0);
        if (refund) Game1.player.dropItem(ItemRegistry.Create($"(O)Spiderbuttons.ButtonsExtraBooks_Book_{powerName}"));
    }
}