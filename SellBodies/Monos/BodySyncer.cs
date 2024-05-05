using CleaningCompany.Patches;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace CleaningCompany.Monos
{
    internal class BodySyncer : NetworkBehaviour
    {
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (IsHost || IsServer)
            {
                Quaternion bodyRotation = KillEnemyServerRpcPatcher.publicBodyRotation;
                PhysicsProp prop = GetComponent<PhysicsProp>();
                int priceBase = Random.Range(prop.itemProperties.minValue, prop.itemProperties.maxValue);
                int totalPowerCount;
                float priceMuliplier;
                int price;

                if (Plugin.cfg.DISABLE_MULTIPLIER == false)
                {
                    totalPowerCount = StartOfRound.Instance.currentLevel.maxEnemyPowerCount + StartOfRound.Instance.currentLevel.maxOutsideEnemyPowerCount;
                    priceMuliplier = (totalPowerCount - Plugin.cfg.MULTIPLIER_POWER_COUNT_SUBTRACTION) / 100f * Plugin.cfg.MULTIPLIER_VALUE;
                    price = priceBase + (int)(priceBase * priceMuliplier);
                }
                else
                {
                    price = priceBase;
                }
                SyncDetailsClientRpc(price, bodyRotation, new NetworkBehaviourReference(prop));
            }
        }

        [ClientRpc]
        void SyncDetailsClientRpc(int price, Quaternion rot, NetworkBehaviourReference netRef)
        {
            netRef.TryGet(out PhysicsProp prop);
            if (prop != null)
            {
                prop.scrapValue = price;
                prop.itemProperties.creditsWorth = price;
                prop.GetComponentInChildren<ScanNodeProperties>().subText = $"Value: ${price}";
                Debug.Log("Successfully synced body values");
                StartCoroutine(RotateBodyClient(prop, rot));
            }
            else Debug.LogError("Failed to resolve network reference!");
        }

        static IEnumerator RotateBodyClient(PhysicsProp prop, Quaternion rot)
        {
            yield return new WaitForSeconds(0);
            while (!prop.hasHitGround)
            {
                Debug.Log("Waiting for the body to hit the ground");
            }
            prop.GetComponent<Transform>().transform.SetPositionAndRotation(prop.transform.position, rot);
        }
    }
}
