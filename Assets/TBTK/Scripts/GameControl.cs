using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using TBTK;
using Cards;

namespace TBTK{

	public enum _GamePhase{ 
		Initialization, 		//the game is being initialized
		UnitDeployment, 	//unit deployment is taking place
		Play, 					//the game is playing	
		Over,					//game over
	}
	
	[RequireComponent (typeof (DamageTable))]
	[RequireComponent (typeof (TurnControl))]
	[RequireComponent (typeof (AIManager))]
	[RequireComponent (typeof (Objective))]
	public class GameControl : MonoBehaviour {
		
		public delegate void GameMessageHandler(string msg);
		public static event GameMessageHandler onGameMessageE;	//listen by UI to display any game message

		public delegate void GameStartHandler();
		public static event GameStartHandler onGameStartE;	//called when game started (unit deployment finished) for UI overlays to show
		
		public delegate void IterateTurnHandler();
		public static event IterateTurnHandler onIterateTurnE;	//listen by EffectTracker and AbilityManager to keep track of the cd/duration and what not
		
		public delegate void GameOverHandler(int factionID);	
		public static event GameOverHandler onGameOverE;	//listen by AudioManager and UI

		public delegate void PassLevelHandler();
		public static event PassLevelHandler onPassLevelE;

		public delegate void SelectUnitHandler ();
		public static event SelectUnitHandler onUnitChosen;
		
		private static _GamePhase gamePhase=_GamePhase.Initialization;
		public static _GamePhase GetGamePhase(){ return gamePhase; }
		
		
		public static Unit selectedUnit;	//the current selected unit, this is only for player's unit as AI unit dont get selected

		public static Tile selectedTile; //the current selected tile
		
		public static bool isUnitChosen;

		public static Unit chosenUnit; 
		
		public bool useGlobalSetting=true;

		public bool enableManualUnitDeployment=true;
		public static bool EnableManualUnitDeployment(){ return instance.enableManualUnitDeployment; }
		
		public bool enableActionAfterAttack=false;
		public static bool EnableActionAfterAttack(){ return instance.enableActionAfterAttack; }
		
		public bool enableCounter=true;
		public static bool EnableCounter(){ return instance.enableCounter; }
		public float counterAPMultiplier=1f;
		public static float GetCounterAPMultiplier(){ return !instance.useAPForAttack ? 0 : instance.counterAPMultiplier; }
		public float counterDamageMultiplier=1f;
		public static float GetCounterDamageMultiplier(){ return instance.counterDamageMultiplier; }
		
		public bool restoreUnitAPOnTurn=true;
		public static bool RestoreUnitAPOnTurn(){ return instance!=null ? instance.restoreUnitAPOnTurn : true; }
		
		public bool useAPForMove=true;
		public bool useAPForAttack=true;
		public static bool UseAPForMove(){ return instance.useAPForMove; }
		public static bool UseAPForAttack(){ return instance.useAPForAttack; }
		
		
		public bool attackThroughObstacle=false;
		public static bool AttackThroughObstacle(){ return instance.attackThroughObstacle; }
		
		
		public bool enableFogOfWar=false;
		public static bool EnableFogOfWar(){ return instance.enableFogOfWar; }
		public float peekFactor=0.4f;
		public static float GetPeekFactor(){ return Mathf.Clamp(instance.peekFactor, 0, 0.5f); }
		
		
		public bool enableCover=false;
		public float exposedCritBonus=0.3f;
		public float fullCoverBonus=0.75f;
		public float halfCoverBonus=0.25f;
		public static bool EnableCover(){ return instance.enableCover; }
		public static float GetFullCoverBonus(){ return instance.fullCoverBonus; }
		public static float GetHalfCoverBonus(){ return instance.halfCoverBonus; }
		
		
		public bool enableFlanking=false;
		public float flankingAngle=120;
		public float flankingBonus=.5f;
		public static bool EnableFlanking(){ return instance.enableFlanking; }
		public static float GetFlankingAngle(){ return instance.flankingAngle; }
		public static float GetFlankingBonus(){ return instance.flankingBonus; }
		
		
		public string nextScene="";
		public string mainMenu="";
		public static void LoadNextScene(){ if(instance.nextScene!="") Load(instance.nextScene); }
		public static void LoadMainMenu(){ if(instance.mainMenu!="") Load(instance.mainMenu); }
		public static void Load(string levelName){ Application.LoadLevel(levelName); }
		
		
		private GameObject defaultShootObject;	//the backup shootObject to use, in case a unit prefab has not shootObject
		public static GameObject GetDefaultShootObject(){ return instance.defaultShootObject; }
		
		
		private static GameControl instance;

		private int actionsAtStart = 0;

		public static float delayPerAction = 0.5f;

		public bool gameStarted = false;

		public static void fastFowardTo(float speed){
			Time.timeScale = speed;
		}

