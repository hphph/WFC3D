using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ModulesData))]
public class ModuleDataEditor: Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        ModulesData moduleData = (ModulesData)target;

        if(GUILayout.Button("Generate Module Data"))
        {
            moduleData.GenerateModulesDataFromPrefab();
        }
    }
}