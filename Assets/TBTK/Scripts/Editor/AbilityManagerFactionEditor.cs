using UnityEngine;
using UnityEditor;

using System;

using System.Collections;
using System.Collections.Generic;

using TBTK;

namespace TBTK{

	[CustomEditor(typeof(AbilityManagerFaction))]
	public class AbilityManagerFactionEditor : Editor {

		private static AbilityManagerFaction instance;
		
		void Awake(){
			instance = (AbilityManagerFaction)target;
		}
		
		
		
		public override void OnInspectorGUI(){
			
			GUI.changed = false;
			
			EditorGUILayout.Space();
			
			DrawDefaultInspector();
			GUIContent cont=new GUIContent("StartWithFullEnergy:", "Check to have the faction(s) starts with full energy");
			instance.startWithFullEnergy=EditorGUILayout.Toggle(cont, instance.startWithFullEnergy);
			
			EditorGUILayout.Space();
			
			if(GUI.changed) EditorUtility.SetDirty(instance);
			
		}
		
	}
	
}