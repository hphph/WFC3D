using System.Collections.Generic;
using UnityEngine;

public class DebugSlot : MonoBehaviour
{
    [System.Serializable]
    public class DebugModule
    {
        public GameObject prefab;
        public int rotation;
        public float probability;
        public DebugModule(Module origin)
        {
            prefab = origin.Prefab;
            rotation = origin.Rotation;
            probability = origin.Probability;
        }
    }
    ModuleSocket observedSlot;
    public List<DebugModule> Possibilities;

    public void SetObservedSlot(ModuleSocket toObserve)
    {
        observedSlot = toObserve;
        gameObject.name = observedSlot.Position + "Debug Slot";
        Possibilities = new List<DebugModule>(observedSlot.Possibilities.Count);
        UpdatePossibilities();
    }

    void UpdatePossibilities()
    {
        Possibilities.Clear();
        foreach(int p in observedSlot.Possibilities)
        {
            Possibilities.Add(new DebugModule(observedSlot.ModuleData.Modules[p]));
        }
    }
}
