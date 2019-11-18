using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdManager : MonoBehaviour
{
	[SerializeField] string gameID = "3365823";
    
    void Awake()
    {
		DontDestroyOnLoad(this);
		Advertisement.Initialize(gameID, true);
	}

	public void ShowAdd() 
	{
#if UNITY_EDITOR
		StartCoroutine(WaitForAd());
#endif

		if (Advertisement.IsReady()) 
			Advertisement.Show();
	}

	IEnumerator WaitForAd() 
	{
		float currentTimeScale = Time.timeScale;
		Time.timeScale = 0f;
		yield return null;

		while (Advertisement.isShowing)
			yield return null;

		Time.timeScale = currentTimeScale;
	}
}
