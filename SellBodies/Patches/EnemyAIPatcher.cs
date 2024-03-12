using HarmonyLib;
using Unity.Netcode;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CleaningCompany.Patches
{
    [HarmonyPatch(typeof(EnemyAI))]
    internal class EnemyAIPatcher
    {
        private static ulong currentEnemy = 9999999;
        [HarmonyPostfix]
        [HarmonyPatch("KillEnemyServerRpc")]
        static void SpawnScrapBody(EnemyAI __instance)
        {
            if (currentEnemy == __instance.NetworkObject.NetworkObjectId) return;
            if (!__instance.IsHost) return;
            currentEnemy = __instance.NetworkObject.NetworkObjectId;
            string name = __instance.enemyType.enemyName;
            if (Plugin.instance.BodySpawns.ContainsKey(name))
            {
                if (name == "Nutcracker")
                {
                    __instance.StartCoroutine(SpawnNutcrackerBody(__instance, name));
                }
                else
                {
                    __instance.StartCoroutine(SpawnGenericBody(__instance, name));
                }
            }
        }

        static IEnumerator SpawnGenericBody(EnemyAI __instance, string name)
        {
            yield return new WaitForSeconds(4);

            GameObject gameObjectCreated = Object.Instantiate(Plugin.instance.BodySpawns[name].spawnPrefab, __instance.transform.position + Vector3.up, Quaternion.identity);
            gameObjectCreated.GetComponent<NetworkObject>().Spawn();
            __instance.GetComponent<NetworkObject>().transform.position = new Vector3(-10000, -10000, -10000);
            __instance.SyncPositionToClients();
        }

        static IEnumerator SpawnNutcrackerBody(EnemyAI __instance, string name)
        {
            __instance.GetComponent<NutcrackerEnemyAI>().gun.GetComponent<NetworkObject>().Despawn(true);

            List<Item> itemsList = StartOfRound.Instance.allItemsList.itemsList;
            foreach (Item item in itemsList)
            {
                string itemName = item.itemName;
                if (itemName == "Shotgun")
                {
                    GameObject shotgunItem = Object.Instantiate(item.spawnPrefab, __instance.transform.position + Vector3.up, Quaternion.identity);
                    shotgunItem.GetComponent<NetworkObject>().Spawn();
                    break;
                }
            }
            yield return new WaitForSeconds(4);

            GameObject gameObjectCreated = Object.Instantiate(Plugin.instance.BodySpawns[name].spawnPrefab, __instance.transform.position + Vector3.up, Quaternion.identity);
            gameObjectCreated.GetComponent<NetworkObject>().Spawn();
            __instance.GetComponent<NetworkObject>().transform.position = new Vector3(-10000, -10000, -10000);
            __instance.SyncPositionToClients();
        }
    }
}