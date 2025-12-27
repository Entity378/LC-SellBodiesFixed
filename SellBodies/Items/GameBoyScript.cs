using UnityEngine;

public class GameBoyScript : GrabbableObject
{
    private GameBoyCartridgeScript insertedCartridge;

    public GameObject gameboyCartrige;
    public GameObject gameboyBackground;

    public Material gameboyCartrigeMat;
    public Material gameboyBackgroundMat;

    public AudioClip gameboyCartridgeSFX;
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

    public override void ItemActivate(bool used, bool buttonDown = true)
    {
        base.ItemActivate(used, buttonDown);
        if (buttonDown && playerHeldBy != null)
        {
            if (hasCartrige)
            {
                if (audio.isPlaying)
                {
                    gameboyBackground.SetActive(false);
                    audio.Pause();
                }
                else
                {
                    gameboyBackground.SetActive(true);
                    audio.UnPause();
                    if (audio.time > 0f)
                    {
                        audio.UnPause();
                    }
                    else
                    {
                        audio.loop = true;
                        audio.clip = gameboyCartridgeSFX;
                        audio.Play();
                    }
                }
            }
        }
    }

    public override void ItemInteractLeftRight(bool right)
    {
        base.ItemInteractLeftRight(right);
        if (right)
        {
            if (!hasCartrige)
            {
                int cartrigeIndex = getCartrigeInventorySlot();
                if (cartrigeIndex != -1)
                {
                    GameBoyCartridgeScript cartridgeInInventory = playerHeldBy.ItemSlots[cartrigeIndex].GetComponent<GameBoyCartridgeScript>();
                    insertedCartridge = cartridgeInInventory;

                    gameboyCartrigeMat = cartridgeInInventory.cartrigeMat;
                    gameboyBackgroundMat.mainTexture = cartridgeInInventory.cartrigeBackground;
                    gameboyCartridgeSFX = cartridgeInInventory.gameboyCartridgeSFX;

                    if (IsOwner)
                    {
                        HUDManager.Instance.itemSlotIcons[cartrigeIndex].enabled = false;
                    }

                    playerHeldBy.ItemSlots[cartrigeIndex] = null;
                    insertedCartridge.gameObject.SetActive(false);
                    gameboyCartrige.SetActive(true);
                    hasCartrige = true;
                }
            }
            else
            {
                gameboyCartrige.SetActive(false);
                gameboyBackground.SetActive(false);
                insertedCartridge.gameObject.SetActive(true);

                audio.loop = false;
                audio.Stop();

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
        }
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
