using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LocalizationManager : MonoBehaviour
{
	public static LocalizationManager instance;

	public const string English = "en";
	public const string MissingText = "Localized text not found";

	// The game's font (spacecow) has plain ASCII only: no accents and no Greek letters. These
	// languages switch every text to Comfortaa (SIL Open Font License, see Comfortaa-OFL.txt).
	static readonly string[] needFullFont = { "es", "pt", "el" };
	const string fullFontPath = "Fonts/Comfortaa-Bold";
	// Translations run longer than English, so their texts may shrink down to this share of
	// their size to stay inside the boxes made for the English words.
	const float minFitScale = 0.6f;

	private Dictionary<string, string> localizedText = new Dictionary<string, string>();
	private bool isReady;
	private string missingTextString = MissingText;
	private string language = English;
	private Font fullFont;

	public string Language { get { return language; } }

	void Awake()
	{
		if (instance == null)
		{
			instance = this;
			SceneManager.sceneLoaded += OnSceneLoaded;
		} else if(instance != this)
		{
			Destroy(gameObject);
		}

		DontDestroyOnLoad(gameObject);
	}

	void OnDestroy()
	{
		if (instance == this) { SceneManager.sceneLoaded -= OnSceneLoaded; }
	}

	// The device language, or English when there is no translation for it.
	public static string LanguageFor(SystemLanguage systemLanguage)
	{
		switch (systemLanguage)
		{
			case SystemLanguage.Spanish: return "es";
			case SystemLanguage.Portuguese: return "pt";
			case SystemLanguage.Indonesian: return "id";
			case SystemLanguage.Greek: return "el";
			default: return English;
		}
	}

	static string FileName(string language)
	{
		return "localizedText_" + language + ".json";
	}

	// English is loaded first and the chosen language on top of it, so a key that is missing
	// from a translation still shows in English.
	static List<string> FilesFor(string language)
	{
		var files = new List<string> { FileName(English) };
		if (language != English) { files.Add(FileName(language)); }
		return files;
	}

	public IEnumerator LoadLocalizedTextAndroid(string language) {
		localizedText = new Dictionary<string, string>();
		foreach (string filename in FilesFor(language)) {
			string filePath = Path.Combine(Application.streamingAssetsPath, filename);
			using (UnityWebRequest reader = UnityWebRequest.Get(filePath)) {
				yield return reader.SendWebRequest();
				bool fileExists = reader.result == UnityWebRequest.Result.Success;
				LoadDataFromFile(fileExists ? reader.downloadHandler.text : "", fileExists, filename);
			}
		}

		Finish(language);
	}

	public void LoadLocalizedText(string language) {
		localizedText = new Dictionary<string, string>();
		foreach (string filename in FilesFor(language)) {
			string filePath = Path.Combine(Application.streamingAssetsPath, filename);
			bool fileExists = File.Exists(filePath);
			LoadDataFromFile(fileExists ? File.ReadAllText(filePath) : "", fileExists, filename);
		}

		Finish(language);
	}


	private void LoadDataFromFile(string dataAsJson, bool fileExists, string filename) {
		if (fileExists) {
			LocalizationData loadedData = JsonUtility.FromJson<LocalizationData>(dataAsJson);
			for (int i = 0; i < loadedData.items.Length; i++) {
				localizedText[loadedData.items[i].key] = loadedData.items[i].value;
			}

			Debug.Log(filename + " loaded, dictionary contains: " + localizedText.Count + " entries");
		} else {
			Debug.LogError("Cannot find " + filename);
		}
	}

	private void Finish(string language) {
		this.language = language;
		fullFont = System.Array.IndexOf(needFullFont, language) >= 0 ? Resources.Load<Font>(fullFontPath) : null;
		isReady = true;
	}

	public string GetLocalizedValue(string key)
	{
		string result = missingTextString;
		if (localizedText.ContainsKey(key))
		{
			result = localizedText[key];
		}

		return result;
	}

	public bool GetIsReady()
	{
		return isReady;
	}

	// Runs before any Start in the scene, so texts and the panels the 1.2.0 code later copies
	// already have the right font and fitting.
	void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if (!isReady || language == English) { return; }
		foreach (Text text in FindObjectsByType<Text>(FindObjectsInactive.Include, FindObjectsSortMode.None))
		{
			if (fullFont != null) { text.font = fullFont; }
			if (text.GetComponent<LocalizedText>() != null) { FitText(text); }
		}
	}

	// Lets a translated text shrink to fit its box. Call again after changing its font size.
	public static void FitText(Text text)
	{
		if (instance == null || instance.language == English || text == null) { return; }
		text.resizeTextMaxSize = text.fontSize;
		text.resizeTextMinSize = Mathf.Max(1, Mathf.RoundToInt(text.fontSize * minFitScale));
		text.resizeTextForBestFit = true;
	}
}
