using System.Collections.Generic;
using UnityEngine;

public class FiniteMap: MonoBehaviour
{
    [SerializeField] Vector3Int size;
    [SerializeField] GameObject dummyModulesPrefab;
    [SerializeField] List<GameObject> mapDummyModulePrefabs;
    List<Module> generatedMapModules;
    Slot[,,] mapData;
    PriorityQueueSet<Slot> entropySortedSlotQueue;

    void Start()
    {
        mapData = new Slot[size.x, size.y, size.z];
        generatedMapModules = new List<Module>();
        mapDummyModulePrefabs = new List<GameObject>(dummyModulesPrefab.transform.childCount);
        foreach(Transform child in dummyModulesPrefab.transform)
        {
            mapDummyModulePrefabs.Add(child.gameObject);
        }
        foreach(GameObject prefab in mapDummyModulePrefabs)
        {
            generatedMapModules.AddRange(Module.GenerateModulesFromDummy(prefab));
        }

        //Iterating through map array
        for(int i = 0; i < size.z; i++)
        {
        for(int j = 0; j < size.y; j++)
        {
        for(int k = 0; k < size.x; k++)
        {
            mapData[k,j,i] = new Slot(new Vector3Int(k,j,i), generatedMapModules);
        }
        }
        }

        entropySortedSlotQueue.Add(mapData[size.x/2, size.y/2, size.z/2], 0);
    }

    public Slot GetSlotAt(int x, int y, int z)
    {
        return mapData[x, y, z];
    }

    //TODO: Add queue of propagation
    public void PropagateSlotCollapse(Slot collapsedSlot)
    {
        foreach(KeyValuePair<WFCTools.DirectionIndex, Vector3Int> neighbourPos in WFCTools.NeighboursToSlot(collapsedSlot.Position))
        {
            Slot neighbour = mapData[neighbourPos.Value.x, neighbourPos.Value.y, neighbourPos.Value.z];
            neighbour.Spread(collapsedSlot.CollapsedModule, neighbourPos.Key);
        }
    }
    
    public void CollapseLowestEntropySlot()
    {
        Slot lowestEntropySlot = entropySortedSlotQueue.ExtractMin();
        lowestEntropySlot.Collapse();
        
    }
}
