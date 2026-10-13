using UnityEngine;

// Ad integration point. The legacy Unity Ads 2.x SDK was removed for the Google Play
// restoration release (it was also initialised in test mode, so it never earned revenue).
// A current ad SDK plugs in here in a later release.
public class AdManager : MonoBehaviour {

	void Awake() {
		DontDestroyOnLoad(this);
	}

	public void ShowAdd() {
	}
}
