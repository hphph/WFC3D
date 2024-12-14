using UnityEditor;
using UnityEngine;

public class CollapsedMapSO : ScriptableObject
{
    public Vector3Int Size;
    public Vector3 ModuleSize;
    public ModuleData ModuleData;
    public Module[] MapData;

    public static void CreateCollapsedMap(Vector3Int size, Module[] mapData, string name, ModuleData moduleData, Vector3 moduleSize)
    {
        CollapsedMapSO newCollapsedMap = CreateInstance<CollapsedMapSO>();
        newCollapsedMap.Size = size;
        newCollapsedMap.MapData = mapData;
        newCollapsedMap.ModuleData = moduleData;
        newCollapsedMap.ModuleSize = moduleSize;
        AssetDatabase.CreateAsset(newCollapsedMap, "Assets/CollapsedMaps/CollapsedMap_" + name + ".asset");
        AssetDatabase.SaveAssets();
    }

    public CollapsedMapData ScriptableObjectToData()
    {
        return new CollapsedMapData(Size, ModuleSize, ModuleData, MapData);
    }


    public Module GetModuleAt(Vector3Int position) => MapData[position.z * Size.y * Size.x + position.y * Size.x + position.x];
    
}