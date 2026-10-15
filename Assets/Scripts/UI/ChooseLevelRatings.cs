using UnityEngine;
using UnityEngine.UI;

// Level select additions: the 1-3 star rating above each unlocked level and a
// Leaderboards button (when Play Games is set up).
public class ChooseLevelRatings : MonoBehaviour
{
	void Start()
	{
		ChooseLevelController chooser = FindAnyObjectByType<ChooseLevelController>();
		if (chooser == null) { return; }
		Button[] buttons = chooser.Buttons;

		Image starTemplate = null;
		foreach (Image image in chooser.GetComponentsInChildren<Image>(true))
		{
			if (image.gameObject.name.Trim() == "Stars Image") { starTemplate = image; break; }
		}

		int maxLevel = PlayersMaxLevelController.GetMaxLevel();
		if (starTemplate != null)
		{
			// The last button is the "coming soon" tile.
			for (int i = 0; i < buttons.Length - 1 && i <= maxLevel; i++)
			{
				int rating = LevelProgress.GetRating(i + 1);
				Image[] stars = RuntimeUI.StarRow(starTemplate.sprite, buttons[i].transform,
					new Vector2(0f, 78f), 32f, 34f);
				RuntimeUI.ShowRating(stars, rating);
			}
		}

		if (PlayGames.IsConfigured)
		{
			Button back = null;
			foreach (Button button in FindObjectsByType<Button>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
			{
				if (button.gameObject.name.Trim() == "Back Button") { back = button; break; }
			}
			if (back != null)
			{
				Vector2 position = ((RectTransform)back.transform).anchoredPosition;
				RuntimeUI.CloneButton(back, back.transform.parent, UiText.Leaderboards,
					position + new Vector2(-660f, 0f), () => PlayGames.ShowLeaderboard(0));
			}
		}
	}
}
