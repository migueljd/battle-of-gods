using UnityEngine;
using UnityEditor;

using System;

using System.Collections;
using System.Collections.Generic;

using TBTK;

namespace TBTK {

	public class FactionAbilityEditorWindow : AbilityEditorWindow {
		
		private static FactionAbilityEditorWindow window;
		
		protected static string[] abilityTypeLabel;
		protected static string[] abilityTypeTooltip;
		
		
		public static void Init () {
			// Get existing open window or if none, make a new one:
			window = (FactionAbilityEditorWindow)EditorWindow.GetWindow(typeof (FactionAbilityEditorWindow));
			//~ window.minSize=new Vector2(375, 449);
			//~ window.maxSize=new Vector2(375, 800);
			
			EditorDBManager.Init();
			
			InitLabel();
			
			int enumLength = Enum.GetValues(typeof(FactionAbility._AbilityType)).Length;
			abilityTypeLabel=new string[enumLength];
			abilityTypeTooltip=new string[enumLength];
			for(int i=0; i<enumLength; i++){
				abilityTypeLabel[i]=((FactionAbility._AbilityType)i).ToString();
				if((FactionAbility._AbilityType)i==FactionAbility._AbilityType.Generic) 		abilityTypeTooltip[i]="Modify stats";
				if((FactionAbility._AbilityType)i==FactionAbility._AbilityType.SpawnNew) 	abilityTypeTooltip[i]="Deploy an additional unit";
				if((FactionAbility._AbilityType)i==FactionAbility._AbilityType.ScanFogOfWar) 		abilityTypeTooltip[i]="Reveal fog of war";
			}
		}
		
		
		
		
		
		
		
		void OnGUI () {
			if(window==null) Init();
			
			List<FactionAbility> factionAbilityList=EditorDBManager.GetFactionAbilityList();
			
			if(GUI.Button(new Rect(window.position.width-120, 5, 100, 20), "Save")) EditorDBManager.SetDirtyFactionAbility();
			
			if(GUI.Button(new Rect(5, 5, 120, 20), "Create New")){
				FactionAbility newAbility=new FactionAbility();
				int newSelectID=EditorDBManager.AddNewFactionAbility(newAbility);
				if(newSelectID!=-1) SelectAbility(newSelectID);
				newAbility.name="Ability "+newSelectID;
			}
			if(factionAbilityList.Count>0 && GUI.Button(new Rect(130, 5, 100, 20), "Clone Selected")){
				FactionAbility newAbility=factionAbilityList[selectID].Clone();
				newAbility.name+=" (Clone)";
				int newSelectID=EditorDBManager.AddNewFactionAbility(newAbility);
				if(newSelectID!=-1) SelectAbility(newSelectID);
			}
			
			
			float startX=5;
			float startY=50;
			
			if(minimiseList){
				if(GUI.Button(new Rect(startX, startY-20, 30, 18), ">>")) minimiseList=false;
			}
			else{
				if(GUI.Button(new Rect(startX, startY-20, 30, 18), "<<")) minimiseList=true;
			}
			Vector2 v2=DrawFactionAbilityList(startX, startY, factionAbilityList);
			
			startX=v2.x+35;
			
			if(factionAbilityList.Count==0) return;
			
			
			Rect visibleRect=new Rect(startX, startY, window.position.width-startX-5, window.position.height-startY-5);
			Rect contentRect=new Rect(startX, startY, contentWidth-startY-100, contentHeight);
			
			scrollPos2 = GUI.BeginScrollView(visibleRect, scrollPos2, contentRect);
			
				v2=DrawFactionAbilityConfigurator(startX, startY, factionAbilityList);
				contentWidth=v2.x;
				contentHeight=v2.y;
			
			GUI.EndScrollView();
			
			
			if(GUI.changed) EditorDBManager.SetDirtyFactionAbility();
		}
		
		
		
