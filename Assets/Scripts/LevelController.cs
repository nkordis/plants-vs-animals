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
		if (!gameEnded && numberOfAttackers <= 0 && levelTimerFinished && lives > 0 )
        {
			StartCoroutine(HandleWinCondition());
		}
    }
	
		IEnumerator HandleWinCondition() {
		gameEnded = true;
		LevelRun.MarkFinished();
			winLabel.SetActive(true);
			audioSource.PlayOneShot(levelWinSound);
			LevelResult result = RecordLevelResult();
			// The win panel now shows the score (stars left), best and rank and waits for the player.
			LevelHud hud = LevelHud.Instance;
			if (hud != null) {
				hud.ShowResults(result);
				yield break;
			}
			yield return new WaitForSeconds(waitToLoad);
			FindAnyObjectByType<LevelLoader>().LoadNextScene();
		}

	// A run saved by a rewarded "continue" unlocks the next level but does not count as a
	// best score.
	private static LevelResult RecordLevelResult() {
		int starsLeft = Convert.ToInt32(FindAnyObjectByType<StarDisplay>().GetStars());
		int score = ScoreRules.FinalScore(starsLeft, LevelRun.Difficulty);
		int level = LevelRun.Level;
		int oldScore = PlayersScoreController.GetScore(level);
		bool newBest = !LevelRun.Continued && score > oldScore;
		if (newBest)
			{
			PlayersScoreController.SetScore(level, score);
			}
		// Unlock the next level now: the player may leave from the win panel without
		// pressing Next level, which is where the original game unlocked it.
		if (PlayersMaxLevelController.GetMaxLevel() < level) {
			PlayersMaxLevelController.SetMaxLeveL(level);
		}
		PlayerPrefs.Save();
		Analytics.LevelWin(level, LevelRun.Difficulty, score, newBest, LevelRun.LivesLost,
			LevelRun.Continued, LevelRun.DefendersPlaced, LevelRun.Duration);
		return new LevelResult {
			Level = level, Score = score, PreviousBest = oldScore, NewBest = newBest,
			Continued = LevelRun.Continued,
		};
	}

	public void HandleLoseCondition()
    {
        loseLabel.SetActive(true);
		audioSource.PlayOneShot(levelLostSound);
		Time.timeScale = 0;
		LevelHud hud = LevelHud.Instance;
		if (hud != null) {
			GameTimer timer = FindAnyObjectByType<GameTimer>();
			hud.OnLevelLost(timer != null ? timer.GetProgress() : 0f);
		}
    }

    public void LevelTimerFinished()
    {
        levelTimerFinished = true;
        StopSpawners();
		AttackerKilled(false);// calls attacker killed method in case there is no other attacker to kill
    }

    private void StopSpawners()
    {
        AttackerSpawner[] spawnerArray = FindObjectsByType<AttackerSpawner>(FindObjectsSortMode.None);
        foreach (AttackerSpawner spawner in spawnerArray)
        {
            spawner.StopSpawning();
        }
    }

	public bool GetGameEnded() {
		return gameEnded;
	}

	public GameObject WinPanel { get { return winLabel; } }

	public GameObject LosePanel { get { return loseLabel; } }
}
