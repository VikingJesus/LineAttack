using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.ProBuilder;

public class Unit : MonoBehaviour
{
	public enum UnitState {Marching, WalkingTo, Attacking, Dead, Idle }
	public UnitState currentState;

	[SerializeField] protected int formationIndex;
	[SerializeField] protected WaveManager waveManager;
	[SerializeField] protected Actor owner;

	NavMeshAgent agent;
	Animator anim;

	[Header("Generic Properties")]
	[SerializeField] protected int team;
    [SerializeField] protected float health;
    [SerializeField] protected float awarenessRange;
    [SerializeField] protected float rotationSpeed;
	[SerializeField] protected float stoppingDist = 0.4f;
	[SerializeField] protected float unitcost;
	[SerializeField] protected ForceTarget forceTarget;

	[Space]
	[Header("Attack State Properties")]
    [SerializeField] protected LayerMask awarenessLayer;
	[SerializeField] protected float attackRange = 0.01f;
    [SerializeField] protected float movementSpeed = 8;
    [SerializeField] protected float attackRate = 1;
    [SerializeField] protected float dammageAmount = 1;

	[Range(1, 0.1f)]
	[SerializeField] float findDelay = 0.25f;
	bool lookingForTarget = false;
	[SerializeField] bool attacking = false;
	Unit target;

	#region Getters

	public virtual int GetTeam()
	{
		return team;
	}

	public virtual int GetFormationID() { return formationIndex; }

	public virtual int GetUnitCost() { return (int)unitcost; }

	#endregion

	#region Setters

	public virtual void SetFormationID(int newID) { formationIndex = newID; }

	public virtual void Setup(Actor _owner, int _team, int _forIndex, WaveManager _waveManager)
	{
		owner = _owner;
		team = _team;
		waveManager = _waveManager;
		formationIndex = _forIndex;

		agent = GetComponent<NavMeshAgent>();

		agent.speed = movementSpeed;
		agent.stoppingDistance = stoppingDist;

		transform.parent = waveManager.GetActiveUnitHolder();
	}

	public virtual void SetForceTarget(ForceTarget ft)
	{
		StopCoroutine(FindClosestEnemy());

		forceTarget = ft;
		forceTarget.ForceTargetToClosestTarget(this);
		attackRange = attackRange * +1.2f;
	}
	
	public virtual void SetTarget(Unit _target)
	{
		if (_target != null)
		{
			ChangeUnitState(UnitState.Attacking);

			target = _target;
			agent.SetDestination(target.transform.position);
		}
		else if (currentState != UnitState.Marching)
		{
			SendBackToFormation();
		}
	}

	#endregion

	private void Awake()
	{
		agent = GetComponent<NavMeshAgent>();
		anim = GetComponent<Animator>();
	}

	public virtual void SendBackToFormation()
	{
		if (currentState == UnitState.Marching)
			return;

		if (waveManager == null)
			waveManager = GetComponentInParent<WaveManager>();

		SetMoveTo(waveManager.ReturnUnitToPositionInFormation(this));
	}

	public void ForceTarget(Unit _target, ForceTarget _forceTarget)
	{
		if (_target != null)
		{
			ChangeUnitState(UnitState.Attacking);

			target = _target;
			agent.SetDestination(target.transform.position);
			StopCoroutine(FindClosestEnemy());
		}
	}

	public virtual void SetMoveTo(Vector3 dest)
	{
		ChangeUnitState(UnitState.WalkingTo);

		agent.SetDestination(dest);
		agent.isStopped = false;
	}

	public virtual void TakeDamage(float dam, Actor damageDealer)
	{
		if (currentState != UnitState.Dead)
		{
			health -= dam;

			if (health <= 0) 
			{
				ChangeUnitState(UnitState.Dead);
				damageDealer.ReciveResources(unitcost * 0.15f);
			}
		}
	}

	public virtual void Die()
	{
		Destroy(gameObject);
	}

	public IEnumerator FindClosestEnemy()
	{
		yield return new WaitForSeconds(findDelay);

		while (gameObject.activeSelf)
		{
			yield return new WaitForSeconds(findDelay);

			if (currentState != UnitState.Dead)
			{
				FindClosedEnemyCall();
			}
			else
				StopCoroutine(FindClosestEnemy());
		}
	}

