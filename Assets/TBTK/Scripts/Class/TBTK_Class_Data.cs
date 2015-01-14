using UnityEngine;
using System.Collections;
using System.Collections.Generic;


using TBTK;

namespace TBTK{

	public class Data {//: MonoBehaviour {

		private static List<List<DataUnit>> factionLoadList=new List<List<DataUnit>>();
		private static List<List<DataUnit>> factionEndList=new List<List<DataUnit>>();
		
		//*******************************************************************//	
		//load
		public static void ClearLoadData(){
			factionLoadList=new List<List<DataUnit>>();
		}
		
		//on loadlist, the ID is always the index of the element of the list
		public static void SetLoadData(int ID, List<DataUnit> list){
			for(int i=0; i<list.Count; i++){
				if(list[i].unit==null){
					list.RemoveAt(i);
					i-=1;
				}
			}
			
			if(ID==factionLoadList.Count) factionLoadList.Add(list);
			else if(ID<factionLoadList.Count) factionLoadList[ID]=list;
			else{
				while(factionLoadList.Count<ID) factionLoadList.Add(null);
				factionLoadList.Add(list);
			}
		}
		
		public static List<DataUnit> GetLoadData(int ID){
			if(ID<0 || ID>=factionLoadList.Count) return null;
			return factionLoadList[ID];
		}
		
		
		//*******************************************************************//	
		//end
		public static bool EndDataExist(){
			return factionEndList.Count==0 ? false : true ;
		}
		
		public static void ClearEndData(){
			factionEndList=new List<List<DataUnit>>();
		}
		
		public static void SetEndData(int ID, List<DataUnit> list){
			if(ID==factionEndList.Count) factionEndList.Add(list);
			else if(ID<factionEndList.Count) factionEndList[ID]=list;
			else{
				while(factionEndList.Count<ID) factionEndList.Add(null);
				factionEndList.Add(list);
			}
		}
		
		public static List<DataUnit> GetEndData(int ID){
			if(ID<0 && ID>=factionEndList.Count) return null;
			return factionEndList[ID];
		}
		
		
		
	}

	
	
	public class DataUnit{
		public Unit unit;
		
		//these value are use to overwrite the default value of the unit spawned in game when set to >0
		public int level=1;
		
		public float HP=-1;
		public float AP=-1;
		
		public float turnPriority=-1;
		public int moveRange=-1;
		public int attackRange=-1;
		
		public float hitChance=-1;
		public float dodgeChance=-1;
		public float damageMin=-1;
		public float damageMax=-1;
		
		public float critChance=-1;
		public float critAvoidance=-1;
		public float critMultiplier=-1;
		
		public float stunChance=-1;
		public float stunAvoidance=-1;
		public int stunDuration=-1;
		
		public float silentChance=-1;
		public float silentAvoidance=-1;
		public int silentDuration=-1;
		
		public float HPPerTurn=-1;
		public float APPerTurn=-1;
		
		
		public void Setup(Unit unitInstance){
			if(unitInstance==null){
				Debug.LogWarning("Data's unit is not set", null);
				return;
			}
			
			unit=unitInstance;
			
			level=unit.GetLevel();
			
			HP=unit.defaultHP; //unit.GetFullHP();
			AP=unit.defaultAP; //unit.GetFullAP();
			
			turnPriority=unit.turnPriority;
			moveRange=unit.moveRange;
			attackRange=unit.attackRange;
			hitChance=unit.hitChance;
			dodgeChance=unit.dodgeChance;
			damageMin=unit.damageMin;
			damageMax=unit.damageMax;
			critChance=unit.critChance;
			critAvoidance=unit.critAvoidance;
			critMultiplier=unit.critMultiplier;
			stunChance=unit.stunChance;
			stunAvoidance=unit.stunAvoidance;
			stunDuration=unit.stunDuration;
			silentChance=unit.silentChance;
			silentAvoidance=unit.silentAvoidance;
			silentDuration=unit.silentDuration;
			HPPerTurn=unit.HPPerTurn;
			APPerTurn=unit.APPerTurn;
		}
	}
	
}