using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static int levelGradingCount = 4;
    public static string[] wagyuGradings = { "Gold", "Silver", "Bronze", "Failure" };
    public static int worldSize = 6;
    public static int customWorldInex = 5;
    public static string[] worldNames = { "Rookie", "Vertigo", "Rhythm", "Portal", "Finale", "Custom" };
    public static int[] levelsOfWorld = new int[] { 6, 6, 6, 6, 6, 6 };
    public static string[,] mapAddress = {
       {"Assets/Resources/MapLevel/Level1-1.map",
        "Assets/Resources/MapLevel/Level1-2.map",
        "Assets/Resources/MapLevel/Level1-3.map",
        "Assets/Resources/MapLevel/Level1-4.map",
        "Assets/Resources/MapLevel/Level1-5.map",
        "Assets/Resources/MapLevel/Level1-6.map"},
       {"Assets/Resources/MapLevel/Level2-1.map",
        "Assets/Resources/MapLevel/Level2-2.map",
        "Assets/Resources/MapLevel/Level2-3.map",
        "Assets/Resources/MapLevel/Level2-4.map",
        "Assets/Resources/MapLevel/Level2-5.map",
        "Assets/Resources/MapLevel/Level2-6.map"},
       {"Assets/Resources/MapLevel/Level3-1.map",
        "Assets/Resources/MapLevel/Level3-2.map",
        "Assets/Resources/MapLevel/Level3-3.map",
        "Assets/Resources/MapLevel/Level3-4.map",
        "Assets/Resources/MapLevel/Level3-5.map",
        "Assets/Resources/MapLevel/Level3-6.map"},
       {"Assets/Resources/MapLevel/Level4-1.map",
        "Assets/Resources/MapLevel/Level4-2.map",
        "Assets/Resources/MapLevel/Level4-3.map",
        "Assets/Resources/MapLevel/Level4-4.map",
        "Assets/Resources/MapLevel/Level4-5.map",
        "Assets/Resources/MapLevel/Level4-6.map"},
       {"Assets/Resources/MapLevel/Level5-1.map",
        "Assets/Resources/MapLevel/Level5-2.map",
        "Assets/Resources/MapLevel/Level5-3.map",
        "Assets/Resources/MapLevel/Level5-4.map",
        "Assets/Resources/MapLevel/Level5-5.map",
        "Assets/Resources/MapLevel/Level5-6.map"},
       {"Assets/Resources/MapLevel/Level6-1.map",
        "Assets/Resources/MapLevel/Level6-2.map",
        "Assets/Resources/MapLevel/Level6-3.map",
        "Assets/Resources/MapLevel/Level6-4.map",
        "Assets/Resources/MapLevel/Level6-5.map",
        "Assets/Resources/MapLevel/Level6-6.map"}
    };

    //game difficulties
    public static int difficultyCount = 5;
    public static string[] difficultyNames = { "Chaos", "Hard", "Normal", "Easy", "Noob" };
    public static int[,] levelGradings = new int[5, 5]{
    { 15, 20, 25, 30, 40},
    { 12, 16, 20, 24, 30},
    { 10, 13, 16, 20, 24},
    {  8, 11, 14, 17, 20},
    {  6,  9, 12, 15, 18} };

}
