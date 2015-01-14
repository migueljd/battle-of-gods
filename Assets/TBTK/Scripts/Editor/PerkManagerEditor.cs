using UnityEngine;
using UnityEditor;

using System;

using System.Collections;
using System.Collections.Generic;

using TBTK;

namespace TBTK{

	[CustomEditor(typeof(PerkManager))]
	public class PerkManagerEditor : Editor {

		private static PerkManager instance;
		
		private static bool showDefaultFlag=false;
		private bool showPerkList=true;
		
		private static List<Perk> perkList=new List<Perk>();
		
		private GUIContent cont;
		private GUIContent[] contList;
		
		
		void Awake(){
			instance = (PerkManager)target;
			
			GetPerk();
			
			EditorUtility.SetDirty(instance);
		}
		
		
		private static void GetPerk(){
			EditorDBManager.Init();
			
			perkList=EditorDBManager.GetPerkList();
			
			if(Application.isPlaying) return;
			
			List<int> perkIDList=EditorDBManager.GetPerkIDList();
			for(int i=0; i<instance.unavailableIDList.Count; i++){
				if(!perkIDList.Contains(instance.unavailableIDList[i])){
					instance.unavailableIDList.RemoveAt(i);	i-=1;
				}
			}
		}
		
		
		
		public override void OnInspectorGUI(){
			
			GUI.changed = false;
			
			EditorGUILayout.Space();
			
			cont=new GUIContent("Single Level Only:", "Check to have the PerkManager work within current level only. Start from fresh and no progress will be saved ");
			instance.singleLevelOnly=EditorGUILayout.Toggle(cont, instance.singleLevelOnly);
			
			EditorGUILayout.Space();
			
			if(!instance.singleLevelOnly){
				cont=new GUIContent("Save Progress:", "Check to have the PerkManager save any purchase made. Any progress made will be load at the next play session. The perk currency will be saved as well.");
				instance.saveProgress=EditorGUILayout.Toggle(cont, instance.saveProgress);
				
				cont=new GUIContent("Perk Currency:", "The starting value of the currency use to purchase perk. The value will be overwrite if any save has been made in previous play session");
				instance.perkCurrency=EditorGUILayout.IntField(cont, instance.perkCurrency);
			}
			
			if(instance.singleLevelOnly){
				cont=new GUIContent("Perk Currency:", "The currency use to purchase perk.");
				instance.perkCurrency=EditorGUILayout.IntField(cont, instance.perkCurrency);
				
				EditorGUILayout.Space();
				
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("", GUILayout.MaxWidth(10));
				showPerkList=EditorGUILayout.Foldout(showPerkList, "Show Perk List");
				EditorGUILayout.EndHorizontal();
				if(showPerkList){
					
					EditorGUILayout.BeginHorizontal();
					if(GUILayout.Button("EnableAll") && !Application.isPlaying){
						instance.unavailableIDList=new List<int>();
					}
					if(GUILayout.Button("DisableAll") && !Application.isPlaying){
						instance.purchasedIDList=new List<int>();
						
						instance.unavailableIDList=new List<int>();
						for(int i=0; i<perkList.Count; i++) instance.unavailableIDList.Add(perkList[i].prefabID);
					}
					EditorGUILayout.EndHorizontal ();
					
					
					for(int i=0; i<perkList.Count; i++){
						Perk perk=perkList[i];
						
						
						GUILayout.BeginHorizontal();
							
							GUILayout.Box("", GUILayout.Width(40),  GUILayout.Height(40));
							Rect rect=GUILayoutUtility.GetLastRect();
							EditorUtilities.DrawSprite(rect, perk.icon, false);
							
							GUILayout.BeginVertical();
								EditorGUILayout.Space();
								GUILayout.Label(perk.name, GUILayout.ExpandWidth(false));
						
								GUILayout.BeginHorizontal();
									bool flag=!instance.unavailableIDList.Contains(perk.prefabID) ? true : false;
									if(Application.isPlaying) flag=!flag;	//switch it around in runtime
									EditorGUILayout.LabelField(new GUIContent(" - enabled: ", "check to enable the perk in this level"), GUILayout.Width(70));
									flag=EditorGUILayout.Toggle(flag);
						
									if(!Application.isPlaying){
										if(flag) instance.unavailableIDList.Remove(perk.prefabID);
										else{
											if(!instance.unavailableIDList.Contains(perk.prefabID)){
												instance.unavailableIDList.Add(perk.prefabID);
												instance.purchasedIDList.Remove(perk.prefabID);
											}
										}
									}
									
									if(!instance.unavailableIDList.Contains(perk.prefabID)){
										flag=instance.purchasedIDList.Contains(perk.prefabID);
										EditorGUILayout.LabelField(new GUIContent("- purchased:", "Check to set the perk as purchased right from the start"), GUILayout.Width(75));
										flag=EditorGUILayout.Toggle(flag);
										if(!flag) instance.purchasedIDList.Remove(perk.prefabID);
										else if(!instance.purchasedIDList.Contains(perk.prefabID)) instance.purchasedIDList.Add(perk.prefabID);
									}
									
								GUILayout.EndHorizontal();
								
							GUILayout.EndVertical();
						
						GUILayout.EndHorizontal();
					}
				
				}
				
			}
			
			
			EditorGUILayout.Space();
			EditorGUILayout.Space();
		
			if(!Application.isPlaying){
				cont=new GUIContent("Reset Saved Progress", "Reset any progress of PerkSystem made in game");
				if(GUILayout.Button(cont, GUILayout.MaxWidth(258))) PerkManager.ClearPerkProgress();
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