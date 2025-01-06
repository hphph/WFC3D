using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class InfiniteMap : MonoBehaviour
{
    [SerializeField] CollapsedMapDatabase generatedChunks;
    [SerializeField] int nearRenderDistance;
    [SerializeField] int farRenderDistance;
    [SerializeField] GameObject playerObject;

    Dictionary<Vector3Int, ModuleSocket> mapData;
    Dictionary<Vector2Int, GameObject> chunkMap;
    Dictionary<Vector2Int, bool> chunkCompletionStatus;
    Vector3Int chunkSize;
    Vector2Int currentPlayerPosition;

    async void Start()
    {
        mapData = new Dictionary<Vector3Int, ModuleSocket>();
        chunkMap = new Dictionary<Vector2Int, GameObject>();
        chunkCompletionStatus = new  Dictionary<Vector2Int, bool>();
        chunkSize = generatedChunks.initialMap.Size;
        currentPlayerPosition = PlayerChunkPosition();
        await UpdateChunks();
    }

    async void Update()
    {
        if(currentPlayerPosition != PlayerChunkPosition())
        {
            currentPlayerPosition = PlayerChunkPosition();
            await UpdateChunks();
        }
    }

    public Vector2Int PlayerChunkPosition()
    {
        Vector3 pos = playerObject.transform.position;
        return new Vector2Int(Mathf.FloorToInt(pos.x/(chunkSize.x * generatedChunks.initialMap.ModuleSize.x)), Mathf.CeilToInt(pos.z/(chunkSize.z * generatedChunks.initialMap.ModuleSize.x)));
    }

    public Vector2Int GlobalPositionToChunkPosition(Vector2Int position)
    {
        return new Vector2Int(Mathf.FloorToInt(position.x/(chunkSize.x * generatedChunks.initialMap.ModuleSize.x)), Mathf.CeilToInt(position.y/(chunkSize.z * generatedChunks.initialMap.ModuleSize.z)));
    }

    public Vector2Int ChunkToGlobalPosition(Vector2Int position)
    {
        return new Vector2Int(position.x * chunkSize.x, position.y * chunkSize.z);
    }

    public async Task UpdateChunks()
    {
        for(int y = -farRenderDistance + currentPlayerPosition.y; y < farRenderDistance + currentPlayerPosition.y; y++)
        {
            for(int x = farRenderDistance + currentPlayerPosition.x; x > -farRenderDistance + currentPlayerPosition.x; x--)
            {
                Vector2Int pos = new Vector2Int(x, y);
                if(chunkMap.TryGetValue(pos, out GameObject mapState))
                {
                    if(mapState == null) InitiateChunk(pos);
                }
                else InitiateChunk(pos);
                await Task.Yield();
            }
        }

        int chunksGenerated = 0;
        for(int y = -nearRenderDistance + currentPlayerPosition.y; y < nearRenderDistance + currentPlayerPosition.y; y++)
        {
            for(int x = nearRenderDistance + currentPlayerPosition.x; x > -nearRenderDistance + currentPlayerPosition.x; x--)
            {
                Vector2Int pos = new Vector2Int(x, y);
                if(chunkCompletionStatus.TryGetValue(pos, out bool comeltionStatus))
                {
                    if(comeltionStatus == false) await ResetAndRecollapseChunk(pos);
                }
                chunksGenerated++;
            }
        }
    }

    async Task ResetAndRecollapseChunk(Vector2Int chunkPosition)
    {

        await ResetChunk(chunkPosition);
        await RecollapseChunk(chunkPosition);
    }

    private async Task ResetChunk(Vector2Int chunkPosition)
    {
        Vector2Int globalPosition = ChunkToGlobalPosition(chunkPosition);
        chunkCompletionStatus.Remove(chunkPosition, out bool haveCompleted);
        if (haveCompleted)
        {
            Debug.Log("Chunk already recollapsed");
            return;
        }

        for (int z = globalPosition.y - (chunkSize.z / 2); z < globalPosition.y + (chunkSize.z / 2); z++)
        {
            for (int x = globalPosition.x - (chunkSize.x / 2); x < globalPosition.x + (chunkSize.x / 2); x++)
            {
                for (int y = 0; y < chunkSize.y; y++)
                {
                    mapData.TryGetValue(new Vector3Int(x, y, z), out ModuleSocket socket);
                    if (socket == null) Debug.Log(new Vector3Int(x, y, z));
                    else
                    {
                        Destroy(socket.SocketGO);
                        socket.ResetSocket(generatedChunks.initialMap.ModuleData.Modules.Select(m => m.Index));
                    }
                }
            }
            await Task.Yield();
        }
    }

    void InitiateChunk(Vector2Int chunkPosition)
    {
        GameObject chunkGo = new GameObject("Chunk " + chunkPosition);
        chunkGo.transform.SetParent(transform);
        chunkGo.transform.localPosition = new Vector3(chunkPosition.x * chunkSize.x, 0, chunkPosition.y * chunkSize.z);
        CollapsedMapData collapsedMap = generatedChunks.basedMaps[Random.Range(0, generatedChunks.basedMaps.Length)];
        chunkMap.Add(chunkPosition, chunkGo);
        chunkCompletionStatus.Add(chunkPosition, false);
        for(int i = 0; i < collapsedMap.Size.z; i++)
        {
        for(int j = 0; j < collapsedMap.Size.y; j++)
        {
        for(int k = 0; k < collapsedMap.Size.x; k++)
        {
            Vector3Int moduleSocketGlobalPosition = new Vector3Int(chunkPosition.x * chunkSize.x, 0, chunkPosition.y * chunkSize.z) + new Vector3Int(k,j,i);
            Module module = collapsedMap.GetModuleAt(new Vector3Int(k, j, i));
            GameObject socketGO = Instantiate(module.Dummy.gameObject, chunkGo.transform, false);
            ModuleSocket moduleSocket = new ModuleSocket(moduleSocketGlobalPosition, generatedChunks.initialMap.ModuleData.Modules.Select(m => m.Index), module, socketGO, generatedChunks.initialMap.ModuleData);
            mapData.Add(moduleSocketGlobalPosition, moduleSocket);
            socketGO.name += " " + moduleSocketGlobalPosition;
            socketGO.transform.localPosition = new Vector3(chunkPosition.x * chunkSize.x, 0, chunkPosition.y * chunkSize.z) 
            + new Vector3(k * collapsedMap.ModuleSize.x,j * collapsedMap.ModuleSize.y,i * collapsedMap.ModuleSize.z);
            socketGO.transform.localRotation = Quaternion.Euler(0, 90 * module.Rotation, 0);
        }
        }
        }
    }

    public void PropagateSocketCollapse(ModuleSocket collapsedSocket, PriorityQueueSet<ModuleSocket> collapseQueue)
    {
        Queue<(WFCTools.DirectionIndex, Vector3Int, ModuleSocket)> updateQueue = new Queue<(WFCTools.DirectionIndex, Vector3Int, ModuleSocket)>(WFCTools.NeighboursToSocket(collapsedSocket));
        while(updateQueue.Count > 0)
        {
            var queueElement = updateQueue.Dequeue();
            ModuleSocket queueElementSocket = GetSocketAt(queueElement.Item2);
            if(queueElementSocket == null) continue;
            if(queueElementSocket.IsCollapsed) continue;
            if(queueElementSocket.Spread(queueElement.Item3, queueElement.Item1))
            {
                foreach(var n in WFCTools.NeighboursToSocket(queueElementSocket))
                {
                    updateQueue.Enqueue(n);
                }
            }
            collapseQueue.Add(queueElementSocket, queueElementSocket.Entropy());
        }
    }
    
    ModuleSocket GetSocketAt(Vector3Int position)
    {
        if(mapData.ContainsKey(position)) return mapData[position];
        else return null;
    }

    public async Task RecollapseChunk(Vector2Int chunkPosition)
    {
        bool hasRecollapsed = false;
        while(!hasRecollapsed)
        {
            PriorityQueueSet<ModuleSocket> collapseQueue = new PriorityQueueSet<ModuleSocket>();
            Vector2Int globalPosition = ChunkToGlobalPosition(chunkPosition);
            PropagateBound(collapseQueue, globalPosition);
            while (collapseQueue.Count > 0)
            {
                ModuleSocket nextToCollapse = collapseQueue.ExtractMin();
                if (nextToCollapse.Possibilities.Count == 0)
                {
                    hasRecollapsed = false;
                    break;
                }
                nextToCollapse.Collapse();
                if(nextToCollapse.IsCollapsed)
                {
                    Vector2Int ChunkPos = GlobalPositionToChunkPosition(new Vector2Int(nextToCollapse.Position.x, nextToCollapse.Position.z));
                    GameObject chunkGO = chunkMap[ChunkPos];
                    GameObject socketGO = Instantiate(nextToCollapse.CollapsedModule.Dummy.gameObject, chunkGO.transform, false);
                    socketGO.name += " " + nextToCollapse.Position;
                    socketGO.transform.position = new Vector3(nextToCollapse.Position.x * generatedChunks.initialMap.ModuleSize.x,nextToCollapse.Position.y * generatedChunks.initialMap.ModuleSize.y,nextToCollapse.Position.z * generatedChunks.initialMap.ModuleSize.z);
                    socketGO.transform.localRotation = Quaternion.Euler(0, 90 * nextToCollapse.CollapsedModule.Rotation, 0);
                    nextToCollapse.SetSocketGO(socketGO);
                    PropagateSocketCollapse(nextToCollapse, collapseQueue);
                }
                
                    
            }
            if(collapseQueue.Count == 0)
            {
                chunkCompletionStatus.Add(chunkPosition, true);
                return;
            }
            else
            {
                await ResetChunk(chunkPosition);
            }
        }
    }

    private void PropagateBound(PriorityQueueSet<ModuleSocket> collapseQueue, Vector2Int globalPosition)
    {
        for (int z = globalPosition.y - (chunkSize.z / 2) - 1; z < globalPosition.y + (chunkSize.z / 2) + 1; z++)
        {
            for (int x = globalPosition.x - (chunkSize.x / 2) - 1; x < globalPosition.x + (chunkSize.x / 2) + 1; x++)
            {
                for (int y = 0; y < chunkSize.y; y++)
                {
                    if (x == globalPosition.x - (chunkSize.x / 2) - 1 || z == globalPosition.y - (chunkSize.z / 2) - 1 || x == globalPosition.x + (chunkSize.x / 2) || z == globalPosition.y + (chunkSize.z / 2))
                    {
                        ModuleSocket propagationSocket = GetSocketAt(new Vector3Int(x, y, z));
                        if (propagationSocket != null)  PropagateSocketCollapse(propagationSocket, collapseQueue);
                        else Debug.Log("ERROR" + new Vector3Int(x, y, z));
                    }
                }
            }
        }
    }
}
