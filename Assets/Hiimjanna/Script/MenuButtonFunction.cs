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
    [SerializeField] private GameObject openingPrefab;
    [SerializeField] private GameObject settingsMenuObj;
    [SerializeField] private GameObject worldMenuCanvas;
    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private GameObject levelMenuCanvas;
    [SerializeField] private GameObject cow;
    [SerializeField] private cowBehaviorInWorldMenu cowScript;
    [SerializeField] private GameObject scalingTeam;

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

    public void OpenLevelSelect()
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

    public void StartGame()
    {
        StartCoroutine("StartGameTransition",true);
    }
    public void BackToMenu()
    {
        StartCoroutine("StartGameTransition",false);
    }

    private void OnShrinkComplete()
    {
        settingsMenuObj.SetActive(false);
    }

    public void ResetScale()
    {
        StartCoroutine("BackToWorldMenu");     
    }

    IEnumerator BackToWorldMenu()
    {
        scalingTeam.transform.DOScale(new Vector3(0f, 0f, 0f), 0.5f).SetEase(Ease.InOutSine);
        yield return new WaitForSeconds(0.5f);

        levelMenuCanvas.SetActive(false);
        cowScript.enabled = true;

        //worldMenuCanvas.SetActive(true);
    }

    IEnumerator ChangeToEditor(int sceneId)
    {
        GameObject temp = Instantiate(closingPrefab);
        yield return new WaitForSeconds(temp.GetComponent<TransitionControl>().GetDuration());

        SceneManager.LoadScene(sceneId);
    }

    IEnumerator StartGameTransition(bool isStart)
    {
        GameObject temp = Instantiate(closingPrefab);

        yield return new WaitForSeconds(temp.GetComponent<TransitionControl>().GetDuration());

        worldMenuCanvas.SetActive(isStart);
        mainMenuCanvas.SetActive(!isStart);
        if (isStart)
        {
            cow.GetComponent<cowBehaviorInWorldMenu>().Reset();
        }
        Instantiate(openingPrefab);
        Destroy(temp);
    }
}
