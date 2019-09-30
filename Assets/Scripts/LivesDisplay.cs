using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LivesDisplay : MonoBehaviour {

    [SerializeField] float baseLives = 3;
    [SerializeField] int damage = 1;
	[SerializeField] AudioClip livesMinusSound;

    float lives;
    Text livesText;

	AudioSource audioSource;

    void Start()
    {
        lives = baseLives - PlayerPrefsController.GetDifficulty();
        livesText = GetComponent<Text>();
        UpdateDisplay();
		audioSource = GetComponent<AudioSource>();
        Debug.Log("difficulty setting currently is " + PlayerPrefsController.GetDifficulty());
    }

    private void UpdateDisplay()
    {
        livesText.text = lives.ToString();
    }

    public void TakeLife()
    {
        lives -= damage;
        UpdateDisplay();
		audioSource.PlayOneShot(livesMinusSound);

        if (lives <= 0)
        {
            FindObjectOfType<LevelController>().HandleLoseCondition();
        }
    }
}
