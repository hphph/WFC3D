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
    public static (DirectionIndex, Vector3Int, ModuleSocket)[] NeighboursToSocket(ModuleSocket socket)
    {
        (DirectionIndex, Vector3Int, ModuleSocket)[] neighbours = new (DirectionIndex, Vector3Int, ModuleSocket)[]
        {
            ( DirectionIndex.Left, new Vector3Int(socket.Position.x + 1, socket.Position.y, socket.Position.z), socket ),
            ( DirectionIndex.Forward, new Vector3Int(socket.Position.x, socket.Position.y, socket.Position.z - 1), socket ),
            ( DirectionIndex.Up, new Vector3Int(socket.Position.x, socket.Position.y - 1, socket.Position.z), socket ),
            ( DirectionIndex.Right, new Vector3Int(socket.Position.x - 1, socket.Position.y, socket.Position.z), socket ),
            ( DirectionIndex.Back, new Vector3Int(socket.Position.x, socket.Position.y, socket.Position.z + 1), socket ),
            ( DirectionIndex.Down, new Vector3Int(socket.Position.x, socket.Position.y + 1, socket.Position.z), socket ),
        };
        
        return neighbours;
    }

    public static int Mod(int x, int m) 
    {
        return (x%m + m)%m;
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
