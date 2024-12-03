using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Collapsed Map Database", menuName ="WFC/CollapsedMapDatabase", order = 1)]
public class CollapsedMapDatabase : ScriptableObject
{
    public CollapsedMap initialMap;
    public void Add(CollapsedMap collapsedMap)
    {
        AssetDatabase.AddObjectToAsset(collapsedMap, AssetDatabase.GetAssetPath(this));
        AssetDatabase.SaveAssets();
    }

    public CollapsedMap[] GetCurrentCollapsedMap()
    {
        Object[] subAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath(AssetDatabase.GetAssetPath(this));
        List<CollapsedMap> maps = new List<CollapsedMap>(subAssets.Length);
        foreach(Object o in subAssets)
        {
            if(o.GetType() == typeof(CollapsedMap))
            {
                maps.Add((CollapsedMap)o);
            }
            else
            {
                Debug.Log("Sub Asset not of CollapsedMap type");
            }
        }
        return maps.ToArray();
    }

    public void Clear()
    {
        Object[] subAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath(AssetDatabase.GetAssetPath(this));
        foreach(Object o in subAssets)
        {
            DestroyImmediate(o, true);
        }
        AssetDatabase.SaveAssets();
    }
}
