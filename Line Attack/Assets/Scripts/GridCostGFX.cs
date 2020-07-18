using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCostGFX : MonoBehaviour
{
	[SerializeField] UnitStamp unitStamp;

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.GetComponent<GridCostGFX>())
		{
			unitStamp.AddToGridCostGFXIsColidingwith(collision.gameObject.GetComponent<GridCostGFX>());
		}
	}

	private void OnCollisionExit(Collision collision)
	{
		if (collision.gameObject.GetComponent<GridCostGFX>())
		{
			unitStamp.RemoveFromGridCostGFXIsColidingwith(collision.gameObject.GetComponent<GridCostGFX>());
		}
	}
}
