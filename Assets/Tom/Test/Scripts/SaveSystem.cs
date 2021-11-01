using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    public static void SaveMap(Map map)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string filename = Application.persistentDataPath + "/test.map";
        FileStream fs = new FileStream(filename, FileMode.Create);

        MapData mapData = new MapData(map);

        formatter.Serialize(fs, mapData);
        fs.Close();
    }
    public static MapData LoadMap()
    {
        string filename = Application.persistentDataPath + "/test.map";
        if (File.Exists(filename))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream fs = new FileStream(filename, FileMode.Open);

            MapData mapData = formatter.Deserialize(fs) as MapData;
            fs.Close();

            return mapData;
        }
        else
        {
            Debug.LogError("map File not found in " + filename);
            return null;
        }
    }
}
