using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
	public enum WaveMangerState {Idle, Moving}
	public WaveMangerState waveMangerState;

	[SerializeField] private int waveNumber;
	[SerializeField] float movementSpeed = 12f;

	[SerializeField] private GameObject activeUnitHolder;
	[SerializeField] private List<FormationInfo> formation = new List<FormationInfo>();
	[SerializeField] private List<Unit> activeUnitAgent = new List<Unit>();

	[SerializeField] Vector3 startingPos;
	[SerializeField] Vector3 endPos;
	[SerializeField] float startingLerch = 30f;

	[SerializeField] private float fraction = 0;
	public Transform GetActiveUnitHolder()
	{
		return activeUnitHolder.transform;
	}

	public void AddToActiveUnitAgent(Unit u) { if (!activeUnitAgent.Contains(u)) { activeUnitAgent.Add(u); } }

	public void RemoveFromActive(Unit u) { if (activeUnitAgent.Contains(u)) { activeUnitAgent.Remove(u); } }
	
	public void RemoveFromFormation(int formationID)
	{
		Debug.Log("Removing " + formationID);

		formation.Remove(formation[formationID]);

		for (int i = 0; i < formation.Count; i++)
		{
			formation[i].unit.SetFormationID(i);
		}

		if (formation.Count == 0)
		{
			Destroy(activeUnitHolder);
			Destroy(gameObject);
		}
	}

	public void AddUnitToWave(Unit _u, Vector3 _off, int _team)
	{
		FormationInfo f = new FormationInfo(_u, _off);
		formation.Add(f);

		_u.Setup(_team, formation.Count - 1, this);
	}

	public virtual void ChangeWaveManagerState(WaveMangerState newState)
	{
		waveMangerState = newState;

		switch (waveMangerState)
		{
			case WaveMangerState.Idle:

				break;
			case WaveMangerState.Moving:
				startingPos = transform.position;
				break;
		}
	}

	public void Start()
	{
		activeUnitHolder = new GameObject("Unit Holder For Wave " + waveNumber);
		activeUnitHolder.transform.position = Vector3.zero;
		activeUnitHolder.transform.parent = null;
	}

	public void Update()
	{
		if (waveMangerState == WaveMangerState.Moving && activeUnitAgent.Count != formation.Count)
		{
			fraction += movementSpeed * Time.deltaTime;
			transform.position = Vector3.Lerp(startingPos, endPos, fraction);
		}
	}

	public void SendWave()
	{
		for (int i = 0; i < formation.Count; i++)
		{
			formation[i].unit.SetMoveTo(formation[i].unit.transform.position +  Vector3.forward * 1);
		}

		transform.position += Vector3.forward * startingLerch;
		RecallAllUnitsToPositions();
	}

	public void RecallAllUnitsToPositions()
	{
		for (int i = 0; i < activeUnitAgent.Count; i++)
		{
			activeUnitAgent[i].SendBackToFormation();
		}

		StartCoroutine(WaitToBeginWave());
	}

	public Vector3 ReturnUnitToPositionInFormation(Unit unit)
	{
		Vector3 pos = Vector3.zero;

		if (formation[unit.GetFormationID()] != null)
			pos = formation[unit.GetFormationID()].localOfSet + transform.position;
		else
			Debug.Log("Unit" + unit.name + "Dose not know were to go");

		return pos;
	}

	public IEnumerator WaitToBeginWave()
	{
		Debug.Log("Lauched Co");
		while (activeUnitAgent.Count > 0)
		{
			yield return new WaitForSeconds(0.1f);
			Debug.Log("Updating Co");

			if (activeUnitAgent.Count == 0)
			{
				ChangeWaveManagerState(WaveMangerState.Moving);
			}

			for (int i = 0; i < activeUnitAgent.Count; i++)
			{
				if (activeUnitAgent.Count < 10)
				{
					if (activeUnitAgent[i].reDis <= 0.5f)
					{
						SnapToPointAndReperent(activeUnitAgent[i].GetFormationID());
					}
				}
			}
		}

		Debug.Log("Exiting Co");


		if (activeUnitAgent.Count == 0)
		{
			ChangeWaveManagerState(WaveMangerState.Moving);
		}
	}

	public void SnapToPointAndReperent(int fi)
	{
		formation[fi].unit.ChangeUnitState(Unit.UnitState.Marching);
		formation[fi].unit.transform.localPosition = formation[fi].localOfSet;
		formation[fi].unit.transform.rotation = Quaternion.identity;
	}
}

[System.Serializable]
public class FormationInfo
{
	public FormationInfo(Unit _u, Vector3 _Offest)
	{
		unit = _u;
		localOfSet = _Offest;
	}

	public Unit unit;
	public Vector3 localOfSet;
}
