using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//implementing
[System.Serializable]
public class MapData
{
    public int difficulty = 2;
    public int width, height;
    public int[] blocks, rotations;
    public int charNum, itemNum, destNum;
    public int[] characters, items, destinations;
    public int[] charPosition, itemPosition, destPosition;

    public MapData()
    {
        difficulty = 2;
        width = 0;
        height = 0;
        charNum = 0;
        itemNum = 0;
        destNum = 0;
    }
    public MapData(int _difficulty, int _width, int _height, int[] _blocks, int[] _rotations, int _charNum, int[] _characters, int[] _charPosition, int _itemNum, int[] _items, int[] _itemPosition, int _destNum, int[] _dests, int[] _destPosition)
    {
        difficulty = _difficulty;
        width = _width;
        height = _height;
        charNum = _charNum;
        itemNum = _itemNum;
        destNum = _destNum;
        blocks = new int[width * height];
        rotations = new int[width * height];
        characters = new int[charNum];
        charPosition = new int[charNum * 2];
        items = new int[itemNum];
        itemPosition = new int[itemNum * 2];
        destinations = new int[destNum];
        destPosition = new int[destNum * 2];

        for (int i = 0;i < width * height; ++i)
        {
            blocks[i] = _blocks[i];
            rotations[i] = _rotations[i];
        }
        for(int i = 0;i < charNum; ++i)
        {
            characters[i] = _characters[i];
            charPosition[i * 2] = _charPosition[i * 2];
            charPosition[i * 2 + 1] = _charPosition[i * 2 + 1];
        }
        for(int i = 0;i < itemNum; ++i)
        {
            items[i] = _items[i];
            itemPosition[i * 2] = _itemPosition[i * 2];
            itemPosition[i * 2 + 1] = _itemPosition[i * 2 + 1];
        }
        for(int i = 0;i < destNum; ++i)
        {
            destinations[i] = _dests[i];
            destPosition[i * 2] = _destPosition[i * 2];
            destPosition[i * 2 + 1] = _destPosition[i * 2 + 1];
        }
    }
}
