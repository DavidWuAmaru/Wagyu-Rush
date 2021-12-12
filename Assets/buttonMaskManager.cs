using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class buttonMaskManager : MonoBehaviour
{
    [SerializeField] private GameObject[] buttons;
    [SerializeField] private GameObject mapPreview;
    [SerializeField] private GameObject buttonManager;
    [SerializeField] private TMP_Text bestText;
    public static int currentWorld = 0;
    private int currentLevel = 0;
    // Start is called before the first frame update
    void Start()
    {
        updateInfo();
    }

    bool enable = true;
    // Update is called once per frame
    void Update()
    {
        if(!enable)
        {
            return;
        }
       if(Input.GetKeyDown(KeyCode.A)||Input.GetKeyDown(KeyCode.LeftArrow))
       {
            if (currentLevel <= 0) return;
            else
            {
                currentLevel--;
                StartCoroutine("buttonMove", 200);
            }

       }
       if(Input.GetKeyDown(KeyCode.D)||Input.GetKeyDown(KeyCode.RightArrow))
       {
            if (currentLevel >= PlayerData.mapInfo.levelLocked[currentWorld]) return;
            else
            {
                currentLevel++;
                StartCoroutine("buttonMove", -200);
            }
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(currentLevel < PlayerData.mapInfo.levelLocked[currentWorld])
                buttonManager.GetComponent<MenuButtonFunction>().callMainMap(currentWorld, currentLevel);
        }
    }

    private void moveEndEvenet()
    {
        if (currentLevel >= PlayerData.mapInfo.levelLocked[currentWorld]) mapPreview.GetComponent<MapLoader>().UpdateMap("Locked");
        else mapPreview.GetComponent<MapLoader>().UpdateMap(DataManager.mapAddress[currentWorld, currentLevel]);

        if (PlayerData.mapInfo.historyBest[currentWorld, currentLevel] == -1) bestText.text = "No record";
        else bestText.text = "Best : " + DataManager.wagyuGradings[PlayerData.mapInfo.historyBest[currentWorld, currentLevel]];
    }

    IEnumerator buttonMove(float distance)
    {
        enable = false;
        while (Mathf.Abs(distance) > 3.0f)
        {
            float step = Mathf.Lerp(0, distance, 8 * Time.deltaTime);
            for (int i = 0; i < buttons.Length; ++i)
            {
                buttons[i].transform.localPosition += new Vector3(step, 0, 0);
            }
            distance -= step;
            yield return null;
        }

        for (int i = 0; i < buttons.Length; ++i)
        {
            buttons[i].transform.localPosition += new Vector3(distance, 0, 0);
        }

        moveEndEvenet();
        enable = true;
    }

    private void OnEnable()
    {
        updateInfo();
    }
    private void updateInfo()
    {
        PlayerData.Load();
        for (int i = 0; i < buttons.Length; ++i)
        {
            buttons[i].transform.localPosition += new Vector3(currentLevel * 200, 0, 0);
        }
        currentLevel = 0;
        if (currentLevel >= PlayerData.mapInfo.levelLocked[currentWorld]) mapPreview.GetComponent<MapLoader>().UpdateMap("Locked");
        else mapPreview.GetComponent<MapLoader>().UpdateMap(DataManager.mapAddress[currentWorld, currentLevel]);

        if (PlayerData.mapInfo.historyBest[currentWorld, currentLevel] == -1) bestText.text = "No record";
        else bestText.text = "Best : " + DataManager.wagyuGradings[PlayerData.mapInfo.historyBest[currentWorld, currentLevel]];

        //update buttons icon
        for (int i = 0; i < PlayerData.mapInfo.levelLocked[currentWorld]; ++i)
        {
            buttons[i].transform.GetChild(1).transform.GetChild(0).gameObject.SetActive(true);
            buttons[i].transform.GetChild(1).transform.GetChild(1).gameObject.SetActive(false);
        }
        for (int i = PlayerData.mapInfo.levelLocked[currentWorld]; i < DataManager.levelsOfWorld[currentWorld]; ++i)
        {
            buttons[i].transform.GetChild(1).transform.GetChild(0).gameObject.SetActive(false);
            buttons[i].transform.GetChild(1).transform.GetChild(1).gameObject.SetActive(true);
        }
    }
}
