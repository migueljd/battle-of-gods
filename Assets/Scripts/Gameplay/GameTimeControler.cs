using UnityEngine;
using System.Collections;
using UnityEngine.UI;

using TBTK;

public class GameTimeControler : MonoBehaviour {

	public Image fastForwardButton;

	private bool buttonPressed = false;
	public float fastTime = 2.0f; 

	private static GameTimeControler instance;

	public AudioClip buttonAudio;

	void Awake(){
		if (instance == null)
			instance = this;
		else
			Destroy (this.gameObject);
	}

	public void buttonPress(){
		AudioManager.PlaySound (buttonAudio);
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

	public static bool IsButtonPressed(){
		return instance.buttonPressed;
	}

	public static float GetFastTime(){
		return instance.fastTime;
	}

}
