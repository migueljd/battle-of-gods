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
	public Text levelText;
	public float hideLevelNameDuration;

	private static LoadingScreen instance;
	private float timeToHide;

	void Awake(){
		instance = this;
		staticSecondsToFadeIn = this.secondsToFadeIn;
		staticSecondsToFadeOut = this.secondsToFadeOut;
		timeToHide = Mathf.Infinity;
	}

	// Update is called once per frame
	void Update () {
		if (fadeIn || fadeOut) {
			UpdateAlpha();
		}
		if (Time.time >= timeToHide)
			HideLoadingScreen ();
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
		instance.timeToHide = Time.time + instance.hideLevelNameDuration + staticSecondsToFadeOut;
	}

	public static void FadeIn(){
		instance.fadeIn = true;
		instance.initialTime = Time.time;
		instance.fadeImage.enabled = true;

	}

	public static void ShowLoadingScreen(){
		instance.levelText.enabled = true;
		instance.levelText.text = "Level " + MapController.level.ToString ();
	}

	public static void HideLoadingScreen(){
		instance.levelText.enabled = false;
	}
}
