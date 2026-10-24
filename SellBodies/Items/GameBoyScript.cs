using System.Collections;
using System.Text.RegularExpressions;
using GameNetcodeStuff;
using SellBodies;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;
using UnityEngine.Video;

public class GameBoyScript : GrabbableObject
{
    public GameObject gameboyCartrige;
    public GameObject gameboyBackground;

    public VideoPlayer gameboyVideoPlayer;

    public AudioClip errorSFX;
    public AudioSource audio;

    private Item cartridgeItem;
    private GameBoyCartridgeScript cartridge;
    private string gameboyURL = "";
    private bool isResolvingURL = false;

    private const string YT_API_URL = "https://www.youtube.com/youtubei/v1/player?key=AIzaSyA8eiZmM1FaDVjRy-df2KTyQ_vz_yYM39w&hl=en";
    private const string YT_USER_AGENT = "com.google.android.youtube/20.10.38 (Linux; U; Android 11) gzip";
    private const string YT_SOCS_COOKIE = "SOCS=CAISEwgDEgk4MTM4MzYzNTIaAmVuIAEaBgiApPzGBg";

    public override void Start()
    {
        base.Start();
        SetCartridge(cartridgeItem, gameboyURL);
    }

    public override void Update()
    {
        base.Update();
        if (CanUseKeys())
        {
            SetupKeyCallbacks();
        }
        else
        {
            StopKeyCallbacks();
        }
    }

    private bool CanUseKeys()
    {
        if (!IsOwner || playerHeldBy == null || isPocketed)
        {
            return false;
        }
        if (playerHeldBy.isGrabbingObjectAnimation || playerHeldBy.isTypingChat || playerHeldBy.inTerminalMenu)
        {
            return false;
        }
        if (playerHeldBy.inSpecialInteractAnimation || playerHeldBy.hoveringOverTrigger != null)
        {
            return false;
        }
        return true;
    }

