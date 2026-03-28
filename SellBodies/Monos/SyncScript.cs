using Unity.Netcode;
using UnityEngine;

namespace SellBodies.Monos
{
    public abstract class SyncScript : NetworkBehaviour
    {
        public GrabbableObject prop;
        public bool justSpawned = true;
        public Quaternion spawnRotation;
    }
}