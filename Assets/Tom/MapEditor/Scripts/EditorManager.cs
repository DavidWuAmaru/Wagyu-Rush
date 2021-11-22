using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class EditorManager : MonoBehaviour
{
    [SerializeField] private GameObject blockPref;
    //[SerializeField] private GameObject grid;
    //[SerializeField] private GameObject blankObj;
    [SerializeField] private Slider widthSlider;
    [SerializeField] private Slider heightSlider;
    [SerializeField] private TMP_Text sizeText;
    [SerializeField] private TMP_InputField filenameInputField;
    [SerializeField] private List<Sprite> blockSprites;
    [SerializeField] private List<float> blockScales;
    [SerializeField] private List<Sprite> charSprites;
    [SerializeField] private List<float> charScales;
    [SerializeField] private List<Sprite> itemSprites;
    [SerializeField] private List<float> itemScales;
    [SerializeField] private List<Sprite> destSprites;
    [SerializeField] private List<float> destScales;
    [SerializeField] private Sprite plainSprite;
    [SerializeField] private Sprite blankSprite;
    [SerializeField] private TMP_Text messageBanner;
    [SerializeField] private GameObject trashCan;
    // [SerializeField] private float puttingArea = 0.5f;
    [SerializeField] private float acceptableArea = 0.7f;
    [SerializeField] private float rightClickArea = 0.6f;
    


    public int width = 4, height = 4;
    private float offsetX, offsetY;
    private float edgeLength = 4.0f;
    private float mapLength = 48.0f;
    GameObject[,] map;
    private Vector2 trashCanPos;
    //GameObject[,] baseMap;
    //GameObject[,] topMap;
    int[,] typeMap, rotationMap;
    int[,] charMap, itemMap, destMap;
    public List<Draggable> blocks;
    public List<Draggable> characters;
    public List<Draggable> items;
    public List<Draggable> dests;
    public List<Draggable> trash_cans;
    public Draggable draggable;
    

    void Start()
    {
        //if (FindObjectOfType<AudioManager>().isSoundPlaying("BGM")) return;
        //FindObjectOfType<AudioManager>().PlaySound("BGM");

        draggable.dragEndedCallback = OnDragEnded_block;
        for (int i = 0; i < blocks.Count; ++i) blocks[i].dragEndedCallback = OnDragEnded_block;
        for (int i = 0; i < characters.Count; ++i) characters[i].dragEndedCallback = OnDragEnded_character;
        for (int i = 0; i < items.Count; ++i) items[i].dragEndedCallback = OnDragEnded_item;
        for (int i = 0; i < dests.Count; ++i) dests[i].dragEndedCallback = OnDragEnded_dest;
        for (int i = 0; i < trash_cans.Count; ++i) trash_cans[i].dragEndedCallback = OnDragEnded_TC;

        trashCanPos = new Vector2(trashCan.transform.position.x, trashCan.transform.position.y);

        CreateMap();
        sizeText.text = "Size : " + width.ToString() + " , " + height.ToString();
        filenameInputField.text = "1-1";
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            SetRotateMap(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
        }
    }
    private Vector2Int GetPos(float x, float y)
    {
        if (x < offsetX - edgeLength * 0.5f * acceptableArea || y < offsetY - edgeLength * 0.5f * acceptableArea) return new Vector2Int(-1, -1);
        if (x > offsetX + edgeLength * (width - 1) + edgeLength * 0.5f * acceptableArea ||
            y > offsetY + edgeLength * (height - 1) + edgeLength * 0.5f * acceptableArea) return new Vector2Int(-1, -1);
        x = (x - offsetX) / edgeLength;
        y = (y - offsetY) / edgeLength;
        int tarX = (int)(x + 0.5f), tarY = (int)(y + 0.5f);
        if (Vector2.Distance(new Vector2(x, y), new Vector2(tarX, tarY)) <= acceptableArea) return new Vector2Int(tarX, tarY);
        else return new Vector2Int(-1, -1);
    }
    private void placeBlockSoundEffect()
    {
        int i = Random.Range(1, 4);
        FindObjectOfType<AudioManager>().PlaySound("PlaceBlock" + i.ToString());
    }
    // placing items
    private void OnDragEnded_block(Vector2 position, int type)
    {
        Vector2Int pos = GetPos(position.x, position.y);
        if (pos.x == -1 || pos.y == -1) return;
        placeBlockSoundEffect();
        map[pos.x, pos.y].tag = "Block";
        map[pos.x, pos.y].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = blockSprites[type];
        map[pos.x, pos.y].transform.GetChild(0).transform.localScale = new Vector3(edgeLength - 0.5f, edgeLength - 0.5f, 0) * blockScales[type];
        typeMap[pos.x, pos.y] = type;

        if (type < 0 || type > 4)
        {
            map[pos.x, pos.y].transform.GetChild(0).transform.eulerAngles = new Vector3(0, 0, 0);
            rotationMap[pos.x, pos.y] = 0;
            //clear obj
            map[pos.x, pos.y].transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = blankSprite;
            itemMap[pos.x, pos.y] = -1;
            charMap[pos.x, pos.y] = -1;
            destMap[pos.x, pos.y] = -1;
        }
    }
    private void OnDragEnded_character(Vector2 position, int type)
    {
        Vector2Int pos = GetPos(position.x, position.y);
        if (pos.x == -1 || pos.y == -1) return;
        placeBlockSoundEffect();
        if (typeMap[pos.x, pos.y] < 0 || typeMap[pos.x, pos.y] > 4) return;
        map[pos.x, pos.y].transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = charSprites[type];
        map[pos.x, pos.y].transform.GetChild(1).transform.localScale = new Vector3(edgeLength, edgeLength, 0) * charScales[type];
        charMap[pos.x, pos.y] = type;
        itemMap[pos.x, pos.y] = -1;
        destMap[pos.x, pos.y] = -1;
    }
    private void OnDragEnded_item(Vector2 position, int type)
    {
        Vector2Int pos = GetPos(position.x, position.y);
        if (pos.x == -1 || pos.y == -1) return;
        placeBlockSoundEffect();
        if (typeMap[pos.x, pos.y] < 0 || typeMap[pos.x, pos.y] > 4) return;
        map[pos.x, pos.y].transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = itemSprites[type];
        map[pos.x, pos.y].transform.GetChild(1).transform.localScale = new Vector3(edgeLength, edgeLength, 0) * itemScales[type];
        charMap[pos.x, pos.y] = -1;
        itemMap[pos.x, pos.y] = type;
        destMap[pos.x, pos.y] = -1;
    }
    private void OnDragEnded_dest(Vector2 position, int type)
    {
        Vector2Int pos = GetPos(position.x, position.y);
        if (pos.x == -1 || pos.y == -1) return;
        placeBlockSoundEffect();
        if (typeMap[pos.x, pos.y] < 0 || typeMap[pos.x, pos.y] > 4) return;
        map[pos.x, pos.y].transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = destSprites[type];
        map[pos.x, pos.y].transform.GetChild(1).transform.localScale = new Vector3(edgeLength, edgeLength, 0) * destScales[type];
        charMap[pos.x, pos.y] = -1;
        itemMap[pos.x, pos.y] = -1;
        destMap[pos.x, pos.y] = type;

    }
    private void OnDragEnded_TC(Vector2 position, int type)
    {
        Vector2Int pos = GetPos(position.x, position.y);
        if (pos.x == -1 || pos.y == -1) return;
        placeBlockSoundEffect();
        if (type == 0)  //clear blocks + item
        {
            map[pos.x, pos.y].tag = "Untagged";
            map[pos.x, pos.y].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = plainSprite;
            map[pos.x, pos.y].transform.GetChild(0).transform.localScale = new Vector3(edgeLength - 0.5f, edgeLength - 0.5f, 0);
            map[pos.x, pos.y].transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = blankSprite;
            typeMap[pos.x, pos.y] = -1;
            rotationMap[pos.x, pos.y] = -1;
            itemMap[pos.x, pos.y] = -1;
            charMap[pos.x, pos.y] = -1;
            destMap[pos.x, pos.y] = -1;
        }
        else if(type == 1)  //clear item
        {
            map[pos.x, pos.y].transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = blankSprite;
            itemMap[pos.x, pos.y] = -1;
            charMap[pos.x, pos.y] = -1;
            destMap[pos.x, pos.y] = -1;
        }
    }
    
    private void OnDragEnded_wholeBlock(GameObject block, Vector2 position, bool isDuplicating)
    {
        //trash can
        if(Vector2.Distance(trashCanPos, position) <= 5.0f)
        {
            for (int y = 0; y < height; ++y) for (int x = 0; x < width; ++x) if (block == map[x, y])
                    {
                        ResetBlockDate(new Vector2Int(x, y));
                        return;
                    }
        }

        //moving blocks
        Vector2Int tar = GetPos(position.x, position.y);
        if (tar.x == -1 || tar.y == -1) return;
        Vector2Int src = GetPos(-1, -1);
        for(int y = 0;y < height;++y) for(int x = 0;x < width; ++x) if (block == map[x, y]) src = new Vector2Int(x, y);
        if (src.x == -1 || src.y == -1 || tar == src) return;

        //add new one
        placeBlockSoundEffect();
        CopyBlockData(src, tar);
        if(!isDuplicating) ResetBlockDate(src);
    }
    private void ResetBlockDate(Vector2Int tar)
    {
        map[tar.x, tar.y].tag = "Untagged";
        map[tar.x, tar.y].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = plainSprite;
        map[tar.x, tar.y].transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = blankSprite;
        map[tar.x, tar.y].transform.GetChild(0).transform.localScale = new Vector3(edgeLength - 0.5f, edgeLength - 0.5f, 0);
        typeMap[tar.x, tar.y] = -1;
        charMap[tar.x, tar.y] = -1;
        itemMap[tar.x, tar.y] = -1;
        destMap[tar.x, tar.y] = -1;
        rotationMap[tar.x, tar.y] = 0;
        map[tar.x, tar.y].transform.GetChild(0).transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
    }
    private void CopyBlockData(Vector2Int src, Vector2Int dest)
    {
        map[dest.x, dest.y].tag = map[src.x, src.y].tag;
        map[dest.x, dest.y].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = map[src.x, src.y].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
        map[dest.x, dest.y].transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = map[src.x, src.y].transform.GetChild(1).GetComponent<SpriteRenderer>().sprite;
        map[dest.x, dest.y].transform.GetChild(0).transform.localScale = map[src.x, src.y].transform.GetChild(0).transform.localScale;
        map[dest.x, dest.y].transform.GetChild(1).transform.localScale = map[src.x, src.y].transform.GetChild(1).transform.localScale;
        typeMap[dest.x, dest.y] = typeMap[src.x, src.y];
        charMap[dest.x, dest.y] = charMap[src.x, src.y];
        itemMap[dest.x, dest.y] = itemMap[src.x, src.y];
        destMap[dest.x, dest.y] = destMap[src.x, src.y];
        rotationMap[dest.x, dest.y] = rotationMap[src.x, src.y];
        map[dest.x, dest.y].transform.GetChild(0).transform.eulerAngles = new Vector3(0.0f, 0.0f, rotationMap[dest.x, dest.y] * 90.0f);
    }

    void CreateMap()
    {
        //create
        map = new GameObject[width, height];
        //baseMap = new GameObject[width, height];
        //topMap = new GameObject[width, height];
        typeMap = new int[width, height];
        charMap = new int[width, height];
        itemMap = new int[width, height];
        destMap = new int[width, height];
        rotationMap = new int[width, height];
        for (int y = 0; y < height; ++y) for (int x = 0; x < width; ++x)
            {
                typeMap[x, y] = -1;
                charMap[x, y] = -1;
                itemMap[x, y] = -1;
                destMap[x, y] = -1;
                rotationMap[x, y] = 0;
            }
        edgeLength = mapLength / Mathf.Max(width, height);
        offsetX = edgeLength * 0.5f + 0.5f;
        offsetY = edgeLength * 0.5f + 0.5f;
        if (width > height) offsetY += edgeLength * 0.5f * (width - height);
        else if (height > width) offsetX += edgeLength * 0.5f * (height - width);

        for (int y = 0; y < height; ++y)
        {
            for (int x = 0; x < width; ++x)
            {
                map[x, y] = Instantiate(blockPref, new Vector3(x * edgeLength + offsetX, y * edgeLength + offsetY, 1), Quaternion.identity);
                map[x,y].transform.GetChild(0).transform.localScale = new Vector3(edgeLength - 0.5f, edgeLength - 0.5f, 0);
                map[x,y].GetComponent<BoxCollider2D>().size = new Vector3(edgeLength - 0.5f, edgeLength - 0.5f, 0);
                map[x, y].GetComponent<Draggable2>().dragEndedCallback = OnDragEnded_wholeBlock;
                //baseMap[x, y] = Instantiate(grid, new Vector3(0, 0, 0), Quaternion.identity);
                //baseMap[x, y].transform.localScale = new Vector3(edgeLength - 0.5f, edgeLength - 0.5f, 0);
                //topMap[x, y] = Instantiate(blankObj, new Vector3(0, 0, 0), Quaternion.identity);
                //binding
                //baseMap[x, y].transform.parent = map[x, y].transform;
                //topMap[x, y].transform.parent = map[x, y].transform;
                //map[x, y].transform.position = new Vector3(x * edgeLength + offsetX, y * edgeLength + offsetY, 1);
            }
        }
    }
    public void SliderUpdateMap()
    {
        int w = (int)widthSlider.value;
        int h = (int)heightSlider.value;
        if(w != width || h != height) UpdateMap(w, h);
    }
    void UpdateMap(int _width, int _height)
    {
        //clear
        for(int y = 0;y < height; ++y)
        {
            for(int x = 0;x < width; ++x)
            {
                Destroy(map[x, y]);
                //Destroy(baseMap[x, y]);
                //Destroy(topMap[x, y]);
                typeMap[x, y] = -1;
                charMap[x, y] = -1;
                itemMap[x, y] = -1;
                destMap[x, y] = -1;
                rotationMap[x, y] = 0;
            }
        }
        //create
        width = _width;
        height = _height;
        sizeText.text = "Size : " + width.ToString() + " , " + height.ToString();
        map = new GameObject[width, height];
        //baseMap = new GameObject[width, height];
        //topMap = new GameObject[width, height];
        typeMap = new int[width, height];
        charMap = new int[width, height];
        itemMap = new int[width, height];
        destMap = new int[width, height];
        rotationMap = new int[width, height];
        for (int y = 0;y < height;++y) for(int x = 0;x < width; ++x)
            {
                typeMap[x, y] = -1;
                charMap[x, y] = -1;
                itemMap[x, y] = -1;
                destMap[x, y] = -1;
                rotationMap[x, y] = 0;
            }
        edgeLength = mapLength / Mathf.Max(width, height);
        offsetX = edgeLength * 0.5f + 0.5f;
        offsetY = edgeLength * 0.5f + 0.5f;
        if (width > height) offsetY += edgeLength * 0.5f * (width - height);
        else if (height > width) offsetX += edgeLength * 0.5f * (height - width);

        for (int y = 0;y < height; ++y)
        {
            for(int x = 0;x < width; ++x)
            {
                map[x, y] = Instantiate(blockPref, new Vector3(x * edgeLength + offsetX, y * edgeLength + offsetY, 1), Quaternion.identity);
                map[x,y].transform.GetChild(0).transform.localScale = new Vector3(edgeLength - 0.5f, edgeLength - 0.5f, 0);
                map[x, y].GetComponent<BoxCollider2D>().size = new Vector3(edgeLength - 0.5f, edgeLength - 0.5f, 0);
                map[x, y].GetComponent<Draggable2>().dragEndedCallback = OnDragEnded_wholeBlock;
                //baseMap[x, y] = Instantiate(grid, new Vector3(x * edgeLength + offsetX, y * edgeLength + offsetY, 1), Quaternion.identity);
                //baseMap[x, y].transform.localScale = new Vector3(edgeLength - 0.5f, edgeLength - 0.5f, 0);
                //topMap[x, y] = Instantiate(blankObj, new Vector3(x * edgeLength + offsetX, y * edgeLength + offsetY, 1), Quaternion.identity);
                //topMap[x, y].transform.parent = baseMap[x, y].transform;
            }
        }
    }
    private void SetRotateMap(float x, float y)
    {
        if (x < offsetX - edgeLength * 0.5f * rightClickArea || y < offsetY - edgeLength * 0.5f * rightClickArea) return;
        if (x > offsetX + edgeLength * (width - 1) + edgeLength * 0.5f * rightClickArea ||
            y > offsetY + edgeLength * (height - 1) + edgeLength * 0.5f * rightClickArea) return;
        x = (x - offsetX) / edgeLength;
        y = (y - offsetY) / edgeLength;
        int tarX = (int)(x + 0.5f), tarY = (int)(y + 0.5f);

        if (Vector2.Distance(new Vector2(x, y), new Vector2(tarX, tarY)) <= rightClickArea && typeMap[tarX, tarY] >=0 && typeMap[tarX, tarY] <= 4 )
        {
            map[tarX, tarY].transform.GetChild(0).transform.eulerAngles += new Vector3(0, 0, 90.0f);
            rotationMap[tarX, tarY] = (rotationMap[tarX, tarY] + 1) % 4;
            //Play sound effect
            FindObjectOfType<AudioManager>().PlaySound("BlockRotate");
        }
    }
    public void ResetMap()
    {
        FindObjectOfType<AudioManager>().PlaySound("Click");
        for (int y = 0;y< height; ++y)
        {
            for(int x = 0; x < width;++x)
            {
                typeMap[x, y] = -1;
                charMap[x, y] = -1;
                itemMap[x, y] = -1;
                destMap[x, y] = -1;
                rotationMap[x, y] = 0;
                map[x, y].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = plainSprite;
                map[x, y].transform.GetChild(0).transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
                map[x, y].transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = blankSprite;
                map[x, y].transform.GetChild(0).transform.localScale = new Vector3(edgeLength - 0.5f, edgeLength - 0.5f, 0);
            }
        }
    }
    public void SaveMap()
    {
        FindObjectOfType<AudioManager>().PlaySound("Click");
        int charNum = 0, itemNum = 0, destNum = 0;
        int[] blocks = new int[width * height];
        int[] rotations = new int[width * height];
        for (int y = 0; y < height; ++y) for (int x = 0; x < width; ++x) {
                blocks[x + y * width] = typeMap[x, y];
                rotations[x + y * width] = rotationMap[x, y];
                if (charMap[x, y] != -1) charNum++;
                if (itemMap[x, y] != -1) itemNum++;
                if (destMap[x, y] != -1) destNum++;
        }
        if (destNum == 0 || charNum == 0)
        {
            Debug.LogWarning("[Warning] cow or van are none!!");
            StopAllCoroutines();
            StartCoroutine(IE_bannerShow(0.5f, 0.3f));
            return;
        }
        int[] charTypes = new int[charNum], charPos = new int[charNum * 2];
        int[] itemTypes = new int[itemNum], itemPos = new int[itemNum * 2];
        int[] destTypes = new int[destNum], destPos = new int[destNum * 2];
        int CTidx = 0, ITidx = 0, DTidx = 0;
        for (int y = 0; y < height; ++y) for (int x = 0; x < width; ++x)
            {
                if (charMap[x, y] != -1)
                {
                    charTypes[CTidx] = charMap[x, y];
                    charPos[CTidx * 2] = x;
                    charPos[CTidx * 2 + 1] = y;
                    CTidx++;
                }
                if (itemMap[x, y] != -1)
                {
                    itemTypes[ITidx] = itemMap[x, y];
                    itemPos[ITidx * 2] = x;
                    itemPos[ITidx * 2 + 1] = y;
                    ITidx++;
                }
                if(destMap[x, y] != -1)
                {
                    destTypes[DTidx] = destMap[x, y];
                    destPos[DTidx * 2] = x;
                    destPos[DTidx * 2 + 1] = y;
                    DTidx++;
                }
            }

        MapData map = new MapData(width, height, blocks, rotations, charNum, charTypes, charPos, itemNum, itemTypes, itemPos, destNum, destTypes, destPos);
        if (filenameInputField.text.IndexOf("/") == -1) SaveSystem.SaveMap(map, "Assets/Resources/MapLevel/Level" + filenameInputField.text + ".map");
        else SaveSystem.SaveMap(map, filenameInputField.text);
        Debug.Log("Successful!!");
    }
    public void LoadMap()
    {
        FindObjectOfType<AudioManager>().PlaySound("Click");
        MapData mapData;
        if (filenameInputField.text.IndexOf("/") == -1) mapData = SaveSystem.LoadMap("Assets/Resources/MapLevel/Level" + filenameInputField.text + ".map");
        else mapData = SaveSystem.LoadMap(filenameInputField.text);
        widthSlider.value = mapData.width;
        heightSlider.value = mapData.height;
        UpdateMap(mapData.width, mapData.height);
        //blocks
        for(int i = 0;i < mapData.blocks.Length; ++i)
        {
            if(mapData.blocks[i] != -1)
            {
                int x = i % width, y = i / width;
                map[x, y].tag = "Block";
                map[x, y].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = blockSprites[mapData.blocks[i]];
                map[x, y].transform.GetChild(0).transform.localScale = new Vector3(edgeLength - 0.5f, edgeLength - 0.5f, 0) * blockScales[mapData.blocks[i]];
                typeMap[x, y] = mapData.blocks[i];
                rotationMap[x, y] = mapData.rotations[i];
                map[x, y].transform.GetChild(0).transform.eulerAngles = new Vector3(0, 0, 90.0f) * rotationMap[x, y];
            }
        }
        //char
        for(int i = 0;i < mapData.charNum; ++i)
        {
            charMap[mapData.charPosition[i * 2], mapData.charPosition[i * 2 + 1]] = mapData.characters[i];
            map[mapData.charPosition[i * 2], mapData.charPosition[i * 2 + 1]].transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = charSprites[mapData.characters[i]];
            map[mapData.charPosition[i * 2], mapData.charPosition[i * 2 + 1]].transform.GetChild(1).transform.localScale = new Vector3(edgeLength, edgeLength, 0) * charScales[mapData.characters[i]];
        }
        //item
        for(int i = 0;i < mapData.itemNum; ++i)
        {
            itemMap[mapData.itemPosition[i * 2], mapData.itemPosition[i * 2 + 1]] = mapData.items[i];
            map[mapData.itemPosition[i * 2], mapData.itemPosition[i * 2 + 1]].transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = itemSprites[mapData.items[i]];
            map[mapData.itemPosition[i * 2], mapData.itemPosition[i * 2 + 1]].transform.GetChild(1).transform.localScale = new Vector3(edgeLength, edgeLength, 0) * itemScales[mapData.items[i]];
        }
        //dest
        for(int i = 0;i < mapData.destNum; ++i)
        {
            destMap[mapData.destPosition[i * 2], mapData.destPosition[i * 2 + 1]] = mapData.destinations[i];
            map[mapData.destPosition[i * 2], mapData.destPosition[i * 2 + 1]].transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = destSprites[mapData.destinations[i]];
            map[mapData.destPosition[i * 2], mapData.destPosition[i * 2 + 1]].transform.GetChild(1).transform.localScale = new Vector3(edgeLength, edgeLength, 0) * destScales[mapData.destinations[i]];
        }
    }
    public void loadStartMenu()
    {
        FindObjectOfType<AudioManager>().PlaySound("Click");
        SceneManager.LoadScene(0);
    }

    IEnumerator IE_bannerShow(float fadingDuration, float pauseDuration)
    {
        float counter = 0;
        while(counter < fadingDuration)
        {
            counter += Time.deltaTime;
            if (counter > fadingDuration) counter = fadingDuration;
            messageBanner.GetComponent<CanvasGroup>().alpha = counter / fadingDuration;
            yield return null;
        }

        counter = 0;
        while(counter < pauseDuration)
        {
            counter += Time.deltaTime;
            yield return null;
        }

        counter = 0;
        while (counter < fadingDuration)
        {
            counter += Time.deltaTime;
            if (counter > fadingDuration) counter = fadingDuration;
            messageBanner.GetComponent<CanvasGroup>().alpha = 1 - counter / fadingDuration;
            yield return null;
        }
        messageBanner.GetComponent<CanvasGroup>().alpha = 0;
    }
}
