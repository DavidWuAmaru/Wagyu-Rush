using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class GameManager : MonoBehaviour
{
    //information for movement
    public enum Direction { Up, Down, Left, Right }
    private Vector2Int[] movement = new Vector2Int[4] { new Vector2Int(0, 1), new Vector2Int(0, -1), new Vector2Int(-1, 0), new Vector2Int(1, 0) };
    private int[] movementX = new int[4] { 0, 0, -1, 1 };
    private int[] movementY = new int[4] { 1, -1, 0, 0 };
    private int moveCount = 0;

    //class definition
    public enum BlockType { Block_U, Block_UD, Block_UR, Block_UDL, Block_UDRL, Obstacle, RotateBlock };
    private class Block
    {
        public enum Type { NormalBlock, Obstacle, RotateBlock };
        public Type type;
        public Vector2Int position;
        public GameObject entity;
        public Vector3 destination;
        public float RotateAngle = 0; // left turn
        public bool[] accessible = new bool[4] { false, false, false, false }; //up down left right
    }
    private class Character
    {
        public enum Type { Wagyu };
        public Type type;
        public Vector2Int position;
        public Vector3 destination;
        public GameObject entity;
    }
    private class Item
    {
        public enum Type { Van, Key, HayStack};
        public Type type;
        public Vector2Int position;
        public Vector3 destination;
        public GameObject entity;
    }

    #region SerializeField
    [SerializeField] private Camera mainCamera;
    [SerializeField] private List<GameObject> blockTypes;
    [SerializeField] private List<GameObject> characterTypes;
    [SerializeField] private List<GameObject> itemTypes;
    [SerializeField] private Vector2Int mapSize;
    [SerializeField] private float blockMoveSpeed = 30.0f;   //block moving speed (exp)
    [SerializeField] private float characterMoveSpeed = 100.0f; //cow moving speed (linear)
    [SerializeField] private float blockRotateSpeed = 15.0f;  //block rotating speed (exp)
    [SerializeField] private Text scoreBoard;
    [SerializeField] private Text levelBoard;
    //test
    [SerializeField] private bool usingCustomMap = true;
    [SerializeField] private List<string> mapAddress;
    private int currentLevel = 0;
    #endregion

    private List<bool[]> oriBlockAccessible;
    private float mapEdgeLength = 8.0f;
    private bool enable = true;
    private List<Block> blocks;
    private List<Character> characters;
    private List<Item> items;
    private int[,] assistMap;

    // Start is called before the first frame update
    void Start()
    {
        //initialization
        blocks = new List<Block>();
        characters = new List<Character>();
        items = new List<Item>();
        oriBlockAccessible = new List<bool[]>();
        oriBlockAccessible.Add(new bool[4] { true, false, false, false });
        oriBlockAccessible.Add(new bool[4] { true, true, false, false });
        oriBlockAccessible.Add(new bool[4] { true, false, false, true });
        oriBlockAccessible.Add(new bool[4] { true, true, true, false });
        oriBlockAccessible.Add(new bool[4] { true, true, true, true });
        oriBlockAccessible.Add(new bool[4] { false, false, false, false });
        oriBlockAccessible.Add(new bool[4] { false, false, false, false });

        if (usingCustomMap) LoadExistingMap(mapAddress[currentLevel]);
        else LoadRandomMap();

        scoreBoard.text = "Move : " + moveCount.ToString();
        levelBoard.text = "Level : " + (currentLevel + 1).ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (!enable) return;
        //temp
        if (win)
        {
            LevelUp();
        }
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            StartCoroutine(IE_move(Direction.Up));
            if (validMove) addMoveCount(1);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            StartCoroutine(IE_move(Direction.Down));
            if (validMove) addMoveCount(1);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            StartCoroutine(IE_move(Direction.Left));
            if (validMove) addMoveCount(1);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            StartCoroutine(IE_move(Direction.Right));
            if (validMove) addMoveCount(1);
        }
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
        blocks.Clear();
        characters.Clear();
        items.Clear();
        enable = true;
        setMoveCount(0);
    }
    private void normalizeString(ref string input)
    {
        input.Trim();
        while (input.IndexOf("  ") != -1) input = input.Replace("  ", " ");
    }
    private void LoadExistingMap(string filename)
    {
        ClearAllObjs();

        //load in
        if (!File.Exists(filename))
        {
            Debug.LogError("Can't find the map file with the filename : " + filename);
            return;
        }
        StreamReader reader = new StreamReader(filename);
        //read in the map
        string input = reader.ReadLine();
        normalizeString(ref input);
        string[] strs = input.Split(' ');
        mapSize = new Vector2Int(int.Parse(strs[0]), int.Parse(strs[1]));
        assistMap = new int[mapSize.x, mapSize.y];
        mainCamera.transform.position = new Vector3(mapSize.x * mapEdgeLength / 2, mapSize.y * mapEdgeLength / 2 - 5, -10);
        Camera.main.orthographicSize = mapSize.y * 5.0f;

        //loading blocks info
        for (int y = 0; y < mapSize.y; ++y)
        {
            input = reader.ReadLine();
            normalizeString(ref input);
            strs = input.Split(' ');
            for (int x = 0; x < mapSize.x; ++x)
            {
                if (int.Parse(strs[x]) == -1) continue;
                int blockIdx = int.Parse(strs[x]) / 4, blockRot = int.Parse(strs[x]) % 4;
                assistMap[x, y] = blockIdx;

                Block newBlock = new Block();
                newBlock.entity = Instantiate(blockTypes[blockIdx], new Vector3(x * mapEdgeLength, y * mapEdgeLength, 0), Quaternion.identity);
                if (blockIdx <= (int)BlockType.Block_UDRL) newBlock.type = Block.Type.NormalBlock;
                else if (blockIdx <= (int)BlockType.Obstacle) newBlock.type = Block.Type.Obstacle;
                else if (blockIdx <= (int)BlockType.RotateBlock) newBlock.type = Block.Type.RotateBlock;
                newBlock.position = new Vector2Int(x, y);
                newBlock.destination = newBlock.entity.transform.position;
                for (int i = 0; i < 4; ++i) newBlock.accessible[i] = oriBlockAccessible[blockIdx][i];
                for (int i = 0; i < blockRot; ++i) blockRotateLeft(ref newBlock);
                blocks.Add(newBlock);
            }
        }

        //loading character info
        input = reader.ReadLine();
        normalizeString(ref input);
        strs = input.Split(' ');
        int num = int.Parse(strs[0]);
        for(int t = 0;t < num; ++t)
        {
            input = reader.ReadLine();
            normalizeString(ref input);
            strs = input.Split(' ');

            int charType = int.Parse(strs[0]), px = int.Parse(strs[1]), py = int.Parse(strs[2]);
            Character newC = new Character();
            newC.type = (Character.Type)charType;
            newC.position = new Vector2Int(px, py);
            newC.entity = Instantiate(characterTypes[(int)newC.type], new Vector3(newC.position.x * mapEdgeLength, newC.position.y * mapEdgeLength, 0), Quaternion.identity);
            newC.destination = newC.entity.transform.position;
            characters.Add(newC);
        }


        //loading item info
        input = reader.ReadLine();
        normalizeString(ref input);
        strs = input.Split(' ');
        num = int.Parse(strs[0]);
        for (int t = 0; t < num; ++t)
        {
            input = reader.ReadLine();
            strs = input.Split(' ');
            int itType = int.Parse(strs[0]), px = int.Parse(strs[1]), py = int.Parse(strs[2]);
            Item newItem = new Item();
            newItem.type = (Item.Type)itType;
            newItem.position = new Vector2Int(px, py);
            newItem.entity = Instantiate(itemTypes[(int)newItem.type], new Vector3(newItem.position.x * mapEdgeLength, newItem.position.y * mapEdgeLength, 0), Quaternion.identity);
            newItem.destination = newItem.entity.transform.position;
            items.Add(newItem);
        }
        reader.Close();
    }
    private void LoadRandomMap()
    {
        ClearAllObjs();

        assistMap = new int[mapSize.x, mapSize.y];
        //adjusting camera position
        mainCamera.transform.position = new Vector3(mapSize.x * mapEdgeLength / 2, mapSize.y * mapEdgeLength / 2 - 5, -10);
        Camera.main.orthographicSize = mapSize.y * 5.0f;

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
                        newBlock.entity = Instantiate(blockTypes[(int)(sel / 6)], new Vector3(x * mapEdgeLength, y * mapEdgeLength, 0), Quaternion.identity);
                        newBlock.type = Block.Type.NormalBlock;
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
                        newBlock.entity = Instantiate(blockTypes[(int)BlockType.Obstacle], new Vector3(x * mapEdgeLength, y * mapEdgeLength, 0), Quaternion.identity);
                        newBlock.type = Block.Type.Obstacle;
                        newBlock.position = new Vector2Int(x, y);
                        newBlock.destination = newBlock.entity.transform.position;
                        blocks.Add(newBlock);
                    }
                    else if (sel < 45)  //rotate
                    {
                        Block newBlock = new Block();
                        newBlock.entity = Instantiate(blockTypes[(int)BlockType.RotateBlock], new Vector3(x * mapEdgeLength, y * mapEdgeLength, 0), Quaternion.identity);
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
        while (blocks[idxCow].type != Block.Type.NormalBlock) idxCow = Random.Range(0, blocks.Count);
        Character cow = new Character();
        cow.type = Character.Type.Wagyu;
        cow.position = blocks[idxCow].position;
        cow.entity = Instantiate(characterTypes[(int)Character.Type.Wagyu], new Vector3(cow.position.x * mapEdgeLength, cow.position.y * mapEdgeLength, 0), Quaternion.identity);
        cow.destination = cow.entity.transform.position;
        characters.Add(cow);
        //Spawn van
        int idxVan = Random.Range(0, blocks.Count);
        while (blocks[idxVan].type != Block.Type.NormalBlock || idxVan == idxCow) idxVan = Random.Range(0, blocks.Count);
        Item van = new Item();
        van.type = Item.Type.Van;
        van.position = blocks[idxVan].position;
        van.entity = Instantiate(itemTypes[(int)Item.Type.Van], new Vector3(van.position.x * mapEdgeLength, van.position.y * mapEdgeLength, 0), Quaternion.identity);
        van.destination = van.entity.transform.position;
        items.Add(van);
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
    bool win = false;
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
            if (blocks[i].type == Block.Type.NormalBlock) val = i;
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
                    blocks[assistMap[x, y]].destination = new Vector3(blocks[assistMap[x, y]].position.x * 8, blocks[assistMap[x, y]].position.y * 8, 0);
                    assistMap[x, y] = -1;
                    validMove = true;
                    //move characters that's on the block
                    for (int i = 0; i < characters.Count; ++i) if (characters[i].position.x == x && characters[i].position.y == y)
                        {
                            characters[i].position += movement[(int)dir];
                            characters[i].destination += new Vector3(mx, my, 0) * mapEdgeLength;
                        }
                    //move items that's on the block
                    for (int i = 0; i < items.Count; ++i) if (items[i].position.x == x && items[i].position.y == y)
                        {
                            items[i].position += movement[(int)dir];
                            items[i].destination += new Vector3(mx, my, 0) * mapEdgeLength;
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
                    blocks[assistMap[x, y]].destination = new Vector3(blocks[assistMap[x, y]].position.x * mapEdgeLength, blocks[assistMap[x, y]].position.y * mapEdgeLength, 0);
                    assistMap[x, y] = -1;
                    validMove = true;
                    //move characters that's on the block
                    for(int i = 0;i < characters.Count;++i) if(characters[i].position.x == x && characters[i].position.y == y)
                        {
                            characters[i].position += movement[(int)dir];
                            characters[i].destination += new Vector3(mx, my, 0) * mapEdgeLength;
                        }
                    //move items that's on the block
                    for (int i = 0; i < items.Count; ++i) if (items[i].position.x == x && items[i].position.y == y)
                        {
                            items[i].position += movement[(int)dir];
                            items[i].destination += new Vector3(mx, my, 0) * mapEdgeLength;
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
                characters[i].entity.transform.eulerAngles = new Vector3(0, 0, 180);
                int x = characters[i].position.x, y = characters[i].position.y;
                while (y < mapSize.y - 1 && blocks[assistMap[x, y]].accessible[0] && assistMap[x, y + 1] >= 0 && blocks[assistMap[x, y + 1]].accessible[1]) 
                {
                    y++;
                    validMove = true;
                    if (checkOnItem(x, y)) break;
                }
                characters[i].position = new Vector2Int(x, y);
                characters[i].destination = new Vector3(x, y, 0) * mapEdgeLength;
            }
        }
        if (dir == Direction.Down)
        {
            for (int i = 0; i < characters.Count; ++i)
            {
                characters[i].entity.transform.eulerAngles = new Vector3(0, 0, 0);
                int x = characters[i].position.x, y = characters[i].position.y;
                while (y > 0 && blocks[assistMap[x, y]].accessible[1] && assistMap[x, y - 1] >= 0 && blocks[assistMap[x, y - 1]].accessible[0])
                {
                    y--;
                    validMove = true;
                    if (checkOnItem(x, y)) break;
                }
                characters[i].position = new Vector2Int(x, y);
                characters[i].destination = new Vector3(x, y, 0) * mapEdgeLength;
            }
        }
        if (dir == Direction.Left)
        {
            for (int i = 0; i < characters.Count; ++i)
            {
                characters[i].entity.transform.eulerAngles = new Vector3(0, 0, 270);
                int x = characters[i].position.x, y = characters[i].position.y;
                while (x > 0 && blocks[assistMap[x, y]].accessible[2] && assistMap[x - 1, y] >= 0 && blocks[assistMap[x - 1, y]].accessible[3])
                {
                    x--;
                    validMove = true;
                    if (checkOnItem(x, y)) break;
                }
                characters[i].position = new Vector2Int(x, y);
                characters[i].destination = new Vector3(x, y, 0) * mapEdgeLength;
            }
        }
        if (dir == Direction.Right)
        {
            for (int i = 0; i < characters.Count; ++i)
            {
                characters[i].entity.transform.eulerAngles = new Vector3(0, 0, 90);
                int x = characters[i].position.x, y = characters[i].position.y;
                while (x < mapSize.x - 1 && blocks[assistMap[x, y]].accessible[3] && assistMap[x + 1, y] >= 0 && blocks[assistMap[x + 1, y]].accessible[2])
                {
                    x++;
                    validMove = true;
                    if (checkOnItem(x, y)) break;
                }
                characters[i].position = new Vector2Int(x, y);
                characters[i].destination = new Vector3(x, y, 0) * mapEdgeLength;
            }
        }
    }

    private bool checkOnItem(int x, int y)
    {
        for(int i = 0;i < items.Count; ++i)
        {
            if(items[i].position == new Vector2Int(x, y))
            {
                if(items[i].type == Item.Type.Van)
                {
                    win = true;
                    return true;
                }
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
        win = false;
        ReloadMap();
    }
    public void LevelDown()
    {
        if (currentLevel <= 0) return;
        currentLevel--;
        levelBoard.text = "Level : " + (currentLevel + 1).ToString();
        win = false;
        ReloadMap();
    }
}