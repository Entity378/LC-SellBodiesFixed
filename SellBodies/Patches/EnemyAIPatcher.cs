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
            else if (Plugin.cfg.MODDEDENEMY && !Plugin.instance.VanillaBody.Contains(name))
            {
                __instance.StartCoroutine(SpawnModdedBody(__instance, name));
            }
        }

        static IEnumerator SpawnGenericBody(EnemyAI __instance, string name)
        {
            Vector3 PropBodyPos = __instance.transform.position;
            Vector3 SpawnPos = new Vector3(0, 1, 0);
            yield return new WaitForSeconds(4);

            GameObject gameObjectCreated = Object.Instantiate(Plugin.instance.BodySpawns[name].spawnPrefab, PropBodyPos + SpawnPos, Quaternion.identity);
            gameObjectCreated.GetComponent<NetworkObject>().Spawn();
            if (name == "Blob")
            {
                __instance.GetComponent<NetworkObject>().Despawn();
            }
        }

        static IEnumerator SpawnNutcrackerBody(EnemyAI __instance, string name)
        {
            Vector3 PropBodyPos = __instance.transform.position;
            Vector3 SpawnPos = new Vector3(0, 1, 0);
            __instance.GetComponent<NutcrackerEnemyAI>().gun.GetComponent<NetworkObject>().Despawn(true);

            List<Item> itemsList = StartOfRound.Instance.allItemsList.itemsList;
            foreach (Item item in itemsList)
            {
                string itemName = item.itemName;
                if (itemName == "Shotgun")
                {
                    GameObject shotgunItem = Object.Instantiate(item.spawnPrefab, PropBodyPos + SpawnPos, Quaternion.identity);
                    shotgunItem.GetComponent<NetworkObject>().Spawn();
                    break;
                }
            }
            yield return new WaitForSeconds(4);

            GameObject gameObjectCreated = Object.Instantiate(Plugin.instance.BodySpawns[name].spawnPrefab, PropBodyPos + SpawnPos, Quaternion.identity);
            gameObjectCreated.GetComponent<NetworkObject>().Spawn();
        }

        static IEnumerator SpawnModdedBody(EnemyAI __instance, string name)
        {
            Vector3 PropBodyPos = __instance.transform.position;
            Vector3 SpawnPos = new Vector3(0, 2, 0);
            yield return new WaitForSeconds(4);

            GameObject gameObjectCreated;
            if (__instance.enemyType.PowerLevel <= 1)
            {
                gameObjectCreated = Object.Instantiate(Plugin.instance.BodySpawns["ModdedEnemyPowerLevel1"].spawnPrefab, PropBodyPos + SpawnPos, Quaternion.identity);
            }
            else if (__instance.enemyType.PowerLevel == 2)
            {
                gameObjectCreated = Object.Instantiate(Plugin.instance.BodySpawns["ModdedEnemyPowerLevel2"].spawnPrefab, PropBodyPos + SpawnPos, Quaternion.identity);
            }
            else
            {
                gameObjectCreated = Object.Instantiate(Plugin.instance.BodySpawns["ModdedEnemyPowerLevel3"].spawnPrefab, PropBodyPos + SpawnPos, Quaternion.identity);
            }

            gameObjectCreated.GetComponent<NetworkObject>().Spawn();
        }
    }
}
