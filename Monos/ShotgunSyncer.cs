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

            transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 90f);

            if (IsHost || IsServer)
            {
                int price = 60;
                if (KillEnemyServerRpcPatcher.publicShotgunPrice != 0)
                {
                    price = KillEnemyServerRpcPatcher.publicShotgunPrice;
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