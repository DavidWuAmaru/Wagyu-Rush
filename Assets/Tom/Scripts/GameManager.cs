using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //information for movement
    public enum Direction { Up, Down, Left, Right }
    private Vector2Int[] movement = new Vector2Int[4] { new Vector2Int(0, 1), new Vector2Int(0, -1), new Vector2Int(-1, 0), new Vector2Int(1, 0) };
    private int[] movementX = new int[4] { 0, 0, -1, 1 };
    private int[] movementY = new int[4] { 1, -1, 0, 0 };

    //class definition
    public enum BlockType { Block1, Block2, Block3, Block4, Obstacle };
    private class Block
    {
        public enum Type { NormalBlock, Obstacle };
        public Type type;
        public Vector2Int position;
        public GameObject entity;
        public Vector3 destination;
        public bool[] accessible = new bool[4] { false, false, false, false }; //up down left right
    }
    private class Character
    {
        public enum Type {Wagyu, Van };
        public Type type;
        public Vector2Int position;
        public Vector3 destination;
        public GameObject entity;
    }

    [SerializeField] private Camera mainCamera;
    [SerializeField] private List<GameObject> blockTypes;
    [SerializeField] private Vector2Int mapSize;
    private List<bool[]> oriBlockAccessible;
    private float mapEdgeLength = 8.0f;
    private bool enable = true;
    private List<Block> blocks;
    private int[,] assistMap;

    // Start is called before the first frame update
    void Start()
    {
        //initialization
        blocks = new List<Block>();
        oriBlockAccessible = new List<bool[]>();
        oriBlockAccessible.Add(new bool[4] { true, true, true, true });
        oriBlockAccessible.Add(new bool[4] { true, false, false, true });
        oriBlockAccessible.Add(new bool[4] { false, false, true, true });
        oriBlockAccessible.Add(new bool[4] { true, false, true, true });
        oriBlockAccessible.Add(new bool[4] { false, false, false, false });
        assistMap = new int[mapSize.x, mapSize.y];

        //adjusting camera position
        mainCamera.transform.position = new Vector3(mapSize.x * mapEdgeLength / 2, mapSize.y * mapEdgeLength / 2 - 5, -10);

        //randomly create the map
        for(int y = 0;y < mapSize.y; ++y)
        {
            for(int x = 0; x < mapSize.x; ++x)
            {
                int sel = Random.Range(0, 8);
                if(sel < 4)  //normal block
                {
                    Block newBlock = new Block();
                    newBlock.entity = Instantiate(blockTypes[sel], new Vector3(x * mapEdgeLength, y * mapEdgeLength, 0), Quaternion.identity);
                    newBlock.type = Block.Type.NormalBlock;
                    newBlock.position = new Vector2Int(x, y);
                    newBlock.destination = newBlock.entity.transform.position;
                    for (int i = 0; i < 4; ++i) newBlock.accessible[i] = oriBlockAccessible[sel][i];
                    for (int i = Random.Range(0, 4); i > 0; --i) blockRotateRight(newBlock);
                    blocks.Add(newBlock);
                }
                else if(sel < 5)  //obstacle
                {
                    Block newBlock = new Block();
                    newBlock.entity = Instantiate(blockTypes[(int)BlockType.Obstacle], new Vector3(x * mapEdgeLength, y * mapEdgeLength, 0), Quaternion.identity);
                    newBlock.type = Block.Type.Obstacle;
                    newBlock.position = new Vector2Int(x, y);
                    newBlock.destination = newBlock.entity.transform.position;
                    blocks.Add(newBlock);
                }
            }
        }
        


    }

    // Update is called once per frame
    void Update()
    {
        if (!enable) return;
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            moveMap(Direction.Up);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            moveMap(Direction.Down);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            moveMap(Direction.Left);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            moveMap(Direction.Right);
        }
    }

    void blockRotateRight(Block target)
    {
        target.entity.transform.eulerAngles += new Vector3(0, 0, 90);
        bool tmp = target.accessible[0];
        target.accessible[0] = target.accessible[2];
        target.accessible[1] = target.accessible[3];
        target.accessible[2] = target.accessible[1];
        target.accessible[3] = tmp;
    }
    void blockRotateLeft(Block target)
    {
        target.entity.transform.eulerAngles += new Vector3(0, 0, -90);
        bool tmp = target.accessible[0];
        target.accessible[0] = target.accessible[3];
        target.accessible[1] = target.accessible[2];
        target.accessible[2] = tmp;
        target.accessible[3] = target.accessible[1];
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
            assistMap[blocks[i].position.x, blocks[i].position.y] = val;
        }
    }
    private bool outOfRange(Vector2Int pos)
    {
        return pos.x < 0 || pos.y < 0 || pos.x >= mapSize.x || pos.y >= mapSize.y;
    }
    private bool outOfRange(int x, int y)
    {
        return x < 0 || y < 0 || x >= mapSize.x || y >= mapSize.y;
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
                    blocks[assistMap[x, y]].destination = new Vector3(blocks[assistMap[x, y]].position.x * 8, blocks[assistMap[x, y]].position.y * 8, 0);
                    assistMap[x, y] = -1;
                }
            }
        }

        enable = false;
        StartCoroutine(moveToDest());
    }

    IEnumerator moveToDest()
    {
        while (!enable)
        {
            bool arrived = true;
            for(int i = 0;i < blocks.Count; ++i)
            {
                if (Vector3.Distance(blocks[i].entity.transform.position, blocks[i].destination) > 0.1f)
                {
                    blocks[i].entity.transform.position = Vector3.Lerp(blocks[i].entity.transform.position, blocks[i].destination, 0.1f);
                    arrived = false;
                }
                else blocks[i].entity.transform.position = blocks[i].destination;
            }

            if (arrived) enable = true;
            else yield return null;
        }
    }
}
