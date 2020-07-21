using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MainMenuUIScript : MonoBehaviour
{
	public void LoadLevel()
	{
		SceneManager.LoadScene (1, LoadSceneMode.Single);
	}
	public void QuitGame()
	{
		Application.Quit();
	}
}
