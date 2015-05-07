using UnityEngine;
using System.Collections;

using TBTK;

namespace TBTK
{
	public class UnitParticles : MonoBehaviour
	{

		private Unit unit;

		public  float height = 1.5f;

		
		//public ParticleSystem particleIdle;
		public ParticleSystem particleMove;
		public ParticleSystem particleAttack;
//		public ParticleSystem particleHit;
		//public ParticleSystem particleDestroy;
		
		
		void Awake () {
			unit=gameObject.GetComponent<Unit>();

			if (unit != null) {
				unit.setParticles (this);
				if(particleAttack != null) particleAttack = (ParticleSystem)Instantiate(particleAttack, this.transform.position,Quaternion.identity);
				if(particleMove != null) particleMove = (ParticleSystem)Instantiate(particleMove, this.transform.position,Quaternion.identity);
				this.particleAttack.transform.SetParent(unit.transform);
			}
			if (particleMove != null)
				particleMove.Stop ();
			if (particleAttack != null)
				particleAttack.Stop ();
		}

		void OnEnable(){
			if(this.transform.GetChild (0) != null &&this.transform.GetChild (0).GetComponent<UnitAnimationEvents>() != null)
				this.transform.GetChild (0).GetComponent<UnitAnimationEvents>().OnAttackEventE += Attack;
		}

		void OnDisable(){
			if(this.transform.GetChild (0) != null &&this.transform.GetChild (0).GetComponent<UnitAnimationEvents>() != null)
				this.transform.GetChild (0).GetComponent<UnitAnimationEvents>().OnAttackEventE += Attack;
		}

		
		
		public void Move(){
			particleMove.Play();
		}
		public void StopMove(){
			particleMove.Stop();
		}
		
		public void Attack(Unit targetUnit){
			Debug.Log ("Called attack");
			if (particleAttack != null) {
				particleAttack.transform.position = targetUnit.transform.position + new Vector3 (0, height, 0);
				particleAttack.Play ();
			}

		}

		public void EndAttack(){
			particleAttack.Stop ();
		}
		
		
	}
}

