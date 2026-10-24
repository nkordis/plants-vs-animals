using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Builds the 1.2.0 screens out of copies of the game's existing buttons, texts and star
// icons, so they match the original look without editing the 40 level scenes.
public static class RuntimeUI
{
	public static GameObject Clone(GameObject template, Transform parent, string name)
	{
		GameObject copy = Object.Instantiate(template, parent, false);
		copy.name = name;
		// The originals set their text from the localization files in Start; copies are
		// given their own text instead.
		foreach (LocalizedText localized in copy.GetComponentsInChildren<LocalizedText>(true))
		{
			Object.DestroyImmediate(localized);
		}
		return copy;
	}

	public static Button CloneButton(Button template, Transform parent, string label,
		Vector2 position, UnityAction onClick)
	{
		GameObject copy = Clone(template.gameObject, parent, label + " Button");
		copy.SetActive(true);
		var rect = (RectTransform)copy.transform;
		rect.anchoredPosition = position;
		Button button = copy.GetComponent<Button>();
		button.onClick = new Button.ButtonClickedEvent();
		button.onClick.AddListener(onClick);
		SetLabel(button, label);
		return button;
	}

	public static void Rewire(Button button, string label, UnityAction onClick)
	{
		button.onClick = new Button.ButtonClickedEvent();
		button.onClick.AddListener(onClick);
		SetLabel(button, label);
		foreach (LocalizedText localized in button.GetComponentsInChildren<LocalizedText>(true))
		{
			Object.DestroyImmediate(localized);
		}
	}

	public static void SetLabel(Button button, string label)
	{
		Text text = button.GetComponentInChildren<Text>(true);
		if (text != null) { text.text = label; }
	}

	public static Text CloneText(Text template, Transform parent, string name, string value,
		Vector2 position, Vector2 size, int fontSize)
	{
		GameObject copy = Clone(template.gameObject, parent, name);
		copy.SetActive(true);
		var rect = (RectTransform)copy.transform;
		rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
		rect.anchoredPosition = position;
		rect.sizeDelta = size;
		Text text = copy.GetComponent<Text>();
		text.text = value;
		text.fontSize = fontSize;
		text.alignment = TextAnchor.MiddleCenter;
		text.horizontalOverflow = HorizontalWrapMode.Wrap;
		text.verticalOverflow = VerticalWrapMode.Overflow;
		return text;
	}

	// The game's star sprite as a UI image, e.g. next to the stars-left count.
	public static Image StarIcon(Sprite star, Transform parent, Vector2 center, float size)
	{
		var go = new GameObject("Star Icon", typeof(RectTransform), typeof(Image));
		go.layer = parent.gameObject.layer;
		var rect = (RectTransform)go.transform;
		rect.SetParent(parent, false);
		rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
		rect.sizeDelta = new Vector2(size, size);
		rect.anchoredPosition = center;
		Image image = go.GetComponent<Image>();
		image.sprite = star;
		image.preserveAspect = true;
		image.raycastTarget = false;
		return image;
	}

	public static T FindChild<T>(GameObject root, string name) where T : Component
	{
		foreach (T component in root.GetComponentsInChildren<T>(true))
		{
			if (component.gameObject.name.Trim() == name) { return component; }
		}
		return null;
	}
}
