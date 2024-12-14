using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CollapsedMapDatabase))]
public class CollapsedMapDatabaseEditor: Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        CollapsedMapDatabase collapsedMapDatabase = (CollapsedMapDatabase)target;

        if(GUILayout.Button("Generate new collapsed map"))
        {
            collapsedMapDatabase.CreateNewMapBasedOnInitial();
        }
        if(GUILayout.Button("Show maps"))
        {
            collapsedMapDatabase.ShowCollapsedBasedMaps();
        }
        
    }
}