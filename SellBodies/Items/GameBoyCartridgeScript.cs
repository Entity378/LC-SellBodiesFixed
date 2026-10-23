using UnityEngine;
using UnityEngine.Video;

public class GameBoyCartridgeScript : GrabbableObject
{
    public Material cartrigeMat;
    public VideoClip videoClip;
    public string URL;
    public bool isURLCartridge = false;

    private bool isEnteringURL = false;
    private string urlInput = "";

    public override void Start()
    {
        base.Start();
        scrapValue = 378;
    }

    public override void ItemActivate(bool used, bool buttonDown = true)
    {
        base.ItemActivate(used, buttonDown);
        if (buttonDown && playerHeldBy != null && isURLCartridge && !isEnteringURL)
        {
            OpenURLInput();
        }
    }

    private void OnGUI()
    {
        if (!isEnteringURL) return;

        float width = 500f;
        float height = 30f;
        float x = (Screen.width - width) / 2f;
        float y = (Screen.height - height) / 2f;

        GUI.Box(new Rect(x - 10, y - 40, width + 20, height + 60), "Insert URL");
        GUI.SetNextControlName("URLField");
        urlInput = GUI.TextField(new Rect(x, y, width, height), urlInput);
        GUI.FocusControl("URLField");

        if (Event.current.isKey && Event.current.keyCode == KeyCode.Return && urlInput.Length > 0)
        {
            URL = urlInput;
            CloseURLInput();
        }
        else if (Event.current.isKey && Event.current.keyCode == KeyCode.Escape)
        {
            CloseURLInput();
        }
    }

    private void OpenURLInput()
    {
        isEnteringURL = true;
        urlInput = URL ?? "";
        playerHeldBy.isTypingChat = true;
        playerHeldBy.disableLookInput = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void CloseURLInput()
    {
        isEnteringURL = false;
        urlInput = "";
        if (playerHeldBy != null)
        {
            playerHeldBy.isTypingChat = false;
            playerHeldBy.disableLookInput = false;
        }
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
