using HarmonyLib;

namespace CleaningCompany.Patches
{
    [HarmonyPatch(typeof(MaskedPlayerEnemy))]
    internal class KillMaskedPatcher
    {
        [HarmonyPostfix]
        [HarmonyPatch("KillEnemy")]
        static void DropMaskOnDeath(MaskedPlayerEnemy __instance)
        {
            if (Plugin.cfg.MASKED && __instance != null) 
            {
                if (__instance.mimickingPlayer != null && __instance.mimickingPlayer.deadBody != null && __instance.mimickingPlayer.isPlayerDead)
                {
                    // Disable enemy model if this was a corrupted player
                    //__instance.gameObject.SetActive(false);

                    // Reenable player ragdoll
                    var deadBody = __instance.mimickingPlayer.deadBody;
                    //deadBody.gameObject.SetActive(true);
                    //deadBody.SetBodyPartsKinematic(false);
                    //deadBody.SetRagdollPositionSafely(__instance.transform.position);
                    //deadBody.deactivated = false;

                    // Hide mask on player ragdoll
                    deadBody.transform.Find("spine.001/spine.002/spine.003/spine.004/HeadMask").gameObject.SetActive(false);
                }
                __instance.gameObject.transform.Find("ScavengerModel/metarig/spine/spine.001/spine.002/spine.003/spine.004/HeadMaskComedy").gameObject.SetActive(false);
                __instance.gameObject.transform.Find("ScavengerModel/metarig/spine/spine.001/spine.002/spine.003/spine.004/HeadMaskTragedy").gameObject.SetActive(false);
            }
        }
    }
}