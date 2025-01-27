using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ModuleSocket
{
    Module collapsedModule;
    HashSet<int> possibilities;
	Vector3Int position;
	GameObject socketGO;
	ModulesData modulesData;

    public bool IsCollapsed => collapsedModule != null;
    public Module CollapsedModule => collapsedModule;
	public Vector3Int Position => position;
	public HashSet<int> Possibilities => possibilities;
	public GameObject SocketGO => socketGO;
	public ModulesData ModuleData => modulesData;

	public float Entropy() 
	{
		float result = 0;
		foreach(int p in possibilities)
		{
			result += modulesData.Modules[p].Probability;
		}
		return result;
	}

    public ModuleSocket(Vector3Int position, IEnumerable<int> possibilites, ModulesData moduleData)
	{
		this.position = position;
		this.possibilities = new HashSet<int>(possibilites);
		this.modulesData = moduleData;
	}

	public ModuleSocket(Vector3Int position, IEnumerable<int> possibilites, Module collapsedModule, GameObject socketGO, ModulesData moduleData)
	{
		this.position = position;
		this.possibilities = new HashSet<int>(possibilites);
		this.collapsedModule = collapsedModule;
		this.socketGO = socketGO;
		this.modulesData = moduleData;
	}

	public void SetSocketGO(GameObject socketGO)
	{
		this.socketGO = socketGO;
	}

    public bool Spread(ModuleSocket neighbour, WFCTools.DirectionIndex directionToNeighbourFromThis)
	{
		int initialPossibilitiesCount = possibilities.Count;
		if(neighbour.IsCollapsed)
		{
			Module colMod = neighbour.CollapsedModule;
			var mNP = colMod.NeighbourPosibilities[(int)directionToNeighbourFromThis];
			possibilities.IntersectWith(mNP);
		}
		else
		{
			HashSet<int> newPossibilities = new HashSet<int>();
			foreach(int np in neighbour.possibilities)
			{
				Module npM = modulesData.Modules[np];
				var npNP = npM.NeighbourPosibilities[(int)directionToNeighbourFromThis];
				newPossibilities.UnionWith(possibilities.Intersect(npNP));
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
		removed = possibilities.RemoveWhere(possibility => neighbour.CollapsedModule.IsModuleExcluded(modulesData.Modules[possibility], connectorIndexToNeighbour));		
	}

	public void Collapse()
	{
		float collapseNumber = Random.Range(0, Entropy());
		float sum = 0;
		foreach(int p in possibilities)
		{
			sum += modulesData.Modules[p].Probability;
			if(collapseNumber <= sum) 
			{
				collapsedModule = modulesData.Modules[p];
				break;
			}
		}
	}

}
