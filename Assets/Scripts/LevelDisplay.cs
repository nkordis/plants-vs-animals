using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelDisplay : MonoBehaviour
{

	Text levelText;
	LevelLoader levelLoader;
	int currentLevel;
   
    void Start()
    {
		levelText = GetComponent<Text>();
		levelLoader = FindObjectOfType<LevelLoader>();
		
	}

	private void Update() {
		currentLevel = levelLoader.GetCurrentLevel();
		levelText.text = "Level " + currentLevel;
	}
}
