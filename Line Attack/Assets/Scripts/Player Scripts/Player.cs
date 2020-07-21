using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : Actor
{
	public static Player player;

	private PlayerUIManager playerUIManager;
	private GameObject currentStampPrefab;

	[Header("Interaction Veribles")]
	[SerializeField] private LayerMask raycastLayer;
	[SerializeField] private LayerMask cameraDefaultCullingLayer;
	[SerializeField] private LayerMask gridCostGFXCulltingLayers;

	[SerializeField] private UnitStamp currentlySelectedStamp;

	#region Getters
	public PlayerUIManager GetPlayerUIManager() { return playerUIManager; }


	#endregion

	#region Setters
	protected override void Setup()
	{
		playerUIManager = GetComponent<PlayerUIManager>();
		playerUIManager.Updateplayercurrnecy(currentCurrency);
		ClearSelectedStamp();

		base.Setup();
	}

	#endregion

	public override void ChangePlayerState(PlayerState newPlayerState)
	{
		currentPlayerState = newPlayerState;

		if (currentStampPrefab != null)
			Destroy(currentStampPrefab);

		switch (currentPlayerState)
		{
			case PlayerState.Idle:
				StopAllCoroutines();
				Camera.main.cullingMask = cameraDefaultCullingLayer;
				break;
			case PlayerState.Paused:
				Camera.main.cullingMask = cameraDefaultCullingLayer;
				break;
			case PlayerState.BuildingStamp:
				ClearSelectedStamp();
				Camera.main.cullingMask = gridCostGFXCulltingLayers;
				break;
		}
	}

	private void Awake()
	{
		player = this;
	}

	private void Update()
	{
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		Physics.Raycast(ray, out hit, 40f, raycastLayer);

		if (currentPlayerState == PlayerState.BuildingStamp && currentStampPrefab != null && hit.collider != null)
		{
			if (currentStampPrefab.GetComponent<UnitStamp>().GetUnitCost() <= currentCurrency)
			{
				if (hit.collider.GetComponent<UnitStamp>() != true)
				{
					Vector3 placePos = new Vector3((int)hit.point.x + 0.5f, hit.point.y, (int)hit.point.z - 0.5f);
					currentStampPrefab.transform.position = placePos;

					if (Input.GetMouseButtonDown(0))
					{
						if (currentStampPrefab.GetComponent<UnitStamp>().IsSafeToPlace())
						{
							PlaceStamp();
						}
					}
				}
			}
			else
			{
				ChangePlayerState(PlayerState.Idle);
			}


			if (Input.GetMouseButtonDown(1))
			{
				ChangePlayerState(PlayerState.Idle);
			}
		}

		if (currentPlayerState == PlayerState.Idle)
		{
			if (currentlySelectedStamp)
				playerUIManager.UpdateFunctionalityWheelPos(currentlySelectedStamp.transform.position);

			if (Input.GetMouseButtonDown(0))
			{
				if (hit.collider != null)
				{
					if (hit.collider.GetComponent<UnitStamp>() && EventSystem.current.IsPointerOverGameObject() == false)
					{
						SelectNewStamp(hit.collider.GetComponent<UnitStamp>());
					}
				}
			}

			if (Input.GetMouseButtonDown(1))
			{
				ClearSelectedStamp();
			}
		}
	}

	private void SelectNewStamp(UnitStamp us)
	{
		currentlySelectedStamp = us;
		playerUIManager.SetFunctionalitywheelActiveState(true);
	}

	private void PlaceStamp()
	{
		UnitStamp us = Instantiate(currentStampPrefab, currentStampPrefab.transform.position, currentStampPrefab.transform.rotation, stampHolder).GetComponent<UnitStamp>();
		us.GetComponent<Collider>().enabled = true;

		activeStamps.Add(us);

		currentCurrency -= currentStampPrefab.GetComponent<UnitStamp>().GetUnitCost();
		playerUIManager.Updateplayercurrnecy(currentCurrency);

		if (currentStampPrefab.GetComponent<UnitStamp>().GetUnitCost() >= currentCurrency)
		{
			ChangePlayerState(PlayerState.Idle);
		}
	}

	private void ClearSelectedStamp()
	{
		playerUIManager.SetFunctionalitywheelActiveState(false);
		playerUIManager.UpdateFunctionalityWheelPos(Vector3.zero);
		currentlySelectedStamp = null;
	}

	public void SellStampButtonPressed()
	{
		activeStamps.Remove(currentlySelectedStamp);
		currentCurrency += currentlySelectedStamp.GetComponent<UnitStamp>().GetUnitCost();
		Destroy(currentlySelectedStamp.gameObject);

		ClearSelectedStamp();
	}

	public void BuildUnitStampButtonPressed(GameObject stampPrefab)
	{
		if(currentStampPrefab != null)
			Destroy(currentStampPrefab);

		if(currentPlayerState != PlayerState.BuildingStamp) 
			ChangePlayerState(PlayerState.BuildingStamp);

		currentStampPrefab = Instantiate(stampPrefab);
	}

	public override void ReciveResources(float _R)
	{
		base.ReciveResources(_R);
		playerUIManager.Updateplayercurrnecy(currentCurrency);
	}

}
