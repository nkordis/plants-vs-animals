using UnityEngine;
using UnityEngine.SceneManagement;

// Starts analytics, Play Games and ads once per app launch, and adds the 1.2.0 screens
// (pause, tutorial, results, ratings) to scenes as they load, so the scene files of the
// 40 levels stay untouched.
public class GameServices : MonoBehaviour
{
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	static void Bootstrap()
	{
		var host = new GameObject("Game Services");
		DontDestroyOnLoad(host);
		host.AddComponent<GameServices>();
	}

	void Awake()
	{
		Analytics.Init();
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	void Start()
	{
		Ads.Init();
		PlayGames.Init();
	}

	void OnDestroy()
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		AudioListener.pause = false;
		if (scene.name.StartsWith("Level "))
		{
			new GameObject("Level Hud").AddComponent<LevelHud>();
		}
		else if (scene.name == "Choose Level Screen")
		{
			new GameObject("Level Ratings").AddComponent<ChooseLevelRatings>();
		}
		else if (scene.name == "Options Screen")
		{
			new GameObject("Privacy Options").AddComponent<PrivacyOptionsButton>();
		}
	}
}
