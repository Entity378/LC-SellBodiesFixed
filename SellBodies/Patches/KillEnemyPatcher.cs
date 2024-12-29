using HarmonyLib;
using UnityEngine;
using System.Collections;

namespace CleaningCompany.Patches
{
    [HarmonyPatch(typeof(EnemyAI))]
    internal class KillEnemyPatcher
    {
        [HarmonyPostfix]
        [HarmonyPatch("KillEnemy")]
        static void MoveBody(EnemyAI __instance)
        {
            if (__instance != null) 
            {
                string name = __instance.enemyType.enemyName;
                if (Plugin.instance.BodySpawns.ContainsKey(name))
                {
                    __instance.StartCoroutine(MoveOldBody(__instance));

                }
                else if (Plugin.cfg.MODDEDENEMY && !Plugin.instance.VanillaBody.Contains(name) && !Plugin.instance.BlackListed.Contains(name))
                {
                    __instance.StartCoroutine(MoveOldBody(__instance));
                }

                if (Plugin.cfg.CONFETTI)
                {
                    SpawnConfetti(__instance);
                }
            }
        }

        static IEnumerator MoveOldBody(EnemyAI __instance)
        {
            yield return new WaitForSeconds(4);
            if(__instance != null)
            {
                Vector3 newBodyPos = new Vector3(-10000, -10000, -10000);
                __instance.transform.position = newBodyPos;
                __instance.SyncPositionToClients();
            }
            else 
            {
                Debug.LogError("__instance is null in MoveOldBody() inside KillEnemy()");
            }
        }

        static void SpawnConfetti(EnemyAI __instance)
        {
            if(__instance != null) 
            {
                Object.Instantiate(Plugin.confettiPrefab, __instance.transform.position, Quaternion.Euler(0f, 0f, 0f),
                RoundManager.Instance.mapPropsContainer.transform).SetActive(value: true);
            }
            else
            {
                Debug.LogError("__instance is null in SpawnConfetti() inside KillEnemy()");
            }
        }
    }
}