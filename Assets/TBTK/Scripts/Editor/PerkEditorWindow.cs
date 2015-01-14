using UnityEngine;
using UnityEditor;

using System;

using System.Collections;
using System.Collections.Generic;

using TBTK;

namespace TBTK {

	public class PerkEditorWindow : EditorWindow {
		
		private static PerkEditorWindow window;
		
		//~ private static PerkDB prefab;
		//~ private static List<Perk> perkList=new List<Perk>();
		//~ private static List<int> perkIDList=new List<int>();

		public static void Init () {
			// Get existing open window or if none, make a new one:
			window = (PerkEditorWindow)EditorWindow.GetWindow(typeof (PerkEditorWindow));
			//~ window.minSize=new Vector2(375, 449);
			//~ window.maxSize=new Vector2(375, 800);
			
			EditorDBManager.Init();
			
			InitLabel();
		}

		private static string[] perkTypeLabel;
		private static string[] perkTypeTooltip;
		
		
		private static void InitLabel(){
			int enumLength = Enum.GetValues(typeof(_PerkType)).Length;
			perkTypeLabel=new string[enumLength];
			perkTypeTooltip=new string[enumLength];
			for(int i=0; i<enumLength; i++){
				perkTypeLabel[i]=((_PerkType)i).ToString();
				if((_PerkType)i==_PerkType.Unit) 	perkTypeTooltip[i]="Modify stats of certains unit(s)";
				if((_PerkType)i==_PerkType.AllUnit) 	perkTypeTooltip[i]="Modify stats of all units";
				
				if((_PerkType)i==_PerkType.UnitAbility) 	perkTypeTooltip[i]="Modify stats of certains unit's ability(s)";
				if((_PerkType)i==_PerkType.AllUnitAbility) 	perkTypeTooltip[i]="Modify stats of all unit's abilities";
				
				if((_PerkType)i==_PerkType.FactionAbility) 	perkTypeTooltip[i]="Modify stats of certains faction's ability(s)";
				if((_PerkType)i==_PerkType.AllFactionAbility) 	perkTypeTooltip[i]="Modify stats of all faction's abilities";
				
				if((_PerkType)i==_PerkType.NewUnitAbility) 	perkTypeTooltip[i]="Add a unit ability to certain unit(s)";
				if((_PerkType)i==_PerkType.NewFactionAbility) 	perkTypeTooltip[i]="Add a faction ability";
			}
		}
		
		
		
		void SelectPerk(int ID){
			selectID=ID;
			GUI.FocusControl ("");
			
			if(selectID*35<scrollPos1.y) scrollPos1.y=selectID*35;
			if(selectID*35>scrollPos1.y+listVisibleRect.height-40) scrollPos1.y=selectID*35-listVisibleRect.height+40;
		}
		
		
		private int selectID=0;
		
		private Vector2 scrollPos1;
		private Vector2 scrollPos2;
		
		private GUIContent cont;
		private GUIContent[] contL;
		
		private float contentHeight=0;
		private float contentWidth=0;
		
		private float spaceX=120;
		private float spaceY=20;
		private float width=150;
		private float height=18;
		
		private bool foldPrereqList=true;
		
