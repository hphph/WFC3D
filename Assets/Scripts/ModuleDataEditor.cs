using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ModuleData))]
public class ModuleDataEditor: Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        ModuleData moduleData = (ModuleData)target;

        if(GUILayout.Button("Generate Module Data"))
        {
            moduleData.GenerateModulesDataFromPrefab();
        }
    }
}