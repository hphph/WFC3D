using UnityEditor;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField]
    FiniteMap map;
    [SerializeField] CollapsedMap collapsedMap;

    public void GenerateMap()
    {
        map.Clear();
        bool hasCollapsed = true;
        while(hasCollapsed)
        {
            hasCollapsed = map.CollapseLowestEntropyModuleSocketAndPropagateChange();
        }
        Debug.Log("Map Generated");
    }
    
    public void Clear()
    {
        map.Clear();
    }

    public void CollapseNextSlot()
    {
        Debug.Log("Has collapsed: " + map.CollapseLowestEntropyModuleSocketAndPropagateChange());
    }

    public void CreateCollapsedMap()
    {
        map.CreateCollapsedMap();
    }

    public void ShowCollapsedMap()
    {
        for(int i = 0; i < collapsedMap.Size.z; i++)
        {
        for(int j = 0; j < collapsedMap.Size.y; j++)
        {
        for(int k = 0; k < collapsedMap.Size.x; k++)
        {
            Module toShow = collapsedMap.GetModuleAt(new Vector3Int(k, j, i));
            GameObject gameObject = Instantiate(toShow.Dummy.gameObject, transform, false);
            gameObject.transform.position = 2 * new Vector3(k, j, i);
            gameObject.transform.rotation = Quaternion.Euler(0, 90 * toShow.Rotation, 0);
        }
        }
        }
    }
}
