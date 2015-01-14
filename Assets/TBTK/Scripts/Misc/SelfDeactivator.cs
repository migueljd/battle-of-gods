using UnityEngine;
using System.Collections;

using TBTK;

namespace TBTK{

	public class SelfDeactivator : MonoBehaviour {
		
		public enum _Type{RealTime, TurnBased}
		public _Type timerTrackType=_Type.RealTime;

		public bool useObjectPool=true;
		public float duration=1;
		
		public DurationCounter durationCounter=new DurationCounter();
		
		
		void Start(){
			durationCounter.Count(duration);
		}
		
		public void Count(int dur){
			duration=dur;
			durationCounter.Count(duration);
		}
		
		void OnEnable(){
			if(timerTrackType==_Type.RealTime) StartCoroutine(DeactivateRoutine());
			else GameControl.onIterateTurnE += IterateDuration;
		}
		
		void OnDisable(){
			if(timerTrackType==_Type.TurnBased)
				GameControl.onIterateTurnE -= IterateDuration;
		}
		
		
		void IterateDuration(){
			durationCounter.Iterate();
			
			if(durationCounter.duration<=0){
				if(useObjectPool) ObjectPoolManager.Unspawn(gameObject);
				else Destroy(gameObject);
			}
		}
		
		
		IEnumerator DeactivateRoutine(){
			yield return new WaitForSeconds(duration);
			if(useObjectPool) ObjectPoolManager.Unspawn(gameObject);
			else Destroy(gameObject);
		}
		
	}

}