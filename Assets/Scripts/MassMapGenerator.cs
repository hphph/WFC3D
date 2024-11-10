using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public class MassMapGenerator : MonoBehaviour
{
    [SerializeField] FiniteMap map;
    [SerializeField] Vector3Int numberOfMaps;
    [SerializeField] Vector3 spaceBetween;

    public void GenerateStartCorutine()
    {
        StartCoroutine(GenerateMapsCoroutine());
    }

    private IEnumerator GenerateMapsCoroutine()
    {
        float timeToGenerate = Time.realtimeSinceStartup;
        int generated = 0;
        for(int i = 0; i < numberOfMaps.x; i++)
        {
        for(int j = 0; j < numberOfMaps.y; j++)
        {
        for(int k = 0; k < numberOfMaps.z; k++)
        {
            FiniteMap newMap = Instantiate(map, new Vector3(spaceBetween.x * i, spaceBetween.y * j, spaceBetween.z * k), Quaternion.identity);
            bool hasCollapsed = true;
            while(hasCollapsed)
            {
                hasCollapsed = newMap.CollapseLowestEntropyModuleSocketAndPropagateChange();
                yield return null;
            }
            generated++;
            Debug.Log("Generated " + (float)generated/(numberOfMaps.x*numberOfMaps.y*numberOfMaps.z) + "%");
            yield return null;
        }
        }
        }
        timeToGenerate = Time.realtimeSinceStartup - timeToGenerate;
        Debug.Log("Generation of maps " + numberOfMaps.x*numberOfMaps.y*numberOfMaps.z + " took " + timeToGenerate +"s.");
        Debug.Log("Average for map " + timeToGenerate/(numberOfMaps.x*numberOfMaps.y*numberOfMaps.z) + "s.");
        if(!File.Exists(Application.dataPath + "/measureData.log"))
        {
            using(StreamWriter sw = File.CreateText(Application.dataPath + "/measureData.log"))
            {
                sw.WriteLine("Date\tMapsNum\tGenTime\tAvgGenTime");
                sw.WriteLine(DateTime.Now.ToString(new CultureInfo("pl-PL")) + "\t" + numberOfMaps.x*numberOfMaps.y*numberOfMaps.z + "\t" + timeToGenerate + "\t" + timeToGenerate/(numberOfMaps.x*numberOfMaps.y*numberOfMaps.z));
            }
        }
        else
        {
            using(StreamWriter sw = File.AppendText(Application.dataPath + "/measureData.log"))
            {
                sw.WriteLine(DateTime.Now.ToString(new CultureInfo("pl-PL")) + "\t" + numberOfMaps.x*numberOfMaps.y*numberOfMaps.z + "\t" + timeToGenerate + "\t" + timeToGenerate/(numberOfMaps.x*numberOfMaps.y*numberOfMaps.z));
            }
        }
    }

    public void GenerateMaps()
    {
        float timeToGenerate = Time.realtimeSinceStartup;
        int generated = 0;
        for(int i = 0; i < numberOfMaps.x; i++)
        {
        for(int j = 0; j < numberOfMaps.y; j++)
        {
        for(int k = 0; k < numberOfMaps.z; k++)
        {
            FiniteMap newMap = Instantiate(map, new Vector3(spaceBetween.x * i, spaceBetween.y * j, spaceBetween.z * k), Quaternion.identity);
            bool hasCollapsed = true;
            while(hasCollapsed)
            {
                hasCollapsed = newMap.CollapseLowestEntropyModuleSocketAndPropagateChange();
            }
            generated++;
            Debug.Log("Generated " + (float)generated/(numberOfMaps.x*numberOfMaps.y*numberOfMaps.z) + "%");
        }
        }
        }
        timeToGenerate = Time.realtimeSinceStartup - timeToGenerate;
        Debug.Log("Generation of maps " + numberOfMaps.x*numberOfMaps.y*numberOfMaps.z + " took " + timeToGenerate +"s.");
        Debug.Log("Average for map " + timeToGenerate/(numberOfMaps.x*numberOfMaps.y*numberOfMaps.z) + "s.");
        if(!File.Exists(Application.dataPath + "/measureData.log"))
        {
            using(StreamWriter sw = File.CreateText(Application.dataPath + "/measureData.log"))
            {
                sw.WriteLine("Date\tMapsNum\tGenTime\tAvgGenTime");
                sw.WriteLine(DateTime.Now.ToString(new CultureInfo("pl-PL")) + "\t" + numberOfMaps.x*numberOfMaps.y*numberOfMaps.z + "\t" + timeToGenerate + "\t" + timeToGenerate/(numberOfMaps.x*numberOfMaps.y*numberOfMaps.z));
            }
        }
        else
        {
            using(StreamWriter sw = File.AppendText(Application.dataPath + "/measureData.log"))
            {
                sw.WriteLine(DateTime.Now.ToString(new CultureInfo("pl-PL")) + "\t" + numberOfMaps.x*numberOfMaps.y*numberOfMaps.z + "\t" + timeToGenerate + "\t" + timeToGenerate/(numberOfMaps.x*numberOfMaps.y*numberOfMaps.z));
            }
        }
    }
}