    public override void OnDestroy()
    {
        StopKeyCallbacks();
        base.OnDestroy();
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

    public override int GetItemDataToSave()
    {
        base.GetItemDataToSave();
        if (cartridgeItem == null) return 0;
        return cartridgeItem.itemId;
    }

    public override void LoadItemSaveData(int saveData)
    {
        base.LoadItemSaveData(saveData);
        foreach (Item item in Plugin.instance.GameboyCartridges)
        {
            if (item.itemId == saveData)
            {
                SetCartridge(item, "");
                return;
            }
        }
        SetCartridge(null, "");
    }

    private void SetupKeyCallbacks()
    {
        Plugin.InputActionsInstance.LoadCartridgeKey.performed += LoadCartridge;
        Plugin.InputActionsInstance.VolumeUpKey.performed += VolumeUp;
        Plugin.InputActionsInstance.VolumeDownKey.performed += VolumeDown;
    }

    private void StopKeyCallbacks()
    {
        Plugin.InputActionsInstance.LoadCartridgeKey.performed -= LoadCartridge;
        Plugin.InputActionsInstance.VolumeUpKey.performed -= VolumeUp;
        Plugin.InputActionsInstance.VolumeDownKey.performed -= VolumeDown;
    }

    public override void ItemActivate(bool used, bool buttonDown = true)
    {
        base.ItemActivate(used, buttonDown);
        if (!buttonDown || playerHeldBy == null || cartridgeItem == null) return;

        if (gameboyVideoPlayer.isPlaying)
        {
            gameboyBackground.SetActive(false);
            gameboyVideoPlayer.Pause();
        }
        else if (gameboyVideoPlayer.frame > 0)
        {
            gameboyBackground.SetActive(true);
            gameboyVideoPlayer.Play();
        }
        else if (cartridge.isURLCartridge)
        {
            if (string.IsNullOrEmpty(gameboyURL))
            {
                audio.PlayOneShot(errorSFX);
                return;
            }
            if (isResolvingURL) return;
            StartCoroutine(ResolveAndPlay(gameboyURL));
        }
        else
        {
            gameboyVideoPlayer.clip = cartridge.videoClip;
            gameboyVideoPlayer.frame = 0;
            gameboyVideoPlayer.Play();
            gameboyBackground.SetActive(true);
        }
    }

    private void VolumeUp(InputAction.CallbackContext context)
    {
        SetVolumeServerRpc(audio.volume + 0.1f);
    }

    private void VolumeDown(InputAction.CallbackContext context)
    {
        SetVolumeServerRpc(audio.volume - 0.1f);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetVolumeServerRpc(float volume)
    {
        SetVolumeClientRpc(Mathf.Clamp01(volume));
    }

    [ClientRpc]
    private void SetVolumeClientRpc(float volume)
    {
        audio.volume = volume;
    }

    private void LoadCartridge(InputAction.CallbackContext context)
    {
        if (!CanUseKeys()) return;

        if (cartridgeItem != null)
        {
            EjectCartridgeServerRpc(cartridgeItem.itemName, gameboyURL, (int)playerHeldBy.playerClientId, GetFreeSlot());
            SetCartridge(null, "");
            return;
        }

        int slot = GetCartridgeSlot();
        if (slot == -1) return;

        GameBoyCartridgeScript inserted = playerHeldBy.ItemSlots[slot].GetComponent<GameBoyCartridgeScript>();
        Item item = inserted.itemProperties;
        string url = inserted.URL;

        playerHeldBy.DestroyItemInSlotAndSync(slot);
        SetCartridge(item, url);
        SetCartridgeServerRpc(item.itemName, url);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetCartridgeServerRpc(string cartridgeName, string url)
    {
        SetCartridgeClientRpc(cartridgeName, url);
    }

    [ClientRpc]
    private void SetCartridgeClientRpc(string cartridgeName, string url)
    {
        if (IsOwner) return;
        SetCartridge(FindCartridge(cartridgeName), url);
    }

    private void SetCartridge(Item item, string url)
    {
        cartridgeItem = item;
        cartridge = null;
        if (item != null)
        {
            cartridge = item.spawnPrefab.GetComponent<GameBoyCartridgeScript>();
        }
        gameboyURL = url;

        gameboyVideoPlayer.Stop();
        gameboyBackground.SetActive(false);
        gameboyCartrige.SetActive(item != null);
        if (item == null) return;

        if (cartridge.isURLCartridge)
        {
            gameboyVideoPlayer.source = VideoSource.Url;
        }
        else
        {
            gameboyVideoPlayer.source = VideoSource.VideoClip;
        }

        foreach (MeshRenderer meshRenderer in gameboyCartrige.GetComponentsInChildren<MeshRenderer>())
        {
            meshRenderer.material = cartridge.cartrigeMat;
        }
    }

    private Item FindCartridge(string cartridgeName)
    {
        foreach (Item item in Plugin.instance.GameboyCartridges)
        {
            if (item.itemName == cartridgeName)
            {
                return item;
            }
        }
        return null;
    }

    private int GetCartridgeSlot()
    {
        for (int i = 0; i < playerHeldBy.ItemSlots.Length; i++)
        {
            if (playerHeldBy.ItemSlots[i] != null && playerHeldBy.ItemSlots[i].GetComponent<GameBoyCartridgeScript>() != null)
            {
                return i;
            }
        }
        return -1;
    }

    private int GetFreeSlot()
    {
        for (int i = 0; i < playerHeldBy.ItemSlots.Length; i++)
        {
            if (playerHeldBy.ItemSlots[i] == null)
            {
                return i;
            }
        }
        return -1;
    }

    [ServerRpc(RequireOwnership = false)]
    private void EjectCartridgeServerRpc(string cartridgeName, string url, int playerId, int slot)
    {
        GameObject ejectedObject = Instantiate(FindCartridge(cartridgeName).spawnPrefab, transform.position, Quaternion.identity);
        NetworkObject ejected = ejectedObject.GetComponent<NetworkObject>();
        ejected.Spawn();
        if (slot != -1)
        {
            ejected.ChangeOwnership(StartOfRound.Instance.allPlayerScripts[playerId].actualClientId);
        }
        EjectCartridgeClientRpc(ejected, url, playerId, slot);
    }

    [ClientRpc]
    private void EjectCartridgeClientRpc(NetworkObjectReference ejectedRef, string url, int playerId, int slot)
    {
        if (!IsOwner)
        {
            SetCartridge(null, "");
        }
        StartCoroutine(PlaceEjectedCartridge(ejectedRef, url, StartOfRound.Instance.allPlayerScripts[playerId], slot));
    }

    private IEnumerator PlaceEjectedCartridge(NetworkObjectReference ejectedRef, string url, PlayerControllerB player, int slot)
    {
        yield return null;
        if (!ejectedRef.TryGet(out NetworkObject networkObject)) yield break;

        GameBoyCartridgeScript ejected = networkObject.GetComponent<GameBoyCartridgeScript>();
        ejected.URL = url;
        ejected.isInElevator = isInElevator;
        ejected.isInShipRoom = isInShipRoom;
        ejected.isInFactory = isInFactory;

        if (slot == -1 || player.ItemSlots[slot] != null)
        {
            DropEjectedCartridge(ejected);
        }
        else
        {
            PutEjectedCartridgeInSlot(ejected, player, slot);
        }
    }

    private void DropEjectedCartridge(GameBoyCartridgeScript ejected)
    {
        if (isInElevator)
        {
            ejected.transform.SetParent(StartOfRound.Instance.elevatorTransform, true);
        }
        else
        {
            ejected.transform.SetParent(StartOfRound.Instance.propsContainer, true);
        }
        ejected.startFallingPosition = ejected.transform.localPosition;
        ejected.FallToGround();
    }

    private void PutEjectedCartridgeInSlot(GameBoyCartridgeScript ejected, PlayerControllerB player, int slot)
    {
        ejected.playerHeldBy = player;
        if (player.IsOwner)
        {
            ejected.parentObject = player.localItemHolder;
        }
        else
        {
            ejected.parentObject = player.serverItemHolder;
        }
        ejected.heldByPlayerOnServer = true;
        ejected.isHeld = true;
        ejected.isPocketed = true;
        ejected.EnablePhysics(false);
        ejected.EnableItemMeshes(false);
        player.ItemSlots[slot] = ejected;

        if (player.IsOwner)
        {
            HUDManager.Instance.itemSlotIcons[slot].sprite = ejected.itemProperties.itemIcon;
            HUDManager.Instance.itemSlotIcons[slot].enabled = true;
        }
    }

    private IEnumerator ResolveAndPlay(string url)
    {
        isResolvingURL = true;

        string directUrl = url;

        if (IsYouTubeURL(url))
        {
            string videoId = ExtractYouTubeVideoId(url);
            if (videoId == null)
            {
                Debug.LogError("[GameBoy] Could not extract YouTube video ID from: " + url);
                audio.PlayOneShot(errorSFX);
                isResolvingURL = false;
                yield break;
            }

            directUrl = null;

            using (UnityWebRequest request = CreateYouTubeRequest(videoId))
            {
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                    directUrl = ExtractStreamURL(request.downloadHandler.text);
            }

            if (directUrl == null)
            {
                Debug.LogError("[GameBoy] Could not resolve YouTube video: " + videoId);
                audio.PlayOneShot(errorSFX);
                isResolvingURL = false;
                yield break;
            }
        }

        isResolvingURL = false;

        if (cartridge == null || !cartridge.isURLCartridge)
            yield break;

        gameboyVideoPlayer.url = directUrl;
        gameboyVideoPlayer.frame = 0;
        gameboyVideoPlayer.Prepare();

        while (!gameboyVideoPlayer.isPrepared)
        {
            if (cartridge == null)
                yield break;
            yield return null;
        }

        gameboyVideoPlayer.Play();
        gameboyBackground.SetActive(true);
    }

    private static UnityWebRequest CreateYouTubeRequest(string videoId)
    {
        string body = "{\"videoId\":\"" + videoId + "\",\"context\":{\"client\":{\"clientName\":\"ANDROID\",\"clientVersion\":\"20.10.38\",\"androidSdkVersion\":30,\"osName\":\"Android\",\"osVersion\":\"11\",\"hl\":\"en\",\"gl\":\"US\"}},\"contentCheckOk\":true,\"racyCheckOk\":true}";

        var request = new UnityWebRequest(YT_API_URL, "POST");
        request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(body));
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("User-Agent", YT_USER_AGENT);
        request.SetRequestHeader("Cookie", YT_SOCS_COOKIE);
        request.timeout = 15;
        return request;
    }

    private static bool IsYouTubeURL(string url)
    {
        return url.Contains("youtube.com") || url.Contains("youtu.be");
    }

    private static string ExtractYouTubeVideoId(string url)
    {
        var match = Regex.Match(url, @"[?&]v=([a-zA-Z0-9_-]{11})");
        if (match.Success) return match.Groups[1].Value;

        match = Regex.Match(url, @"youtu\.be/([a-zA-Z0-9_-]{11})");
        if (match.Success) return match.Groups[1].Value;

        match = Regex.Match(url, @"youtube\.com/embed/([a-zA-Z0-9_-]{11})");
        if (match.Success) return match.Groups[1].Value;

        return null;
    }

    private static string ExtractStreamURL(string json)
    {
        var matches = Regex.Matches(json, @"""url""\s*:\s*""(https://[^""]*googlevideo\.com/videoplayback[^""]+)""");

        string bestUrl = null;
        int bestPriority = int.MaxValue;

        foreach (Match m in matches)
        {
            string streamUrl = m.Groups[1].Value.Replace("\\u0026", "&");
            streamUrl = Regex.Unescape(streamUrl);

            if (!streamUrl.Contains("mime=video%2Fmp4") && !streamUrl.Contains("mime=video/mp4"))
                continue;

            var itagMatch = Regex.Match(streamUrl, @"itag=(\d+)");
            int itag = itagMatch.Success ? int.Parse(itagMatch.Groups[1].Value) : 0;

            // itag 18 = 360p mp4 (priority 0), itag 22 = 720p mp4 (priority 1), other = priority 2
            int priority = itag == 18 ? 0 : itag == 22 ? 1 : 2;
            if (priority < bestPriority)
            {
                bestUrl = streamUrl;
                bestPriority = priority;
            }
        }

        if (bestUrl != null) return bestUrl;

        // Fallback
        if (matches.Count > 0)
        {
            string streamUrl = matches[0].Groups[1].Value.Replace("\\u0026", "&");
            return Regex.Unescape(streamUrl);
        }

        return null;
    }
}