	public virtual void FindClosedEnemyCall()
	{
		if (forceTarget != null)
		{
			if (target == null)
				forceTarget.ForceTargetToClosestTarget(this);
			else
			{
				if (Vector3.Distance(target.transform.position, transform.position) > attackRange -0.1f)
					agent.SetDestination(target.transform.position);
				else
					agent.SetDestination(transform.position);
			}

			return;
		}

		Collider[] enemies;

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
			if (Vector3.Distance(transform.position, target.transform.position) > (awarenessRange + 1f))
			{
				Debug.Log(target.name);
				Debug.Log("Target is out of range, setting it to null");
				SetTarget(null);
			}
		}
	}

	public virtual void ChangeUnitState(UnitState newUnitState)
	{
		currentState = newUnitState;

		switch (currentState)
		{
			case UnitState.Idle:

				anim.SetBool("Idle", true);
				anim.SetBool("Marching", false);

				waveManager.RemoveFromActive(this);

				agent.enabled = false;
				
				waveManager.SnapToPointAndReperent(formationIndex);
				break;

			case UnitState.Marching:

				anim.SetBool("Marching", true);

				anim.SetBool("Idle", false);
				anim.SetBool("AttackMove", false);

				waveManager.RemoveFromActive(this);
				transform.parent = waveManager.transform;

				break;

			case UnitState.WalkingTo:

				transform.parent = waveManager.GetActiveUnitHolder();
				waveManager.AddToActiveUnitAgent(this);
				agent.enabled = true;

				anim.SetBool("Marching", true);
				anim.SetBool("AttackMove", false);
				anim.SetBool("Attack", false);
				break;

			case UnitState.Attacking:
				//Anim stuff
				anim.SetBool("AttackMove", true);
				anim.SetBool("Marching", false);

				waveManager.AddToActiveUnitAgent(this);
				agent.enabled = true;
				transform.parent = waveManager.GetActiveUnitHolder();

				break;

			case UnitState.Dead:

				if (waveManager != null)
				{
					waveManager.RemoveFromActive(this);
					waveManager.RemoveFromFormation(formationIndex);
				}

				anim.SetTrigger("Death");

				agent.enabled = false;
				transform.parent = waveManager.GetActiveUnitHolder();
				attackRate = 0;
				GetComponent<BoxCollider>().enabled = false;
				break;
		}
	}

	public IEnumerator ControlledUpdate()
	{
		yield return new WaitForSeconds(findDelay / 2);

		while (gameObject.activeSelf)
		{
			yield return new WaitForSeconds(findDelay/2);

			if (currentState != UnitState.Marching)
			{
				if (currentState == UnitState.WalkingTo)
				{
					if (agent.isOnNavMesh)
					{
						if (agent.remainingDistance <= 1f)
						{
							ChangeUnitState(UnitState.Idle);
						}
					}
				}

				if (currentState == UnitState.Attacking)
				{
					if (target != null)
					{
						//TODO EnterDraw anim.
						//Wait till its done.
						transform.LookAt(target.transform);

						if (Vector3.Distance(target.transform.position, transform.position) > attackRange - .72f)
						{
							agent.SetDestination(target.transform.position);
						}
						else
						{
							agent.SetDestination(transform.position);

							//Is close enough to attck, ATTACK
							if (attacking == false)
								StartCoroutine(AttackRate());
						}
					}
					else
					{
						FindClosedEnemyCall();

						if (target == null)
						{
							SendBackToFormation();
						}
					}
				}
			}
		}
	}

	public IEnumerator AttackRate()
	{
		attacking = true;
		anim.SetBool("Attack", true);

		while (currentState == UnitState.Attacking)
		{
			yield return new WaitForSeconds(attackRate/2);
			CreateAtatckEffect();
			yield return new WaitForSeconds(attackRate/2);

			if (target == null)
			{
				StopCoroutine(AttackRate());
			}
			else
			{

				if (Vector3.Distance(target.transform.position, transform.position) > attackRange - 0.02f)
				{
					if(agent.isOnNavMesh)
						agent.SetDestination(target.transform.position);
				}

				target.TakeDamage(dammageAmount, owner);
			}
		}

		anim.SetBool("Attack", false);
		attacking = false;
	}

	public virtual void CreateAtatckEffect()
	{ 
	
	}

	private void OnEnable()
	{
		StartCoroutine(FindClosestEnemy());
	}

	private void OnDisable()
	{
		StopCoroutine(FindClosestEnemy());
		StopCoroutine(AttackRate());
	}
}
