using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour {

    [Tooltip("Our level timer in SECONDS")]
    [SerializeField] float levelTime = 10;

	[Tooltip("Slider delay")]
	[Range(0.0f, 1.0f)]
	[SerializeField] float delaySlider = 1f;

	bool triggeredLevelFinished = false;

	// Share of the level time that has passed, 0 to 1.
	public float GetProgress()
	{
		return Mathf.Clamp01(Time.timeSinceLevelLoad / levelTime);
	}


	
	void Update ()
    {
        if (triggeredLevelFinished) { GetComponent<Slider>().value = (Time.timeSinceLevelLoad / levelTime) * delaySlider;  return; }
        GetComponent<Slider>().value = (Time.timeSinceLevelLoad  / levelTime) * delaySlider;

        bool timerFinished = (Time.timeSinceLevelLoad >= levelTime);
        if (timerFinished)
        {
            FindAnyObjectByType<LevelController>().LevelTimerFinished();
            triggeredLevelFinished = true;
        }
	}
}
