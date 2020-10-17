using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public static class SceneParameter
{

    // Load Scene with
    // 1. Difficulty
    // 2. OSU beatmap file path
    // 3. mp3 file path
    // 4. Model
    public static EDifficulty Difficulty { get; set; }
    public static string BeatmapFilePath { get; set; }
    public static string MP3FilePath { get; set; }
    public static EPlayerModel PlayerModel { get; set; }
}