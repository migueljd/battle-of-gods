using UnityEngine;
using UnityEditor;

using System;

using System.Collections;
using System.Collections.Generic;

using TBTK;

namespace TBTK {

	public class UnitEditorWindow : EditorWindow {
		
		private static UnitEditorWindow window;
		
		public static void Init () {
			// Get existing open window or if none, make a new one:
			window = (UnitEditorWindow)EditorWindow.GetWindow(typeof (UnitEditorWindow));
			window.minSize=new Vector2(878, 665);
			//window.maxSize=new Vector2(870, 597);
			
			EditorDBManager.Init();
			
			InitLabel();
			UpdateObjectHierarchyList();
		}
		
		
		private static void InitLabel(){
			//~ int enumLength = Enum.GetValues(typeof(UnitAbility._TargetType)).Length;
			//~ targetTypeLabel=new string[enumLength];
			//~ targetTypeTooltip=new string[enumLength];
			//~ for(int i=0; i<enumLength; i++){
				//~ targetTypeLabel[i]=((UnitAbility._TargetType)i).ToString();
				//~ if((UnitAbility._TargetType)i==UnitAbility._TargetType.AllUnit) 	targetTypeTooltip[i]="Target All Units";
			//~ }
		}
		
		
		private static List<GameObject> objHList=new List<GameObject>();
		private static string[] objHLabelList=new string[0];
		private static void UpdateObjectHierarchyList(){
			List<Unit> unitList=EditorDBManager.GetUnitList();
			if(unitList.Count==0 || selectID>=unitList.Count) return;
			EditorUtilities.GetObjectHierarchyList(unitList[selectID].gameObject, SetObjListCallback);
		}
		public static void SetObjListCallback(List<GameObject> objList, string[] labelList){
			objHList=objList;
			objHLabelList=labelList;
		}
		
		void SelectUnit(int ID){
			selectID=ID;
			UpdateObjectHierarchyList();
			GUI.FocusControl ("");
			
			if(selectID*35<scrollPos1.y) scrollPos1.y=selectID*35;
			if(selectID*35>scrollPos1.y+listVisibleRect.height-40) scrollPos1.y=selectID*35-listVisibleRect.height+40;
		}
		
		
		private Vector2 scrollPos1;
		private Vector2 scrollPos2;
		
		private static int selectID=0;
		private float contentHeight=0;
		private float contentWidth=0;
		
		private GUIContent cont;
		private GUIContent[] contL;
		
		protected static float spaceX=110;
		protected static float spaceY=20;
		protected static float width=150;
		protected static float height=18;
		
		protected static bool shootPointFoldout=false;
		
		
		
		void OnGUI () {
			if(window==null) Init();
			
			List<Unit> unitList=EditorDBManager.GetUnitList();
			
			if(GUI.Button(new Rect(window.position.width-120, 5, 100, 25), "Save")) EditorDBManager.SetDirtyUnit();
			
			EditorGUI.LabelField(new Rect(5, 7, 150, 17), "Add new unit:");
			Unit newUnit=null;
			newUnit=(Unit)EditorGUI.ObjectField(new Rect(100, 7, 140, 17), newUnit, typeof(Unit), false);
			if(newUnit!=null){
				int newSelectID=EditorDBManager.AddNewUnit(newUnit);
				if(newSelectID!=-1) SelectUnit(newSelectID);
			}
			
			
			float startX=5;
			float startY=50;
			
			if(minimiseList){
				if(GUI.Button(new Rect(startX, startY-20, 30, 18), ">>")){
					minimiseList=false;
					//window.minSize=new Vector2(878, 595);
					//window.Repaint();
				}
			}
			else{
				if(GUI.Button(new Rect(startX, startY-20, 30, 18), "<<")){
					minimiseList=true;
					//window.minSize=new Vector2(678, 595);
					//window.position=new Rect(window.position.x, window.position.y-5, 678, 595);
				}
			}
			Vector2 v2=DrawUnitList(startX, startY, unitList);
			
			startX=v2.x+25;
			
			if(unitList.Count==0) return;
			
			cont=new GUIContent("Unit Prefab:", "The prefab object of the unit\nClick this to highlight it in the ProjectTab");
			EditorGUI.LabelField(new Rect(startX, startY, width, height), cont);
			EditorGUI.ObjectField(new Rect(startX+90, startY, 185, height), unitList[selectID].gameObject, typeof(GameObject), false);
			
			//cont=new GUIContent("Disable in BuildManager:", "When checked, tower won't appear on BuildManager list and thus can't be built\nThis is to mark towers that can only be upgrade from a built tower");
			//EditorGUI.LabelField(new Rect(startX+295, startY, width, height), cont);
			//unitList[selectID].disableInBuildManager=EditorGUI.Toggle(new Rect(startX+440, startY, 185, height), unitList[selectID].disableInBuildManager);
			
			startY+=spaceY+10;
			
			
			Rect visibleRect=new Rect(startX, startY, window.position.width-startX-10, window.position.height-startY-5);
			Rect contentRect=new Rect(startX, startY, contentWidth-startY, contentHeight);
			
			scrollPos2 = GUI.BeginScrollView(visibleRect, scrollPos2, contentRect);
			
				v2=DrawUnitConfigurator(startX, startY, unitList);
				//contentWidth=v2.x;
				//contentHeight=v2.y;
			
			GUI.EndScrollView();
			
			
			if(GUI.changed) EditorDBManager.SetDirtyUnit();
		}
		
		
		
