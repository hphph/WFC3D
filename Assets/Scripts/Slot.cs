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
	public List<Module> Possibilities => possibilities;

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
    public bool Spread(Slot neighbour, WFCTools.DirectionIndex connectorIndexToNeighbour)
	{
		HashSet<Module> newPossibilities = new HashSet<Module>();
		if(neighbour.IsCollapsed)
		{
			ReduceExcludedPossibilities(neighbour, connectorIndexToNeighbour);
			for(int i = 0; i < possibilities.Count(); i++)
			{
				if(possibilities[i].IsFitting(neighbour.CollapsedModule, (int)connectorIndexToNeighbour))
				{
					newPossibilities.Add(possibilities[i]);
				}
			}
		}
		else
		{
			foreach(var np in neighbour.possibilities)
			{
				for(int i = 0; i < possibilities.Count(); i++)
				{
					if(possibilities[i].IsFitting(np, (int)connectorIndexToNeighbour))
					{
						newPossibilities.Add(possibilities[i]);
					}
				}
			}
		}
		bool hasChanged = possibilities.Count() != newPossibilities.Count();
		possibilities = newPossibilities.ToList();
		return hasChanged;
	}

	public void ReduceExcludedPossibilities(Slot neighbour, WFCTools.DirectionIndex connectorIndexToNeighbour)
	{
		if(!neighbour.IsCollapsed) return;
		int removed;
		removed = possibilities.RemoveAll(possibility => neighbour.CollapsedModule.IsModuleExcluded(possibility, connectorIndexToNeighbour));		
	}

	public void Collapse()
	{
		if(possibilities.Count == 0) Debug.Log("No Possibilities to collapse");
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
	}

}
