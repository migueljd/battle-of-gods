using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using TBTK;

namespace TBTK {

	[RequireComponent (typeof (EffectTracker))]
	public class AbilityManagerFaction : MonoBehaviour {
		
		public delegate void ClearSelectionHandler();
		public static event ClearSelectionHandler onClearSelectedAbilityE;		//listen by UI only
		
		public delegate void AbilityActivatedHandler();
		public static event AbilityActivatedHandler onAbilityActivatedE;			//listen by AudioManager only
		
		
		//see class defination in TBTK_Class_Faction?
		[HideInInspector] public List<FactionAbilityFaction> abilityFactionList=new List<FactionAbilityFaction>();		
		
		[HideInInspector] public bool startWithFullEnergy=false;
		
		
		[HideInInspector] public List<FactionAbility> facAbilityDBList=new List<FactionAbility>();
		public static List<FactionAbility> GetAbilityDBList(){ return instance.facAbilityDBList; }
		
		
		[HideInInspector] public int selectedAbilityID=-1;
		private FactionAbility currentAbility;
		public static int GetSelectedAbilityID(){ return instance.selectedAbilityID; }
		private bool requireTargetSelection=false;	//indicate if current selected Ability require target selection
		
		
		private DurationCounter counter=new DurationCounter();
		
		
		private static AbilityManagerFaction instance;
		
		
		void Awake(){
			instance=this;
			
			facAbilityDBList=FactionAbilityDB.Load();
		}
		
		void Start(){
			List<Faction> factionList=FactionManager.GetFactionList();
			abilityFactionList=new List<FactionAbilityFaction>(); 
			for(int i=0; i<factionList.Count; i++){
				FactionAbilityFaction abFac=new FactionAbilityFaction();
				abFac.factionID=factionList[i].ID;
				abFac.fullEnergy=factionList[i].fullEnergy;
				abFac.energyGainPerTurn=factionList[i].energyGainPerTurn;
				abFac.Init(i, factionList[i].unavailableAbilityIDList, startWithFullEnergy, factionList[i].isPlayerFaction);
				abilityFactionList.Add(abFac);
			}
		}
		
		
		
		public static void PerkUnlockNewAbility(int ID){ if(instance!=null) instance._PerkUnlockNewAbility(ID); }
		public void _PerkUnlockNewAbility(int ID){
			int dbIndex=-1;
			for(int i=0; i<facAbilityDBList.Count; i++){
				if(facAbilityDBList[i].prefabID==ID){
					dbIndex=i;	break;
				}
			}
			
			if(dbIndex<0) return;
			
			for(int i=0; i<abilityFactionList.Count; i++){
				if(FactionManager.IsPlayerFaction(abilityFactionList[i].factionID)){
					FactionAbility ability=facAbilityDBList[i].Clone();
					ability.factionIndex=i;
					abilityFactionList[i].facAbilityList.Add(ability);
				}
			}
			
			if(GameControl.selectedUnit!=null) GameControl.selectedUnit.Select();	//this will call onUnitSelected on the unit which will refresh the UI
		}
		
		
		
		void OnEnable(){
			GameControl.onIterateTurnE += IterateTurn;
		}
		void OnDisable(){
			GameControl.onIterateTurnE -= IterateTurn;
		}
		
		//called by GameControl when game start
		public static void StartCounter(){ if(instance!=null) instance._StartCounter(); }
		public void _StartCounter(){ counter.Count(1); }
		
		public void IterateTurn(){
			IterateCounter();
			IterateAbilityCooldown();
		}
		
		//calculate the energy gain for each faction at the end of each turn
		public void IterateCounter(){
			counter.Iterate();
			if(counter.duration==0){
				for(int i=0; i<abilityFactionList.Count; i++){
					float fullEnergy=abilityFactionList[i].fullEnergy;
					float energy=abilityFactionList[i].energy;
					float energyGainPerTurn=abilityFactionList[i].energyGainPerTurn;
					abilityFactionList[i].energy=Mathf.Min(fullEnergy, energy+energyGainPerTurn);
				}
				
				counter.Count(1);
			}
		}
		
