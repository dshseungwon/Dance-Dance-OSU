using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Valve.VR.Extras;
using Valve.VR;

[RequireComponent(typeof(SteamVR_LaserPointer))]
public class VRUIInput_PlayScene : MonoBehaviour
{
    public GameHandler gameHandler;
    public SteamVR_LaserPointer laserPointer;

    void Awake()
    {
        laserPointer.PointerIn += PointerInside;
        laserPointer.PointerOut += PointerOutside;
        laserPointer.PointerClick += PointerClick;
    }

    private void Start()
    {
    }



    public void PointerClick(object sender, PointerEventArgs e)
    {
        switch (e.target.name)
        {
            case "Lobby":
                gameHandler.LoadLobbyScene();
                break;
            default:
                break;
        }
    }

    public void PointerInside(object sender, PointerEventArgs e)
    {
    }

    public void PointerOutside(object sender, PointerEventArgs e)
    {
    }
}