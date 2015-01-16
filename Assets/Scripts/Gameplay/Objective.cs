using UnityEngine;
using System.Collections;

using TBTK;

public class Objective : MonoBehaviour {

	public enum _ObjectiveType{
		DestroyTemple, //this objective involves the destruction of a certain temple in the map
		KillTarget, //this objective involves destroying a target enemy
		KillAllEnemies //this objective involves killing all enemies units
	}
	[Tooltip("Type of objective needed to be completed to complete the level")]
	public _ObjectiveType objective;
	public Unit target;

}