		//calculate the ability cooldown at the end of each turn
		public void IterateAbilityCooldown(){
			for(int i=0; i<abilityFactionList.Count; i++){
				List<FactionAbility> abList=abilityFactionList[i].facAbilityList;
				for(int n=0; n<abList.Count; n++) abList[n].IterateCooldown();
			}
		}
		
		
		
		void Update(){
			#if UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8 || UNITY_BLACKBERRY
				if(Input.touchCount==2) ClearSelectedAbility();
			#else
				if(Input.GetMouseButtonDown(1)) ClearSelectedAbility();
			#endif
		}
		
		
		
		//called by ability button from UI, select an ability
		public static string SelectAbility(int ID){ return instance._SelectAbility(ID); }
		public string _SelectAbility(int ID){
			if(GameControl.selectedUnit.GetSelectedAbilityID()>=0) GameControl.selectedUnit.ClearSelectedAbility();
			
			if(selectedAbilityID>=0){
				if(selectedAbilityID==ID){	//if the same ability has been selected, deselect it
					if(!requireTargetSelection) ActivateAbility(currentAbility);
					ClearSelectedAbility();
					return "";
				}
				else ClearSelectedAbility();
			}
			
			
			int facID=FactionManager.GetSelectedFactionID();
			FactionAbility ability=null;
			for(int i=0; i<abilityFactionList.Count; i++){
				if(abilityFactionList[i].factionID==facID){
					//list=abilityFactionList[i].facAbilityList;
					ability=abilityFactionList[i].facAbilityList[ID];
				}
			}
			
			if(ability==null) return "error";
			
			string exception=ability.IsAvailable();
			if(exception!="") return exception;
			
			//Debug.Log("select    "+ability.name+"      "+selectedAbilityID+"    "+ability.requireTargetSelection);
			
			selectedAbilityID=ID;
			currentAbility=ability;
			requireTargetSelection=ability.requireTargetSelection;
			
			if(requireTargetSelection) 
				GridManager.AbilityTargetMode(ability.GetAOERange(), ability.targetType, this.AbilityTargetSelected);
			
			//if(!ability.requireTargetSelection) ActivateAbility(ability);		//no target selection required, fire it away
			//else{
				//selectedAbilityID=ID;
				//currentAbility=ability;
				//GridManager.AbilityTargetMode(ability.GetAOERange(), ability.targetType, this.AbilityTargetSelected);
			//}
				
			return "";
		}
		public static void ClearSelectedAbility(){ 
			if(instance.selectedAbilityID<0) return;
			instance.selectedAbilityID=-1;
			instance.currentAbility=null;
			GridManager.ClearTargetMode();
			if(onClearSelectedAbilityE!=null) onClearSelectedAbilityE();
		}
		
		
		//callback function for GridManager when a target has been selected for selected ability
		public void AbilityTargetSelected(Tile tile){
			if(tile==null){
				ClearSelectedAbility();
				return;
			}
			
			ActivateAbility(currentAbility, tile);
		}
		
		//called when an ability is fired, reduce the energy, start the cooldown and what not
		public void ActivateAbility(FactionAbility ability, Tile tile=null){
			ability.Use();
			abilityFactionList[ability.factionIndex].energy-=ability.GetCost();
			
			if(onAbilityActivatedE!=null) onAbilityActivatedE();
			
			CastAbility(ability, tile);
		}
		
		
		//called from ActivateAbility, cast the ability, visual effect and actual effect goes here
		public void CastAbility(FactionAbility ability, Tile tile=null){
			if(ability.effectObject!=null && tile!=null){
				ObjectPoolManager.Spawn(ability.effectObject, tile.GetPos(), Quaternion.identity);
			}
			
			if(ability.useDefaultEffect){
				StartCoroutine(ApplyAbilityEffect(ability, tile));
			}
		}
		
		
		
