using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.IO;
using System;
using System.Linq;
using UnityEngine.SceneManagement;
using Valve.VR;

public enum EPlayerModel
{
    UnityChan,
    Rin,
    Knight,
    Zombie
}

public class LobbyHandler : MonoBehaviour
{
    public GameObject CurrentPlayerModel;
    public EPlayerModel CurrentPlayerModelType;
    public EDifficulty CurrentGameDifficulty;

    public GameObject UnityChan;
    public GameObject Rin;
    public GameObject Knight;
    public GameObject Zombie;

    public Text VeryEasy_Text;
    public Text Easy_Text;
    public Text Hard_Text;
    public Text VeryHard_Text;

    public Button GameStartButton;

    public GameObject OsuBeatmapButtonPrefab;
    public GameObject ScrollViewContent;

    public ScrollRect ScrollRectObject;

    private List<Text> OsuSongNameTextList;
    private Dictionary<string, string> OsuFileNameToPathDic;
    private Dictionary<string, string> OsuFileNameToMp3Dic;

    private string SelectedSong;
    private string SelectedSongPath;
    private string SelectedSongMp3Path;


    private void Awake()
    {
        Assert.IsNotNull(CurrentPlayerModel, "Assign CurrentPlayerModel.");
        SteamVR.Initialize();
    }

    // Start is called before the first frame update
    void Start()
    {
        OsuSongNameTextList = new List<Text>();
        OsuFileNameToPathDic = new Dictionary<string, string>();
        OsuFileNameToMp3Dic = new Dictionary<string, string>();
        ChangePlayerModel(CurrentPlayerModel);
        SetGameDifficulty(CurrentGameDifficulty);
        GetBeatmaps();

        GameStartButton.interactable = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void GetBeatmaps()
    {
        string UserDirectoryPath = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).FullName;
        if (Environment.OSVersion.Version.Major >= 6)
        {
            UserDirectoryPath = Directory.GetParent(UserDirectoryPath).ToString();
        }
        string OsuSongsDirectoryPath = Path.Combine(UserDirectoryPath, "AppData", "Local", "osu!", "Songs");

        // 하위 디렉터리 목록 조회
        var SongDirectories = (from dir in Directory.GetDirectories(OsuSongsDirectoryPath)
                               let info = new DirectoryInfo(dir)
                               select new
                               {
                                   FullName = info.FullName
                               }).ToList();

        foreach (var songFolder in SongDirectories)
        {
            // 하위 파일 목록 조회
            var OsuFiles = (from file in Directory.GetFiles(songFolder.FullName)
                            let info = new FileInfo(file)
                            where info.Extension == ".osu"
                            select new
                            {
                                Name = info.Name,
                                Path = info.FullName
                            }).ToList();

            var Mp3File = (from file in Directory.GetFiles(songFolder.FullName)
                           let info = new FileInfo(file)
                           where info.Extension == ".mp3"
                           orderby info.Length descending
                           select new
                           {
                               Name = info.Name,
                               Path = info.FullName
                           }).First();

            foreach (var file in OsuFiles)
            {
                OsuFileNameToPathDic.Add(file.Name, file.Path);
                OsuFileNameToMp3Dic.Add(file.Name, Mp3File.Path);

                var index = Instantiate(OsuBeatmapButtonPrefab, Vector3.zero, Quaternion.identity);
                Text SongText = index.GetComponentInChildren<Text>();
                OsuSongNameTextList.Add(SongText);
                SongText.text = file.Name;

                index.transform.SetParent(ScrollViewContent.transform);
                index.transform.localPosition = Vector3.zero;
                index.transform.localRotation = Quaternion.identity;
                index.transform.localScale = Vector3.one;
            }
        }
    }

    public void SelectSong(string SongName, Text SelectedText)
    {
        SelectedSong = SongName;
        if (!OsuFileNameToPathDic.ContainsKey(SongName))
        {
            Debug.Log("Dictionary does not have the key");
            return;
        }
        SelectedSongPath = OsuFileNameToPathDic[SongName];
        if (!OsuFileNameToMp3Dic.ContainsKey(SongName))
        {
            Debug.Log("Dictionary does not have the key");
            return;
        }
        SelectedSongMp3Path = OsuFileNameToMp3Dic[SongName];

        foreach (var text in OsuSongNameTextList)
        {
            text.fontStyle = FontStyle.Normal;
        }
        SelectedText.fontStyle = FontStyle.Bold;
        GameStartButton.interactable = true;
    }

    public void GameStart()
    {
        if (SelectedSongPath.Length > 0)
        {
            // Load Scene with
            // 1. Difficulty
            SceneParameter.Difficulty = CurrentGameDifficulty;
            // 2. OSU beatmap file path
            SceneParameter.BeatmapFilePath = SelectedSongPath;
            // 3. mp3 file path
            SceneParameter.MP3FilePath = SelectedSongMp3Path;
            // 4. Model
            SceneParameter.PlayerModel = CurrentPlayerModelType;

            SceneManager.LoadScene("PlayScene");
        }
    }

    public void BeatmapScrollUp()
    {
        ScrollRectObject.verticalNormalizedPosition += 0.03f;
    }

    public void BeatmapScrollDown()
    {
        ScrollRectObject.verticalNormalizedPosition -= 0.03f;
    }

    public void SetGameDifficulty(EDifficulty difficulty)
    {
        VeryEasy_Text.fontStyle = FontStyle.Normal;
        Easy_Text.fontStyle = FontStyle.Normal;
        Hard_Text.fontStyle = FontStyle.Normal;
        VeryHard_Text.fontStyle = FontStyle.Normal;

        CurrentGameDifficulty = difficulty;

        switch (CurrentGameDifficulty)
        {
            case EDifficulty.VeryEasy:
                VeryEasy_Text.fontStyle = FontStyle.Bold;
                break;
            case EDifficulty.Easy:
                Easy_Text.fontStyle = FontStyle.Bold;
                break;
            case EDifficulty.Hard:
                Hard_Text.fontStyle = FontStyle.Bold;
                break;
            case EDifficulty.VeryHard:
                VeryHard_Text.fontStyle = FontStyle.Bold;
                break;
            default:
                Debug.Log("Invalid Difficulty.");
                break;
        }
    }

    public void ChangePlayerModel(GameObject playerModel)
    {
        UnityChan.SetActive(false);
        Rin.SetActive(false);
        Knight.SetActive(false);
        Zombie.SetActive(false);

        playerModel.SetActive(true);

        if (playerModel == UnityChan)
        {
            CurrentPlayerModelType = EPlayerModel.UnityChan;
        }
        else if (playerModel == Rin)
        {
            CurrentPlayerModelType = EPlayerModel.Rin;
        }
        else if (playerModel == Knight)
        {
            CurrentPlayerModelType = EPlayerModel.Knight;
        }
        else if (playerModel == Zombie)
        {
            CurrentPlayerModelType = EPlayerModel.Zombie;
        }
        else
        {
            Debug.Log("Invalid Model");
        }

        gameObject.GetComponent<HeightChange>().player = CurrentPlayerModel;
    }

    public void ChangePlayerModel(EPlayerModel model)
    {
        UnityChan.SetActive(false);
        Rin.SetActive(false);
        Knight.SetActive(false);
        Zombie.SetActive(false);

        CurrentPlayerModelType = model;

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
}
