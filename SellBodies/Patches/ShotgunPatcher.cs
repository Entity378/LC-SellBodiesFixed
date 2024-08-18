using HarmonyLib;
using CleaningCompany.Monos;
using System.Collections.Generic;

namespace CleaningCompany.Patches
{
    [HarmonyPatch(typeof(StartOfRound))]
    internal class ShotgunPatcher
    {
        [HarmonyPostfix]
        [HarmonyPatch("Awake")]
        private static void PatchAwake(StartOfRound __instance)
        {
            List<Item> itemsList = __instance.allItemsList.itemsList;
            foreach (Item item in itemsList)
            {
                string itemName = item.itemName;
                if (itemName == "Shotgun" && item.spawnPrefab.AddComponent<ShotgunSyncer>() == null)
                {
                    item.spawnPrefab.AddComponent<ShotgunSyncer>();
                    break;
                }
            }
        }
    }
}