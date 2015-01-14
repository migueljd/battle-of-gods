using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Collections;
using System.Collections.Generic;

using TBTK;

namespace TBTK {

	public class DamageArmorDB : MonoBehaviour {

		public List<ArmorType> armorTypeList=new List<ArmorType>();
		public List<DamageType> damageTypeList=new List<DamageType>();
		
		public static DamageArmorDB LoadDB(){
			GameObject obj=Resources.Load("DB_TBTK/DamageArmorDB", typeof(GameObject)) as GameObject;
			
			#if UNITY_EDITOR
				if(obj==null) obj=CreatePrefab();
			#endif
			
			return obj.GetComponent<DamageArmorDB>();
		}
		
		#if UNITY_EDITOR
			private static GameObject CreatePrefab(){
				GameObject obj=new GameObject();
				obj.AddComponent<DamageArmorDB>();
				GameObject prefab=PrefabUtility.CreatePrefab("Assets/TBTK/Resources/DB_TBTK/DamageArmorDB.prefab", obj, ReplacePrefabOptions.ConnectToPrefab);
				DestroyImmediate(obj);
				AssetDatabase.Refresh ();
				return prefab;
			}
		#endif
		
	}

}
