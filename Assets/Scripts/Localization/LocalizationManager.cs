using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;

public class LocalizationManager : MonoBehaviour
{
	public static LocalizationManager instance;

	private Dictionary<string, string> localizedText;
	private bool isReady;
	private string missingTextString = "Localized text not found";

	void Awake() 
	{
		if (instance == null) 
		{
			instance = this;
		} else if(instance != this)
		{
			Destroy(gameObject);
		}

		DontDestroyOnLoad(gameObject);
	}

    void Start()
    {
        
    }

	public IEnumerator LoadLocalizedTextAndroid(string filename) {
		localizedText = new Dictionary<string, string>();
		string filePath = "";
		string dataAsJson = "";
		bool fileExists = false;

		filePath = Path.Combine("jar:file://" + Application.dataPath + "!/assets", filename);
		UnityWebRequest reader = UnityWebRequest.Get(filePath);
		yield return reader.SendWebRequest(); 
		if (!reader.isNetworkError && !reader.isHttpError){
		   dataAsJson = reader.downloadHandler.text;
           fileExists = true;
        }       		

		LoadDataFromFile(dataAsJson, fileExists);
	}

	public void LoadLocalizedText(string filename) {
		localizedText = new Dictionary<string, string>();
		string filePath = "";
		string dataAsJson = "";
		bool fileExists = false;

		filePath = Path.Combine(Application.streamingAssetsPath, filename);
		if (File.Exists(filePath)) {
			dataAsJson = File.ReadAllText(filePath);
			fileExists = true;
		}

		LoadDataFromFile(dataAsJson, fileExists);
	}


	private void LoadDataFromFile(string dataAsJson, bool fileExists) {
		if (fileExists) {
			LocalizationData loadedData = JsonUtility.FromJson<LocalizationData>(dataAsJson);
			for (int i = 0; i < loadedData.items.Length; i++) {
				localizedText.Add(loadedData.items[i].key, loadedData.items[i].value);
			}

			Debug.Log("Data loaded, dictionary contains: " + localizedText.Count + " entries");
		} else {
			Debug.LogError("Cannot find file!");
		}

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
}
