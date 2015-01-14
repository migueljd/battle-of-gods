using UnityEngine;
using System.Collections;

using TBTK;

namespace TBTK {

	[System.Serializable]
	public class UnitStat{
		public float HPBuff=0;
		public float APBuff=0;
		
		public float moveAPCost=0;
		public float attackAPCost=0;
		
		public float turnPriority=0;
		
		public int moveRange=0;
		public int attackRange=0;
		public int sight=0;
		
		public int movePerTurn=0;
		public int attackPerTurn=0;
		public int counterPerTurn=0;	//counter attack
		
		public float damage=0;
		// public float damageMin=0;
		// public float damageMax=0;
		
		public float hitChance=0f;
		public float dodgeChance=0;
		
		public float critChance=0;
		public float critAvoidance=0;
		public float critMultiplier=0;
		
		public float stunChance=0;
		public float stunAvoidance=0;
		public int stunDuration=0;
		
		public float silentChance=0;
		public float silentAvoidance=0;
		public int silentDuration=0;
		
		public float flankingBonus=0;
		public float flankedModifier=0;
		
		public float HPPerTurn=0;
		public float APPerTurn=0;
		
		
		
		public UnitStat Clone(){
			UnitStat stat=new UnitStat();
			
			stat.HPBuff=HPBuff;
			stat.APBuff=APBuff;
			
			stat.moveAPCost=moveAPCost;
			stat.attackAPCost=attackAPCost;
			
			stat.turnPriority=turnPriority;
			
			stat.moveRange=moveRange;
			stat.attackRange=attackRange;
			stat.sight=sight;
			
			stat.movePerTurn=movePerTurn;
			stat.attackPerTurn=attackPerTurn;
			stat.counterPerTurn=counterPerTurn;
			
			stat.damage=damage;
			//stat.damageMin=damageMin;
			//stat.damageMax=damageMax;
			
			stat.hitChance=hitChance;
			stat.dodgeChance=dodgeChance;
			
			stat.critChance=critChance;
			stat.critAvoidance=critAvoidance;
			stat.critMultiplier=critMultiplier;
			
			stat.stunChance=stunChance;
			stat.stunAvoidance=stunAvoidance;
			stat.stunDuration=stunDuration;
			
			stat.silentChance=silentChance;
			stat.silentAvoidance=silentAvoidance;
			stat.silentDuration=silentDuration;
			
			stat.flankingBonus=flankingBonus;
			stat.flankedModifier=flankedModifier;
			
			stat.HPPerTurn=HPPerTurn;
			stat.APPerTurn=APPerTurn;
		
			return stat;
		}
	}
	
}