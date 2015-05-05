using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;

using TBTK;

namespace TBTK {

	public class UIPauseMenu : MonoBehaviour {

		private GameObject thisObj;
		private static UIPauseMenu instance;
		
		public Slider sliderMusicVolume;
		public Slider sliderSFXVolume;
		
		public GameObject pauseMenuObj;
		public GameObject optionMenuObj;

		public AudioClip click;
		
		
		void Awake(){
			instance=this;
			thisObj=gameObject;
			
			transform.localPosition=Vector3.zero;
			
			//sliderMusicVolume.value=AudioManager.GetMusicVolume()*100;
			//sliderSFXVolume.value=AudioManager.GetSFXVolume()*100;
		}
		
		// Use this for initialization
		void Start () {
//			OnOptionBackButton();
			optionMenuObj.SetActive (false);
			Hide();
			sliderMusicVolume.value = AudioManager.GetMusicVolume () * 100;
			sliderSFXVolume.value = AudioManager.GetSFXVolume () * 100;
		}
		
		
		public void OnPauseButton(){
			AudioManager.PlaySound (click);
			if(GameControl.GetGamePhase()==_GamePhase.Over) return;
			
			//~ if(gameState==_GameState.Pause){
				//~ GameControl.ResumeGame();
				//~ UIPauseMenu.Hide();
			//~ }
			//~ else{
				//~ GameControl.PauseGame();
				//~ UIPauseMenu.Show();
			//~ }

			pauseMenuObj.SetActive (true);

			if(isOn) Hide();
			else Show();
		}
		
		
		public void OnResumeButton(){
			AudioManager.PlaySound (click);
			Hide();
			//GameControl.ResumeGame();
		}

		bool musicOn;

		public Sprite musicOnSprite;

		public Sprite musicOffSprite;

		public void OnOptionButton(Button button){

			AudioManager.PlaySound (click);


			if (musicOn)
				button.GetComponent<Image>().sprite = musicOffSprite;
			else
				button.GetComponent<Image>().sprite = musicOnSprite;
			musicOn = !musicOn;

//			if(isOn) Hide();
//			else Show();
//			pauseMenuObj.SetActive (false);
//			optionMenuObj.SetActive(true);

		}
		
		public void OnMainMenuButton(){
			Time.timeScale=1;
			GameControl.LoadMainMenu();
		}
		
		
		public static bool isOn=true;
		public static void Show(){ instance._Show(); }
		public void _Show(){
			Cards.CardsHandManager.movingCard = true;
			Time.timeScale=0;
			isOn=true;
			thisObj.SetActive(isOn);
		}
		public static void Hide(){ instance._Hide(); }
		public void _Hide(){
			Cards.CardsHandManager.movingCard = false;

			Time.timeScale= GameTimeControler.IsButtonPressed()? GameTimeControler.GetFastTime(): 1.0f ;
			isOn=false;
			thisObj.SetActive(isOn);
		}
		
		
		public void OnMusicVolumeSlider(){
			if(Time.timeSinceLevelLoad>0.5f)
				AudioManager.SetMusicVolume(sliderMusicVolume.value/100);
		}
		public void OnSFXVolumeSlider(){
			if(Time.timeSinceLevelLoad>0.5f)
				AudioManager.SetSFXVolume(sliderSFXVolume.value/100);
		}
		
		public void OnOptionBackButton(){
			optionMenuObj.SetActive(false);
			AudioManager.PlaySound (click);
			Hide ();
		}

		public void Restart(){
			StartCoroutine(GameControl.RestartGame());
		}
	}


}