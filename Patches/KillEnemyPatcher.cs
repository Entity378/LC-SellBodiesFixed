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
            string name = __instance.enemyType.enemyName;
            if (Plugin.instance.BodySpawns.ContainsKey(name))
            {
                __instance.StartCoroutine(MoveOldBody(__instance));

            }
            else if (Plugin.cfg.MODDEDENEMY && !Plugin.instance.VanillaBody.Contains(name))
            {
                __instance.StartCoroutine(MoveOldBody(__instance));
            }
            SpawnConfetti(__instance);
        }

        static IEnumerator MoveOldBody(EnemyAI __instance)
        {
            yield return new WaitForSeconds(4);

            Vector3 newBodyPos = new Vector3(-10000, -10000, -10000);
            __instance.transform.position = newBodyPos;
            __instance.SyncPositionToClients();
        }

        static void SpawnConfetti(EnemyAI __instance)
        {
            if (Plugin.cfg.CONFETTI)
            {
                try 
                {
                    Object.Instantiate(Plugin.confettiPrefab, __instance.transform.position, Quaternion.Euler(0f, 0f, 0f),
                    RoundManager.Instance.mapPropsContainer.transform).SetActive(value: true);
                }
                catch{ }

                if (Plugin.cfg.YIPPEE)
                {
                    __instance.GetComponent<AudioSource>().PlayOneShot(Plugin.Yippee);
                }
                else
                {
                    __instance.GetComponent<AudioSource>().PlayOneShot(Plugin.Cheer);
                }
            }
        }
    }
}
