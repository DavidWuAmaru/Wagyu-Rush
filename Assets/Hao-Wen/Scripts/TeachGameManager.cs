using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;
using UnityEngine.SceneManagement;

public class TeachGameManager : MonoBehaviour
{
    //information for movement
    public enum Direction { Up, Down, Left, Right }
    private Vector2Int[] movement = new Vector2Int[4] { new Vector2Int(0, 1), new Vector2Int(0, -1), new Vector2Int(-1, 0), new Vector2Int(1, 0) };
    private int[] movementX = new int[4] { 0, 0, -1, 1 };
    private int[] movementY = new int[4] { 1, -1, 0, 0 };

    //class definition

    #region SerializeField
    [SerializeField] private Camera mainCamera;
    [SerializeField] private List<GameObject> blockTypes;
    [SerializeField] private List<GameObject> characterTypes;
    [SerializeField] private List<GameObject> itemTypes;
    [SerializeField] private List<GameObject> destinationTypes;
    [SerializeField] private Vector2Int mapSize;
    [SerializeField] private float blockMoveSpeed = 30.0f;   //block moving speed (exp)
    [SerializeField] private float characterMoveSpeed = 100.0f; //cow moving speed (linear)
    [SerializeField] private float blockRotateSpeed = 15.0f;  //block rotating speed (exp)
    [SerializeField] private TMP_Text levelBoard;
    [SerializeField] private Canvas initialUI, ResultUI, SettingUI;
    [SerializeField] private TMP_Text ResultUI_satietyBoard;
    [SerializeField] private Text ResultUI_wagyuGradingBoard;
    [SerializeField] private Button ResultUI_NextLevel;
    [SerializeField] private GameObject UnlockNewWorldObj;
    [SerializeField] private TMP_Text UnlockNewWorldText;
    [SerializeField] private Button settingButton;
    //test
    [SerializeField] private bool usingCustomMap = true;
    private bool[] itemReusability;
    private List<Sprite> cowSprites;

    private int currentWorld = 0;
    private int currentLevel = 0;
    //satiety
    [SerializeField] private Slider satietySlider;
    [SerializeField] private int stepAllowed = 40;
    private float satietyMax = 1000.0f;
    private float satiety = 1000.0f, satietyTar = 1000.0f;

    //Target Indicator
    [SerializeField] private Image targetBoardImage;
    [SerializeField] private TMP_Text targetBoardText;
    //Cow status
    [SerializeField] private Image resultCowImage;
    [SerializeField] private Sprite[] resultCowPics;
    [SerializeField] private Image cowEmojiImage;
    [SerializeField] private Sprite[] cowEmojiPics;

    //Grading boards
    [SerializeField] private Image[] scoringBoards;
    [SerializeField] private Sprite[] scoringPics;

    [SerializeField] private Text dialogTxt;
    [SerializeField] [Multiline]private string[] dialogs;
    [SerializeField] private GameObject dialogBox;
    [SerializeField] private GameObject dialogSecretary;
    [SerializeField] private GameObject buttonsIcon;
    [SerializeField] private KeyCode dialogKey;
    [SerializeField] private Animator btnsAnim;
    [SerializeField] private GameObject levelMask;
    [SerializeField] private GameObject canvasMask;

    [SerializeField] private GameObject closingPrefab;

    #endregion

    private List<bool[]> oriBlockAccessible;
    private float mainMapLength = 48.0f;
    private float blockEdgeLength = 8.0f;
    private float mapOffsetX = 0.5f, mapOffsetY = 0.5f;
    private Vector3 mapOffset;
    private bool m_enabled = true;
    private List<Block> blocks;
    private List<Character> characters;
    private List<Item> items;
    private List<Destination> destinations;
    private int[,] assistMap;
    private static string levelFilename;
    private int stepIndicator;
    private bool needPressRight;
    private bool needPressDown;
    private bool isTraining;
    private int difficulty;
    private int stepCount = 0, stepMax = 40;

