using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseLevelController : MonoBehaviour
{

	[SerializeField] Button[] buttons;
	[SerializeField] Sprite comingSoonImage;
	[SerializeField] Sprite defaultImage;
	
	void Start()
    {
		buttons[0].transform.GetChild(3).GetComponent<Image>().enabled = false;
		buttons[0].transform.GetChild(2).GetComponent<Text>().text = PlayersScoreController.GetScore(1).ToString();
		buttons[buttons.Length - 1].GetComponent<Image>().sprite = defaultImage;
		buttons[buttons.Length-1].GetComponent<Image>().color = new Color32(144, 192, 46, 130);
		buttons[buttons.Length-1].transform.GetChild(0).GetComponent<Text>().enabled = false;
        buttons[buttons.Length-1].transform.GetChild(1).GetComponent<Text>().enabled = false;

		for (int i = 1; i < buttons.Length - 1; i++) 
		{
			if (PlayersMaxLevelController.GetMaxLevel() < i) 
			{
				buttons[i].transform.GetChild(1).GetComponent<Image>().enabled = false;
				buttons[i].transform.GetChild(0).GetComponent<Text>().enabled = false;
				buttons[i].transform.GetChild(2).GetComponent<Text>().enabled = false;
				buttons[i].GetComponent<Image>().color = new Color32(144,192,46,130);

				buttons[i].enabled = false; 

			} else 
			{
				buttons[i].transform.GetChild(3).GetComponent<Image>().enabled = false;
				buttons[i].transform.GetChild(2).GetComponent<Text>().text = PlayersScoreController.GetScore(i+1).ToString();
			}
		}

		if (PlayersMaxLevelController.GetMaxLevel() == buttons.Length - 1) {
			buttons[buttons.Length - 1].GetComponent<Image>().sprite = comingSoonImage;
			buttons[buttons.Length - 1].GetComponent<Image>().color = new Color32(255, 255, 255, 105);
			buttons[buttons.Length-1].transform.GetChild(0).GetComponent<Text>().enabled = true;
			buttons[buttons.Length-1].transform.GetChild(1).GetComponent<Text>().enabled = true;
			buttons[buttons.Length-1].transform.GetChild(2).GetComponent<Image>().enabled = false;
		}
		
    }

   
}
