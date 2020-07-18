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
    [SerializeField] WaveManager owningWave;
    [SerializeField] float health;
    [SerializeField] float awarenessRange;
    [SerializeField] float rotationSpeed;
    [SerializeField] float stoppingDistance;
    [SerializeField] float unitcost;

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

	private void OnEnable()
	{
		StartCoroutine(FindClosestEnemy());
	}

	private void OnDisable()
	{
		StopCoroutine(FindClosestEnemy());
	}

	public virtual void Setup(int _team)
	{
		team = _team;

		agent = GetComponent<NavMeshAgent>();

		agent.speed = movementSpeed;
		agent.stoppingDistance = attackRange;
	}

	public virtual int GetTeam()
	{
		return team;
	}

	public virtual void SetTarget(Unit _target)
	{
		target = _target;
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

				break;
			case UnitState.Marching:

				break;
			case UnitState.WalkingTo:

				break;
			case UnitState.Attacking:
				agent.SetDestination(target.transform.position);
				transform.parent = null;
				break;
		}
	}



}
