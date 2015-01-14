using UnityEngine;
using UnityEditor;

using System;

using System.Collections;
using System.Collections.Generic;

using TBTK;

namespace TBTK {

	public class FactionManagerEditorWindow : EditorWindow {
		
		public static FactionManager instance;
		public static FactionManagerEditorWindow window;
		
		private int playerCount=0;
		
		private float width=120;
		private float spaceX=100;
		private float height=17;
		private float spaceY=20;
		
		private float contentWidth;
		private float contentHeight;
		private Vector2 scrollPos;
		
		private int deleteID=-1;
		
		//both are used for delete spawngroup
		private int deleteIDFac=-1;
		private int deleteIDGroup=-1;
		
		private GUIContent cont;
		private GUIContent[] contList;
		
		private static string[] limitTypeLabel=new string[0];
		private static string[] limitTypeTooltip=new string[0];
		
		public static void Init(){
			// Get existing open window or if none, make a new one:
			window = (FactionManagerEditorWindow)EditorWindow.GetWindow(typeof (FactionManagerEditorWindow));
			//window.minSize=new Vector2(670, 620);
			
			EditorDBManager.Init();
			
			GetFactionManager();
			
			int enumLength = Enum.GetValues(typeof(FactionSpawnInfo._LimitType)).Length;
			limitTypeLabel=new string[enumLength];
			limitTypeTooltip=new string[enumLength];
			for(int i=0; i<enumLength; i++){
				limitTypeLabel[i]=((FactionSpawnInfo._LimitType)i).ToString();
				if((FactionSpawnInfo._LimitType)i==FactionSpawnInfo._LimitType.UnitCount) limitTypeTooltip[i]="Limited to an arbitary number";
				else if((FactionSpawnInfo._LimitType)i==FactionSpawnInfo._LimitType.UnitValue) limitTypeTooltip[i]="Limited based on the total value of the added unit";
			}
		}
		
		private static void GetFactionManager(){
			FactionManager.SetInstance();
			instance=FactionManager.GetInstance();
			//instance=(FactionManager)FindObjectOfType(typeof(FactionManager));
		}
		
		void OnGUI(){
			if(window==null) Init();
			if(instance==null){
				EditorGUI.LabelField(new Rect(5, 5, 350, 18), "No FactionManager in current scene");
				GetFactionManager();
				return;
			}
			
			float startX=5;
			float startY=5;
			
			if(GUI.Button(new Rect(window.position.width-130, 5, 125, 25), "Generate Unit")) instance._GenerateUnit();
			
			cont=new GUIContent("Generate Unit On Start: ", "Check to have generate unit on the grid based on each faction spawn info. Any existing unit will be wiped from the grid.");
			EditorGUI.LabelField(new Rect(startX, startY, width+50, height), cont);
			instance.generateUnitOnStart=EditorGUI.Toggle(new Rect(startX+spaceX+50, startY, width, height), instance.generateUnitOnStart);
			
			//cont=new GUIContent("Manual Unit Deployment: ", "Check to enable manual deployment of player's starting units. Otherwise the starting unit will be deployed randomly based on the assigned starting tiles.\n\nNote that this applies to StartingUnit in each faction only, not the units that are already on the grid");
			//EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width+50, height), cont);
			//instance.allowManualUnitDeployment=EditorGUI.Toggle(new Rect(startX+spaceX+50, startY, width, height), instance.allowManualUnitDeployment);
			
			
			startY+=10;
			
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width+50, height), "Total Faction Count: "+instance.factionList.Count);
			if(GUI.Button(new Rect(startX+150, startY, 120, height), "Add New Faction")) instance.factionList.Add(new Faction());
			
