using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class Slot
{
    Module collapsedModule;
    List<Module> possibilities;
	Vector3Int position;

    public bool IsCollapsed => collapsedModule != null;
    public Module CollapsedModule => collapsedModule;
	public Vector3Int Position => position;

	public float Entropy() 
	{
		float result = 0;
		foreach(Module p in possibilities)
		{
			result += p.Probability;
		}
		return result;
	}
    public Slot(Vector3Int position, IEnumerable<Module> possibilites)
	{
		this.position = position;
		this.possibilities = new List<Module>(possibilites);
	}
	
	/// <summary>
	/// Removes possibilities that not align with given neighbour.
	/// Retruns true if number of possibilities have dropped.
	/// </summary>
    public bool Spread(Module neighbouringModule, WFCTools.DirectionIndex connectorIndexToNeighbour)
	{
		List<Module> newPossibilities = new List<Module>();
		for(int i = 0; i < possibilities.Count(); i++)
		{
			if(possibilities[i].IsFitting(neighbouringModule, (int)connectorIndexToNeighbour))
			{
				newPossibilities.Add(possibilities[i]);
			}
		}
		bool hasChanged = possibilities.Count() != newPossibilities.Count();
		possibilities = newPossibilities;
		return hasChanged;
	}

	public void Collapse()
	{
		float collapseNumber = Random.Range(0, Entropy());
		float sum = 0;
		foreach(Module p in possibilities)
		{
			sum += p.Probability;
			if(collapseNumber <= sum) 
			{
				collapsedModule = p;
				break;
			}
		}
		//Rand get one element from possibilities
	}

}
