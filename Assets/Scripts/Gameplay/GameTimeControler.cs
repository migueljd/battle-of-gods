using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameTimeControler : MonoBehaviour {

	public Image fastForwardButton;

	private bool buttonPressed = false;
	public float fastTime = 2.0f; 

	public void buttonPress(){
		if(buttonPressed) {
			fastForwardButton.color = Color.white;
			Time.timeScale = 1.0f;
			buttonPressed = false;
		}
		else{
			buttonPressed = true;
			fastForwardButton.color = Color.grey;
			Time.timeScale = fastTime;
		}
	}

}
