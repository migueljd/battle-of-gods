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

		public GameObject unitSelectedArrow;
		public GameObject enemySelectedArrow;

		public float height;

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
//				Vector3 screenPos = mainCam.WorldToScreenPoint(overlay.unit.thisT.position+new Vector3(0, 0, 0));
//				overlay.rootT.localPosition=(screenPos+new Vector3(0, -20, 0))/UI.GetScaleFactor();

				unitSelectedArrow.GetComponent<SelectorAnimator>().unit = unit;
			}
			else endTurnButtonObj.SetActive(false);
		}

		private void OnHostileSelected(Unit unit){
			Unit enemy = unit;
			if(enemy != null){
				enemyImg.enabled = true;
				enemyImg.sprite = enemy.iconSprite;
				enemyHP.text = enemy.HP.ToString();
				enemyAttack.text = ((int)(enemy.damageMin + unit.tile.tileAttack)).ToString();
				enemyDefense.text = (enemy.GetEffectiveGuard() + unit.tile.tileDefense).ToString();

				//enemySelectedArrow.SetActive(true);
				//enemySelectedArrow.GetComponent<SelectorAnimator>().unit = unit;
			}
		}

		private void OnHostileDeselect(){
			enemyImg.enabled = false;
			enemyAttack.text = "";
			enemyDefense.text = "";
			enemyHP.text = "";
			enemySelectedArrow.GetComponent<SelectorAnimator> ().unit = null;
			enemySelectedArrow.SetActive(false) ;

		}

		public static void UpdateUnitInfo(Unit unit){
			instance.OnUnitSelected (unit);
		}

		public static void UpdateEnemyInfo(Unit unit){
			instance.OnHostileSelected(unit);
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