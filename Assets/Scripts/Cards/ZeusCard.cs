using UnityEngine;
using System.Collections;

using Cards;
using TBTK;

namespace Cards
{
	public class ZeusCard : Card
	{
		public int turnsStunned;
		
		void Awake(){
			base.BaseAwake ();
		}
		
		void Start(){
			magicCard = true;
		}
		
		public override void ActivateMagic(){
			Effect stun = new Effect ();
			stun.stun = turnsStunned;
//			stun.duration = turnsStunned;
			GameControl.selectedTile.unit.ApplyEffect (stun);

		}
		
		public override IEnumerator PlayParticle(Vector3 position){
			particles.transform.position = GameControl.selectedTile.transform.position + new Vector3(0, height,0);
			particles.Play ();
			float timeToEnd = Time.time + particles.duration;
			while(Time.time < timeToEnd) yield return null;
			
			particles.Stop ();
		}

		public override bool CanUseCard(){
			//check if the selected tile has an enemy unit
			if (GameControl.selectedTile != null && GameControl.selectedTile.unit != null && GameControl.selectedTile.unit.factionID != GameControl.selectedUnit.factionID) 
				return true;
			return false;
		}

}
}