		Vector2 DrawFactionAbilityConfigurator(float startX, float startY, List<FactionAbility> factionAbilityList){
			FactionAbility ability=factionAbilityList[selectID];
			
			float cachedY=startY;
			//float cachedX=startX;
			//startX+=65;	//startY+=20;
			
			EditorUtilities.DrawSprite(new Rect(startX, startY, 60, 60), ability.icon);
			startX+=65;
			
			cont=new GUIContent("Name:", "The ability name to be displayed in game");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY/2, width, height), cont);
			ability.name=EditorGUI.TextField(new Rect(startX+spaceX-65, startY, width-5, height), ability.name);
			
			cont=new GUIContent("Icon:", "The ability icon to be displayed in game, must be a sprite");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			ability.icon=(Sprite)EditorGUI.ObjectField(new Rect(startX+spaceX-65, startY, width-5, height), ability.icon, typeof(Sprite), false);
			
			startX-=65;
			startY+=20+spaceY*0.5f;
			
			
			cont=new GUIContent("Only Available Via Perk:", "Check if the ability can only be added by perk ");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			ability.onlyAvailableViaPerk=EditorGUI.Toggle(new Rect(startX+160, startY, 40, height), ability.onlyAvailableViaPerk);
			
			startY+=10;
			
			int targetType=(int)ability.targetType;
			cont=new GUIContent("Target Type:", "Target type of the ability. Define which target the ability can be apply to");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			contL=new GUIContent[targetTypeLabel.Length];
			for(int i=0; i<contL.Length; i++) contL[i]=new GUIContent(targetTypeLabel[i], targetTypeTooltip[i]);
			targetType = EditorGUI.Popup(new Rect(startX+80, startY, width-40, 15), new GUIContent(""), targetType, contL);
			ability.targetType=(_TargetType)targetType;
			
			
			startY+=5;
			
			cont=new GUIContent("Duration:", "The duration (in round) of the buff/debuff effect on target. Depends on the combination turn mode and move order, a round typically means all unit/faction has took their turn\n\nWhen set to 0, the buff/debuff value will only last until the turn is ends.\n When set to -1, none of the buff/debuff effect will apply.");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			ability.duration=EditorGUI.IntField(new Rect(startX+80, startY, 40, height), ability.duration);
			
			startY+=5;
			
			cont=new GUIContent("Cost:", "AP cost to use the ability");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			ability.cost=EditorGUI.FloatField(new Rect(startX+80, startY, 40, height), ability.cost);
			
			cont=new GUIContent("Cooldown:", "The cooldown period (in turn) of the ability after used");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			ability.cooldown=EditorGUI.IntField(new Rect(startX+80, startY, 40, height), ability.cooldown);
			
			cont=new GUIContent("Use Limit:", "How many time the ability can be used in a single battle");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			ability.useLimit=EditorGUI.IntField(new Rect(startX+80, startY, 40, height), ability.useLimit);
			
			startY+=10;
			
			cont=new GUIContent("Target Selection:", "Check if the ability require target selection");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			ability.requireTargetSelection=EditorGUI.Toggle(new Rect(startX+100, startY, 40, height), ability.requireTargetSelection);
			
			
			//~ cont=new GUIContent("Range:", "Range of the ability");
			//~ EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			//~ if(ability.requireTargetSelection) ability.range=EditorGUI.IntField(new Rect(startX+80, startY, 40, height), ability.range);
			//~ else EditorGUI.LabelField(new Rect(startX+80, startY, 40, height), "-");
			
				cont=new GUIContent("AOE Range:", "Check if the ability require target selection");
				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
				if(ability.requireTargetSelection) ability.aoeRange=EditorGUI.IntField(new Rect(startX+80, startY, 40, height), ability.aoeRange);
				else EditorGUI.LabelField(new Rect(startX+80, startY, 40, height), "-");
			
			startY+=10;
			
