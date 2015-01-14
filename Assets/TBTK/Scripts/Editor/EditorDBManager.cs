using UnityEngine;
using UnityEditor;

using System;

using System.Collections;
using System.Collections.Generic;

using TBTK;

namespace TBTK {

	public class EditorDBManager : EditorWindow {

		private static bool init=false;
		public static void Init(){
			if(GridManager.GetInstance()==null) GridManager.SetInstance();
			if(FactionManager.GetInstance()==null) FactionManager.SetInstance();
			
			if(init) return;
			
			init=true;
			//Debug.Log("Init");
			
			LoadUnit();
			LoadPerk();
			LoadUnitAbility();
			LoadFactionAbility();
			LoadDamageArmorTable();
		}
		
		
		
		private static DamageArmorDB DAPrefab;
		private static List<ArmorType> armorTypeList=new List<ArmorType>();
		private static List<DamageType> damageTypeList=new List<DamageType>();
		private static string[] damageTypeLabel;
		private static string[] armorTypeLabel;
		private static void LoadDamageArmorTable(){ 
			DAPrefab=DamageArmorDB.LoadDB();
			armorTypeList=DAPrefab.armorTypeList;
			damageTypeList=DAPrefab.damageTypeList;
			UpdateDamageArmorLabel();
		}
		private static void UpdateDamageArmorLabel(){
			damageTypeLabel=new string[DAPrefab.damageTypeList.Count];
			for(int i=0; i<DAPrefab.damageTypeList.Count; i++) damageTypeLabel[i]=DAPrefab.damageTypeList[i].name;
			armorTypeLabel=new string[DAPrefab.armorTypeList.Count];
			for(int i=0; i<DAPrefab.armorTypeList.Count; i++) armorTypeLabel[i]=DAPrefab.armorTypeList[i].name;
		}
		
		public static List<ArmorType> GetArmorTypeList(){ return armorTypeList; }
		public static List<DamageType> GetDamageTypeList(){ return damageTypeList; }
		public static string[] GetDamageTypeLabel(){ return damageTypeLabel; }
		public static string[] GetArmorTypeLabel(){ return armorTypeLabel; }
		public static void SetDirtyDamageArmor(){ 
			UpdateDamageArmorLabel();
			EditorUtility.SetDirty(DAPrefab);
		}
		
		public static void AddNewArmorType(){
			ArmorType armorType=new ArmorType();
			armorType.name="Armor"+DAPrefab.armorTypeList.Count;
			DAPrefab.armorTypeList.Add(armorType);
			UpdateDamageArmorLabel();
		}
		public static void AddNewDamageType(){
			DamageType damageType=new DamageType();
			damageType.name="Damage"+DAPrefab.damageTypeList.Count;
			DAPrefab.damageTypeList.Add(damageType);
			UpdateDamageArmorLabel();
		}
		public static void RemoveArmorType(int listID){
			DAPrefab.armorTypeList.RemoveAt(listID);
			UpdateDamageArmorLabel();
		}
		public static void RemoveDamageType(int listID){
			DAPrefab.damageTypeList.RemoveAt(listID);
			UpdateDamageArmorLabel();
		}
		
		
		
		
		
		
		
		private static UnitDB unitDBPrefab;
		private static List<Unit> unitList=new List<Unit>();
		private static List<int> unitIDList=new List<int>();
		private static string[] unitNameList=new string[0];
		private static void LoadUnit(){
			unitDBPrefab=UnitDB.LoadDB();
			unitList=unitDBPrefab.unitList;
			
			for(int i=0; i<unitList.Count; i++){
				//towerList[i].prefabID=i;
				if(unitList[i]!=null){
					unitIDList.Add(unitList[i].prefabID);
				}
				else{
					unitList.RemoveAt(i);
					i-=1;
				}
			}
			
			UpdateUnitNameList();
		}
		private static void UpdateUnitNameList(){
			List<string> tempList=new List<string>();
			tempList.Add(" - ");
			for(int i=0; i<unitList.Count; i++){
				string name=unitList[i].unitName;
				while(tempList.Contains(name)) name+=".";
				tempList.Add(name);
			}
			
			unitNameList=new string[tempList.Count];
			for(int i=0; i<tempList.Count; i++) unitNameList[i]=tempList[i];
		}
		
		public static List<Unit> GetUnitList(){ return unitList; }
		public static string[] GetUnitNameList(){ return unitNameList; }
		public static List<int> GetUnitIDList(){ return unitIDList; }
		public static void SetDirtyUnit(){
			EditorUtility.SetDirty(unitDBPrefab);
			for(int i=0; i<unitList.Count; i++) EditorUtility.SetDirty(unitList[i]);
		}
		
