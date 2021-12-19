using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
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
    [SerializeField] private Image themeImg;
    [SerializeField] private Sprite[] themes;
    [SerializeField] private RectTransform cowInitialPos;
    [SerializeField] private TMP_Text[] medalNumTexts;
    [SerializeField] private Button editButton;

    [SerializeField] private GameObject scalingTeam;

    private float moveAmount;
    private float[] alpha;
    private bool controlenable;
    //private Vector3 startposition;
    private int positionIndex, page;
    private const int itemsNumber = 3;
    float cowMovePeriod = 0.35f;
    float bgMovePeriod = 0.5f;
    private const int maxPages = 2;
    bool initialized = false;
    // Start is called before the first frame update
    void Start()
    {
        //²¾°Êªº¶ZÂ÷
        controlenable = true;
        alpha = new float[bubbles.Length];
        for (int i = 0; i < bubbles.Length; ++i)
        {
            alpha[i] = bubbles[i].GetComponent<Image>().color.a;
        }
        positionIndex = 0;
        page = 0;

        initialized = true;
        //update medal count
        updateMedalCount();
    }

    // Update is called once per frame
    void Update()
    {
        moveAmount = buttons[1].GetComponent<RectTransform>().localPosition.x - buttons[0].GetComponent<RectTransform>().localPosition.x;

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
            if (positionIndex < DataManager.worldSize)
            {
                WorldTitle.text = string.Format("{0}.{1}", positionIndex + 1, DataManager.worldNames[positionIndex]);
                MenuButtonFunction.ChapterNumber = positionIndex;
                buttonMaskManager.currentWorld = positionIndex;
                if(positionIndex == DataManager.customWorldInex)
                {
                    editButton.gameObject.SetActive(true);
                }
                else
                {
                    editButton.gameObject.SetActive(false);
                }
                //update levelMenu
                themeImg.GetComponent<Image>().sprite = themes[positionIndex];
            }
            else
            {
                Debug.LogError("World Title Text Error");
                return;
            }

            LevelMenu.gameObject.SetActive(true);
            //WorldMenu.gameObject.SetActive(false);
            scalingTeam.transform.DOScale(new Vector3(1, 1, 1), 0.5f).SetEase(Ease.InOutSine);
            FindObjectOfType<AudioManager>().PlaySound("Click");

            this.enabled = false;
        }

    }

    private void updateMedalCount()
    {
        PlayerData.Load();
        int[] medalCount = new int[3] { 0, 0, 0 };
        for (int w = 0; w < DataManager.customWorldInex; ++w)
        {
            for (int l = 0; l < DataManager.levelsOfWorld[w]; ++l)
            {
                if (PlayerData.mapInfo.historyBest[w, l] >= 0 && PlayerData.mapInfo.historyBest[w, l] < DataManager.levelGradingCount - 1)
                    medalCount[PlayerData.mapInfo.historyBest[w, l]]++;
            }
        }
        for (int i = 0; i < DataManager.levelGradingCount - 1; ++i)
        {
            medalNumTexts[i].text = string.Format("x{0}", medalCount[i]);
        }
    }
    IEnumerator IECowMove()
    {
        controlenable = false;
        float timeElapsed = 0.0f;
        Vector3 initialPos = GetComponent<RectTransform>().localPosition;
        Vector3 target = new Vector3((positionIndex % itemsNumber) * moveAmount, 0, 0) + cowInitialPos.localPosition;
        while (timeElapsed <= cowMovePeriod)
        {
            timeElapsed += Time.deltaTime;
            GetComponent<RectTransform>().localPosition = initialPos +  (target - initialPos) / cowMovePeriod * timeElapsed;
            yield return null;
        }
        GetComponent<RectTransform>().localPosition = target;
        controlenable = true;
        alpha[positionIndex % itemsNumber] = 1;
    }

    Vector3 bgLayerDistance = new Vector3(800.0f,0,0);
    IEnumerator IEbgLayerMove(bool isleft)
    {
        controlenable = false;
        float timeElapsed = 0.0f;
        Vector3 bgLayerInitialPos = bgLayer.GetComponent<RectTransform>().localPosition;
        Vector3 _cowInitialPos = GetComponent<RectTransform>().localPosition;
        Vector3 cowDistance = new Vector3(2 * moveAmount, 0, 0);
        while(timeElapsed <= bgMovePeriod)
        {
            timeElapsed += Time.deltaTime;
            if(isleft == false)
            {
                bgLayer.GetComponent<RectTransform>().localPosition = bgLayerInitialPos - bgLayerDistance / bgMovePeriod * timeElapsed;
                GetComponent<RectTransform>().localPosition = _cowInitialPos - cowDistance / bgMovePeriod * timeElapsed;
            }
            else
            {
                bgLayer.GetComponent<RectTransform>().localPosition = bgLayerInitialPos + bgLayerDistance / bgMovePeriod * timeElapsed;
                GetComponent<RectTransform>().localPosition = _cowInitialPos + cowDistance / bgMovePeriod * timeElapsed;
            }
            yield return null;
        }
        bgLayer.GetComponent<RectTransform>().localPosition = bgLayerInitialPos;
        if(isleft == false)
        {
            GetComponent<RectTransform>().localPosition = cowInitialPos.localPosition;
        }
        else
        {
            GetComponent<RectTransform>().localPosition = cowInitialPos.localPosition + new Vector3(2 * moveAmount,0,0);
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
        if (!initialized) return;

        positionIndex = 0;
        page = 0;
        for (int i = 1; i < itemsNumber; ++i)
        {
            alpha[i] = 0;
            bubbles[i].GetComponent<Image>().color = new Vector4(1, 1, 1, alpha[i]);
        }
        alpha[0] = 1;
        bubbles[0].GetComponent<Image>().color = new Vector4(1, 1, 1, alpha[0]);
        GetComponent<RectTransform>().localPosition = cowInitialPos.localPosition;
        controlenable = true;
        updateBubbleImg();
        updateMedalCount();
    }
}
