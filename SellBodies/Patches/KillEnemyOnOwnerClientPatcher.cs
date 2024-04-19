using HarmonyLib;
using UnityEngine;
using System.Collections;

namespace CleaningCompany.Patches
{
    [HarmonyPatch(typeof(EnemyAI))]
    internal class EnemyAIPosPatcher
    {
        [HarmonyPrefix]
        [HarmonyPatch("KillEnemyOnOwnerClient")]
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
        }

        static IEnumerator MoveOldBody(EnemyAI __instance)
        {
            yield return new WaitForSeconds(4);

            Vector3 OriginalBodyPos = new Vector3(-10000, -10000, -10000);
            __instance.transform.position = OriginalBodyPos;
            __instance.SyncPositionToClients();
        }
    }
}
