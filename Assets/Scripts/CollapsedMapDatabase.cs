using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Collapsed Map Database", menuName ="WFC/CollapsedMapDatabase", order = 1)]
public class CollapsedMapDatabase : ScriptableObject
{
    public CollapsedMap initialMap;
    public CollapsedMap[] basedMaps;

    public void CreateNewBasedOnInitial()
    {
        if(!AssetDatabase.IsValidFolder("Assets/CollapsedMaps/"+ initialMap.name)) AssetDatabase.CreateFolder("Assets/CollapsedMaps", initialMap.name);
    }
}
