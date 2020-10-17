using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public enum ECursorType
{
    Left,
    Right
}

public class Cursor : MonoBehaviour
{
    public GameObject LeftController;
    public GameObject RightController;

    //public GameObject LeftCursorTrail;
    //public GameObject RightCursorTrail;

    public ECursorType cursorType;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(UpdateRoutine()); // Using coroutine instead of Update()
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator UpdateRoutine()
    {
        //Debug.Log("updateRoutine");
        while (true)
        {
            Vector3 ControllerPos;
            //GameObject CursorTrail;

            if (cursorType == ECursorType.Left)
            {
                ControllerPos = LeftController.transform.position;
                //CursorTrail = LeftCursorTrail;
            }
            else
            {
                ControllerPos = RightController.transform.position;
                //CursorTrail = RightCursorTrail;
            }

            float currentZPos = gameObject.transform.position.z;
            //Debug.Log($"PosX: {ControllerPos.x} PosY: {ControllerPos.y} PosZ: {gameObject.transform.position.z}");
            gameObject.transform.position = new Vector3(ControllerPos.x, ControllerPos.y, currentZPos);

            // Cursor trail movement
            //CursorTrail.transform.position = new Vector3(ControllerPos.x, ControllerPos.y, currentZPos);

            yield return null;
        }
    }
}
