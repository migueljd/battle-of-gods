using UnityEngine;
using System.Collections;
using System.Collections.Generic;


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

	public List<Image> fadeImages;
	public Text levelText;
	public float hideLevelNameDuration;

	public Image parentPosition;

	public float levelTextMoveSpeed = 0.1f;

	private static LoadingScreen instance;
	private float timeToHide;

	private bool addedActionAtPassLevel;

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

	void OnEnable(){
		GameControl.onPassLevelE += OnPassLevel;
	}

	void OnDisable(){
		GameControl.onPassLevelE -= OnPassLevel;
	}

	private void UpdateAlpha(){

		if (fadeIn) {
			float t = (Time.time - initialTime) / secondsToFadeIn;
			foreach(Image i in fadeImages) i.color = new Color(i.color.r, i.color.g, i.color.b, Mathf.Lerp ( 0, 1, t));
			if (t >= 1){
				fadeIn = false;
				if(addedActionAtPassLevel){ 
					GameControl.CompleteActionAtPassLevel();
					addedActionAtPassLevel = false;
				}
			
			}
		} else if (fadeOut) {
			float t = (Time.time - initialTime) / secondsToFadeOut;
			foreach(Image i in fadeImages)
				i.color = new Color(i.color.r, i.color.g, i.color.b,  Mathf.Lerp( 1, 0, t));

			if (t >= 1){
				fadeOut = false;
				foreach(Image i in fadeImages) i.enabled = false;
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
		foreach (Image i in instance.fadeImages) {
			i.enabled = true;
		}

	}

	public static void ShowLoadingScreen(){
		instance.levelText.enabled = true;
		instance.levelText.text = "Level " + (MapController.instance != null? MapController.level.ToString () : "1");
	}

	public static void HideLoadingScreen(){
		instance.levelText.rectTransform.SetParent(instance.parentPosition.rectTransform);
		instance.levelText.transform.localScale = new Vector3 (1, 1, 1);
		instance.levelText.fontSize = 30;
		instance.StartCoroutine(instance.MoveLevelText());
	}

	private IEnumerator MoveLevelText(){

		Vector2 initialPosition = instance.levelText.rectTransform.localPosition;


		float interpolate = 0;
		while (Vector2.Distance(instance.levelText.rectTransform.localPosition, Vector2.zero) > 0.01f) {
			instance.levelText.rectTransform.localPosition = Vector2.Lerp(initialPosition, Vector2.zero, interpolate);
			interpolate += levelTextMoveSpeed;
			yield return null;
		}
		
	}

	public void OnPassLevel(){
		GameControl.AddActionAtPassLevel ();

		addedActionAtPassLevel = true;

		FadeIn ();
	}
}
