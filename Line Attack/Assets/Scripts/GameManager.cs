using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager gameManager;

	[Header("Wave Veribles")]
	[SerializeField] float timeBetweenWavesInSeconds = 60;
	[SerializeField] float currentTime;
	[SerializeField] int currentWave = 1;

	[SerializeField] Player player;
	[SerializeField] AIPlayer aIplayer;

	public delegate void OnStartWave();
	public OnStartWave onStartWave;

	public GameObject victoryScreen;
	public GameObject lossScreen;

	public int GetWaveNumber()
	{
		return currentWave;
	}

	public void Awake()
	{
		gameManager = this;
	}

	public void Start()
	{
		currentTime = timeBetweenWavesInSeconds;
		StartCoroutine(WaveCounter());
	}

	public IEnumerator WaveCounter()
	{
		while (gameObject.activeSelf)
		{
			yield return new WaitForSeconds(1);
			currentTime -= 1;

			player.GetPlayerUIManager().UpdateTimeLeftBeforNextWave(currentTime);

			if (currentTime <= 0)
			{
				//VS Says this is not needed, it is.
				if (onStartWave != null)
					onStartWave();

				currentWave += 1;

				currentTime = timeBetweenWavesInSeconds;
			}
		}
	}

	public void AplayerHasLostALLItsBases(Actor actor)
	{
		aIplayer.ChangePlayerState(Actor.PlayerState.Idle);
		player.ChangePlayerState(Actor.PlayerState.Idle);

		if (actor is AIPlayer)
		{
			victoryScreen.SetActive(true);
			//open win screen
		}
		else
		{
			lossScreen.SetActive(true);
			//open Lose screen
		}

	}



}
