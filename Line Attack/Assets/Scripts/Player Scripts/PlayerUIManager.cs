using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUIManager : MonoBehaviour
{
	[SerializeField] TextMeshProUGUI playerCurrnecy;
	[SerializeField] List<BuildUnitButton> buildUnitButtons = new List<BuildUnitButton>();

	[SerializeField] TextMeshProUGUI currentSecondsLeftBeforNextWaveText;

	[SerializeField] GameObject errorEffectsGFX;
	[SerializeField] GameObject functionalitywheel;
	public void Updateplayercurrnecy(float newVal)
	{
		playerCurrnecy.text = newVal.ToString();
		CheckIfPlayerCanAfforUnits(newVal);
	}

	public void UpdateFunctionalityWheelPos(Vector3 pos)
	{
		functionalitywheel.transform.position = pos;
	}

	public void SetFunctionalitywheelActiveState(bool state)
	{
		functionalitywheel.SetActive(state);
	}

	public void UpdateTimeLeftBeforNextWave(float currentSecondsLeft)
	{
		currentSecondsLeftBeforNextWaveText.text = currentSecondsLeft.ToString();
	}

	public void CheckIfPlayerCanAfforUnits(float currentPlayerCurrnecy)
	{
		for (int i = 0; i < buildUnitButtons.Count; i++)
		{
			if (buildUnitButtons[i].GetUnitStampCost() <= currentPlayerCurrnecy)
			{
				buildUnitButtons[i].GetComponent<Button>().interactable = true;
			}
			else
				buildUnitButtons[i].GetComponent<Button>().interactable = false;
		}
	}

	public void EnableErrorEffect()
	{

		errorEffectsGFX.SetActive(true);
		Invoke("DisableErrorEffect", 0.2f);
	}

	public void DisableErrorEffect()
	{

		errorEffectsGFX.SetActive(false);
	}

}
