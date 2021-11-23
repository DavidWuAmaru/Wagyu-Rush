using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class cowBehaviorInWorldMenu : MonoBehaviour
{
    [SerializeField] private Sprite cowleft, cowright;
    [SerializeField] private float moveAmount;
    [SerializeField] private Image[] bubbles;
    [SerializeField] private float alphaspeed;
    private float[] alpha;
    private bool controlenable;
    //private Vector3 startposition;
    private int positionIndex,page;
    private const int itemsNumber = 3;
    // Start is called before the first frame update
    void Start()
    {
        controlenable = true;
        alpha = new float[bubbles.Length];
        for(int i=0;i<bubbles.Length;++i)
        {
            alpha[i] = bubbles[i].GetComponent<Image>().color.a;
        }
        positionIndex = 0;
        page = 0;
        //startposition = gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            if(positionIndex>page*itemsNumber)
            {
                controlenable = false;
                alpha[positionIndex] = 0;
                positionIndex--;
                StartCoroutine(IECowMove(moveAmount,true));
            }
            
            GetComponent<Image>().sprite = cowleft;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            if(positionIndex<(page+1)*itemsNumber-1)
            {
                controlenable = false;
                alpha[positionIndex] = 0;
                positionIndex++;
                StartCoroutine(IECowMove(moveAmount,false));
            }

            GetComponent<Image>().sprite = cowright;
        }

        for (int i = 0; i < itemsNumber; ++i)
        {
            bubbles[i + page * itemsNumber].GetComponent<Image>().color = Vector4.Lerp(bubbles[i + page * itemsNumber].GetComponent<Image>().color, new Vector4(1, 1, 1, alpha[i + page*itemsNumber]), alphaspeed);
        }

    }

    IEnumerator IECowMove(float moveAmount,bool isleft)
    {
        float temp = moveAmount;
        while (moveAmount>0)
        {
            if(isleft)
            {
                gameObject.transform.position += new Vector3(-(temp / 10.0f), 0, 0);
            }
            else
            {
                gameObject.transform.position += new Vector3((temp / 10.0f), 0, 0);
            }
            
            moveAmount -= (temp / 10.0f);

            yield return null;
        }
        alpha[positionIndex] = 1;
        controlenable = true;
    }

    /*
    IEnumerator IEbubbleShow(int index)
    {
        bubbles[index].
        while()
        yield return null;
    }

    IEnumerable IEbubbleClose()
    {
        yield return null;
    }*/
}
