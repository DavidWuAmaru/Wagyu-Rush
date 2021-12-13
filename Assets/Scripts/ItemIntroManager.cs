using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemIntroManager : MonoBehaviour
{
    float originalX = 0, topY = -20, botY = -105, difY = 85, originalZ = 0;
    float originalWidth = 45, originalHeight = 170;
    bool isOn = false;

    private void Start()
    {

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
