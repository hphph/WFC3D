using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ModuleSocket
{
    Module collapsedModule;
    HashSet<int> possibilities;
	Vector3Int position;
	GameObject socketGO;
	ModuleData moduleData;

    public bool IsCollapsed => collapsedModule != null;
    public Module CollapsedModule => collapsedModule;
	public Vector3Int Position => position;
	public HashSet<int> Possibilities => possibilities;
	public GameObject SocketGO => socketGO;
	public ModuleData ModuleData => moduleData;

	public float Entropy() 
	{
		float result = 0;
		foreach(int p in possibilities)
		{
			result += moduleData.Modules[p].Probability;
		}
		return result;
	}

    public ModuleSocket(Vector3Int position, IEnumerable<int> possibilites, ModuleData moduleData)
	{
		this.position = position;
		this.possibilities = new HashSet<int>(possibilites);
		this.moduleData = moduleData;
	}

	public ModuleSocket(Vector3Int position, IEnumerable<int> possibilites, Module collapsedModule, GameObject socketGO, ModuleData moduleData)
	{
		this.position = position;
		this.possibilities = new HashSet<int>(possibilites);
		this.collapsedModule = collapsedModule;
		this.socketGO = socketGO;
		this.moduleData = moduleData;
	}

	public void SetSocketGO(GameObject socketGO)
	{
		this.socketGO = socketGO;
	}
	
	/// <summary>
	/// direction points from this socket to neighbour
	/// Removes possibilities that not align with given neighbour.
	/// Retruns true if number of possibilities have dropped.
	/// </summary>
    // public bool Spread(ModuleSocket neighbour, WFCTools.DirectionIndex directionToNeighbourFromThis)
	// {
	// 	HashSet<Module> newPossibilities = new HashSet<Module>();
	// 	if(neighbour.IsCollapsed)
	// 	{
	// 		ReduceExcludedPossibilities(neighbour, directionToNeighbourFromThis);
	// 		for(int i = 0; i < possibilities.Count(); i++)
	// 		{
	// 			if(possibilities[i].IsFitting(neighbour.CollapsedModule, (int)directionToNeighbourFromThis))
	// 			{
	// 				newPossibilities.Add(possibilities[i]);
	// 			}
	// 		}
	// 	}
	// 	else
	// 	{
	// 		foreach(var np in neighbour.possibilities)
	// 		{
	// 			for(int i = 0; i < possibilities.Count(); i++)
	// 			{
	// 				if(possibilities[i].IsFitting(np, (int)directionToNeighbourFromThis))
	// 				{
	// 					newPossibilities.Add(possibilities[i]);
	// 				}
	// 			}
	// 		}
	// 	}
	// 	bool hasChanged = possibilities.Count() != newPossibilities.Count();
	// 	possibilities = newPossibilities.ToList();
	// 	return hasChanged;
	// }

    public bool Spread(ModuleSocket neighbour, WFCTools.DirectionIndex directionToNeighbourFromThis)
	{
		int initialPossibilitiesCount = possibilities.Count;
		if(neighbour.IsCollapsed)
		{
			// ReduceExcludedPossibilities(neighbour, directionToNeighbourFromThis);
			possibilities.IntersectWith(neighbour.CollapsedModule.NeighbourPosibilities[(int)directionToNeighbourFromThis]);
		}
		else
		{
			HashSet<int> newPossibilities = new HashSet<int>();
			foreach(int np in neighbour.possibilities)
			{
				newPossibilities.UnionWith(possibilities.Intersect(moduleData.Modules[np].NeighbourPosibilities[(int)directionToNeighbourFromThis]));
			}
			if(newPossibilities.Count != initialPossibilitiesCount) 
			{
				possibilities = newPossibilities;
				return true;
			}
			return false;
		}
		if(possibilities.Count != initialPossibilitiesCount) return true;
		return false;
	}

	public void ResetSocket(IEnumerable<int> possibilites)
	{
		this.possibilities = new HashSet<int>(possibilites);
		collapsedModule = null;
		socketGO = null;
	}

	/// <summary>
	/// direction points from this socket to neighbour
	/// Removes possibilities that not align with given neighbour.
	/// Retruns true if number of possibilities have dropped.
	/// </summary>
	// public bool Spread(Module neighbour, WFCTools.DirectionIndex directionToNeighbourFromThis)
	// {
	// 	HashSet<Module> newPossibilities = new HashSet<Module>();
	// 	for(int i = 0; i < possibilities.Count(); i++)
	// 	{
	// 		if(possibilities[i].IsFitting(neighbour, (int)directionToNeighbourFromThis))
	// 		{
	// 			newPossibilities.Add(possibilities[i]);
	// 		}
	// 	}
		
	// 	bool hasChanged = possibilities.Count() != newPossibilities.Count();
	// 	possibilities = newPossibilities.ToList();
	// 	return hasChanged;
	// }

	public bool Spread(Module neighbour, WFCTools.DirectionIndex directionToNeighbourFromThis)
	{
		int initialPossibilitiesCount = possibilities.Count;
		possibilities.IntersectWith(neighbour.NeighbourPosibilities[(int)directionToNeighbourFromThis]);
		if(possibilities.Count != initialPossibilitiesCount) return true;
		return false;
	}

	public void ReduceExcludedPossibilities(ModuleSocket neighbour, WFCTools.DirectionIndex connectorIndexToNeighbour)
	{
		if(!neighbour.IsCollapsed) return;
		int removed;
		removed = possibilities.RemoveWhere(possibility => neighbour.CollapsedModule.IsModuleExcluded(moduleData.Modules[possibility], connectorIndexToNeighbour));		
	}

	public void Collapse()
	{
		if(possibilities.Count == 0) Debug.Log("No Possibilities to collapse");
		float collapseNumber = Random.Range(0, Entropy());
		float sum = 0;
		foreach(int p in possibilities)
		{
			sum += moduleData.Modules[p].Probability;
			if(collapseNumber <= sum) 
			{
				collapsedModule = moduleData.Modules[p];
				break;
			}
		}
	}

}
