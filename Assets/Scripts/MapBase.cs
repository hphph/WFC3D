using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Map Base", menuName = "WFC/MapBase", order = 1)]
[System.Serializable]
public class MapBase : ScriptableObject
{
    public List<Vector3Int> BaseModulesPositions;
    public String[] ModulesTags;
    public Vector3Int FrameSize;
    public Vector3Int Position;

    public Vector3Int TopRightBasePoint()
    {
        Vector3Int topRightPoint = Vector3Int.zero;
        foreach(Vector3Int v in BaseModulesPositions)
        {
            topRightPoint.x = v.x > topRightPoint.x ? v.x : topRightPoint.x;
            topRightPoint.y = v.y > topRightPoint.y ? v.y : topRightPoint.y;
            topRightPoint.z = v.z > topRightPoint.z ? v.z : topRightPoint.z;
        }
        return topRightPoint;
    }

    public void FillWithFrame(Vector3Int size)
    {
        if(size.x <= 0 || size.z <= 0) return;
        if(size.x == 1 || size.z == 1)
        {
            BaseModulesPositions = size.x == 1 ? new List<Vector3Int>(size.z) : new List<Vector3Int>(size.x);
        }
        else
        {
            BaseModulesPositions = new List<Vector3Int>(2*size.x + 2*size.z - 4);
        }
        for(int i = Position.z; i < size.z + Position.y; i++)
        {
            for(int j = Position.x; j < size.x + Position.x; j++)
            {
                if(i == Position.z || i == size.z + Position.z - 1)
                {
                    BaseModulesPositions.Add(new Vector3Int(j, size.y, i));
                }
                else if(j == Position.x || j == size.x + Position.x - 1)
                {
                    BaseModulesPositions.Add(new Vector3Int(j, size.y, i));
                }
            }
        }
    }
}
