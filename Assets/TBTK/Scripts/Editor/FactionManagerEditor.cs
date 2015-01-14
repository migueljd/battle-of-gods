using UnityEngine;
using UnityEditor;

using System;

using System.Collections;
using System.Collections.Generic;

using TBTK;

namespace TBTK{

	[CustomEditor(typeof(FactionManager))]
	public class FactionManagerEditor : Editor {

		private static FactionManager instance;
		
		private static bool showDefaultFlag=false;
		
		private GUIContent cont;
		private GUIContent[] contList;
		
		
		void Awake(){
			instance = (FactionManager)target;
			
			EditorDBManager.Init();
			
			
			
			EditorUtility.SetDirty(instance);
		}
		
		
		
		public override void OnInspectorGUI(){
			
			GUI.changed = false;
			
			EditorGUILayout.Space();
			
			//cont=new GUIContent("Generate Unit On Start:", "Check to re-generate the unit whenever the level is loaded. Note that this will overwrite all the existing unit set on the grid.");
			//instance.generateUnitOnStart=EditorGUILayout.Toggle(cont, instance.generateUnitOnStart);
			
			//cont=new GUIContent("Allow Unit Deployment:", "Check to manually deploy starting unit. Otherwise they will be automatically deployed");
			//instance.allowManualUnitDeployment=EditorGUILayout.Toggle(cont, instance.allowManualUnitDeployment);
			
			
			if(!Application.isPlaying){
				if(GUILayout.Button("Generate Unit")) instance._GenerateUnit();
			}
			
			
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.Space();
			
			EditorGUILayout.HelpBox("Editing of Faction Information via Inspector is not recommended, please use FactionManager-EditorWindow instead", MessageType.Info);
			//GUIStyle style=new GUIStyle();
			//style.wordWrap=true;
			//EditorGUILayout.LabelField("Editing of Faction Information via Inspector is not recommended, please use FactionManager-EditorWindow instead", style);
			EditorGUILayout.Space();
			EditorGUILayout.EndHorizontal();
			
			
			EditorGUILayout.Space();
			if(GUILayout.Button("Open FactionManager-EditorWindow")){
				FactionManagerEditorWindow.Init();
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