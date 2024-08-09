using CleaningCompany.Patches;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace CleaningCompany.Monos
{
    internal class ShotgunSyncer : NetworkBehaviour
    {
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (IsHost || IsServer)
            {
                Quaternion shotgunRotation = KillEnemyServerRpcPatcher.publicShotgunRotation;
                ShotgunItem prop = GetComponent<ShotgunItem>();
                int price = KillEnemyServerRpcPatcher.publicShotgunPrice;
                int ammo = 2;
                SyncDetailsClientRpc(price, shotgunRotation, ammo, new NetworkBehaviourReference(prop));
                Debug.Log("End of OnNetworkSpawn shotgun override");
            }
        }

        [ClientRpc]
        void SyncDetailsClientRpc(int price, Quaternion rot, int ammo, NetworkBehaviourReference netRef)
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
                StartCoroutine(RotateShotgunClient(prop, rot));
            }
            else Debug.LogError("Failed to resolve network reference!");
        }

        static IEnumerator RotateShotgunClient(ShotgunItem prop, Quaternion rot)
        {
            yield return new WaitForSeconds(0);
            while (!prop.hasHitGround || prop.playerHeldBy != null)
            {
                Debug.Log("Waiting for the body to hit the ground");
            }

            if (prop.playerHeldBy != null)
            {
                prop.GetComponent<Transform>().transform.SetPositionAndRotation(prop.transform.position, rot);
                KillEnemyServerRpcPatcher.publicBodyRotation = new Quaternion();
            }
        }
    }
}
