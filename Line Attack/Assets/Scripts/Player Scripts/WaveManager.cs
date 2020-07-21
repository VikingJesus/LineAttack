using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WaveManager : MonoBehaviour
{
	public enum WaveMangerState {Idle, Moving}
	public WaveMangerState waveMangerState;

	[SerializeField] float movementSpeed = 12f;
	[SerializeField] Actor owner;

	[SerializeField] private GameObject activeUnitHolder;
	[SerializeField] private List<FormationInfo> formation = new List<FormationInfo>();
	[SerializeField] private List<Unit> activeUnitAgent = new List<Unit>();

	[SerializeField] Vector3 movmentDirection;
	[SerializeField] float startingLerch = 30f;

	#region ListHandlers

	public void AddToActiveUnitAgent(Unit u) { if (!activeUnitAgent.Contains(u)) { activeUnitAgent.Add(u); } }

	public void RemoveFromActive(Unit u) { if (activeUnitAgent.Contains(u)) { activeUnitAgent.Remove(u); } }

	public void RemoveFromFormation(int formationID)
	{
		if (formation.Count == 1)
		{
			Destroy(activeUnitHolder);
			Destroy(gameObject);
			return;
		}

		formation.Remove(formation[formationID]);

		for (int i = 0; i < formation.Count; i++)
		{
			if (formation[i].unit != null)
				formation[i].unit.SetFormationID(i);
		}
	}

	public void AddUnitToWave(Unit _u, Vector3 _off, int _team, Actor _owner)
	{
		FormationInfo f = new FormationInfo(_u, _off);
		formation.Add(f);

		_u.Setup(_owner, _team, formation.Count - 1, this);
	}

	#endregion

	#region Getters

	public Transform GetActiveUnitHolder()
	{
		return activeUnitHolder.transform;
	}

	#endregion

	public void SetForwardDirection(Vector3 movementDirection)
	{
		movmentDirection = movementDirection;
	}

	public virtual void ChangeWaveManagerState(WaveMangerState newState)
	{
		waveMangerState = newState;

		switch (waveMangerState)
		{
			case WaveMangerState.Idle:
				break;
			case WaveMangerState.Moving:
				break;
		}
	}

	private void Start()
	{
		activeUnitHolder = new GameObject(gameObject.name + " Unit Holder For Wave " + GameManager.gameManager.GetWaveNumber());
		activeUnitHolder.transform.position = transform.position;
		activeUnitHolder.transform.parent = null;
	}

	private void Update()
	{
		if (waveMangerState == WaveMangerState.Moving && activeUnitAgent.Count != formation.Count)
		{
			transform.position += (movmentDirection * movementSpeed) * Time.deltaTime;
		}
	}

	public void SendWave()
	{
		for (int i = 0; i < formation.Count; i++)
		{
			formation[i].unit.SetMoveTo(formation[i].unit.transform.position + movmentDirection * startingLerch);
			formation[i].unit.StartCoroutine(formation[i].unit.ControlledUpdate());
		}

		transform.position += movmentDirection * startingLerch;
		StartCoroutine(WaitToBeginWave());
	}

	private IEnumerator WaitToBeginWave()
	{
		while (activeUnitAgent.Count > 0)
		{
			yield return new WaitForSeconds(0.1f);

			if (activeUnitAgent.Count == 0)
			{
				ChangeWaveManagerState(WaveMangerState.Moving);
			}
		}

		if (activeUnitAgent.Count == 0)
		{
			ChangeWaveManagerState(WaveMangerState.Moving);
		}
	}

	public void SnapToPointAndReperent(int fi)
	{
		formation[fi].unit.ChangeUnitState(Unit.UnitState.Marching);
		formation[fi].unit.transform.localPosition = formation[fi].localOfSet;
		formation[fi].unit.transform.rotation = transform.rotation;
	}

	public Vector3 ReturnUnitToPositionInFormation(Unit unit)
	{
		Vector3 pos = Vector3.zero;

		if (formation[unit.GetFormationID()] != null)
			pos = transform.TransformPoint(formation[unit.GetFormationID()].localOfSet);
		else
			Debug.Log("Unit" + unit.name + "Dose not know were to go");

		return pos;
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
