using CleaningCompany.Patches;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace CleaningCompany.Monos
{
    internal class MaskSyncer : NetworkBehaviour
    {
        public HauntedMaskItem prop;
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            prop = GetComponent<HauntedMaskItem>();
            if (IsHost || IsServer)
            {
                Quaternion maskRotation = KillEnemyServerRpcPatcher.publicMaskRotation;
                int price = Random.Range(prop.itemProperties.minValue, prop.itemProperties.maxValue);
                SyncDetailsClientRpc(price, maskRotation);
                Debug.Log("End of OnNetworkSpawn mask override");
            }
        }

        [ClientRpc]
        void SyncDetailsClientRpc(int price, Quaternion rot)
        {
            if (prop != null)
            {
                prop.scrapValue = price;
                prop.itemProperties.creditsWorth = price;
                prop.GetComponentInChildren<ScanNodeProperties>().subText = $"Value: ${price}";
                RoundManager.Instance.totalScrapValueInLevel += price;
                Debug.Log("Successfully synced mask values");
                StartCoroutine(RotateMaskClient(prop, rot));
            }
            else Debug.LogError("Failed to resolve network reference!");
        }

        static IEnumerator RotateMaskClient(HauntedMaskItem prop, Quaternion rot)
        {
            yield return new WaitForSeconds(0);
            while (!prop.hasHitGround && prop.playerHeldBy == null)
            {
                Debug.Log("Waiting for the mask to hit the ground");
            }

            if (prop.playerHeldBy == null) 
            {
                Vector3 eulerAngles = rot.eulerAngles;
                eulerAngles.x = 270f;
                Quaternion newRotation = Quaternion.Euler(eulerAngles);
                prop.GetComponent<Transform>().transform.SetPositionAndRotation(prop.transform.position, newRotation);
                KillEnemyServerRpcPatcher.publicBodyRotation = new Quaternion();
            }
        }
    }
}