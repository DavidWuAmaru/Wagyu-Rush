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
    [SerializeField] private Canvas WorldMenu;
    [SerializeField] private GameObject bgLayer;
    [SerializeField] private TextMeshProUGUI WorldTitle;
    [SerializeField] private Sprite[] bubbleImg;
    private float moveAmount;
    private float[] alpha;
    private bool controlenable;
    private Vector3 startposition;
    private int positionIndex, page;
    private const int itemsNumber = 3;
    float cowMovePeriod = 0.35f;
    float bgMovePeriod = 0.5f;
    private const int maxPages = 2;
    bool initializaed = false;
    // Start is called before the first frame update
    void Start()
    {
        //���ʪ��Z��
        startposition = transform.position;

        controlenable = true;
        alpha = new float[bubbles.Length];
        for (int i = 0; i < bubbles.Length; ++i)
        {
            alpha[i] = bubbles[i].GetComponent<Image>().color.a;
        }
        positionIndex = 0;
        page = 0;
        Debug.Log(bgLayer.GetComponent<RectTransform>().localPosition.x);

        initializaed = true;
    }

    // Update is called once per frame
    void Update()
    {
        moveAmount = buttons[1].gameObject.transform.position.x - buttons[0].gameObject.transform.position.x;

        for (int i = 0; i < itemsNumber; ++i)
        {
            bubbles[i].GetComponent<Image>().color = Vector4.Lerp(bubbles[i].GetComponent<Image>().color, new Vector4(1, 1, 1, alpha[i]), alphaspeed);
        }

        if (!controlenable) return;

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            if(positionIndex>page*itemsNumber)
            {
                controlenable = false;
                alpha[positionIndex % itemsNumber] = 0;
                positionIndex--;
                StartCoroutine(IECowMove());
            }
            else
            {
                if(page != 0)
                {
                    alpha[positionIndex % itemsNumber] = 0;
                    page--;
                    positionIndex--;
                    StartCoroutine(IEbgLayerMove(true));
                }
            }
            GetComponent<Image>().sprite = cowleft;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            if(positionIndex<(page+1)*itemsNumber-1)
            {
                controlenable = false;
                alpha[positionIndex % itemsNumber] = 0;
                positionIndex++;
                StartCoroutine(IECowMove());
            }
            else
            {
                if(page<maxPages - 1)
                {
                    alpha[positionIndex % itemsNumber] = 0;
                    page++;
                    positionIndex++;
                    StartCoroutine(IEbgLayerMove(false));
                }
            }

            GetComponent<Image>().sprite = cowright;
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            switch (positionIndex)
            {
                case 0:
                    WorldTitle.text = "1.beginner";
                    MenuButtonFunction.ChapterNumber = 0;
                    buttonMaskManager.currentWorld = 0;
                    break;
                case 1:
                    WorldTitle.text = "2.Rotation Block";
                    MenuButtonFunction.ChapterNumber = 1;
                    buttonMaskManager.currentWorld = 1;
                    break;
                case 2:
                    WorldTitle.text = "3.Dancing Wagyu";
                    MenuButtonFunction.ChapterNumber = 2;
                    buttonMaskManager.currentWorld = 2;
                    break;
                case 3:
                    WorldTitle.text = "4.Portal";
                    MenuButtonFunction.ChapterNumber = 3;
                    buttonMaskManager.currentWorld = 3;
                    break;
                case 4:
                    WorldTitle.text = "5.Doom";
                    MenuButtonFunction.ChapterNumber = 4;
                    buttonMaskManager.currentWorld = 4;
                    break;
                case 5:
                    WorldTitle.text = "6.Custom";
                    MenuButtonFunction.ChapterNumber = 5;
                    buttonMaskManager.currentWorld = 5;
                    break;
                default:
                    Debug.Log("World Title Text Error");
                    return;
            }

            LevelMenu.gameObject.SetActive(true);
            WorldMenu.gameObject.SetActive(false);
            FindObjectOfType<AudioManager>().PlaySound("Click");
        }

    }

    IEnumerator IECowMove()
    {
        //Debug.Log(moveAmount + " " + positionIndex);
        Vector3 target = new Vector3((positionIndex % itemsNumber) * moveAmount, 0, 0) + startposition;
        Vector3 step = new Vector3(moveAmount, 0, 0) * Time.deltaTime / cowMovePeriod; //speed up
        while (Vector3.Distance(transform.position, target) >= step.x * 1.5f)
        {
            step = new Vector3(moveAmount, 0, 0) * Time.deltaTime / cowMovePeriod; //speed up
            if (target.x < transform.position.x) step *= -1;
            transform.position += step;
            yield return null;
        }
        transform.position = target;
        alpha[positionIndex % itemsNumber] = 1;
        controlenable = true;
    }
    Vector3 bgLayerDistance = new Vector3(800.0f,0,0);
    IEnumerator IEbgLayerMove(bool isleft)
    {
        controlenable = false;
        float timeElapsed = 0.0f;
        Vector3 bgLayerInitialPos = bgLayer.transform.position;
        Vector3 cowInitialPos = transform.position;
        Vector3 cowDistance = new Vector3(2 * moveAmount, 0, 0);
        while(timeElapsed <= bgMovePeriod)
        {
            timeElapsed += Time.deltaTime;
            if(isleft == false)
            {
                bgLayer.transform.position = bgLayerInitialPos - bgLayerDistance / bgMovePeriod * timeElapsed;
                transform.position = cowInitialPos - cowDistance / bgMovePeriod * timeElapsed;
            }
            else
            {
                bgLayer.transform.position = bgLayerInitialPos + bgLayerDistance / bgMovePeriod * timeElapsed;
                transform.position = cowInitialPos + cowDistance / bgMovePeriod * timeElapsed;
            }
            yield return null;
        }
        bgLayer.transform.position = bgLayerInitialPos;
        if(isleft == false)
        {
            transform.position = startposition;
        }
        else
        {
            transform.position = startposition + new Vector3(2*moveAmount,0,0);
        }
        controlenable = true;
        alpha[positionIndex % itemsNumber] = 1;
        updateBubbleImg();
    }
    void updateBubbleImg()
    {
        for (int i = 0; i < itemsNumber; ++i)
        {
            buttons[i].transform.GetChild(1).GetComponent<Image>().sprite = bubbleImg[page * itemsNumber + i];
        }
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
    private void OnEnable()
    {

    }
    public void Reset()
    {
        if (!initializaed) return;

        positionIndex = 0;
        page = 0;
        for (int i = 1; i < itemsNumber; ++i)
        {
            alpha[i] = 0;
            bubbles[i].GetComponent<Image>().color = new Vector4(1, 1, 1, alpha[i]);
        }
        alpha[0] = 1;
        bubbles[0].GetComponent<Image>().color = new Vector4(1, 1, 1, alpha[0]);
        gameObject.transform.position = startposition;
        controlenable = true;
        updateBubbleImg();
    }
}
