using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField]
    FiniteMap map;

    public void GenerateMap()
    {
        map.Clear();
        bool hasCollapsed = true;
        while(hasCollapsed)
        {
            hasCollapsed = map.CollapseLowestEntropySlotAndPropagateChange();
        }
        Debug.Log("Map Generated");
    }
    
    public void Clear()
    {
        map.Clear();
    }

    public void CollapseNextSlot()
    {
        Debug.Log("Has collapsed: " + map.CollapseLowestEntropySlotAndPropagateChange());
    }
}
