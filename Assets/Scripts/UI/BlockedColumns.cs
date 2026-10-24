using System.Collections;
using UnityEngine;

// The lawn is 9 columns wide but plants can only go in columns 1-7 (the Core Game Area
// collider). Players tapped columns 8-9 and thought the game was broken, so a tap there
// briefly tints them and says why nothing happens. They look as before otherwise (Nikos,
// 2026-10-24). Planting rules are unchanged.
public class BlockedColumns : MonoBehaviour
{
	const float LeftEdge = 7.5f;
	const float RightEdge = 9.5f;
	const float Bottom = 0.5f;
	const float Top = 5.5f;
	static readonly Color Clear = new Color(0f, 0f, 0f, 0f);

	int level;
	SpriteRenderer shade;
	TextMesh message;
	bool reported;
	Coroutine flashing;

	public static void Create(int level)
	{
		var go = new GameObject("Blocked Columns");
		go.transform.position = new Vector3((LeftEdge + RightEdge) / 2f, (Bottom + Top) / 2f, 0f);
		go.AddComponent<BlockedColumns>().level = level;
	}

	void Start()
	{
		var size = new Vector2(RightEdge - LeftEdge, Top - Bottom);
		Texture2D texture = Texture2D.whiteTexture;
		var shadeObject = new GameObject("Shade");
		shadeObject.transform.SetParent(transform, false);
		shadeObject.transform.localScale = new Vector3(size.x, size.y, 1f);
		shade = shadeObject.AddComponent<SpriteRenderer>();
		// One world unit wide, stretched to the two columns by the scale above.
		shade.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
			new Vector2(0.5f, 0.5f), texture.width);
		shade.color = Clear;
		// Above the background canvas (order 0), below plants and animals (5-6).
		shade.sortingOrder = 1;

		var area = gameObject.AddComponent<BoxCollider2D>();
		area.isTrigger = true;
		area.size = size;

		var label = new GameObject("Message");
		label.transform.SetParent(transform, false);
		label.transform.localPosition = new Vector3(0f, 0f, -0.1f);
		message = label.AddComponent<TextMesh>();
		message.text = UiText.CannotPlant.Replace(" ", "\n");
		message.anchor = TextAnchor.MiddleCenter;
		message.alignment = TextAlignment.Center;
		message.characterSize = 0.06f;
		message.fontSize = 60;
		message.color = Color.white;
		Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
		message.font = font;
		MeshRenderer textRenderer = label.GetComponent<MeshRenderer>();
		textRenderer.sharedMaterial = font.material;
		textRenderer.sortingOrder = 20;
		label.SetActive(false);
	}

	void OnMouseDown()
	{
		LevelHud hud = LevelHud.Instance;
		if (hud != null && hud.BlocksInput) { return; }
		if (!reported)
		{
			reported = true;
			Analytics.BlockedTileTapped(level);
		}
		if (flashing != null) { StopCoroutine(flashing); }
		flashing = StartCoroutine(Flash());
	}

	IEnumerator Flash()
	{
		message.gameObject.SetActive(true);
		shade.color = new Color(0.6f, 0f, 0f, 0.35f);
		yield return new WaitForSeconds(1.5f);
		shade.color = Clear;
		message.gameObject.SetActive(false);
		flashing = null;
	}
}
