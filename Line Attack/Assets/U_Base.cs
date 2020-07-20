using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class U_Base : Unit
{
	public override void ChangeUnitState(UnitState newUnitState)
	{
		currentState = newUnitState;

		switch (currentState)
		{
			case UnitState.Idle:
				break;

			case UnitState.Marching:
				break;

			case UnitState.WalkingTo:
				break;

			case UnitState.Attacking:
				break;

			case UnitState.Dead:
				owner.LostBase(this);
				//SpawnDeath Effect Here
				break;
		}
	}

	public override int GetFormationID()
	{
		return base.GetFormationID();
	}

	public override int GetTeam()
	{
		return base.GetTeam();
	}

	public override void Die()
	{
		base.Die();
	}

	public override void Setup(Actor _owner, int _team, int _forIndex, WaveManager _waveManager)
	{
		team = _team;
		owner = _owner;
	}

	public override void TakeDamage(float dam, Actor damageDealer)
	{
		if (currentState != UnitState.Dead)
		{
			health -= dam;

			if (health <= 0)
			{
				ChangeUnitState(UnitState.Dead);
			}
		}

	}

	#region BlackedFunctions
	public override void FindClosedEnemyCall(){}

	public override void SendBackToFormation(){}

	public override void SetFormationID(int newID){}

	public override void SetMoveTo(Vector3 dest){}

	public override void SetTarget(Unit _target){}
	
	private void OnEnable(){}

	private void OnDisable(){}

	private void Update() { }

	#endregion BlackedFunctions

}
