using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using TBTK;

namespace TBTK{

	[RequireComponent (typeof (EffectTracker))]
	public class AbilityManagerUnit : MonoBehaviour{
		
		public delegate void IterateCooldownHandler();
		public static event IterateCooldownHandler onIterateAbilityCooldownE;
		
		public delegate void AbilityActivatedHandler();
		public static event AbilityActivatedHandler onAbilityActivatedE;			//listen by AudioManager only
		
		
		
		public static AbilityManagerUnit instance;
		
		private List<UnitAbility> unitAbilityDBList=new List<UnitAbility>();
		
		void Awake(){
			if(instance==null) instance=this;
		}
		
		public void Init(){
			if(instance==null) instance=this;
			
			unitAbilityDBList=UnitAbilityDB.LoadClone();
		}
		
		
		void OnEnable(){
			GameControl.onIterateTurnE += _IterateAbilityCooldown;
		}
		void OnDisable(){
			GameControl.onIterateTurnE -= _IterateAbilityCooldown;
		}
		
		public static void IterateAbilityCooldown(){ instance._IterateAbilityCooldown(); }
		public void _IterateAbilityCooldown(){
			if(onIterateAbilityCooldownE!=null) onIterateAbilityCooldownE();
		}
		
		
		
		void Update(){
			#if UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8 || UNITY_BLACKBERRY
				if(Input.touchCount==2){
					if(GameControl.selectedUnit!=null && GameControl.selectedUnit.GetSelectedAbilityID()>=0){
						GameControl.selectedUnit.ClearSelectedAbility();
					}
				}
			#else
				if(Input.GetMouseButtonDown(1)){
					if(GameControl.selectedUnit!=null && GameControl.selectedUnit.GetSelectedAbilityID()>=0){
						GameControl.selectedUnit.ClearSelectedAbility();
					}
				}
			#endif
		}
		
		
		
		public static void AbilityActivated(){
			if(onAbilityActivatedE!=null) onAbilityActivatedE();
		}
		
		
		public static void PerkUnlockNewAbility(int unitID, int abID){ if(instance!=null) instance._PerkUnlockNewAbility(unitID, abID); }
		public void _PerkUnlockNewAbility(int unitID, int abID){ 
			int abIndex=-1;
			for(int i=0; i<unitAbilityDBList.Count; i++){
				if(unitAbilityDBList[i].prefabID==abID){
					abIndex=i;	break;
				}
			}
			
			if(abIndex==-1) return;
			
			List<Unit> unitList=FactionManager.GetAllUnit();
			for(int i=0; i<unitList.Count; i++){
				Unit unit=unitList[i];
				if(unit.isAIUnit) continue;
				if(unit.prefabID==unitID){
					UnitAbility unitAbility=unitAbilityDBList[abIndex].Clone();
					unitAbility.SetUnit(unit);
					unit.abilityIDList.Add(abID);
					unit.abilityList.Add(unitAbility);
				}
			}
			
			if(GameControl.selectedUnit!=null) GameControl.selectedUnit.Select();	//this will call onUnitSelected on the unit which will refresh the UI
		}
		
		
		
