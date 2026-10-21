using UnityEngine;
using UnityEngine.UI;

// Privacy buttons on the Options screen, in the free row above Defaults and Back.
// "Privacy policy" is always shown (Families policy: the policy must be reachable in the app).
// "Privacy choices" reopens Google's consent form, only for players in regions where it applies.
public class PrivacyOptionsButton : MonoBehaviour
{
	public const string PolicyUrl = "https://doc-hosting.flycricket.io/plants-vs-animals-privacy-policy/a2969790-5227-4195-b213-7a9ce096d751/privacy";

	void Start()
	{
		Button defaults = null;
		foreach (Button button in FindObjectsByType<Button>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
		{
			if (button.gameObject.name.Trim() == "Defaults Button") { defaults = button; break; }
		}
		if (defaults == null) { return; }
		Vector2 position = ((RectTransform)defaults.transform).anchoredPosition;
		RuntimeUI.CloneButton(defaults, defaults.transform.parent, UiText.PrivacyPolicy,
			position + new Vector2(0f, 190f), () => Application.OpenURL(PolicyUrl));
		if (!Ads.PrivacyOptionsRequired) { return; }
		// Same x as the Back button, which sits 744 to the right of Defaults.
		RuntimeUI.CloneButton(defaults, defaults.transform.parent, UiText.Privacy,
			position + new Vector2(744f, 190f), () => Ads.ShowPrivacyOptions(null));
	}
}
