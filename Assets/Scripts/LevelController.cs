using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelController : MonoBehaviour {

    [SerializeField] float waitToLoad = 4f;
    [SerializeField] GameObject winLabel;
    [SerializeField] GameObject loseLabel;
	[SerializeField] AudioClip levelWinSound;
	[SerializeField] AudioClip levelLostSound;
	[SerializeField] Text livesText;

    int numberOfAttackers = 0;
    bool levelTimerFinished = false;
	bool gameEnded = false;

	AudioSource audioSource;

	private void Start()
    {
        winLabel.SetActive(false);
        loseLabel.SetActive(false);
		audioSource = GetComponent<AudioSource>();
    }

    public void AttackerSpawned()
    {
        numberOfAttackers++;
    }

	//This method is also called when the timer ends
    public void AttackerKilled(bool isAttackerKilled)
    {
		if (isAttackerKilled) {
			numberOfAttackers--;
		}
		int lives = Convert.ToInt32(livesText.text);
		if (numberOfAttackers <= 0 && levelTimerFinished && lives > 0 )
        {
			Debug.Log("Hnadle win condition: ");
			StartCoroutine(HandleWinCondition());
			Debug.Log("Hnadle win condition: OK1");
			//HandleWinCondition();
		}
    }
	
		IEnumerator HandleWinCondition() {
		Debug.Log("Hnadle win condition: OK2");
		gameEnded = true;
			winLabel.SetActive(true);
			audioSource.PlayOneShot(levelWinSound);
			AddLevelScore();
			yield return new WaitForSeconds(waitToLoad);
			FindObjectOfType<LevelLoader>().LoadNextScene();
		}


	private static void AddLevelScore() {
		int score = Convert.ToInt32(FindObjectOfType<StarDisplay>().GetStars());
		float difficulty = PlayerPrefsController.GetDifficulty();
		if (difficulty == 1f) 
			{ score = (3 * score) / 2; } else if (difficulty == 2f) { score = 2 * score; }

		int level = FindObjectOfType<LevelDisplay>().GetCurrentLevel();
		int oldScore = PlayersScoreController.GetScore(level);
		Debug.Log("score: " + score);
		Debug.Log("oldScore: " + oldScore);
		if (score > oldScore) 
			{
			PlayersScoreController.SetScore(level, score);
			}
	}

	public void HandleLoseCondition()
    {
        loseLabel.SetActive(true);
		audioSource.PlayOneShot(levelLostSound);
		Time.timeScale = 0;
    }

    public void LevelTimerFinished()
    {
        levelTimerFinished = true;
        StopSpawners();
		AttackerKilled(false);// calls attacker killed method in case there is no other attacker to kill
    }

    private void StopSpawners()
    {
        AttackerSpawner[] spawnerArray = FindObjectsOfType<AttackerSpawner>();
        foreach (AttackerSpawner spawner in spawnerArray)
        {
            spawner.StopSpawning();
        }
    }

	public bool GetGameEnded() {
		return gameEnded;
	}
}
