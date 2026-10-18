// Per-level results kept on the device: the best score (stars left) is stored by
// PlayersScoreController.
public static class LevelProgress
{
	public const int LevelCount = 40;

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
}
