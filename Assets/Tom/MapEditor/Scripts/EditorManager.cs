using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class EditorManager : MonoBehaviour
{
    [SerializeField] private GameObject grid;
    [SerializeField] private GameObject blankObj;
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
    // [SerializeField] private float puttingArea = 0.5f;
    [SerializeField] private float acceptableArea = 0.7f;
    [SerializeField] private float rightClickArea = 0.6f;



    public int width = 4, height = 4;
    private float offsetX, offsetY;
    private float edgeLength = 4.0f;
    private float mapLength = 48.0f;
    GameObject[,] baseMap;
    GameObject[,] topMap;
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
        draggable.dragEndedCallback = OnDragEnded_block;
        for (int i = 0; i < blocks.Count; ++i) blocks[i].dragEndedCallback = OnDragEnded_block;
        for (int i = 0; i < characters.Count; ++i) characters[i].dragEndedCallback = OnDragEnded_character;
        for (int i = 0; i < items.Count; ++i) items[i].dragEndedCallback = OnDragEnded_item;
        for (int i = 0; i < dests.Count; ++i) dests[i].dragEndedCallback = OnDragEnded_dest;
        for (int i = 0; i < trash_cans.Count; ++i) trash_cans[i].dragEndedCallback = OnDragEnded_TC;

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
    private void OnDragEnded_block(Vector2 position, int type)
    {
        Vector2Int pos = GetPos(position.x, position.y);
        if (pos.x == -1 || pos.y == -1) return;

        baseMap[pos.x, pos.y].GetComponent<SpriteRenderer>().sprite = blockSprites[type];
        baseMap[pos.x, pos.y].transform.localScale = new Vector3(edgeLength - 0.5f, edgeLength - 0.5f, 0) * blockScales[type];
        typeMap[pos.x, pos.y] = type;

        if (type < 0 || type > 4)
        {
            baseMap[pos.x, pos.y].transform.eulerAngles = new Vector3(0, 0, 0);
            rotationMap[pos.x, pos.y] = 0;
            OnDragEnded_TC(position, 1); //clear obj
        }
    }
    private void OnDragEnded_character(Vector2 position, int type)
    {
        Vector2Int pos = GetPos(position.x, position.y);
        if (pos.x == -1 || pos.y == -1) return;
        if (typeMap[pos.x, pos.y] < 0 || typeMap[pos.x, pos.y] > 4) return;
        topMap[pos.x, pos.y].GetComponent<SpriteRenderer>().sprite = charSprites[type];
        topMap[pos.x, pos.y].transform.localScale = new Vector3(edgeLength, edgeLength, 0) * charScales[type];
        charMap[pos.x, pos.y] = type;
        itemMap[pos.x, pos.y] = -1;
        destMap[pos.x, pos.y] = -1;
    }
    private void OnDragEnded_item(Vector2 position, int type)
    {
        Vector2Int pos = GetPos(position.x, position.y);
        if (pos.x == -1 || pos.y == -1) return;
        if (typeMap[pos.x, pos.y] < 0 || typeMap[pos.x, pos.y] > 4) return;
        topMap[pos.x, pos.y].GetComponent<SpriteRenderer>().sprite = itemSprites[type];
        topMap[pos.x, pos.y].transform.localScale = new Vector3(edgeLength, edgeLength, 0) * itemScales[type];
        charMap[pos.x, pos.y] = -1;
        itemMap[pos.x, pos.y] = type;
        destMap[pos.x, pos.y] = -1;
    }
    private void OnDragEnded_dest(Vector2 position, int type)
    {
        Vector2Int pos = GetPos(position.x, position.y);
        if (pos.x == -1 || pos.y == -1) return;
        if (typeMap[pos.x, pos.y] < 0 || typeMap[pos.x, pos.y] > 4) return;
        topMap[pos.x, pos.y].GetComponent<SpriteRenderer>().sprite = destSprites[type];
        topMap[pos.x, pos.y].transform.localScale = new Vector3(edgeLength, edgeLength, 0) * destScales[type];
        charMap[pos.x, pos.y] = -1;
        itemMap[pos.x, pos.y] = -1;
        destMap[pos.x, pos.y] = type;

    }
    private void OnDragEnded_TC(Vector2 position, int type)
    {
        Vector2Int pos = GetPos(position.x, position.y);
        if (pos.x == -1 || pos.y == -1) return;
        if(type == 0)  //clear blocks + item
        {
            baseMap[pos.x, pos.y].GetComponent<SpriteRenderer>().sprite = plainSprite;
            baseMap[pos.x, pos.y].transform.localScale = new Vector3(edgeLength - 0.5f, edgeLength - 0.5f, 0);
            topMap[pos.x, pos.y].GetComponent<SpriteRenderer>().sprite = blankSprite;
            typeMap[pos.x, pos.y] = -1;
            rotationMap[pos.x, pos.y] = -1;
            itemMap[pos.x, pos.y] = -1;
            charMap[pos.x, pos.y] = -1;
            destMap[pos.x, pos.y] = -1;
        }
        else if(type == 1)  //clear item
        {
            topMap[pos.x, pos.y].GetComponent<SpriteRenderer>().sprite = blankSprite;
            itemMap[pos.x, pos.y] = -1;
            charMap[pos.x, pos.y] = -1;
            destMap[pos.x, pos.y] = -1;
        }
    }
    void CreateMap()
    {
        //create
        baseMap = new GameObject[width, height];
        topMap = new GameObject[width, height];
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
                baseMap[x, y] = Instantiate(grid, new Vector3(x * edgeLength + offsetX , y * edgeLength + offsetY, 1), Quaternion.identity);
                baseMap[x, y].transform.localScale = new Vector3(edgeLength - 0.5f, edgeLength- 0.5f, 0);
                topMap[x ,y] = Instantiate(blankObj, new Vector3(x * edgeLength + offsetX, y * edgeLength + offsetY, 1), Quaternion.identity);

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
                Destroy(baseMap[x, y]);
                Destroy(topMap[x, y]);
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
        baseMap = new GameObject[width, height];
        topMap = new GameObject[width, height];
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
                baseMap[x, y] = Instantiate(grid, new Vector3(x * edgeLength + offsetX, y * edgeLength + offsetY, 1), Quaternion.identity);
                baseMap[x, y].transform.localScale = new Vector3(edgeLength - 0.5f, edgeLength - 0.5f, 0);
                topMap[x, y] = Instantiate(blankObj, new Vector3(x * edgeLength + offsetX, y * edgeLength + offsetY, 1), Quaternion.identity);

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
            baseMap[tarX, tarY].transform.eulerAngles += new Vector3(0, 0, 90.0f);
            rotationMap[tarX, tarY] = (rotationMap[tarX, tarY] + 1) % 4;
        }
    }
    public void ResetMap()
    {
        for(int y = 0;y< height; ++y)
        {
            for(int x = 0; x < width;++x)
            {
                typeMap[x, y] = -1;
                charMap[x, y] = -1;
                itemMap[x, y] = -1;
                destMap[x, y] = -1;
                rotationMap[x, y] = 0;
                baseMap[x, y].GetComponent<SpriteRenderer>().sprite = plainSprite;
                topMap[x, y].GetComponent<SpriteRenderer>().sprite = blankSprite;
                baseMap[x, y].transform.localScale = new Vector3(edgeLength - 0.5f, edgeLength - 0.5f, 0);
            }
        }
    }
    public void SaveMap()
    {
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
        if (destNum == 0 || charNum == 0) Debug.LogWarning("[Warning] cow or van are none!!");
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
        if (filenameInputField.text.IndexOf("/") == -1) SaveSystem.SaveMap(map, "Assets/MapLevel/Level" + filenameInputField.text + ".map");
        else SaveSystem.SaveMap(map, filenameInputField.text);
        Debug.Log("Successful!!");
    }
    public void LoadMap()
    {
        MapData map;
        if (filenameInputField.text.IndexOf("/") == -1) map = SaveSystem.LoadMap("Assets/MapLevel/Level" + filenameInputField.text + ".map");
        else map = SaveSystem.LoadMap(filenameInputField.text);
        widthSlider.value = map.width;
        heightSlider.value = map.height;
        UpdateMap(map.width, map.height);
        //blocks
        for(int i = 0;i < map.blocks.Length; ++i)
        {
            if(map.blocks[i] != -1)
            {
                int x = i % width, y = i / width;
                baseMap[x, y].transform.localScale = new Vector3(edgeLength - 0.5f, edgeLength - 0.5f, 0) / 5.82f;
                baseMap[x, y].GetComponent<SpriteRenderer>().sprite = blockSprites[map.blocks[i]];
                typeMap[x, y] = map.blocks[i];
                rotationMap[x, y] = map.rotations[i];
                baseMap[x, y].transform.eulerAngles = new Vector3(0, 0, 90.0f) * rotationMap[x, y];
            }
        }
        //char
        for(int i = 0;i < map.charNum; ++i)
        {
            charMap[map.charPosition[i * 2], map.charPosition[i * 2 + 1]] = map.characters[i];
            topMap[map.charPosition[i * 2], map.charPosition[i * 2 + 1]].GetComponent<SpriteRenderer>().sprite = charSprites[map.characters[i]];
        }
        //item
        for(int i = 0;i < map.itemNum; ++i)
        {
            itemMap[map.itemPosition[i * 2], map.itemPosition[i * 2 + 1]] = map.items[i];
            topMap[map.itemPosition[i * 2], map.itemPosition[i * 2 + 1]].GetComponent<SpriteRenderer>().sprite = itemSprites[map.items[i]];
        }
        //dest
        for(int i = 0;i < map.destNum; ++i)
        {
            destMap[map.destPosition[i * 2], map.destPosition[i * 2 + 1]] = map.destinations[i];
            topMap[map.destPosition[i * 2], map.destPosition[i * 2 + 1]].GetComponent<SpriteRenderer>().sprite = destSprites[map.destinations[i]];
        }
    }
    public void loadStartMenu()
    {
        SceneManager.LoadScene(0);
    }
}
