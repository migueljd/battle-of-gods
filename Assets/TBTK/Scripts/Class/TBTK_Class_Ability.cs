using UnityEngine;
using System.Collections;

using TBTK;

namespace TBTK{
	
	
	[System.Serializable]
	public class Effect{
		//public enum _EffectType{Positive, Negative}
		
		public int prefabID;
		//public int instanceID;
		
		public Sprite icon;
		public string name;
		public string desp;
		
		
		public int duration=-1;	//how many round the effect last, a round is when all unit or all faction has been moved (depends on the turn/move order combination)
										//when set to 0, the effect only last as long as the unit is active (until end turn button is pressed)
										//when set to -1, none of the buff effect will apply, only immediate value (ie. HPMin, HPMax, stun...) will apply
		//public int durationAlt=0;	//counter use to keep track of how many unit has moved since the last round, not used in FactionPerTurn mode
		public DurationCounter durationCounter=new DurationCounter();
		//public _EffectType type;
		
		public float HPMin=0;	//direct dmg/gain for the HP
		public float HPMax=0;
		public float APMin=0;	//direct dmg/gain for the AP
		public float APMax=0;
		
		public int stun=0;
		
		public UnitStat unitStat=new UnitStat();
		
		
		public void StartDurationCount(){ durationCounter.Count(duration); }
		public void IterateDuration(){
			durationCounter.Iterate();
			duration=(int)durationCounter.duration;
		}
		
		public bool EffectExpired(){ return durationCounter.duration<=0 ? true : false; }
		
	}

	
	
	public enum _TargetType{AllUnit, HostileUnit, FriendlyUnit, AllTile, Tile}
	
	[System.Serializable]
	public class Ability : Effect {
		
		public bool isFactionAbility=false;
		
		//~ public enum _AbilityType{Generic, Teleport, SpawnNew, ClearFog}
		//~ public _AbilityType type;
		
		public bool onlyAvailableViaPerk=false;
		
		public _TargetType targetType;
		
		//~ private Unit unit;
		//~ public void SetUnit(Unit un){ unit=un; }
		
		public float cost=5;
		public int cooldown=5;
		public DurationCounter currentCD=new DurationCounter();
		
		public int useLimit=-1;
		public int useCount=0;
		
		public bool requireConfirmation=true;
		public bool requireTargetSelection;
		
		public int range=5;
		public int aoeRange=0;
		
		public bool shoot=false;
		public GameObject shootObject;
		public GameObject effectObject;				//to be use when hit/cast
		public GameObject effectObjectOnTarget;	//to be used on individual target
		
		public float delayDuration=0.25f;
		
		public GameObject spawnUnit;
		
		public bool enableMoveAfterCast=true;
		public bool enableAttackAfterCast=true;
		public bool enableAbilityAfterCast=true;
		
		
		public void Use(){
			currentCD.Count(GetCooldown());
			useCount+=1;
		}
		
		public void IterateCooldown(){ currentCD.Iterate(); }
		
		
		
		public int GetDuration(){ return duration+PerkManager.GetAbilityDuration(prefabID, isFactionAbility); }	
		public float GetCost(){ return cost+PerkManager.GetAbilityCost(prefabID, isFactionAbility); }					
		public int GetCooldown(){ return cooldown+PerkManager.GetAbilityCD(prefabID, isFactionAbility); }		
		public int GetUseLimit(){ return useLimit+PerkManager.GetAbilityUseLimit(prefabID, isFactionAbility); }			
		public int GetRange(){ return range+PerkManager.GetAbilityRange(prefabID, isFactionAbility); }				
		public int GetAOERange(){ return aoeRange+PerkManager.GetAbilityAOERange(prefabID, isFactionAbility); }	
		
		public float GetHPMin(){ return HPMin+PerkManager.GetAbilityHP(prefabID, isFactionAbility); }		
		public float GetHPMax(){ return HPMax+PerkManager.GetAbilityHP(prefabID, isFactionAbility); }	
		public float GetAPMin(){ return APMin+PerkManager.GetAbilityAP(prefabID, isFactionAbility); }		
		public float GetAPMax(){ return APMax+PerkManager.GetAbilityAP(prefabID, isFactionAbility); }	
		public int GetStun(){ return stun+PerkManager.GetAbilityStun(prefabID, isFactionAbility); }		
		
		public float GetHPBuff(){ return unitStat.HPBuff+PerkManager.GetAbilityHPBuff(prefabID, isFactionAbility); }		
		public float GetAPBuff(){ return unitStat.APBuff+PerkManager.GetAbilityAPBuff(prefabID, isFactionAbility); }		
		
		public float GetMoveAPCost(){ return unitStat.moveAPCost+PerkManager.GetAbilityMoveAPCost(prefabID, isFactionAbility); }		
		public float GetAttackAPCost(){ return unitStat.attackAPCost+PerkManager.GetAbilityAttackAPCost(prefabID, isFactionAbility); }		
		