    // Start is called before the first frame update
    void Start()
    {
        //initialization
        blocks = new List<Block>();
        characters = new List<Character>();
        items = new List<Item>();
        destinations = new List<Destination>();
        oriBlockAccessible = new List<bool[]>();
        oriBlockAccessible.Add(new bool[4] { true, false, false, false });
        oriBlockAccessible.Add(new bool[4] { true, true, false, false });
        oriBlockAccessible.Add(new bool[4] { true, false, false, true });
        oriBlockAccessible.Add(new bool[4] { true, true, true, false });
        oriBlockAccessible.Add(new bool[4] { true, true, true, true });
        oriBlockAccessible.Add(new bool[4] { false, false, false, false });
        oriBlockAccessible.Add(new bool[4] { false, false, false, false });
        itemReusability = new bool[] { false, false, false, false, true };
        if (usingCustomMap) LoadExistingMap(DataManager.mapAddress[0, 0]);
        else LoadRandomMap();

        //change BGM
        FindObjectOfType<AudioManager>().StopMusic("BGM");
        FindObjectOfType<AudioManager>().PlayMusic("BGM_game");

        levelBoard.text = "Level : 1-1";
        difficulty = 4;

        //load cow sprites
        cowSprites = new List<Sprite>();
        cowSprites.Add(Resources.Load<Sprite>("cow_up"));
        cowSprites.Add(Resources.Load<Sprite>("cow_down"));
        cowSprites.Add(Resources.Load<Sprite>("cow_left"));
        cowSprites.Add(Resources.Load<Sprite>("cow_right"));

        stepIndicator = 0;
        m_enabled = false;
        needPressRight = false;
        needPressDown = false;
        isTraining = true;
        buttonsIcon.SetActive(false);
        canvasMask.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(stepIndicator < 4)
        {
            //�e�|�Ӥ奻
            dialogTxt.text = dialogs[stepIndicator];
            if(Input.GetKeyDown(dialogKey))
            {
                stepIndicator++; 
            }
        }
        else if (stepIndicator == 4)
        {
            //�s���a���k
            dialogBox.SetActive(false);
            buttonsIcon.SetActive(true);
            btnsAnim.SetTrigger("PressRight");

            m_enabled = true;
            needPressRight = true;
        }
        else if (stepIndicator == 5)
        {
            //���a���k��
            btnsAnim.SetTrigger("Back");
            dialogBox.SetActive(true);
            buttonsIcon.SetActive(false);
            m_enabled = false;
            needPressRight = false;
            levelMask.SetActive(true);

            dialogTxt.text = dialogs[stepIndicator];
            if (Input.GetKeyDown(dialogKey))
            {
                stepIndicator++;
            }
        }
        else if (stepIndicator == 6)
        {
            dialogTxt.text = dialogs[stepIndicator];
            if (Input.GetKeyDown(dialogKey))
            {
                stepIndicator++;
            }
        }
        else if(stepIndicator == 7)
        {
            //�s���a���U
            levelMask.SetActive(false);
            dialogBox.SetActive(false);
            buttonsIcon.SetActive(true);
            btnsAnim.SetTrigger("PressDown");

            m_enabled = true;
            needPressDown = true;
        }
        else if(stepIndicator == 8)
        {
            //���a���U��
            btnsAnim.SetTrigger("Back");
            dialogBox.SetActive(true);
            buttonsIcon.SetActive(false);
            canvasMask.SetActive(true);
            m_enabled = false;
            needPressDown = false;

            dialogTxt.text = dialogs[stepIndicator];
            if (Input.GetKeyDown(dialogKey))
            {
                stepIndicator++;
            }
        }
        else if (stepIndicator == 9)
        {
            dialogTxt.text = dialogs[stepIndicator];
            if (Input.GetKeyDown(dialogKey))
            {
                stepIndicator++;
            }
        }
        else if (stepIndicator == 10)
        {
            dialogTxt.text = dialogs[stepIndicator];
            canvasMask.SetActive(false);
            if (Input.GetKeyDown(dialogKey))
            {
                stepIndicator++;
            }
        }
        if (stepIndicator == 11)
        {
            dialogBox.SetActive(false);
            buttonsIcon.SetActive(false);           
            dialogSecretary.SetActive(false);

            isTraining = false;
            m_enabled = true;

            settingButton.gameObject.SetActive(true);

            stepIndicator++;
        }







            if (!m_enabled) return;
        if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))&& isTraining == false)
        {
            StartCoroutine(IE_move(Direction.Up));
        }
        if ((Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) && (needPressDown == true || isTraining == false))
        {
            StartCoroutine(IE_move(Direction.Down));

            if (needPressDown)
            {
                stepIndicator++;
            }
        }
        if ((Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A) )&& isTraining == false)
        {
            StartCoroutine(IE_move(Direction.Left));
        }
        if ((Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D) ) && (needPressRight == true || isTraining == false))
        {
            StartCoroutine(IE_move(Direction.Right));

            if(needPressRight)
            {
                stepIndicator++;
            }
        }
        //Satiety update
        //AddSatiety(-0.005f);
        satiety = Mathf.Lerp(satiety, satietyTar, 0.05f);
        satietySlider.value = satiety;
    }

    private void ClearAllObjs()
    {
        //Clear objects
        StopAllCoroutines();
        for (int i = 0; i < blocks.Count; ++i) Destroy(blocks[i].entity);
        for (int i = 0; i < characters.Count; ++i) Destroy(characters[i].entity);
        for (int i = 0; i < items.Count; ++i) Destroy(items[i].entity);
        for (int i = 0; i < destinations.Count; ++i) Destroy(destinations[i].entity);
        blocks.Clear();
        characters.Clear();
        items.Clear();
        destinations.Clear();
        m_enabled = true;
        SetStep(0);
    }
    private void LoadExistingMap(string filename)
    {
        ClearAllObjs();
        MapData mapData = SaveSystem.LoadMap(filename);
        difficulty = mapData.difficulty;
        stepMax = DataManager.levelGradings[difficulty, DataManager.levelGradingCount];
        mapSize.x = mapData.width; mapSize.y = mapData.height;
        assistMap = new int[mapSize.x, mapSize.y];
        //update edge length
        blockEdgeLength = mainMapLength / Mathf.Max(mapSize.x, mapSize.y);
        mapOffsetX = blockEdgeLength * 0.5f + 0.5f;
        mapOffsetY = blockEdgeLength * 0.5f + 0.5f;
        if (mapSize.x > mapSize.y) mapOffsetY += blockEdgeLength * 0.5f * (mapSize.x - mapSize.y);
        else mapOffsetX += blockEdgeLength * 0.5f * (mapSize.y - mapSize.x);
        mapOffset = new Vector3(mapOffsetX, mapOffsetY, 0);

        //load blocks
        for (int y = 0; y < mapSize.y; ++y)
        {
            for (int x = 0; x < mapSize.x; ++x)
            {
                if (mapData.blocks[x + y * mapSize.x] == -1) continue;
                Block newBlock = new Block();
                newBlock.entity = Instantiate(blockTypes[mapData.blocks[x + y * mapSize.x]], new Vector3(x * blockEdgeLength + mapOffsetX, y * blockEdgeLength + mapOffsetY, 0), Quaternion.identity);
                newBlock.entity.transform.localScale = new Vector3(blockEdgeLength, blockEdgeLength, 1);
                newBlock.type = (Block.Type)mapData.blocks[x + y * mapSize.x];
                newBlock.position = new Vector2Int(x, y);
                newBlock.destination = newBlock.entity.transform.position;
                for (int i = 0; i < 4; ++i) newBlock.accessible[i] = oriBlockAccessible[(int)newBlock.type][i];
                for (int i = mapData.rotations[x + y * mapSize.x]; i > 0; --i) blockRotateLeft(ref newBlock);
                blocks.Add(newBlock);
            }
        }

        //Load characters
        characters.Clear();
        for (int i = 0; i < mapData.charNum; ++i)
        {
            Character cow = new Character();
            cow.type = (Character.Type)mapData.characters[i];
            cow.position = new Vector2Int(mapData.charPosition[i * 2], mapData.charPosition[i * 2 + 1]);
            cow.entity = Instantiate(characterTypes[mapData.characters[i]], new Vector3(cow.position.x * blockEdgeLength + mapOffsetX, cow.position.y * blockEdgeLength + mapOffsetY, 0), Quaternion.identity);
            cow.entity.transform.localScale = new Vector3(blockEdgeLength, blockEdgeLength, 1);
            cow.destination = cow.entity.transform.position;
            cow.entity.GetComponent<CharFunc>().collectItemCallback = onCollectingItem;
            characters.Add(cow);
        }
        //load items
        items.Clear();
        for (int i = 0; i < mapData.itemNum; ++i)
        {
            if ((Item.Type)mapData.items[i] == Item.Type.Portal) continue;
            Item newItem = new Item();
            newItem.type = (Item.Type)mapData.items[i];
            newItem.position = new Vector2Int(mapData.itemPosition[i * 2], mapData.itemPosition[i * 2 + 1]);
            newItem.entity = Instantiate(itemTypes[mapData.items[i]], new Vector3(newItem.position.x * blockEdgeLength + mapOffsetX, newItem.position.y * blockEdgeLength + mapOffsetY, 0), Quaternion.identity);
            newItem.entity.transform.localScale = new Vector3(blockEdgeLength, blockEdgeLength, 1);
            newItem.destination = newItem.entity.transform.position;
            items.Add(newItem);
        }
        //load destinations
        destinations.Clear();
        for (int i = 0; i < mapData.destNum; ++i)
        {
            Destination newDest = new Destination();
            newDest.type = (Destination.Type)mapData.destinations[i];
            newDest.position = new Vector2Int(mapData.destPosition[i * 2], mapData.destPosition[i * 2 + 1]);
            newDest.entity = Instantiate(destinationTypes[mapData.destinations[i]], new Vector3(newDest.position.x * blockEdgeLength + mapOffsetX, newDest.position.y * blockEdgeLength + mapOffsetY, 0), Quaternion.identity);
            newDest.entity.transform.localScale = new Vector3(blockEdgeLength, blockEdgeLength, 1);
            newDest.destination = newDest.entity.transform.position;
            destinations.Add(newDest);
        }

        updateTargetBoard();
    }

    private void updateTargetBoard()
    {
        int ticketCount = 0;
        for (int i = 0; i < items.Count; ++i) if (items[i].type == Item.Type.Key) ticketCount++;
        if (ticketCount > 0)
        {
            targetBoardImage.GetComponent<Image>().sprite = itemTypes[(int)(Item.Type.Key)].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
            //targetBoardText.text = "x" + ticketCount.ToString();
        }
        else
        {
            targetBoardImage.GetComponent<Image>().sprite = destinationTypes[0].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
            targetBoardText.text = "";
        }
    }

    private void LoadRandomMap()
    {
        ClearAllObjs();
        assistMap = new int[mapSize.x, mapSize.y];
        //update edge length
        blockEdgeLength = mainMapLength / Mathf.Max(mapSize.x, mapSize.y);
        mapOffsetX = blockEdgeLength * 0.5f + 0.5f;
        mapOffsetY = blockEdgeLength * 0.5f + 0.5f;
        if (mapSize.x > mapSize.y) mapOffsetY += blockEdgeLength * 0.5f * (mapSize.x - mapSize.y);
        else mapOffsetX += blockEdgeLength * 0.5f * (mapSize.y - mapSize.x);
        mapOffset = new Vector3(mapOffsetX, mapOffsetY, 0);

        //randomly create the map
        int landNum = 0;
        while (landNum < 2)
        {
            //reset asset
            for (int i = 0; i < blocks.Count; ++i) Destroy(blocks[i].entity);
            blocks.Clear();

            landNum = 0;
            for (int y = 0; y < mapSize.y; ++y)
            {
                for (int x = 0; x < mapSize.x; ++x)
                {
                    int sel = Random.Range(0, 100);
                    if (sel < 30)  //normal block
                    {
                        Block newBlock = new Block();
                        newBlock.entity = Instantiate(blockTypes[(int)(sel / 6)], new Vector3(x * blockEdgeLength + mapOffsetX, y * blockEdgeLength + mapOffsetY, 0), Quaternion.identity);
                        newBlock.entity.transform.localScale = new Vector3(blockEdgeLength, blockEdgeLength, 1);
                        newBlock.type = (Block.Type)(int)(sel / 6);
                        newBlock.position = new Vector2Int(x, y);
                        newBlock.destination = newBlock.entity.transform.position;
                        for (int i = 0; i < 4; ++i) newBlock.accessible[i] = oriBlockAccessible[(int)(sel / 6)][i];
                        //rotate the block
                        for (int i = Random.Range(0, 4); i > 0; --i) blockRotateRight(ref newBlock);
                        blocks.Add(newBlock);
                        landNum++;
                    }
                    else if (sel < 40)  //obstacle
                    {
                        Block newBlock = new Block();
                        newBlock.entity = Instantiate(blockTypes[(int)Block.Type.Obstacle], new Vector3(x * blockEdgeLength + mapOffsetX, y * blockEdgeLength + mapOffsetY, 0), Quaternion.identity);
                        newBlock.entity.transform.localScale = new Vector3(blockEdgeLength, blockEdgeLength, 1);
                        newBlock.type = Block.Type.Obstacle;
                        newBlock.position = new Vector2Int(x, y);
                        newBlock.destination = newBlock.entity.transform.position;
                        blocks.Add(newBlock);
                    }
                    else if (sel < 45)  //rotate
                    {
                        Block newBlock = new Block();
                        newBlock.entity = Instantiate(blockTypes[(int)Block.Type.RotateBlock], new Vector3(x * blockEdgeLength + mapOffsetX, y * blockEdgeLength + mapOffsetY, 0), Quaternion.identity);
                        newBlock.entity.transform.localScale = new Vector3(blockEdgeLength, blockEdgeLength, 1);
                        newBlock.type = Block.Type.RotateBlock;
                        newBlock.position = new Vector2Int(x, y);
                        newBlock.destination = newBlock.entity.transform.position;
                        blocks.Add(newBlock);
                    }
                }
            }
        }

        //Spawning extra components
        int idxCow = Random.Range(0, blocks.Count);
        //Spawn cow
        while (blocks[idxCow].type < Block.Type.Block_U || blocks[idxCow].type > Block.Type.Block_UDRL) idxCow = Random.Range(0, blocks.Count);
        Character cow = new Character();
        cow.type = Character.Type.Cow;
        cow.position = blocks[idxCow].position;
        cow.entity = Instantiate(characterTypes[(int)Character.Type.Cow], new Vector3(cow.position.x * blockEdgeLength + mapOffsetX, cow.position.y * blockEdgeLength + mapOffsetY, 0), Quaternion.identity);
        cow.entity.transform.localScale = new Vector3(blockEdgeLength, blockEdgeLength, 1);
        cow.destination = cow.entity.transform.position;
        cow.entity.GetComponent<CharFunc>().collectItemCallback = onCollectingItem;
        characters.Add(cow);
        //Spawn van
        int idxVan = Random.Range(0, blocks.Count);
        while (blocks[idxVan].type < Block.Type.Block_U || blocks[idxVan].type > Block.Type.Block_UDRL || idxVan == idxCow) idxVan = Random.Range(0, blocks.Count);
        Destination van = new Destination();
        van.type = Destination.Type.Van;
        van.position = blocks[idxVan].position;
        van.entity = Instantiate(destinationTypes[(int)Destination.Type.Van], new Vector3(van.position.x * blockEdgeLength + mapOffsetX, van.position.y * blockEdgeLength + mapOffsetY, 0), Quaternion.identity);
        van.entity.transform.localScale = new Vector3(blockEdgeLength, blockEdgeLength, 1);
        van.destination = van.entity.transform.position;
        destinations.Add(van);
    }

    private void onCollectingItem(GameObject srcGameObject, GameObject tarGameObject, bool isItem)
    {
        if (isItem)  //deal with item
        {
            for (int i = 0; i < items.Count; ++i) if (items[i].entity == tarGameObject)
                {
                    if (items[i].type == Item.Type.Key)
                    {
                        //play sound effect
                        FindObjectOfType<AudioManager>().PlaySound("CollectTag");

                        srcGameObject.transform.GetChild(3).gameObject.SetActive(false);
                        srcGameObject.transform.GetChild(3).gameObject.SetActive(true);

                        Debug.Log("Collect a key");
                    }
                    else if (items[i].type == Item.Type.HayStack)
                    {
                        //play particle system
                        srcGameObject.transform.position = tarGameObject.transform.position;
                        srcGameObject.transform.GetChild(2).gameObject.SetActive(false);
                        srcGameObject.transform.GetChild(2).gameObject.SetActive(true);

                        AddStep(-5);
                    }
                    else if (items[i].type == Item.Type.Trap)
                    {
                        //play particle system
                        srcGameObject.transform.position = tarGameObject.transform.position;
                        srcGameObject.transform.GetChild(1).gameObject.SetActive(false);
                        srcGameObject.transform.GetChild(1).gameObject.SetActive(true);

                        AddStep(5);
                    }
                    else if (items[i].type == Item.Type.HeadPhone)
                    {
                        for (int j = 0; j < characters.Count; ++j) if (characters[j].entity == srcGameObject)
                            {
                                srcGameObject.transform.GetChild(0).GetComponent<Animator>().enabled = true;
                                characters[j].position = items[i].position;
                                characters[j].destination = items[i].entity.transform.position;
                                characters[j].freeze += 3;
                            }
                    }
                    else if (items[i].type == Item.Type.Portal)
                    {
                        for (int j = 0; j < characters.Count; ++j) if (characters[j].entity == srcGameObject)
                            {
                                for (int k = 0; k < items.Count; ++k)
                                {
                                    if (items[k].type == Item.Type.Portal && i != k)
                                    {
                                        items[i].entity.GetComponent<BoxCollider2D>().enabled = false;
                                        items[k].entity.GetComponent<BoxCollider2D>().enabled = false;
                                        characters[j].destination = items[i].entity.transform.position;
                                        StartCoroutine(IE_swap(characters[j], items[k].position));
                                    }
                                }
                            }
                    }
                    if (!itemReusability[(int)items[i].type])
                    {
                        Destroy(items[i].entity);
                        items.RemoveAt(i);
                    }
                    break;
                }
        }
        else  //deal with dest
        {
            for (int i = 0; i < destinations.Count; ++i) if (destinations[i].entity == tarGameObject)
                {
                    if (destinations[i].type == Destination.Type.Van)
                    {
                        bool existKey = false;
                        for (int j = 0; j < items.Count; ++j)
                        {
                            if (items[j].type == Item.Type.Key)
                            {
                                existKey = true;
                                break;
                            }
                        }
                        if (!existKey)  //pass the map
                        {
                            for (int j = 0; j < characters.Count; ++j)
                            {
                                if (characters[j].entity == srcGameObject)
                                {
                                    Destroy(characters[j].entity);
                                    characters.RemoveAt(j);
                                }
                            }
                        }
                    }
                    break;
                }
        }
        updateTargetBoard();
    }
    IEnumerator IE_swap(Character ch, Vector2Int destPos)
    {
        float scalar = 1.0f, step = 1.5f;
        float rotator = 0.0f, rstep = 360.0f / (scalar / step);
        while (scalar > 0)
        {
            m_enabled = false;
            scalar -= step * Time.deltaTime;
            rotator += rstep * Time.deltaTime;
            ch.entity.transform.localScale = new Vector3(scalar, scalar, 1) * blockEdgeLength;
            ch.entity.transform.eulerAngles = new Vector3(0, 0, rotator);
            yield return null;
        }
        ch.position = destPos;
        ch.entity.transform.position = new Vector3(ch.position.x, ch.position.y, 0) * blockEdgeLength + mapOffset;
        ch.destination = ch.entity.transform.position;
        while (scalar < 1.0f)
        {
            m_enabled = false;
            scalar += step * Time.deltaTime;
            rotator -= rstep * Time.deltaTime;
            ch.entity.transform.localScale = new Vector3(scalar, scalar, 1) * blockEdgeLength;
            ch.entity.transform.eulerAngles = new Vector3(0, 0, rotator);
            yield return null;
        }

        ch.entity.transform.localScale = new Vector3(blockEdgeLength, blockEdgeLength, 1);
        ch.entity.transform.eulerAngles = new Vector3(0, 0, 0);

        m_enabled = true;
    }

    //helping functions declaration
    #region aid functions
    void blockRotateRight(int idx)
    {
        blocks[idx].entity.transform.eulerAngles += new Vector3(0, 0, 270);
        Swap(ref blocks[idx].accessible[0], ref blocks[idx].accessible[2]);
        Swap(ref blocks[idx].accessible[2], ref blocks[idx].accessible[1]);
        Swap(ref blocks[idx].accessible[1], ref blocks[idx].accessible[3]);
    }
    void blockRotateRight(ref Block target)
    {
        target.entity.transform.eulerAngles += new Vector3(0, 0, 270);
        Swap(ref target.accessible[0], ref target.accessible[2]);
        Swap(ref target.accessible[2], ref target.accessible[1]);
        Swap(ref target.accessible[1], ref target.accessible[3]);
    }
    void blockRotateLeft(int idx)
    {
        blocks[idx].entity.transform.eulerAngles += new Vector3(0, 0, 90);
        Swap(ref blocks[idx].accessible[0], ref blocks[idx].accessible[3]);
        Swap(ref blocks[idx].accessible[3], ref blocks[idx].accessible[1]);
        Swap(ref blocks[idx].accessible[1], ref blocks[idx].accessible[2]);
    }
    void blockRotateLeft(ref Block target)
    {
        target.entity.transform.eulerAngles += new Vector3(0, 0, 90);
        Swap(ref target.accessible[0], ref target.accessible[3]);
        Swap(ref target.accessible[3], ref target.accessible[1]);
        Swap(ref target.accessible[1], ref target.accessible[2]);
    }
    void blockRotateRight_accOnly(ref Block target)
    {
        Swap(ref target.accessible[0], ref target.accessible[2]);
        Swap(ref target.accessible[2], ref target.accessible[1]);
        Swap(ref target.accessible[1], ref target.accessible[3]);
    }
    void blockRotateLeft_accOnly(ref Block target)
    {
        Swap(ref target.accessible[0], ref target.accessible[3]);
        Swap(ref target.accessible[3], ref target.accessible[1]);
        Swap(ref target.accessible[1], ref target.accessible[2]);
    }
    void blockRotateRight_accOnly(int idx)
    {
        Swap(ref blocks[idx].accessible[0], ref blocks[idx].accessible[2]);
        Swap(ref blocks[idx].accessible[2], ref blocks[idx].accessible[1]);
        Swap(ref blocks[idx].accessible[1], ref blocks[idx].accessible[3]);
    }
    void blockRotateLeft_accOnly(int idx)
    {
        Swap(ref blocks[idx].accessible[0], ref blocks[idx].accessible[3]);
        Swap(ref blocks[idx].accessible[3], ref blocks[idx].accessible[1]);
        Swap(ref blocks[idx].accessible[1], ref blocks[idx].accessible[2]);
    }

    private bool outOfRange(Vector2Int pos)
    {
        return pos.x < 0 || pos.y < 0 || pos.x >= mapSize.x || pos.y >= mapSize.y;
    }
    private bool outOfRange(int x, int y)
    {
        return x < 0 || y < 0 || x >= mapSize.x || y >= mapSize.y;
    }
    public static void Swap<T>(ref T lhs, ref T rhs)
    {
        T temp = lhs;
        lhs = rhs;
        rhs = temp;
    }
    #endregion

    //temp var
    float distTolerance = 0.3f;
    float rotTolerance = 0.3f;
    bool validMove = false;
    //temp var for grading properties
    int[,] gradingProbs = new int[4, 11] {
        {0, 0, 0, 0, 0, 0, 0, 0, 5, 40, 100 },
        {0, 0, 0, 0, 0, 0, 5, 40, 95, 100, 0 },
        {0, 0, 0, 0, 5, 25, 80, 100, 0, 0, 0 },
        {60, 95, 100, 0, 0, 0, 0, 0, 0, 0, 0 },
    };
    IEnumerator IE_move(Direction dir)
    {
        m_enabled = false;
        validMove = false;
        bool finished = false;
        //move blocks
        moveMap(dir);

        //play sound effect
        FindObjectOfType<AudioManager>().PlaySound("BlockSlide");

        finished = false;
        while (!finished)
        {
            finished = true;
            //check for blocks
            for (int i = 0; i < blocks.Count; ++i)
            {
                if (Vector3.Distance(blocks[i].entity.transform.position, blocks[i].destination) > distTolerance)
                {
                    blocks[i].entity.transform.position = Vector3.Lerp(blocks[i].entity.transform.position, blocks[i].destination, blockMoveSpeed * Time.deltaTime);
                    finished = false;
                }
                else blocks[i].entity.transform.position = blocks[i].destination;
            }
            //check for characters
            for (int i = 0; i < characters.Count; ++i)
            {
                if (Vector3.Distance(characters[i].entity.transform.position, characters[i].destination) > distTolerance)
                {
                    characters[i].entity.transform.position = Vector3.Lerp(characters[i].entity.transform.position, characters[i].destination, blockMoveSpeed * Time.deltaTime);
                    finished = false;
                }
                else characters[i].entity.transform.position = characters[i].destination;
            }
            //check for items
            for (int i = 0; i < items.Count; ++i)
            {
                if (Vector3.Distance(items[i].entity.transform.position, items[i].destination) > distTolerance)
                {
                    items[i].entity.transform.position = Vector3.Lerp(items[i].entity.transform.position, items[i].destination, blockMoveSpeed * Time.deltaTime);
                    finished = false;
                }
                else items[i].entity.transform.position = items[i].destination;
            }
            //check for destinations
            for (int i = 0; i < destinations.Count; ++i)
            {
                if (Vector3.Distance(destinations[i].entity.transform.position, destinations[i].destination) > distTolerance)
                {
                    destinations[i].entity.transform.position = Vector3.Lerp(destinations[i].entity.transform.position, destinations[i].destination, blockMoveSpeed * Time.deltaTime);
                    finished = false;
                }
                else destinations[i].entity.transform.position = destinations[i].destination;
            }
            if (!finished) yield return null;
        }

        //rotate blocks
        rotateBlockFunc();

        //play sound effect
        FindObjectOfType<AudioManager>().PlaySound("BlockRotate");

        finished = false;
        while (!finished)
        {
            finished = true;
            for (int i = 0; i < blocks.Count; ++i)
            {
                if (blocks[i].RotateAngle > rotTolerance)
                {
                    float rotateAmount = Mathf.Lerp(0, blocks[i].RotateAngle, blockRotateSpeed * Time.deltaTime);
                    blocks[i].entity.transform.eulerAngles += new Vector3(0, 0, rotateAmount);
                    blocks[i].RotateAngle -= rotateAmount;
                    finished = false;
                }
                else
                {
                    blocks[i].entity.transform.eulerAngles += new Vector3(0, 0, blocks[i].RotateAngle);
                    blocks[i].RotateAngle = 0;
                }
            }
            if (!finished) yield return null;
        }

        yield return new WaitForSeconds(0.04f);
        //move characters
        moveCharacter(dir);
        finished = false;
        while (!finished)
        {
            finished = true;
            for (int i = 0; i < characters.Count; ++i)
            {
                if (Vector3.Distance(characters[i].entity.transform.position, characters[i].destination) > distTolerance)
                {
                    characters[i].entity.transform.position = Vector3.MoveTowards(characters[i].entity.transform.position, characters[i].destination, characterMoveSpeed * Time.deltaTime); //
                    finished = false;
                }
                else characters[i].entity.transform.position = characters[i].destination;
            }
            if (!finished) yield return null;
        }

        moveEndEvent();
    }
    private void moveEndEvent()
    {
        for (int i = 0; i < characters.Count; ++i)
        {
            bool OnPortal = false;
            for (int j = 0; j < items.Count; ++j)
            {
                if (items[j].type == Item.Type.Portal && items[j].position == characters[i].position)
                {
                    OnPortal = true;
                    for (int k = 0; k < items.Count; ++k)
                    {
                        if (j != k && items[k].type == Item.Type.Portal) items[i].entity.GetComponent<BoxCollider2D>().enabled = false;
                    }
                    break;
                }
            }
            if (!OnPortal) for (int j = 0; j < items.Count; ++j)
                {
                    items[j].entity.GetComponent<BoxCollider2D>().enabled = true;
                }
        }

        //Update satiety
        if (validMove) AddStep(1);

        //see if all cows are on the car
        if (characters.Count == 0)  //win
        {
            Debug.Log("Win!!!");
            //play sound effect
            FindObjectOfType<AudioManager>().PlaySound("Pass");

            m_enabled = false;
            initialUI.gameObject.SetActive(false);
            SettingUI.gameObject.SetActive(false);
            ResultUI.gameObject.SetActive(true);
            ResultUI_satietyBoard.text = "Satiety : " + (int)(satiety / satietyMax * 100) + "%";

            //update result picture
            int grading = DataManager.levelGradingCount - 1;
            for (int i = 0; i < DataManager.levelGradingCount; ++i) if (stepCount <= DataManager.levelGradings[difficulty, i])
                {
                    grading = i;
                    break;
                }

            Debug.Log(grading);

            ResultUI_wagyuGradingBoard.text = DataManager.wagyuGradings[grading];
            resultCowImage.GetComponent<Image>().sprite = resultCowPics[grading];

            for (int i = 0; i < scoringBoards.Length; ++i)
            {
                int ran = Random.Range(0, 100);
                for (int j = 0; j <= 10; ++j) if (ran < gradingProbs[grading, j])
                    {
                        scoringBoards[i].GetComponent<Image>().sprite = scoringPics[j];
                        break;
                    }
            }

            if (currentLevel + 1 >= DataManager.levelsOfWorld[currentWorld]) ResultUI_NextLevel.gameObject.SetActive(false);
            else ResultUI_NextLevel.gameObject.SetActive(true);

            //update history best
            levelBoard.text = "Level : " + (currentWorld + 1).ToString() + "-" + (currentLevel + 1).ToString();

            PlayerData.Load();
            if (PlayerData.mapInfo.historyBest[currentWorld, currentLevel] == -1 || PlayerData.mapInfo.historyBest[currentWorld, currentLevel] < grading)  //update best score
            {
                PlayerData.mapInfo.historyBest[currentWorld, currentLevel] = grading;
            }
            if (currentWorld + 1 < DataManager.worldSize && currentLevel >= 2 && PlayerData.mapInfo.levelLocked[currentWorld + 1] <= 0)  //unlock next world
            {
                PlayerData.mapInfo.levelLocked[currentWorld + 1] = 1;
                UnlockNewWorldText.text = string.Format("Unlock New World!!!\n{0}", DataManager.worldNames[currentWorld + 1]);
                UnlockNewWorldObj.gameObject.SetActive(true);
            }
            else UnlockNewWorldObj.gameObject.SetActive(false);

            if (currentLevel + 1 < DataManager.levelsOfWorld[currentWorld] && PlayerData.mapInfo.levelLocked[currentWorld] < currentLevel + 2)  //unlock next level
            {
                PlayerData.mapInfo.levelLocked[currentWorld] = currentLevel + 2;
            }
            PlayerData.Save();
            return;
        }

        m_enabled = true;
        updateTargetBoard();
    }
    private void setUpAssistMap()
    {
        //setup the map
        for (int x = 0; x < mapSize.x; ++x) for (int y = 0; y < mapSize.y; ++y) assistMap[x, y] = -1;
        for (int i = 0; i < blocks.Count; ++i)
        {
            int val = -1;
            if (blocks[i].type >= Block.Type.Block_U && blocks[i].type <= Block.Type.Block_UDRL) val = i;
            else if (blocks[i].type == Block.Type.Obstacle) val = -2;
            else if (blocks[i].type == Block.Type.RotateBlock) val = -3;
            assistMap[blocks[i].position.x, blocks[i].position.y] = val;
        }
    }
    private void moveMap(Direction dir)
    {
        int mx = movementX[(int)dir], my = movementY[(int)dir];
        setUpAssistMap();
        if (dir == Direction.Down || dir == Direction.Left)
        {
            for (int y = 0; y < mapSize.y; ++y)
            {
                for (int x = 0; x < mapSize.x; ++x)
                {
                    if (assistMap[x, y] < 0 || outOfRange(x + mx, y + my) || assistMap[x + mx, y + my] != -1) continue;
                    blocks[assistMap[x, y]].position += movement[(int)dir];
                    blocks[assistMap[x, y]].destination = new Vector3(blocks[assistMap[x, y]].position.x * blockEdgeLength + mapOffsetX, blocks[assistMap[x, y]].position.y * blockEdgeLength + mapOffsetY, 0);
                    assistMap[x, y] = -1;
                    validMove = true;
                    //move characters that's on the block
                    for (int i = 0; i < characters.Count; ++i) if (characters[i].position.x == x && characters[i].position.y == y)
                        {
                            characters[i].position += movement[(int)dir];
                            characters[i].destination += new Vector3(mx, my, 0) * blockEdgeLength;
                        }
                    //move items that's on the block
                    for (int i = 0; i < items.Count; ++i) if (items[i].position.x == x && items[i].position.y == y)
                        {
                            items[i].position += movement[(int)dir];
                            items[i].destination += new Vector3(mx, my, 0) * blockEdgeLength;
                        }
                    //move dests that's on the block
                    for (int i = 0; i < destinations.Count; ++i) if (destinations[i].position.x == x && destinations[i].position.y == y)
                        {
                            destinations[i].position += movement[(int)dir];
                            destinations[i].destination += new Vector3(mx, my, 0) * blockEdgeLength;
                        }
                }
            }
        }
        if (dir == Direction.Up || dir == Direction.Right)
        {
            for (int y = mapSize.y - 1; y >= 0; --y)
            {
                for (int x = mapSize.x - 1; x >= 0; --x)
                {
                    if (assistMap[x, y] < 0 || outOfRange(x + mx, y + my) || assistMap[x + mx, y + my] != -1) continue;
                    blocks[assistMap[x, y]].position += movement[(int)dir];
                    blocks[assistMap[x, y]].destination = new Vector3(blocks[assistMap[x, y]].position.x, blocks[assistMap[x, y]].position.y, 0) * blockEdgeLength + mapOffset;
                    assistMap[x, y] = -1;
                    validMove = true;
                    //move characters that's on the block
                    for (int i = 0; i < characters.Count; ++i) if (characters[i].position.x == x && characters[i].position.y == y)
                        {
                            characters[i].position += movement[(int)dir];
                            characters[i].destination += new Vector3(mx, my, 0) * blockEdgeLength;
                        }
                    //move items that's on the block
                    for (int i = 0; i < items.Count; ++i) if (items[i].position.x == x && items[i].position.y == y)
                        {
                            items[i].position += movement[(int)dir];
                            items[i].destination += new Vector3(mx, my, 0) * blockEdgeLength;
                        }
                    //move dests that's on the block
                    for (int i = 0; i < destinations.Count; ++i) if (destinations[i].position.x == x && destinations[i].position.y == y)
                        {
                            destinations[i].position += movement[(int)dir];
                            destinations[i].destination += new Vector3(mx, my, 0) * blockEdgeLength;
                        }
                }
            }
        }
    }

    private void moveCharacter(Direction dir)
    {
        setUpAssistMap();
        if (dir == Direction.Up)
        {
            for (int i = 0; i < characters.Count; ++i)
            {
                if (characters[i].freeze > 0)
                {
                    characters[i].freeze--;
                    if (characters[i].freeze <= 0)
                    {
                        characters[i].entity.transform.GetChild(0).GetComponent<Animator>().enabled = false;
                        characters[i].entity.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = cowSprites[0];
                    }
                    continue;
                }
                int x = characters[i].position.x, y = characters[i].position.y;
                while (y < mapSize.y - 1 && blocks[assistMap[x, y]].accessible[0] && assistMap[x, y + 1] >= 0 && blocks[assistMap[x, y + 1]].accessible[1])
                {
                    y++;
                    validMove = true;
                    characters[i].entity.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = cowSprites[0];
                }
                characters[i].position = new Vector2Int(x, y);
                characters[i].destination = new Vector3(x, y, 0) * blockEdgeLength + new Vector3(mapOffsetX, mapOffsetY, 0);
            }
        }
        if (dir == Direction.Down)
        {
            for (int i = 0; i < characters.Count; ++i)
            {
                if (characters[i].freeze > 0)
                {
                    characters[i].freeze--;
                    if (characters[i].freeze <= 0)
                    {
                        characters[i].entity.transform.GetChild(0).GetComponent<Animator>().enabled = false;
                        characters[i].entity.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = cowSprites[1];
                    }
                    continue;
                }
                int x = characters[i].position.x, y = characters[i].position.y;
                while (y > 0 && blocks[assistMap[x, y]].accessible[1] && assistMap[x, y - 1] >= 0 && blocks[assistMap[x, y - 1]].accessible[0])
                {
                    y--;
                    validMove = true;
                    characters[i].entity.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = cowSprites[1];
                }
                characters[i].position = new Vector2Int(x, y);
                characters[i].destination = new Vector3(x, y, 0) * blockEdgeLength + new Vector3(mapOffsetX, mapOffsetY, 0);
            }
        }
        if (dir == Direction.Left)
        {
            for (int i = 0; i < characters.Count; ++i)
            {
                if (characters[i].freeze > 0)
                {
                    characters[i].freeze--;
                    if (characters[i].freeze <= 0)
                    {
                        characters[i].entity.transform.GetChild(0).GetComponent<Animator>().enabled = false;
                        characters[i].entity.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = cowSprites[2];
                    }
                    continue;
                }
                int x = characters[i].position.x, y = characters[i].position.y;
                while (x > 0 && blocks[assistMap[x, y]].accessible[2] && assistMap[x - 1, y] >= 0 && blocks[assistMap[x - 1, y]].accessible[3])
                {
                    x--;
                    validMove = true;
                    characters[i].entity.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = cowSprites[2];
                }
                characters[i].position = new Vector2Int(x, y);
                characters[i].destination = new Vector3(x, y, 0) * blockEdgeLength + new Vector3(mapOffsetX, mapOffsetY, 0);
            }
        }
        if (dir == Direction.Right)
        {
            for (int i = 0; i < characters.Count; ++i)
            {
                if (characters[i].freeze > 0)
                {
                    characters[i].freeze--;
                    if (characters[i].freeze <= 0)
                    {
                        characters[i].entity.transform.GetChild(0).GetComponent<Animator>().enabled = false;
                        characters[i].entity.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = cowSprites[3];
                    }
                    continue;
                }
                int x = characters[i].position.x, y = characters[i].position.y;
                while (x < mapSize.x - 1 && blocks[assistMap[x, y]].accessible[3] && assistMap[x + 1, y] >= 0 && blocks[assistMap[x + 1, y]].accessible[2])
                {
                    x++;
                    validMove = true;
                    characters[i].entity.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = cowSprites[3];
                }
                characters[i].position = new Vector2Int(x, y);
                characters[i].destination = new Vector3(x, y, 0) * blockEdgeLength + new Vector3(mapOffsetX, mapOffsetY, 0);
            }
        }
    }

    private void rotateBlockFunc()
    {
        setUpAssistMap();
        for (int i = 0; i < blocks.Count; ++i)
        {
            if (blocks[i].type == Block.Type.RotateBlock)
            {
                int x = blocks[i].position.x, y = blocks[i].position.y;
                for (int t = 0; t < 4; ++t)
                {
                    if (!outOfRange(blocks[i].position + movement[t]) && assistMap[x + movementX[t], y + movementY[t]] >= 0)
                    {
                        validMove = true;
                        blocks[assistMap[x + movementX[t], y + movementY[t]]].RotateAngle += 90.0f;
                        blockRotateLeft_accOnly(assistMap[x + movementX[t], y + movementY[t]]);
                    }
                }
            }
        }
    }

    public void ReloadMap()
    {
        FindObjectOfType<AudioManager>().PlaySound("Click");

        initialUI.gameObject.SetActive(true);
        ResultUI.gameObject.SetActive(false);
        if (usingCustomMap) LoadExistingMap(DataManager.mapAddress[0, 0]);
        else LoadRandomMap();
        m_enabled = true;
    }
    public void LoadMainMap(int level)
    {
        GameManager.currentWorld = 0;
        GameManager.currentLevel = level;

        //StartCoroutine("ChangeToEditor", 2);

        PlayerData.Load();
        PlayerData.mapInfo.isTrainingLevelFinished = true;
        PlayerData.Save();

        FindObjectOfType<AudioManager>().StopMusic("BGM_game");
        FindObjectOfType<AudioManager>().PlayMusic("BGM");
        SceneManager.LoadScene(2);
    }
    public void BackToStartMenu()
    {
        FindObjectOfType<AudioManager>().PlaySound("Click");

        StartCoroutine("ChangeToEditor", 0);
        //SceneManager.LoadScene(0);
    }

    IEnumerator ChangeToEditor(int sceneId)
    {
        GameObject temp = Instantiate(closingPrefab);
        yield return new WaitForSeconds(temp.GetComponent<TransitionControl>().GetDuration());

        FindObjectOfType<AudioManager>().StopMusic("BGM_game");
        FindObjectOfType<AudioManager>().PlayMusic("BGM");
        SceneManager.LoadScene(sceneId);
    }

    public void OpenSettingMenu()
    {
        FindObjectOfType<AudioManager>().PlaySound("Click");

        initialUI.gameObject.SetActive(false);
        SettingUI.gameObject.SetActive(true);
        ResultUI.gameObject.SetActive(false);
    }

    public void CloseSettingMenu()
    {
        FindObjectOfType<AudioManager>().PlaySound("Click");

        initialUI.gameObject.SetActive(true);
        SettingUI.gameObject.SetActive(false);
        ResultUI.gameObject.SetActive(false);
    }

    private void SetStep(int val)
    {
        stepCount = val;
        //correction
        if (stepCount < 0) stepCount = 0;
        if (stepCount > stepMax) stepCount = stepMax;
        //update satiety bar
        SetSatiety(satietyMax * (float)(stepMax - stepCount) / (float)stepMax);
        //update emoji
        for (int i = 0; i < DataManager.levelGradingCount; ++i) if (stepCount <= DataManager.levelGradings[difficulty, i])
            {
                cowEmojiImage.GetComponent<Image>().sprite = cowEmojiPics[i];
                return;
            }
    }
    private void AddStep(int increment)
    {
        stepCount += increment;
        //correction
        if (stepCount < 0) stepCount = 0;
        if (stepCount > stepMax) stepCount = stepMax;
        //update satiety bar
        SetSatiety(satietyMax * (float)(stepMax - stepCount) / (float)stepMax);
        //update emoji
        for (int i = 0; i < DataManager.levelGradingCount; ++i) if (stepCount <= DataManager.levelGradings[difficulty, i])
            {
                cowEmojiImage.GetComponent<Image>().sprite = cowEmojiPics[i];
                return;
            }
    }
    
    private void SetSatiety(float val)
    {
        if (val < 0) val = 0;
        if (val > satietyMax) val = satietyMax;
        satiety = val;
        satietyTar = val;
        satietySlider.value = satiety;
    }
    private void AddSatiety(float increment)
    {
        satietyTar += increment;
        if (satietyTar < 0) satietyTar = 0;
        if (satietyTar > satietyMax) satietyTar = satietyMax;
    }
}
