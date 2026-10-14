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

	// A row of three rating stars. Earned stars use the game's star sprite in full colour,
	// missing ones are dimmed.
	public static Image[] StarRow(Sprite star, Transform parent, Vector2 center, float size,
		float spacing)
	{
		var images = new Image[3];
		for (int i = 0; i < 3; i++)
		{
			var go = new GameObject("Rating Star " + (i + 1), typeof(RectTransform), typeof(Image));
			go.layer = parent.gameObject.layer;
			var rect = (RectTransform)go.transform;
			rect.SetParent(parent, false);
			rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
			rect.sizeDelta = new Vector2(size, size);
			rect.anchoredPosition = center + new Vector2((i - 1) * spacing, i == 1 ? size * 0.15f : 0f);
			Image image = go.GetComponent<Image>();
			image.sprite = star;
			image.preserveAspect = true;
			image.raycastTarget = false;
			images[i] = image;
		}
		return images;
	}

	public static void ShowRating(Image[] stars, int rating)
	{
		for (int i = 0; i < stars.Length; i++)
		{
			stars[i].color = i < rating ? Color.white : new Color(0.15f, 0.15f, 0.15f, 0.45f);
		}
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
