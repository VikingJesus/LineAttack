using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour
{
	public enum UnitState { Idle, Marching, WalkingTo, Attacking }
	public UnitState currentState;

	[Header("Generic Properties")]

	[SerializeField] int team;
	[SerializeField] int formationIndex;
    [SerializeField] WaveManager waveManager;
    [SerializeField] float health;
    [SerializeField] float awarenessRange;
    [SerializeField] float rotationSpeed;
    [SerializeField] float stoppingDistance;
    [SerializeField] float unitcost;

	[SerializeField] float stoppingDist = 0.5f;

	[Header("Attack State Properties")]
	[SerializeField] NavMeshAgent agent;
    [SerializeField] LayerMask awarenessLayer;
	[SerializeField] float attackRange = 1;
    [SerializeField] float movementSpeed = 8;
    [SerializeField] float attackRate = 1;
    [SerializeField] float dammageAmount = 1;

	[Range(1, 0.1f)]
	[SerializeField] float findDelay = 0.25f;
	[SerializeField] bool lookingForTarget = false;
	[SerializeField] Unit target;
	
	public virtual int GetTeam()
	{
		return team;
	}

	public virtual int GetFormationID() { return formationIndex; }

	public virtual void Setup(int _team, int _forIndex, WaveManager _waveManager)
	{
		team = _team;
		waveManager = _waveManager;
		formationIndex = _forIndex;

		agent = GetComponent<NavMeshAgent>();

		agent.speed = movementSpeed;
		agent.stoppingDistance = stoppingDist;
	}

	public virtual void SetTarget(Unit _target)
	{
		target = _target;
		agent.SetDestination(target.transform.position);

		ChangeUnitState(UnitState.Attacking);
	}

	public virtual void SetMoveTo(Vector3 dest)
	{
		agent.destination = dest;
		ChangeUnitState(UnitState.WalkingTo);
	}

	public IEnumerator FindClosestEnemy()
	{
		Collider[] enemies;

		while (gameObject.activeSelf)
		{
			if (target == null)
			{
				yield return new WaitForSeconds(findDelay);

				float smallestDist = float.MaxValue;
				Collider newClosestEnemy = null;

				enemies = Physics.OverlapSphere(transform.position, awarenessRange, awarenessLayer);

				foreach (Collider enemyObj in enemies)
				{
					if (enemyObj.GetComponent<Unit>())
					{
						if (enemyObj.GetComponent<Unit>().GetTeam() != GetTeam())
						{
							float dist = Vector3.SqrMagnitude(enemyObj.transform.position - transform.position);

							if (dist < smallestDist)
							{
								newClosestEnemy = enemyObj;
								smallestDist = dist;
							}
						}
					}
				}

				if (newClosestEnemy != null)
				{
					SetTarget(newClosestEnemy.GetComponent<Unit>());
				}
			}
		}

	}

	public virtual void ChangeUnitState(UnitState newUnitState)
	{
		currentState = newUnitState;

		switch (currentState)
		{
			case UnitState.Idle:
				agent.enabled = false;
				waveManager.SnapToPointAndReperent(formationIndex);
				break;
			case UnitState.Marching:
				transform.parent = waveManager.transform;
				break;
			case UnitState.WalkingTo:
				agent.enabled = true;
				transform.parent = null;
				break;
			case UnitState.Attacking:
				agent.enabled = true;
				transform.parent = null;
				break;
		}
	}

	public void Update()
	{
		if (currentState == UnitState.WalkingTo)
		{
			float dist = agent.remainingDistance;

			if (dist != Mathf.Infinity && agent.pathStatus == NavMeshPathStatus.PathComplete && agent.remainingDistance == 0)
			{
				ChangeUnitState(UnitState.Idle);
			}
		}
	}

	private void OnEnable()
	{
		StartCoroutine(FindClosestEnemy());
	}

	private void OnDisable()
	{
		StopCoroutine(FindClosestEnemy());
	}
}
