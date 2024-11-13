using System;
using HarmonyLib;
using ButtonsExtraBooks.Helpers;

namespace ButtonsExtraBooks.Powers
{
    [HarmonyPatch]
    static class ArtisanMachines
    {
        private static int CheckArtisanBuff(int currentMinutesUntilReady)
        {
            if (currentMinutesUntilReady == 0) return currentMinutesUntilReady;
            var artisanManualPercentDecrease = (100.0f - ModEntry.Config.ArtisanMachinesPercentDecrease)/100.0f;
            
            if (Utils.AnyoneHasPower("ArtisanMachines"))
            {
                currentMinutesUntilReady = (int) (currentMinutesUntilReady * artisanManualPercentDecrease);
            }
            
            return currentMinutesUntilReady;
        }
        
        [HarmonyPatch(typeof(StardewValley.Object), nameof(StardewValley.Object.OutputMachine))]
        private static void Postfix(StardewValley.Object __instance)
        {
            if (!ModEntry.Config.EnableArtisanMachines || __instance.heldObject.Value.Category != -26) return;
            try
            {
                __instance.MinutesUntilReady = CheckArtisanBuff(__instance.MinutesUntilReady);
            }
            catch (Exception ex)
            {
                Log.Error("Error in ButtonsExtraBooks_ArtisanMachines.OutputMachine_Postfix: \n" + ex);
            }
        }
    }
}