using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Cards;
using TBTK;

namespace Cards
{
	public class AresCard : Card
	{
		public int AmountDamaged;
		
		void Awake(){
			base.BaseAwake ();
		}
		
		void Start(){
			magicCard = true;
		}
		
		public override void ActivateMagic(){
			Debug.Log ("Activated");
			//Animate somehow
			List<Unit> enemies = FactionManager.GetAllHostileUnit (GameControl.selectedUnit.factionID);
			foreach (Unit enemy in enemies) {
				enemy.ApplyDamage(AmountDamaged);
			}
		}
		
		public override IEnumerator PlayParticle(Vector3 position){
			particles.enableEmission = true;
			particles.transform.position = Camera.main.ViewportToWorldPoint (new Vector3 (0.5f, 0.5f, 12));
			particles.transform.parent = Camera.main.transform;
			particles.Play ();
			float timeToEnd = Time.time + particles.duration;
			while(Time.time < timeToEnd) yield return null;
			
			particles.Stop ();
			particles.transform.parent = null;
		}
		
	}
}

