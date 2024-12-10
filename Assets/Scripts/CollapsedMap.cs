using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class CollapsedMap : ScriptableObject
{
    public Vector3Int Size;
    public Module[] MapData;

    public void CreateCollapsedMap(Vector3Int size, Module[] mapData, string name)
    {
        Size = size;
        MapData = new Module[Size.x*Size.y*Size.z];
        System.Array.Copy(mapData, MapData, Size.x*Size.y*Size.z);
        AssetDatabase.CreateAsset(this, "Assets/CollapsedMaps/CollapsedMap_" + name + ".asset");
        AssetDatabase.SaveAssets();
    }


    public Module GetModuleAt(Vector3Int position) => MapData[position.z * Size.y * Size.x + position.y * Size.x + position.x];
    
}