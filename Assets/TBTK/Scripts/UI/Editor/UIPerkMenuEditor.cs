using UnityEngine;
using UnityEditor;

using System.Collections;
using System.Collections.Generic;

using TBTK;

namespace TBTK {

	[CustomEditor(typeof(UIPerkMenu))]
	public class UIPerkMenuEditor : Editor {

		private static UIPerkMenu instance;
		
		private bool showDefaultFlag=false;
		
		private GUIContent cont;
		
		void Awake(){
			instance = (UIPerkMenu)target;
			
			EditorDBManager.Init();
			
			EditorUtility.SetDirty(instance);
		}
		
		
		public override void OnInspectorGUI(){
			
			//EditorGUILayout.Space();
			
			//cont=new GUIContent("HideMenuOnStart:", "Check to disable perk-menu in the scene entirely. This is similar to deactivate the component's gameObject");
			//instance.hideMenuOnStart=EditorGUILayout.Toggle(cont, instance.hideMenuOnStart);
			
			//cont=new GUIContent("PerkButtonAlwaysOn:", "Check have the perk menu button always visible");
			//instance.perkButtonAlwaysOn=EditorGUILayout.Toggle(cont, instance.perkButtonAlwaysOn);
			
			
			EditorGUILayout.Space();
			
			cont=new GUIContent("AssignItemManually:", "Check to manually assign the item and their associate perk");
			instance.assignItemManually=EditorGUILayout.Toggle(cont, instance.assignItemManually);
			
			if(instance.assignItemManually){
				GUILayout.BeginHorizontal();
				//EditorGUILayout.Space();
				if(GUILayout.Button("Add Item", GUILayout.MaxWidth(120))){
					//AddItem();
					instance.itemList.Add(new UIPerkMenu.PerkItem());
				}
				if(GUILayout.Button("Reduce Item", GUILayout.MaxWidth(120))){
					//RemoveItem();
					instance.itemList.RemoveAt(instance.itemList.Count-1);
				}
				GUILayout.EndHorizontal();
				
				EditorGUILayout.Space();
				
				string[] perkNameList=EditorDBManager.GetPerkNameList();
				List<Perk> perkList=EditorDBManager.GetPerkList();
				int[] intList=new int[perkNameList.Length];
				for(int i=0; i<perkNameList.Length; i++){
					if(i==0) intList[i]=-1;
					else intList[i]=perkList[i-1].prefabID;
				}
				
				for(int i=0; i<instance.itemList.Count; i++){
					GUILayout.BeginHorizontal();
					
					GUILayout.Label(" - Element "+(i+1), GUILayout.Width(75));
					
					instance.itemList[i].button.rootObj=(GameObject)EditorGUILayout.ObjectField(instance.itemList[i].button.rootObj, typeof(GameObject), true);
					
					//~ if(GUILayout.Button("+", GUILayout.MaxWidth(20))){
						//~ InsertWaypoints(i);
					//~ }
					//~ if(GUILayout.Button("-", GUILayout.MaxWidth(20))){
						//~ i-=RemoveWaypoints(i);
					//~ }
					GUILayout.EndHorizontal();
					
					
					GUILayout.BeginHorizontal();
					GUILayout.Label("    Link   ", GUILayout.Width(75));
					instance.itemList[i].linkObj=(GameObject)EditorGUILayout.ObjectField(instance.itemList[i].linkObj, typeof(GameObject), true);
					GUILayout.EndHorizontal();
					
					GUILayout.BeginHorizontal();
					GUILayout.Label("    Label:   ", GUILayout.Width(75));
					instance.itemList[i].perkID=EditorGUILayout.IntPopup(instance.itemList[i].perkID, perkNameList, intList);
					GUILayout.EndHorizontal();
					
					EditorGUILayout.Space();
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