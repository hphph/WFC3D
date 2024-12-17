using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ModuleSocket
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
    public ModuleSocket(Vector3Int position, IEnumerable<Module> possibilites)
	{
		this.position = position;
		this.possibilities = new List<Module>(possibilites);
	}

	public ModuleSocket(Vector3Int position, IEnumerable<Module> possibilites, Module collapsedModule)
	{
		this.position = position;
		this.possibilities = new List<Module>(possibilites);
		this.collapsedModule = collapsedModule;
	}
	
	/// <summary>
	/// direction points from this socket to neighbour
	/// Removes possibilities that not align with given neighbour.
	/// Retruns true if number of possibilities have dropped.
	/// </summary>
    public bool Spread(ModuleSocket neighbour, WFCTools.DirectionIndex directionToNeighbourFromThis)
	{
		HashSet<Module> newPossibilities = new HashSet<Module>();
		if(neighbour.IsCollapsed)
		{
			ReduceExcludedPossibilities(neighbour, directionToNeighbourFromThis);
			for(int i = 0; i < possibilities.Count(); i++)
			{
				if(possibilities[i].IsFitting(neighbour.CollapsedModule, (int)directionToNeighbourFromThis))
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
					if(possibilities[i].IsFitting(np, (int)directionToNeighbourFromThis))
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

	/// <summary>
	/// direction points from this socket to neighbour
	/// Removes possibilities that not align with given neighbour.
	/// Retruns true if number of possibilities have dropped.
	/// </summary>
	public bool Spread(Module neighbour, WFCTools.DirectionIndex directionToNeighbourFromThis)
	{
		HashSet<Module> newPossibilities = new HashSet<Module>();
		for(int i = 0; i < possibilities.Count(); i++)
		{
			if(possibilities[i].IsFitting(neighbour, (int)directionToNeighbourFromThis))
			{
				newPossibilities.Add(possibilities[i]);
			}
		}
		
		bool hasChanged = possibilities.Count() != newPossibilities.Count();
		possibilities = newPossibilities.ToList();
		return hasChanged;
	}

	public void ReduceExcludedPossibilities(ModuleSocket neighbour, WFCTools.DirectionIndex connectorIndexToNeighbour)
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
