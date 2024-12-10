using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FiniteMap: MonoBehaviour
{
    [SerializeField] Vector3Int size;
    [SerializeField] Vector3 moduleSize;
    [SerializeField] ModuleData modulesData;
    [SerializeField] bool IsWrapping;
    [SerializeField] bool IsDebugging;
    [SerializeField] MapBase[] mapBases;
    ModuleSocket[,,] mapData;
    GameObject[,,] spawnSlots;
    PriorityQueueSet<ModuleSocket> entropySortedModuleSocketQueue;
    Dictionary<string, IEnumerable<Module>> tagModuleCache;

    public Vector3Int Size => size;
    public Vector3 MoudleSize => moduleSize;
    
    void Awake()
    {
        PreInit();
    }

    public void InitCollapsedMap(Vector3Int size, Vector3 moduleSize, GameObject dummyModulesPrefab)
    {
        this.size = size;
        this.moduleSize = moduleSize;
        IsWrapping = true;
        IsDebugging = false;
    }

    void PreInit()
    {
        tagModuleCache = new Dictionary<string, IEnumerable<Module>>();
        foreach(MapBase mapBase in mapBases)
        {
            Vector3Int topRightPoint = mapBase.TopRightBasePoint();
            size.x = topRightPoint.x > size.x ? topRightPoint.x : size.x; 
            size.y = topRightPoint.y > size.y ? topRightPoint.y : size.y; 
            size.z = topRightPoint.z > size.z ? topRightPoint.z : size.z; 
        }
        InitNewMap();
    }

    public void InitNewMap()
    {
        entropySortedModuleSocketQueue = new PriorityQueueSet<ModuleSocket>();
        mapData = new ModuleSocket[size.x, size.y, size.z];
        spawnSlots = new GameObject[size.x, size.y, size.z];
        
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
                mapData[k,j,i] = new ModuleSocket(new Vector3Int(k,j,i), initModules);
            }
            else
            {
                mapData[k,j,i] = new ModuleSocket(new Vector3Int(k,j,i), modulesData.Modules);
            }
            spawnSlots[k,j,i] = new GameObject("Slot " + "( " + k + ", " + j + ", " + i + ")");
            spawnSlots[k,j,i].transform.SetParent(transform);
            spawnSlots[k,j,i].transform.localPosition = new Vector3(k * moduleSize.x, j*moduleSize.y, i*moduleSize.z);
            if(IsDebugging)
            {
                spawnSlots[k,j,i].AddComponent<DebugSlot>();
                spawnSlots[k,j,i].GetComponent<DebugSlot>().SetObservedSlot(mapData[k, j, i]);
            }
        }
        }
        }

        entropySortedModuleSocketQueue.Add(mapData[size.x/2, size.y/2, size.z/2], mapData[size.x/2, size.y/2, size.z/2].Entropy());
    }

    public ModuleSocket GetSocketAt(Vector3Int position)
    {
        return mapData[position.x, position.y, position.z];
    }

    public IEnumerable<Module> ModulesWithTag(string tag)
    {
        return modulesData.Modules.Where<Module>(m => m.Tags.Contains<string>(tag));
    }

    public void PropagateSocketCollapse(ModuleSocket collapsedSocket)
    {
        Queue<(WFCTools.DirectionIndex, Vector3Int, ModuleSocket)> updateQueue = new Queue<(WFCTools.DirectionIndex, Vector3Int, ModuleSocket)>(WFCTools.NeighboursToSocket(collapsedSocket));
        while(updateQueue.Count > 0)
        {
            var queueElement = updateQueue.Dequeue();
            if(IsWrapping)
            {
                queueElement.Item2.x = WFCTools.Mod(queueElement.Item2.x, size.x);
                queueElement.Item2.z = WFCTools.Mod(queueElement.Item2.z, size.z);
                if(queueElement.Item2.y >= size.y || queueElement.Item2.y < 0) continue;
            }
            else if(queueElement.Item2.x >= size.x || queueElement.Item2.y >= size.y || queueElement.Item2.z >= size.z || 
                queueElement.Item2.x < 0 || queueElement.Item2.y < 0 || queueElement.Item2.z < 0) continue;
            ModuleSocket queueElementSocket = GetSocketAt(queueElement.Item2);
            if(queueElementSocket.IsCollapsed) continue;
            if(queueElementSocket.Spread(queueElement.Item3, queueElement.Item1))
            {
                foreach(var n in WFCTools.NeighboursToSocket(queueElementSocket))
                {
                    updateQueue.Enqueue(n);
                }
            }
            entropySortedModuleSocketQueue.Add(queueElementSocket, queueElementSocket.Entropy());
        }
    }
    
    public bool CollapseLowestEntropyModuleSocketAndPropagateChange()
    {
        if(entropySortedModuleSocketQueue.Count == 0) return false;
        ModuleSocket lowestEntropySocket = entropySortedModuleSocketQueue.ExtractMin();
        if(lowestEntropySocket.Possibilities.Count == 0) 
        {
            Clear();
            return true;
        }
        lowestEntropySocket.Collapse();
        if(lowestEntropySocket.IsCollapsed)
        {
            GameObject collapsedModuleSocket = Instantiate(lowestEntropySocket.CollapsedModule.Prefab, spawnSlots[lowestEntropySocket.Position.x, lowestEntropySocket.Position.y, lowestEntropySocket.Position.z].transform, false);
            collapsedModuleSocket.transform.localPosition = Vector3.zero;
            collapsedModuleSocket.transform.rotation = Quaternion.Euler(0, 90 * lowestEntropySocket.CollapsedModule.Rotation, 0);
            PropagateSocketCollapse(lowestEntropySocket);
        }
        return true;
    }

    public void CreateCollapsedMap(string name)
    {
        CollapsedMap result = ScriptableObject.CreateInstance<CollapsedMap>();
        Module[] moduleData = new Module[size.x * size.y * size.z];
        for(int i = 0; i < size.z; i++)
        {
        for(int j = 0; j < size.y; j++)
        {
        for(int k = 0; k < size.x; k++)
        {
            moduleData[i * size.y  * size.x + j*size.x + k] = mapData[k,j,i].CollapsedModule;
        }
        }
        }
        result.CreateCollapsedMap(size, moduleData, name);
    }

    public void Clear()
    {
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        InitNewMap();
    }
}
