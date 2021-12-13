using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapLoader : MonoBehaviour
{
    private Vector2 position;
    public string filename = "";
    [SerializeField] private Image blockPref;
    [SerializeField] private List<Sprite> blockSprites;
    [SerializeField] private List<Sprite> charSprites;
    [SerializeField] private List<Sprite> itemSprites;
    [SerializeField] private List<Sprite> destSprites;
    [SerializeField] private Sprite lockedImage;

    Image[,] baseMap = null, topMap = null;
    private int width = 0, height = 0;
    private float offsetX, offsetY;
    private float edgeLength = 4.0f;
    [SerializeField] private float mapLength;
    private List<Color> portalColor;

    private void Start()
    {
        portalColor = new List<Color>();
        portalColor.Add(new Color(1, 1, 1, 1));
        for (float i = 1.0f; i >= 0; i -= 0.2f)
        {
            portalColor.Add(new Color(i, 0, 0, 1));
            portalColor.Add(new Color(0, i, 0, 1));
            portalColor.Add(new Color(0, 0, i, 1));
            portalColor.Add(new Color(i, i, 0, 1));
            portalColor.Add(new Color(i, 0, i, 1));
            portalColor.Add(new Color(0, i, i, 1));
        }
        portalColor.Add(new Color(0, 0, 0, 1));


        LoadMap();
    }
    void ClearMap()
    {
        if (baseMap != null) for (int i = 0; i < width; ++i) for (int j = 0; j < height; ++j) Destroy(baseMap[i, j]);
        if (topMap != null) for (int i = 0; i < width; ++i) for (int j = 0; j < height; ++j) Destroy(topMap[i, j]);
    }
    void ResetMap()
    {
        //create
        baseMap = new Image[width, height];
        topMap = new Image[width, height];
        edgeLength = mapLength / Mathf.Max(width, height);
        offsetX = edgeLength * (Mathf.Max(width, height) / -2.0f + 0.5f);
        offsetY = edgeLength * (Mathf.Max(width, height) / -2.0f + 0.5f);
        if (width > height) offsetY += edgeLength * 0.5f * (width - height);
        else if (height > width) offsetX += edgeLength * 0.5f * (height - width);

        for (int y = 0; y < height; ++y)
        {
            for (int x = 0; x < width; ++x)
            {
                baseMap[x, y] = Instantiate(blockPref, new Vector3(x * edgeLength + offsetX, y * edgeLength + offsetY, 1), Quaternion.identity);
                baseMap[x, y].transform.parent = this.transform;
                baseMap[x, y].rectTransform.localScale = new Vector3(1.0f / Mathf.Max(width, height), 1.0f / Mathf.Max(width, height), 1);
                baseMap[x, y].rectTransform.localPosition = new Vector3(x * edgeLength + offsetX, y * edgeLength + offsetY, 1);
                baseMap[x, y].GetComponent<Image>().color = new Color(108 / 255.0f, 70 / 255.0f, 34 / 255.0f);

                topMap[x, y] = Instantiate(blockPref, new Vector3(x * edgeLength + offsetX, y * edgeLength + offsetY, 1), Quaternion.identity);
                topMap[x, y].transform.parent = this.transform;
                topMap[x, y].rectTransform.localScale = new Vector3(1.0f / Mathf.Max(width, height), 1.0f / Mathf.Max(width, height), 1) * 0.275f;
                topMap[x, y].rectTransform.localPosition = new Vector3(x * edgeLength + offsetX, y * edgeLength + offsetY, 1);
                topMap[x, y].GetComponent<Image>().color = new Color(1, 1, 1, 0);
            }
        }
    }
    public void LoadMap()
    {
        MapData mapData;
        if (filename.IndexOf("/") == -1) mapData = SaveSystem.LoadMap("Assets/Resources/MapLevel/Level" + filename + ".map");
        else mapData = SaveSystem.LoadMap(filename);
        if (mapData == null)
        {
            return;
        }
        ClearMap();
        width = mapData.width;
        height = mapData.height;
        ResetMap();

        //blocks
        for (int i = 0; i < mapData.blocks.Length; ++i)
        {
            if (mapData.blocks[i] != -1)
            {
                int x = i % width, y = i / width;
                baseMap[x, y].GetComponent<Image>().sprite = blockSprites[mapData.blocks[i]];
                baseMap[x, y].transform.eulerAngles = new Vector3(0, 0, 90.0f) * mapData.rotations[i];
                baseMap[x, y].GetComponent<Image>().color = new Color(1, 1, 1, 1);
            }
        }
        //char
        for (int i = 0; i < mapData.charNum; ++i)
        {
            topMap[mapData.charPosition[i * 2], mapData.charPosition[i * 2 + 1]].GetComponent<Image>().sprite = charSprites[mapData.characters[i]];
            topMap[mapData.charPosition[i * 2], mapData.charPosition[i * 2 + 1]].GetComponent<Image>().color = new Color(1, 1, 1, 1);
        }
        //item
        for (int i = 0; i < mapData.itemNum; ++i)
        {
            topMap[mapData.itemPosition[i * 2], mapData.itemPosition[i * 2 + 1]].GetComponent<Image>().sprite = itemSprites[mapData.items[i]];
            topMap[mapData.itemPosition[i * 2], mapData.itemPosition[i * 2 + 1]].GetComponent<Image>().color = new Color(1, 1, 1, 1);
        }
        //portal
        for (int i = 0; i < mapData.portalNum; ++i)
        {
            topMap[mapData.portalPosition[i * 2], mapData.portalPosition[i * 2 + 1]].GetComponent<Image>().color = portalColor[i / 2];
        }
        //dest
        for (int i = 0; i < mapData.destNum; ++i)
        {
            topMap[mapData.destPosition[i * 2], mapData.destPosition[i * 2 + 1]].GetComponent<Image>().sprite = destSprites[mapData.destinations[i]];
            topMap[mapData.destPosition[i * 2], mapData.destPosition[i * 2 + 1]].GetComponent<Image>().color = new Color(1, 1, 1, 1);
        }
    }
    public void UpdateMap(string _filename)
    {
        if(_filename == "Locked")
        {
            ClearMap();
            GetComponent<Image>().sprite = lockedImage;
        }
        else
        {
            GetComponent<Image>().sprite = null;
            filename = _filename;
            LoadMap();
        }
    }
}
