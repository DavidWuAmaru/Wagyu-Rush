using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaveLevelMenu : MonoBehaviour
{
    [SerializeField] private MenuButtonFunction mbf;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            mbf.ClickSoundEffect();
            mbf.ResetScale();
        }
    }
}
