using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Dummy Module", menuName = "WFC/DummyModule", order = 1)]
public class DummyModuleSO : ScriptableObject
{
    public DummyModule moduleData;

    public void SetModuleData(DummyModule data)
    {
        moduleData = data;
    }
    
}
