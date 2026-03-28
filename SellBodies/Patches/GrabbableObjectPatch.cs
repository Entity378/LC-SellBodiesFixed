using HarmonyLib;
using SellBodies.Monos;

namespace SellBodies.Patches
{
    [HarmonyPatch(typeof(GrabbableObject))]
    internal class GrabbableObjectPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("LateUpdate")]
        static void LateUpdatePatch(ref GrabbableObject __instance)
        {
            SyncScript itemSyncScript = __instance.GetComponent<SyncScript>();
            if (itemSyncScript != null)
            {
                GrabbableObject gObjectScript = __instance.GetComponent<GrabbableObject>();
                if (itemSyncScript.justSpawned && gObjectScript.hasHitGround) 
                {
                    gObjectScript.gameObject.transform.rotation = itemSyncScript.spawnRotation;
                    itemSyncScript.justSpawned = false;
                }
            }
            return;
        }
    }
}