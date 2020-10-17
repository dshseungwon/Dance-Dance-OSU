using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.UI;
using Valve.VR;
using Valve.VR.Extras;
using UnityEngine.SceneManagement;

public enum EDifficulty
{
    VeryEasy = 20,
    Easy = 15,
    Hard = 10,
    VeryHard = 5
}

public class GameHandler : MonoBehaviour
{
    // ----------------------------------------------------------------------------

    [Header("Objects")]
    public GameObject Circle; // Circle Object

    [Header("Map")]
    //public DefaultAsset MapFile; // Map file (.osu format), attach from editor
    public AudioClip MainMusic; // Music file, attach from editor
    public AudioClip HitSound; // Hit sound
    public AudioClip VictoryMusic;
    public AudioClip FailedMusic;

    [Header("Canvas")]
    public GameObject Canvas;

    [Header("Difficulty")]
    public EDifficulty Difficulty;

    // ----------------------------------------------------------------------------

    const int SPAWN = -100; // Spawn coordinates for objects

    public static double timer = 0; // Main song timer
    public static int ApprRate = 600; // Approach rate (in ms)
    private int DelayPos = 0; // Delay song position

    public static int ClickedCount = 0; // Clicked objects counter
    public static float ClickedPercentage = 0f;
    public static int MissedCount = 0; // Missed objects counter

    private int GameOverCount = 0;
    private bool bGameOver = false;
    private int ObjCount = 0; // Spawned objects counter
    
    private List<GameObject> CircleList; // Circles List
    private static string[] LineParams; // Object Parameters

    // Audio stuff
    private AudioSource Sounds;
    private AudioSource Music;
    public static AudioSource pSounds;
    public static AudioClip pHitSound;

    // Other stuff
    private Camera MainCamera;
    public GameObject StereoRenderer;
    //public GameObject StereoRendererPrefab;

    public GameObject LeftCursor;
    public GameObject RightCursor;

    public GameObject LeftController;
    public GameObject RightController;

    public SteamVR_Action_Boolean clickNote;
    public SteamVR_Action_Boolean grapNote;

    public GameObject UnityChan;
    public GameObject Rin;
    public GameObject Knight;
    public GameObject Zombie;

    private GameObject CurrentPlayerModel;

    public Text PerformanceText;

    public GameObject LobbyButton;
    public SteamVR_LaserPointer LeftLaserPointer;
    public SteamVR_LaserPointer RightLaserPointer;

    private void Awake()
    {
        SteamVR.Initialize();
    }
    public void ChangePlayerModel(EPlayerModel model)
    {
        UnityChan.SetActive(false);
        Rin.SetActive(false);
        Knight.SetActive(false);
        Zombie.SetActive(false);

        if (model == EPlayerModel.UnityChan)
        {
            CurrentPlayerModel = UnityChan;
        }
        else if (model == EPlayerModel.Rin)
        {
            CurrentPlayerModel = Rin;
        }
        else if (model == EPlayerModel.Knight)
        {
            CurrentPlayerModel = Knight;
        }
        else if (model == EPlayerModel.Zombie)
        {
            CurrentPlayerModel = Zombie;
        }
        else
        {
            Debug.Log("Invalid Model");
        }
        CurrentPlayerModel.SetActive(true);
        gameObject.GetComponent<HeightChange>().player = CurrentPlayerModel;
    }

    public void LoadSong(string path)
    {
        StartCoroutine(LoadSongCoroutine(path));
    }

    IEnumerator LoadSongCoroutine(string path)
    {
        string url = string.Format("file://{0}", path);
        WWW www = new WWW(url);
        while (!www.isDone)
            yield return null;

        Music.clip = NAudioPlayer.FromMp3Data(www.bytes);
    }

    private void GetSceneParameter()
    {
        Difficulty = SceneParameter.Difficulty;
        ReadCircles(SceneParameter.BeatmapFilePath);
        ChangePlayerModel(SceneParameter.PlayerModel);
        LoadSong(SceneParameter.MP3FilePath);
    }

