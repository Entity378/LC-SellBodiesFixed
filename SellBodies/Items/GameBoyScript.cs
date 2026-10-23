using System.Collections;
using System.Text.RegularExpressions;
using SellBodies;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;
using UnityEngine.Video;

public class GameBoyScript : GrabbableObject
{
    private GameBoyCartridgeScript insertedCartridge;

    public GameObject gameboyCartrige;
    public GameObject gameboyBackground;

    public Material gameboyCartrigeMat;

    public VideoPlayer gameboyVideoPlayer;
    public VideoClip gameboyVideoClip;
    public string gameboyURL;

    public AudioClip errorSFX;
    public AudioClip useButtonSFX;
    public AudioClip dropSFX;
    public AudioClip grabSFX;
    public AudioSource audio;

    private bool hasCartrige = false;
    private bool isResolvingURL = false;

    private const string YT_API_URL = "https://www.youtube.com/youtubei/v1/player?key=AIzaSyA8eiZmM1FaDVjRy-df2KTyQ_vz_yYM39w&hl=en";
    private const string YT_USER_AGENT = "com.google.android.apps.youtube.vr.oculus/1.60.19 (Linux; U; Android 12L; Quest 3 Build/SQ3A.220605.009.A1) gzip";
    private const string YT_SOCS_COOKIE = "SOCS=CAISEwgDEgk4MTM4MzYzNTIaAmVuIAEaBgiApPzGBg";

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
        if (!buttonDown || playerHeldBy == null || !hasCartrige) return;

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
        else if (insertedCartridge.isURLCartridge)
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
            gameboyVideoPlayer.clip = gameboyVideoClip;
            gameboyVideoPlayer.frame = 0;
            gameboyVideoPlayer.Play();
            gameboyBackground.SetActive(true);
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
                GameBoyCartridgeScript cartridgeInInventory = playerHeldBy.ItemSlots[cartrigeIndex].GetComponent<GameBoyCartridgeScript>();
                gameboyURL = "";

                if (cartridgeInInventory)
                {
                    gameboyURL = cartridgeInInventory.URL;
                }

                ExecuteLoadCartridge(cartrigeIndex, gameboyURL);
                LoadCartridgeServerRpc(cartrigeIndex, gameboyURL);
            }
        }
        else
        {
            ExecuteUnloadCartridge();
            UnloadCartridgeServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void LoadCartridgeServerRpc(int cartrigeIndex, string url)
    {
        LoadCartridgeClientRpc(cartrigeIndex, url);
    }

    [ClientRpc]
    private void LoadCartridgeClientRpc(int cartrigeIndex, string url)
    {
        if (IsOwner) return;
        ExecuteLoadCartridge(cartrigeIndex, url);
    }

    private void ExecuteLoadCartridge(int cartrigeIndex, string url = "")
    {
        if (playerHeldBy == null) return;

        GameBoyCartridgeScript cartridgeInInventory = playerHeldBy.ItemSlots[cartrigeIndex].GetComponent<GameBoyCartridgeScript>();
        insertedCartridge = cartridgeInInventory;

        gameboyCartrigeMat = cartridgeInInventory.cartrigeMat;

        if (cartridgeInInventory.isURLCartridge)
        {
            gameboyVideoPlayer.source = VideoSource.Url;
            gameboyURL = url;
        }
        else
        {
            gameboyVideoPlayer.source = VideoSource.VideoClip;
            gameboyVideoClip = cartridgeInInventory.videoClip;
            gameboyURL = "";
        }

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

            string visitorData = "";

            using (UnityWebRequest initReq = CreateYouTubeRequest(videoId, null))
            {
                yield return initReq.SendWebRequest();

                string initJson = initReq.downloadHandler?.text ?? "";
                var vdMatch = Regex.Match(initJson, @"""visitorData""\s*:\s*""([^""]+)""");
                if (vdMatch.Success)
                    visitorData = vdMatch.Groups[1].Value;

                directUrl = ExtractStreamURL(initJson);
            }

            if (directUrl == null && !string.IsNullOrEmpty(visitorData))
            {
                using (UnityWebRequest retryReq = CreateYouTubeRequest(videoId, visitorData))
                {
                    yield return retryReq.SendWebRequest();

                    if (retryReq.result == UnityWebRequest.Result.Success)
                        directUrl = ExtractStreamURL(retryReq.downloadHandler.text);
                }
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

        if (!hasCartrige || insertedCartridge == null || !insertedCartridge.isURLCartridge)
            yield break;

        gameboyVideoPlayer.url = directUrl;
        gameboyVideoPlayer.frame = 0;
        gameboyVideoPlayer.Prepare();

        while (!gameboyVideoPlayer.isPrepared)
        {
            if (!hasCartrige || insertedCartridge == null)
                yield break;
            yield return null;
        }

        gameboyVideoPlayer.Play();
        gameboyBackground.SetActive(true);
    }

    private static UnityWebRequest CreateYouTubeRequest(string videoId, string visitorData)
    {
        string visitorField = string.IsNullOrEmpty(visitorData) ? "" : ",\"visitorData\":\"" + visitorData + "\"";
        string body = "{\"videoId\":\"" + videoId + "\",\"context\":{\"client\":{\"clientName\":\"ANDROID_VR\",\"clientVersion\":\"1.60.19\",\"deviceMake\":\"Meta\",\"deviceModel\":\"Quest 3\",\"osName\":\"Android\",\"osVersion\":\"12L\",\"platform\":\"MOBILE\",\"hl\":\"en\",\"gl\":\"US\"" + visitorField + "}},\"contentCheckOk\":true,\"racyCheckOk\":true}";

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
