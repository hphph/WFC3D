using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteMap : MonoBehaviour
{
    [SerializeField] CollapsedMapDatabase generatedChunks;
    [SerializeField] int nearRenderDistance;
    [SerializeField] int farRenderDistance;
    [SerializeField] GameObject playerObject;

    Dictionary<Vector3Int, ModuleSocket> mapData;
    Dictionary<Vector2Int, int> chunkMap;
    Vector2Int chunkSize;
    Vector2Int currentPlayerPosition;

    void Start()
    {
        mapData = new Dictionary<Vector3Int, ModuleSocket>();
        chunkMap = new Dictionary<Vector2Int, int>();
        chunkSize = new Vector2Int(generatedChunks.initialMap.Size.x, generatedChunks.initialMap.Size.z);
        InitiateChunks();
        currentPlayerPosition = PlayerChunkPosition();
    }

    void Update()
    {
        if(currentPlayerPosition != PlayerChunkPosition())
        {
            currentPlayerPosition = PlayerChunkPosition();
            Debug.Log(currentPlayerPosition);
            UpdateChunks();
        }
    }

    public Vector2Int PlayerChunkPosition()
    {
        Vector3 pos = playerObject.transform.position;
        return new Vector2Int(Mathf.FloorToInt(pos.x/chunkSize.x), Mathf.CeilToInt(pos.z/chunkSize.y));
    }

    public void UpdateChunks()
    {
         for(int y = -farRenderDistance + currentPlayerPosition.y; y < farRenderDistance + currentPlayerPosition.y; y++)
        {
            for(int x = farRenderDistance + currentPlayerPosition.x; x > -farRenderDistance + currentPlayerPosition.x; x--)
            {
                Vector2Int pos = new Vector2Int(x, y);
                if(chunkMap.TryGetValue(pos, out int mapState))
                {
                    if(mapState == 0) InitiateChunk(pos);
                }
                else InitiateChunk(pos);
            }
        }
    }

    public void InitiateChunks()
    {
        for(int y = -farRenderDistance; y < farRenderDistance; y++)
        {
            for(int x = farRenderDistance; x > -farRenderDistance; x--)
            {
                InitiateChunk(new Vector2Int(x, y));
            }

        }
    }

    void InitiateChunk(Vector2Int chunkPosition)
    {
        GameObject chunkGo = new GameObject("Chunk " + chunkPosition);
        chunkGo.transform.SetParent(transform);
        chunkGo.transform.localPosition = new Vector3(chunkPosition.x * chunkSize.x, 0, chunkPosition.y * chunkSize.y);
        CollapsedMapData collapsedMap = generatedChunks.basedMaps[Random.Range(0, generatedChunks.basedMaps.Length)];
        chunkMap.Add(chunkPosition, 1);
        for(int i = 0; i < collapsedMap.Size.z; i++)
        {
        for(int j = 0; j < collapsedMap.Size.y; j++)
        {
        for(int k = 0; k < collapsedMap.Size.x; k++)
        {
            Vector3Int moduleSocketGlobalPosition = new Vector3Int(chunkPosition.x * chunkSize.x, 0, chunkPosition.y * chunkSize.y) + new Vector3Int(k,j,i);
            Module module = collapsedMap.GetModuleAt(new Vector3Int(k, j, i));
            ModuleSocket moduleSocket = new ModuleSocket(moduleSocketGlobalPosition, generatedChunks.initialMap.ModuleData.Modules, module);
            mapData.Add(moduleSocketGlobalPosition, moduleSocket);
            GameObject gameObject = Instantiate(module.Dummy.gameObject, chunkGo.transform, false);
            gameObject.name += " " + moduleSocketGlobalPosition;
            gameObject.transform.localPosition = new Vector3(chunkPosition.x * chunkSize.x, 0, chunkPosition.y * chunkSize.y) 
            + new Vector3(k * collapsedMap.ModuleSize.x,j * collapsedMap.ModuleSize.y,i * collapsedMap.ModuleSize.z);
            gameObject.transform.localRotation = Quaternion.Euler(0, 90 * module.Rotation, 0);
        }
        }
        }
}
    
    ModuleSocket GetSocketAt(Vector3Int position)
    {
        if(mapData.ContainsKey(position)) return mapData[position];
        else return null;
    }

    public IEnumerator StartMapGeneration()
    {
        yield return null;
    }
}
