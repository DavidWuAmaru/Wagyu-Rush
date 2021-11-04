using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;

public class GameManager : MonoBehaviour
{
    //information for movement
    public enum Direction { Up, Down, Left, Right }
    private Vector2Int[] movement = new Vector2Int[4] { new Vector2Int(0, 1), new Vector2Int(0, -1), new Vector2Int(-1, 0), new Vector2Int(1, 0) };
    private int[] movementX = new int[4] { 0, 0, -1, 1 };
    private int[] movementY = new int[4] { 1, -1, 0, 0 };
    private int moveCount = 0;

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
    [SerializeField] private TMP_Text scoreBoard;
    [SerializeField] private TMP_Text levelBoard;
    //test
    [SerializeField] private bool usingCustomMap = true;
    [SerializeField] private List<string> mapAddress;
    private bool[] itemReusability;
    private List<Sprite> cowSprites;
    private int currentLevel = 0;
    //satiety
    [SerializeField] private Slider satietySlider;
    [SerializeField] private int stepAllowed = 20;
    private float satietyMax = 1000.0f;
    private float satiety = 1000.0f, satietyTar = 1000.0f;

    #endregion

    private List<bool[]> oriBlockAccessible;
    private float mainMapLength = 48.0f;
    private float blockEdgeLength = 8.0f;
    private float mapOffsetX = 0.5f, mapOffsetY = 0.5f;
    private Vector3 mapOffset;
    private bool enable = true;
    private List<Block> blocks;
    private List<Character> characters;
    private List<Item> items;
    private List<Destination> destinations;
    private int[,] assistMap;

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
        if (usingCustomMap) LoadExistingMap(mapAddress[currentLevel]);
        else LoadRandomMap();

        scoreBoard.text = "Move : " + moveCount.ToString();
        levelBoard.text = "Level : " + (currentLevel + 1).ToString();

