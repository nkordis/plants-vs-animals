// Google Play Games leaderboard ids, from Play Console > Play Games Services > Leaderboards.
// Empty ids are skipped, so the game runs normally before they are filled in.
public static class LeaderboardIds
{
	// Sum of the player's best score on every level.
	public const string Total = "";

	// Index 0 is level 1.
	static readonly string[] levels =
	{
		"", "", "", "", "", "", "", "", "", "",
		"", "", "", "", "", "", "", "", "", "",
		"", "", "", "", "", "", "", "", "", "",
		"", "", "", "", "", "", "", "", "", "",
	};

	public static string ForLevel(int level)
	{
		if (level < 1 || level > levels.Length) { return ""; }
		return levels[level - 1];
	}
}
