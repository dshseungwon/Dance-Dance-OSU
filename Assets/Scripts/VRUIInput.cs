using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Valve.VR.Extras;
using Valve.VR;

[RequireComponent(typeof(SteamVR_LaserPointer))]
public class VRUIInput : MonoBehaviour
{
    public LobbyHandler lobbyHandler;
    public SteamVR_LaserPointer laserPointer;
    public SteamVR_Action_Boolean moveTrackPad;

    void Awake()
    {
        laserPointer.PointerIn += PointerInside;
        laserPointer.PointerOut += PointerOutside;
        laserPointer.PointerClick += PointerClick;
    }

    private void Start()
    {
        moveTrackPad.AddOnStateDownListener(DisableLaserPointer, SteamVR_Input_Sources.LeftHand);
        moveTrackPad.AddOnStateDownListener(DisableLaserPointer, SteamVR_Input_Sources.RightHand);

        moveTrackPad.AddOnStateUpListener(EnableLaserPointer, SteamVR_Input_Sources.LeftHand);
        moveTrackPad.AddOnStateUpListener(EnableLaserPointer, SteamVR_Input_Sources.RightHand);
    }

    private void DisableLaserPointer(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        laserPointer.active = false;
    }
    private void EnableLaserPointer(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        laserPointer.active = true;
    }

    public void PointerClick(object sender, PointerEventArgs e)
    {
        switch (e.target.name)
        {
            case "Very Easy":
                lobbyHandler.SetGameDifficulty(EDifficulty.VeryEasy);
                break;
            case "Easy":
                lobbyHandler.SetGameDifficulty(EDifficulty.Easy);
                break;
            case "Hard":
                lobbyHandler.SetGameDifficulty(EDifficulty.Hard);
                break;
            case "Very Hard":
                lobbyHandler.SetGameDifficulty(EDifficulty.VeryHard);
                break;
            case "Game Start":
                lobbyHandler.GameStart();
                break;
            case "unitychan_statue":
                lobbyHandler.ChangePlayerModel(EPlayerModel.UnityChan);
                break;
            case "zombie_statue":
                lobbyHandler.ChangePlayerModel(EPlayerModel.Zombie);
                break;
            case "rin_statue":
                lobbyHandler.ChangePlayerModel(EPlayerModel.Rin);
                break;
            case "knight_statue":
                lobbyHandler.ChangePlayerModel(EPlayerModel.Knight);
                break;
            case "UP":
                lobbyHandler.BeatmapScrollUp();
                break;
            case "DOWN":
                lobbyHandler.BeatmapScrollDown();
                break;
            case "Osu Beatmap Button(Clone)":
                Text TargetText = e.target.GetComponentInChildren<Text>();
                string SongName = TargetText.text;
                lobbyHandler.SelectSong(SongName, TargetText);
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