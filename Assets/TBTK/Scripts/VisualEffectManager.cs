using UnityEngine;
using System.Collections;

using TBTK;

namespace TBTK{

	public class VisualEffectManager : MonoBehaviour {

		private static VisualEffectManager instance;
		
		void Awake(){
			instance=this;
		}
		
		
		public GameObject stunEffect;
		
		public static void UnitStunned(Unit unit, int duration){ 
			if(instance==null) return; 
			instance._UnitStunned(unit, duration);
		}
		public void _UnitStunned(Unit unit, int duration){
			Vector3 pos = Vector3.zero;

			if (unit.name.Contains ("Hades")) {
				pos = unit.thisT.position + new Vector3 (.25f, 2.5f, 0);

			} else {
				pos = unit.thisT.position + new Vector3 (.2f, 2.5f, -0.5f);
			}
			GameObject obj=(GameObject)Instantiate(stunEffect, pos, Quaternion.identity);
			obj.transform.parent=unit.thisT;
			
			//for SelfDeactivator 
			obj.SendMessage("Count", duration);
		}
		
	}

}