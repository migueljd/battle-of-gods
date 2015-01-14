using UnityEngine;
using UnityEngine.UI;

using System.Collections;

using TBTK;

namespace TBTK{

	public class UIGameOver : MonoBehaviour {
		
		public Text labelTitle;
		
		private static UIGameOver instance;
		
		private GameObject thisObj;
		
		void Start(){
			instance=this;
			thisObj=gameObject;
			
			transform.localPosition=Vector3.zero;
			
			_Hide();
		}
		
		
		public void OnNextLevelButton(){
			Time.timeScale=1;
			GameControl.LoadNextScene();
		}
		public void OnRestartButton(){
			Time.timeScale=1;
			Application.LoadLevel(Application.loadedLevelName);
		}
		public void OnMainMenuButton(){
			Time.timeScale=1;
			GameControl.LoadMainMenu();
		}
		
		
		void UpdateDisplay(int factionID){
			Faction fac=FactionManager.GetFaction(factionID);
			if(!fac.isPlayerFaction) labelTitle.text="Game Over";
			else{
				if(FactionManager.GetPlayerFactionCount()==1) labelTitle.text="Victory!";
				else	labelTitle.text="Player "+factionID+" Wins!";
			}
		}
		
		
		public static bool isOn=true;
		public static void Show(int factionID){ instance._Show(factionID); }
		public void _Show(int factionID){
			UpdateDisplay(factionID);
			
			isOn=true;
			thisObj.SetActive(isOn);
		}
		public static void Hide(){ instance._Hide(); }
		public void _Hide(){
			isOn=false;
			thisObj.SetActive(isOn);
		}
		
		
	}

}