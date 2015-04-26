using UnityEngine;
using System.Collections;

using Cards;
using TBTK;

namespace Cards
{
	public class AphroditeCard : Card
	{
		public int AmountHealed;

		void Awake(){
			base.BaseAwake ();
		}

		void Start(){
			magicCard = true;
		}

		public override void ActivateMagic(){
			Debug.Log ("Activated");
			//Animate somehow
			FactionManager.playerHP += AmountHealed;
			PlayerHP.UpdatePlayerHP (false);
		}

		public override IEnumerator PlayParticle(Vector3 position){
			particles.transform.position = Camera.main.ViewportToWorldPoint (new Vector3 (0.5f, 0.5f, 2));
			particles.transform.parent = Camera.main.transform;
			particles.Play ();
			float timeToEnd = Time.time + particles.duration;
			while(Time.time < timeToEnd) yield return null;

			particles.Stop ();
			particles.transform.parent = null;

		}

	}
}