		public float GetTurnPriority(){ return unitStat.turnPriority+PerkManager.GetAbilityTurnPriority(prefabID, isFactionAbility); }		
		
		public int GetMoveRange(){ return unitStat.moveRange+PerkManager.GetAbilityMoveRange(prefabID, isFactionAbility); }		
		public int GetAttackRange(){ return unitStat.attackRange+PerkManager.GetAbilityAttackRange(prefabID, isFactionAbility); }	
		public int GetSight(){ return unitStat.sight+PerkManager.GetAbilitySight(prefabID, isFactionAbility); }		
		
		public int GetMovePerTurn(){ return unitStat.movePerTurn+PerkManager.GetAbilityMovePerTurn(prefabID, isFactionAbility); }		
		public int GetAttackPerTurn(){ return unitStat.attackPerTurn+PerkManager.GetAbilityAttackPerTurn(prefabID, isFactionAbility); }		
		public int GetCounterPerTurn(){ return unitStat.counterPerTurn+PerkManager.GetAbilityCounterPerTurn(prefabID, isFactionAbility); }		
		
		public float GetDamage(){ return unitStat.damage+PerkManager.GetAbilityDamage(prefabID, isFactionAbility); }		
		//public float GetDamageMin(){ return unitStat.damageMin+PerkManager.GetAbilityDamage(prefabID, isFactionAbility); }		
		//public float GetDamageMax(){ return unitStat.damageMax+PerkManager.GetAbilityDamage(prefabID, isFactionAbility); }		
		
		public float GetHitChance(){ return unitStat.hitChance+PerkManager.GetAbilityHitChance(prefabID, isFactionAbility); }		
		public float GetDodgeChance(){ return unitStat.dodgeChance+PerkManager.GetAbilityDodgeChance(prefabID, isFactionAbility); }	
		
		public float GetCritChance(){ return unitStat.critChance+PerkManager.GetAbilityCritChance(prefabID, isFactionAbility); }	
		public float GetCritAvoidance(){ return unitStat.critAvoidance+PerkManager.GetAbilityCritAvoidance(prefabID, isFactionAbility); }		
		public float GetCritMultiplier(){ return unitStat.critMultiplier+PerkManager.GetAbilityCritMultiplier(prefabID, isFactionAbility); }		
		
		public float GetStunChance(){ return unitStat.stunChance+PerkManager.GetAbilityStunChance(prefabID, isFactionAbility); }		
		public float GetStunAvoidance(){ return unitStat.stunAvoidance+PerkManager.GetAbilityStunAvoidance(prefabID, isFactionAbility); }		
		public int GetStunDuration(){ return unitStat.stunDuration+PerkManager.GetAbilityStunDuration(prefabID, isFactionAbility); }	
		
		public float GetFlankingBonus(){ return unitStat.flankingBonus+PerkManager.GetAbilityFlankingBonus(prefabID, isFactionAbility); }		
		public float GetFlankedModifier(){ return unitStat.flankedModifier+PerkManager.GetAbilityFlankedModifier(prefabID, isFactionAbility); }	
		
