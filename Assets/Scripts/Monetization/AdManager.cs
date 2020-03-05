using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;


public class AdManager : MonoBehaviour {
	[SerializeField] string gameID = "3365823";
	[SerializeField] bool testMode = true;
	[SerializeField] string rewardedVideotId = "rewardedVideo";
	[SerializeField] int skipAddRate = 80;

	void Awake() {
		DontDestroyOnLoad(this);
	}

	private void Start() {

		Advertisement.Initialize(gameID, testMode);
	}


	public void ShowAdd() {
#if UNITY_EDITOR
		StartCoroutine(WaitForAd());
#endif

		if (Advertisement.IsReady()) {
			ShowAddsByRate();
		}

	}

	/**
	* Shows video and rewarded video adds depending on the skipAddRate
	* No adds for levels 1-9. No skip adds for levels 30-40.
	* Skip adds rate for levels 10-19. 1-skip addds rate for levels 20-29.
	*/

	private void ShowAddsByRate() {
		System.Random rnd = new System.Random();
		int num = rnd.Next(1, 101);

		if (FindObjectOfType<LevelLoader>().GetCurrentLevel() < 10) {
			
		} else if (FindObjectOfType<LevelLoader>().GetCurrentLevel() > 29) {
			Advertisement.Show(rewardedVideotId);
		} else if (FindObjectOfType<LevelLoader>().GetCurrentLevel() < 20) {
			if (num > skipAddRate) {
				Advertisement.Show(rewardedVideotId);
			} else {
				Advertisement.Show();
			}
		} else {
			if (num < skipAddRate) {
				Advertisement.Show(rewardedVideotId);
			} else {
				Advertisement.Show();
			}
		}

	}

	IEnumerator WaitForAd() {
		float currentTimeScale = Time.timeScale;
		Time.timeScale = 0f;
		yield return null;

		while (Advertisement.isShowing)
			yield return null;

		Time.timeScale = currentTimeScale;
	}
}
