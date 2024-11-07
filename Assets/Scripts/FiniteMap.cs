using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FiniteMap: MonoBehaviour
{
    [SerializeField] Vector3Int size;
    [SerializeField] Vector3 moduleSize;
    [SerializeField] GameObject dummyModulesPrefab;
    [SerializeField] GameObject DebugSlotPrefab;
    [SerializeField] MapBase[] mapBases;
    List<GameObject> mapDummyModulePrefabs;
    List<Module> generatedMapModules;
    Slot[,,] mapData;
    GameObject[,,] debugSlots;
    PriorityQueueSet<Slot> entropySortedSlotQueue;
    Dictionary<string, IEnumerable<Module>> tagModuleCache;
    

    void Awake()
    {
        foreach(MapBase mapBase in mapBases)
        {
            Vector3Int topRightPoint = mapBase.TopRightBasePoint();
            size.x = topRightPoint.x > size.x ? topRightPoint.x : size.x; 
            size.y = topRightPoint.y > size.y ? topRightPoint.y : size.y; 
            size.z = topRightPoint.z > size.z ? topRightPoint.z : size.z; 
        }
        
        entropySortedSlotQueue = new PriorityQueueSet<Slot>();
        mapData = new Slot[size.x, size.y, size.z];
        debugSlots = new GameObject[size.x, size.y, size.z];
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
        tagModuleCache = new Dictionary<string, IEnumerable<Module>>();

        //Iterating through map array
        for(int i = 0; i < size.z; i++)
        {
        for(int j = 0; j < size.y; j++)
        {
        for(int k = 0; k < size.x; k++)
        {
            var bases = mapBases.Where(mb => mb.BaseModulesPositions.Contains(new Vector3Int(k, j, i)));
            if(bases.Count() > 0)
            {
                List<Module> initModules = new List<Module>();
                foreach(MapBase mb in bases)
                {
                    foreach(string tag in mb.ModulesTags)
                    {
                        IEnumerable<Module> cached;
                        if(tagModuleCache.TryGetValue(tag, out cached))
                        {
                            initModules.AddRange(cached);
                        }
                        else
                        {
                            cached = ModulesWithTag(tag);
                            tagModuleCache.Add(tag, cached);
                            initModules.AddRange(cached);
                        }
                    }
                }
                mapData[k,j,i] = new Slot(new Vector3Int(k,j,i), initModules);
            }
            else
            {
                mapData[k,j,i] = new Slot(new Vector3Int(k,j,i), generatedMapModules);
            }
            debugSlots[k,j,i] = Instantiate(DebugSlotPrefab,new Vector3Int(k,j,i)*2, Quaternion.identity, transform);
            debugSlots[k,j,i].GetComponent<DebugSlot>().SetObservedSlot(mapData[k, j, i]);
        }
        }
        }

        entropySortedSlotQueue.Add(mapData[0, 0, 0], mapData[0,0,0].Entropy());
    }

    public Slot GetSlotAt(Vector3Int position)
    {
        return mapData[position.x, position.y, position.z];
    }

    public IEnumerable<Module> ModulesWithTag(string tag)
    {
        return generatedMapModules.Where<Module>(m => m.Tags.Contains<string>(tag));
    }

    public void PropagateSlotCollapse(Slot collapsedSlot)
    {
        Queue<(WFCTools.DirectionIndex, Vector3Int, Slot)> updateQueue = new Queue<(WFCTools.DirectionIndex, Vector3Int, Slot)>(WFCTools.NeighboursToSlot(collapsedSlot));
        while(updateQueue.Count > 0)
        {
            var queueElement = updateQueue.Dequeue();
            if(queueElement.Item2.x >= size.x || queueElement.Item2.y >= size.y || queueElement.Item2.z >= size.z || 
                queueElement.Item2.x < 0 || queueElement.Item2.y < 0 || queueElement.Item2.z < 0) continue;
            Slot queueElementSlot = GetSlotAt(queueElement.Item2);
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
        if(lowestEntropySlot.Possibilities.Count == 0) 
        {
            Clear();
            return true;
        }
        lowestEntropySlot.Collapse();
        if(lowestEntropySlot.IsCollapsed)
        {
            Instantiate(lowestEntropySlot.CollapsedModule.Prefab, new Vector3(lowestEntropySlot.Position.x * moduleSize.x, lowestEntropySlot.Position.y * moduleSize.y, lowestEntropySlot.Position.z * moduleSize.z), Quaternion.Euler(0, 90 * lowestEntropySlot.CollapsedModule.Rotation, 0), debugSlots[lowestEntropySlot.Position.x, lowestEntropySlot.Position.y, lowestEntropySlot.Position.z].transform);
            PropagateSlotCollapse(lowestEntropySlot);
        }
        return true;
    }

    public void Clear()
    {
        entropySortedSlotQueue = new PriorityQueueSet<Slot>();
        mapData = new Slot[size.x, size.y, size.z];
        debugSlots = new GameObject[size.x, size.y, size.z];
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
            var bases = mapBases.Where(mb => mb.BaseModulesPositions.Contains(new Vector3Int(k, j, i)));
            if(bases.Count() > 0)
            {
                List<Module> initModules = new List<Module>();
                foreach(MapBase mb in bases)
                {
                    foreach(string tag in mb.ModulesTags)
                    {
                        IEnumerable<Module> cached;
                        if(tagModuleCache.TryGetValue(tag, out cached))
                        {
                            initModules.AddRange(cached);
                        }
                        else
                        {
                            cached = ModulesWithTag(tag);
                            tagModuleCache.Add(tag, cached);
                            initModules.AddRange(cached);
                        }
                    }
                }
                initModules.ForEach(m => Debug.Log(m.Dummy.name));
                mapData[k,j,i] = new Slot(new Vector3Int(k,j,i), initModules);
            }
            else
            {
                mapData[k,j,i] = new Slot(new Vector3Int(k,j,i), generatedMapModules);
            }
            debugSlots[k,j,i] = Instantiate(DebugSlotPrefab,new Vector3Int(k,j,i)*2, Quaternion.identity, transform);
            debugSlots[k,j,i].GetComponent<DebugSlot>().SetObservedSlot(mapData[k, j, i]);
        }
        }
        }

        entropySortedSlotQueue.Add(mapData[0, 0, 0], mapData[0,0,0].Entropy());
    }
}