		public static int AddNewUnit(Unit newUnit){
			if(unitList.Contains(newUnit)) return -1;
			
			int ID=GenerateNewID(unitIDList);
			newUnit.prefabID=ID;
			unitIDList.Add(ID);
			unitList.Add(newUnit);
			
			UpdateUnitNameList();
			
			SetDirtyUnit();
			
			return unitList.Count-1;
		}
		public static void RemoveUnit(int listID){
			int prefabID=unitList[listID].prefabID;
			
			for(int i=0; i<perkList.Count; i++){
				if(perkList[i].type==_PerkType.Unit) perkList[i].itemIDList.Remove(prefabID);
			}
			
			unitIDList.Remove(prefabID);
			unitList.RemoveAt(listID);
			UpdateUnitNameList();
			SetDirtyUnit();
		}
		
		
		
		
		
		
		private static UnitAbilityDB unitAbilityDBPrefab;
		private static List<UnitAbility> unitAbilityList=new List<UnitAbility>();
		private static List<int> unitAbilityIDList=new List<int>();
		private static string[] unitAbilityNameList=new string[0];
		private static void LoadUnitAbility(){
			unitAbilityDBPrefab=UnitAbilityDB.LoadDB();
			unitAbilityList=unitAbilityDBPrefab.unitAbilityList;
			
			for(int i=0; i<unitAbilityList.Count; i++){
				//towerList[i].prefabID=i;
				if(unitAbilityList[i]!=null){
					unitAbilityIDList.Add(unitAbilityList[i].prefabID);
				}
				else{
					unitAbilityList.RemoveAt(i);
					i-=1;
				}
			}
			
			UpdateUnitAbilityNameList();
		}
		private static void UpdateUnitAbilityNameList(){
			List<string> tempList=new List<string>();
			tempList.Add(" - ");
			for(int i=0; i<unitAbilityList.Count; i++){
				string name=unitAbilityList[i].name;
				while(tempList.Contains(name)) name+=".";
				tempList.Add(name);
			}
			
			unitAbilityNameList=new string[tempList.Count];
			for(int i=0; i<tempList.Count; i++) unitAbilityNameList[i]=tempList[i];
		}
		
		public static List<UnitAbility> GetUnitAbilityList(){ return unitAbilityList; }
		public static string[] GetUnitAbilityNameList(){ return unitAbilityNameList; }
		public static List<int> GetUnitAbilityIDList(){ return unitAbilityIDList; }
		public static void SetDirtyUnitAbility(){
			EditorUtility.SetDirty(unitAbilityDBPrefab);
			//for(int i=0; i<unitAbilityList.Count; i++) EditorUtility.SetDirty(unitAbilityList[i]);
		}
		
		public static int AddNewUnitAbility(UnitAbility newUnitAbility){
			if(unitAbilityList.Contains(newUnitAbility)) return -1;
			
			int ID=GenerateNewID(unitAbilityIDList);
			newUnitAbility.prefabID=ID;
			unitAbilityIDList.Add(ID);
			unitAbilityList.Add(newUnitAbility);
			
			UpdateUnitAbilityNameList();
			
			SetDirtyUnitAbility();
			
			return unitAbilityList.Count-1;
		}
		public static void RemoveUnitAbility(int listID){
			int prefabID=unitAbilityList[listID].prefabID;
			
			for(int i=0; i<unitList.Count; i++){
				if(unitList[i].abilityIDList.Contains(prefabID)){
					unitList[i].abilityIDList.Remove(prefabID);
				}
			}
			
			for(int i=0; i<perkList.Count; i++){
				if(perkList[i].type==_PerkType.UnitAbility || perkList[i].type==_PerkType.NewUnitAbility)
					perkList[i].itemIDList.Remove(prefabID);
			}
			
			unitAbilityIDList.Remove(prefabID);
			unitAbilityList.RemoveAt(listID);
			UpdateUnitAbilityNameList();
			
			SetDirtyUnitAbility();
		}
		
		
		
		
		
		
		
		
		private static FactionAbilityDB factionAbilityDBPrefab;
		private static List<FactionAbility> factionAbilityList=new List<FactionAbility>();
		private static List<int> factionAbilityIDList=new List<int>();
		private static string[] factionAbilityNameList=new string[0];
		private static void LoadFactionAbility(){
			factionAbilityDBPrefab=FactionAbilityDB.LoadDB();
			factionAbilityList=factionAbilityDBPrefab.factionAbilityList;
			
			for(int i=0; i<factionAbilityList.Count; i++){
				//towerList[i].prefabID=i;
				if(factionAbilityList[i]!=null){
					factionAbilityIDList.Add(factionAbilityList[i].prefabID);
				}
				else{
					factionAbilityList.RemoveAt(i);
					i-=1;
				}
			}
			
			UpdateFactionAbilityNameList();
		}
		private static void UpdateFactionAbilityNameList(){
			List<string> tempList=new List<string>();
			tempList.Add(" - ");
			for(int i=0; i<factionAbilityList.Count; i++){
				string name=factionAbilityList[i].name;
				while(tempList.Contains(name)) name+=".";
				tempList.Add(name);
			}
			
			factionAbilityNameList=new string[tempList.Count];
			for(int i=0; i<tempList.Count; i++) factionAbilityNameList[i]=tempList[i];
		}
		
