using System;
using System.Collections.Generic;
using UnityEngine;
#if PVA_FIREBASE
using Firebase;
using Firebase.Analytics;
using Firebase.Crashlytics;
using Firebase.Extensions;
#endif

// Firebase Analytics + Crashlytics. Every call is safe before Firebase is ready or when it
// is missing (no google-services.json): events are then only written to the log.
public static class Analytics
{
	static bool ready;
	static readonly List<KeyValuePair<string, Dictionary<string, object>>> pending =
		new List<KeyValuePair<string, Dictionary<string, object>>>();

	public static void Init()
	{
#if PVA_FIREBASE && !UNITY_EDITOR
		FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
		{
			if (task.Result != DependencyStatus.Available)
			{
				Debug.LogWarning("Firebase unavailable: " + task.Result);
				return;
			}
			try
			{
				FirebaseApp app = FirebaseApp.DefaultInstance;
				Crashlytics.ReportUncaughtExceptionsAsFatal = true;
				ready = app != null;
			}
			catch (Exception e)
			{
				Debug.LogWarning("Firebase init failed: " + e.Message);
				return;
			}
			foreach (var e in pending) { Send(e.Key, e.Value); }
			pending.Clear();
		});
#endif
	}

	// --- Game events ------------------------------------------------------------------

	public static void LevelStart(int level, float difficulty)
	{
		Log("level_start", "level", level, "difficulty", Mathf.RoundToInt(difficulty));
		SetCrashKey("level", level.ToString());
	}

	public static void LevelWin(int level, float difficulty, int score, bool newBest,
		int livesLost, bool continued, int defenders, float seconds)
	{
		Log("level_win", "level", level, "difficulty", Mathf.RoundToInt(difficulty),
			"score", score, "new_best", newBest ? 1 : 0,
			"lives_lost", livesLost, "continued", continued ? 1 : 0,
			"defenders", defenders, "seconds", Mathf.RoundToInt(seconds));
	}

	public static void LevelLose(int level, float difficulty, int defenders, float seconds,
		float progress, bool continued)
	{
		Log("level_lose", "level", level, "difficulty", Mathf.RoundToInt(difficulty),
			"defenders", defenders, "seconds", Mathf.RoundToInt(seconds),
			"progress_pct", Mathf.RoundToInt(progress * 100f), "continued", continued ? 1 : 0);
	}

	public static void LevelQuit(int level, string how, float seconds)
	{
		Log("level_quit", "level", level, "how", how, "seconds", Mathf.RoundToInt(seconds));
	}

	public static void ContinueOffered(int level) { Log("continue_offered", "level", level); }

	public static void ContinueUsed(int level) { Log("continue_used", "level", level); }

	public static void TutorialStep(int step) { Log("tutorial_step", "step", step); }

	public static void TutorialComplete() { Log("tutorial_complete"); }

	public static void LeaderboardOpened(string which) { Log("leaderboard_open", "which", which); }

	public static void PlayGamesSignIn(bool success) { Log("pgs_sign_in", "success", success ? 1 : 0); }

	public static void BlockedTileTapped(int level) { Log("blocked_tile_tap", "level", level); }

	// --- Plumbing -----------------------------------------------------------------------

	public static void Log(string name, params object[] keyValues)
	{
		var parameters = new Dictionary<string, object>();
		for (int i = 0; i + 1 < keyValues.Length; i += 2)
		{
			parameters[(string)keyValues[i]] = keyValues[i + 1];
		}
		if (!ready)
		{
#if PVA_FIREBASE && !UNITY_EDITOR
			if (pending.Count < 50)
			{
				pending.Add(new KeyValuePair<string, Dictionary<string, object>>(name, parameters));
			}
#endif
			Debug.Log("[analytics] " + name);
			return;
		}
		Send(name, parameters);
	}

	static void Send(string name, Dictionary<string, object> parameters)
	{
#if PVA_FIREBASE
		var list = new List<Parameter>();
		foreach (var p in parameters)
		{
			if (p.Value is string s) { list.Add(new Parameter(p.Key, s)); }
			else if (p.Value is float f) { list.Add(new Parameter(p.Key, (double)f)); }
			else { list.Add(new Parameter(p.Key, Convert.ToInt64(p.Value))); }
		}
		FirebaseAnalytics.LogEvent(name, list.ToArray());
#endif
	}

	static void SetCrashKey(string key, string value)
	{
#if PVA_FIREBASE && !UNITY_EDITOR
		if (ready) { Crashlytics.SetCustomKey(key, value); }
#endif
	}
}
