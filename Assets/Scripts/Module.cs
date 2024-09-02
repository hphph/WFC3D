using UnityEngine;

public class Module
{
    DummyModule dummy;
    GameObject prefab;
    Quaternion rotation;
    int modulesFromDummy;

    public DummyModule Dummy => dummy;
    public GameObject Prefab => prefab;
    public Quaternion Rotation => rotation;
    public float Probability => dummy.Probability/modulesFromDummy;
    
    public Module(GameObject dummyPrefab, Quaternion rotation, int modulesFromDummy)
    {
        dummy = dummyPrefab.GetComponent<DummyModule>();
        if(dummy == null) Debug.LogError("Missing DummyModule component in prefab");
        this.rotation = rotation;
        this.modulesFromDummy = modulesFromDummy;
    }

    public bool IsFitting(Module neighbour, int connectorIndexToNeighbour)
    {
        if(WFCTools.IsConnectorHorizontal(connectorIndexToNeighbour))
        {
            var connector1 = neighbour.Dummy.OppositeConnector(connectorIndexToNeighbour) as DummyModule.HorizontalConnector;
            var connector2 = dummy.ModuleConnectors[connectorIndexToNeighbour] as DummyModule.HorizontalConnector;
            return (connector1.ConnectionId == connector2.ConnectionId) && (connector1.Symmetric || connector2.Symmetric || ((connector1.Filpped || connector2.Filpped) && !(connector1.Filpped && connector2.Filpped)));
        }
        else
        {
            var connector1 = neighbour.Dummy.OppositeConnector(connectorIndexToNeighbour) as DummyModule.VerticalConnector;
            var connector2 = dummy.ModuleConnectors[connectorIndexToNeighbour] as DummyModule.VerticalConnector;
            return (connector1.ConnectionId == connector2.ConnectionId) && (connector1.rotation == DummyModule.VerticalConnector.RotationState.Invariant || connector2.rotation == DummyModule.VerticalConnector.RotationState.Invariant || connector1.rotation == connector2.rotation);
        }
    }

    public static Module[] GenerateModulesFromDummy(GameObject dummyPrefab)
    {
        //TODO: make less modules based on dummy connections 
        Module[] generatedModules = new Module[4];
        for(int i = 0; i < 4; i++)
        {
            generatedModules[i] = new Module(dummyPrefab, Quaternion.Euler(0, 90 * i, 0), 4);
        }

        return generatedModules;
    }
}
