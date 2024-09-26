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
    /// <returns>Dictionary of direction, vector pairs for every neighbour to a slot. Direction points at parent slot.</returns>
    public static (DirectionIndex, Vector3Int, Slot)[] NeighboursToSlot(Slot slot)
    {
        (DirectionIndex, Vector3Int, Slot)[] neighbours = new (DirectionIndex, Vector3Int, Slot)[]
        {
            ( DirectionIndex.Left, new Vector3Int(slot.Position.x + 1, slot.Position.y, slot.Position.z), slot ),
            ( DirectionIndex.Forward, new Vector3Int(slot.Position.x, slot.Position.y, slot.Position.z - 1), slot ),
            ( DirectionIndex.Up, new Vector3Int(slot.Position.x, slot.Position.y - 1, slot.Position.z), slot ),
            ( DirectionIndex.Right, new Vector3Int(slot.Position.x - 1, slot.Position.y, slot.Position.z), slot ),
            ( DirectionIndex.Back, new Vector3Int(slot.Position.x, slot.Position.y, slot.Position.z + 1), slot ),
            ( DirectionIndex.Down, new Vector3Int(slot.Position.x, slot.Position.y + 1, slot.Position.z), slot ),
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