			//~ cont=new GUIContent("Shoot:", "Check if the ability shoots at target");
			//~ EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			//~ if(ability.requireTargetSelection) ability.shoot=EditorGUI.Toggle(new Rect(startX+80, startY, 40, height), ability.shoot);
			//~ else EditorGUI.LabelField(new Rect(startX+80, startY, 40, height), "-");
			
			
			//~ cont=new GUIContent("ShootObject:", "Check if the ability shoots at target");
			//~ EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			//~ if(!ability.requireTargetSelection || !ability.shoot) EditorGUI.LabelField(new Rect(startX+80, startY, 40, height), "-");
			//~ else ability.shootObject=(GameObject)EditorGUI.ObjectField(new Rect(startX+80, startY, width-5, height), ability.shootObject, typeof(GameObject), false);
			
			
			cont=new GUIContent("EffectObject:", "Check if the ability shoots at target");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			ability.effectObject=(GameObject)EditorGUI.ObjectField(new Rect(startX+80, startY, width-5, height), ability.effectObject, typeof(GameObject), false);
			
			cont=new GUIContent("EffectObjTgt:", "The effect object to be spawned for each individual target unit when the ability is cast. 1 instance is spawned for each target unit if there's multiple unit.");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			ability.effectObjectOnTarget=(GameObject)EditorGUI.ObjectField(new Rect(startX+80, startY, width-5, height), ability.effectObjectOnTarget, typeof(GameObject), false);
			
			
			cont=new GUIContent("Effect Delay:", "Delay before the effect actually take place. It's for the visual effect to kicks in");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			ability.delayDuration=EditorGUI.FloatField(new Rect(startX+80, startY, 40, height), ability.delayDuration);
			
			
			startY+=10;
			
			
			cont=new GUIContent("Description:", "Ability description to be shown on the in game tooltip");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
		
			GUIStyle style=new GUIStyle("TextArea");
			style.wordWrap=true;
			ability.desp=EditorGUI.TextArea(new Rect(startX, startY+spaceY-3, 60+width, 120), ability.desp, style);
			startY+=120;
			
			
			
			
			
			
			
			
			
			float cachedHeight=startY;
			
			startX+=250;
			startY=cachedY+20+2*spaceY;
			
			
			cont=new GUIContent("Use Default Effect:", "Check if the ability use default TBTK effect");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			ability.useDefaultEffect=EditorGUI.Toggle(new Rect(startX+125, startY, 40, height), ability.useDefaultEffect);
			
			if(ability.useDefaultEffect){
				
				int type=(int)ability.type;
				cont=new GUIContent("Type:", "Type of the ability. Define what the ability do");
				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
				contL=new GUIContent[abilityTypeLabel.Length];
				for(int i=0; i<contL.Length; i++) contL[i]=new GUIContent(abilityTypeLabel[i], abilityTypeTooltip[i]);
				type = EditorGUI.Popup(new Rect(startX+40, startY, width-50, 15), new GUIContent(""), type, contL);
				ability.type=(FactionAbility._AbilityType)type;
				
				if(ability.type==FactionAbility._AbilityType.SpawnNew){
					ability.targetType=_TargetType.Tile;
					ability.requireTargetSelection=true;
					ability.aoeRange=0;
				}
				
				
				if(ability.type==FactionAbility._AbilityType.Generic){
					startY+=10;
					Vector2 v2=DrawEffectStats(startX, startY, ability);
					if(v2.y>cachedHeight) cachedHeight=v2.y;
				}
				
				if(ability.type==FactionAbility._AbilityType.SpawnNew){
					string[] unitNameList=EditorDBManager.GetUnitNameList();
					List<Unit> unitList=EditorDBManager.GetUnitList();
					cont=new GUIContent("Unit To Spawn:", "Unit to be spawned");
					GUI.Label(new Rect(startX, startY+=spaceY, width, height), cont);
					int ID=-1;
					for(int i=0; i<unitList.Count; i++){ if(unitList[i].gameObject==ability.spawnUnit) ID=i+1; }
					ID = EditorGUI.Popup(new Rect(startX, startY+spaceY-2, width-10, height), ID, unitNameList);
					if(ID>0) ability.spawnUnit=unitList[ID-1].gameObject;
					else if(ID==0) ability.spawnUnit=null;
				}
				
				if(ability.type==FactionAbility._AbilityType.ScanFogOfWar){
					GUI.Label(new Rect(startX, startY+=spaceY, width, height), "- No input required -");
				}
				
			}
			
