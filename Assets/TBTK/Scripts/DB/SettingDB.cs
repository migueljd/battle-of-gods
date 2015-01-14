using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Collections;
using System.Collections.Generic;

using TBTK;

namespace TBTK {
	
	public class SettingDB : MonoBehaviour {
		
		public bool generateGridOnStart=false;
		public bool generateUnitOnStart=false;
		
		public _TurnMode turnMode;
		
		public _MoveOrder moveOrder;
		
		
		public bool enableManualUnitDeployment=true;
		
		public bool enableActionAfterAttack=false;
		
		public bool enableCounter=false;
		public float counterAPMultiplier=1f;
		public float counterDamageMultiplier=1f;
		
		public bool restoreUnitAPOnTurn=true;
		
		public bool useAPForMove=true;
		public bool useAPForAttack=true;
		
		public bool attackThroughObstacle=false;
		
		public bool enableFogOfWar=false;
		public float peekFactor=0.4f;
		
		public bool enableCover=false;
		public float exposedCritBonus=0.3f;
		public float fullCoverBonus=0.75f;
		public float halfCoverBonus=0.25f;
		
		public bool enableFlanking=false;
		public float flankingAngle=120;
		public float flankingBonus=1.5f;
		
		
		public bool savePerk=true;
		
		
		public static SettingDB LoadDB(){
			GameObject obj=Resources.Load("DB_TBTK/SettingDB", typeof(GameObject)) as GameObject;
			
			#if UNITY_EDITOR
				if(obj==null) obj=CreatePrefab();
			#endif
			
			if(obj==null){
				Debug.Log("no object");
				return null;
			}
			
			return obj.GetComponent<SettingDB>();
		}
		
		
		#if UNITY_EDITOR
			private static GameObject CreatePrefab(){
				GameObject obj=new GameObject();
				obj.AddComponent<UnitDB>();
				GameObject prefab=PrefabUtility.CreatePrefab("Assets/TBTK/Resources/DB_TBTK/SettingDB.prefab", obj, ReplacePrefabOptions.ConnectToPrefab);
				DestroyImmediate(obj);
				AssetDatabase.Refresh ();
				return prefab;
			}
		#endif
		
		
	}
	
}

