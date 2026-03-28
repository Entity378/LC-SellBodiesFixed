using SellBodies;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Video;

public class GameBoyScript : GrabbableObject
{
    private GameBoyCartridgeScript insertedCartridge;

    public GameObject gameboyCartrige;
    public GameObject gameboyBackground;

    public Material gameboyCartrigeMat;

    public VideoPlayer gameboyVideoPlayer;
    public VideoClip gameboyVideoClip;

    public AudioClip errorSFX;
    public AudioClip useButtonSFX;
    public AudioClip dropSFX;
    public AudioClip grabSFX;
    public AudioSource audio;

    private bool hasCartrige = false;

    public override void Start()
    {
        base.Start();
        if (!hasCartrige) 
        {
            gameboyCartrige.SetActive(false);
            gameboyBackground.SetActive(false);
        }
    }
    public override void Update()
    {
        base.Update();
        if (playerHeldBy != null && !isPocketed) 
        {
            if (!playerHeldBy.isGrabbingObjectAnimation && 
                !playerHeldBy.isTypingChat && 
                !playerHeldBy.inTerminalMenu && 
                !playerHeldBy.inSpecialInteractAnimation && 
                playerHeldBy.hoveringOverTrigger == null)
            {
                SetupKeyCallbacks();
                return;
            }
        }
        StopKeyCallbacks();
    }

    public override void GrabItem()
    {
        base.GrabItem();
        playerHeldBy.equippedUsableItemQE = true;
    }

    public override void EquipItem()
    {
        base.EquipItem();
        playerHeldBy.equippedUsableItemQE = true;
    }

    public override void DiscardItem()
    {
        playerHeldBy.equippedUsableItemQE = false;
        base.DiscardItem();
    }

    public override void PocketItem()
    {
        playerHeldBy.equippedUsableItemQE = false;
        base.PocketItem();
    }
    public void SetupKeyCallbacks()
    {
        Plugin.InputActionsInstance.LoadCartridgeKey.performed += LoadCartridge;
        Plugin.InputActionsInstance.VolumeUpKey.performed += VolumeUp;
        Plugin.InputActionsInstance.VolumeDownKey.performed += VolumeDown;
    }

    public void StopKeyCallbacks()
    {
        Plugin.InputActionsInstance.LoadCartridgeKey.performed -= LoadCartridge;
        Plugin.InputActionsInstance.VolumeUpKey.performed -= VolumeUp;
        Plugin.InputActionsInstance.VolumeDownKey.performed -= VolumeDown;
    }


    public override void ItemActivate(bool used, bool buttonDown = true)
    {
        base.ItemActivate(used, buttonDown);
        if (buttonDown && playerHeldBy != null)
        {
            if (hasCartrige)
            {
                if (gameboyVideoPlayer.isPlaying)
                {
                    gameboyBackground.SetActive(false);
                    gameboyVideoPlayer.Pause();
                }
                else
                {
                    gameboyBackground.SetActive(true);
                    if (gameboyVideoPlayer.frame > 0)
                    {
                        gameboyVideoPlayer.Play();
                    }
                    else
                    {
                        gameboyVideoPlayer.clip = gameboyVideoClip;
                        gameboyVideoPlayer.frame = 0;
                        gameboyVideoPlayer.Play();
                    }
                }
            }
        }
    }

