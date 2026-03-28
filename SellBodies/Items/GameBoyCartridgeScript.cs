using UnityEngine;
using UnityEngine.Video;

public class GameBoyCartridgeScript : GrabbableObject
{
    public Material cartrigeMat;
    public VideoClip videoClip;


    public override void Start()
    {
        base.Start();
        scrapValue = 378;
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
