using System;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;

[CreateAssetMenu(fileName = "Module Data", menuName = "WFC/ModuleData", order = 1)]
public class ModuleData: ScriptableObject
{
    public GameObject dummyModulesPrefab;
    public Module[] Modules;

    public void GenerateModulesDataFromPrefab()
    {
        //TODO: make less modules based on dummy connections

        List<Module> currentModules = new List<Module>();
        int modulesCount = 0;
        foreach(Transform child in dummyModulesPrefab.transform)
        {
            DummyModule dm = child.GetComponent<DummyModule>();

            foreach(DummyModule.VerticalConnector vc in dm.VerticalConnectors)
            {
                //return 1
                if(vc.rotation != DummyModule.VerticalConnector.RotationState.Invariant)
                {
                    // return 4
                    break;
                }
            }

            foreach(DummyModule.HorizontalConnector hc in dm.HorizontalConnectors)
            {
                // do zrobienia
            }

            currentModules.Add(new Module(child.gameObject, 0, 4, modulesCount));
            currentModules.Add(new Module(child.gameObject, 1, 4, modulesCount + 1));
            currentModules.Add(new Module(child.gameObject, 2, 4, modulesCount + 2));
            currentModules.Add(new Module(child.gameObject, 3, 4, modulesCount + 3));
            modulesCount += 4;
        }
        Modules = currentModules.ToArray();
        Array.ForEach(Modules, m => m.FillNeighbourPosibilities(Modules));
        EditorUtility.SetDirty(this);
    }
}