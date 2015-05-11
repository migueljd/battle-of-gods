using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Cards;
using TBTK;

public class HephestusCurse : Card {

	public int AmountProtected;
	
	void Awake(){
		base.BaseAwake ();
	}
	
	void Start(){
		magicCard = true;
		guardCard = true;
		guard = AmountProtected;
	}
	
	public override void ActivateMagic(){

		AbilityManagerFaction.SetMagicCard (this);
		AbilityManagerFaction.SelectAbility (1);

	}
	
//	public void PlayParticle(Vector3 position){
////		particles.enableEmission = true;
////		particles.transform.position = Camera.main.ViewportToWorldPoint (new Vector3 (0.5f, 0.5f, 12));
////		particles.transform.parent = Camera.main.transform;
////		particles.Play ();
////		float timeToEnd = Time.time + particles.duration;
////		while(Time.time < timeToEnd) yield return null;
////		
////		particles.Stop ();
////		particles.transform.parent = null;
//	}
}