		Vector3 DrawUnitConfigurator(float startX, float startY, List<Unit> unitList){
			//public static Vector3 DrawIconAndName(Unit unit, float startX, float startY){
			float cachedX=startX;
			float cachedY=startY;
			
			
			Unit unit=unitList[selectID];
			
			
			EditorUtilities.DrawSprite(new Rect(startX, startY, 60, 60), unit.iconSprite);
			startX+=65;
			
			cont=new GUIContent("Name:", "The unit name to be displayed in game");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY/2, width, height), cont);
			unit.unitName=EditorGUI.TextField(new Rect(startX+spaceX-65, startY, width-5, height), unit.unitName);
			
			cont=new GUIContent("Icon:", "The unit icon to be displayed in game, must be a sprite");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			unit.iconSprite=(Sprite)EditorGUI.ObjectField(new Rect(startX+spaceX-65, startY, width-5, height), unit.iconSprite, typeof(Sprite), false);
			
			startX-=65;
			startY=cachedY+spaceY/2;
			startX+=35+spaceX+width;	//startY+=20;
			cont=new GUIContent("HitPoint(HP):", "The unit default's HitPoint.\nThis is the base value to be modified by various in game bonus.");
			EditorGUI.LabelField(new Rect(startX, startY, width, height), cont);
			unit.defaultHP=EditorGUI.FloatField(new Rect(startX+100, startY, 40, height), unit.defaultHP);
			unit.HP=unit.defaultHP;
			
			cont=new GUIContent("ActionPoint(AP):", "The unit default's HitPoint.\nThis is the base value to be modified by various in game bonus.");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			unit.defaultHP=EditorGUI.FloatField(new Rect(startX+100, startY, 40, height), unit.defaultHP);
			unit.AP=unit.defaultAP;
			
			
			startY=cachedY+spaceY/2;
			startX+=35+spaceX;	//startY+=20;
			
			cont=new GUIContent("Regen/Turn:", "HitPoint (HP) gain/lost per turn");
			EditorGUI.LabelField(new Rect(startX, startY, width, height), cont);
			unit.HPPerTurn=EditorGUI.FloatField(new Rect(startX+80, startY, 30, height), unit.HPPerTurn);
			
			cont=new GUIContent("Regen/Turn:", "ActionPoint (AP) gain/lost per turn");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			unit.APPerTurn=EditorGUI.FloatField(new Rect(startX+80, startY, 30, height), unit.APPerTurn);
			
			
			startX=cachedX;
			startY=cachedY+height+35;
			
			
				cont=new GUIContent("Unit Value:", "The value of the unit. Used in unit generation on grid if the limit mode is set to be based on value.");
				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
				unit.value=EditorGUI.IntField(new Rect(startX+spaceX, startY, 40, height), unit.value);
				
				
			
			startY+=10;
			
			
				string[] armorTypeLabel=EditorDBManager.GetArmorTypeLabel();
				cont=new GUIContent("Armor Type:", "The armor type of the unit\nArmor type can be configured in Damage Armor Table Editor");
				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
				unit.armorType=EditorGUI.Popup(new Rect(startX+spaceX, startY, width, height), unit.armorType, armorTypeLabel);
				
