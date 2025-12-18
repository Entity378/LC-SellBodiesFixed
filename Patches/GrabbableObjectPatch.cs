using HarmonyLib;
using SellBodies.Monos;
using UnityEngine;

namespace SellBodies.Patches
{
    [HarmonyPatch(typeof(GrabbableObject))]
    internal class GrabbableObjectPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("FallToGround")]
        static void FallToGroundPatch(ref GrabbableObject __instance)
        {
            if (__instance.GetComponent<SyncScript>() != null)
            {
                __instance.fallTime = -378;
                __instance.reachedFloorTarget = true;
            }
            return;
        }

        [HarmonyPrefix]
        [HarmonyPatch("FallWithCurve")]
        static bool FallWithCurvePatch(ref GrabbableObject __instance)
        {
            if (__instance.GetComponent<BodySyncer>() != null)
            {
                if (__instance.GetComponent<BodySyncer>().justSpawned || __instance.fallTime == -378)
                {
                    __instance.fallTime = 1;
                    __instance.reachedFloorTarget = true;
                    __instance.transform.localPosition = __instance.targetFloorPosition;
                    __instance.GetComponent<BodySyncer>().justSpawned = false;
                    return false;
                }
            }
            else if (__instance.GetComponent<MaskSyncer>() != null) 
            {
                if (__instance.GetComponent<MaskSyncer>().justSpawned) 
                {
                    float num = __instance.startFallingPosition.y - __instance.targetFloorPosition.y;
                    float fallTimeOffset = 378;

                    if (num > 5f)
                    {
                        __instance.transform.localPosition = Vector3.Lerp(__instance.startFallingPosition, __instance.targetFloorPosition, StartOfRound.Instance.objectFallToGroundCurveNoBounce.Evaluate(__instance.fallTime + fallTimeOffset));
                    }
                    else
                    {
                        __instance.transform.localPosition = Vector3.Lerp(__instance.startFallingPosition, __instance.targetFloorPosition, StartOfRound.Instance.objectFallToGroundCurve.Evaluate(__instance.fallTime + fallTimeOffset));
                    }

                    __instance.fallTime += Mathf.Abs(Time.deltaTime * 6f / num);

                    if (__instance.fallTime >= -377 && !__instance.reachedFloorTarget)
                    {
                        __instance.fallTime = 1;
                        __instance.reachedFloorTarget = true;
                        __instance.transform.localPosition = __instance.targetFloorPosition;
                        __instance.GetComponent<MaskSyncer>().justSpawned = false;
                    }
                    return false;
                }
            }
            else if (__instance.GetComponent<ShotgunSyncer>() != null)
            {
                if (__instance.GetComponent<ShotgunSyncer>().justSpawned) 
                {
                    float num = __instance.startFallingPosition.y - __instance.targetFloorPosition.y;
                    float fallTimeOffset = 378;

                    if (num > 5f)
                    {
                        __instance.transform.localPosition = Vector3.Lerp(__instance.startFallingPosition, __instance.targetFloorPosition, StartOfRound.Instance.objectFallToGroundCurveNoBounce.Evaluate(__instance.fallTime + fallTimeOffset));
                    }
                    else
                    {
                        __instance.transform.localPosition = Vector3.Lerp(__instance.startFallingPosition, __instance.targetFloorPosition, StartOfRound.Instance.objectFallToGroundCurve.Evaluate(__instance.fallTime + fallTimeOffset));
                    }

                    __instance.fallTime += Mathf.Abs(Time.deltaTime * 6f / num);

                    if (__instance.fallTime >= -377 && !__instance.reachedFloorTarget)
                    {
                        __instance.fallTime = 1;
                        __instance.reachedFloorTarget = true;
                        __instance.transform.localPosition = __instance.targetFloorPosition;
                        __instance.GetComponent<ShotgunSyncer>().justSpawned = false;
                    }
                    return false;
                }
            }
            return true;
        }
    }
}