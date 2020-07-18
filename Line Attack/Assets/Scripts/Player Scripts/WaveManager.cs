using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
	[SerializeField] private int waveNumber;
	[SerializeField] private List<FormationInfo> formation = new List<FormationInfo>();
	[SerializeField] private List<Unit> activeUnitAgent = new List<Unit>();

	public void AddUnitToWave(Unit _u, Vector3 _off, int _team)
	{
		FormationInfo f = new FormationInfo(_u, _off);
		formation.Add(f);

		_u.Setup(_team);
	}

	public void SendWave()
	{ 
		
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
