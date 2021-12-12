using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class MenuButtonFunction : MonoBehaviour
{
    public static int ChapterNumber;
    public static int LevelNumber;
    private AssetBundle myLoadedAssetBundle;
    private string[] scenePaths;

    [SerializeField] private GameObject closingPrefab;
    [SerializeField] private GameObject settingsMenuObj;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void callSetting()
    {
        settingsMenuObj.SetActive(true);

        settingsMenuObj.transform.DOScale(new Vector3(1, 1, 1), 0.5f).SetEase(Ease.InOutSine);
    }

    public void leaveSetting()
    {
        Tween t = settingsMenuObj.transform.DOScale(new Vector3(0, 0, 0), 0.5f).SetEase(Ease.InOutSine);

        t.OnComplete(OnShrinkComplete);       
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

        //Need to wait for transition
        StartCoroutine("ChangeToEditor",1);
        //SceneManager.LoadScene(1);
    }
    public void callMainMap(int level)
    {
        LevelNumber = level;
        FindObjectOfType<AudioManager>().PlaySound("Click");

        GameManager.currentWorld = ChapterNumber;
        GameManager.currentLevel = LevelNumber;

        if (ChapterNumber == 0 && LevelNumber == 0) StartCoroutine("ChangeToEditor", 3);
        else StartCoroutine("ChangeToEditor", 2);
        /*if(levelInfoFromUItoMainGame == "1-1") SceneManager.LoadScene(3);
        else SceneManager.LoadScene(2);*/
    }
    public void callMainMap(int world, int level)
    {
        LevelNumber = level;
        ChapterNumber = world;
        FindObjectOfType<AudioManager>().PlaySound("Click");

        GameManager.currentWorld = ChapterNumber;
        GameManager.currentLevel = LevelNumber;

        if (ChapterNumber == 0 && LevelNumber == 0) StartCoroutine("ChangeToEditor", 3);
        else StartCoroutine("ChangeToEditor", 2);
        /*if(levelInfoFromUItoMainGame == "1-1") SceneManager.LoadScene(3);
        else SceneManager.LoadScene(2);*/
    }
    public void ClickSoundEffect()
    {
        FindObjectOfType<AudioManager>().PlaySound("Click");
    }

    private void OnShrinkComplete()
    {
        settingsMenuObj.SetActive(false);
    }

    IEnumerator ChangeToEditor(int sceneId)
    {
        GameObject temp = Instantiate(closingPrefab);
        yield return new WaitForSeconds(temp.GetComponent<TransitionControl>().GetDuration());

        SceneManager.LoadScene(sceneId);
    }
}