			if(playerCount==0) EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width+150, height), "WARNING: No player's faction!");
			else{
				string text=playerCount>1 ? " (Hotseat Mode)" : "";
				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width+150, height), "Total Player Faction: "+playerCount+text);
			}
			
			
			startY+=10;
			
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width+50, height), "Faction List: ");
			startY+=spaceY;
			startX+=10;
			
			
			Rect visibleRect=new Rect(startX, startY, window.position.width-10, window.position.height-startY-5);
			Rect contentRect=new Rect(startX-5, startY-5, contentWidth-25, contentHeight-startY+50);
			
			scrollPos = GUI.BeginScrollView(visibleRect, scrollPos, contentRect);
			
			contentHeight=startY;
			
			playerCount=0;
			for(int i=0; i<instance.factionList.Count; i++){
				instance.factionList[i].ID=i;
				
				if(instance.factionList[i].isPlayerFaction) playerCount+=1;
				
				Vector2 v2=DrawFactionConfigurator(startX, startY, instance.factionList[i]);
				//startY=v2.y+spaceY;
				startX+=320;
				contentHeight=Mathf.Max(contentHeight, v2.y);
				contentWidth=startX;
			}
			
			GUI.EndScrollView();
			
			if(GUI.changed) EditorUtility.SetDirty(instance);
		}
		
		Vector2 DrawFactionConfigurator(float startX, float startY, Faction faction){
			
			GUI.Box(new Rect(startX-5, startY-5, 305, 9999), "");
			
			if(deleteID!=faction.ID){
				if(GUI.Button(new Rect(startX+170, startY, 120, height), "Remove Faction")) deleteID=faction.ID;
			}
			else{
				if(GUI.Button(new Rect(startX+170, startY, 120, height), "Cancel")) deleteID=-1;
				GUI.color=Color.red;
				if(GUI.Button(new Rect(startX+105, startY, 60, height), "Remove")){
					deleteID=-1;
					instance.factionList.RemoveAt(faction.ID);
				}
				GUI.color=Color.white;
			}
			
			
			List<Unit> unitList=EditorDBManager.GetUnitList();
			string[] unitNameList=EditorDBManager.GetUnitNameList();
			
			
			cont=new GUIContent("Faction Name:", "The name of the faction. Just for user reference. Has no real effect in game");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width+50, height), cont);
			faction.name=EditorGUI.TextField(new Rect(startX+spaceX, startY, width, height), faction.name);
			
			//cont=new GUIContent("Colour:", "The colour to be used for gizmo related to this faction. Has no effect in game");
			//EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width+50, height), cont);
			//EditorGUI.ColorField(new Rect(startX+spaceX, startY, width+50, height), faction.color);
			
			faction.color=EditorGUI.ColorField(new Rect(startX+spaceX+width+10, startY, width-70, height), faction.color);
			
			
			startY+=5;
			
			
			cont=new GUIContent("Player Faction:", "Check if the faction is to be controlled by a player. Otherwise it will be run by AI");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width+50, height), cont);
			faction.isPlayerFaction=EditorGUI.Toggle(new Rect(startX+spaceX, startY, width, height), faction.isPlayerFaction);
			
			cont=new GUIContent("Load From Data:", "Check if the faction to load it's startingUnitList's unit from the data set in previous scene");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width+50, height), cont);
			faction.loadFromData=EditorGUI.Toggle(new Rect(startX+spaceX, startY, 30, height), faction.loadFromData);
			
			if(faction.loadFromData){
				cont=new GUIContent("Data-ID:", "The reference ID use to load the matching data as there can be multiple set of data.");
				EditorGUI.LabelField(new Rect(startX+spaceX+20, startY, width+50, height), cont);
				faction.dataID=EditorGUI.IntField(new Rect(startX+spaceX+75, startY, 40, height), faction.dataID);
			}
			else{
			
				cont=new GUIContent("Starting Unit:", "Unit to be spawned and deployed before the start of the game");
				GUI.Label(new Rect(startX, startY+=spaceY, width, height), cont);	startY-=spaceY-3;
				
				int ID=-1;
				for(int i=0; i<faction.startingUnitList.Count; i++){
					Unit unit=faction.startingUnitList[i];
					for(int n=0; n<unitList.Count; n++){ if(unitList[n]==unit) ID=n+1; }
					
					if(ID==-1){
						faction.startingUnitList.RemoveAt(i);
						i-=1;
						continue;
					}
					
					GUI.Label(new Rect(startX+80, startY+=spaceY-3, width, height), " - ");
					ID = EditorGUI.Popup(new Rect(startX+100, startY, width, height), ID, unitNameList);
					
					if(ID>0) faction.startingUnitList[i]=unitList[ID-1];
					else if(ID==0){ faction.startingUnitList.RemoveAt(i);	i-=1; }
				}
				
				ID=-1;
				GUI.Label(new Rect(startX+80, startY+=spaceY-3, width, height), " - ");
				ID = EditorGUI.Popup(new Rect(startX+100, startY, width, height), ID, unitNameList);
				if(ID>0) faction.startingUnitList.Add(unitList[ID-1]);
				
			}
			
			startY+=10;
			
			
			faction.foldSpawnInfo = EditorGUI.Foldout(new Rect(startX, startY+=spaceY, 60, 15), faction.foldSpawnInfo, "Show Spawn Info");
			if(faction.foldSpawnInfo){
				
				if(GUI.Button(new Rect(startX+165, startY, 120, height), "Add Spawn Group")) faction.spawnInfoList.Add(new FactionSpawnInfo());
				startY+=5;
				
				startX+=20;
				for(int i=0; i<faction.spawnInfoList.Count; i++){
					//float offset=faction.spawnInfoList[i].unitPrefabList.Count>=unitList.Count ? -1 : 0;
					//GUI.Box(new Rect(startX-3, startY+height, 275, 3+3*spaceY+(faction.spawnInfoList[i].unitPrefabList.Count+offset)*(spaceY-3)), "");
					
					startY+=2;
					
					startY+=spaceY;
					if(deleteIDGroup!=i || deleteIDFac!=faction.ID){
						if(GUI.Button(new Rect(startX+165, startY, 100, height-2), "Remove Group")){
							deleteIDGroup=i;
							deleteIDFac=faction.ID;
						}
					}
					else{
						if(GUI.Button(new Rect(startX+185, startY, 80, height-2), "Cancel")){
							deleteIDGroup=-1;
							deleteIDFac=-1;
						}
						GUI.color=Color.red;
						if(GUI.Button(new Rect(startX+125, startY, 60, height-2), "Remove")){
							deleteIDGroup=-1;
							deleteIDFac=-1;
							//instance.factionList.RemoveAt(faction.ID);
							faction.spawnInfoList.RemoveAt(i);
							continue;
						}
						GUI.color=Color.white;
					}
					startY-=spaceY;
					
					startY+=2;
					
					if(i>0) GUI.Label(new Rect(startX, startY, 270, 20), "____________________________________________");
					
					Vector2 v2=DrawSpawnInfo(startX, startY, faction.spawnInfoList[i]);		startY=v2.y+5;
					
				}
				startX-=20;
			}
			
			startY+=10;
			
			
			cont=new GUIContent("Faction's Ability Setting:", "Check if the faction to load it's startingUnitList's unit from the data set in previous scene");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width+50, height), cont);
			
			cont=new GUIContent(" - Full Energy:", "The maximum energy pool available to use ability for the faction");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width+50, height), cont);
			faction.fullEnergy=EditorGUI.FloatField(new Rect(startX+spaceX, startY, width, height), faction.fullEnergy);
			
			cont=new GUIContent(" - Energy Rate:", "The amount of energy gained each turn");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width+50, height), cont);
			faction.energyGainPerTurn=EditorGUI.FloatField(new Rect(startX+spaceX, startY, width, height), faction.energyGainPerTurn);
			
			//~ public List<int> unavailableAbilityIDList=new List<int>();	//ID list of ability not available for this level, modified in editor
			
			List<FactionAbility> facAbilityList=EditorDBManager.GetFactionAbilityList();
			string[] facAbilityNameList=EditorDBManager.GetFactionAbilityNameList();
			
			cont=new GUIContent(" - Ability:", "Ability available for this faction");
			GUI.Label(new Rect(startX, startY+=spaceY, width, height), cont);	startY-=spaceY-3;
			
			for(int i=0; i<facAbilityList.Count; i++){
				FactionAbility ability=facAbilityList[i];
				if(ability.onlyAvailableViaPerk) continue;
				if(faction.unavailableAbilityIDList.Contains(ability.prefabID)) continue;
				
				int ID=i+1;		int cachedID=i+1;
				
				GUI.Label(new Rect(startX+80, startY+=spaceY-3, width, height), " - ");
				ID = EditorGUI.Popup(new Rect(startX+100, startY, width, height), ID, facAbilityNameList);
				
				if(ID!=cachedID){
					if(ID>0 && facAbilityList[ID-1].prefabID!=ability.prefabID){
						if(faction.unavailableAbilityIDList.Contains(facAbilityList[ID-1].prefabID)){
							if(!facAbilityList[ID-1].onlyAvailableViaPerk) faction.unavailableAbilityIDList.Remove(facAbilityList[ID-1].prefabID);
						}
						faction.unavailableAbilityIDList.Add(ability.prefabID);
					}
					else if(ID==0 && !faction.unavailableAbilityIDList.Contains(ability.prefabID)) faction.unavailableAbilityIDList.Add(ability.prefabID); 
				}
			}
			
			if(faction.unavailableAbilityIDList.Count!=0){
				int ID=-1;
				GUI.Label(new Rect(startX+80, startY+=spaceY-3, width, height), " - ");
				ID = EditorGUI.Popup(new Rect(startX+100, startY, width, height), ID, facAbilityNameList);
				
				if(ID>0){
					if(facAbilityList[ID-1].onlyAvailableViaPerk){
						Debug.Log("ability is only available via perk");
					}
					else if(!faction.unavailableAbilityIDList.Contains(facAbilityList[ID-1].prefabID)){
						Debug.Log("ability already in list");
					}
					else{
						faction.unavailableAbilityIDList.Remove(facAbilityList[ID-1].prefabID);
					}
				}
			}
			
			
			return new Vector2(startX, startY);
		}
		
		
		Vector2 DrawSpawnInfo(float startX, float startY, FactionSpawnInfo spawnInfo){
			//~ public enum _LimitType{UnitCount, UnitValue}
			
			
			cont=new GUIContent("Tiles Count:", "The number of tiles assigned to this SpawnGroup in which the unit can be placed on. This can be Edit in GridEditor");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			EditorGUI.LabelField(new Rect(startX+spaceX-20, startY, width, 15), spawnInfo.startingTileList.Count.ToString());
			startY-=2;
			
			
			int limitType=(int)spawnInfo.limitType;
			cont=new GUIContent("Limit Type:", "The type of spawn limit applied to this spawn group.");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			cont=new GUIContent("", "");
			contList=new GUIContent[limitTypeLabel.Length];
			for(int i=0; i<contList.Length; i++) contList[i]=new GUIContent(limitTypeLabel[i], limitTypeTooltip[i]);
			limitType = EditorGUI.Popup(new Rect(startX+spaceX-20, startY, width, 15), cont, limitType, contList);
			spawnInfo.limitType=(FactionSpawnInfo._LimitType)limitType;
			
		
			if(spawnInfo.limitType==FactionSpawnInfo._LimitType.UnitCount) 
				cont=new GUIContent("-Limit:", "The maximum number of unit to be spawned");
			else if(spawnInfo.limitType==FactionSpawnInfo._LimitType.UnitValue) 
				cont=new GUIContent("-Limit:", "The maximum value of of the total unit spawned");
			//~ EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width+50, height), cont);
			//~ spawnInfo.limit=EditorGUI.IntField(new Rect(startX+spaceX, startY, width, height), spawnInfo.limit);
			
			//~ cont=new GUIContent("-Limit:", "The maximum spawn amount for this particular unit prefab");
			EditorGUI.LabelField(new Rect(startX+81+width, startY, width-50, height), cont);
			spawnInfo.limit=EditorGUI.IntField(new Rect(startX+121+width, startY, 25, height), spawnInfo.limit);
			spawnInfo.limit=Mathf.Clamp(spawnInfo.limit, 1, 99);
			
			//~ public _LimitType limitType;
			//~ public int limit=2;
			
			//~ public List<Unit> unitPrefabList=new List<Unit>();	//the prefab to be spawned
			//~ public List<int> unitLimitList=new List<int>();		//the limite of each unit, match to the count of unitPrefab
			
			List<Unit> unitList=EditorDBManager.GetUnitList();
			string[] unitNameList=EditorDBManager.GetUnitNameList();
			
			cont=new GUIContent("Prefab List:", "Unit to be spawned for this spawn group");
			GUI.Label(new Rect(startX, startY+=spaceY, width, height), cont);	startY-=spaceY-3;
			
			while(spawnInfo.unitLimitList.Count<spawnInfo.unitPrefabList.Count) spawnInfo.unitLimitList.Add(50);
			while(spawnInfo.unitLimitList.Count>spawnInfo.unitPrefabList.Count) spawnInfo.unitLimitList.RemoveAt(spawnInfo.unitLimitList.Count-1);
			
			int ID=-1;
			for(int i=0; i<spawnInfo.unitPrefabList.Count; i++){
				Unit unit=spawnInfo.unitPrefabList[i];
				for(int n=0; n<unitList.Count; n++){ if(unitList[n]==unit) ID=n+1; }
				
				GUI.Label(new Rect(startX+80, startY+=spaceY-3, width, height), " - ");
				ID = EditorGUI.Popup(new Rect(startX+spaceX-20, startY, width, height), ID, unitNameList);
				
				if(ID>0){
					if(!spawnInfo.unitPrefabList.Contains(unitList[ID-1])) spawnInfo.unitPrefabList[i]=unitList[ID-1];
				}
				else if(ID==0){ 
					spawnInfo.unitPrefabList.RemoveAt(i);	
					spawnInfo.unitLimitList.RemoveAt(i);
					if((i-=1)<0) break;
				}
				
				if(i<spawnInfo.unitLimitList.Count){
					cont=new GUIContent("-Limit:", "The maximum spawn amount for this particular unit prefab");
					EditorGUI.LabelField(new Rect(startX+81+width, startY, width-50, height), cont);
					spawnInfo.unitLimitList[i]=EditorGUI.IntField(new Rect(startX+121+width, startY, 25, height), spawnInfo.unitLimitList[i]);
					spawnInfo.unitLimitList[i]=Mathf.Clamp(spawnInfo.unitLimitList[i], 1, 99);
				}
			}
			
			if(spawnInfo.unitPrefabList.Count<unitList.Count){
				ID=-1;
				GUI.Label(new Rect(startX+80, startY+=spaceY-3, width, height), " - ");
				ID = EditorGUI.Popup(new Rect(startX+spaceX-20, startY, width, height), ID, unitNameList);
				if(ID>0 && !spawnInfo.unitPrefabList.Contains(unitList[ID-1])){
					spawnInfo.unitPrefabList.Add(unitList[ID-1]);
					spawnInfo.unitLimitList.Add(50);
				}
			}
			
			
			
			
			
			return new Vector2(startX, startY);
		}
		
	}

}