		IEnumerator ApplyAbilityEffect(FactionAbility ability, Tile targetTile=null){
			if(ability.delayDuration>0) yield return new WaitForSeconds(ability.delayDuration);
			
			int factionID=FactionManager.GetSelectedFactionID();
			
			if(ability.type==FactionAbility._AbilityType.Generic){
				List<Tile> targetTileList=new List<Tile>();
				if(targetTile!=null){
					targetTileList=GridManager.GetTilesWithinDistance(targetTile, ability.GetAOERange());
					targetTileList.Add(targetTile);
					
					if(ability.targetType==_TargetType.AllUnit){
						for(int i=0; i<targetTileList.Count; i++){
							if(targetTileList[i].unit!=null){
								targetTileList[i].unit.ApplyEffect(ability.Clone(false));
								SpawnEffectObjectOnTarget(ability.effectObjectOnTarget, targetTileList[i]);
							}
						}
					}
					if(ability.targetType==_TargetType.HostileUnit){
						for(int i=0; i<targetTileList.Count; i++){
							if(targetTileList[i].unit!=null && targetTileList[i].unit.factionID!=factionID){
								targetTileList[i].unit.ApplyEffect(ability.Clone(false));
								SpawnEffectObjectOnTarget(ability.effectObjectOnTarget, targetTileList[i]);
							}
						}
					}
					if(ability.targetType==_TargetType.FriendlyUnit){
						for(int i=0; i<targetTileList.Count; i++){
							if(targetTileList[i].unit!=null && targetTileList[i].unit.factionID==factionID){
								targetTileList[i].unit.ApplyEffect(ability.Clone(false));
								SpawnEffectObjectOnTarget(ability.effectObjectOnTarget, targetTileList[i]);
							}
						}
					}
					if(ability.targetType==_TargetType.AllTile){
						for(int i=0; i<targetTileList.Count; i++){
							targetTileList[i].ApplyEffect(ability.Clone(false));
							SpawnEffectObjectOnTarget(ability.effectObjectOnTarget, targetTileList[i]);
						}
					}
					if(ability.targetType==_TargetType.Tile){
						for(int i=0; i<targetTileList.Count; i++){
							if(targetTileList[i].unit==null){
								targetTileList[i].unit.ApplyEffect(ability.Clone(false));
								SpawnEffectObjectOnTarget(ability.effectObjectOnTarget, targetTileList[i]);
							}
						}
					}
					
				}
				else{
					if(ability.targetType==_TargetType.AllUnit){
						List<Unit> unitList=FactionManager.GetAllUnit();
						for(int i=0; i<unitList.Count; i++){
							targetTileList.Add(unitList[i].tile);
						}
					}
					if(ability.targetType==_TargetType.HostileUnit){
						List<Unit> unitList=FactionManager.GetAllUnit();
						for(int i=0; i<unitList.Count; i++){
							if(unitList[i].factionID!=factionID) targetTileList.Add(unitList[i].tile);
						}
					}
					if(ability.targetType==_TargetType.FriendlyUnit){
						List<Unit> unitList=FactionManager.GetAllUnit();
						for(int i=0; i<unitList.Count; i++){
							if(unitList[i].factionID==factionID) targetTileList.Add(unitList[i].tile);
						}
					}
					
					for(int i=0; i<targetTileList.Count; i++){
						targetTileList[i].unit.ApplyEffect(ability.Clone(false));
						SpawnEffectObjectOnTarget(ability.effectObjectOnTarget, targetTileList[i]);
					}
				}
				
				
			}
			else if(ability.type==FactionAbility._AbilityType.SpawnNew){
				GameObject unitObj=(GameObject)Instantiate(ability.spawnUnit, targetTile.GetPos(), Quaternion.identity);
				Unit unit=unitObj.GetComponent<Unit>();
				unit.tile=targetTile;
				targetTile.unit=unit;
				FactionManager.InsertUnit(unit, FactionManager.GetSelectedFactionID());
				
				Unit selectedUnit=GameControl.selectedUnit;
				if(GridManager.GetDistance(targetTile, selectedUnit.tile)<=selectedUnit.GetMoveRange()) GameControl.SelectUnit(selectedUnit);
			}
			else if(ability.type==FactionAbility._AbilityType.ScanFogOfWar){
				List<Tile> targetTileList=GridManager.GetTilesWithinDistance(targetTile, ability.GetAOERange());
				targetTileList.Add(targetTile);
				
				for(int i=0; i<targetTileList.Count; i++) targetTileList[i].ForceVisible(ability.GetDuration());
			}
			
			yield return null;
		}
		
		
		public void SpawnEffectObjectOnTarget(GameObject effectObject, Tile targetTile){
			if(effectObject!=null) Instantiate(effectObject, targetTile.GetPos(), Quaternion.identity);
		}
		
		
		//energy related function
		public static float GetFactionEnergyFull(int factionID){ return instance._GetFactionEnergyFull(factionID); }
		public float _GetFactionEnergyFull(int factionID){
			for(int i=0; i<abilityFactionList.Count; i++){
				if(abilityFactionList[i].factionID==factionID) return abilityFactionList[i].GetEnergyFull();
			}
			return 0;
		}
		public static float GetFactionEnergy(int factionID){ return instance._GetFactionEnergy(factionID); }
		public float _GetFactionEnergy(int factionID){
			for(int i=0; i<abilityFactionList.Count; i++){
				if(abilityFactionList[i].factionID==factionID) return abilityFactionList[i].GetEnergy();
			}
			return 0;
		}
		public static float GetFactionEnergyGainPerTurn(int factionID){ return instance._GetFactionEnergyGainPerTurn(factionID); }
		public float _GetFactionEnergyGainPerTurn(int factionID){
			for(int i=0; i<abilityFactionList.Count; i++){
				if(abilityFactionList[i].factionID==factionID) return abilityFactionList[i].GetEnergyGainPerTurn();
			}
			return 0;
		}
		
		
		public static List<FactionAbility> GetFactionAbilityList(int factionID){ return instance._GetFactionAbilityList(factionID); }
		public List<FactionAbility> _GetFactionAbilityList(int factionID){
			for(int i=0; i<abilityFactionList.Count; i++){
				if(abilityFactionList[i].factionID==factionID) return abilityFactionList[i].facAbilityList;
			}
			return new List<FactionAbility>();
		}
		
		
		
