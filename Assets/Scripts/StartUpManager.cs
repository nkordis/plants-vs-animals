using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartUpManager : MonoBehaviour
{

	const string englishFileName = "localizedText_en.json";
	const string spanishFileName = "localizedText_es.json";

	private IEnumerator Start()
    {
		
		string fileName = englishFileName;
		/*
		if (Application.systemLanguage == SystemLanguage.English) {

			fileName = englishFileName;

		} else if (Application.systemLanguage == SystemLanguage.Spanish) {
			
			fileName = spanishFileName;
		}*/

		// Load once and wait; the old loop started a new load every frame until ready.
#if UNITY_ANDROID && !UNITY_EDITOR
		yield return StartCoroutine(LocalizationManager.instance.LoadLocalizedTextAndroid(fileName));
#else
		LocalizationManager.instance.LoadLocalizedText(fileName);
		yield return null;
#endif

		SceneManager.LoadScene(1);
    }

    
}
