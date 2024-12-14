using UnityEngine;

[CreateAssetMenu(fileName = "Collapsed Map Database", menuName ="WFC/CollapsedMapDatabase", order = 1)]
public class CollapsedMapDatabase : ScriptableObject
{
    public CollapsedMapSO initialMap;
    public CollapsedMapData[] basedMaps;

    public void CreateNewMapBasedOnInitial()
    {
        if(basedMaps.Length == 0)
        {
            basedMaps = new CollapsedMapData[1];
            basedMaps[0] = initialMap.ScriptableObjectToData();
        }
        CollapsedMapData[] updatedBasedMaps = new CollapsedMapData[basedMaps.Length + 1];
        basedMaps.CopyTo(updatedBasedMaps, 0);
        //Generate new map based on initial =)
        GameObject finiteMapGO = new GameObject("TempFiniteMap", typeof(FiniteMap));
        FiniteMap tempFiniteMap = finiteMapGO.GetComponent<FiniteMap>();
        updatedBasedMaps[updatedBasedMaps.Length-1] = tempFiniteMap.GenerateCollapsedMapWithBoundary(initialMap);
        DestroyImmediate(finiteMapGO);
        basedMaps = updatedBasedMaps;
    }

     public void ShowCollapsedBasedMaps()
    {
        GameObject root = new GameObject("root");
        Vector3 offset = Vector3.zero;
        foreach(var map in basedMaps)
        {
            for(int i = 0; i < map.Size.z; i++)
            {
            for(int j = 0; j < map.Size.y; j++)
            {
            for(int k = 0; k < map.Size.x; k++)
            {
                Module toShow = map.GetModuleAt(new Vector3Int(k, j, i));
                GameObject gameObject = Instantiate(toShow.Dummy.gameObject, root.transform, false);
                gameObject.transform.position = offset + 2 * new Vector3(k, j, i);
                gameObject.transform.rotation = Quaternion.Euler(0, 90 * toShow.Rotation, 0);
            }
            }
            }
            offset += new Vector3(map.ModuleSize.x * map.Size.x,0,0);
        }
    }
}
