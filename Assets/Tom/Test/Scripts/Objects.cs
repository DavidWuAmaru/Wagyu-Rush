using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block
{ 
    public enum Type { Block_U, Block_UD, Block_UR, Block_UDL, Block_UDRL, Obstacle, RotateBlock };
    public Type type;
    public Vector2Int position;
    public GameObject entity;
    public Vector3 destination;
    public float RotateAngle = 0; // left turn
    public bool[] accessible = new bool[4] { false, false, false, false }; //up down left right
}
public class Character
{
    public enum Type { Cow };
    public Type type;
    public Vector2Int position;
    public Vector3 destination;
    public GameObject entity;
}
public class Item
{
    public enum Type { Key, HayStack, Trap, HeadPhone, Portal };
    public Type type;
    public Vector2Int position;
    public Vector3 destination;
    public GameObject entity;
}

public class Destination
{
    public enum Type { Van };
    public Type type;
    public Vector2Int position;
    public Vector3 destination;
    public GameObject entity;
}

