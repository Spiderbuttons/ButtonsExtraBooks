using System;
using System.Collections.Generic;
using System.Diagnostics;
using HarmonyLib;
using StardewValley;
using StardewModdingAPI;
using ButtonsExtraBooks.Helpers;
using StardewModdingAPI.Events;
using StardewValley.GameData;
using StardewValley.GameData.Shops;

namespace ButtonsExtraBooks.Powers
{
    static class Carols
    {
        public static void AddShopModifiers(object sender, AssetRequestedEventArgs e)
        {
            if (!e.NameWithoutLocale.IsEquivalentTo("Data/Shops")) return;
            e.Edit(asset =>
            {
                var data = asset.AsDictionary<string, ShopData>().Data;
                foreach ((string shopID, ShopData shopData) in data)
                {
                    foreach (var item in shopData.Items)
                    {
                        item.PriceModifiers ??= [];
                        item.PriceModifiers.Add(GetPriceModifier());
                    }
                }
            }, AssetEditPriority.Late + 69);
        }

        private static QuantityModifier GetPriceModifier()
        {
            QuantityModifier mod = new QuantityModifier
            {
                Id = "Spiderbuttons.ButtonsExtraBooks_CarolsPrices",
                Modification = QuantityModifier.ModificationType.Multiply,
                Condition = "PLAYER_STAT Current Spiderbuttons.ButtonsExtraBooks_Book_Carols 1, SEASON winter, !PLAYER_LOCATION_NAME Current JojaMart",
                Amount = 0.85f
            };
            return mod;
        }
    }
}