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
			Effect stun = new Effect ();
			stun.stun = 1;
			stun.duration = turnsStunned;
			GameControl.selectedTile.unit.ApplyEffect (stun);
			GameControl.selectedTile.unit.ApplyDamage (appliedDamage);
		}
		
		public override IEnumerator PlayParticle(Vector3 position){

			particles.enableEmission = true;
			particles.transform.SetParent (null);
			particles.transform.position = GameControl.selectedTile.unit.transform.position + new Vector3(0,height,0);
			
			particles.Play ();
			float timeToEnd = Time.time + particles.duration;
			while(Time.time < timeToEnd) yield return null;
			
			particles.Stop ();
			particles.transform.SetParent (this.transform);
			Debug.Log ("parent set");
		}

		public override bool CanUseCard(){
			//check if the selected tile has an enemy unit
			if (GameControl.selectedTile != null && GameControl.selectedTile.unit != null && GameControl.selectedTile.unit.factionID != GameControl.selectedUnit.factionID) 
				return true;
			return false;
		}

}
}

