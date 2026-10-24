using SellBodies.Patches;
using Unity.Netcode;
using UnityEngine;

namespace SellBodies.Monos
{
    internal class ShotgunSyncer : SyncScript
    {
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            prop = GetComponent<ShotgunItem>();

            spawnRotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 90f);
            transform.rotation = spawnRotation;

            if ((IsHost || IsServer) && !prop.scrapPersistedThroughRounds)
            {
                int price;
                int priceBase = 60;

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

                int ammo = 2;
                SyncDetailsClientRpc(price, ammo);
                Debug.Log("End of OnNetworkSpawn shotgun override");
            }
        }

        [ClientRpc]
        void SyncDetailsClientRpc(int price, int ammo)
        {
            if (prop != null)
            {
                prop.GetComponent<ShotgunItem>().shellsLoaded = ammo;
                prop.scrapValue = price;
                prop.itemProperties.creditsWorth = price;
                prop.GetComponentInChildren<ScanNodeProperties>().subText = $"Value: ${price}";
                RoundManager.Instance.totalScrapValueInLevel += price;
                Debug.Log("Successfully synced shotgun values");
            }
            else Debug.LogError("Failed to resolve network reference!");
        }
    }
}