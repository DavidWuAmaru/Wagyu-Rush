using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtonFunction : MonoBehaviour
{
    public static string levelInfoFromUItoMainGame = "1-1";
    public static string ChapterNumber;
    public static string LevelNumber;
    private AssetBundle myLoadedAssetBundle;
    private string[] scenePaths;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void exitGameMenu()
    {
        FindObjectOfType<AudioManager>().PlaySound("Click");

        //UnityEditor.EditorApplication.isPlaying = false;//Ãö±¼unity play mode
        Application.Quit();//exit game
    }

    public void callMapEditor()
    {
        FindObjectOfType<AudioManager>().PlaySound("Click");

        SceneManager.LoadScene(1);
    }

    public void callMainMap(string s)
    {
        FindObjectOfType<AudioManager>().PlaySound("Click");

        levelInfoFromUItoMainGame = s;
        SceneManager.LoadScene(2);
    }
    public void ClickSoundEffect()
    {
        FindObjectOfType<AudioManager>().PlaySound("Click");
    }
}
