using UnityEngine;

[System.Serializable]
public class CollapsedMapData
{
    public Vector3Int Size;
    public Vector3 ModuleSize;
    public ModuleData ModuleData;
    public Module[] MapData;

    public CollapsedMapData(Vector3Int size, Vector3 moduleSize, ModuleData moduleData, Module[] mapData)
    {
        Size = size;
        ModuleSize = moduleSize;
        ModuleData = moduleData;
        MapData = mapData;
    }

    public Module GetModuleAt(Vector3Int position) => MapData[position.z * Size.y * Size.x + position.y * Size.x + position.x];
}
