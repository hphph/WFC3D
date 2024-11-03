using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Map Base", menuName = "WFC/MapBase", order = 1)]
public class MapBase : ScriptableObject
{
    public List<Vector3Int> baseModulePosition;
    public String[] modulesTags;
    public Vector2Int frameSize;
    public Vector3Int position;

    public void FillWithFrame(Vector2Int size, Vector3Int topLeftPos)
    {
        if(size.x <= 0 || size.y <= 0) return;
        if(size.x == 1 || size.y == 1)
        {
            baseModulePosition = size.x == 1 ? new List<Vector3Int>(size.y) : new List<Vector3Int>(size.x);
        }
        else
        {
            baseModulePosition = new List<Vector3Int>(2*size.x + 2*size.y - 4);
        }
        for(int i = position.y; i < size.y + position.y; i++)
        {
            for(int j = position.x; j < size.x + position.x; j++)
            {
                if(i == position.y || i == size.y + position.y - 1)
                {
                    baseModulePosition.Add(new Vector3Int(j, i, 0));
                }
                else if(j == position.x || j == size.x + position.x - 1)
                {
                    baseModulePosition.Add(new Vector3Int(j, i, 0));
                }
            }
        }
    }
}
