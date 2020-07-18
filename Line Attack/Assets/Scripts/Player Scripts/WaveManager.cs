using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
	[SerializeField] private int waveNumber;
	[SerializeField] private List<FormationInfo> formation = new List<FormationInfo>();
	[SerializeField] private List<Unit> activeUnitAgent = new List<Unit>();

	public void AddToActiveUnitAgent(Unit u) { activeUnitAgent.Add(u); }
	
	public void AddUnitToWave(Unit _u, Vector3 _off, int _team)
	{
		FormationInfo f = new FormationInfo(_u, _off);
		formation.Add(f);

		_u.Setup(_team, formation.Count - 1, this);
	}

	public void SendWave()
	{
		for (int i = 0; i < formation.Count; i++)
		{
			formation[i].unit.SetMoveTo(formation[i].unit.transform.position +  Vector3.forward * 10);
		}

		transform.position += Vector3.forward * 15;

		Invoke("RecallAllUnitsToPositions", 0.5f);
	}

	public void RecallAllUnitsToPositions()
	{
		for (int i = 0; i < activeUnitAgent.Count; i++)
		{
			activeUnitAgent[i].SetMoveTo(transform.position + formation[activeUnitAgent[i].GetFormationID()].localOfSet);
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
