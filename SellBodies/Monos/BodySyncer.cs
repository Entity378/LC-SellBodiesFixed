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
                PhysicsProp prop = GetComponent<PhysicsProp>();
                int priceBase = Random.Range(prop.itemProperties.minValue, prop.itemProperties.maxValue);
                int totalPowerCount;
                float priceMuliplier;
                int price;

                if (Plugin.cfg.DISABLE_MULTIPLIER == false)
                {
                    totalPowerCount = StartOfRound.Instance.currentLevel.maxEnemyPowerCount + StartOfRound.Instance.currentLevel.maxOutsideEnemyPowerCount;
                    priceMuliplier = ((((float)totalPowerCount - Plugin.cfg.MULTIPLIER_POWER_COUNT_SUBTRACTION) / 100f) * Plugin.cfg.MULTIPLIER_VALUE);
                    price = (int)(priceBase + (int)(priceBase * priceMuliplier));
                }
                else
                {
                    price = priceBase;
                }

                SyncDetailsClientRpc(price, new NetworkBehaviourReference(prop));
            }
        }

        [ClientRpc]
        void SyncDetailsClientRpc(int price, NetworkBehaviourReference netRef)
        {
            netRef.TryGet(out PhysicsProp prop);
            if (prop != null)
            {
                prop.scrapValue = price;
                prop.itemProperties.creditsWorth = price;
                prop.GetComponentInChildren<ScanNodeProperties>().subText = $"Value: ${price}";
                Debug.Log("Successfully synced body values");
            }
            else Debug.LogError("Failed to resolve network reference!");
        }
    }
}
