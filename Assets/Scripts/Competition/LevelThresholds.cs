// Score needed for the third star on each level (Normal-difficulty score scale; Hard and
// Very Hard scores are already multiplied, so they reach it sooner).
//
// PROVISIONAL: 250 starting stars plus one per second of level time. To be replaced by
// values calibrated from good real runs (Nikos's best scores) and from analytics.
public static class LevelThresholds
{
	static readonly int[] threeStarScore =
	{
		// 1-10
		270, 280, 290, 300, 310, 320, 330, 340, 350, 360,
		// 11-20
		370, 380, 390, 400, 410, 450, 460, 490, 490, 520,
		// 21-30
		550, 580, 610, 640, 670, 700, 730, 760, 790, 820,
		// 31-40
		850, 880, 910, 940, 970, 1000, 1030, 1060, 1090, 1150,
	};

	public static int ThreeStarScore(int level)
	{
		if (level < 1 || level > threeStarScore.Length) { return int.MaxValue; }
		return threeStarScore[level - 1];
	}
}
