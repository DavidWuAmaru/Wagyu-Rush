using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static int worldSize = 3;
    public static int[] levelsOfWorld = new int[] { 6, 6, 6 };
    public static string[,] mapAddress= {
       {"Assets/Resources/MapLevel/Level1-1.map",
        "Assets/Resources/MapLevel/Level1-2.map",
        "Assets/Resources/MapLevel/Level1-3.map"},
       {"Assets/Resources/MapLevel/Level2-1.map",
        "Assets/Resources/MapLevel/Level2-2.map",
        "Assets/Resources/MapLevel/Level2-3.map"},
       {"Assets/Resources/MapLevel/Level3-1.map",
        "Assets/Resources/MapLevel/Level3-2.map",
        "Assets/Resources/MapLevel/Level3-3.map"}
    };




}