		void OnGUI () {
			if(window==null) Init();
			
			List<Perk> perkList=EditorDBManager.GetPerkList();
			
			if(GUI.Button(new Rect(window.position.width-120, 5, 100, 25), "Save")) EditorDBManager.SetDirtyPerk();
			
			
			if(GUI.Button(new Rect(5, 5, 120, 25), "Create New")){
				int newSelectID=EditorDBManager.AddNewPerk(new Perk());
				if(newSelectID!=-1) SelectPerk(newSelectID);
			}
			if(perkList.Count>0 && GUI.Button(new Rect(130, 5, 100, 25), "Clone Selected")){
				int newSelectID=EditorDBManager.AddNewPerk(perkList[selectID].Clone());
				if(newSelectID!=-1) SelectPerk(newSelectID);
			}
			
			
			float startX=5;
			float startY=55;
			
			
			if(minimiseList){
				if(GUI.Button(new Rect(startX, startY-20, 30, 18), ">>")) minimiseList=false;
			}
			else{
				if(GUI.Button(new Rect(startX, startY-20, 30, 18), "<<")) minimiseList=true;
			}
			Vector2 v2=DrawPerkList(startX, startY, perkList);	
			
			startX=v2.x+25;
			
			if(perkList.Count==0) return;
			
			
			Rect visibleRect=new Rect(startX, startY, window.position.width-startX-10, window.position.height-startY-5);
			Rect contentRect=new Rect(startX, startY, contentWidth-startY, contentHeight);
			
			//~ GUI.color=new Color(.8f, .8f, .8f, 1f);
			//~ GUI.Box(visibleRect, "");
			//~ GUI.color=Color.white;
			
			scrollPos2 = GUI.BeginScrollView(visibleRect, scrollPos2, contentRect);
			
				//float cachedX=startX;
				v2=DrawPerkConfigurator(startX, startY, perkList[selectID]);
				contentWidth=v2.x+50;
				contentHeight=v2.y-55;
			
			GUI.EndScrollView();
			
			
			if(GUI.changed) EditorDBManager.SetDirtyPerk();
		}
		
		int GetListIndexFromPerkID(int ID){
			List<Perk> perkList=EditorDBManager.GetPerkList();
			for(int i=0; i<perkList.Count; i++){ if(perkList[i].prefabID==ID) return i;}
			return 0;
		}
		int GetPerkIDFromListIndex(int index){ return EditorDBManager.GetPerkList()[index].prefabID; }
		
		
		Vector2 DrawPerkConfigurator(float startX, float startY, Perk perk){
			
			float cachedX=startX;
			float cachedY=startY;
			
			EditorUtilities.DrawSprite(new Rect(startX, startY, 60, 60), perk.icon);
			startX+=65;
			
			cont=new GUIContent("Name:", "The ability name to be displayed in game");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY/2, width, height), cont);
			perk.name=EditorGUI.TextField(new Rect(startX+spaceX-65, startY, width-5, height), perk.name);
			
			cont=new GUIContent("Icon:", "The ability icon to be displayed in game, must be a sprite");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			perk.icon=(Sprite)EditorGUI.ObjectField(new Rect(startX+spaceX-65, startY, width-5, height), perk.icon, typeof(Sprite), false);
			
			startX-=65;
			startY+=10+spaceY/2;	cachedY=startY;
			
			
			
			cont=new GUIContent("Cost:", "How many perk currency is required to purchase this perk");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			perk.cost=EditorGUI.IntField(new Rect(startX+spaceX, startY, 40, height), perk.cost);
			
			cont=new GUIContent("Min PerkPoint req:", "Minimum perk point to have before the perk becoming available");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			perk.minPerkPoint=EditorGUI.IntField(new Rect(startX+spaceX, startY, 40, height), perk.minPerkPoint);
			
			
			cont=new GUIContent("Prerequisite Perk:", "Perks that needs to be purchased before this perk is unlocked and become available");
			foldPrereqList=EditorGUI.Foldout(new Rect(startX, startY+=spaceY, spaceX, height), foldPrereqList, cont);
			
			if(foldPrereqList){
				//~ cont=new GUIContent("Prerequisite Perk:", "Perks that needs to be purchased before this perk is unlocked and become available");
				//~ EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
				
				int listIndex=0;
				string[] perkNameList=EditorDBManager.GetPerkNameList();
				for(int i=0; i<perk.prereq.Count; i++){
					listIndex=GetListIndexFromPerkID(perk.prereq[i])+1;
					listIndex=EditorGUI.Popup(new Rect(startX+spaceX, startY, width, height), listIndex, perkNameList);
					if(listIndex>0){
						int ID=GetPerkIDFromListIndex(listIndex-1);
						if(ID!=perk.prefabID && !perk.prereq.Contains(ID)) perk.prereq[i]=ID;
					}
					else{
						perk.prereq.RemoveAt(i);
						i-=1;
					}
					startY+=spaceY;
				}
				listIndex=0;
				listIndex = EditorGUI.Popup(new Rect(startX+spaceX, startY, width, height), listIndex, perkNameList);
				if(listIndex>0){
					int ID=GetPerkIDFromListIndex(listIndex-1);
					if(ID!=perk.prefabID && !perk.prereq.Contains(ID)) perk.prereq.Add(ID);
				}
			}
			else{
				EditorGUI.LabelField(new Rect(startX+spaceX, startY, width, height), perk.prereq.Count.ToString());
			}
			
			
			startY+=15;
			
