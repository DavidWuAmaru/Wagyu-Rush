using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class SettingInfo
{
    //For Teaching map
    public float musicVol, soundVol;

    public SettingInfo()
    {
        musicVol = 1.0f;
        soundVol = 1.0f;
    }
}
public class SettingData
{
    public static SettingInfo settingInfo = new SettingInfo();

    public static void Reset()
    {
        settingInfo = new SettingInfo();
    }
    public static void Save()
    {
        string filename = Application.persistentDataPath + "/settingData.pda";
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream fs = new FileStream(filename, FileMode.Create);

        formatter.Serialize(fs, settingInfo);
        fs.Close();
    }
    public static void Load()
    {
        string filename = Application.persistentDataPath + "/settingData.pda";
        if (File.Exists(filename))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream fs = new FileStream(filename, FileMode.Open);

            SettingInfo _settingInfo = formatter.Deserialize(fs) as SettingInfo;
            fs.Close();

            settingInfo.musicVol = _settingInfo.musicVol;
            settingInfo.soundVol = _settingInfo.soundVol;
        }
        else
        {
            //Debug.LogError("setting data File not found in " + filename);
        }
    }
}
