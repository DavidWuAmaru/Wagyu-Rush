using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buttonsetactive : MonoBehaviour
{
    [SerializeField] private Canvas SettingUI;
    [SerializeField] private bool isactive;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void setSettingBtnActive()
    {
        //Debug.Log(isactive);
        SettingUI.gameObject.SetActive(isactive);
        isactive = !isactive;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
