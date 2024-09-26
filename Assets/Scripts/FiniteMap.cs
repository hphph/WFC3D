using System.Collections.Generic;
using Unity.VisualScripting;
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
        Queue<(WFCTools.DirectionIndex, Vector3Int, Slot)> updateQueue = new Queue<(WFCTools.DirectionIndex, Vector3Int, Slot)>(WFCTools.NeighboursToSlot(collapsedSlot));
        while(updateQueue.Count > 0)
        {
            var queueElement = updateQueue.Dequeue();
            if(queueElement.Item2.x >= size.x || queueElement.Item2.y >= size.y || queueElement.Item2.z >= size.z || 
                queueElement.Item2.x < 0 || queueElement.Item2.y < 0 || queueElement.Item2.z < 0) continue;
            Slot queueElementSlot = mapData[queueElement.Item2.x, queueElement.Item2.y, queueElement.Item2.z];
            if(queueElementSlot.IsCollapsed) continue;
            if(queueElementSlot.Spread(queueElement.Item3, queueElement.Item1))
            {
                foreach(var n in WFCTools.NeighboursToSlot(queueElementSlot))
                {
                    updateQueue.Enqueue(n);
                }
            }
            entropySortedSlotQueue.Add(queueElementSlot, queueElementSlot.Entropy());
        }
    }
    
    public bool CollapseLowestEntropySlotAndPropagateChange()
    {
        if(entropySortedSlotQueue.Count == 0) return false;
        Slot lowestEntropySlot = entropySortedSlotQueue.ExtractMin();
        lowestEntropySlot.Collapse();
        if(lowestEntropySlot.IsCollapsed)
        {
            Instantiate(lowestEntropySlot.CollapsedModule.Prefab, lowestEntropySlot.Position*2, Quaternion.Euler(0, 90 * lowestEntropySlot.CollapsedModule.Rotation, 0), transform);
            PropagateSlotCollapse(lowestEntropySlot);
        }
        return true;
    }

    public void Clear()
    {
        entropySortedSlotQueue = new PriorityQueueSet<Slot>();
        mapData = new Slot[size.x, size.y, size.z];
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
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
}