		void Awake(){
			instance=this;
			
			Data.ClearEndData();
			
			SettingDB settingDB=InitSetting();
			
			if(enableCover){
				CoverSystem.SetFullCoverDodgeBonus(fullCoverBonus);
				CoverSystem.SetHalfCoverDodgeBonus(halfCoverBonus);
				CoverSystem.SetExposedCritChanceBonus(exposedCritBonus);
			}
			
			//get the instance of each component and initiate them, the order in which these component matters
			
			PerkManager perkManager = (PerkManager)FindObjectOfType(typeof(PerkManager));
			if(perkManager!=null) perkManager.Init();
			
			AbilityManagerUnit abilityManagerUnit = (AbilityManagerUnit)FindObjectOfType(typeof(AbilityManagerUnit));
			abilityManagerUnit.Init();
			
			TurnControl turnControl = (TurnControl)FindObjectOfType(typeof(TurnControl));
			turnControl.Init();
			
			if(settingDB!=null){
				turnControl.turnMode=settingDB.turnMode;
				turnControl.moveOrder=settingDB.moveOrder;
			}
			
			GridManager gridManager = (GridManager)FindObjectOfType(typeof(GridManager));
			if(settingDB!=null) gridManager.generateGridOnStart=settingDB.generateGridOnStart;
			gridManager.Init();
			
			FactionManager factionManager = (FactionManager)FindObjectOfType(typeof(FactionManager));
			if(settingDB!=null) factionManager.generateUnitOnStart=settingDB.generateUnitOnStart;
			factionManager.Init();
			
			GridManager.SetupGridForFogOfWar();
			
			defaultShootObject=Resources.Load("ScenePrefab/DefaultShootObject", typeof(GameObject)) as GameObject;
			
			gamePhase=_GamePhase.Initialization;
		}
		
		
		void Start () {
			if(!FactionManager.RequireManualUnitDeployment()) StartCoroutine(DelayStartGame(0.5f));
			else StartCoroutine(DelayUnitDeployment(0.25f)); 
		}
		
		//start the game, this is called after unit deployment is complete, or after initialization if no deployment is required
		public static void StartGame(){ instance.StartCoroutine(instance.DelayStartGame(0.5f)); }
		IEnumerator DelayStartGame(float delay=0.5f){

			LoadingScreen.ShowLoadingScreen ();

			yield return null;
			FactionManager.SetupFaction();
			GridManager.SetupGridForFogOfWar();
			yield return null;
			
			if(delay>0) yield return new WaitForSeconds(delay);

			AbilityManagerFaction.StartCounter();	//for ability energy to start charging
			
			if(onGameStartE!=null) onGameStartE();
			yield return new WaitForSeconds (0.2f);
			Unit.gameStarted = true;
			while (actionsAtStart !=0)
				yield return null;
			LoadingScreen.FadeOut ();

			gamePhase=_GamePhase.Play;
//			CardsHandManager.ShuffleDeck ();

			TurnControl.EndTurn();	//this will initiate unit selection and start the game

			yield return null;
		}
		
		//called if unit deployment is required
		IEnumerator DelayUnitDeployment(float delay=0.5f){
			if(delay>0) yield return new WaitForSeconds(delay);
			gamePhase=_GamePhase.UnitDeployment;
			FactionManager.StartUnitDeploymentPhase();
			yield return null;
		}
		
		
		
		
		private bool allowUnitSelect=true;	//lock unit select after a unit has been moved
		public static bool AllowUnitSelect(){ return instance.allowUnitSelect; }
		public static void LockUnitSelect(){
//			if(TurnControl.GetTurnMode()==_TurnMode.FactionUnitPerTurn) instance.allowUnitSelect=false;
		}
		public static void UnlockUnitSelect(){ instance.allowUnitSelect=true; }
		
		
		//function to select unit, unit selection start here
		public static void SelectUnit(Tile tile){ SelectUnit(tile.unit); }
		public static void SelectUnit(Unit unit){
			if(!FactionManager.IsPlayerFaction(unit.factionID)){	//used in FactionUnitPerTurn & UnitPerTurn mode
				ClearSelectedUnit();
				AIManager.MoveUnit(unit);
			}
			else{

				ClearSelectedUnit();
				if(!unit.usedThisTurn && !isUnitChosen) chosenUnit = unit;
				selectedUnit=unit;
				GridManager.Select(unit);
				unit.Select();
			}
		}
		public static void ClearSelectedUnit(){
			if(selectedUnit!=null) selectedUnit.ClearSelectedAbility();
			GridManager.ClearAllTile();
			selectedUnit=null;
			Unit.Deselect();
		}

