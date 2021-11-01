using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block
{
    public enum Type { NormalBlock, Obstacle, RotateBlock };
    public Type type;
    public Vector2Int position;
    public GameObject entity;
    public Vector3 destination;
    public float RotateAngle = 0; // left turn
    public bool[] accessible = new bool[4] { false, false, false, false }; //up down left right
}
public class Character
{
    public enum Type { Wagyu };
    public Type type;
    public Vector2Int position;
    public Vector3 destination;
    public GameObject entity;
}
public class Item
{
    public enum Type { Van, Key, HayStack };
    public Type type;
    public Vector2Int position;
    public Vector3 destination;
    public GameObject entity;
}