		//call by ability to check energy
		public static float GetEnergy(int index){
			return instance.abilityFactionList[index].GetEnergy();
		}
		
	}

	
	
	
	//class contains the each faction's FactionAbility info
	[System.Serializable]
	public class FactionAbilityFaction{
		public int factionID=-1;	//correspond to the factionID in FactionManager
		
		public float fullEnergy=100;
		public float energy=0;
		public float energyGainPerTurn=20;
		public float GetEnergyFull(){ return fullEnergy; }//+PerkManager.GetEnergyCapModifier(); }
		public float GetEnergy(){ return energy; }
		public float GetEnergyGainPerTurn(){ return energyGainPerTurn; }
		
		public List<FactionAbility> facAbilityList=new List<FactionAbility>();
		
		//load up the ability and setup the energy
		public void Init(int facIndex, List<int> unavailableIDList, bool startWithFullEnergy=false, bool isPlayerFaction=true){
			if(startWithFullEnergy) energy=fullEnergy;
			
			List<FactionAbility> dbList=AbilityManagerFaction.GetAbilityDBList();
			
			facAbilityList=new List<FactionAbility>();
			for(int i=0; i<dbList.Count; i++){
				if(dbList[i].onlyAvailableViaPerk) continue;
				if(!unavailableIDList.Contains(dbList[i].prefabID)){
					facAbilityList.Add(dbList[i].Clone());
				}
			}
			
			//add ability unlocked via perk (PerkManager is carried forth from last level)
			if(isPlayerFaction){
				List<int> perkAbilityIDList=PerkManager.GetFactionAbilityIDList();
				for(int n=0; n<perkAbilityIDList.Count; n++){
					for(int i=0; i<dbList.Count; i++){
						if(dbList[i].prefabID==perkAbilityIDList[n]){
							facAbilityList.Add(dbList[i].Clone());
							break;
						}
					}
				}
			}
			
			for(int i=0; i<facAbilityList.Count; i++){
				facAbilityList[i].factionIndex=facIndex;
			}
		}
		
		public void GainEnergy(int value){
			energy+=value;
			energy=Mathf.Min(energy, GetEnergyFull());
		}
	}
	

}