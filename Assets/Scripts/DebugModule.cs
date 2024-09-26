using System;
using System.Collections.Generic;
using UnityEngine;

public class DebugModule : MonoBehaviour
{
    [Serializable]
    public class ModuleWithRotation
    {
        GameObject prefab;
        string name;
        int rotation;

        public ModuleWithRotation(GameObject prefab, string name, int rotation)
        {
            this.prefab = prefab;
            this.name = name;
            this.rotation = rotation;
        }

    }

    public List<ModuleWithRotation> modules;

    public void SendPossibilities(IEnumerable<Module> possibilities)
    {
        modules = new List<ModuleWithRotation>();
        foreach(Module p in possibilities)
        {
            modules.Add(new ModuleWithRotation(p.Prefab, p.Prefab.name, p.Rotation));
        }
    }
}
