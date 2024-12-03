using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        MapGenerator generator = (MapGenerator)target;
        if(GUILayout.Button("Generate map"))
        {
            generator.GenerateMap();
        }
        if(GUILayout.Button("Collapse next slot"))
        {
            generator.CollapseNextSlot();
        }
        if(GUILayout.Button("Clear map"))
        {
            generator.Clear();
        }
        if(GUILayout.Button("Create CollapsedMap"))
        {
            generator.CreateCollapsedMap();
        }
        if(GUILayout.Button("Show Collapsed Map"))
        {
            generator.ShowCollapsedMap();
        }
    }
}
