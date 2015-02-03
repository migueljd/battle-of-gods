using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;
using TBTK;

public class ObjectiveRandomizer : MonoBehaviour {

	public Unit target;
	public string templeTag = "Temple";
	public Text text;

	// Use this for initialization
	void Start () {
	
		Objective obj = Objective.instance;
//		int a = Random.Range(0,3);
		int a = 0;
		if(a == 0){
			obj.objective = Objective._ObjectiveType.KillAllEnemies;
			destroyTemple();
			destroyTarget();
		}
		else if(a == 1){
			destroyTarget();
			GameObject[] list = GameObject.FindGameObjectsWithTag(templeTag);
			obj.target = list[0].GetComponent<MultiTileUnit>();
			obj.objective = Objective._ObjectiveType.DestroyTemple;
		}
		else{
			obj.target = this.target;
			obj.objective =Objective._ObjectiveType.KillTarget;
			destroyTemple();
		}

		text.text = Objective.objectiveText();

	}

	void destroyTemple(){
		GameObject[] list = GameObject.FindGameObjectsWithTag(templeTag);
		if(list.Length > 0){
			MultiTileUnit temple = list[0].GetComponent<MultiTileUnit>();
			Debug.Log(temple);
			temple.ApplyDamage(100000, false, false);
		}

	}

	void destroyTarget(){
		target.ApplyDamage(100000, false, false);
	}
}
