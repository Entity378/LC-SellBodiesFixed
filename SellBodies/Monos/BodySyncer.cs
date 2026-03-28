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

            spawnRotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
            transform.rotation = spawnRotation;

            if (IsHost || IsServer)
            {
                int price;
                int priceBase = Random.Range(prop.itemProperties.minValue, prop.itemProperties.maxValue);

                if (Plugin.cfg.MULTIPLIER)
                {
                    float powerFactor = 1;
                    float vanillaFactor = 1;
                    float baseFactor = Plugin.cfg.MULTIPLIER_VALUE;

                    if (Plugin.cfg.ENEMY_MULTIPLIER) 
                    {
                        SelectableLevel currentLevel = StartOfRound.Instance.currentLevel;
                        int totalPowerCount = currentLevel.maxEnemyPowerCount + currentLevel.maxOutsideEnemyPowerCount;
                        powerFactor = 1 + (totalPowerCount - 10) / 100f;
                    }

                    if (Plugin.cfg.VANILLA_MULTIPLIER)
                    {
                        float vanillaMultiplier = RoundManager.Instance.scrapValueMultiplier;
                        vanillaFactor = vanillaMultiplier * 2.5f;
                    }

                    float priceMultiplier = powerFactor * vanillaFactor * baseFactor;
                    price = (int)(priceBase * priceMultiplier);
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
