using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
	public static Player player;
	int team = 1;
	private enum PlayerState { Idle, Paused, BuildingStamp}
	[SerializeField] private PlayerState currentPlayerState;

	private PlayerUIManager playerUIManager;
	private GameObject currentStampPrefab;

	[Header("Wave Veribles")]
	[SerializeField] private WaveManager waveMangerPrefab;
	[SerializeField] private WaveManager currentWaveManagerObject;
	[SerializeField] private Transform waveMangerSpawnPoint;

	[SerializeField] List<UnitStamp> activeStamps = new List<UnitStamp>();

	[Header("Interaction Veribles")]
	[SerializeField] private LayerMask raycastLayer;
	[SerializeField] private LayerMask cameraDefaultCullingLayer;
	[SerializeField] private LayerMask gridCostGFXCulltingLayers;

	[SerializeField] private UnitStamp currentlySelectedStamp;

	[Header("Player Stats")]
	[SerializeField] private float currentCurrency = 1000;

	public PlayerUIManager GetPlayerUIManager() { return playerUIManager; }
	public int GetTeam() { return team; }

	private void ChangePlayerState(PlayerState newPlayerState)
	{
		currentPlayerState = newPlayerState;

		if (currentStampPrefab != null)
			Destroy(currentStampPrefab);

		switch (currentPlayerState)
		{
			case PlayerState.Idle:
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

	void Awake()
	{
		player = this;
	}

	void Start()
	{
		GameManager.gameManager.onStartWave += OnNextWave;

		playerUIManager = GetComponent<PlayerUIManager>();
		playerUIManager.Updateplayercurrnecy(currentCurrency);

		SpawnNextWaveManager();
		ClearSelectedStamp();
	}

	void Update()
	{
		if (EventSystem.current.IsPointerOverGameObject())
		{
			Debug.Log("Over Gameobject");
		}

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
						else
						{
							playerUIManager.EnableErrorEffect();
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

	void SelectNewStamp(UnitStamp us)
	{
		currentlySelectedStamp = us;
		playerUIManager.SetFunctionalitywheelActiveState(true);
	}

	void ClearSelectedStamp()
	{
		playerUIManager.SetFunctionalitywheelActiveState(false);
		playerUIManager.UpdateFunctionalityWheelPos(Vector3.zero);
		currentlySelectedStamp = null;
	}

	void PlaceStamp()
	{
		UnitStamp us = Instantiate(currentStampPrefab, currentStampPrefab.transform.position, currentStampPrefab.transform.rotation, null).GetComponent<UnitStamp>();
		us.GetComponent<Collider>().enabled = true;

		activeStamps.Add(us);

		currentCurrency -= currentStampPrefab.GetComponent<UnitStamp>().GetUnitCost();
		playerUIManager.Updateplayercurrnecy(currentCurrency);

		if (currentStampPrefab.GetComponent<UnitStamp>().GetUnitCost() >= currentCurrency)
		{
			ChangePlayerState(PlayerState.Idle);
		}
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

	public void ReciveResources(float _R)
	{
		currentCurrency += _R;
		playerUIManager.Updateplayercurrnecy(currentCurrency);
	}

	void OnNextWave()
	{
		PoppulateNextWave();
		SpawnNextWaveManager();
	}

	void PoppulateNextWave()
	{
		for (int i = 0; i < activeStamps.Count; i++)
		{
			Unit u = Instantiate(activeStamps[i].GetAttachedUnit(), currentWaveManagerObject.transform);
			u.transform.localPosition = activeStamps[i].transform.position;

			currentWaveManagerObject.AddUnitToWave(u, u.transform.localPosition, team);
		}
	}

	void SpawnNextWaveManager()
	{
		if (currentWaveManagerObject != null)
			currentWaveManagerObject.SendWave();

		currentWaveManagerObject = Instantiate(waveMangerPrefab, waveMangerSpawnPoint.transform.position, waveMangerSpawnPoint.transform.rotation, null);
		currentWaveManagerObject.name = "Wave Manager " + GameManager.gameManager.GetWaveNumber();
	}

	void OnDestroy()
	{
		GameManager.gameManager.onStartWave -= OnNextWave;
	}
}
