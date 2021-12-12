using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class cowBehaviorInWorldMenu : MonoBehaviour
{
    [SerializeField] private Sprite cowleft, cowright;
    [SerializeField] private Image[] bubbles;
    [SerializeField] private GameObject[] buttons = new GameObject[3];
    [SerializeField] private float alphaspeed;
    [SerializeField] private Canvas LevelMenu;
    [SerializeField] private TextMeshProUGUI WorldTitle;
    private float moveAmount;
    private float[] alpha;
    private bool controlenable;
    private Vector3 startposition;
    private int positionIndex,page;
    private const int itemsNumber = 3;
    public static string worldNum;
    // Start is called before the first frame update
    void Start()
    {
        //²¾°Êªº¶ZÂ÷
       
        controlenable = true;
        alpha = new float[bubbles.Length];
        for(int i=0;i<bubbles.Length;++i)
        {
            alpha[i] = bubbles[i].GetComponent<Image>().color.a;
        }
        positionIndex = 0;
        page = 0;
        
    }

    // Update is called once per frame
    void Update()
    {
        startposition = new Vector3(buttons[0].gameObject.transform.position.x,transform.position.y,transform.position.z);
        moveAmount = buttons[1].gameObject.transform.position.x - buttons[0].gameObject.transform.position.x;
        for (int i = 0; i < itemsNumber; ++i)
        {
            bubbles[i + page * itemsNumber].GetComponent<Image>().color = Vector4.Lerp(bubbles[i + page * itemsNumber].GetComponent<Image>().color, new Vector4(1, 1, 1, alpha[i + page * itemsNumber]), alphaspeed);
        }

        if (!controlenable) return;

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            if(positionIndex>page*itemsNumber)
            {
                controlenable = false;
                alpha[positionIndex] = 0;
                positionIndex--;
                StartCoroutine(IECowMove());
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
                StartCoroutine(IECowMove());
            }

            GetComponent<Image>().sprite = cowright;
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            switch (positionIndex)
            {
                case 0:
                    WorldTitle.text = "1.beginner";
                    worldNum = "1-";
                    buttonMaskManager.currentWorld = 0;
                    break;
                case 1:
                    WorldTitle.text = "2.Rotation Block";
                    worldNum = "2-";
                    buttonMaskManager.currentWorld = 1;
                    break;
                case 2:
                    WorldTitle.text = "3.Dancing Wagyu";
                    worldNum = "3-";
                    buttonMaskManager.currentWorld = 2;
                    break;
                default:
                    Debug.Log("World Title Text Error");
                    break;
            }

            LevelMenu.gameObject.SetActive(true);
            FindObjectOfType<AudioManager>().PlaySound("Click");
        }

    }

    IEnumerator IECowMove()
    {
        //Debug.Log(moveAmount + " " + positionIndex);
        Vector3 step = new Vector3(moveAmount, 0, 0) * 3; //speed up
        Vector3 target = new Vector3((positionIndex % itemsNumber)*moveAmount ,0,0) + startposition;
        if (target.x < transform.position.x) step *= -1;
        while (Vector3.Distance(transform.position, target) >= Mathf.Abs(step.x) * Time.deltaTime * 2)
        {
            transform.position += step * Time.deltaTime;
            yield return null;
        }
        transform.position = target;
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
