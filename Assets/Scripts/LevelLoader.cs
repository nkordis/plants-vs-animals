using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour {

    [SerializeField] float timeToWait = 4f;
	[SerializeField] float timeToWaitUI = 0.5f;


	int currentSceneIndex;

	 void Start () {
		//ResetAllStoredValues();
		currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
		if (currentSceneIndex == 1) {
			StartCoroutine(WaitForTime(timeToWait));
		}
	}

	public void LoadChooseLevelScreen() {
		
		StartCoroutine(WaitForTime(1));
	}

	IEnumerator WaitForTime(float time)
    {
        yield return new WaitForSeconds(time);
        LoadNextScene();
    }

    public void RestartScene()
    {
		Time.timeScale = 1;
		SceneManager.LoadScene(currentSceneIndex);
	}

	public void LoadMainMenuNormally() {
		Time.timeScale = 1;
		SceneManager.LoadScene("Start Screen");
	}

	public void LoadMainMenu()
    {
		StartCoroutine(MainMenusWaitForTime(timeToWaitUI));
    }

	IEnumerator MainMenusWaitForTime(float time) {
		Time.timeScale = 1;
		yield return new WaitForSeconds(time);
		SceneManager.LoadScene("Start Screen");
	}

	public void LoadOptionsScreen()
    {
		StartCoroutine(OptionsWaitForTime(timeToWaitUI));
    }

	IEnumerator OptionsWaitForTime(float time) {
		yield return new WaitForSeconds(time);
		SceneManager.LoadScene("Options Screen");
	}

	public void LoadHelpScreen() 
	{
		StartCoroutine(HelpsWaitForTime(timeToWaitUI));
	}

	IEnumerator HelpsWaitForTime(float time) {
		yield return new WaitForSeconds(time);
		SceneManager.LoadScene("Help Screen");
	}


	public void LoadNextScene()
    {
        
		if (currentSceneIndex > 2 && PlayersMaxLevelController.GetMaxLevel() < GetCurrentLevel() ) {
			PlayersMaxLevelController.SetMaxLeveL(GetCurrentLevel());
		}

		Debug.Log("Current max level: " + PlayersMaxLevelController.GetMaxLevel());
		if (GetCurrentLevel() > 1) {
			FindObjectOfType<AdManager>().ShowAdd();
		}
		Time.timeScale = 1;
		SceneManager.LoadScene(currentSceneIndex + 1);
	}

    public void LoadYouLose()
    {
        SceneManager.LoadScene("Lose Screen");
    }

	public void LoadLevel(int level) 
	{
		//SceneManager.LoadScene(level + 2);
		StartCoroutine(LoadLevelsWaitForTime(level, timeToWaitUI));
	}

	IEnumerator LoadLevelsWaitForTime(int level,float time) {
		yield return new WaitForSeconds(time);
		SceneManager.LoadScene(level + 3);
	}

	public int GetCurrentLevel() 
	{
		return currentSceneIndex - 3;
	}
	
    public void QuitGame()
    {
		StartCoroutine(QuitsWaitForTime(timeToWaitUI));
    }

	IEnumerator QuitsWaitForTime(float time) {
		yield return new WaitForSeconds(time);
		Application.Quit();
	}

	private static void ResetAllStoredValues() {
		PlayersMaxLevelController.SetMaxLeveL(0);
		for (int i = 1; i < 20; i++) {
			PlayersScoreController.SetScore(i, 0);
		}
	}

}
