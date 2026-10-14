using UnityEngine;
using UnityEngine.UI;

// Google's consent rules require a way to change ad consent later, for players in regions
// where the consent form applies. Shown on the Options screen only for them.
public class PrivacyOptionsButton : MonoBehaviour
{
	void Start()
	{
		if (!Ads.PrivacyOptionsRequired) { return; }
		Button defaults = null;
		foreach (Button button in FindObjectsByType<Button>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
		{
			if (button.gameObject.name.Trim() == "Defaults Button") { defaults = button; break; }
		}
		if (defaults == null) { return; }
		Vector2 position = ((RectTransform)defaults.transform).anchoredPosition;
		RuntimeUI.CloneButton(defaults, defaults.transform.parent, UiText.Privacy,
			position + new Vector2(0f, 190f), () => Ads.ShowPrivacyOptions(null));
	}
}
