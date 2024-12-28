using HarmonyLib;
using CleaningCompany.Monos;
using System.Linq;
using UnityEngine;

namespace CleaningCompany.Patches
{
    [HarmonyPatch(typeof(StartOfRound))]
    internal class ItemsPatcher
    {
        public static Item tragedyMask;
        public static Item comedyMask;
        public static Item shotgun;

        [HarmonyPostfix]
        [HarmonyPatch("Awake")]
        private static void PatchAwake(StartOfRound __instance)
        {
            try 
            {
                tragedyMask = StartOfRound.Instance.allItemsList.itemsList.First(i => i.name == "TragedyMask");
                comedyMask = StartOfRound.Instance.allItemsList.itemsList.First(i => i.name == "ComedyMask");
                shotgun = StartOfRound.Instance.allItemsList.itemsList.First(i => i.name == "Shotgun");

                shotgun.spawnPrefab.AddComponent<ShotgunSyncer>();
                tragedyMask.spawnPrefab.AddComponent<MaskSyncer>();
                comedyMask.spawnPrefab.AddComponent<MaskSyncer>();
            }
            catch 
            {
                Debug.LogError("ItemScripts not applied correctly");
            }
        }
    }
}