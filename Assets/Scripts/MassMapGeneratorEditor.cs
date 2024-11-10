using System.Collections;
using System.Collections.Generic;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MassMapGenerator))]
public class MassMapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        MassMapGenerator generator = (MassMapGenerator)target;
        if(GUILayout.Button("Start generation"))
        {
            generator.GenerateMaps();
        }
        if(GUILayout.Button("Start generate coroutine map"))
        {
            generator.GenerateStartCorutine();
        }
    }
}
