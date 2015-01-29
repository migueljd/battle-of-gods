using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using TBTK;

public class Objective : MonoBehaviour {

	public enum _ObjectiveType{
		DestroyTemple, //this objective involves the destruction of a certain temple in the map
		KillTarget, //this objective involves destroying a target enemy
		KillAllEnemies //this objective involves killing all enemies units
	}
	[Tooltip("Type of objective needed to be completed to complete the level")]
	public _ObjectiveType objective = _ObjectiveType.KillAllEnemies;
	public _ObjectiveType GetObjective(){return instance.objective;}

	public static Objective instance;
	public Unit target;

	private int targetId;

	void Awake(){
		if(instance==null){ 
			instance=this;
		}
	}
	
	public static string objectiveText(){
		if(instance.objective == _ObjectiveType.KillTarget){
			return "Kill the " + instance.target.name;
		}
		else if(instance.objective == _ObjectiveType.DestroyTemple){
			return "Destroy the temple of Hades";
		}
		else return "Kill all enemy units";
	}

	public static bool objectiveCompleted(int ID){
		if(instance.objective == _ObjectiveType.KillTarget || instance.objective == _ObjectiveType.DestroyTemple){
			instance.targetId = instance.target.GetInstanceID();
			return ID == instance.targetId;
		}
		else {
			List<Faction> factionList = FactionManager.GetFactionList();
			return factionList.Count == 1 && FactionManager.IsPlayerFaction(factionList[0].ID);
		}
	}
}
