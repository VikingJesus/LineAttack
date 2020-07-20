using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour
{
	public enum UnitState { Idle, Marching, WalkingTo, Attacking }
	public UnitState currentState;

	int formationIndex;
	NavMeshAgent agent;
	WaveManager waveManager;

	[Header("Generic Properties")]
	[SerializeField] int team;
    [SerializeField] float health;
    [SerializeField] float awarenessRange;
    [SerializeField] float rotationSpeed;
	[SerializeField] float stoppingDist = 0.5f;
	[SerializeField] float unitcost;

	[Space]
	[Header("Attack State Properties")]
    [SerializeField] LayerMask awarenessLayer;
	[SerializeField] float attackRange = 1;
    [SerializeField] float movementSpeed = 8;
    [SerializeField] float attackRate = 1;
    [SerializeField] float dammageAmount = 1;

	[Range(1, 0.1f)]
	[SerializeField] float findDelay = 0.25f;
	bool lookingForTarget = false;
	Unit target;
	
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

	private void Awake()
	{
		agent = GetComponent<NavMeshAgent>();
	}

	public virtual void SendBackToFormation()
	{
		SetMoveTo(waveManager.ReturnUnitToPositionInFormation(this));
	}

	public virtual void SetTarget(Unit _target)
	{
		if (_target != null)
		{
			ChangeUnitState(UnitState.Attacking);

			target = _target;
			agent.SetDestination(target.transform.position);
		}
		else
		{
			SendBackToFormation();
		}
	}

	public virtual void SetMoveTo(Vector3 dest)
	{
		ChangeUnitState(UnitState.WalkingTo);

		if(agent.isOnNavMesh)
			agent.destination = dest;
		else
			ChangeUnitState(UnitState.Idle);
	}

	public IEnumerator FindClosestEnemy()
	{
		Collider[] enemies;

		while (gameObject.activeSelf)
		{
			yield return new WaitForSeconds(findDelay);

			if (target == null)
			{
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
					SetTarget(newClosestEnemy.GetComponent<Unit>());
				else
					SetTarget(null);
			}
			else
			{
				if (Vector3.Distance(transform.position, target.transform.position) > awarenessRange)
				{
					SetTarget(null);
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
				waveManager.RemoveFromActive(this);
				agent.enabled = false;
				agent.stoppingDistance = stoppingDist;
				waveManager.SnapToPointAndReperent(formationIndex);
				break;
			case UnitState.Marching:
				waveManager.RemoveFromActive(this);
				transform.parent = waveManager.transform;
				break;
			case UnitState.WalkingTo:
				waveManager.AddToActiveUnitAgent(this);
				agent.enabled = true;
				agent.stoppingDistance = stoppingDist;
				transform.parent = null;
				break;
			case UnitState.Attacking:
				Debug.Log("Entering Attack State");
				waveManager.AddToActiveUnitAgent(this);
				agent.enabled = true;
				agent.stoppingDistance = attackRange;
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

		if (currentState == UnitState.Attacking)
		{
			//TODO EnterDraw anim.
			//Wait till its done.
			if (Vector3.Distance(target.transform.position, transform.position) <= attackRange)
			{
				agent.SetDestination(target.transform.position);
			}
			else
			{
				Debug.Log("Sucsses");
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
