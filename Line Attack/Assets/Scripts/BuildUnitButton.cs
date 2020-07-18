using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildUnitButton : MonoBehaviour
{
    
    [SerializeField] GameObject unitStampPrefab;
    [SerializeField] Text unitCostText;

    float unitCost;

    public float  GetUnitStampCost()
    {
        return unitCost;
    }

	private void Start()
	{
        unitCost = unitStampPrefab.GetComponent<UnitStamp>().GetUnitCost();
        unitCostText.text = unitCost.ToString();
    }

	public void CallPlayerBuildStamp()
    {
        Player.player.BuildUnitStampButtonPressed(unitStampPrefab);
    }

}
