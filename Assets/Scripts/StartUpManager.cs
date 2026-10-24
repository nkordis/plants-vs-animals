using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartUpManager : MonoBehaviour
{

	private IEnumerator Start()
    {
		// Device language, English when there is no translation for it.
		string language = LocalizationManager.LanguageFor(Application.systemLanguage);

		// Load once and wait; the old loop started a new load every frame until ready.
#if UNITY_ANDROID && !UNITY_EDITOR
		yield return StartCoroutine(LocalizationManager.instance.LoadLocalizedTextAndroid(language));
#else
		LocalizationManager.instance.LoadLocalizedText(language);
		yield return null;
#endif

		SceneManager.LoadScene(1);
    }

    
}
