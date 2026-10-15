using System;
using UnityEngine;
#if UNITY_ANDROID
using GooglePlayGames;
using GooglePlayGames.BasicApi;
#endif

// Google Play Games Services v2: automatic sign-in and leaderboards. Does nothing until the
// Play Games app id is set up (Window > Google Play Games > Setup) and on non-Android builds.
public static class PlayGames
{
	public static bool SignedIn { get; private set; }

	public static bool IsConfigured
	{
		get
		{
#if UNITY_ANDROID && !UNITY_EDITOR
			PlayGamesSettings settings = PlayGamesSettings.LoadInstance();
			return settings != null && !string.IsNullOrEmpty(settings.AppId);
#else
			return false;
#endif
		}
	}

	public static void Init()
	{
		if (!IsConfigured) { return; }
#if UNITY_ANDROID
		PlayGamesPlatform.Activate();
		PlayGamesPlatform.Instance.Authenticate(OnSignIn);
#endif
	}

	// Explicit sign-in from a button, for players whose automatic sign-in did not happen.
	public static void SignIn(Action<bool> done)
	{
		if (!IsConfigured) { done?.Invoke(false); return; }
		if (SignedIn) { done?.Invoke(true); return; }
#if UNITY_ANDROID
		PlayGamesPlatform.Instance.ManuallyAuthenticate(status =>
		{
			OnSignIn(status);
			done?.Invoke(SignedIn);
		});
#endif
	}

#if UNITY_ANDROID
	static void OnSignIn(SignInStatus status)
	{
		bool wasSignedIn = SignedIn;
		SignedIn = status == SignInStatus.Success;
		if (SignedIn && !wasSignedIn)
		{
			Analytics.PlayGamesSignIn(true);
			SubmitAllBestScores();
		}
		else if (!SignedIn)
		{
			Analytics.PlayGamesSignIn(false);
		}
	}
#endif

	// Leaderboards keep the best score, so re-sending local bests is harmless. This also puts
	// the scores of players who played before 1.2.0 on the boards.
	static void SubmitAllBestScores()
	{
		for (int level = 1; level <= LevelProgress.LevelCount; level++)
		{
			int best = PlayersScoreController.GetScore(level);
			if (best > 0) { Report(LeaderboardIds.ForLevel(level), best, null); }
		}
		SubmitTotal();
	}

	public static void SubmitLevelScore(int level, int score, Action<bool> done)
	{
		Report(LeaderboardIds.ForLevel(level), score, done);
		SubmitTotal();
	}

	static void SubmitTotal()
	{
		long total = LevelProgress.TotalBestScore();
		if (total > 0) { Report(LeaderboardIds.Total, total, null); }
	}

	static void Report(string leaderboardId, long score, Action<bool> done)
	{
		if (!SignedIn || string.IsNullOrEmpty(leaderboardId)) { done?.Invoke(false); return; }
#if UNITY_ANDROID
		PlayGamesPlatform.Instance.ReportScore(score, leaderboardId, ok => done?.Invoke(ok));
#endif
	}

	// The player's all-time public rank on a level: done(rank, playerCount), or (0, 0) when
	// it is not available.
	public static void LoadLevelRank(int level, Action<int, int> done)
	{
		string id = LeaderboardIds.ForLevel(level);
		if (!SignedIn || string.IsNullOrEmpty(id)) { done(0, 0); return; }
#if UNITY_ANDROID
		PlayGamesPlatform.Instance.LoadScores(id, LeaderboardStart.PlayerCentered, 1,
			LeaderboardCollection.Public, LeaderboardTimeSpan.AllTime, data =>
			{
				if (data == null || !data.Valid || data.PlayerScore == null) { done(0, 0); return; }
				done(data.PlayerScore.rank, (int)Math.Min(data.ApproximateCount, int.MaxValue));
			});
#endif
	}

	// level 0 shows the total leaderboard.
	public static void ShowLeaderboard(int level)
	{
		string id = level > 0 ? LeaderboardIds.ForLevel(level) : LeaderboardIds.Total;
		Analytics.LeaderboardOpened(level > 0 ? level.ToString() : "total");
		SignIn(ok =>
		{
			if (!ok) { return; }
#if UNITY_ANDROID
			if (string.IsNullOrEmpty(id)) { PlayGamesPlatform.Instance.ShowLeaderboardUI(); }
			else { PlayGamesPlatform.Instance.ShowLeaderboardUI(id); }
#endif
		});
	}
}
