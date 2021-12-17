using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class MapInfo
{
    //For Teaching map
    public bool isTrainingLevelFinished;
    //For Map controller
    public int[] levelLocked;
    public int[,] historyBest;
    //For Help list
    public bool[] seenItem;
    public bool seenBlock;
    public MapInfo()
    {
        isTrainingLevelFinished = false;
        levelLocked = new int[6] { 3, 0, 0, 0, 0, 0 };
        historyBest = new int[6, 6] { { -1, -1, -1, -1, -1, -1 },
                                      { -1, -1, -1, -1, -1, -1 },
                                      { -1, -1, -1, -1, -1, -1 },
                                      { -1, -1, -1, -1, -1, -1 },
                                      { -1, -1, -1, -1, -1, -1 },
                                      { -1, -1, -1, -1, -1, -1 } };
        seenItem = new bool[4] { false, false, false, false };
        seenBlock = false;
    }
    public void Show()
    {
        string s = "";
        for(int i = 0;i < levelLocked.Length; ++i)
        {
            s += levelLocked[i] + " ";
        }
        Debug.Log(s);
        for (int i = 0; i < historyBest.GetLength(0); ++i)
        {
            s = "";
            for (int j = 0; j < historyBest.GetLength(1); ++j)
            {
                s += historyBest[i, j] + " ";
            }
            Debug.Log(s);
        }
    }
}
public class PlayerData
{
    public static MapInfo mapInfo = new MapInfo();

    public static void Reset()
    {
        mapInfo = new MapInfo();
    }
    public static void Save()
    {
        string filename = Application.persistentDataPath + "/playerData.pda";
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream fs = new FileStream(filename, FileMode.Create);

        formatter.Serialize(fs, mapInfo);
        fs.Close();
    }
    public static void Load()
    {
        string filename = Application.persistentDataPath + "/playerData.pda";
        if (File.Exists(filename))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream fs = new FileStream(filename, FileMode.Open);

            MapInfo _mapInfo = formatter.Deserialize(fs) as MapInfo;
            fs.Close();

            mapInfo.isTrainingLevelFinished = _mapInfo.isTrainingLevelFinished;

            mapInfo.levelLocked = new int[_mapInfo.levelLocked.Length];
            for(int i = 0;i < mapInfo.levelLocked.Length; ++i)
            {
                mapInfo.levelLocked[i] = _mapInfo.levelLocked[i];
            }

            mapInfo.historyBest = new int[_mapInfo.historyBest.GetLength(0), _mapInfo.historyBest.GetLength(1)];
            for(int i=0;i<mapInfo.historyBest.GetLength(0); ++i)
            {
                for(int j=0;j<mapInfo.historyBest.GetLength(1);++j)
                {
                    mapInfo.historyBest[i, j] = _mapInfo.historyBest[i, j];
                }
            }

            mapInfo.seenItem = new bool[_mapInfo.seenItem.Length];
            for (int i = 0; i < _mapInfo.seenItem.Length; ++i)
            {
                mapInfo.seenItem[i] = _mapInfo.seenItem[i];
            }
            mapInfo.seenBlock = _mapInfo.seenBlock; 
        }
        else
        {
            Debug.LogError("player data File not found in " + filename);
        }
    }
}
