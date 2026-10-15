using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Added to every level scene at load (see GameServices): pause button and menu, the
// level-1 tutorial, the marking of the two lawn columns that cannot be planted, the star
// rating and leaderboard rank on the win panel, and the rewarded "continue" on the lose panel.
public class LevelHud : MonoBehaviour
{
	public static LevelHud Instance { get; private set; }

	const string TUTORIAL_DONE_KEY = "tutorial done";

	int level;
	LevelController controller;
	LivesDisplay livesDisplay;
	LevelLoader loader;
	GameObject winPanel;
	GameObject losePanel;
	Button buttonTemplate;
	Text textTemplate;
	Sprite starSprite;

	GameObject pausePanel;
	Button continueButton;
	Text loseNote;
	bool paused;
	bool loseShown;
	bool overlayOpen;
	float timeScaleBeforePause = 1f;

	void Awake()
	{
		Instance = this;
	}

	void OnDestroy()
	{
		if (Instance == this) { Instance = null; }
	}

	IEnumerator Start()
	{
		level = SceneManager.GetActiveScene().buildIndex - 3;
		controller = FindAnyObjectByType<LevelController>();
		livesDisplay = FindAnyObjectByType<LivesDisplay>();
		loader = FindAnyObjectByType<LevelLoader>();
		if (controller == null || livesDisplay == null || loader == null) { yield break; }

		LevelRun.Begin(level);

		winPanel = controller.WinPanel;
		losePanel = controller.LosePanel;
		buttonTemplate = RuntimeUI.FindChild<Button>(losePanel, "Try Again Button");
		textTemplate = RuntimeUI.FindChild<Text>(losePanel, "Level Lost Text");
		StarDisplay starDisplay = FindAnyObjectByType<StarDisplay>();
		Canvas mainCanvas = starDisplay != null ? starDisplay.GetComponentInParent<Canvas>() : null;
		Image starIcon = mainCanvas != null ? RuntimeUI.FindChild<Image>(mainCanvas.gameObject, "Star icon") : null;
		starSprite = starIcon != null ? starIcon.sprite : null;

		BlockedColumns.Create(level);
		if (buttonTemplate == null || textTemplate == null || mainCanvas == null) { yield break; }

		Button pauseButton = RuntimeUI.CloneButton(buttonTemplate, mainCanvas.transform, "II",
			new Vector2(850f, 470f), Pause);
		((RectTransform)pauseButton.transform).sizeDelta = new Vector2(120f, 110f);
		// The template is a see-through grey box made for the dark lose panel; on the lawn it
		// needs a solid colour to read as a button.
		Image pauseImage = pauseButton.GetComponent<Image>();
		if (pauseImage != null) { pauseImage.color = new Color(0.33f, 0.13f, 0.5f, 0.92f); }
		Text pauseLabel = pauseButton.GetComponentInChildren<Text>(true);
		if (pauseLabel != null)
		{
			pauseLabel.fontSize = 60;
			pauseLabel.fontStyle = FontStyle.Bold;
			pauseLabel.color = Color.white;
		}

		// Let the scene finish its own Start (it hides the panels) before opening anything.
		yield return null;
		if (level == 1 && PlayerPrefs.GetInt(TUTORIAL_DONE_KEY) == 0)
		{
			ShowTutorial();
		}
	}

