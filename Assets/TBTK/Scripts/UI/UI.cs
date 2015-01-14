using UnityEngine;
using UnityEngine.UI;

using System.Collections;

using TBTK;

namespace TBTK{

	public class UI : MonoBehaviour {

		public float scaleFactor=1;
		public static float GetScaleFactor(){ return instance.scaleFactor; }
		
		public GameObject endTurnButtonObj;
		
		public bool disablePerkMenu=false;
		
		private static UI instance;
		
		void Awake(){
			instance=this;
			transform.position=Vector3.zero;
		}
		
		// Use this for initialization
		void Start () {
			endTurnButtonObj.SetActive(false);
			
			if(disablePerkMenu) UIPerkMenu.Disable();
		}
		
		void OnEnable(){
			FactionManager.onUnitDeploymentPhaseE += OnUnitDeploymentPhase;
			Unit.onUnitSelectedE += OnUnitSelected;
			
			GameControl.onGameOverE += OnGameOver;
		}
		void OnDisable(){
			FactionManager.onUnitDeploymentPhaseE -= OnUnitDeploymentPhase;
			Unit.onUnitSelectedE -= OnUnitSelected;
			
			GameControl.onGameOverE -= OnGameOver;
		}
		
		
		void OnUnitDeploymentPhase(bool flag){
			if(flag) UIUnitDeployment.Show();
			else UIUnitDeployment.Hide();
			//StartCoroutine(_OnUnitDeploymentPhase());
		}
		IEnumerator _OnUnitDeploymentPhase(){
			yield return null;
			
		}
		
		
		void OnUnitSelected(Unit unit){
			if(unit!=null) endTurnButtonObj.SetActive(true);
			else endTurnButtonObj.SetActive(false);
		}
		
		
		public void OnEndTurnButton(){
			GameControl.EndTurn();
		}
		
		
		
		public void OnGameOver(int factionID){ StartCoroutine(ShowGameOverScreen(factionID)); }
		IEnumerator ShowGameOverScreen(int factionID){
			yield return new WaitForSeconds(2);
			UIGameOver.Show(factionID);
		}
		
		
		// Update is called once per frame
		void Update () {
		
		}
		
	}

}