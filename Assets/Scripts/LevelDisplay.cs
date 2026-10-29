using System;
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
		levelLoader = FindAnyObjectByType<LevelLoader>();
		
	}

	private void Update() {
		currentLevel = levelLoader.GetCurrentLevel();
		levelText.text = UiText.Get("choose_level", "Level") + " " + currentLevel;
	}

	public int GetCurrentLevel() {
		return currentLevel;
	}
}
