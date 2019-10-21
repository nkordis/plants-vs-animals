using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour {

    [SerializeField] int timeToWait = 4;
	
    int currentSceneIndex;

	void Start () {
		//ResetAllStoredValues();
		currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
		if (currentSceneIndex == 0) {
			StartCoroutine(WaitForTime());
		}
	}

	IEnumerator WaitForTime()
    {
        yield return new WaitForSeconds(timeToWait);
        LoadNextScene();
    }

    public void RestartScene()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(currentSceneIndex);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Start Screen");
    }

    public void LoadOptionsScreen()
    {
        SceneManager.LoadScene("Options Screen");
    }

	public void LoadHelpScreen() {
		SceneManager.LoadScene("Help Screen");
	}

	public void LoadNextScene()
    {
        
		if (currentSceneIndex > 2 && PlayersMaxLevelController.GetMaxLevel() < GetCurrentLevel() ) {
			PlayersMaxLevelController.SetMaxLeveL(GetCurrentLevel());
		}

		Debug.Log("Current max level: " + PlayersMaxLevelController.GetMaxLevel());
		SceneManager.LoadScene(currentSceneIndex + 1);
	}

    public void LoadYouLose()
    {
        SceneManager.LoadScene("Lose Screen");
    }

	public void LoadLevel(int level) 
	{
		SceneManager.LoadScene(level + 2);
	}

	public int GetCurrentLevel() 
	{
		return currentSceneIndex - 2;
	}
	
    public void QuitGame()
    {
        Application.Quit();
    }

	private static void ResetAllStoredValues() {
		PlayersMaxLevelController.SetMaxLeveL(0);
		for (int i = 1; i < 20; i++) {
			PlayersScoreController.SetScore(i, 0);
		}
	}

}
