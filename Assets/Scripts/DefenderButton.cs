using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DefenderButton : MonoBehaviour {

    [SerializeField] Defender defenderPrefab;
	[SerializeField] AudioClip defenderSelectSound;

	AudioSource audioSource;

    private void Start()
    {
        LabelButtonWithCost();
		audioSource = GetComponent<AudioSource>();
    }

    private void LabelButtonWithCost()
    {
        Text costText = GetComponentInChildren<Text>();
        if (!costText)
        {
            //Debug.LogError(name + " has no cost text, add some!");
        }
        else
        {
            costText.text = defenderPrefab.GetStarCost().ToString();
        }
    }

    private void OnMouseDown()
    {
        var buttons = FindObjectsOfType<DefenderButton>();
        foreach(DefenderButton button in buttons)
        {
            button.GetComponent<SpriteRenderer>().color = new Color32(140, 140, 140, 255);
        }



		if (FindObjectOfType<DefenderSpawner>().GetSelectedDefender() != defenderPrefab) {
		
			audioSource.PlayOneShot(defenderSelectSound);
		}

		GetComponent<SpriteRenderer>().color = Color.white;
		FindObjectOfType<DefenderSpawner>().SetSelectedDefender(defenderPrefab);

	}

}
