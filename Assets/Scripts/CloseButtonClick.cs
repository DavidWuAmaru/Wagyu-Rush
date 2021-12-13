using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseButtonClick : MonoBehaviour
{
    [SerializeField] GameObject[] objectToEnable;
    [SerializeField] GameObject[] objectToDisable;
    private void OnMouseDown()
    {
        for(int i = 0; i < objectToEnable.Length; ++i)
        {
            objectToEnable[i].SetActive(true);
        }
        for (int i = 0; i < objectToDisable.Length; ++i)
        {
            objectToDisable[i].SetActive(false);
        }
    }
}
