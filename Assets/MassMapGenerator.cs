using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MassMapGenerator : MonoBehaviour
{
    [SerializeField] FiniteMap map;
    [SerializeField] Vector3Int numberOfMaps;
    [SerializeField] Vector3 spaceBetween;

    public void Start()
    {
        GenerateMaps();
    }

    public void GenerateMaps()
    {
        int generated = 0;
        for(int i = 0; i < numberOfMaps.x; i++)
        {
        for(int j = 0; j < numberOfMaps.y; j++)
        {
        for(int k = 0; k < numberOfMaps.z; k++)
        {
            FiniteMap newMap = Instantiate(map, new Vector3(spaceBetween.x * i, spaceBetween.y * j, spaceBetween.z * k), Quaternion.identity);
            if(newMap == null)
            {
                Debug.Log("NULL returning");
                return;
            }
            bool hasCollapsed = true;
            //newMap.Clear();
            while(hasCollapsed)
            {
                hasCollapsed = newMap.CollapseLowestEntropySlotAndPropagateChange();
            }
            generated++;
            Debug.Log("Generated " + (float)generated/(numberOfMaps.x*numberOfMaps.y*numberOfMaps.z) + "%");
        }
        }
        }
    }
}
