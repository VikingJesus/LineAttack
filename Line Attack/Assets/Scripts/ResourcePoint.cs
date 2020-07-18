using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcePoint : MonoBehaviour
{
    [SerializeField] int level = 0;
    [SerializeField] Player playerOwner;
    [Space]
    [SerializeField] float generationAmountLevel1 = 100;
    [SerializeField] float generationAmountLevel2 = 200;
    [SerializeField] float generationAmountLevel3 = 300;
    [Space]
    [SerializeField] GameObject extractorGFXlevel1Prefab;
    [SerializeField] GameObject extractorGFXlevel2Prefab;
    [SerializeField] GameObject extractorGFXlevel3Prefab;

    [Space]
    GameObject currentExtractorGFX;
    bool upgradedThisWave = false;
    float currentResourceGainPerWave = 0;

    void Start()
    {
       LevelUpResourcePoint(level);
        GameManager.gameManager.onStartWave += OnNextWave;
        upgradedThisWave = false;
    }

    public void LevelUpResourcePoint(int newLevel)
    {
        level = newLevel;
        upgradedThisWave = true;

        if (currentExtractorGFX != null)
            Destroy(currentExtractorGFX);

        switch (newLevel)
        {
            case 0:
                currentExtractorGFX = null;
                currentResourceGainPerWave = 0;
                break;
            case 1:
                currentExtractorGFX = Instantiate(extractorGFXlevel1Prefab, transform.position, transform.rotation, transform);
                currentResourceGainPerWave = generationAmountLevel1;
                break;
            case 2:
                currentExtractorGFX = Instantiate(extractorGFXlevel2Prefab, transform.position, transform.rotation, transform);
                currentResourceGainPerWave = generationAmountLevel2;
                break;
            case 3:
                currentExtractorGFX = Instantiate(extractorGFXlevel3Prefab, transform.position, transform.rotation, transform);
                currentResourceGainPerWave = generationAmountLevel3;
                break;
        }
    }

    public void OnNextWave()
    {
        if (upgradedThisWave == true)
        {
            LevelUpResourcePoint(level += 1);
            upgradedThisWave = false;
            return;
        }

        GiveResourcesToPlayer();
    }

    public void GiveResourcesToPlayer()
    {
        playerOwner.ReciveResources(currentResourceGainPerWave);
    }

	private void OnDestroy()
	{
        GameManager.gameManager.onStartWave -= OnNextWave;

    }
}
