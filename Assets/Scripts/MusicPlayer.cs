using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour {

	const string MASTER_VOLUME_KEY = "master volume";
	AudioSource audioSource;

	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(this);
        audioSource = GetComponent<AudioSource>();
		if (PlayerPrefs.HasKey(MASTER_VOLUME_KEY)) {
			audioSource.volume = PlayerPrefsController.GetMasterVolume();
		} else {
			PlayerPrefsController.SetMasterVolume(0.3f);
			SetVolume(0.3f);
		}
	}
	
    public void SetVolume(float volume)
    {
        audioSource.volume = volume;
    }

}
