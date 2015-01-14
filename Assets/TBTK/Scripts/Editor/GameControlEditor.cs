using UnityEngine;
using UnityEditor;

using System;

using System.Collections;
using System.Collections.Generic;

using TBTK;

namespace TBTK{

	[CustomEditor(typeof(GameControl))]
	public class GameControlEditor : Editor {

		private static GameControl instance;
		private static TurnControl turnControl;
		private static GridManager gridManager;
		private static FactionManager factionManager;
		private static SettingDB settingDB;
		
		private static bool showDefaultFlag=false;
		
		
		private string[] turnModeLabel;
		private string[] turnModeTooltip;
		private string[] moveOrderLabel;
		private string[] moveOrderTooltip;
		
		
		private GUIContent cont;
		private GUIContent[] contList;
		
		
		void Awake(){
			instance = (GameControl)target;
			
			settingDB=SettingDB.LoadDB();
			
			EditorDBManager.Init();
			
			turnControl = (TurnControl)FindObjectOfType(typeof(TurnControl));
			gridManager = (GridManager)FindObjectOfType(typeof(GridManager));
			factionManager = (FactionManager)FindObjectOfType(typeof(FactionManager));
			
			int enumLength = Enum.GetValues(typeof(_TurnMode)).Length;
			turnModeLabel=new string[enumLength];
			turnModeTooltip=new string[enumLength];
			for(int i=0; i<enumLength; i++){
				turnModeLabel[i]=((_TurnMode)i).ToString();
				if((_TurnMode)i==_TurnMode.FactionPerTurn) 
					turnModeTooltip[i]="Always show the tile currently being hovered over by the cursor";
				if((_TurnMode)i==_TurnMode.FactionUnitPerTurn) 
					turnModeTooltip[i]="Only show the tile currently being hovered over by the cursor if it's available to be built on";
				if((_TurnMode)i==_TurnMode.UnitPerTurn) 
					turnModeTooltip[i]="Never show the tile currently being hovered over by the cursor";
			}
			
			enumLength = Enum.GetValues(typeof(_MoveOrder)).Length;
			moveOrderLabel=new string[enumLength];
			moveOrderTooltip=new string[enumLength];
			for(int i=0; i<enumLength; i++){
				moveOrderLabel[i]=((_MoveOrder)i).ToString();
				if((_MoveOrder)i==_MoveOrder.Free) 
					moveOrderTooltip[i]="Always show the tile currently being hovered over by the cursor";
				if((_MoveOrder)i==_MoveOrder.Random) 
					moveOrderTooltip[i]="Only show the tile currently being hovered over by the cursor if it's available to be built on";
				if((_MoveOrder)i==_MoveOrder.StatsBased) 
					moveOrderTooltip[i]="Never show the tile currently being hovered over by the cursor";
			}
			
			
			EditorUtility.SetDirty(instance);
		}
		
		
		
		
		public override void OnInspectorGUI(){
			
			GUI.changed = false;
			
			EditorGUILayout.Space();
			
			cont=new GUIContent("Use Global Setting:", "Check to use a global setting. This setting will be used for all the scene in the project that have this flag checked");
			instance.useGlobalSetting=EditorGUILayout.Toggle(cont, instance.useGlobalSetting);
			
			//EditorGUILayout.Space();
			EditorGUILayout.LabelField("_____________________________________________________________________________________");
			EditorGUILayout.Space();
			
			if(instance.useGlobalSetting){
				
				cont=new GUIContent("Generate Grid On Start:", "");
				settingDB.generateGridOnStart=EditorGUILayout.Toggle(cont, settingDB.generateGridOnStart);
				cont=new GUIContent("Generate Unit On Start:", "");
				settingDB.generateUnitOnStart=EditorGUILayout.Toggle(cont, settingDB.generateUnitOnStart);
				
				EditorGUILayout.Space();
				
				int turnMode=(int)settingDB.turnMode;
				cont=new GUIContent("Turn Mode:", "The turn logic to be used. Determine how each faction takes turn");
				contList=new GUIContent[turnModeLabel.Length];
				for(int i=0; i<contList.Length; i++) contList[i]=new GUIContent(turnModeLabel[i], turnModeTooltip[i]);
				turnMode = EditorGUILayout.Popup(cont, turnMode, contList);
				settingDB.turnMode=(_TurnMode)turnMode;
				
				if(settingDB.turnMode!=_TurnMode.UnitPerTurn){
					int moveOrder=(int)settingDB.moveOrder;
					cont=new GUIContent("Move Order:", "The move order to be used. Determine which unit gets to move first");
					contList=new GUIContent[moveOrderLabel.Length];
					for(int i=0; i<contList.Length; i++) contList[i]=new GUIContent(moveOrderLabel[i], moveOrderTooltip[i]);
					moveOrder = EditorGUILayout.Popup(cont, moveOrder, contList);
					settingDB.moveOrder=(_MoveOrder)moveOrder;
				}
				else{
					cont=new GUIContent("Move Order:            Not valid for TurnMode", "The move order to be used. Determine which unit gets to move first");
					EditorGUILayout.LabelField(cont);
				}
				
				EditorGUILayout.Space();
				
				cont=new GUIContent("Enable ManualUnitDeployment:", "");
				settingDB.enableManualUnitDeployment=EditorGUILayout.Toggle(cont, settingDB.enableManualUnitDeployment);
				
				EditorGUILayout.Space();
				
				cont=new GUIContent("Enable ActionAfterAttack:", "Check to enable unit to perform other action after attacking a target");
				settingDB.enableActionAfterAttack=EditorGUILayout.Toggle(cont, settingDB.enableActionAfterAttack);
				
				cont=new GUIContent("Enable CounterAttack:", "Check to enable unit to counter attack when attacked. The counter is subject to the unit attack range, counter move remain, AP, etc.");
				settingDB.enableCounter=EditorGUILayout.Toggle(cont, settingDB.enableCounter);
				
				if(settingDB.enableCounter){
					if(settingDB.useAPForAttack){
						cont=new GUIContent(" - Counter AP Multiplier:", "Multiplier to attack AP cost when the unit are attacking via a counter attack move. Takes value from 0 and above with 0.7 being 70% of the default AP cost");
						settingDB.counterAPMultiplier=EditorGUILayout.FloatField(cont, settingDB.counterAPMultiplier);
					}
					
					cont=new GUIContent(" - Damage Multiplier:", "Multiplier to damage inflicted when the unit are attacking via a counter attack move. Takes value from 0 and above with 0.6 being 60% of the default damage");
					settingDB.counterDamageMultiplier=EditorGUILayout.FloatField(cont, settingDB.counterDamageMultiplier);
				}
				
				EditorGUILayout.Space();
				
				cont=new GUIContent("Restore Unit AP on Turn:", "Check to have unit's AP restored to full on each turn");
				settingDB.restoreUnitAPOnTurn=EditorGUILayout.Toggle(cont, settingDB.restoreUnitAPOnTurn);
				
				
				cont=new GUIContent("Use AP For Move:", "Check to have unit use AP for each move");
				settingDB.useAPForMove=EditorGUILayout.Toggle(cont, settingDB.useAPForMove);
				
				cont=new GUIContent("Use AP For Attack:", "Check to have unit use AP for each attack");
				settingDB.useAPForAttack=EditorGUILayout.Toggle(cont, settingDB.useAPForAttack);
				
				EditorGUILayout.Space();
				
				
				cont=new GUIContent("Attack Through Obstacle:", "Check to enable unit to attack through obstacle.\nOnly applies when Fog-of-War is disabled\n\nNote: only obstacle wth full cover can obstruct an attack. Unit can always through obstacle with half cover");
				if(settingDB.enableFogOfWar) EditorGUILayout.Toggle(cont, true);
				else settingDB.attackThroughObstacle=EditorGUILayout.Toggle(cont, settingDB.attackThroughObstacle);
				
				EditorGUILayout.Space();
				
				cont=new GUIContent("Enable Fog-Of-War:", "Check to enable Fog-of-War in the game");
				settingDB.enableFogOfWar=EditorGUILayout.Toggle(cont, settingDB.enableFogOfWar);
				if(settingDB.enableFogOfWar){
					cont=new GUIContent(" - Peek Factor:", "A value indicate if the units can peek around a obstacle to see what's on the other end.\nTakes value from 0-0.5\nWhen set to 0, unit cannot peek at all (can only see 45degree from the obstacle)\nWhen set to 0.5, unit can peek and will be able to see what's behind the obstacle");
					settingDB.peekFactor=EditorGUILayout.FloatField(cont, settingDB.peekFactor);
				}
				
				
				EditorGUILayout.Space();
				
				cont=new GUIContent("Enable Cover System:", "Check to enable cover system in the game. Unit will get a hit penalty when attacking target behind a wall/obstacle (in cover) as well as getting a critical bonus when attacking a target not in cover.");
				settingDB.enableCover=EditorGUILayout.Toggle(cont, settingDB.enableCover);
				
				if(settingDB.enableCover){
					cont=new GUIContent(" - Exposed Critical Bonus:", "The citical chance bonus for attacking a unit not in cover. Value is used to modify the critical chance directly. ie. 0.25 means 25% increase in critical chance");
					settingDB.exposedCritBonus=EditorGUILayout.FloatField(cont, settingDB.exposedCritBonus);
					
					cont=new GUIContent(" - Full Cover Bonus:", "The dodge bonus for unit attacked from behind a 'full' cover. Value is used to modify the unit dodge directly. ie. 0.25 means 25% increase in dodge chance");
					settingDB.fullCoverBonus=EditorGUILayout.FloatField(cont, settingDB.fullCoverBonus);
					
					cont=new GUIContent(" - Half Cover Bonus:", "The dodge bonus for unit attacked from behind a 'half' cover. Value is used to modify the unit dodge directly. ie. 0.25 means 25% increase in dodge chance");
					settingDB.halfCoverBonus=EditorGUILayout.FloatField(cont, settingDB.halfCoverBonus);
				}
				
				EditorGUILayout.Space();
				
				cont=new GUIContent("Enable Flanking:", "Check to enable flanking, unit will get a damage bonus when attacking a target from the rear");
				settingDB.enableFlanking=EditorGUILayout.Toggle(cont, settingDB.enableFlanking);
				
				if(settingDB.enableFlanking){
					cont=new GUIContent(" - Flanking Angle:", "The angle at which the target will be considered flanked. This angle origin from target's front. ie, when set to 80, the target is considered flanked when attacked from the side");
					settingDB.flankingAngle=EditorGUILayout.FloatField(cont, settingDB.flankingAngle);
					
					cont=new GUIContent(" - Flanking Bonus:", "The damage multiplier to be applied to the damage. Takes value from 0 and above with 0.2 being increase damage by 20%");
					settingDB.flankingBonus=EditorGUILayout.FloatField(cont, settingDB.flankingBonus);
				}
				
			}
			else{
				
				if(gridManager!=null){
					cont=new GUIContent("Generate Grid On Start:", "");
					gridManager.generateGridOnStart=EditorGUILayout.Toggle(cont, gridManager.generateGridOnStart);
				}
				if(factionManager!=null){
					cont=new GUIContent("Generate Unit On Start:", "");
					factionManager.generateUnitOnStart=EditorGUILayout.Toggle(cont, factionManager.generateUnitOnStart);
				}
				
				EditorGUILayout.Space();
				
				if(turnControl!=null){
					int turnMode=(int)turnControl.turnMode;
					cont=new GUIContent("Turn Mode:", "The turn logic to be used. Determine how each faction takes turn");
					contList=new GUIContent[turnModeLabel.Length];
					for(int i=0; i<contList.Length; i++) contList[i]=new GUIContent(turnModeLabel[i], turnModeTooltip[i]);
					turnMode = EditorGUILayout.Popup(cont, turnMode, contList);
					turnControl.turnMode=(_TurnMode)turnMode;
					
					int moveOrder=(int)turnControl.moveOrder;
					cont=new GUIContent("Move Order:", "The move order to be used. Determine which unit gets to move first");
					contList=new GUIContent[moveOrderLabel.Length];
					for(int i=0; i<contList.Length; i++) contList[i]=new GUIContent(moveOrderLabel[i], moveOrderTooltip[i]);
					moveOrder = EditorGUILayout.Popup(cont, moveOrder, contList);
					turnControl.moveOrder=(_MoveOrder)moveOrder;
				}
				
				EditorGUILayout.Space();
				
				cont=new GUIContent("Enable ManualUnitDeployment:", "");
				instance.enableManualUnitDeployment=EditorGUILayout.Toggle(cont, instance.enableManualUnitDeployment);
				
				EditorGUILayout.Space();
				
				cont=new GUIContent("Enable ActionAfterAttack:", "");
				instance.enableActionAfterAttack=EditorGUILayout.Toggle(cont, instance.enableActionAfterAttack);
				
				cont=new GUIContent("Enable CounterAttack:", "");
				instance.enableCounter=EditorGUILayout.Toggle(cont, instance.enableCounter);
				
				if(instance.enableCounter){
					if(instance.useAPForAttack){
						cont=new GUIContent(" - Counter AP Multiplier:", "");
						instance.counterAPMultiplier=EditorGUILayout.FloatField(cont, instance.counterAPMultiplier);
					}
					
					cont=new GUIContent(" - Damage Multiplier:", "");
					instance.counterDamageMultiplier=EditorGUILayout.FloatField(cont, instance.counterDamageMultiplier);
				}
				
				EditorGUILayout.Space();
				
				cont=new GUIContent("Restore Unit AP on Turn:", "");
				instance.restoreUnitAPOnTurn=EditorGUILayout.Toggle(cont, instance.restoreUnitAPOnTurn);
				
				
				cont=new GUIContent("Use AP For Move:", "");
				instance.useAPForMove=EditorGUILayout.Toggle(cont, instance.useAPForMove);
				
				cont=new GUIContent("Use AP For Attack:", "");
				instance.useAPForAttack=EditorGUILayout.Toggle(cont, instance.useAPForAttack);
		
				EditorGUILayout.Space();
				
				
				cont=new GUIContent("Attack Through Obstacle:", "Check to enable unit to attack through obstacle.\nOnly applies when Fog-of-War is disabled\n\nNote: only obstacle wth full cover can obstruct an attack. Unit can always through obstacle with half cover");
				if(instance.enableFogOfWar) EditorGUILayout.Toggle(cont, true);
				else instance.attackThroughObstacle=EditorGUILayout.Toggle(cont, instance.attackThroughObstacle);
				
				EditorGUILayout.Space();
				
				cont=new GUIContent("Enable Fog-Of-War:", "Check to enable Fog-of-War in the game");
				instance.enableFogOfWar=EditorGUILayout.Toggle(cont, instance.enableFogOfWar);
				if(instance.enableFogOfWar){
					cont=new GUIContent(" - Peek Factor:", "A value indicate if the units can peek around a obstacle to see what's on the other end.\nTakes value from 0-0.5\nWhen set to 0, unit cannot peek at all (can only see 45degree from the obstacle)\nWhen set to 0.5, unit can peek and will be able to see what's behind the obstacle");
					instance.peekFactor=EditorGUILayout.FloatField(cont, instance.peekFactor);
				}
				
				EditorGUILayout.Space();
		
		
				cont=new GUIContent("Enable Cover System:", "");
				instance.enableCover=EditorGUILayout.Toggle(cont, instance.enableCover);
				
				if(instance.enableCover){
					cont=new GUIContent(" - Exposed Critical Bonus:", "The citical chance bonus for attacking a unit not in cover. Value is used to modify the critical chance directly. ie. 0.25 means 25% increase in critical chance");
					instance.exposedCritBonus=EditorGUILayout.FloatField(cont, instance.exposedCritBonus);
					
					cont=new GUIContent(" - Full Cover Bonus:", "");
					instance.fullCoverBonus=EditorGUILayout.FloatField(cont, instance.fullCoverBonus);
					
					cont=new GUIContent(" - Half Cover Bonus:", "");
					instance.halfCoverBonus=EditorGUILayout.FloatField(cont, instance.halfCoverBonus);
				}
				
				EditorGUILayout.Space();
				
				cont=new GUIContent("Enable Flanking:", "");
				instance.enableFlanking=EditorGUILayout.Toggle(cont, instance.enableFlanking);
				
				if(instance.enableFlanking){
					cont=new GUIContent(" - Flanking Angle:", "");
					instance.flankingAngle=EditorGUILayout.FloatField(cont, instance.flankingAngle);
					
					cont=new GUIContent(" - Flanking Bonus:", "");
					instance.flankingBonus=EditorGUILayout.FloatField(cont, instance.flankingBonus);
				}
				
			
			}
			
			EditorGUILayout.LabelField("_____________________________________________________________________________________");
			EditorGUILayout.Space();
			
			cont=new GUIContent("Next Scene Name:", "");
			instance.nextScene=EditorGUILayout.TextField(cont, instance.nextScene);
			
			cont=new GUIContent("Main Menu Name:", "");
			instance.mainMenu=EditorGUILayout.TextField(cont, instance.mainMenu);
			
			EditorGUILayout.LabelField("_____________________________________________________________________________________");
			EditorGUILayout.Space();
			
			if(!Application.isPlaying){
				if(gridManager!=null){
					if(GUILayout.Button("Generate Grid", GUILayout.MaxWidth(258))) gridManager.GenerateGrid();
				}
				if(factionManager!=null){
					if(GUILayout.Button("Generate Unit", GUILayout.MaxWidth(258))) FactionManager.GenerateUnit();
				}
			}
			
			EditorGUILayout.Space();
			
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("", GUILayout.MaxWidth(10));
			showDefaultFlag=EditorGUILayout.Foldout(showDefaultFlag, "Show default editor");
			EditorGUILayout.EndHorizontal();
			if(showDefaultFlag) DrawDefaultInspector();
			
			
			if(GUI.changed) EditorUtility.SetDirty(instance);
			
		}
		
		
		
	}

	
}