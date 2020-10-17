using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circle : MonoBehaviour
{
    // Circle parameters
    private float PosX = 0;
    private float PosY = 0;
    private float PosZ = 0;
    [HideInInspector]
    public int PosA = 0;

    private Color MainColor, MainColor1, MainColor2; // Circle sprites color
    public GameObject MainApproach, MainFore, MainBack; // Circle objects

    [HideInInspector]
    public SpriteRenderer Fore, Back, Appr; // Circle sprites

    // Checker stuff
    private bool RemoveNow = false;
    private bool GotIt = false;

    private void Awake()
    {
        Fore = MainFore.GetComponent<SpriteRenderer>();
        Back = MainBack.GetComponent<SpriteRenderer>();
        Appr = MainApproach.GetComponent<SpriteRenderer>();
    }

    // Set circle configuration
    public void Set(float x, float y, float z, int a)
    {
        PosX = x;
        PosY = y;
        PosZ = z;
        PosA = a;
        MainColor = Appr.color;
        MainColor1 = Fore.color;
        MainColor2 = Back.color;
    }

    // Spawning the circle
    public void Spawn()
    {
        //Debug.Log($"PosX: {PosX} PosY: {PosY} PosZ: {PosZ}");
        gameObject.transform.localPosition = new Vector3(PosX, PosY, PosZ);
        gameObject.transform.localScale = new Vector3(50f, 50f, 1f);
        this.enabled = true;
        StartCoroutine(Checker());
    }

    // If circle wasn't clicked
    public void Remove()
    {
        if (!GotIt)
        {
            RemoveNow = true;
            this.enabled = true;
            GameHandler.MissedCount++;
        }
    }

    // If circle was clicked
    public void Got()
    {
        if (!RemoveNow)
        {
            GotIt = true;
            MainApproach.transform.position = new Vector2(-101, -101);
            GameHandler.pSounds.PlayOneShot(GameHandler.pHitSound);
            RemoveNow = false;
            this.enabled = true;
        }
    }

    // Check if circle wasn't clicked
    private IEnumerator Checker()
    {
        while (true)
        {
            // 95 means delay before removing
            if (GameHandler.timer >= PosA + (GameHandler.ApprRate + 95) && !GotIt)
            {
                Remove(); 
                break;
            }
            yield return null;
        }
    }

    // Main Update
    private void Update()
    {
        // Approach Circle modifier
        if (MainApproach.transform.localScale.x >= 0.9f)
        {
            //MainApproach.transform.localScale -= new Vector3(4f, 4f, 4f) * Time.deltaTime;
            MainApproach.transform.localScale -= new Vector3(5.166667f, 5.166667f, 0f) * Time.deltaTime;
            MainColor.a += 4f * Time.deltaTime;
            MainColor1.a += 4f * Time.deltaTime;
            MainColor2.a += 4f * Time.deltaTime;
            Fore.color = MainColor1;
            Back.color = MainColor2;
            Appr.color = MainColor;

        }
        // If circle wasn't clicked
        else if (!GotIt)
        {
            // Remove circle
            if (!RemoveNow)
            {
                MainApproach.transform.position = new Vector2(-101, -101);
                this.enabled = false;
            }
            // If circle wasn't clicked
            else
            {
                MainColor1.a -= 10f * Time.deltaTime;
                MainColor2.a -= 10f * Time.deltaTime;
                MainFore.transform.localPosition += (Vector3.down * 2) * Time.deltaTime;
                MainBack.transform.localPosition += Vector3.down * Time.deltaTime;
                Fore.color = MainColor1;
                Back.color = MainColor2;
                if (MainColor1.a <= 0f)
                {
                    gameObject.transform.position = new Vector2(-101, -101);
                    this.enabled = false;
                }
            }
        }
        // If circle was clicked
        if (GotIt)
        {
            MainColor1.a -= 10f * Time.deltaTime;
            MainColor2.a -= 10f * Time.deltaTime;
            MainFore.transform.localScale += new Vector3(2, 2, 0) * Time.deltaTime;
            MainBack.transform.localScale += new Vector3(2, 2, 0) * Time.deltaTime;
            Fore.color = MainColor1;
            Back.color = MainColor2;
            if (MainColor1.a <= 0f)
            {
                gameObject.transform.position = new Vector2(-101, -101);
                this.enabled = false;
            }
        }
    }
}