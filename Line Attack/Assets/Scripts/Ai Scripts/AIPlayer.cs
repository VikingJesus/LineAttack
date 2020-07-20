using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayer : Actor
{
    [SerializeField] private GameObject prefFeild;
    [SerializeField] GameObject testStampPrefab;
    [SerializeField] Bounds bounds;

	protected override void Setup()
	{
		base.Setup();

        bounds = GetChildRendererBounds(prefFeild);

        for (int i = 0; i < 10;)
        {
            if (SpawnObjectInBounds() == true)
            {
                i++;
            }
        }
    }

	bool SpawnObjectInBounds()
    {
        float posX  = Random.Range(bounds.min.x + 2f, bounds.max.x - 2f);
        float posZ = Random.Range(bounds.min.z + 2f, bounds.max.z - 2f);

        Vector3 pos = new Vector3((int)posX +0.5f, 0.7f, (int)posZ - 0.5f);

        GameObject newTestObject = Instantiate(testStampPrefab.gameObject, pos, Quaternion.LookRotation(-Vector3.forward, transform.up), stampHolder);

        if (newTestObject.GetComponent<UnitStamp>().IsSafeToPlace())
        {
            activeStamps.Add(newTestObject.GetComponent<UnitStamp>());
            return true;
        }
        else
        {
            Destroy(newTestObject);
            return false;
        }
    }

    Bounds GetChildRendererBounds(GameObject go)
    {
        Renderer[] renderers = go.GetComponentsInChildren<Renderer>();

        if (renderers.Length > 0)
        {
            Bounds bounds = renderers[0].bounds;
            for (int i = 1, ni = renderers.Length; i < ni; i++)
            {
                bounds.Encapsulate(renderers[i].bounds);
            }
            return bounds;
        }
        else
        {
            return new Bounds();
        }
    }


}