		public static List<FactionAbility> GetFactionAbilityList(){ return factionAbilityList; }
		public static string[] GetFactionAbilityNameList(){ return factionAbilityNameList; }
		public static List<int> GetFactionAbilityIDList(){ return factionAbilityIDList; }
		public static void SetDirtyFactionAbility(){
			EditorUtility.SetDirty(factionAbilityDBPrefab);
			//for(int i=0; i<factionAbilityList.Count; i++) EditorUtility.SetDirty(factionAbilityList[i]);
		}
		
		public static int AddNewFactionAbility(FactionAbility newFactionAbility){
			if(factionAbilityList.Contains(newFactionAbility)) return -1;
			
			int ID=GenerateNewID(factionAbilityIDList);
			newFactionAbility.prefabID=ID;
			factionAbilityIDList.Add(ID);
			factionAbilityList.Add(newFactionAbility);
			
			UpdateFactionAbilityNameList();
			
			SetDirtyFactionAbility();
			
			return factionAbilityList.Count-1;
		}
		public static void RemoveFactionAbility(int listID){
			int prefabID=factionAbilityList[listID].prefabID;
			
			for(int i=0; i<perkList.Count; i++){
				if(perkList[i].type==_PerkType.FactionAbility || perkList[i].type==_PerkType.NewFactionAbility) 
					perkList[i].itemIDList.Remove(prefabID);
			}
			
			factionAbilityIDList.Remove(prefabID);
			factionAbilityList.RemoveAt(listID);
			UpdateFactionAbilityNameList();
			SetDirtyFactionAbility();
		}
		
		
		
		
		
		
		
		private static PerkDB perkDBPrefab;
		private static List<Perk> perkList=new List<Perk>();
		private static List<int> perkIDList=new List<int>();
		private static string[] perkNameList=new string[0];
		private static void LoadPerk(){
			perkDBPrefab=PerkDB.LoadDB();
			perkList=perkDBPrefab.perkList;
			
			for(int i=0; i<perkList.Count; i++){
				//towerList[i].prefabID=i;
				if(perkList[i]!=null){
					perkIDList.Add(perkList[i].prefabID);
				}
				else{
					perkList.RemoveAt(i);
					i-=1;
				}
			}
			
			UpdatePerkNameList();
		}
		private static void UpdatePerkNameList(){
			List<string> tempList=new List<string>();
			tempList.Add(" - ");
			for(int i=0; i<perkList.Count; i++){
				string name=perkList[i].name;
				while(tempList.Contains(name)) name+=".";
				tempList.Add(name);
			}
			
			perkNameList=new string[tempList.Count];
			for(int i=0; i<tempList.Count; i++) perkNameList[i]=tempList[i];
		}
		
		public static List<Perk> GetPerkList(){ return perkList; }
		public static string[] GetPerkNameList(){ return perkNameList; }
		public static List<int> GetPerkIDList(){ return perkIDList; }
		public static void SetDirtyPerk(){
			EditorUtility.SetDirty(perkDBPrefab);
			//for(int i=0; i<perkList.Count; i++) EditorUtility.SetDirty(perkList[i]);
		}
		
		public static int AddNewPerk(Perk newPerk){
			if(perkList.Contains(newPerk)) return -1;
			
			int ID=GenerateNewID(perkIDList);
			newPerk.prefabID=ID;
			perkIDList.Add(ID);
			perkList.Add(newPerk);
			
			UpdatePerkNameList();
			
			SetDirtyPerk();
			
			return perkList.Count-1;
		}
		public static void RemovePerk(int listID){
			perkIDList.Remove(perkList[listID].prefabID);
			perkList.RemoveAt(listID);
			UpdatePerkNameList();
			SetDirtyPerk();
		}
		
		
		
		
		
		
		
		
		private static int GenerateNewID(List<int> list){
			int ID=0;
			while(list.Contains(ID)) ID+=1;
			return ID;
		}
		
		
	}

}
