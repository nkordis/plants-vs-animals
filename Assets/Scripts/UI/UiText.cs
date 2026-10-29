// Texts set from code (the 1.2.0 screens, the level number). Looked up in the localization files when the key exists
// there, otherwise the English text below is used.
public static class UiText
{
	public static string Get(string key, string english)
	{
		LocalizationManager manager = LocalizationManager.instance;
		if (manager == null || !manager.GetIsReady()) { return english; }
		string value = manager.GetLocalizedValue(key);
		return value == LocalizationManager.MissingText ? english : value;
	}

	public static string Paused { get { return Get("pause_heading", "Paused"); } }
	public static string Resume { get { return Get("pause_resume", "Resume"); } }
	public static string Restart { get { return Get("pause_restart", "Restart"); } }
	public static string NextLevel { get { return Get("next_level", "Next level"); } }
	public static string Retry { get { return Get("win_retry", "Retry"); } }
	public static string Next { get { return Get("tutorial_next", "Next"); } }
	public static string Play { get { return Get("tutorial_play", "Play!"); } }
	public static string Privacy { get { return Get("privacy_options", "Privacy choices"); } }
	public static string PrivacyPolicy { get { return Get("privacy_policy", "Privacy policy"); } }
	public static string CannotPlant { get { return Get("cannot_plant", "Plants can't grow here"); } }

	public static string Score(int score) { return Get("win_score", "Score") + ": " + score; }
	public static string NewBest { get { return Get("win_new_best", "New best!"); } }
	public static string Best(int best) { return Get("win_best", "Best") + ": " + best; }

	public static string[] TutorialSteps
	{
		get
		{
			return new[]
			{
				Get("tutorial_1", "Hungry animals are coming for your garden! Stop them before they reach the left side."),
				Get("tutorial_2", "Pick a plant from the bar, then tap a square of the lawn to plant it."),
				Get("tutorial_3", "Plants cost stars. Mushrooms make more stars, so plant some early."),
				Get("tutorial_4", "Every animal that gets through costs a life. Hold on until the timer runs out to win."),
				Get("tutorial_5", "Your score is the stars you have left at the end. Save as many as you can!"),
			};
		}
	}
}
