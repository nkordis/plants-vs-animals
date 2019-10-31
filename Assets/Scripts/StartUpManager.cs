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
		//Debug.Log(Application.systemLanguage.ToString());
		string fileName = englishFileName;

		if (Application.systemLanguage == SystemLanguage.English) {

			

		} else if (Application.systemLanguage == SystemLanguage.English) {
			
			fileName = spanishFileName;
		}

		while (!LocalizationManager.instance.GetIsReady()) 
		{
			yield return null;
			FindObjectOfType<LocalizationManager>().LoadLocalizedText(fileName);
		}

			SceneManager.LoadScene(1);
    }

    
}
