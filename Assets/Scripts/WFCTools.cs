using System.Collections.Generic;
using UnityEngine;

public class WFCTools 
{
     public enum DirectionIndex
    {
        Left, Forward, Up, Right, Back, Down
    };

    /// <summary>
    /// </summary>
    /// <param name="slotPosition"></param>
    /// <returns>Dictionary of direction, vector pairs for every neighbour to a slot. Direction points at parent slot.</returns>
    public static Dictionary<DirectionIndex, Vector3Int> NeighboursToSlot(Vector3Int slotPosition)
    {
        Dictionary<DirectionIndex, Vector3Int> neighbours = new Dictionary<DirectionIndex, Vector3Int>
        {
            { DirectionIndex.Left, new Vector3Int(slotPosition.x + 1, slotPosition.y, slotPosition.z) },
            { DirectionIndex.Down, new Vector3Int(slotPosition.x, slotPosition.y + 1, slotPosition.z) },
            { DirectionIndex.Back, new Vector3Int(slotPosition.x, slotPosition.y, slotPosition.z + 1) },
            { DirectionIndex.Right, new Vector3Int(slotPosition.x - 1, slotPosition.y, slotPosition.z) },
            { DirectionIndex.Up, new Vector3Int(slotPosition.x, slotPosition.y - 1, slotPosition.z) },
            { DirectionIndex.Forward, new Vector3Int(slotPosition.x, slotPosition.y, slotPosition.z - 1) }
        };
        
        return neighbours;
    }

    public static bool IsConnectorHorizontal(int connectorIndex)
    {
        return !((connectorIndex + 1) % 3 == 0);
    }

    public static int OppositeConnectorIndex(int connectorIndex)
    {
        return (connectorIndex + 3) % 6;
    }
}
