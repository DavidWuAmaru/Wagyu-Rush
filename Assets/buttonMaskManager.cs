using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class buttonMaskManager : MonoBehaviour
{
    [SerializeField] private GameObject[] buttons;
    [SerializeField] private GameObject mapPreview;
    public static int currentWorld = 0;
    private int currentLevel = 0;
    // Start is called before the first frame update
    void Start()
    {
        
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
                StartCoroutine("buttonMove", 200);
                currentLevel--;
            }

       }
       if(Input.GetKeyDown(KeyCode.D)||Input.GetKeyDown(KeyCode.RightArrow))
       {
            if (currentLevel >= PlayerData.mapInfo.levelLocked[currentWorld]) return;
            else
            {
                StartCoroutine("buttonMove", -200);
                currentLevel++;
            }
        }
    }

    private void moveEndEvenet()
    {
        if (currentLevel == PlayerData.mapInfo.levelLocked[currentWorld]) mapPreview.GetComponent<MapLoader>().UpdateMap("Locked");
        else mapPreview.GetComponent<MapLoader>().UpdateMap(DataManager.mapAddress[currentWorld, currentLevel]);
    }

    IEnumerator buttonMove(float distance)
    {
        enable = false;
        while (Mathf.Abs(distance) > 3.0f)
        {
            float step = Mathf.Lerp(0, distance, 5 * Time.deltaTime);
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
        Debug.Log(currentLevel);
        for (int i = 0; i < buttons.Length; ++i)
        {
            buttons[i].transform.localPosition += new Vector3(currentLevel * 200, 0, 0);
        }
        currentLevel = 0;
        mapPreview.GetComponent<MapLoader>().UpdateMap(DataManager.mapAddress[currentWorld, currentLevel]);
    }
}
