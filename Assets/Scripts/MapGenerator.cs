using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;

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
        for(int i = 0; i < 4; i++)
        {
            Debug.Log(Module.RotateHorizontallyConnector(0, i));
        }
        Debug.Log("Has collapsed: " + map.CollapseLowestEntropySlotAndPropagateChange());
    }
}