	void Update()
	{
		// Android back button.
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (paused) { Resume(); } else { Pause(); }
		}
	}

	void OnApplicationPause(bool pausing)
	{
		if (pausing) { Pause(); }
	}

	// True while the game must ignore taps on the lawn.
	public bool BlocksInput
	{
		get { return paused || overlayOpen || loseShown || Time.timeScale == 0f; }
	}

	// --- Pause ------------------------------------------------------------------------

	void Pause()
	{
		if (paused || overlayOpen || loseShown || controller == null || controller.GetGameEnded()) { return; }
		if (pausePanel == null) { BuildPausePanel(); }
		if (pausePanel == null) { return; }
		paused = true;
		timeScaleBeforePause = Time.timeScale;
		Time.timeScale = 0f;
		AudioListener.pause = true;
		pausePanel.SetActive(true);
	}

	void Resume()
	{
		if (!paused) { return; }
		paused = false;
		pausePanel.SetActive(false);
		AudioListener.pause = false;
		Time.timeScale = timeScaleBeforePause;
	}

	void BuildPausePanel()
	{
		if (losePanel == null) { return; }
		pausePanel = RuntimeUI.Clone(losePanel, losePanel.transform.parent, "Pause Canvas");
		pausePanel.SetActive(false);
		Text title = RuntimeUI.FindChild<Text>(pausePanel, "Level Lost Text");
		Button restart = RuntimeUI.FindChild<Button>(pausePanel, "Try Again Button");
		Button menu = RuntimeUI.FindChild<Button>(pausePanel, "Main Menu");
		if (title != null)
		{
			title.text = UiText.Paused;
			((RectTransform)title.transform).anchoredPosition = new Vector2(0f, 250f);
		}
		if (restart != null)
		{
			RuntimeUI.CloneButton(restart, restart.transform.parent, UiText.Resume, new Vector2(0f, 60f), Resume);
			RuntimeUI.Rewire(restart, UiText.Restart, () => Leave("restart", loader.RestartScene));
			((RectTransform)restart.transform).anchoredPosition = new Vector2(0f, -125f);
		}
		if (menu != null)
		{
			RuntimeUI.Rewire(menu, UiText.Get("main_menu", "Main Menu"), () => Leave("menu", loader.LoadMainMenuNormally));
			((RectTransform)menu.transform).anchoredPosition = new Vector2(0f, -310f);
		}
	}

	void Leave(string how, System.Action go)
	{
		Analytics.LevelQuit(level, how, LevelRun.Duration);
		AudioListener.pause = false;
		Time.timeScale = 1f;
		go();
	}

	// --- Tutorial (level 1, first time) -----------------------------------------------------

	void ShowTutorial()
	{
		if (controller.GetGameEnded()) { return; }
		GameObject panel = RuntimeUI.Clone(losePanel, losePanel.transform.parent, "Tutorial Canvas");
		Text text = RuntimeUI.FindChild<Text>(panel, "Level Lost Text");
		Button next = RuntimeUI.FindChild<Button>(panel, "Try Again Button");
		Button menu = RuntimeUI.FindChild<Button>(panel, "Main Menu");
		Image background = RuntimeUI.FindChild<Image>(panel, "Background");
		if (text == null || next == null) { Destroy(panel); return; }
		if (menu != null) { menu.gameObject.SetActive(false); }
		if (background != null)
		{
			Color c = background.color;
			background.color = new Color(c.r, c.g, c.b, Mathf.Max(c.a, 0.75f));
		}
		var textRect = (RectTransform)text.transform;
		textRect.anchoredPosition = new Vector2(0f, 120f);
		textRect.sizeDelta = new Vector2(1500f, 420f);
		text.fontSize = 70;
		text.horizontalOverflow = HorizontalWrapMode.Wrap;
		text.verticalOverflow = VerticalWrapMode.Overflow;
		((RectTransform)next.transform).anchoredPosition = new Vector2(0f, -250f);

		string[] steps = UiText.TutorialSteps;
		int step = 0;
		overlayOpen = true;
		Time.timeScale = 0f;
		panel.SetActive(true);

		System.Action show = null;
		show = () =>
		{
			text.text = steps[step];
			Analytics.TutorialStep(step + 1);
			bool last = step == steps.Length - 1;
			RuntimeUI.Rewire(next, last ? UiText.Play : UiText.Next, () =>
			{
				if (!last) { step++; show(); return; }
				PlayerPrefs.SetInt(TUTORIAL_DONE_KEY, 1);
				PlayerPrefs.Save();
				Analytics.TutorialComplete();
				Destroy(panel);
				overlayOpen = false;
				Time.timeScale = 1f;
			});
		};
		show();
	}

	// --- Win ----------------------------------------------------------------------------

	public void ShowResults(LevelResult result)
	{
		if (winPanel == null) { return; }
		Text title = RuntimeUI.FindChild<Text>(winPanel, "Level Complete Text");
		Image banner = RuntimeUI.FindChild<Image>(winPanel, "Background");
		Button next = RuntimeUI.FindChild<Button>(winPanel, "Continue Button");
		Button menu = RuntimeUI.FindChild<Button>(winPanel, "Main Menu");

		Image[] stars = null;
		if (starSprite != null)
		{
			stars = RuntimeUI.StarRow(starSprite, winPanel.transform, new Vector2(0f, 345f), 150f, 190f);
			RuntimeUI.ShowRating(stars, 0);
		}

		Text info = null;
		if (title != null)
		{
			if (banner != null)
			{
				GameObject infoBanner = RuntimeUI.Clone(banner.gameObject, winPanel.transform, "Result Banner");
				var bannerRect = (RectTransform)infoBanner.transform;
				bannerRect.anchoredPosition = new Vector2(0f, -10f);
				bannerRect.sizeDelta = new Vector2(1200f, 160f);
				infoBanner.transform.SetSiblingIndex(banner.transform.GetSiblingIndex() + 1);
			}
			info = RuntimeUI.CloneText(title, winPanel.transform, "Result Text", ResultLine(result, 0, 0),
				new Vector2(0f, -10f), new Vector2(1180f, 160f), 50);
		}

		if (next != null) { ((RectTransform)next.transform).anchoredPosition = new Vector2(0f, -170f); }
		Button template = next != null ? next : buttonTemplate;
		bool showLeaderboard = PlayGames.IsConfigured;
		float rowY = -345f;
		if (menu != null)
		{
			((RectTransform)menu.transform).anchoredPosition = new Vector2(showLeaderboard ? 0f : 260f, rowY);
		}
		if (template != null)
		{
			RuntimeUI.CloneButton(template, winPanel.transform, UiText.Retry,
				new Vector2(showLeaderboard ? -520f : -260f, rowY), loader.RestartScene);
			if (showLeaderboard)
			{
				RuntimeUI.CloneButton(template, winPanel.transform, UiText.Leaderboard,
					new Vector2(520f, rowY), () => PlayGames.ShowLeaderboard(level));
			}
		}

		if (stars != null) { StartCoroutine(RevealStars(stars, result.Rating)); }

		if (!result.Continued)
		{
			PlayGames.SubmitLevelScore(level, result.Score, submitted =>
			{
				if (!submitted || info == null) { return; }
				PlayGames.LoadLevelRank(level, (rank, players) =>
				{
					if (rank > 0 && info != null) { info.text = ResultLine(result, rank, players); }
				});
			});
		}
	}

	static string ResultLine(LevelResult result, int rank, int players)
	{
		if (result.Continued) { return UiText.Score(result.Score) + "\n" + UiText.ContinuedRun; }
		string line = UiText.Score(result.Score) + "   " +
			(result.NewBest ? UiText.NewBest : UiText.Best(result.PreviousBest));
		if (rank > 0) { line += "\n" + UiText.Rank(rank, players); }
		return line;
	}

	IEnumerator RevealStars(Image[] stars, int rating)
	{
		for (int i = 0; i < rating && i < stars.Length; i++)
		{
			yield return new WaitForSecondsRealtime(0.35f);
			stars[i].color = Color.white;
			Transform t = stars[i].transform;
			for (float s = 0f; s < 1f; s += Time.unscaledDeltaTime * 5f)
			{
				t.localScale = Vector3.one * (1f + 0.4f * Mathf.Sin(s * Mathf.PI));
				yield return null;
			}
			t.localScale = Vector3.one;
		}
	}

	// --- Lose ---------------------------------------------------------------------------

	public void OnLevelLost(float progress)
	{
		if (loseShown) { return; }
		loseShown = true;
		if (paused) { Resume(); Time.timeScale = 0f; }
		Analytics.LevelLose(level, LevelRun.Difficulty, LevelRun.DefendersPlaced, LevelRun.Duration,
			progress, LevelRun.Continued);
		if (LevelRun.Continued || !Ads.RewardedReady || losePanel == null || buttonTemplate == null) { return; }

		if (continueButton == null)
		{
			Text title = RuntimeUI.FindChild<Text>(losePanel, "Level Lost Text");
			Button menu = RuntimeUI.FindChild<Button>(losePanel, "Main Menu");
			if (title != null) { ((RectTransform)title.transform).anchoredPosition = new Vector2(0f, 250f); }
			continueButton = RuntimeUI.CloneButton(buttonTemplate, buttonTemplate.transform.parent,
				UiText.ContinueAd, new Vector2(0f, 60f), WatchAdToContinue);
			((RectTransform)buttonTemplate.transform).anchoredPosition = new Vector2(0f, -125f);
			if (menu != null) { ((RectTransform)menu.transform).anchoredPosition = new Vector2(0f, -310f); }
		}
		continueButton.gameObject.SetActive(true);
		continueButton.interactable = true;
		Analytics.ContinueOffered(level);
	}

	void WatchAdToContinue()
	{
		continueButton.interactable = false;
		Ads.ShowRewarded(earned =>
		{
			if (!earned)
			{
				RuntimeUI.SetLabel(continueButton, UiText.NoContinue);
				return;
			}
			LevelRun.MarkContinued();
			Analytics.ContinueUsed(level);
			continueButton.gameObject.SetActive(false);
			losePanel.SetActive(false);
			loseShown = false;
			livesDisplay.AddLife();
			Time.timeScale = 1f;
		});
	}
}