        //load cow sprites
        cowSprites = new List<Sprite>();
        cowSprites.Add(Resources.Load<Sprite>("cow_up"));
        cowSprites.Add(Resources.Load<Sprite>("cow_down"));
        cowSprites.Add(Resources.Load<Sprite>("cow_left"));
        cowSprites.Add(Resources.Load<Sprite>("cow_right"));
    }

    // Update is called once per frame
    void Update()
    {
        if (!enable) return;
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            StartCoroutine(IE_move(Direction.Up));
            if (validMove)
            {
                AddSatiety(-1 * satietyMax / stepAllowed);
                addMoveCount(1);
            }
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            StartCoroutine(IE_move(Direction.Down));
            if (validMove)
            {
                AddSatiety(-1 * satietyMax / stepAllowed);
                addMoveCount(1);
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            StartCoroutine(IE_move(Direction.Left));
            if (validMove)
            {
                AddSatiety(-1 * satietyMax / stepAllowed);
                addMoveCount(1);
            }
        }
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            StartCoroutine(IE_move(Direction.Right));
            if (validMove)
            {
                AddSatiety(-1 * satietyMax / stepAllowed);
                addMoveCount(1);
            }
        }
        //Satiety update
        AddSatiety(-0.005f);
        satiety = Mathf.Lerp(satiety, satietyTar, 0.05f);
        satietySlider.value = satiety;
    }

    private void addMoveCount(int increment)
    {
        moveCount += increment;
        scoreBoard.text = "Move : " + moveCount.ToString();
    }
    private void setMoveCount(int val)
    {
        moveCount = val;
        scoreBoard.text = "Move : " + moveCount.ToString();
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
        enable = true;
        setMoveCount(0);
        SetSatiety(satietyMax);
    }
    private void LoadExistingMap(string filename)
    {
        ClearAllObjs();
        MapData mapData = SaveSystem.LoadMap(filename);
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
        for(int y = 0;y < mapSize.y; ++y)
        {
            for(int  x= 0;x < mapSize.x; ++x)
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
        for(int i = 0;i < mapData.charNum; ++i)
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
        while (blocks[idxCow].type <Block.Type.Block_U || blocks[idxCow].type > Block.Type.Block_UDRL) idxCow = Random.Range(0, blocks.Count);
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

    private void onCollectingItem(GameObject srcGameObject, GameObject tarGameObject , bool isItem)
    {
        if (isItem)  //deal with item
        {
            for(int i = 0;i < items.Count;++i) if(items[i].entity == tarGameObject)
                {
                    if(items[i].type == Item.Type.Key)
                    {
                        Debug.Log("Collect key");
                    }
                    else if (items[i].type == Item.Type.HayStack)
                    {
                        AddSatiety(5 * (satietyMax / stepAllowed));
                    }
                    else if (items[i].type == Item.Type.Trap)
                    {
                        AddSatiety(-5 * (satietyMax / stepAllowed));
                    }
                    else if (items[i].type == Item.Type.HeadPhone)
                    {
                        for (int j = 0; j < characters.Count; ++j) if (characters[j].entity == srcGameObject)
                            {
                                srcGameObject.transform.GetChild(0).GetComponent<Animator>().enabled = true;
                                characters[j].destination = items[i].entity.transform.position;
                                characters[j].freeze += 5;
                            }
                    }
                    else if (items[i].type == Item.Type.Portal)
                    {
                        //for (int j = 0; j < characters.Count; ++j) if (characters[j].entity == srcGameObject)
                        //    {
                        //        if (!characters[j].transferred)
                        //        {
                        //            characters[j].destination = items[i].entity.transform.position;
                        //            StartCoroutine(IE_swap(characters[j], items[i].position));
                        //            characters[j].transferred = true;
                        //        }
                        //    }
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
                    if(destinations[i].type == Destination.Type.Van)
                    {
                        bool existKey = false;
                        for(int j = 0;j < items.Count; ++j)
                        {
                            if(items[j].type == Item.Type.Key)
                            {
                                existKey = true;
                                break;
                            }
                        }
                        if (!existKey)  //pass the map
                        {
                            Debug.Log("Win");
                            LevelUp();
                        }
                    }
                    break;
                }
        }
    }
    IEnumerator IE_swap(Character ch, Vector2Int destPos)
    {
        float scalar = 1.0f, step = 0.001f;
        float rotator = 0.0f, rstep = 360.0f / (scalar / step);
        while(scalar > 0)
        {
            enable = false;
            scalar -= step;
            rotator += rstep;
            ch.entity.transform.localScale = new Vector3(scalar, scalar, 1);
            ch.entity.transform.eulerAngles = new Vector3(0, 0, rotator);
            yield return null;
        }
        ch.position = destPos;
        ch.entity.transform.position = new Vector3(ch.position.x, ch.position.y, 0) * blockEdgeLength + mapOffset;
        while(scalar < 1.0f)
        {
            enable = false;
            scalar += step;
            rotator -= rstep;
            ch.entity.transform.localScale = new Vector3(scalar, scalar, 1);
            ch.entity.transform.eulerAngles = new Vector3(0, 0, rotator);
            yield return null;
        }
        ch.entity.transform.localScale = new Vector3(1, 1, 1);
        ch.entity.transform.eulerAngles = new Vector3(0, 0, 0);
        enable = true;
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
    IEnumerator IE_move(Direction dir)
    {
        enable = false;
        validMove = false;
        bool finished = false;
        //move blocks
        moveMap(dir);
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

        yield return new WaitForSeconds(0.05f);
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

        enable = true;
    }

    private void setUpAssistMap()
    {
        //setup the map
        for (int x = 0; x < mapSize.x; ++x) for (int y = 0; y < mapSize.y; ++y) assistMap[x, y] = -1;
        for (int i = 0; i < blocks.Count; ++i)
        {
            int val = -1;
            if (blocks[i].type>=Block.Type.Block_U && blocks[i].type <= Block.Type.Block_UDRL) val = i;
            else if (blocks[i].type == Block.Type.Obstacle) val = -2;
            else if (blocks[i].type == Block.Type.RotateBlock) val = -3;
            assistMap[blocks[i].position.x, blocks[i].position.y] = val;
        }
    }
    
    private void moveMap(Direction dir)
    {
        int mx = movementX[(int)dir], my = movementY[(int)dir];
        setUpAssistMap();
        if(dir == Direction.Down || dir == Direction.Left)
        {
            for (int y = 0; y < mapSize.y; ++y)
            {
                for(int x = 0; x < mapSize.x; ++x)
                {
                    if (assistMap[x, y] < 0 || outOfRange(x + mx, y + my) || assistMap[x + mx, y + my]  != -1) continue;
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
                    for(int i = 0;i < destinations.Count;++i) if(destinations[i].position.x == x && destinations[i].position.y == y)
                        {
                            destinations[i].position += movement[(int)dir];
                            destinations[i].destination += new Vector3(mx, my, 0) * blockEdgeLength;
                        }
                }
            }
        }
        if(dir == Direction.Up || dir == Direction.Right)
        {
            for(int y = mapSize.y - 1; y >= 0; --y)
            {
                for(int x = mapSize.x - 1; x >= 0; --x)
                {
                    if (assistMap[x, y] < 0 || outOfRange(x + mx, y + my) || assistMap[x + mx, y + my] != -1) continue;
                    blocks[assistMap[x, y]].position += movement[(int)dir];
                    blocks[assistMap[x, y]].destination = new Vector3(blocks[assistMap[x, y]].position.x, blocks[assistMap[x, y]].position.y, 0) * blockEdgeLength + mapOffset;
                    assistMap[x, y] = -1;
                    validMove = true;
                    //move characters that's on the block
                    for(int i = 0;i < characters.Count;++i) if(characters[i].position.x == x && characters[i].position.y == y)
                        {
                            characters[i].position += movement[(int)dir];
                            characters[i].destination += new Vector3(mx, my, 0) * blockEdgeLength ;
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
                    if (checkOnItem(x, y)) break;
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
                    if (checkOnItem(x, y)) break;
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
                    if (checkOnItem(x, y)) break;
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
                    if (checkOnItem(x, y)) break;
                }
                characters[i].position = new Vector2Int(x, y);
                characters[i].destination = new Vector3(x, y, 0) * blockEdgeLength + new Vector3(mapOffsetX, mapOffsetY, 0);
            }
        }
    }

    private bool checkOnItem(int x, int y)
    {
        for(int i = 0;i < items.Count; ++i)
        {
            if(items[i].position == new Vector2Int(x, y))
            {
               
            }
        }
        return false;
    }

    private void rotateBlockFunc()
    {
        setUpAssistMap();
        for(int i = 0;i < blocks.Count; ++i)
        {
            if(blocks[i].type == Block.Type.RotateBlock)
            {
                int x = blocks[i].position.x, y = blocks[i].position.y;
                for(int t = 0;t < 4; ++t)
                {
                    if(!outOfRange(blocks[i].position + movement[t]) && assistMap[x + movementX[t], y+movementY[t]] >= 0)
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
        if (usingCustomMap) LoadExistingMap(mapAddress[currentLevel]);
        else LoadRandomMap();
    }
    public void LevelUp()
    {
        if (currentLevel >= mapAddress.Count - 1) return;
        currentLevel++;
        levelBoard.text = "Level : " + (currentLevel + 1).ToString();
        ReloadMap();
    }
    public void LevelDown()
    {
        if (currentLevel <= 0) return;
        currentLevel--;
        levelBoard.text = "Level : " + (currentLevel + 1).ToString();
        ReloadMap();
    }

    private void SetSatiety(float val)
    {
        satiety = val;
        satietyTar = val;
        satietySlider.value = satiety;
    }
    private void AddSatiety(float increment)
    {
        satietyTar += increment;
    }
}
