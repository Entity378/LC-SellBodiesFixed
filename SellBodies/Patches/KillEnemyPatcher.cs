using HarmonyLib;
using UnityEngine;
using System.Collections;

namespace CleaningCompany.Patches
{
    [HarmonyPatch(typeof(EnemyAI))]
    internal class KillEnemyPatcher
    {
        [HarmonyPrefix]
        [HarmonyPatch("KillEnemy")]
        static void MoveBody(EnemyAI __instance)
        {
            if (!__instance.isEnemyDead) 
            {
                string name = __instance.enemyType.enemyName;
                if (Plugin.instance.BodySpawns.ContainsKey(name))
                {
                    __instance.StartCoroutine(MoveOldBody(__instance));

                }
                else if (Plugin.cfg.MODDEDENEMY && !Plugin.instance.VanillaBody.Contains(name))
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
            try 
            {
                Vector3 newBodyPos = new Vector3(-10000, -10000, -10000);
                __instance.transform.position = newBodyPos;
                __instance.SyncPositionToClients();
            }
            catch
            {
                Debug.LogError("An error has occurred in MoveOldBody() inside KillEnemy()");
            }
        }

        static void SpawnConfetti(EnemyAI __instance)
        {
            try
            {
                Object.Instantiate(Plugin.confettiPrefab, __instance.transform.position, Quaternion.Euler(0f, 0f, 0f),
                RoundManager.Instance.mapPropsContainer.transform).SetActive(value: true);
            }
            catch
            {
                Debug.LogError("An error has occurred in SpawnConfetti() inside KillEnemy()");
            }
        }
    }
}
