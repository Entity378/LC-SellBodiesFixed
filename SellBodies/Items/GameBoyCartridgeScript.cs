using UnityEngine;

public class GameBoyCartridgeScript : GrabbableObject
{
    public Material cartrigeMat;
    public Texture2D cartrigeBackground;

    public AudioClip gameboyCartridgeSFX;
    public AudioClip dropSFX;
    public AudioClip grabSFX;

    public override void Start()
    {
        base.Start();
    }

    public override void DiscardItem()
    {
        base.DiscardItem();
    }

    public override void GrabItem()
    {
        base.GrabItem();
    }
}
