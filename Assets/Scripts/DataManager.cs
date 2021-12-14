using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static string[] wagyuGradings = { "1", "2", "3", "4", "5" };
    public static int worldSize = 6;
    public static string[] worldNames = { "beginner", "Rotation Block", "Dancing Wagyu", "Portal", "Doom", "Custom" };
    public static int[] levelsOfWorld = new int[] { 6, 6, 6, 6, 6, 6 };
    public static string[,] mapAddress= {
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




}
