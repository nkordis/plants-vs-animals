using UnityEngine;

// Per-level results kept on the device: best score (PlayersScoreController) and the
// 1-3 star rating added in 1.2.0.
public static class LevelProgress
{
	public const int LevelCount = 40;

	const string RATING_KEY = "Level Rating ";

	public static int GetRating(int level)
	{
		return PlayerPrefs.GetInt(RATING_KEY + level);
	}

	// Keeps the best rating ever earned; returns true when this one is better.
	public static bool RecordRating(int level, int rating)
	{
		if (rating <= GetRating(level)) { return false; }
		PlayerPrefs.SetInt(RATING_KEY + level, rating);
		PlayerPrefs.Save();
		return true;
	}

	// Sum of the best score of every level, the value of the "total" leaderboard.
	public static long TotalBestScore()
	{
		long total = 0;
		for (int level = 1; level <= LevelCount; level++)
		{
			total += PlayersScoreController.GetScore(level);
		}
		return total;
	}

	public static int TotalRating()
	{
		int total = 0;
		for (int level = 1; level <= LevelCount; level++)
		{
			total += GetRating(level);
		}
		return total;
	}
}
