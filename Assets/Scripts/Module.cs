using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Module
{
    [System.Serializable]
    public class NeigbourIndexArray
    {
        public int[] neighbourIndexes;
    }
    
    [SerializeField] DummyModule dummy;
    [SerializeField] GameObject prefab;
    [SerializeField] int rotation;
    [SerializeField] int modulesFromDummy;
    [SerializeField] string[] tags;
    [SerializeField] int index;
    public NeigbourIndexArray[] neighbourPosibilities;

    public DummyModule Dummy => dummy;
    public HashSet<int>[] NeighbourPosibilities;
    public GameObject Prefab => prefab;
    public int Rotation => rotation;
    public float Probability => dummy.Probability/modulesFromDummy;
    public string[] Tags => tags;
    public int Index => index;
    
    public Module(GameObject dummyPrefab, int rotation, int modulesFromDummy, int index)
    {
        neighbourPosibilities = new NeigbourIndexArray[6];
        dummy = dummyPrefab.GetComponent<DummyModule>();
        if(dummy == null) Debug.LogError("Missing DummyModule component in prefab");
        prefab = dummyPrefab;
        this.rotation = rotation;
        if(rotation > 3) Debug.LogError("Wrong rotation value");
        this.modulesFromDummy = modulesFromDummy;
        this.index = index;
        tags = dummy.Tags;
    }

    public bool IsFitting(Module neighbour, int connectorIndexToNeighbour)
    {
        if(WFCTools.IsConnectorHorizontal(connectorIndexToNeighbour))
        {
            var connector1 = neighbour.dummy.ModuleConnectors[RotateHorizontallyConnector(WFCTools.OppositeConnectorIndex(connectorIndexToNeighbour), neighbour.Rotation)] as DummyModule.HorizontalConnector;
            var connector2 = dummy.ModuleConnectors[RotateHorizontallyConnector(connectorIndexToNeighbour, rotation)] as DummyModule.HorizontalConnector;
            return (connector1.ConnectionId == connector2.ConnectionId) && 
                    (connector1.Symmetric || connector2.Symmetric || (!connector1.Flipped == connector2.Flipped));
        }
        else
        {
            var connector1 = neighbour.Dummy.OppositeConnector(connectorIndexToNeighbour) as DummyModule.VerticalConnector;
            var connector2 = dummy.ModuleConnectors[connectorIndexToNeighbour] as DummyModule.VerticalConnector;
            return (connector1.ConnectionId == connector2.ConnectionId) && 
                    (connector1.Rotation == DummyModule.VerticalConnector.RotationState.Invariant || 
                        connector2.Rotation == DummyModule.VerticalConnector.RotationState.Invariant || 
                        (((int)connector1.Rotation + neighbour.Rotation) % 4) == (((int)connector2.Rotation + rotation) % 4));
        }
    }

    public bool IsModuleExcluded(Module moduleToCheck, WFCTools.DirectionIndex connectorIndexOppositeToModuleToCheck)
    {
        if(WFCTools.IsConnectorHorizontal((int)connectorIndexOppositeToModuleToCheck))
        {
            var connector = dummy.ModuleConnectors[RotateHorizontallyConnector(WFCTools.OppositeConnectorIndex((int)connectorIndexOppositeToModuleToCheck), rotation)];
            foreach(DummyModule excluded in connector.ExcludedDummyModules)
            {
                if(excluded.GetComponent<MeshFilter>().sharedMesh == moduleToCheck.Dummy.GetComponent<MeshFilter>().sharedMesh)
                {
                    return true;
                }
            }
        }
        else
        {
            var connector = dummy.ModuleConnectors[WFCTools.OppositeConnectorIndex((int)connectorIndexOppositeToModuleToCheck)];
            foreach(DummyModule excluded in connector.ExcludedDummyModules)
            {
                if(excluded.GetComponent<MeshFilter>().sharedMesh == moduleToCheck.Dummy.GetComponent<MeshFilter>().sharedMesh)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void FillNeighbourPosibilities(Module[] modules)
    {
        for(int i = 0; i<6; i++)
        {
            neighbourPosibilities[i] = new NeigbourIndexArray();
            neighbourPosibilities[i].neighbourIndexes = modules.Where(m => m.IsFitting(this, i) && !this.IsModuleExcluded(m, (WFCTools.DirectionIndex)i)).Select(m => m.Index).ToArray();
        }
    }

    static public int[] HorizontalIndexes()
    {
        return new int[]{0, 1, 3, 4};
    }

    static public int RotateHorizontallyConnector(int connectorIndex, int rotation)
    {
        return HorizontalIndexes()[WFCTools.Mod(System.Array.IndexOf<int>(HorizontalIndexes(), connectorIndex) - rotation, 4)];
    }

    public void FillHashSet()
    {
        NeighbourPosibilities = neighbourPosibilities.Select(p => p.neighbourIndexes.ToHashSet()).ToArray();
    }
}
