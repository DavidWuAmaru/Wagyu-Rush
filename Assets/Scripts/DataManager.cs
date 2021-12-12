using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static string[] wagyuGradings = { "A1", "A2", "A3", "A4", "A5" };
    public static int worldSize = 3;
    public static int[] levelsOfWorld = new int[] { 6, 6, 6 };
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
        "Assets/Resources/MapLevel/Level3-6.map"}
    };




}