				int objID=GetObjectIDFromHList(unit.targetPoint, objHList);
				cont=new GUIContent("TargetPoint:", "The transform object which indicate the center point of the unit\nThis would be the point where the shootObject and unit will be aiming at");
				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
				objID = EditorGUI.Popup(new Rect(startX+spaceX, startY, width, height), objID, objHLabelList);
				unit.targetPoint = (objHList[objID]==null) ? null : objHList[objID].transform;
				
				cont=new GUIContent("Hit Threshold:", "The range from the targetPoint where a shootObject is considered reached the target");
				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
				unit.hitThreshold=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), unit.hitThreshold);
				
			startY+=10;
				
				cont=new GUIContent("Move Speed:", "The unit's move speed when moving on the grid. This is cosmetic and has no effect on the core game mechanic");
				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
				unit.moveSpeed=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), unit.moveSpeed);
				
				//cont=new GUIContent("Rotate Speed:", "The unit's rotate speed when moving on the grid. This is cosmetic and has no effect on the core game mechanic");
				//EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
				//unit.rotateSpeed=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), unit.rotateSpeed);
			
			
			startX+=295;
			startY=cachedY+height+35;
			
				string[] damageTypeLabel=EditorDBManager.GetDamageTypeLabel();
				cont=new GUIContent("Damage Type:", "The damage type of the unit\nDamage type can be configured in Damage Armor Table Editor");
				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
				unit.damageType=EditorGUI.Popup(new Rect(startX+spaceX, startY, width, height), unit.damageType, damageTypeLabel);
			
				cont=new GUIContent("Require LOS to Attack:", "Check if the unit require target to be in direct line-of-sight to attack. Otherwise the unit can attack any target as long as it's seen by other friendly unit.\nThis only applicable if Fog-Of-War is enabled");
				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
				unit.requireDirectLOSToAttack=EditorGUI.Toggle(new Rect(startX+140, startY, 40, height), unit.requireDirectLOSToAttack);
			
				cont=new GUIContent("ShootPoint:", "The transform which indicate the position where the shootObject will be fired from (Optional)\nEach shootPoint assigned will fire a shootObject instance in each attack\nIf left empty, the unit transform itself will be use as the shootPoint\nThe orientation of the shootPoint matter as they dictate the orientation of the shootObject starting orientation.\n");
				shootPointFoldout=EditorGUI.Foldout(new Rect(startX, startY+=spaceY, spaceX, height), shootPointFoldout, cont);
				int shootPointCount=unit.shootPointList.Count;
				shootPointCount=EditorGUI.IntField(new Rect(startX+spaceX, startY, 40, height), shootPointCount);
				
				//if(!EditorGUIUtility.editingTextField && shootPointCount!=unit.shootPoints.Count){
				if(shootPointCount!=unit.shootPointList.Count){
					while(unit.shootPointList.Count<shootPointCount) unit.shootPointList.Add(null);
					while(unit.shootPointList.Count>shootPointCount) unit.shootPointList.RemoveAt(unit.shootPointList.Count-1);
				}
					
				if(shootPointFoldout){
					for(int i=0; i<unit.shootPointList.Count; i++){
						//GetObjectID(unit.shootPoints[i]);
						objID=GetObjectIDFromHList(unit.shootPointList[i], objHList);
						EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), "    - Element "+(i+1));
						objID = EditorGUI.Popup(new Rect(startX+spaceX, startY, width, height), objID, objHLabelList);
						unit.shootPointList[i] = (objHList[objID]==null) ? null : objHList[objID].transform;
					}
				}
				
				if(unit.shootPointList.Count>1){
					cont=new GUIContent("Shots delay Between ShootPoint:", "Delay in second between shot fired at each shootPoint. When set to zero all shootPoint fire simulteneously");
					EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width+60, height), cont);
					unit.delayBetweenShootPoint=EditorGUI.FloatField(new Rect(startX+spaceX+90, startY-1, 55, height-1), unit.delayBetweenShootPoint);
				}
			
				startY+=5;
				
				cont=new GUIContent("ShootObject:", "The shootObject that the unit used. All unit must have one in order to attack.");
				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
				unit.shootObject=(ShootObject)EditorGUI.ObjectField(new Rect(startX+spaceX, startY, width, height), unit.shootObject, typeof(ShootObject), false);
			
				//GetObjectID(unit.turretObject);
				objID=GetObjectIDFromHList(unit.turretObject, objHList);
				cont=new GUIContent("TurretObject:", "The object under unit's hierarchy which is used to aim toward target (Optional). When left unassigned, no aiming will be done.");
				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
				objID = EditorGUI.Popup(new Rect(startX+spaceX, startY, width, height), objID, objHLabelList);
				unit.turretObject = (objHList[objID]==null) ? null : objHList[objID].transform;
				
				//GetObjectID(unit.barrelObject);
				objID=GetObjectIDFromHList(unit.barrelObject, objHList);
				cont=new GUIContent("BarrelObject:", "The object under unit's hierarchy which is used to aim toward target (Optional). This is only required if the unit barrel and turret rotates independently on different axis");
				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
				objID = EditorGUI.Popup(new Rect(startX+spaceX, startY, width, height), objID, objHLabelList);
				unit.barrelObject = (objHList[objID]==null) ? null : objHList[objID].transform;
				
				startY+=5;
				
				
				cont=new GUIContent("Rotate turret only when aiming:", "Check if the unit only rotate it's turret only to aim when attacking");
				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width+35, height), cont);
				unit.rotateTurretOnly=EditorGUI.Toggle(new Rect(startX+spaceX+75, startY, 40, height), unit.rotateTurretOnly);
				
				cont=new GUIContent("Rotate in x-axis when aiming:", "Check if the unit turret/barrel can rotate in x-axis (elevation) to aim when attacking");
				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width+35, height), cont);
				unit.rotateTurretAimInXAxis=EditorGUI.Toggle(new Rect(startX+spaceX+75, startY, 40, height), unit.rotateTurretAimInXAxis);
			
				
			//DrawAbility(startX, startY, unit);
				
				
			
			//DrawStats(startX+295, cachedY+height+35, unit);
			//Debug.Log(startY);
			
			startX=cachedX;
			startY=Mathf.Max(startY+40, 340);
			
			if(currentTab==_Tab.Stats) GUI.color=new Color(.5f, 1f, 1f, 1f);
			if(GUI.Button(new Rect(startX, startY, 120, 25), "Stats")){
				currentTab=_Tab.Stats;
			}
			GUI.color=Color.white;
			if(currentTab==_Tab.Ability) GUI.color=new Color(.5f, 1f, 1f, 1f);
			if(GUI.Button(new Rect(startX+130, startY, 120, 25), "Ability ("+unit.abilityIDList.Count+")")){
				currentTab=_Tab.Ability;
			}
			GUI.color=Color.white;
			if(currentTab==_Tab.AudioNAnimation) GUI.color=new Color(.5f, 1f, 1f, 1f);
			if(GUI.Button(new Rect(startX+260, startY, 120, 25), "Audio & Animation")){
				currentTab=_Tab.AudioNAnimation;
			}
			GUI.color=Color.white;
			if(currentTab==_Tab.Desp) GUI.color=new Color(.5f, 1f, 1f, 1f);
			if(GUI.Button(new Rect(startX+390, startY, 120, 25), "Description")){
				currentTab=_Tab.Desp;
			}
			GUI.color=Color.white;
			
			startY+=30;
			GUI.Box(new Rect(startX, startY, 558, window.position.height-startY-5), "");
			
			
			startX=cachedX+5;		startY-=spaceY-10;
			
			if(currentTab==_Tab.Stats){
				DrawStats(startX, startY, unit);
			}
			else if(currentTab==_Tab.Ability){
				DrawAbility(startX, startY, unit);
			}
			else if(currentTab==_Tab.AudioNAnimation){
				DrawAudioNAnimation(startX, startY, unit);
			}
			else if(currentTab==_Tab.Desp){
				DrawDesp(startX, startY, unit);
			}
			
			startX-=5;		startY-=5;
			
			return new Vector2(startX, startY+spaceY);
		}
		
		
		private enum _Tab{Stats, Ability, AudioNAnimation, Desp}
		private _Tab currentTab=_Tab.Stats;
		
		
		Vector2 DrawDesp(float startX, float startY, Unit unit){
			startY+=15;
			startX+=5;
			
			cont=new GUIContent("Description:", "Ability description to be shown on the in game tooltip");
			EditorGUI.LabelField(new Rect(startX, startY, width, height), cont);
		
			GUIStyle style=new GUIStyle("TextArea");
			style.wordWrap=true;
			unit.desp=EditorGUI.TextArea(new Rect(startX, startY+spaceY-3, 530, 120), unit.desp, style);
			startY+=120;
			
			return Vector2.zero;
		}
		
		
		private Vector2 scrollPosAbility;
		Vector2 DrawAbility(float startX, float startY, Unit unit){
			
			startY+=10;
			
			List<UnitAbility> abilityList=EditorDBManager.GetUnitAbilityList();
			
			//~ Rect visibleRect=new Rect(startX, startY, window.position.width-startX-10, window.position.height-startY-5);
			Rect visibleRect=new Rect(startX, startY, 568, window.position.height-startY-5);
			Rect contentRect=new Rect(startX, startY, 20, Mathf.Ceil((float)abilityList.Count/9f)*62);
			
			//Debug.Log((window.position.width-startX-10)+"    "+(abilityList.Count/9));
			
			scrollPosAbility = GUI.BeginScrollView(visibleRect, scrollPosAbility, contentRect);
			
			startY+=5;
			int count=0;
			for(int i=0; i<abilityList.Count; i++){
				if(abilityList[i].onlyAvailableViaPerk) continue;
				
				if(count==9){
					startY+=62;
					count=0;
				}
				
				bool inList=unit.abilityIDList.Contains(abilityList[i].prefabID);
				if(inList) GUI.color=new Color(0f, 1f, .35f, 1f);
				
				cont=new GUIContent("", abilityList[i].name);
				bool flag=GUI.Button(new Rect(startX+count*62, startY, 50, 50), cont);	GUI.color=Color.white;
				EditorUtilities.DrawSprite(new Rect(startX+count*62+5, startY+5, 40, 40), abilityList[i].icon);
				
				if(flag){
					if(!inList && unit.abilityIDList.Count<5) unit.abilityIDList.Add(abilityList[i].prefabID);
					else unit.abilityIDList.Remove(abilityList[i].prefabID);
				}
				
				count+=1;
			}
			
			GUI.EndScrollView();
			
			
			return Vector2.zero;
		}
		
		
		Vector2 DrawAudioNAnimation(float startX, float startY, Unit unit){
			
			startX+=10;		width-=15;
			float cachedY=(startY+=spaceY);
			
			UnitAudio uAudio=unit.gameObject.GetComponent<UnitAudio>();
			if(uAudio==null) uAudio=unit.gameObject.AddComponent<UnitAudio>();
			//~ UnitAudio uAudio=unit.unitAudio;
			//~ if(uAudio==null){
				//~ uAudio=unit.gameObject.GetComponent<UnitAudio>();
				//~ if(uAudio==null) uAudio=unit.gameObject.AddComponent<UnitAudio>();
				//~ unit.SetAudio(uAudio);
			//~ }
			
			EditorGUI.LabelField(new Rect(startX, startY, width, height), "Audio:");
			EditorGUI.LabelField(new Rect(startX, startY+3, spaceX+width, height), "___________________________________________________________");
			
			cont=new GUIContent("Select Sound:", "Audio clip to play when the unit is selected");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			uAudio.selectSound=(AudioClip)EditorGUI.ObjectField(new Rect(startX+spaceX, startY, width, height), uAudio.selectSound, typeof(AudioClip), false);
			
			cont=new GUIContent("Move Sound:", "Audio clip to play when the unit moves");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			uAudio.moveSound=(AudioClip)EditorGUI.ObjectField(new Rect(startX+spaceX, startY, width, height), uAudio.moveSound, typeof(AudioClip), false);
			
			cont=new GUIContent("Attack Sound:", "Audio clip to play when the unit attacks");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			uAudio.attackSound=(AudioClip)EditorGUI.ObjectField(new Rect(startX+spaceX, startY, width, height), uAudio.attackSound, typeof(AudioClip), false);
			
			cont=new GUIContent("Hit Sound:", "Audio clip to play when the unit is hit");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			uAudio.hitSound=(AudioClip)EditorGUI.ObjectField(new Rect(startX+spaceX, startY, width, height), uAudio.hitSound, typeof(AudioClip), false);
			
			cont=new GUIContent("Destroy Sound:", "Audio clip to play when the unit is destroyed");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			uAudio.destroySound=(AudioClip)EditorGUI.ObjectField(new Rect(startX+spaceX, startY, width, height), uAudio.destroySound, typeof(AudioClip), false);
			
			
			startY=cachedY;
			startX+=280;
			
			UnitAnimation uAnim=unit.gameObject.GetComponent<UnitAnimation>();
			if(uAnim==null) uAnim=unit.gameObject.AddComponent<UnitAnimation>();
			//~ UnitAnimation uAnim=unit.unitAnim;
			//~ if(uAnim==null){
				//~ uAnim=unit.gameObject.GetComponent<UnitAnimation>();
				//~ if(uAnim==null) uAnim=unit.gameObject.AddComponent<UnitAnimation>();
				//~ unit.SetAnimation(uAnim);
			//~ }
			
			EditorGUI.LabelField(new Rect(startX, startY, width, height), "Animation:");
			EditorGUI.LabelField(new Rect(startX, startY+3, spaceX+width, height), "___________________________________________________________");
			
			
			Transform tempT=uAnim.aniRootObj==null ? null : uAnim.aniRootObj.transform;
			int objID=GetObjectIDFromHList(tempT, objHList);
			cont=new GUIContent("Animation Object:", "The object under unit's hierarchy which contain the animator component");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			objID = EditorGUI.Popup(new Rect(startX+spaceX, startY, width, height), objID, objHLabelList);
			
			if(objHList[objID]==null) uAnim.aniRootObj=null;
			else if(tempT!=objHList[objID].transform){
				//if(objHList[objID]==null) uAnim.aniRootObj=null;
				//else if(objHList[objID].GetComponent<Animator>()!=null) uAnim.aniRootObj=objHList[objID];
				if(objHList[objID].GetComponent<Animator>()!=null) uAnim.aniRootObj=objHList[objID];
				else Debug.Log("selected object doesn't have an Animator component");
			}
			
			//uAnim.aniRootObj = (objHList[objID]==null) ? null : objHList[objID];
			
			
			if(uAnim.aniRootObj!=null){
				cont=new GUIContent("Idle Animation:", "Animation clip to play when the unit is idle.");
				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
				uAnim.clipIdle=(AnimationClip)EditorGUI.ObjectField(new Rect(startX+spaceX, startY, width, height), uAnim.clipIdle, typeof(AnimationClip), false);
				
				cont=new GUIContent("Move Animation:", "Animation clip to play when the unit moves.");
				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
				uAnim.clipMove=(AnimationClip)EditorGUI.ObjectField(new Rect(startX+spaceX, startY, width, height), uAnim.clipMove, typeof(AnimationClip), false);
				
				cont=new GUIContent("Attack Animation:", "Animation clip to play when the unit attacks.");
				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
				uAnim.clipAttack=(AnimationClip)EditorGUI.ObjectField(new Rect(startX+spaceX, startY, width, height), uAnim.clipAttack, typeof(AnimationClip), false);
				
				cont=new GUIContent("Hit Animation:", "Animation clip to play when the unit is Hit.");
				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
				uAnim.clipHit=(AnimationClip)EditorGUI.ObjectField(new Rect(startX+spaceX, startY, width, height), uAnim.clipHit, typeof(AnimationClip), false);
				
				cont=new GUIContent("Destroy Animation:", "Animation clip to play when the unit is destroyed. ");
				EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
				uAnim.clipDestroy=(AnimationClip)EditorGUI.ObjectField(new Rect(startX+spaceX, startY, width, height), uAnim.clipDestroy, typeof(AnimationClip), false);
			}
			
			
			startX-=5;		width+=15;
			
			return Vector2.zero;
		}
		
		
		Vector2 DrawStats(float startX, float startY, Unit unit){
			
			float cachedY=startY;
			float spaceX=110;
			
			cont=new GUIContent("MoveAPCost:", "AP cost per tile when moving, only applicable when useAPForMove flag in GameControl is checked");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			unit.moveAPCost=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), unit.moveAPCost);
			
			cont=new GUIContent("AttackAPCost:", "AP cost for each attack, only applicable when useAPForAttack flag in GameControl is checked");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			unit.attackAPCost=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), unit.attackAPCost);
			
			startY+=10;

			cont=new GUIContent("Turn Priority:", "Value to determine the move order of the unit. The unit with highest value move first. Only used in UnitPerTurn mode or when MoveOrder is not free");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			unit.turnPriority=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), unit.turnPriority);
			
			startY+=10;
			
			cont=new GUIContent("MoveRange:", "Movement range in term of tile");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			unit.moveRange=EditorGUI.IntField(new Rect(startX+spaceX, startY, 40, height), unit.moveRange);
			
			cont=new GUIContent("AttackRange:", "Attack range in term of tile");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			unit.attackRange=EditorGUI.IntField(new Rect(startX+spaceX, startY, 40, height), unit.attackRange);
			
			cont=new GUIContent("SightRange:", "How far the unit can see. For AI unit in trigger mode, this is the range at which the unit will be actived. For player's units, this only applicable if Fog-Of-War is enabled");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			unit.sight=EditorGUI.IntField(new Rect(startX+spaceX, startY, 40, height), unit.sight);
			
			startY+=10;
			
			cont=new GUIContent("AttackPerTurn:", "Number of attack the unit can do in a turn");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			unit.attackPerTurn=EditorGUI.IntField(new Rect(startX+spaceX, startY, 40, height), unit.attackPerTurn);
			
			cont=new GUIContent("MovePerTurn:", "Number of move the unit can do in a turn");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			unit.movePerTurn=EditorGUI.IntField(new Rect(startX+spaceX, startY, 40, height), unit.movePerTurn);
			
			cont=new GUIContent("CounterPerTurn:", "Number of counter attack the unit can do in a turn");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			unit.counterPerTurn=EditorGUI.IntField(new Rect(startX+spaceX, startY, 40, height), unit.counterPerTurn);
			
			//startY+=10;
			startY=cachedY; 	startX+=190;
			
			cont=new GUIContent("DamageMin:", "The minimum damage inflicted on target when hit");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			unit.damageMin=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), unit.damageMin);
			
			cont=new GUIContent("DamageMax:", "The maximum damage inflicted on target when hit");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			unit.damageMax=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), unit.damageMax);
			
			startY+=10;
			
			cont=new GUIContent("HitChance:", "Chance to hit the target in an attack. Takes value from 0 and above with 0.7f being 70% to hit. Value is further modifed with target's dodge chance");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			unit.hitChance=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), unit.hitChance);
			
			cont=new GUIContent("DodgeChance:", "Chance to dodge an incoming attack. Takes value from 0 and above with 0.2f being 20% to dodge. Value is further modifed with attacker's hit chance");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			unit.dodgeChance=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), unit.dodgeChance);
			
			startY+=10;
			
			cont=new GUIContent("CritChance:", "Chance to score a cirtical hit when attacking. Takes value from 0 and above with 0.4% being 40%. Value is further modified with attacker's Critical Avoidance");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			unit.critChance=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), unit.critChance);
			
			cont=new GUIContent("CritAvoidance:", "Chance to avoid a critical hit when being attacked. Takes value from 0 and above with 0.2% being 20%. Value is further modified with attacker's Critical Chance");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			unit.critAvoidance=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), unit.critAvoidance);
			
			cont=new GUIContent("CritMultiplier:", "Multiplier for the damage value when a cirtical hit is scored. Takes value from 0 and above with 0.2 being 20% increase in damage cause");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			unit.critMultiplier=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), unit.critMultiplier);
			
			startY+=10;
			
			cont=new GUIContent("FlankingBonus:", "Damge multiplier when flanking a unit. Takes value from 0 and above with 0.4% being 40% bonus. Value is further modified with target's Flanked-Modifier");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			unit.flankingBonus=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), unit.flankingBonus);
			
			cont=new GUIContent("FlankedModifier:", "Damge multiplier when being flanked by an attacker. Takes value from 0 and above with 0.2% being 20% reduction in damage. Value is further modified with attacker's Flanking Bonus");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			unit.flankedModifier=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), unit.flankedModifier);
			
			
			startY=cachedY; 	startX+=190;
			
			cont=new GUIContent("StunChance:", "Chance to stun the target when attack. Takes value from 0 and above with 0.4% being 40%. Value is further modified with attacker's Stun Avoidance");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			unit.stunChance=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), unit.stunChance);
			
			cont=new GUIContent("StunAvoidance:", "Chance to avoid being stunned when being attacked. Takes value from 0 and above with 0.2% being 20%. Value is further modified with attacker's Stun Chance");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			unit.stunAvoidance=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), unit.stunAvoidance);
			
			cont=new GUIContent("StunDuration:", "The stun duration of target (in turn) when an attack succesfully stuns the target");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			unit.stunDuration=EditorGUI.IntField(new Rect(startX+spaceX, startY, 40, height), unit.stunDuration);
			
			startY+=10;
			
			cont=new GUIContent("SilentChance:", "Chance to silent the target when attack. Silenced unit cannot use ability. Takes value from 0 and above with 0.4% being 40%. Value is further modified with attacker's Stun Avoidance");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			unit.silentChance=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), unit.silentChance);
			
			cont=new GUIContent("SilentAvoidance:", "Chance to avoid being silenced when being attacked. Silenced unit cannot use ability. Takes value from 0 and above with 0.2% being 20%. Value is further modified with attacker's Stun Chance");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			unit.silentAvoidance=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), unit.silentAvoidance);
			
			cont=new GUIContent("SilentDuration:", "The silent duration of target (in turn) when an attack succesfully silences the target");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			unit.silentDuration=EditorGUI.IntField(new Rect(startX+spaceX, startY, 40, height), unit.silentDuration);
			
			
			return new Vector2(startX, startY);
		}
		
		
		
		
		
		
		
		private Rect listVisibleRect;
		private Rect listContentRect;
		
		private int deleteID=-1;
		private bool minimiseList=false;
		Vector2 DrawUnitList(float startX, float startY, List<Unit> unitList){
			
			float width=260;
			if(minimiseList) width=60;
			
			if(!minimiseList){
				if(GUI.Button(new Rect(startX+180, startY-20, 40, 18), "up")){
					if(selectID>0){
						Unit tower=unitList[selectID];
						unitList[selectID]=unitList[selectID-1];
						unitList[selectID-1]=tower;
						selectID-=1;
						
						if(selectID*35<scrollPos1.y) scrollPos1.y=selectID*35;
					}
				}
				if(GUI.Button(new Rect(startX+222, startY-20, 40, 18), "down")){
					if(selectID<unitList.Count-1){
						Unit tower=unitList[selectID];
						unitList[selectID]=unitList[selectID+1];
						unitList[selectID+1]=tower;
						selectID+=1;
						
						if(listVisibleRect.height-35<selectID*35) scrollPos1.y=(selectID+1)*35-listVisibleRect.height+5;
					}
				}
			}
			
			
			listVisibleRect=new Rect(startX, startY, width+15, window.position.height-startY-5);
			listContentRect=new Rect(startX, startY, width, unitList.Count*35+5);
			
			GUI.color=new Color(.8f, .8f, .8f, 1f);
			GUI.Box(listVisibleRect, "");
			GUI.color=Color.white;
			
			scrollPos1 = GUI.BeginScrollView(listVisibleRect, scrollPos1, listContentRect);
			
			//Debug.Log(scrollPos1.y+"   "+selectID*35+"    "+(scrollPos1.y+visibleRect.width));
			
			
			
				startY+=5;	startX+=5;
			
				for(int i=0; i<unitList.Count; i++){
					
					EditorUtilities.DrawSprite(new Rect(startX, startY+(i*35), 30, 30), unitList[i].iconSprite);
					
					if(minimiseList){
						if(selectID==i) GUI.color = new Color(0, 1f, 1f, 1f);
						if(GUI.Button(new Rect(startX+35, startY+(i*35), 30, 30), "")) SelectUnit(i);
						GUI.color = Color.white;
						
						continue;
					}
					
					
					
					if(selectID==i) GUI.color = new Color(0, 1f, 1f, 1f);
					if(GUI.Button(new Rect(startX+35, startY+(i*35), 150, 30), unitList[i].unitName)) SelectUnit(i);
					GUI.color = Color.white;
					
					if(deleteID==i){
						
						if(GUI.Button(new Rect(startX+190, startY+(i*35), 60, 15), "cancel")) deleteID=-1;
						
						GUI.color = Color.red;
						if(GUI.Button(new Rect(startX+190, startY+(i*35)+15, 60, 15), "confirm")){
							if(selectID>=deleteID) SelectUnit(Mathf.Max(0, selectID-1));
							EditorDBManager.RemoveUnit(deleteID);
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
		
		
		
		public static int GetObjectIDFromHList(Transform objT, List<GameObject> objHList){
			if(objT==null) return 0;
			for(int i=1; i<objHList.Count; i++){
				if(objT==objHList[i].transform) return i;
			}
			return 0;
		}
		
	}

}
