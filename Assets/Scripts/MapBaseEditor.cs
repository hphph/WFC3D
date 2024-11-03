using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapBase))]
public class MapBaseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        MapBase mapBase = (MapBase)target;

        if(GUILayout.Button("Generate frame"))
        {
            mapBase.FillWithFrame(mapBase.frameSize, mapBase.position);
        }
    }
}