			return new Vector2(startX, cachedHeight);
		}
		
		
		
		
		
		
		
		
		
		
		//~ private Rect listVisibleRect;
		//~ private Rect listContentRect;
		
		//~ private int deleteID=-1;
		//~ private bool minimiseList=false;
		Vector2 DrawFactionAbilityList(float startX, float startY, List<FactionAbility> factionAbilityList){
			
			float width=260;
			if(minimiseList) width=60;
			
			if(!minimiseList){
				if(GUI.Button(new Rect(startX+180, startY-20, 40, 18), "up")){
					if(selectID>0){
						FactionAbility ability=factionAbilityList[selectID];
						factionAbilityList[selectID]=factionAbilityList[selectID-1];
						factionAbilityList[selectID-1]=ability;
						selectID-=1;
						
						if(selectID*35<scrollPos1.y) scrollPos1.y=selectID*35;
					}
				}
				if(GUI.Button(new Rect(startX+222, startY-20, 40, 18), "down")){
					if(selectID<factionAbilityList.Count-1){
						FactionAbility ability=factionAbilityList[selectID];
						factionAbilityList[selectID]=factionAbilityList[selectID+1];
						factionAbilityList[selectID+1]=ability;
						selectID+=1;
						
						if(listVisibleRect.height-35<selectID*35) scrollPos1.y=(selectID+1)*35-listVisibleRect.height+5;
					}
				}
			}
			
			
			listVisibleRect=new Rect(startX, startY, width+15, window.position.height-startY-5);
			listContentRect=new Rect(startX, startY, width, factionAbilityList.Count*35+5);
			
			GUI.color=new Color(.8f, .8f, .8f, 1f);
			GUI.Box(listVisibleRect, "");
			GUI.color=Color.white;
			
			scrollPos1 = GUI.BeginScrollView(listVisibleRect, scrollPos1, listContentRect);
			
			//Debug.Log(scrollPos1.y+"   "+selectID*35+"    "+(scrollPos1.y+visibleRect.width));
			
			
			
				startY+=5;	startX+=5;
			
				for(int i=0; i<factionAbilityList.Count; i++){
					
					EditorUtilities.DrawSprite(new Rect(startX, startY+(i*35), 30, 30), factionAbilityList[i].icon);
					
					if(minimiseList){
						if(selectID==i) GUI.color = new Color(0, 1f, 1f, 1f);
						if(GUI.Button(new Rect(startX+35, startY+(i*35), 30, 30), "")) SelectAbility(i);
						GUI.color = Color.white;
						
						continue;
					}
					
					
					
					if(selectID==i) GUI.color = new Color(0, 1f, 1f, 1f);
					if(GUI.Button(new Rect(startX+35, startY+(i*35), 150, 30), factionAbilityList[i].name)) SelectAbility(i);
					GUI.color = Color.white;
					
					if(deleteID==i){
						
						if(GUI.Button(new Rect(startX+190, startY+(i*35), 60, 15), "cancel")) deleteID=-1;
						
						GUI.color = Color.red;
						if(GUI.Button(new Rect(startX+190, startY+(i*35)+15, 60, 15), "confirm")){
							if(selectID>=deleteID) SelectAbility(selectID-1);
							EditorDBManager.RemoveFactionAbility(deleteID);
							deleteID=-1;
						}
						GUI.color = Color.white;
					}
					else{
						if(GUI.Button(new Rect(startX+190, startY+(i*35), 60, 15), "remove")) deleteID=i;
					}
				}
			
			GUI.EndScrollView();
			
			return new Vector2(startX+width, startY);
		}
		
		
		
	}

}
