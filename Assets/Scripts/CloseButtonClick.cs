using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseButtonClick : MonoBehaviour
{
    [SerializeField] GameObject[] objectToDisable;
    private void OnMouseDown()
    {
        for (int i = 0; i < objectToDisable.Length; ++i)
        {
            objectToDisable[i].SetActive(false);
        }
    }
}
