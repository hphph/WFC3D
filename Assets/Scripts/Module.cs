using System;
using UnityEngine;

public class Module
{
    DummyModule dummy;
    GameObject prefab;
    int rotation;
    int modulesFromDummy;

    public DummyModule Dummy => dummy;
    public GameObject Prefab => prefab;
    public int Rotation => rotation;
    public float Probability => dummy.Probability/modulesFromDummy;
    
    public Module(GameObject dummyPrefab, int rotation, int modulesFromDummy)
    {
        dummy = dummyPrefab.GetComponent<DummyModule>();
        if(dummy == null) Debug.LogError("Missing DummyModule component in prefab");
        prefab = dummyPrefab;
        this.rotation = rotation;
        if(rotation > 3) Debug.LogError("Wrong rotation value");
        this.modulesFromDummy = modulesFromDummy;
    }

    public bool IsFitting(Module neighbour, int connectorIndexToNeighbour)
    {
        if(WFCTools.IsConnectorHorizontal(connectorIndexToNeighbour))
        {
            var connector1 = neighbour.dummy.ModuleConnectors[RotateHorizontallyConnector(WFCTools.OppositeConnectorIndex(connectorIndexToNeighbour), neighbour.Rotation)] as DummyModule.HorizontalConnector;
            var connector2 = dummy.ModuleConnectors[RotateHorizontallyConnector(connectorIndexToNeighbour, rotation)] as DummyModule.HorizontalConnector;
            return (connector1.ConnectionId == connector2.ConnectionId) && 
                (connector1.Symmetric || connector2.Symmetric || (!connector1.Filpped == connector2.Filpped));
        }
        else
        {
            var connector1 = neighbour.Dummy.OppositeConnector(connectorIndexToNeighbour) as DummyModule.VerticalConnector;
            var connector2 = dummy.ModuleConnectors[connectorIndexToNeighbour] as DummyModule.VerticalConnector;
            return (connector1.ConnectionId == connector2.ConnectionId) && 
                (connector1.rotation == DummyModule.VerticalConnector.RotationState.Invariant || connector2.rotation == DummyModule.VerticalConnector.RotationState.Invariant || 
                    connector1.rotation == connector2.rotation);
        }
    }

    static public int[] HorizontalIndexes()
    {
        return new int[]{0, 1, 3, 4};
    }

    static public int RotateHorizontallyConnector(int connectorIndex, int rotation)
    {
        return HorizontalIndexes()[mod(Array.IndexOf<int>(HorizontalIndexes(), connectorIndex) - rotation, 4)];
    }

    public static int mod(int x, int m) 
    {
        return (x%m + m)%m;
    }

    public static Module[] GenerateModulesFromDummy(GameObject dummyPrefab)
    {
        //TODO: make less modules based on dummy connections 
        Module[] generatedModules = new Module[4];
        generatedModules[0] = new Module(dummyPrefab, 0, 4);
        generatedModules[1] = new Module(dummyPrefab, 1, 4);
        generatedModules[2] = new Module(dummyPrefab, 2, 4);
        generatedModules[3] = new Module(dummyPrefab, 3, 4);

        return generatedModules;
    }
}
