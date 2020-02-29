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

		while (!LocalizationManager.instance.GetIsReady()) 
		{
			yield return null;
#if UNITY_EDITOR || UNITY_IOS
			FindObjectOfType<LocalizationManager>().LoadLocalizedText(fileName);
#elif UNITY_ANDROID
			StartCoroutine(FindObjectOfType<LocalizationManager>().LoadLocalizedTextAndroid(fileName));
#endif
		}

		SceneManager.LoadScene(1);
    }

    
}
