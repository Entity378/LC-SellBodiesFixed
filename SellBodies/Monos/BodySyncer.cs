using Unity.Netcode;
using UnityEngine;

namespace SellBodies.Monos
{
    internal class BodySyncer : SyncScript
    {
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            prop = GetComponent<PhysicsProp>();

            transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);

            if (IsHost || IsServer)
            {
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
                SyncDetailsClientRpc(price);
                Debug.Log("End of OnNetworkSpawn body override");
            }
        }

        [ClientRpc]
        void SyncDetailsClientRpc(int price)
        {
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