		public float GetHPPerTurn(){ return unitStat.HPPerTurn+PerkManager.GetAbilityHpPerTurn(prefabID, isFactionAbility); }		
		public float GetAPPerTurn(){ return unitStat.APPerTurn+PerkManager.GetAbilityAPPerTurn(prefabID, isFactionAbility); }
		
		
		public void Copy(Ability ability, bool useDefaultValue=true){
			prefabID	=ability.prefabID;
			icon		=ability.icon;
			name		=ability.name;
			desp		=ability.desp;
			
			useCount					=ability.useCount;
			requireConfirmation		=ability.requireConfirmation;
			requireTargetSelection	=ability.requireTargetSelection;
			targetType					=ability.targetType;
			
			shoot				=ability.shoot;
			shootObject	=ability.shootObject;
			effectObject	=ability.effectObject;
			effectObjectOnTarget	=ability.effectObjectOnTarget;
			
			delayDuration	=ability.delayDuration;
			
			spawnUnit		=ability.spawnUnit;
			
			enableMoveAfterCast	=ability.enableMoveAfterCast;
			enableAttackAfterCast	=ability.enableAttackAfterCast;
			enableAbilityAfterCast	=ability.enableAbilityAfterCast;
			
			
			if(useDefaultValue){		//for when application is not running or when it's loading from DB
				duration		=ability.duration;
				cost			=ability.cost;
				cooldown	=ability.cooldown;
				useLimit		=ability.useLimit;
				range			=ability.range;
				aoeRange	=ability.aoeRange;
				
				HPMin	=ability.HPMin;
				HPMax	=ability.HPMax;
				APMin	=ability.APMin;
				APMax	=ability.APMax;
				stun		=ability.stun;
				
				unitStat=ability.unitStat.Clone();
			}
			else{						//for runtime, to get the effective value taking acount of perk bonus
				duration		=ability.GetDuration();
				cost			=ability.GetCost();
				cooldown	=ability.GetCooldown();
				useLimit		=ability.GetUseLimit();
				range			=ability.GetRange();
				aoeRange	=ability.GetAOERange();
				
				HPMin		=ability.GetHPMin();
				HPMax		=ability.GetHPMax();
				APMin		=ability.GetAPMin();
				APMax		=ability.GetAPMax();
				stun			=ability.GetStun();
				
				unitStat.HPBuff = ability.GetHPBuff();
				unitStat.APBuff = ability.GetAPBuff();
				
				unitStat.moveAPCost = ability.GetMoveAPCost();
				unitStat.attackAPCost = ability.GetAttackAPCost();
				
				unitStat.turnPriority = ability.GetTurnPriority();
				
				unitStat.moveRange = ability.GetMoveRange();
				unitStat.attackRange = ability.GetAttackRange();
				unitStat.sight = ability.GetSight();
				
				unitStat.movePerTurn = ability.GetMovePerTurn();
				unitStat.attackPerTurn = ability.GetAttackPerTurn();
				unitStat.counterPerTurn = ability.GetCounterPerTurn();
				
				unitStat.damage = ability.GetDamage();
				//unitStat.damageMin = ability.GetDamageMin();
				//unitStat.damageMax = ability.GetDamageMax();
				
				unitStat.hitChance = ability.GetHitChance();
				unitStat.dodgeChance = ability.GetDodgeChance();
				
				unitStat.critChance = ability.GetCritChance();
				unitStat.critAvoidance = ability.GetCritAvoidance();
				unitStat.critMultiplier = ability.GetCritMultiplier();
				
				unitStat.stunChance = ability.GetStunChance();
				unitStat.stunAvoidance = ability.GetStunAvoidance();
				unitStat.stunDuration = ability.GetStunDuration();
				
				unitStat.flankingBonus = ability.GetFlankingBonus();
				unitStat.flankedModifier = ability.GetFlankedModifier();
				
				unitStat.HPPerTurn = ability.GetHPPerTurn();
				unitStat.APPerTurn = ability.GetAPPerTurn();
			}
			
		}
		
	}
	
	
	
	
	
	
	[System.Serializable]
	public class FactionAbility : Ability {
		
		public int factionIndex=0;	//this is the index of the faction in abilityFactionList in AbilityManager, not the factionID of each faction in FactionManager
												//use to check energy, only assigned in runtime
		
		public enum _AbilityType{Generic, SpawnNew, ScanFogOfWar}
		public _AbilityType type;
		
		public bool useDefaultEffect=true;
		
		
		public FactionAbility(){ isFactionAbility=true; }
		
		
		
		public string IsAvailable(){
			if(AbilityManagerFaction.GetEnergy(factionIndex)<GetCost()) return "Insufficient AP";
			//~ if(currentCD.duration>0) return "Ability on cooldown";
			if(currentCD.duration>0) return name+"  Ability on cooldown   "+currentCD.duration;
			
			int limit=GetUseLimit();
			if(limit>=1 && useCount>=limit) return "Ability is used up";
			
			return "";
		}
		
		public FactionAbility Clone(bool useDefaultValue=true){
			FactionAbility facAB=new FactionAbility();
			
			facAB.factionIndex=factionIndex;
			facAB.type=type;
			facAB.useDefaultEffect=useDefaultEffect;
			
			facAB.Copy(this, useDefaultValue);
			
			return facAB;
		}
	}
	
	
	
	
	
	
	[System.Serializable]
	public class UnitAbility : Ability {
		
		public enum _AbilityType{Generic, Teleport, SpawnNew, ScanFogOfWar}
		public _AbilityType type;
		
		private Unit unit;
		public void SetUnit(Unit un){ unit=un; }
		
		
		public UnitAbility(){ isFactionAbility=false; }
		
		
		
		public string IsAvailable(){
			if(unit.IsSilenced()) return "Unit is silenced";
			if(unit.IsStunned()) return "Unit is stunned";
			if(unit.DisableAbilities()) return "Cannot use abilities";
			if(unit.AP<GetCost()) return "Insufficient AP";
			if(currentCD.duration>0) return "Ability on cooldown";
			
			int limit=GetUseLimit();
			if(limit>=1 && useCount>=limit) return "Ability is used up";
			
			return "";
		}
		
		
		public UnitAbility Clone(bool useDefaultValue=true){
			UnitAbility uAB=new UnitAbility();
			
			uAB.type=type;
			
			uAB.Copy(this, useDefaultValue);
			
			return uAB;
		}
		
	}
	
}