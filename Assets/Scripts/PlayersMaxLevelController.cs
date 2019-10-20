using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayersMaxLevelController 
{
	const string MAX_LEVEL_KEY = "max level";

	public static void SetMaxLeveL(int maxLevel) 
	{
		PlayerPrefs.SetInt(MAX_LEVEL_KEY, maxLevel);
	}

	public static int GetMaxLevel() 
	{
		return PlayerPrefs.GetInt(MAX_LEVEL_KEY);
	}
}
