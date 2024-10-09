using System;
using System.Collections.Generic;
using UnityEngine;

public class DebugSlot : MonoBehaviour
{
    [Serializable]
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
    Slot observedSlot;
    public List<DebugModule> Possibilities;

    public void SetObservedSlot(Slot toObserve)
    {
        observedSlot = toObserve;
        gameObject.name = observedSlot.Position + "Debug Slot";
        Possibilities = new List<DebugModule>(observedSlot.Possibilities.Count);
        UpdatePossibilities();
    }

    void UpdatePossibilities()
    {
        Possibilities.Clear();
        foreach(Module m in observedSlot.Possibilities)
        {
            Possibilities.Add(new DebugModule(m));
        }
    }

    void Update()
    {
        if(observedSlot.Possibilities.Count < Possibilities.Count)
        {
            UpdatePossibilities();
        }
    }
}
