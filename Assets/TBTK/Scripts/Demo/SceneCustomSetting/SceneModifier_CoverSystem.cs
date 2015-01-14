using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using TBTK;

public class SceneModifier_CoverSystem : MonoBehaviour {
	
	// Use this for initialization
	void Start () {
		FactionManager.onUnitDeploymentPhaseE += DeploymentPhaseChanged;
	}
	
	void DeploymentPhaseChanged(bool flag){
		if(!flag) StartCoroutine(DelayStart());
	}
	IEnumerator DelayStart(){
		yield return null;
		yield return null;
		yield return null;
		
		List<Unit> unitList=FactionManager.GetAllUnit();
		for(int i=0; i<unitList.Count; i++){
			Unit unit=unitList[i];
			
			unit.movePerTurn=2;
			unit.moveRemain=2;
			//unit.sight+=1;
		}
	}
	
}
