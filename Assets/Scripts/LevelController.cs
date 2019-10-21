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

    public void AttackerKilled()
    {
        numberOfAttackers--;
		int lives = Convert.ToInt32(livesText.text);
		if (numberOfAttackers <= 0 && levelTimerFinished && lives > 0 )
        {
            StartCoroutine(HandleWinCondition());
        }
    }

    IEnumerator HandleWinCondition()
    {
        winLabel.SetActive(true);
        audioSource.PlayOneShot(levelWinSound);
        yield return new WaitForSeconds(waitToLoad);
        FindObjectOfType<LevelLoader>().LoadNextScene();
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
    }

    private void StopSpawners()
    {
        AttackerSpawner[] spawnerArray = FindObjectsOfType<AttackerSpawner>();
        foreach (AttackerSpawner spawner in spawnerArray)
        {
            spawner.StopSpawning();
        }
    }

}
