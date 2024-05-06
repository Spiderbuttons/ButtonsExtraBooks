using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using ButtonsExtraBooks.Helpers;
using Microsoft.Xna.Framework;
using StardewValley.Characters;
using StardewValley.GameData.Pets;
using StardewValley.Internal;
using StardewValley.Monsters;

namespace ButtonsExtraBooks.Powers
{
    [HarmonyPatch]
    static class PetGifts
    {
        [HarmonyPatch(typeof(Pet), nameof(Pet.TryGetGiftItem))]
        private static bool Prefix(List<PetGift> gifts, Pet __instance, ref Item __result)
        {
            try
            {
                float totalWeight = 0f;
                foreach (PetGift gift2 in gifts)
                {
                    if (gift2.MinimumFriendshipThreshold <= __instance.friendshipTowardFarmer.Value)
                    {
                        totalWeight += gift2.Weight;
                    }
                }
                if (Game1.getAllFarmers().Any(f => f.stats.Get("Spiderbuttons.ButtonsExtraBooks_Book_PetGifts") != 0))
                {
                    totalWeight = Utility.RandomFloat(Utility.RandomFloat(0f, totalWeight), totalWeight);
                }
                else
                {
                    totalWeight = Utility.RandomFloat(0f, totalWeight);
                }
                foreach (PetGift gift in gifts)
                {
                    if (gift.MinimumFriendshipThreshold > __instance.friendshipTowardFarmer.Value)
                    {
                        continue;
                    }
                    totalWeight -= gift.Weight;
                    if (totalWeight <= 0f)
                    {
                        Item i = ItemQueryResolver.TryResolveRandomItem(gift.QualifiedItemID, null);
                        if (i != null && !i.Name.Contains("Error"))
                        {
                            i.Stack = gift.Stack;
                            __result = i;
                            return false;
                        }
                        __result = ItemRegistry.Create(gift.QualifiedItemID, gift.Stack);
                        return false;
                    }
                }
                __result = null;
                return false;
            }
            catch (Exception ex)
            {
                Loggers.Log("Error in ButtonsExtraBooks_PetGifts.TryGetGiftItem_Transpiler: \n" + ex, LogLevel.Error);
                return true;
            }
        }
    }
}