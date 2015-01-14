using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using TBTK;

namespace TBTK {
	
	public enum _PerkType{Unit, AllUnit, UnitAbility, AllUnitAbility, FactionAbility, AllFactionAbility, NewUnitAbility, NewFactionAbility}

	[System.Serializable]
	public class Perk {
		
		public int prefabID;
		public int instanceID;
		
		public Sprite icon;
		public string name;
		public string desp;
		
		public _PerkType type;
		
		public bool purchased=false;
		
		public int cost=1;
		//public int minLevel=1;								//min level to reach before becoming available (check GameControl.levelID)
		//public int minWave=0;								//min wave to reach before becoming available
		public int minPerkPoint=0;						//min perk point 
		public List<int> prereq=new List<int>();	//prerequisite perk before becoming available, element is removed as the perk is unlocked in runtime
		
		public List<int> itemIDList=new List<int>();
		
		
		//for ability modifier
		public int abDurationMod=0;
		
		public float abCostMod=5;
		public int abCooldownMod=5;
		
		public int abUseLimitMod=-1;
		
		public int abRangeMod=5;
		public int abAOERangeMod=0;
		
		public float abHPMod=0;	//direct dmg/gain for the HP
		//public float abHPMaxMod=0;
		public float abAPMod=0;	//direct dmg/gain for the AP
		//public float abAPMaxMod=0;
		
		public int abStunMod=0;
		
		
		//for unit and ability modifier
		public UnitStat stats=new UnitStat();
		
		
		public bool addAbilityToAllUnit=false;
		public int abilityID=0;	//for add new ability
		
		
		public string IsAvailable(){
			//Debug.Log("  "+SpawnManager.GetCurrentWaveID());
			if(purchased) return "Purchased";
			//if(GameControl.GetLevelID()<minLevel) return "Unlocked at level "+minLevel;
			//if(Mathf.Max(SpawnManager.GetCurrentWaveID()+1, 1)<minWave) return "Unlocked at Wave "+minWave;
			if(PerkManager.GetPerkCurrency()<cost) return "Insufficient perk currency";
			if(PerkManager.GetPerkPoint()<minPerkPoint) return "Insufficient perk point";
			if(prereq.Count>0){
				string text="Require: ";
				bool first=true;
				List<Perk> perkList=PerkManager.GetPerkList();
				for(int i=0; i<prereq.Count; i++){
					for(int n=0; n<perkList.Count; n++){
						if(perkList[n].prefabID==prereq[i]){
							text+=((!first) ? ", " : "")+perkList[n].name;
							first=false;
							break;
						}
					}
				}
				return text;
				//return "Not all prerequisite perk has been unlocked";
			}
			return "";
		}
		
		public string Purchase(bool useCurrency=true){
			purchased=true;
			
			if(useCurrency){
				if(PerkManager.GetPerkCurrency()<cost) return "Insufficient perk currency";
				PerkManager.SpendCurrency(cost);
			}
			
			return "";
		}
	
		public Perk Clone(){
			Perk perk=new Perk();
			
			perk.prefabID=prefabID;
			
			perk.icon=icon;
			perk.name=name;
			perk.desp=desp;
			
			perk.type=type;
			
			perk.purchased=purchased;
			
			perk.cost=cost;
			perk.minPerkPoint=minPerkPoint;
			for(int i=0; i<prereq.Count; i++) perk.prereq.Add(prereq[i]);
			
			
			for(int i=0; i<itemIDList.Count; i++) perk.itemIDList.Add(itemIDList[i]);
			
			perk.abDurationMod=abDurationMod;
			perk.abCostMod=abCostMod;
			perk.abCooldownMod=abCooldownMod;
			perk.abUseLimitMod=abUseLimitMod;
			perk.abRangeMod=abRangeMod;
			perk.abAOERangeMod=abAOERangeMod;
			perk.abHPMod=abHPMod;
			//perk.abHPMaxMod=abHPMaxMod;
			perk.abAPMod=abAPMod;
			//perk.abAPMaxMod=abAPMaxMod;
			perk.abStunMod=abStunMod;
			
			perk.stats=stats.Clone();
			
			perk.addAbilityToAllUnit=addAbilityToAllUnit;
			perk.abilityID=abilityID;
			
			return perk;
		}

	}
	
	
	
	public class PerkAbilityModifier{
		public int prefabID;
		
		public int duration=0;
		
		public float cost=0;
		public int cooldown=0;
		
		public int useLimit=0;
		
		public int range=0;
		public int aoeRange=0;
		
		public float HP=0;	//direct dmg/gain for the HP
		public float AP=0;
		
		//public float HPMin=0;	//direct dmg/gain for the HP
		//public float HPMax=0;
		//public float APMin=0;	//direct dmg/gain for the AP
		//public float APMax=0;
		
		public int stun=0;
		
		public UnitStat stats=new UnitStat();
	}
	
	public class PerkUnitModifier{
		public int prefabID;
		
		public UnitStat stats=new UnitStat();
		
		public List<int> abilityIDList=new List<int>();
	}
	
	
	
	
	

}