    public void VolumeUp(InputAction.CallbackContext VolumeUpContext)
    {
        if (!VolumeUpContext.performed || playerHeldBy == null) return;
        audio.volume = Mathf.Clamp(audio.volume + 0.1f, 0f, 1f);
        VolumeUpServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void VolumeUpServerRpc()
    {
        VolumeUpClientRpc();
    }

    [ClientRpc]
    private void VolumeUpClientRpc()
    {
        if (IsOwner) return;
        audio.volume = Mathf.Clamp(audio.volume + 0.1f, 0f, 1f);
    }

    public void VolumeDown(InputAction.CallbackContext VolumeDownContext)
    {
        if (!VolumeDownContext.performed || playerHeldBy == null) return;
        audio.volume = Mathf.Clamp(audio.volume - 0.1f, 0f, 1f);
        VolumeDownServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void VolumeDownServerRpc()
    {
        VolumeDownClientRpc();
    }

    [ClientRpc]
    private void VolumeDownClientRpc()
    {
        if (IsOwner) return;
        audio.volume = Mathf.Clamp(audio.volume - 0.1f, 0f, 1f);
    }

    public void LoadCartridge(InputAction.CallbackContext LoadCartridgeContext)
    {
        if (!LoadCartridgeContext.performed || playerHeldBy == null) return;

        if (!hasCartrige)
        {
            int cartrigeIndex = getCartrigeInventorySlot();
            if (cartrigeIndex != -1)
            {
                ExecuteLoadCartridge(cartrigeIndex);
                LoadCartridgeServerRpc(cartrigeIndex);
            }
        }
        else
        {
            ExecuteUnloadCartridge();
            UnloadCartridgeServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void LoadCartridgeServerRpc(int cartrigeIndex)
    {
        LoadCartridgeClientRpc(cartrigeIndex);
    }

    [ClientRpc]
    private void LoadCartridgeClientRpc(int cartrigeIndex)
    {
        if (IsOwner) return;
        ExecuteLoadCartridge(cartrigeIndex);
    }

    private void ExecuteLoadCartridge(int cartrigeIndex)
    {
        if (playerHeldBy == null) return;

        GameBoyCartridgeScript cartridgeInInventory = playerHeldBy.ItemSlots[cartrigeIndex].GetComponent<GameBoyCartridgeScript>();
        insertedCartridge = cartridgeInInventory;

        gameboyCartrigeMat = cartridgeInInventory.cartrigeMat;
        gameboyVideoClip = cartridgeInInventory.videoClip;

        var renderers = gameboyCartrige.GetComponentsInChildren<MeshRenderer>();

        foreach (var r in renderers)
        {
            r.material = gameboyCartrigeMat;
        }

        if (IsOwner)
        {
            HUDManager.Instance.itemSlotIcons[cartrigeIndex].enabled = false;
        }

        playerHeldBy.ItemSlots[cartrigeIndex] = null;
        insertedCartridge.gameObject.SetActive(false);
        gameboyCartrige.SetActive(true);
        hasCartrige = true;
    }

    [ServerRpc(RequireOwnership = false)]
    private void UnloadCartridgeServerRpc()
    {
        UnloadCartridgeClientRpc();
    }

    [ClientRpc]
    private void UnloadCartridgeClientRpc()
    {
        if (IsOwner) return;
        ExecuteUnloadCartridge();
    }

    private void ExecuteUnloadCartridge()
    {
        if (playerHeldBy == null) return;

        gameboyCartrige.SetActive(false);
        gameboyBackground.SetActive(false);
        insertedCartridge.gameObject.SetActive(true);

        gameboyVideoPlayer.Stop();

        int freeSlot = GetFreeInventorySlot();

        if (freeSlot != -1)
        {
            playerHeldBy.ItemSlots[freeSlot] = insertedCartridge;
            if (IsOwner)
            {
                HUDManager.Instance.itemSlotIcons[freeSlot].enabled = true;
            }
        }
        else
        {
            DropItem(insertedCartridge);
        }

        insertedCartridge = null;
        hasCartrige = false;
    }

    public int getCartrigeInventorySlot()
    {
        for (int i = 0; i < playerHeldBy.ItemSlots.Length; i++)
        {
            if (playerHeldBy.ItemSlots[i] != null)
            {
                GameBoyCartridgeScript cartridge = playerHeldBy.ItemSlots[i].GetComponent<GameBoyCartridgeScript>();
                if (cartridge != null)
                {
                    return i;
                }
            }
        }
        return -1;
    }

    public int GetFreeInventorySlot()
    {
        for (int i = 0; i < playerHeldBy.ItemSlots.Length; i++)
        {
            if (playerHeldBy.ItemSlots[i] == null)
                return i;
        }
        return -1;
    }

    private void DropItem(GrabbableObject item)
    {
        if (item == null || playerHeldBy == null) return;

        item.parentObject = null;
        item.heldByPlayerOnServer = false;

        if (playerHeldBy.isInElevator)
            item.transform.SetParent(playerHeldBy.playersManager.elevatorTransform, true);
        else
            item.transform.SetParent(playerHeldBy.playersManager.propsContainer, true);

        playerHeldBy.SetItemInElevator(playerHeldBy.isInHangarShipRoom, playerHeldBy.isInElevator, item);

        item.EnablePhysics(true);
        item.EnableItemMeshes(true);
        item.transform.localScale = item.originalScale;
        item.isHeld = false;
        item.isPocketed = false;

        item.startFallingPosition = item.transform.parent.InverseTransformPoint(transform.position - Vector3.downVector);
        item.FallToGround(true, false, default(Vector3));
        item.fallTime = Random.Range(-0.3f, 0.05f);

        item.DiscardItem();

        item.playerHeldBy = null;
    }
}
