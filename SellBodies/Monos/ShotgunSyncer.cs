using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace CleaningCompany.Monos
{
    internal class ShotgunSyncer : NetworkBehaviour
    {
        public override void OnNetworkSpawn()
        {
            if (IsHost || IsServer)
            {
                StartCoroutine(WaitToSync());
            }
            base.OnNetworkSpawn();
        }

        private IEnumerator WaitToSync()
        {
            yield return new WaitForSeconds(1f);
            ShotgunItem prop = GetComponent<ShotgunItem>();
            int price = Random.Range(30, 90);
            int ammo = 2;
            SyncDetailsClientRpc(price, ammo, new NetworkBehaviourReference(prop));
        }

        [ClientRpc]
        void SyncDetailsClientRpc(int price, int ammo, NetworkBehaviourReference netRef)
        {
            netRef.TryGet(out ShotgunItem prop);
            if (prop != null)
            {
                prop.shellsLoaded = ammo;
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