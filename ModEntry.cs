using System;
using Microsoft.Xna.Framework;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace ButtonsExtraBooks
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            var harmony = new Harmony(base.ModManifest.UniqueID);
            ButtonsExtraBooks_Luck.Apply(harmony, base.Monitor);
            ButtonsExtraBooks_TreesIgnoreSeason.Apply(harmony, base.Monitor);
            ButtonsExtraBooks_ExtraGifts.Apply(harmony, base.Monitor);
            ButtonsExtraBooks_ArtisanMachines.Apply(harmony, base.Monitor);
        }
    }
}