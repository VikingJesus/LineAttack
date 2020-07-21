using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class WinLooseUI : MonoBehaviour
{
	public void RestartLevel()
	{
		Scene scene = SceneManager.GetActiveScene(); SceneManager.LoadScene(scene.name);
	}
	public void QuitGame()
	{
		Application.Quit();
	}
}