    private void Start()
    {
        timer = 0;
        ClickedCount = 0; // Clicked objects counter
        ClickedPercentage = 0f;
        MissedCount = 0; // Missed objects counter

        MainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        Music = GameObject.Find("Music Source").GetComponent<AudioSource>();
        Sounds = gameObject.GetComponent<AudioSource>();
        Music.clip = MainMusic;
        pSounds = Sounds;
        pHitSound = HitSound;
        CircleList = new List<GameObject>();

        GetSceneParameter();
        //ReadCircles(AssetDatabase.GetAssetPath(MapFile));

        clickNote.AddOnStateDownListener(ClickNote, SteamVR_Input_Sources.LeftHand);
        clickNote.AddOnStateDownListener(ClickNote, SteamVR_Input_Sources.RightHand);

        grapNote.AddOnStateDownListener(ClickNote, SteamVR_Input_Sources.LeftHand);
        grapNote.AddOnStateDownListener(ClickNote, SteamVR_Input_Sources.RightHand);

        PerformanceText.text = string.Format("Success: {0}\nPercentage: {1:0.00}%", ClickedCount, ClickedPercentage);
    }

    // MAP READER
    void ReadCircles(string path)
    {
        StreamReader reader = new StreamReader(path);
        string line;

        // Skip to [HitObjects] part
        while (true)
        {
            if (reader.ReadLine() == "[HitObjects]")
                break;
        }

        int TotalLines = 0;

        // Count all lines
        while (!reader.EndOfStream)
        {
            reader.ReadLine();
            TotalLines++;
        }

        reader.Close();

        reader = new StreamReader(path);

        // Skip to [HitObjects] part again
        while (true)
        {
            if (reader.ReadLine() == "[HitObjects]")
                break;
        }

        // Sort objects on load
        int ForeOrder = TotalLines + 2; // Sort foreground layer
        int BackOrder = TotalLines + 1; // Sort background layer
        int ApproachOrder = TotalLines; // Sort approach circles layer

        // Some crazy Z axis modifications for sorting
        string TotalLinesStr = "0.0000";
        for (int i = 3; i > TotalLines.ToString().Length; i--)
            TotalLinesStr += "0";
        TotalLinesStr += TotalLines.ToString();
        float Z_Index = (float.Parse(TotalLinesStr));

        while (!reader.EndOfStream)
        {
            line = reader.ReadLine();
            if (line == null)
                break;

            LineParams = line.Split(','); // Line parameters (X&Y axis, time position)

            // Sorting configuration
            GameObject CircleObject = Instantiate(Circle, new Vector2(SPAWN, SPAWN), Quaternion.identity);
            CircleObject.GetComponent<Circle>().Fore.sortingOrder = ForeOrder;
            CircleObject.GetComponent<Circle>().Back.sortingOrder = BackOrder;
            CircleObject.GetComponent<Circle>().Appr.sortingOrder = ApproachOrder;
            CircleObject.transform.localPosition += new Vector3((float)CircleObject.transform.localPosition.x, (float)CircleObject.transform.localPosition.y, (float)Z_Index);
            CircleObject.transform.SetAsFirstSibling();
            ForeOrder--; BackOrder--; ApproachOrder--; Z_Index -= 0.000001f;

            int FlipY = 384 - int.Parse(LineParams[1]); // Flip Y axis

            var CanvasHeight = Canvas.GetComponent<RectTransform>().rect.height;
            var CanvasWdith = Canvas.GetComponent<RectTransform>().rect.width;

            int AdjustedX_Canvas = Mathf.RoundToInt(CanvasHeight * 1.333333f); // Aspect Ratio

            float Slices = 8f;
            float PaddingX_Canvas = AdjustedX_Canvas / Slices;
            float PaddingY_Canvas = CanvasHeight / Slices;
  

            float NewRangeX_Canvas = ((AdjustedX_Canvas - PaddingX_Canvas) - PaddingX_Canvas);
            float NewValueX_Canvas = ((int.Parse(LineParams[0]) * NewRangeX_Canvas) / 512f) + PaddingX_Canvas + ((CanvasWdith - AdjustedX_Canvas) / 2f);
            float NewRangeY_Canvas = CanvasHeight;
            float NewValueY_Canvas = ((FlipY * NewRangeY_Canvas) / 512f) + PaddingY_Canvas;

            CircleObject.transform.parent = Canvas.transform;
            Circle MainCircle = CircleObject.GetComponent<Circle>();
            MainCircle.Set(NewValueX_Canvas, NewValueY_Canvas, CircleObject.transform.position.z, int.Parse(LineParams[2]) - ApprRate);

            //MainCircle.Set(MainPos.x, MainPos.y, CircleObject.transform.position.z, int.Parse(LineParams[2]) - ApprRate);

            CircleList.Add(CircleObject);
        }

        GameOverCount = (int)Math.Ceiling((float)CircleList.Count * (float)0.2f);
        StartCoroutine(GameStartCoroutine());
    }
    IEnumerator GameStartCoroutine()
    {
        yield return new WaitForSeconds(3f);
        //Instantiate(StereoRendererPrefab, new Vector3(0, 5, -1.01f), Quaternion.Euler(-90, 180, 0));

        GameStart();
    }

