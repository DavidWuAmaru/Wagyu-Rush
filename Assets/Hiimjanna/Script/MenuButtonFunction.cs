using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtonFunction: MonoBehaviour
{
    private AssetBundle myLoadedAssetBundle;
    private string[] scenePaths;

    // Start is called before the first frame update
    void Start()
    {
        myLoadedAssetBundle = AssetBundle.LoadFromFile("Assets/Scenes");
        scenePaths = myLoadedAssetBundle.GetAllScenePaths();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void exitGameMenu()
    {
        UnityEditor.EditorApplication.isPlaying = false;//Ãö±¼unity play mode
        Application.Quit();//exit game
        //Debug.Log('1');
    }

    public void callMapEditor()
    {
        Debug.Log('1');
        SceneManager.LoadScene(1);
    }
}
