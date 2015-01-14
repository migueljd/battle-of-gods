using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using TBTK;

public class SceneModifier_FogOfWar : MonoBehaviour {
	
	// Use this for initialization
	void Start () {
		FactionManager.onUnitDeploymentPhaseE += DeploymentPhaseChanged;
	}
	
	void DeploymentPhaseChanged(bool flag){
		if(!flag) StartCoroutine(DelayStart());
	}
	IEnumerator DelayStart(){
		
		
		List<Unit> unitList=FactionManager.GetAllUnit();
		for(int i=0; i<unitList.Count; i++){
			Unit unit=unitList[i];
			
			unit.sight=4;
			unit.moveRange=3;
			//unit.sight+=1;
			
			if(unit.factionID==0) unit.SetupFogOfWar();
		}
		
		yield return null;
	}
	
}
