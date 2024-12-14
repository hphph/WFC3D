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
    /// <returns>Dictionary of direction, vector pairs for every neighbour to a socket. Direction points at parent socket.</returns>
    public static (DirectionIndex, Vector3Int, ModuleSocket)[] NeighboursToSocket(ModuleSocket parentSocket)
    {
        (DirectionIndex, Vector3Int, ModuleSocket)[] neighbours = new (DirectionIndex, Vector3Int, ModuleSocket)[]
        {
            ( DirectionIndex.Left, new Vector3Int(parentSocket.Position.x + 1, parentSocket.Position.y, parentSocket.Position.z), parentSocket ),
            ( DirectionIndex.Forward, new Vector3Int(parentSocket.Position.x, parentSocket.Position.y, parentSocket.Position.z - 1), parentSocket ),
            ( DirectionIndex.Up, new Vector3Int(parentSocket.Position.x, parentSocket.Position.y - 1, parentSocket.Position.z), parentSocket ),
            ( DirectionIndex.Right, new Vector3Int(parentSocket.Position.x - 1, parentSocket.Position.y, parentSocket.Position.z), parentSocket ),
            ( DirectionIndex.Back, new Vector3Int(parentSocket.Position.x, parentSocket.Position.y, parentSocket.Position.z + 1), parentSocket ),
            ( DirectionIndex.Down, new Vector3Int(parentSocket.Position.x, parentSocket.Position.y + 1, parentSocket.Position.z), parentSocket ),
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
