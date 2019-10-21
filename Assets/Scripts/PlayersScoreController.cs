using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayersScoreController : MonoBehaviour
{
	const string LEVEL_SCORE_KEY = "Level Score ";

	public static void SetScore(int level, int score) {
		PlayerPrefs.SetInt(LEVEL_SCORE_KEY+ level.ToString(), score);
	}

	public static int GetScore(int level) {
		return PlayerPrefs.GetInt(LEVEL_SCORE_KEY + level.ToString());
	}
}
