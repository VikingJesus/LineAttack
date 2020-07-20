using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceTarget : MonoBehaviour
{
    [SerializeField] List<Unit> targets = new List<Unit>();
    [SerializeField] int team;

    public void SetTeam(int _team)
    {
        team = _team;
    }

	public void OnTriggerEnter(Collider other)
	{
        if (targets.Count == 0)
            return;

        if (other.GetComponent<Unit>())
        {
            if (other.GetComponent<Unit>().GetTeam() != team)
            {
                ForceTargetToClosestTarget(other.GetComponent<Unit>());
            }
        }
	}

    public void ForceTargetToClosestTarget(Unit u)
    {
        if (targets.Count == 0)
            Destroy(gameObject);


        float smallestDist = float.MaxValue;
        Unit newClosestEnemy = null;

        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i] != null)
            {
                if (newClosestEnemy == null)
                {
                    newClosestEnemy = targets[i];
                    smallestDist = Vector3.Distance(targets[i].transform.position, newClosestEnemy.transform.position);
                }
                else if (Vector3.Distance(targets[i].transform.position, newClosestEnemy.transform.position) < smallestDist)
                {
                    newClosestEnemy = targets[i];
                    smallestDist = Vector3.Distance(targets[i].transform.position, newClosestEnemy.transform.position);
                }
            }
            else
            {
                targets.RemoveAt(i);
                ForceTargetToClosestTarget(u);
                return;
            }
        }

        if (newClosestEnemy != null)
            u.ForceTarget(newClosestEnemy);
    }

}
