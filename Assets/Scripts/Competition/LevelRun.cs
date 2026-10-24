using UnityEngine;

// State of the level attempt in progress. Reset every time a level scene starts.
public static class LevelRun
{
	public static int Level { get; private set; }
	public static float Difficulty { get; private set; }
	public static int LivesLost { get; private set; }
	public static int DefendersPlaced { get; private set; }
	public static bool Finished { get; private set; }

	static float startTime;

	public static float Duration { get { return Time.timeSinceLevelLoad - startTime; } }

	public static void Begin(int level)
	{
		Level = level;
		Difficulty = PlayerPrefsController.GetDifficulty();
		LivesLost = 0;
		DefendersPlaced = 0;
		Finished = false;
		startTime = Time.timeSinceLevelLoad;
		Analytics.LevelStart(level, Difficulty);
	}

	public static void LifeLost() { LivesLost++; }

	public static void DefenderPlaced() { DefendersPlaced++; }

	public static void MarkFinished() { Finished = true; }
}
