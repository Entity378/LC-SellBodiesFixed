using HarmonyLib;
using Unity.Netcode;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameNetcodeStuff;
using System.Linq;

namespace CleaningCompany.Patches
{
    [HarmonyPatch(typeof(EnemyAI))]
    internal class KillEnemyServerRpcPatcher
    {
        public static Quaternion publicBodyRotation;
        public static Quaternion publicShotgunRotation;
        public static Quaternion publicMaskRotation;
        public static int publicShotgunPrice;

        private static ulong currentEnemy = 9999999;

        [HarmonyPrefix]
        [HarmonyPatch("KillEnemyServerRpc")] 
        static void SpawnScrapBody(EnemyAI __instance)
        {
            if (currentEnemy == __instance.NetworkObject.NetworkObjectId) return;
            if (!__instance.IsHost) return;
            if (__instance.GetComponentInChildren<PlayerControllerB>()) return;
            currentEnemy = __instance.NetworkObject.NetworkObjectId;
            string name = __instance.enemyType.enemyName;
            Vector3 propBodyPos = __instance.transform.position;

            if (Plugin.instance.BodySpawns.ContainsKey(name))
            {
                if (name == "Nutcracker")
                {
                    __instance.StartCoroutine(SpawnNutcrackerBody(__instance, name, propBodyPos));
                }
                else
                {
                    __instance.StartCoroutine(SpawnGenericBody(__instance, name, propBodyPos));
                }
            }
            else if (name == "Masked" && Plugin.cfg.MASKED)
            {
                __instance.StartCoroutine(SpawnMask(__instance));
            }
            else if (Plugin.cfg.MODDEDENEMY && !Plugin.instance.VanillaBody.Contains(name) && !Plugin.instance.BlackListed.Contains(name))
            {
                __instance.StartCoroutine(SpawnModdedBody(__instance, propBodyPos));
            }
        }

        static IEnumerator SpawnGenericBody(EnemyAI __instance, string name, Vector3 propBodyPos)
        {
            Quaternion propBodyRot = __instance.transform.rotation;
            Vector3 spawnPos = propBodyPos + new Vector3(0, 1, 0);
            yield return new WaitForSeconds(4);

            publicBodyRotation = propBodyRot;
            GameObject gameObjectCreated = Object.Instantiate(Plugin.instance.BodySpawns[name].spawnPrefab, spawnPos, propBodyRot, RoundManager.Instance.mapPropsContainer.transform);
            gameObjectCreated.GetComponent<NetworkObject>().Spawn();

            if (name == "Blob")
            {
                __instance.GetComponent<NetworkObject>().Despawn();
            }
        }

        static IEnumerator SpawnNutcrackerBody(EnemyAI __instance, string name, Vector3 propBodyPos)
        {
            Quaternion propBodyRot = __instance.transform.rotation;
            Vector3 spawnPos = propBodyPos + new Vector3(0, 1, 0);

            publicShotgunPrice = __instance.GetComponent<NutcrackerEnemyAI>().gun.scrapValue;
            __instance.GetComponent<NutcrackerEnemyAI>().gun.GetComponent<NetworkObject>().Despawn();

            List<Item> itemsList = StartOfRound.Instance.allItemsList.itemsList;
            foreach (Item item in itemsList)
            {
                string itemName = item.itemName;
                if (itemName == "Shotgun")
                {
                    GameObject shotgunItem = Object.Instantiate(item.spawnPrefab, spawnPos, propBodyRot, RoundManager.Instance.mapPropsContainer.transform);
                    publicShotgunRotation = propBodyRot;
                    shotgunItem.GetComponent<NetworkObject>().Spawn();
                    break;
                }
            }
            yield return new WaitForSeconds(4);

            GameObject gameObjectCreated = Object.Instantiate(Plugin.instance.BodySpawns[name].spawnPrefab, spawnPos, propBodyRot, RoundManager.Instance.mapPropsContainer.transform);
            publicBodyRotation = propBodyRot;
            gameObjectCreated.GetComponent<NetworkObject>().Spawn();
        }

        static IEnumerator SpawnMask(EnemyAI __instance) 
        {
            yield return new WaitForSeconds(0.1f);
            MaskedPlayerEnemy masked = __instance.GetComponent<MaskedPlayerEnemy>();
            var maskToSpawn = masked.maskTypes[0].activeSelf ? ItemsPatcher.comedyMask : ItemsPatcher.tragedyMask;
            Vector3 spawnPos = __instance.transform.position + new Vector3(0, 2.5f, 0);
            Quaternion propBodyRot = __instance.transform.rotation;
            publicMaskRotation = propBodyRot;
            GameObject gameObjectCreated = Object.Instantiate(maskToSpawn.spawnPrefab, spawnPos, propBodyRot, RoundManager.Instance.mapPropsContainer.transform);
            gameObjectCreated.GetComponent<NetworkObject>().Spawn();
        }

        static IEnumerator SpawnModdedBody(EnemyAI __instance, Vector3 propBodyPos)
        {
            Quaternion propBodyRot = __instance.transform.rotation;
            Vector3 spawnPos = propBodyPos + new Vector3(0, 2, 0);
            yield return new WaitForSeconds(4);

            GameObject gameObjectCreated;
            if (__instance.GetComponent<EnemyAI>().enemyType.PowerLevel <= 1)
            {
                gameObjectCreated = Object.Instantiate(Plugin.instance.BodySpawns["ModdedEnemyPowerLevel1"].spawnPrefab, spawnPos, propBodyRot, RoundManager.Instance.mapPropsContainer.transform);
            }
            else if (__instance.GetComponent<EnemyAI>().enemyType.PowerLevel == 2)
            {
                gameObjectCreated = Object.Instantiate(Plugin.instance.BodySpawns["ModdedEnemyPowerLevel2"].spawnPrefab, spawnPos, propBodyRot, RoundManager.Instance.mapPropsContainer.transform);
            }
            else
            {
                gameObjectCreated = Object.Instantiate(Plugin.instance.BodySpawns["ModdedEnemyPowerLevel3"].spawnPrefab, spawnPos, propBodyRot, RoundManager.Instance.mapPropsContainer.transform);
            }
            publicBodyRotation = propBodyRot;
            gameObjectCreated.GetComponent<NetworkObject>().Spawn();
        }
    }
}
