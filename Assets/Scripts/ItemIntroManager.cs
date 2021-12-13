using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ItemIntroManager : MonoBehaviour
{
    [SerializeField] private Button helpButton;
    [SerializeField] private Button[] buttonList;
    [SerializeField] private GameObject[] introList;
    [SerializeField] private GameObject content;
    float buttonHeight = 40;
    float originalX = 0, topY = -20, botY = -125, difY = 105, originalZ = 0;
    float originalWidth = 45, originalHeight = 170;  //base -> 10 (+ 4 * 40)
    bool isOn = false;

    public void SetButtonActive(bool isHayStack, bool isTrap, bool isHeadphone, bool isPortal, bool isRotateBlock)
    {
        for (int i = 0; i < introList.Length; ++i) introList[i].gameObject.SetActive(false);
        if (isHayStack == false && isTrap == false && isHeadphone==false && isPortal == false && isRotateBlock == false)
        {
            helpButton.gameObject.SetActive(false);
            return;
        }
        int index = 0;
        bool[] buttonActives = { isHayStack, isTrap, isHeadphone, isPortal, isRotateBlock };
        helpButton.gameObject.SetActive(true);
        for (int i = 0; i < buttonActives.Length; ++i)
        {
            if (buttonActives[i])
            {
                buttonList[i].gameObject.SetActive(true);
                buttonList[i].GetComponent<RectTransform>().localPosition = new Vector3(0, -25 - buttonHeight * index, 0);
                index++;
            }
            else buttonList[i].gameObject.SetActive(false);
        }

        originalHeight = 10 + buttonHeight * index;
        difY = 5 + index * 20;
        botY = topY - difY;
        //update button list
        if (isOn)
        {
            GetComponent<RectTransform>().sizeDelta = new Vector2(originalWidth, originalHeight);
            GetComponent<RectTransform>().localPosition = new Vector3(originalX, topY - difY, originalZ);
        }
    }


    public void ToggleOff()
    {
        isOn = false;
        Close();
    }
    public void ToggleOn()
    {
        isOn = true;
        Show();
    }
    public void Toggle()
    {
        isOn = !isOn;
        if (isOn) Show();
        else Close();
    }
    private void Show()
    {
        StopAllCoroutines();
        StartCoroutine(Opening());
    }
    private void Close()
    {
        StopAllCoroutines();
        StartCoroutine(Closing());
    }
    IEnumerator Opening()
    {
        //height -> 170
        float y = GetComponent<RectTransform>().sizeDelta.y, step = 0;
        float dist = originalHeight - y;
        while (dist >= 2)
        {
            step = Mathf.Lerp(0, dist, 6* Time.deltaTime);
            y += step;
            dist -= step;
            GetComponent<RectTransform>().sizeDelta = new Vector2(originalWidth, y);
            GetComponent<RectTransform>().localPosition = new Vector3(originalX, topY - difY * (y / originalHeight), originalZ);
            yield return null;
        }
        GetComponent<RectTransform>().localPosition = new Vector3(originalX, botY, originalZ);
        GetComponent<RectTransform>().sizeDelta = new Vector2(originalWidth, originalHeight);
    }
    IEnumerator Closing()
    {
        //height -> 0
        float y = GetComponent<RectTransform>().sizeDelta.y, step = 0;
        float dist = GetComponent<RectTransform>().sizeDelta.y;
        while (dist >= 2)
        {
            step = Mathf.Lerp(0, dist, 6 * Time.deltaTime);
            y -= step;
            dist -= step;
            GetComponent<RectTransform>().sizeDelta = new Vector2(originalWidth, y);
            GetComponent<RectTransform>().localPosition = new Vector3(originalX, topY - difY * (y / originalHeight), originalZ);
            yield return null;
        }
        GetComponent<RectTransform>().localPosition = new Vector3(originalX, topY, originalZ);
        GetComponent<RectTransform>().sizeDelta = new Vector2(originalWidth, 0);
    }
}