		public static void SelectTile(Tile tile){
			selectedTile = tile;
			GridManager.Select(tile);
		}
		public static void ClearSelectedTile(){
			GridManager.ClearSelectedTile();
			selectedTile = null;
		}
		
		
		
		
		public static void GameOver(int factionID){
			if(FactionManager.IsPlayerFaction(factionID)){
				PerkManager.GainPerkCurrencyOnVictory();
			}
			
			if(onGameMessageE!=null) onGameMessageE("GameOver");
			
			gamePhase=_GamePhase.Over;
			
			FactionManager.GameOver();
			
			if(onGameOverE!=null) onGameOverE(factionID);
		}
		
		
		
		
		void OnEnable(){
			Unit.onUnitDestroyedE += OnUnitDestroyed;
			GridManager.onHostileSelectE += OnHostileSelected;
			GridManager.onHostileDeselectE += ClearSelectedTile;
			GridManager.onHostileDeselectE += OnHostileDeselected;


		}
		void OnDisable(){
			Unit.onUnitDestroyedE -= OnUnitDestroyed;
			GridManager.onHostileSelectE -= OnHostileSelected;
			GridManager.onHostileDeselectE -= ClearSelectedTile;
			GridManager.onHostileDeselectE -= OnHostileDeselected;

		}
		
		void OnUnitDestroyed(Unit unit){
			if(TurnControl.GetTurnMode()==_TurnMode.FactionPerTurn) return;
			if(TurnControl.GetTurnMode()==_TurnMode.FactionUnitPerTurn){
				if(TurnControl.GetMoveOrder()==_MoveOrder.Free) return;
			}
			
			if(onIterateTurnE!=null) onIterateTurnE();	//listen by EffectTracker and AbilityManager to iterate effect and cd duration
																		//listen by tile in for tracking forceVisible(scan)

		}
		
		//end the turn, called when EndTurn button are pressed or when a unit has used up all its move(in FactionUnitPerTurn & UnitPerTurn mode)
		public static void EndTurn(){ 
			if(chosenUnit != null)chosenUnit.usedThisTurn = true;


			isUnitChosen = false;
			chosenUnit = null;
			if(onIterateTurnE!=null) onIterateTurnE();	//listen by EffectTracker and AbilityManager to iterate effect and cd duration
																		//listen by tile in for tracking forceVisible(scan)

			Debug.Log("end turn");
			
			ClearSelectedUnit();
			TurnControl.EndTurn();
		}
		
		
		
		private SettingDB InitSetting(){
			//if set to use global setting, overwrite all the local setting with the one from DB
			if(!useGlobalSetting) return null;
			
			SettingDB db=SettingDB.LoadDB();
			if(db==null) return null;
			
			enableManualUnitDeployment=db.enableManualUnitDeployment;
			
			enableActionAfterAttack=db.enableActionAfterAttack;
			
			enableCounter=db.enableCounter;
			counterAPMultiplier=db.counterAPMultiplier;
			counterDamageMultiplier=db.counterDamageMultiplier;
			
			restoreUnitAPOnTurn=db.restoreUnitAPOnTurn;
			
			useAPForMove=db.useAPForMove;
			useAPForAttack=db.useAPForAttack;

			attackThroughObstacle=db.attackThroughObstacle;
			
			enableFogOfWar=db.enableFogOfWar;
			peekFactor=db.peekFactor;
			
			enableCover=db.enableCover;
			exposedCritBonus=db.exposedCritBonus;
			fullCoverBonus=db.fullCoverBonus;
			halfCoverBonus=db.halfCoverBonus;
			
			enableFlanking=db.enableFlanking;
			flankingAngle=db.flankingAngle;
			flankingBonus=db.flankingBonus;
			
			return db;
		}

		void OnHostileSelected(Unit unit){
			selectedTile = unit.tile;
		}

		void OnHostileDeselected(){
			selectedTile = null;
		}

		public static IEnumerator PassLevel(){

			if (onPassLevelE != null)
				onPassLevelE ();

			yield return new WaitForSeconds (LoadingScreen.staticSecondsToFadeIn + 0.2f);

			Application.LoadLevel (Levels_DB.GetSceneLevel(MapController.level));
			yield return null;
		}
		
		
		public static void DisplayMessage(string msg){ 
			if(onGameMessageE!=null) onGameMessageE(msg);
		}

		public static void AddActionAtStart(){
			instance.actionsAtStart ++;
		}

		public static void CompleteActionAtStart(){
			instance.actionsAtStart--;
		}

		
		public static void ChooseSelectedUnit(){
			if (!isUnitChosen && selectedUnit != null) {
				isUnitChosen = true;
				chosenUnit = selectedUnit;
				selectedUnit.usedThisTurn = true;
				selectedUnit.getStack ().updateDamageAndGuard ();
				selectedUnit.getStack().updateAttributesForLists();
				if (onUnitChosen != null)
					onUnitChosen ();
			}

		}

		public static bool HasGameStarted(){
			return instance.gameStarted;
		}

		public static void GameStarted(){
			instance.gameStarted = true;
		}


		
	}

}