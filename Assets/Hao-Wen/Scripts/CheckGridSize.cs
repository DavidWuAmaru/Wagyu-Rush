using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CheckGridSize : MonoBehaviour
{
    private float gridWidth;
    private float gridHeight;
    private Bounds gridBound;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    
    void Update()
    {
        this.transform.position = GameObject.FindGameObjectWithTag("FuckingPosition").transform.position;
        Debug.Log(transform.position);
    }
}
