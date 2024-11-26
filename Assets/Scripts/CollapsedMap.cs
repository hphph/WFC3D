using UnityEngine;

[CreateAssetMenu(fileName = "Collapsed Map", menuName = "WFC/CollapsedMap", order = 1)]
public class CollapsedMap : ScriptableObject
{
    [SerializeField] Vector3Int chunkSize;
    [SerializeField] Vector3 moduleSize;
    [SerializeField] GameObject dummyModulesPrefab;
    public FiniteMap mapData;
    GameObject mapGO;

    public void GenerateCollapsedMap()
    {
        mapGO = new GameObject("CollapsedMap");
        mapData = mapGO.AddComponent<FiniteMap>();
        mapData.InitCollapsedMap(chunkSize, moduleSize, dummyModulesPrefab);
    }
}