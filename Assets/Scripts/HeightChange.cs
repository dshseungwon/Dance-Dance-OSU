using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Valve.VR;


public class HeightChange : MonoBehaviour
{
    public SteamVR_Input_Sources head = SteamVR_Input_Sources.Head;

    public SteamVR_Action_Boolean headSet = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("HeadsetOnHead", true);
    public GameObject player;
    public Transform headTransfrom;

    private float height;

    void Awake()
    {
        Assert.IsNotNull(player, "Assign player prefab.");
        Assert.IsNotNull(headTransfrom, "Assign headTransform");
    }

    // Start is called before the first frame update
    void Start()
    {
        height = headTransfrom.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (SteamVR.initializedState != SteamVR.InitializedStates.InitializeSuccess)
        {
            return;
        }

        if (headSet.GetState(head) && height < headTransfrom.position.y)
        {
            height = headTransfrom.position.y;
            player.transform.localScale = new Vector3((float)(height / 1.3), (float)(height / 1.3), (float)(height / 1.3));
        }
    }
}
