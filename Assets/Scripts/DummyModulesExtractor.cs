using UnityEditor;
using UnityEngine;

public class DummyModulesExtractor : MonoBehaviour
{
    public void ExtractDummyModules()
    {
        foreach(Transform child in transform)
        {
            DummyModuleSO dmSO = ScriptableObject.CreateInstance<DummyModuleSO>();
            dmSO.SetModuleData(child.GetComponent<DummyModule>());
            AssetDatabase.CreateAsset(dmSO, "Assets/DummyModuleScriptableObjects/" + transform.name + "/" + child.name + ".asset");
        }
    }
}
