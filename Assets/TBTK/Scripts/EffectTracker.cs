using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using TBTK;

namespace TBTK{
	
	public class EffectTracker : MonoBehaviour {
		
		private List<Unit> unitList=new List<Unit>();
		private List<Tile> tileList=new List<Tile>();
		
		
		private static EffectTracker instance;
		
		void Awake(){
			instance=this;
		}
		
		
		void OnEnable(){
			GameControl.onIterateTurnE += _IterateEffectDuration;
		}
		void OnDisable(){
			GameControl.onIterateTurnE -= _IterateEffectDuration;
		}
		
		public static void IterateEffectDuration(){ instance._IterateEffectDuration(); }
		public void _IterateEffectDuration(){
			for(int i=0; i<tileList.Count; i++) tileList[i].ProcessEffectList();
			
			//bool turnPriorityChanged=false;
			List<int> factionRequirePriorityUpdate=new List<int>();
			for(int i=0; i<unitList.Count; i++){
				bool flag=unitList[i].ProcessEffectList();
				if(flag) factionRequirePriorityUpdate.Add(unitList[i].factionID); //turnPriorityChanged=true;
			}
			
			if(factionRequirePriorityUpdate.Count>0)
				FactionManager.UnitTurnPriorityChanged(factionRequirePriorityUpdate);
		}
		
		
		public static void AddTileWithEffect(Tile tile){ if(!instance.tileList.Contains(tile)) instance.tileList.Add(tile); }
		public static void RemoveTileWithEffect(Tile tile){ instance.StartCoroutine(instance._RemoveTileWithEffect(tile)); }
		IEnumerator _RemoveTileWithEffect(Tile tile){
			yield return null;
			tileList.Remove(tile);
		}
		
		
		public static void AddUnitWithEffect(Unit unit){ if(!instance.unitList.Contains(unit)) instance.unitList.Add(unit); }
		public static void RemoveUnitWithEffect(Unit unit){ instance.StartCoroutine(instance._RemoveUnitWithEffect(unit)); }//instance.unitList.Remove(unit); }
		IEnumerator _RemoveUnitWithEffect(Unit unit){
			yield return null;
			unitList.Remove(unit);
		}
		
	}

}


