using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Identity
{
    public Vector2Int position;
    public GameObject entity;
    public Vector3 destination;
}
public class Block : Identity
{ 
    public enum Type { Block_U, Block_UD, Block_UR, Block_UDL, Block_UDRL, Obstacle, RotateBlock };
    public Type type;
    public float RotateAngle = 0; // left turn
    public bool[] accessible = new bool[4] { false, false, false, false }; //up down left right
}
public class Character : Identity
{
    public enum Type { Cow };
    public Type type;
    //extra function
    public int freeze = 0;
}
public class Item : Identity
{
    public enum Type { Key, HayStack, Trap, HeadPhone, Portal };
    public Type type;
    //extra function

}

public class Destination : Identity
{
    public enum Type { Van };
    public Type type;
}

