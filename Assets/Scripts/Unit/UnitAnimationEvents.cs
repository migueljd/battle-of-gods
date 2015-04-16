using UnityEngine;
using System.Collections;
using TBTK;

public class UnitAnimationEvents : MonoBehaviour {

	public delegate void AttackEvent(Unit unit);
	public event AttackEvent OnAttackEventE;

	private Unit parent;

	void Start(){
		parent = this.transform.parent.GetComponent<Unit> ();
	}

	public void Attack(){
		if (OnAttackEventE != null && parent.target != null)
			OnAttackEventE (parent.target);
		else if (parent.target == null)
			Debug.Log ("Target is null");
	}
}
