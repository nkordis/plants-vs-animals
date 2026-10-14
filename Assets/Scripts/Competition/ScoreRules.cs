// How a finished level is scored and rated. The level balance itself is untouched:
// the score is still the stars left at the end, multiplied for the harder settings.
public static class ScoreRules
{
	// Same arithmetic as the 1.0/1.1 LevelController, so old and new best scores compare.
	public static int FinalScore(int starsLeft, float difficulty)
	{
		if (difficulty == 1f) { return (3 * starsLeft) / 2; }
		if (difficulty == 2f) { return 2 * starsLeft; }
		return starsLeft;
	}

	// 1 star: level won. 2 stars: won without losing a life. 3 stars: also scored at least
	// the level's threshold. A run saved by a rewarded "continue" only ever earns 1 star.
	public static int Rating(int level, int score, int livesLost, bool continued)
	{
		if (continued || livesLost > 0) { return 1; }
		return score >= LevelThresholds.ThreeStarScore(level) ? 3 : 2;
	}
}
