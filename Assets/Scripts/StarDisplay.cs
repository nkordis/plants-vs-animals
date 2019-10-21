using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarDisplay : MonoBehaviour
{

    [SerializeField] int stars = 100;
    Text starText;
	LevelController levelController;

    void Start()
    {
        starText = GetComponent<Text>();
        UpdateDisplay();
		levelController = FindObjectOfType<LevelController>();
	}

    private void UpdateDisplay()
    {
        starText.text = stars.ToString();
    }

    public bool HaveEnoughStars(int amount)
    {
        return stars >= amount;
    }

    public void AddStars(int amount)
    {
		if (!levelController.GetGameEnded()) {
			stars += amount;
			UpdateDisplay();
		}
    }

    public void SpendStars(int amount)
    {
        if (stars >= amount)
        {
            stars -= amount;
            UpdateDisplay();
        }
    }

	public int GetStars() {
		return stars;
	}
}
