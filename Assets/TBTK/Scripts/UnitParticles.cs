using UnityEngine;
using System.Collections;

using TBTK;

namespace TBTK
{
	public class UnitParticles : MonoBehaviour
	{

		private Unit unit;

		public static float height = 1.5f;

		
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
			}
			if (particleMove != null)
				particleMove.Stop ();
			if (particleAttack != null)
				particleAttack.Stop ();
		}

		
		
		public void Move(){
			particleMove.Play();
		}
		public void StopMove(){
			particleMove.Stop();
		}
		
		public void Attack(Unit targetUnit){
			particleAttack.transform.position = targetUnit.transform.position + new Vector3 (0, height, 0);
			particleAttack.Play();
		}

		public void EndAttack(){
			particleAttack.Stop ();
		}
		
		
	}
}