		public static void ApplyAbilityEffect(Unit srcUnit, Tile targetTile, UnitAbility ability){
			instance.StartCoroutine(instance._ApplyAbilityEffect(srcUnit, targetTile, ability));
		}
		IEnumerator _ApplyAbilityEffect(Unit srcUnit, Tile targetTile, UnitAbility ability){
			if(ability.effectObject!=null)	Instantiate(ability.effectObject, targetTile.GetPos(), Quaternion.identity);
			
			if(ability.delayDuration>0) yield return new WaitForSeconds(ability.delayDuration);
			
			if(ability.type==UnitAbility._AbilityType.Generic){
			
				List<Tile> tileList=new List<Tile>();
				if(ability.aoeRange>0) tileList=GridManager.GetTilesWithinDistance(targetTile, ability.aoeRange);
				tileList.Add(targetTile);
				
				if(ability.targetType==_TargetType.AllUnit){
					for(int i=0; i<tileList.Count; i++){
						if(tileList[i].unit!=null){
							tileList[i].unit.ApplyEffect(ability.Clone(false));
							if(ability.effectObjectOnTarget!=null)	Instantiate(ability.effectObjectOnTarget, tileList[i].GetPos(), Quaternion.identity);
						}
					}
				}
				if(ability.targetType==_TargetType.HostileUnit){
					for(int i=0; i<tileList.Count; i++){
						if(tileList[i].unit!=null && tileList[i].unit.factionID!=srcUnit.factionID){
							tileList[i].unit.ApplyEffect(ability.Clone(false));
							if(ability.effectObjectOnTarget!=null)	Instantiate(ability.effectObjectOnTarget, tileList[i].GetPos(), Quaternion.identity);
						}
					}
				}
				if(ability.targetType==_TargetType.FriendlyUnit){
					for(int i=0; i<tileList.Count; i++){
						if(tileList[i].unit!=null && tileList[i].unit.factionID==srcUnit.factionID){
							tileList[i].unit.ApplyEffect(ability.Clone(false));
							if(ability.effectObjectOnTarget!=null)	Instantiate(ability.effectObjectOnTarget, tileList[i].GetPos(), Quaternion.identity);
						}
					}
				}
				if(ability.targetType==_TargetType.AllTile){
					for(int i=0; i<tileList.Count; i++){
						tileList[i].ApplyEffect(ability.Clone(false));
						if(ability.effectObjectOnTarget!=null)	Instantiate(ability.effectObjectOnTarget, tileList[i].GetPos(), Quaternion.identity);
					}
				}
				if(ability.targetType==_TargetType.Tile){
					for(int i=0; i<tileList.Count; i++){
						if(tileList[i].unit==null){
							tileList[i].unit.ApplyEffect(ability.Clone(false));
							if(ability.effectObjectOnTarget!=null)	Instantiate(ability.effectObjectOnTarget, tileList[i].GetPos(), Quaternion.identity);
						}
					}
				}
				
			}
			else if(ability.type==UnitAbility._AbilityType.Teleport){
				GameControl.ClearSelectedUnit();
				srcUnit.SetNewTile(targetTile);
				GameControl.SelectUnit(srcUnit);
			}
			else if(ability.type==UnitAbility._AbilityType.SpawnNew){
				GameObject unitObj=(GameObject)Instantiate(ability.spawnUnit, targetTile.GetPos(), srcUnit.thisT.rotation);
				Unit unit=unitObj.GetComponent<Unit>();
				
				unit.SetNewTile(targetTile);
				
				FactionManager.InsertUnit(unit, srcUnit.factionID);
				if(GridManager.GetDistance(targetTile, srcUnit.tile)<=srcUnit.GetMoveRange()) GameControl.SelectUnit(srcUnit);
			}
			else if(ability.type==UnitAbility._AbilityType.ScanFogOfWar){
				List<Tile> targetTileList=GridManager.GetTilesWithinDistance(targetTile, ability.GetAOERange());
				targetTileList.Add(targetTile);
				
				for(int i=0; i<targetTileList.Count; i++) targetTileList[i].ForceVisible(ability.duration);
			}
			
			yield return null;
		}
		
		
		
		
		//to setup abilities of individual unit
		public static List<UnitAbility> GetAbilityListBasedOnIDList(List<int> IDList){ 
			if(instance==null) return null;
			return instance._GetAbilityListBasedOnIDList(IDList);
		}
		public List<UnitAbility> _GetAbilityListBasedOnIDList(List<int> IDList){
			List<UnitAbility> newList=new List<UnitAbility>();
			for(int i=0; i<IDList.Count; i++){
				for(int n=0; n<unitAbilityDBList.Count; n++){
					if(unitAbilityDBList[n].prefabID==IDList[i]){
						//if(!unitAbilityDBList[n].onlyAvailableViaPerk) 
							newList.Add(unitAbilityDBList[n].Clone());
						break;
					}
				}
			}
			return newList;
		}
		
	}

}
