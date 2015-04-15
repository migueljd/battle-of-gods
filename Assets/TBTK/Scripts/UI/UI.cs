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
		public Text heroHP;
		public Text enemyHP;


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
			GridManager.onHostileSelectE +=OnHostileSelected;
			GridManager.onHostileDeselectE += OnHostileDeselect;
			GameControl.onGameOverE += OnGameOver;
			OnHostileDeselect ();
		}
		void OnDisable(){
			FactionManager.onUnitDeploymentPhaseE -= OnUnitDeploymentPhase;
			Unit.onUnitSelectedE -= OnUnitSelected;
			GridManager.onHostileSelectE -=OnHostileSelected;
			GridManager.onHostileDeselectE -= OnHostileDeselect;
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
				heroHP.text = unit.HP.ToString();
				heroAttack.text = (unit.GetEffectiveDamage()+ unit.tile.tileAttack).ToString();
				heroDefense.text = (unit.GetEffectiveGuard() + unit.tile.tileDefense).ToString();
			}
			else endTurnButtonObj.SetActive(false);
		}

		void OnHostileSelected(Unit unit){
			Unit enemy = unit;
			if(enemy != null){
				enemyImg.enabled = true;
				enemyImg.sprite = enemy.iconSprite;
				enemyHP.text = enemy.HP.ToString();
				enemyAttack.text = (enemy.damageMin + unit.tile.tileAttack).ToString();
				enemyDefense.text = (enemy.GetEffectiveGuard() + unit.tile.tileDefense).ToString();
			}
		}

		void OnHostileDeselect(){
			enemyImg.enabled = false;
			enemyAttack.text = "";
			enemyDefense.text = "";
			enemyHP.text = "";

		}

		public static void UpdateUnitInfo(){
			instance.OnUnitSelected ();
		}

		public static void UpdateEnemyInfo(){
			instance.OnHostileSelected();
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