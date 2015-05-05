using UnityEngine;
using System.Collections;

using Cards;
using TBTK;

namespace Cards
{
	public class ZeusCard : Card
	{
		public int turnsStunned;
		public int appliedDamage;
		
		void Awake(){
			base.BaseAwake ();
		}
		
		void Start(){
			magicCard = true;
		}
		
		public override void ActivateMagic(){
//			GridManager.TargetModeSelectedCallBack += PlayMagicParticle;
			AbilityManagerFaction.SelectAbility (0);
//			Effect stun = new Effect ();
//			stun.stun = 1;
//			stun.duration = turnsStunned;
//			GameControl.selectedTile.unit.ApplyEffect (stun);
//			GameControl.selectedTile.unit.ApplyDamage (appliedDamage);
		}


		public override bool CanUseCard(){
			if (FactionManager.GetAllHostileUnit (FactionManager.GetPlayerFactionID () [0]).Count > 0)
				return true;
			return false;
		}

}
}

