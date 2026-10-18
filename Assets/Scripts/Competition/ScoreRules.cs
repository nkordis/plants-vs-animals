// How a finished level is scored. The level balance itself is untouched: the score is the
// stars left at the end, multiplied for the harder settings.
public static class ScoreRules
{
	// Same arithmetic as the 1.0/1.1 LevelController, so old and new best scores compare.
	public static int FinalScore(int starsLeft, float difficulty)
	{
		if (difficulty == 1f) { return (3 * starsLeft) / 2; }
		if (difficulty == 2f) { return 2 * starsLeft; }
		return starsLeft;
	}
}
