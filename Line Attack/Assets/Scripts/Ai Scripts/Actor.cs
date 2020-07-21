using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
	public enum PlayerState { Idle, Paused, BuildingStamp }
	[SerializeField] protected int team = 1;

	[SerializeField] protected PlayerState currentPlayerState;
    [SerializeField] protected List<U_Base> playerBases = new List<U_Base>();

	[Header("Wave Veribles")]
	[SerializeField] protected WaveManager waveMangerPrefab;
	protected WaveManager currentWaveManagerObject;
	[SerializeField] protected Transform waveMangerSpawnPoint;

	[SerializeField] protected ForceTarget forceTarget;
	[SerializeField] protected Transform stampHolder;
	protected List<UnitStamp> activeStamps = new List<UnitStamp>();


	[Header("Player Stats")]
	[SerializeField] Vector3 forward = new Vector3(0f, 0f, -1f);
	[SerializeField] protected float currentCurrency = 1000;

	#region Getters
	public virtual int GetTeam() { return team; }

	#endregion

	#region Setters

	protected virtual void Setup()
	{
		GameManager.gameManager.onStartWave += OnNextWave;

		forceTarget.SetTeam(team);
		SpawnNextWaveManager();

		for (int i = 0; i < playerBases.Count; i++)
		{
			playerBases[i].Setup(this, team, 0, null);
		}
	}

	#endregion
	
	public virtual void ChangePlayerState(PlayerState newPlayerState)
	{
		currentPlayerState = newPlayerState;

		switch (currentPlayerState)
		{
			case PlayerState.Idle:
				StopAllCoroutines();
				break;
			case PlayerState.Paused:
				break;
			case PlayerState.BuildingStamp:
				break;
		}
	}

	void Start()
	{
		Setup();
	}

	public virtual void LostBase(U_Base u_Base)
	{
		playerBases.Remove(u_Base);

		if (playerBases.Count == 0)
			Debug.Log(gameObject.name + " Has lost the game");
	}

	public virtual void ReciveResources(float _R)
	{
		currentCurrency += _R;
	}

	protected virtual void OnNextWave()
	{
		PoppulateNextWave();
		SpawnNextWaveManager();
	}

	protected virtual void PoppulateNextWave()
	{
		for (int i = 0; i < activeStamps.Count; i++)
		{
			Unit u = Instantiate(activeStamps[i].GetAttachedUnit(), currentWaveManagerObject.transform);
			u.transform.localPosition = activeStamps[i].transform.localPosition;

			currentWaveManagerObject.AddUnitToWave(u, u.transform.localPosition, team, this);
		}
	}

	protected virtual void SpawnNextWaveManager()
	{
		if (currentWaveManagerObject != null)
			currentWaveManagerObject.SendWave();

		currentWaveManagerObject = Instantiate(waveMangerPrefab, waveMangerSpawnPoint.transform.position, waveMangerSpawnPoint.transform.rotation, null);
		currentWaveManagerObject.name = gameObject.name + " Wave Manager " + GameManager.gameManager.GetWaveNumber();
		currentWaveManagerObject.SetForwardDirection(forward);

	}

	void OnDestroy()
	{
		GameManager.gameManager.onStartWave -= OnNextWave;
	}

}
