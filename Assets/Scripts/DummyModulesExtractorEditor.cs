using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DummyModulesExtractor))]
public class DummyModuleExtractorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        DummyModulesExtractor extractor = (DummyModulesExtractor)target;
        if(GUILayout.Button("Extract dummy modules"))
        {
            extractor.ExtractDummyModules();
        }
    }
}
