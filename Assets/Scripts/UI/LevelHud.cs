using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Added to every level scene at load (see GameServices): pause button and menu, the
// level-1 tutorial, the marking of the two lawn columns that cannot be planted, the score
// (stars left) and best on the win panel, and the end-of-level ad from level 10.
public class LevelHud : MonoBehaviour
{
	public static LevelHud Instance { get; private set; }


	int level;
	LevelController controller;
	LevelLoader loader;
	GameObject winPanel;
	GameObject losePanel;
	Button buttonTemplate;
	Text textTemplate;
	Sprite starSprite;

	GameObject pausePanel;
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
		loader = FindAnyObjectByType<LevelLoader>();
		if (controller == null || loader == null) { yield break; }

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
		Button loseMenu = RuntimeUI.FindChild<Button>(losePanel, "Main Menu");
		if (buttonTemplate != null)
		{
			RuntimeUI.Rewire(buttonTemplate, UiText.Get("try_again", "Play Again"),
				() => AfterLevelAd(loader.RestartScene));
		}
		if (loseMenu != null)
		{
			RuntimeUI.Rewire(loseMenu, UiText.Get("main_menu", "Main Menu"),
				() => AfterLevelAd(loader.LoadMainMenuNormally));
		}
		if (buttonTemplate == null || textTemplate == null || mainCanvas == null) { yield break; }

		BuildPauseButton(mainCanvas.transform);

		// Let the scene finish its own Start (it hides the panels) before opening anything.
		yield return null;
		if (level == 1) { ShowTutorial(); }
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

	// A light square with the usual two dark bars, drawn with images rather than a text
	// label. x 760 keeps it on screen on 16:10 and 4:3 devices, which crop the sides of the
	// 1920-wide canvas.
	void BuildPauseButton(Transform canvas)
	{
		Button pauseButton = RuntimeUI.CloneButton(buttonTemplate, canvas, "",
			new Vector2(760f, 440f), Pause);
		pauseButton.transition = Selectable.Transition.None;
		((RectTransform)pauseButton.transform).sizeDelta = new Vector2(110f, 110f);
		Image square = pauseButton.GetComponent<Image>();
		if (square != null) { square.color = new Color(1f, 1f, 1f, 0.9f); }
		foreach (Text label in pauseButton.GetComponentsInChildren<Text>(true))
		{
			label.gameObject.SetActive(false);
		}
		foreach (float x in new[] { -18f, 18f })
		{
			var bar = new GameObject("Pause Bar", typeof(RectTransform), typeof(Image));
			bar.layer = pauseButton.gameObject.layer;
			var rect = (RectTransform)bar.transform;
			rect.SetParent(pauseButton.transform, false);
			rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
			rect.sizeDelta = new Vector2(20f, 60f);
			rect.anchoredPosition = new Vector2(x, 0f);
			Image image = bar.GetComponent<Image>();
			image.color = new Color(0.2f, 0.07f, 0.28f, 1f);
			image.raycastTarget = false;
		}
	}

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

	// --- Tutorial (every start of level 1) ---------------------------------------------------

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

		// The score is the stars left at the end of the level: a big star and the count.
		Image star = null;
		if (starSprite != null)
		{
			star = RuntimeUI.StarIcon(starSprite, winPanel.transform, new Vector2(-110f, 345f), 150f);
		}

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
			RuntimeUI.CloneText(title, winPanel.transform, "Result Text", ResultLine(result),
				new Vector2(0f, -10f), new Vector2(1180f, 160f), 50);
			RuntimeUI.CloneText(title, winPanel.transform, "Stars Left Text", result.Score.ToString(),
				new Vector2(star != null ? 80f : 0f, 345f), new Vector2(300f, 170f), 120);
		}

		// The scene hides these two because the original game moved on by itself after a win.
		if (next != null)
		{
			next.gameObject.SetActive(true);
			RuntimeUI.Rewire(next, UiText.NextLevel, () => AfterLevelAd(loader.LoadNextScene));
			((RectTransform)next.transform).anchoredPosition = new Vector2(0f, -170f);
		}
		Button template = next != null ? next : buttonTemplate;
		float rowY = -345f;
		if (menu != null)
		{
			menu.gameObject.SetActive(true);
			RuntimeUI.Rewire(menu, UiText.Get("main_menu", "Main Menu"),
				() => AfterLevelAd(loader.LoadMainMenuNormally));
			((RectTransform)menu.transform).anchoredPosition = new Vector2(260f, rowY);
		}
		if (template != null)
		{
			RuntimeUI.CloneButton(template, winPanel.transform, UiText.Retry,
				new Vector2(-260f, rowY), () => AfterLevelAd(loader.RestartScene));
		}

		if (star != null) { StartCoroutine(Pop(star.transform)); }
	}

	// From level 10, leaving a finished level (won or lost) plays an interstitial first, as
	// the original game did after level 10.
	const int FirstLevelWithAds = 10;
	bool leaving;

	void AfterLevelAd(System.Action go)
	{
		if (leaving) { return; }
		leaving = true;
		if (level < FirstLevelWithAds) { go(); return; }
		Ads.ShowInterstitial(() => go());
	}

	static string ResultLine(LevelResult result)
	{
		string line = UiText.Score(result.Score) + "   " +
			(result.NewBest ? UiText.NewBest : UiText.Best(result.PreviousBest));
		return line;
	}

	IEnumerator Pop(Transform t)
	{
		yield return new WaitForSecondsRealtime(0.3f);
		for (float s = 0f; s < 1f; s += Time.unscaledDeltaTime * 4f)
		{
			t.localScale = Vector3.one * (1f + 0.4f * Mathf.Sin(s * Mathf.PI));
			yield return null;
		}
		t.localScale = Vector3.one;
	}

	// --- Lose ---------------------------------------------------------------------------

	public void OnLevelLost(float progress)
	{
		if (loseShown) { return; }
		loseShown = true;
		if (paused) { Resume(); Time.timeScale = 0f; }
		Analytics.LevelLose(level, LevelRun.Difficulty, LevelRun.DefendersPlaced, LevelRun.Duration,
			progress);
	}
}
