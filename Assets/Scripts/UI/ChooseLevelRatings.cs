using UnityEngine;
using UnityEngine.UI;

// Level select addition: a Leaderboards button (when Play Games is set up). Each level
// button already shows its best score, the stars left.
public class ChooseLevelRatings : MonoBehaviour
{
	void Start()
	{
		ChooseLevelController chooser = FindAnyObjectByType<ChooseLevelController>();
		if (chooser == null) { return; }
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
