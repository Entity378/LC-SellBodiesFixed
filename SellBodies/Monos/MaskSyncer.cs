using Unity.Netcode;
using UnityEngine;

namespace SellBodies.Monos
{
    internal class MaskSyncer : SyncScript
    {
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            prop = GetComponent<HauntedMaskItem>();

            transform.rotation = Quaternion.Euler(270f, transform.rotation.eulerAngles.y, 0f);

            if (IsHost || IsServer)
            {
                int price = Random.Range(prop.itemProperties.minValue, prop.itemProperties.maxValue);
                SyncDetailsClientRpc(price);
                Debug.Log("End of OnNetworkSpawn mask override");
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
                RoundManager.Instance.totalScrapValueInLevel += price;
                Debug.Log("Successfully synced mask values");
            }
            else Debug.LogError("Failed to resolve network reference!");
        }
    }
}