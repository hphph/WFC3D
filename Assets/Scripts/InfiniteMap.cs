using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteMap : MonoBehaviour
{
    [SerializeField] Vector3 moduleSize;
    [SerializeField] GameObject dummyModulesPrefab;
    List<GameObject> mapDummyModulePrefabs;
    List<Module> generatedMapModules;
    Dictionary<Vector3Int, ModuleSocket> mapData;
    
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
