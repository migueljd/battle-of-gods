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

		public Image heroImg;
		public Image enemyImg;
		public Text heroAttack;
		public Text enemyAttack;
		public Text heroDefense;
		public Text enemyDefense;


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
			if (unit != null) {
				endTurnButtonObj.SetActive (true);
				heroImg.sprite = unit.iconSprite;
				heroAttack.text = unit.GetEffectiveDamage().ToString();
				heroDefense.text = unit.GetEffectiveHP().ToString();
			}
			else endTurnButtonObj.SetActive(false);
		}

		void OnHostileSelected(Tile selectedTile, Tile hoveredTile){
			Unit enemy = hoveredTile.unit;
			if(enemy != null){
				enemyImg.sprite = enemy.iconSprite;
				enemyAttack.text = enemy.GetEffectiveDamage().ToString();
				enemyDefense.text = enemy.GetEffectiveHP().ToString();
			}
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