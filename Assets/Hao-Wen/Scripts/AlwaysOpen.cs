using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlwaysOpen : MonoBehaviour
{
    private void Awake()
    {
        this.transform.GetChild(0).gameObject.SetActive(true);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
