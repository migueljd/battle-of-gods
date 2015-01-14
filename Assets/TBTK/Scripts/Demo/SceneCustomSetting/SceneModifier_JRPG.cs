using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using TBTK;

public class SceneModifier_JRPG : MonoBehaviour {
	
	// Use this for initialization
	void Start () {
		StartCoroutine(DelayStart());
	}
	
	IEnumerator DelayStart(){
		yield return null;
		yield return null;
		yield return null;
		
		List<Unit> unitList=FactionManager.GetAllUnit();
		for(int i=0; i<unitList.Count; i++){
			Unit unit=unitList[i];
			
			unit.moveRange=0;
			unit.attackRange=12;
			
			if(unit.factionID==0){
				unit.thisT.rotation=Quaternion.Euler(0, 30, 0);
			}
			else{
				unit.thisT.rotation=Quaternion.Euler(0, -150, 0);
			}
			
			for(int n=0; n<unit.abilityList.Count; n++){
				if(unit.abilityList[n].name=="Teleport"){
					unit.abilityList.RemoveAt(n);	n-=1;
				}
				if(unit.abilityList[n].name=="Battle Scanner"){
					unit.abilityList.RemoveAt(n);	n-=1;
				}
				if(unit.abilityList[n].name=="Tactical Charge"){
					unit.abilityList.RemoveAt(n);	n-=1;
				}
			}
		}
		
		
		List<DataUnit> dataList=Data.GetLoadData(0);
		Data.SetEndData(0, dataList);
	}
	
	
	
}
