using Unity.Netcode;

namespace SellBodies.Monos
{
    public abstract class SyncScript : NetworkBehaviour
    {
        public GrabbableObject prop;
        public bool justSpawned = true;
    }
}