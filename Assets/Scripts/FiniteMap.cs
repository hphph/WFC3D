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
        entropySortedSlotQueue = new PriorityQueueSet<Slot>();
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
        Queue<KeyValuePair<WFCTools.DirectionIndex, Vector3Int>> updateQueue = new Queue<KeyValuePair<WFCTools.DirectionIndex, Vector3Int>>(WFCTools.NeighboursToSlot(collapsedSlot.Position));
        while(updateQueue.Count > 0)
        {
            var neighbourPos = updateQueue.Dequeue();
            if(neighbourPos.Value.x >= size.x || neighbourPos.Value.y >= size.y || neighbourPos.Value.z >= size.z || neighbourPos.Value.x < 0 || neighbourPos.Value.y < 0 || neighbourPos.Value.z < 0) continue;
            Slot neighbour = mapData[neighbourPos.Value.x, neighbourPos.Value.y, neighbourPos.Value.z];
            if(neighbour.IsCollapsed) continue;
            if(neighbour.Spread(collapsedSlot.CollapsedModule, neighbourPos.Key))
            {
                foreach(var n in WFCTools.NeighboursToSlot(neighbourPos.Value))
                {
                    updateQueue.Enqueue(n);
                }
            }
            entropySortedSlotQueue.Add(neighbour, neighbour.Entropy());
        }
    }
    
    public bool CollapseLowestEntropySlotAndPropagateChange()
    {
        if(entropySortedSlotQueue.Count == 0) return false;
        Slot lowestEntropySlot = entropySortedSlotQueue.ExtractMin();
        lowestEntropySlot.Collapse();
        if(lowestEntropySlot.IsCollapsed)
        {
            Instantiate(lowestEntropySlot.CollapsedModule.Prefab, lowestEntropySlot.Position*2, Quaternion.Euler(0, 90 * lowestEntropySlot.CollapsedModule.Rotation, 0));
            PropagateSlotCollapse(lowestEntropySlot);
        }
        return true;
    }
}