    // END MAP READER

    private void GameStart()
    {
        //Debug.Log("GameStart");
        Application.targetFrameRate = -1; // Unlimited framerate
        Music.Play();
        StartCoroutine(UpdateRoutine()); // Using coroutine instead of Update()
    }
    private void ClickNote(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {

        Transform ControllerTransform;
        if (fromSource == SteamVR_Input_Sources.LeftHand)
        {
            ControllerTransform = LeftController.transform;
        }
        else
        {
            ControllerTransform = RightController.transform;
        }
        
        RaycastHit hitInfo;
        //Debug.DrawRay(ControllerTransform.position, Vector3.forward * 5, Color.blue, 3);
        int layerMask = 1 << 5;

        if (Physics.Raycast(ControllerTransform.position, Vector3.back, out hitInfo, Mathf.Infinity, layerMask))
        {
            if (hitInfo.collider.name == "Circle(Clone)")
            {
                int noteTime = hitInfo.collider.gameObject.GetComponent<Circle>().PosA + ApprRate;
                if (timer >= noteTime - (int)Difficulty)
                {
                    hitInfo.collider.gameObject.GetComponent<Circle>().Got();
                    hitInfo.collider.enabled = false;
                    ClickedCount++;
                    ClickedPercentage = ClickedCount * 100 / CircleList.Count;
                    PerformanceText.text = string.Format("Success: {0}\nPercentage: {1:0.00}%", ClickedCount, ClickedPercentage);
                }
            }
        }

    }

    private IEnumerator UpdateRoutine()
    {
        //Debug.Log("updateRoutine");
        while (Music.isPlaying)
        {
            timer = (Music.time * 1000); // Convert timer

            if (CircleList.Count > ObjCount)
            {
                DelayPos = (CircleList[ObjCount].GetComponent<Circle>().PosA);
                //Debug.Log($"timer: {timer}, DelayPos: {DelayPos}");
                // Spawn object
                if (timer >= DelayPos)
                {
                    //Debug.Log("Spawn");
                    CircleList[ObjCount].GetComponent<Circle>().Spawn();
                    ObjCount++;
                }
            }
            
            if (MissedCount >= GameOverCount)
            {
                Debug.Log("GameOver");
                GameOver();
                break;
            }
            yield return null;
        }
        if (!bGameOver)
        {
            StartCoroutine(VictoryCoroutine());
        }
    }

    private void GameOver()
    {
        bGameOver = true;
        Music.Stop();
        Music.clip = FailedMusic;
        Music.Play();

        HandleGameEndUI();
    }

    IEnumerator VictoryCoroutine()
    {

        yield return new WaitForSeconds(3f);
        Victory();
    }

    private void Victory()
    {
        Music.Stop();
        Music.clip = VictoryMusic;
        Music.Play();

        HandleGameEndUI();
    }

    private void HandleGameEndUI()
    {
        clickNote.RemoveOnStateDownListener(ClickNote, SteamVR_Input_Sources.LeftHand);
        clickNote.RemoveOnStateDownListener(ClickNote, SteamVR_Input_Sources.RightHand);

        grapNote.RemoveOnStateDownListener(ClickNote, SteamVR_Input_Sources.LeftHand);
        grapNote.RemoveOnStateDownListener(ClickNote, SteamVR_Input_Sources.RightHand);

        LeftCursor.SetActive(false);
        RightCursor.SetActive(false);
        LobbyButton.SetActive(true);
        EnableLaserPointer();
    }

    private void EnableLaserPointer()
    {
        LeftLaserPointer.active = true;
        RightLaserPointer.active = true;
    }

    public void LoadLobbyScene()
    {
        SceneManager.LoadScene("LobbyScene");
    }

}
