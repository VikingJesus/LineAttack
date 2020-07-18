using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitStamp : MonoBehaviour
{
	[SerializeField] Unit attachedUnit;

	[Header("Grid Properties")]
	[SerializeField] float unitCost = 10;

	[SerializeField] int gridCost = 1;
	[SerializeField] GridCostGFX gridCostGFX;

	[SerializeField] List<GridCostGFX> gridCostGFXIsColidingwith = new List<GridCostGFX>();

	public void AddToGridCostGFXIsColidingwith(GridCostGFX gridCostGFX)
	{
		gridCostGFXIsColidingwith.Add(gridCostGFX);
	}
	public void RemoveFromGridCostGFXIsColidingwith(GridCostGFX gridCostGFX)
	{
		gridCostGFXIsColidingwith.Remove(gridCostGFX);
	}

	public bool IsSafeToPlace()
	{ 
		if(gridCostGFXIsColidingwith.Count > 0)
		{
			return false;	
		}

		return true;
	}

	private void Start()
	{
		GetComponent<BoxCollider>().size = new Vector3(gridCost + 0.5f, 1, gridCost + 0.5f);
		gridCostGFX.transform.localScale = new Vector3(gridCost - 0.1f, gridCostGFX.transform.localScale.y, gridCost - 0.1f);
	}

	public float GetUnitCost()
	{
		return unitCost;
	}
	
	public GridCostGFX GetGridCostGFX()
	{
		return gridCostGFX;
	}

	public Unit GetAttachedUnit()
	{
		return attachedUnit;
	}


}