			GUIStyle style=new GUIStyle("TextArea");
			style.wordWrap=true;
			cont=new GUIContent("Perk description (to be used in runtime): ", "");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, 400, 20), cont);
			perk.desp=EditorGUI.TextArea(new Rect(startX, startY+spaceY-3, 270, 150), perk.desp, style);
			startY+=spaceY+150;
			
			//~ float temp=cachedY;
			//~ cachedY=startY+15;	//startY=temp;	
			startX=cachedX+310;
			
			Vector2 v2=DrawPerkType(startX, cachedY, perk, startY+20);  float maxHeight=v2.y+40;
			
			//~ startX=cachedX;	startY=cachedY;	
			
			
			return new Vector2(startX+280, Mathf.Max(maxHeight, startY+170));
		}
		
		
		
		Vector2 DrawItemIDUnit(float startX, float startY, Perk perk, int limit=4, string tooltip=""){
			string[] unitNameList=EditorDBManager.GetUnitNameList();
			List<Unit> unitList=EditorDBManager.GetUnitList();
			
			if(perk.itemIDList.Count==0) perk.itemIDList.Add(-1);
			while(perk.itemIDList.Count>limit) perk.itemIDList.RemoveAt(perk.itemIDList.Count-1);
			
			if(tooltip=="") tooltip="The unit to be associated with this perk";
			
			for(int i=0; i<perk.itemIDList.Count; i++){
				int ID=perk.itemIDList[i];
				
				if(ID>=0){
					for(int n=0; n<unitList.Count; n++){ 
						if(unitList[n].prefabID==ID){ ID=n+1;	break; }
					}
				}
				
				cont=new GUIContent(" - Unit:", tooltip);
				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
				ID = EditorGUI.Popup(new Rect(startX+spaceX-30, startY, width, 15), ID, unitNameList);
				if(ID>0 && !perk.itemIDList.Contains(unitList[ID-1].prefabID)) perk.itemIDList[i]=unitList[ID-1].prefabID;
				else if(ID==0) perk.itemIDList[i]=-1;
				
				//if the list is full, extend it
				if(i==perk.itemIDList.Count-1 && ID>=0 && perk.itemIDList.Count<limit) perk.itemIDList.Add(-1);
				
				//if one of the element in the list is empty, shrink it
				if(i<perk.itemIDList.Count-1 && perk.itemIDList[i]==-1){ perk.itemIDList.RemoveAt(i); i-=1; }
			}
			
			return new Vector2(startX, startY);
		}
		
		Vector2 DrawItemIDUnitAbility(float startX, float startY, Perk perk, int limit=4){
			string[] unitAbilityNameList=EditorDBManager.GetUnitAbilityNameList();
			List<UnitAbility> unitAbilityList=EditorDBManager.GetUnitAbilityList();
			
			if(perk.itemIDList.Count==0) perk.itemIDList.Add(-1);
			while(perk.itemIDList.Count>limit) perk.itemIDList.RemoveAt(perk.itemIDList.Count-1);
			
			for(int i=0; i<perk.itemIDList.Count; i++){
				int ID=perk.itemIDList[i];
				
				if(ID>=0){
					for(int n=0; n<unitAbilityList.Count; n++){ 
						if(unitAbilityList[n].prefabID==ID){ ID=n+1;	break; }
					}
				}
				
				cont=new GUIContent(" - Ability:", "The ability to be associated with this perk");
				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
				ID = EditorGUI.Popup(new Rect(startX+spaceX-30, startY, width, 15), ID, unitAbilityNameList);
				if(ID>0 && !perk.itemIDList.Contains(unitAbilityList[ID-1].prefabID)) perk.itemIDList[i]=unitAbilityList[ID-1].prefabID;
				else if(ID==0) perk.itemIDList[i]=-1;
				
				//if the list is full, extend it
				if(i==perk.itemIDList.Count-1 && ID>=0 && perk.itemIDList.Count<limit) perk.itemIDList.Add(-1);
				
				//if one of the element in the list is empty, shrink it
				if(i<perk.itemIDList.Count-1 && perk.itemIDList[i]==-1){ perk.itemIDList.RemoveAt(i); i-=1; }
			}
			
			return new Vector2(startX, startY);
		}
		
		Vector2 DrawItemIDFactionAbility(float startX, float startY, Perk perk, int limit=4){
			string[] factionAbilityNameList=EditorDBManager.GetFactionAbilityNameList();
			List<FactionAbility> factionAbilityList=EditorDBManager.GetFactionAbilityList();
			
			if(perk.itemIDList.Count==0) perk.itemIDList.Add(-1);
			while(perk.itemIDList.Count>limit) perk.itemIDList.RemoveAt(perk.itemIDList.Count-1);
			
			for(int i=0; i<perk.itemIDList.Count; i++){
				int ID=perk.itemIDList[i];
				
				if(ID>=0){
					for(int n=0; n<factionAbilityList.Count; n++){ 
						if(factionAbilityList[n].prefabID==ID){ ID=n+1;	break; }
					}
				}
				
				cont=new GUIContent(" - Ability:", "The ability to be associated with this perk");
				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
				ID = EditorGUI.Popup(new Rect(startX+spaceX-30, startY, width, 15), ID, factionAbilityNameList);
				if(ID>0 && !perk.itemIDList.Contains(factionAbilityList[ID-1].prefabID)) perk.itemIDList[i]=factionAbilityList[ID-1].prefabID;
				else if(ID==0) perk.itemIDList[i]=-1;
				
				//if the list is full, extend it
				if(i==perk.itemIDList.Count-1 && ID>=0 && perk.itemIDList.Count<limit) perk.itemIDList.Add(-1);
				
				//if one of the element in the list is empty, shrink it
				if(i<perk.itemIDList.Count-1 && perk.itemIDList[i]==-1){ perk.itemIDList.RemoveAt(i); i-=1; }
			}
			
			return new Vector2(startX, startY);
		}
		
		
		
		Vector2 DrawAbilityID(float startX, float startY, Perk perk, bool isUnitAbility){
			int ID=perk.abilityID;
			string[] abilityNameList=new string[0];
			List<Ability> abilityList=new List<Ability>();
			
			if(isUnitAbility){
				abilityNameList=EditorDBManager.GetUnitAbilityNameList();
				List<UnitAbility> uAbilityList=EditorDBManager.GetUnitAbilityList();
				for(int i=0; i<uAbilityList.Count; i++) abilityList.Add(uAbilityList[i]);
			}
			else{
				abilityNameList=EditorDBManager.GetFactionAbilityNameList();
				List<FactionAbility> fAbilityList=EditorDBManager.GetFactionAbilityList();
				for(int i=0; i<fAbilityList.Count; i++) abilityList.Add(fAbilityList[i]);
			}
			
			if(ID>=0){
				for(int n=0; n<abilityList.Count; n++){ 
					if(abilityList[n].prefabID==ID){ ID=n+1;	break; }
				}
			}
			
			string tooltip="Ability to be added";
			if(isUnitAbility) tooltip+=" to the unit specified";
			
			cont=new GUIContent(" - Ability:", tooltip);
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			ID = EditorGUI.Popup(new Rect(startX+spaceX-30, startY, width, 15), ID, abilityNameList);
			perk.abilityID=(ID>0) ? abilityList[ID-1].prefabID : -1 ;
			
			return new Vector2(startX, startY);
		}
		
		
		
		
		
		
		Vector2 DrawPerkType(float startX, float startY, Perk perk, float startYAlt){
			int type=(int)perk.type;	int cachedType=type;
			cont=new GUIContent("Perk Type:", "What the perk does");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			contL=new GUIContent[perkTypeLabel.Length];
			for(int i=0; i<contL.Length; i++) contL[i]=new GUIContent(perkTypeLabel[i], perkTypeTooltip[i]);
			type = EditorGUI.Popup(new Rect(startX+spaceX-20, startY, width, 15), new GUIContent(""), type, contL);
			perk.type=(_PerkType)type;
			
			if(cachedType!=type) perk.itemIDList=new List<int>();
			
			
			_PerkType perkType=perk.type;
			
			
			startX+=10;	startY+=5;
			
			Vector2 v2=Vector2.zero;
			
			if(perkType==_PerkType.Unit || perkType==_PerkType.AllUnit){
				if(perkType==_PerkType.Unit) v2=DrawItemIDUnit(startX, startY, perk);	startY=v2.y;
				DrawUnitStats(startYAlt, perk, false);
			}
			
			if(perkType==_PerkType.UnitAbility || perkType==_PerkType.AllUnitAbility){
				if(perkType==_PerkType.UnitAbility){ v2=DrawItemIDUnitAbility(startX, startY, perk);	startY=v2.y; }
				DrawAbilityStat(startX, startY, perk);
				DrawUnitStats(startYAlt, perk, true);
			}
			
			if(perkType==_PerkType.FactionAbility || perkType==_PerkType.AllFactionAbility){
				if(perkType==_PerkType.FactionAbility){ v2=DrawItemIDFactionAbility(startX, startY, perk);		startY=v2.y; }
				DrawAbilityStat(startX, startY, perk, true);
				DrawUnitStats(startYAlt, perk, true);
			}
			
			if(perkType==_PerkType.NewUnitAbility){
				cont=new GUIContent("Add to all Unit:", "Check if the ability is to add to all player unit");
				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
				perk.addAbilityToAllUnit=EditorGUI.Toggle(new Rect(startX+spaceX-30, startY, 40, height), perk.addAbilityToAllUnit);
				if(!perk.addAbilityToAllUnit){
					v2=DrawItemIDUnit(startX, startY, perk, 1, "Unit to receive the new ability");		startY=v2.y;
				}
				v2=DrawAbilityID(startX, startY, perk, true);	startY=v2.y;
			}
			
			if(perkType==_PerkType.NewFactionAbility){
				DrawAbilityID(startX, startY, perk, false);		startY=v2.y;
			}
			
			
			return new Vector2(startX, startY);
		}
		
		
		Vector2 DrawUnitStats(float startYAlt, Perk perk, bool isAbility=false){
			float startX=300;	float startY=startYAlt;	spaceX-=10;
					//float cachedX=startX;		
			
			string text="";	float offset=0;
			text="Modifiers/Multipliers value to unit's stats:";
			if(isAbility){
				text="Modifiers/Multipliers value to ability's target's stats:\n*Depend on the ability target type, some of them may not applicable";
				offset=spaceY-3;
			}
			EditorGUI.LabelField(new Rect(startX, startY, 500, 2*height), text);
			EditorGUI.LabelField(new Rect(startX, startY, 500, 2*height), "__________________________________________");
			
			float cachedY=(startY+=3+offset);
			
			UnitStat stats=perk.stats;
			
			cont=new GUIContent("HP Buff:", "HP buff multiplier to be applied to the unit. Takes value from 0 and above with 0.3 being increment of 30%");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			stats.HPBuff=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), stats.HPBuff);
			cont=new GUIContent("AP Buff:", "AP buff multiplier to be applied to the unit. Takes value from 0 and above with 0.3 being increment of 30%");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			stats.HPBuff=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), stats.HPBuff);
			
			startY+=10;
			
			cont=new GUIContent("Move AP Cost:", "Move cost modifier to be applied to the unit. Value is used to directly modify the cost. ie, -2 reduce the cost by 2");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			stats.moveAPCost=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), stats.moveAPCost);
			cont=new GUIContent("Attack AP Cost:", "Attack cost modifier to be applied to the unit. Value is used to directly modify the cost. ie, -2 reduce the cost by 2");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			stats.attackAPCost=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), stats.attackAPCost);
			
			startY+=10;
			
			cont=new GUIContent("Turn Priority:", "Priority modifier to be applied to the unit. Value is used to directly modify the base value. ie, -2 reduce turnPriority by 2");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			stats.turnPriority=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), stats.turnPriority);
			
			startY+=10;
			
			cont=new GUIContent("Move Range:", "Move range modifier to be applied to the unit. Value is used to directly modify the base value. ie, 2 incrase move range by 2");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			stats.moveRange=EditorGUI.IntField(new Rect(startX+spaceX, startY, 40, height), stats.moveRange);
			cont=new GUIContent("Attack Range:", "Attack range modifier to be applied to the unit. Value is used to directly modify the base value. ie, 2 incrase attack range by 2");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			stats.attackRange=EditorGUI.IntField(new Rect(startX+spaceX, startY, 40, height), stats.attackRange);
			cont=new GUIContent("Sight Range:", "Sight range modifier to be applied to the unit. Value is used to directly modify the base value. ie, 2 incrase sight range by 2");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			stats.sight=EditorGUI.IntField(new Rect(startX+spaceX, startY, 40, height), stats.sight);
			
			startY+=10;
			
			cont=new GUIContent("MovePerTurn:", "Move per turn modifier to be applied to the unit. Value is used to directly modify the base value. ie, 2 incrase move available in a turn by 2");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			stats.movePerTurn=EditorGUI.IntField(new Rect(startX+spaceX, startY, 40, height), stats.movePerTurn);
			cont=new GUIContent("AttackPerTurn:", "Attack per turn modifier to be applied to the unit. Value is used to directly modify the base value. ie, 2 incrase attack available in a turn by 2");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			stats.attackPerTurn=EditorGUI.IntField(new Rect(startX+spaceX, startY, 40, height), stats.attackPerTurn);
			cont=new GUIContent("CounterPerTurn:", "Counter per turn modifier to be applied to the unit. Value is used to directly modify the base value. ie, 2 incrase counter available in a turn by 2");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			stats.counterPerTurn=EditorGUI.IntField(new Rect(startX+spaceX, startY, 40, height), stats.counterPerTurn);
			
			startX+=200;	startY=cachedY;		spaceX+=10;
			
			cont=new GUIContent("Damage:", "Damage multiplier to be applied to the unit. Takes value from 0 and above with 0.3 being increment of 30%");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			stats.damage=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), stats.damage);
			//~ cont=new GUIContent("DamageMax:", "");
			//~ EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			//~ stats.damageMax=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), stats.damageMax);
			
			startY+=10;
			
			cont=new GUIContent("Hit Chance:", "Hit chance modifier to be applied to the unit. Value is used to directly modify the base value. ie, 0.2 increase hit chance by 20%");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			stats.hitChance=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), stats.hitChance);
			cont=new GUIContent("Dodge Chance:", "Dodge chance modifier to be applied to the unit. Value is used to directly modify the base value. ie, 0.2 increase dodge chance by 20%");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			stats.dodgeChance=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), stats.dodgeChance);
			
			startY+=10;
			
			cont=new GUIContent("Critical Chance:", "Critical chance modifier to be applied to the unit. Value is used to directly modify the base value. ie, 0.2 increase critical chance by 20%");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			stats.critChance=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), stats.critChance);
			cont=new GUIContent("Critical Avoidance:", "Critical avoidance modifier to be applied to the unit. Value is used to directly modify the base value. ie, 0.2 increase critical avoidance by 20%");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			stats.critAvoidance=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), stats.critAvoidance);
			cont=new GUIContent("Critical Multiplier:", "Critical damage modifier to be applied to the unit. Value is used to directly modify the base value. ie, 0.2 increase critical damage by 20%");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			stats.critMultiplier=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), stats.critMultiplier);
			
			startY+=10;
			
			cont=new GUIContent("Flanking Bonus:", "Damage multiplier to be applied to the unit when flanking a target. Takes value from 0 and above with 0.2 being increase the damage by 20%");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			stats.flankingBonus=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), stats.flankingBonus);
			cont=new GUIContent("Flanked Modifier:", "Damage multiplier to be applied to the unit when being flanked. Takes value from 0 and above with 0.2 being reduce the incoming damage by 20%");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			stats.flankedModifier=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), stats.flankedModifier);
			
			startY+=10;
			
			cont=new GUIContent("HP Gain Per Turn:", "HP Gain per turn multiplier to be applied to the unit. Takes value from 0 and above with 0.3 being increment of 30%");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			stats.HPPerTurn=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), stats.HPPerTurn);
			cont=new GUIContent("AP Gain Per Turn:", "AP Gain per turn multiplier to be applied to the unit. Takes value from 0 and above with 0.3 being increment of 30%");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			stats.APPerTurn=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), stats.APPerTurn);
			
			
			startX+=200;	startY=cachedY;		spaceX-=10;
			
			cont=new GUIContent("Stun Chance:", "Stun chance modifier to be applied to the unit. Value is used to directly modify the base value. ie, 0.2 increase the chance by 20%");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			stats.stunChance=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), stats.stunChance);
			cont=new GUIContent("Stun Avoidance:", "Stun avoidance modifier to be applied to the unit. Value is used to directly modify the base value. ie, 0.2 increase stun avoidance by 20%");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			stats.stunAvoidance=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), stats.stunAvoidance);
			cont=new GUIContent("Stun Duration:", "Stun duration modifier to be applied to the unit. Value is used to directly modify the base value. ie, 2 incrase stun duration by 2");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			stats.stunDuration=EditorGUI.IntField(new Rect(startX+spaceX, startY, 40, height), stats.stunDuration);
			
			startY+=10;
			
			cont=new GUIContent("Silent Chance:", "Silent chance modifier to be applied to the unit. Value is used to directly modify the base value. ie, 0.2 increase the chance by 20%");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			stats.silentChance=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), stats.silentChance);
			cont=new GUIContent("Silent Avoidance:", "Silent avoidance modifier to be applied to the unit. Value is used to directly modify the base value. ie, 0.2 increase silent avoidance by 20%");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			stats.silentAvoidance=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), stats.silentAvoidance);
			cont=new GUIContent("Silent Duration:", "Silent duration modifier to be applied to the unit. Value is used to directly modify the base value. ie, 2 incrase silent duration by 2");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			stats.silentDuration=EditorGUI.IntField(new Rect(startX+spaceX, startY, 40, height), stats.silentDuration);
			
			spaceX+=10;
			
			return new Vector2(startX, startY);
		}
		
		
		Vector2 DrawAbilityStat(float startX, float startY, Perk perk, bool isFactionAbility=false){
			spaceX-=30;
			
			startY+=5;
			
			cont=new GUIContent("Duration:", "Effect duration modifier to be applied to the ability. Value is used to directly modify the base value. ie. 2 increase the effect duration by 2");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			perk.abDurationMod=EditorGUI.IntField(new Rect(startX+spaceX, startY, 40, height), perk.abDurationMod);
			cont=new GUIContent("Cost:", "Energy cost multiplier to be applied to the ability. Takes value from 0 and above with 0.3 being reduce cost by 30%");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			perk.abCostMod=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), perk.abCostMod);
			
			startY+=5;
			
			cont=new GUIContent("Cooldown:", "Cooldown duration modifier to be applied to the ability. Value is used to directly modify the base value. ie. -2 decrease the cooldown duration by 2");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			perk.abCooldownMod=EditorGUI.IntField(new Rect(startX+spaceX, startY, 40, height), perk.abCooldownMod);
			cont=new GUIContent("UseLimit:", "Limit modifier to be applied to the ability. Value is used to directly modify the base value. ie. -2 decrease the use limit by 2");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			perk.abUseLimitMod=EditorGUI.IntField(new Rect(startX+spaceX, startY, 40, height), perk.abUseLimitMod);
			
			startY+=5;
			
			cont=new GUIContent("Range:", "Range modifier to be applied to the ability. Value is used to directly modify the base value. ie, 2 incrase range by 2");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			if(isFactionAbility) EditorGUI.LabelField(new Rect(startX+spaceX, startY, 40, height), "-");
			else perk.abRangeMod=EditorGUI.IntField(new Rect(startX+spaceX, startY, 40, height), perk.abRangeMod);
			cont=new GUIContent("AOE Range:", "AOE range modifier to be applied to the ability. Value is used to directly modify the base value. ie, 2 incrase aoe range by 2");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			perk.abAOERangeMod=EditorGUI.IntField(new Rect(startX+spaceX, startY, 40, height), perk.abAOERangeMod);
			
			startY+=5;
			
			cont=new GUIContent("HP Min/Max:", "HP effect (modify target's HP directly) multiplier to be applied to the ability. Takes value from 0 and above with 0.3 being increase the ability HP effect by 30%");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			perk.abHPMod=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), perk.abHPMod);
			//~ cont=new GUIContent("HP Max:", "");
			//~ EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			//perk.abHPMaxMod=EditorGUI.FloatField(new Rect(startX+spaceX+45, startY, 40, height), perk.abHPMaxMod);
			cont=new GUIContent("AP Min/Max:", "HP effect (modify target's AP directly) multiplier to be applied to the ability. Takes value from 0 and above with 0.3 being increase the ability HP effect by 30%");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			perk.abAPMod=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), perk.abAPMod);
			//~ cont=new GUIContent("AP Max:", "");
			//~ EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			//perk.abAPMaxMod=EditorGUI.FloatField(new Rect(startX+spaceX+45, startY, 40, height), perk.abAPMaxMod);
			
			startY+=5;
			
			cont=new GUIContent("Stun:", "Stun effect duration modifier to be applied to the ability. Value is used to directly modify the base value. ie, 2 incrase stun duration by 2");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			perk.abStunMod=EditorGUI.IntField(new Rect(startX+spaceX, startY, 40, height), perk.abStunMod);
			
			spaceX+=30;
			
			return new Vector2(startX, startY);
		}
		
		
		
		
		
		
		
		
		
		
		private Rect listVisibleRect;
		private Rect listContentRect;
		
		private int deleteID=-1;
		private bool minimiseList=false;
		Vector2 DrawPerkList(float startX, float startY, List<Perk> perkList){
			
			float width=260;
			if(minimiseList) width=60;
			
			
			if(!minimiseList){
				if(GUI.Button(new Rect(startX+180, startY-20, 40, 18), "up")){
					if(selectID>0){
						Perk perk=perkList[selectID];
						perkList[selectID]=perkList[selectID-1];
						perkList[selectID-1]=perk;
						selectID-=1;
						
						if(selectID*35<scrollPos1.y) scrollPos1.y=selectID*35;
					}
				}
				if(GUI.Button(new Rect(startX+222, startY-20, 40, 18), "down")){
					if(selectID<perkList.Count-1){
						Perk perk=perkList[selectID];
						perkList[selectID]=perkList[selectID+1];
						perkList[selectID+1]=perk;
						selectID+=1;
						
						if(listVisibleRect.height-35<selectID*35) scrollPos1.y=(selectID+1)*35-listVisibleRect.height+5;
					}
				}
			}
			
			
			listVisibleRect=new Rect(startX, startY, width+15, window.position.height-startY-5);
			listContentRect=new Rect(startX, startY, width, perkList.Count*35+5);
			
			GUI.color=new Color(.8f, .8f, .8f, 1f);
			GUI.Box(listVisibleRect, "");
			GUI.color=Color.white;
			
			scrollPos1 = GUI.BeginScrollView(listVisibleRect, scrollPos1, listContentRect);
			
			
				startY+=5;	startX+=5;
			
				for(int i=0; i<perkList.Count; i++){
					
					EditorUtilities.DrawSprite(new Rect(startX, startY+(i*35), 30, 30), perkList[i].icon);
					
					if(minimiseList){
						if(selectID==i) GUI.color = new Color(0, 1f, 1f, 1f);
						if(GUI.Button(new Rect(startX+35, startY+(i*35), 30, 30), "")) SelectPerk(i);
						GUI.color = Color.white;
						
						continue;
					}
					
					
					
					if(selectID==i) GUI.color = new Color(0, 1f, 1f, 1f);
					if(GUI.Button(new Rect(startX+35, startY+(i*35), 150, 30), perkList[i].name)) SelectPerk(i);
					GUI.color = Color.white;
					
					if(deleteID==i){
						
						if(GUI.Button(new Rect(startX+190, startY+(i*35), 60, 15), "cancel")) deleteID=-1;
						
						GUI.color = Color.red;
						if(GUI.Button(new Rect(startX+190, startY+(i*35)+15, 60, 15), "confirm")){
							if(selectID>=deleteID) SelectPerk(selectID-1);
							perkList.RemoveAt(deleteID);
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