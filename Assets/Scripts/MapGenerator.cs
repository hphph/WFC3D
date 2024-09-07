using UnityEngine;
using UnityEditor;

public class MapGenerator : MonoBehaviour
{
    [SerializeField]
    FiniteMap map;

    public void GenerateMap()
    {
        bool hasCollapsed = true;
        while(hasCollapsed)
        {
            hasCollapsed = map.CollapseLowestEntropySlotAndPropagateChange();
        }
        Debug.Log("Map Generated");
    }
}
