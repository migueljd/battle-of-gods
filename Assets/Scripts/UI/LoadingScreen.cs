using UnityEngine;
using System.Collections;

using UnityEngine.UI;

using TBTK;

public class LoadingScreen : MonoBehaviour {

	public float secondsToFadeOut = 0.5f;
	public float secondsToFadeIn = 0.5f;

	public static float staticSecondsToFadeOut = 0.5f;
	public static float staticSecondsToFadeIn = 0.5f;


	private float initialTime;

	private bool fadeOut;
	private bool fadeIn;

	public Image fadeImage;

	private static LoadingScreen instance;

	void Awake(){
		instance = this;
		staticSecondsToFadeIn = this.secondsToFadeIn;
		staticSecondsToFadeOut = this.secondsToFadeOut;
	}

	// Update is called once per frame
	void Update () {
		if (fadeIn || fadeOut) {
			UpdateAlpha();
		}
	}

	private void UpdateAlpha(){
		if (fadeIn) {
			float t = (Time.time - initialTime) / secondsToFadeIn;
			fadeImage.color = Color.Lerp (new Color (0, 0, 0, 0), new Color (0, 0, 0, 1), t);
			if (t >= 1)
				fadeIn = false;
		} else if (fadeOut) {
			float t = (Time.time - initialTime) / secondsToFadeOut;
			fadeImage.color = Color.Lerp (new Color (0, 0, 0, 1), new Color (0, 0, 0, 0), t);
			if (t >= 1){
				fadeOut = false;
				fadeImage.enabled = false;
			}
		}
	}

	public static void FadeOut(){
		instance.fadeOut = true;
		instance.initialTime = Time.time;
	}

	public static void FadeIn(){
		instance.fadeIn = true;
		instance.initialTime = Time.time;
		instance.fadeImage.enabled = true;

	}
}
