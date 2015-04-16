using UnityEngine;
using System.Collections;

using UnityEngine.UI;

using TBTK;

public class SelectorAnimator : MonoBehaviour {
		
	public float maxY;
	public float minY;

	public float changeYPerFrame;

	private float Y;

	private bool increasing;

	public Unit unit;
	
	// Update is called once per frame
	void Update () {
		if (unit != null) {
			Debug.Log ("here");
			Vector3 screenPos = Camera.main.WorldToScreenPoint (unit.transform.position);
			transform.localPosition = (screenPos + new Vector3 (-10, 120, 0)) / UI.GetScaleFactor ();
//			if (increasing && Y < maxY) {
//				Y += changeYPerFrame;
//				this.transform.localPosition += Camera.main.WorldToScreenPoint(unit.transform.position + new Vector3 (0, changeYPerFrame, 0));
//			} else {
//				Y -= changeYPerFrame;
//				this.transform.localPosition -= Camera.main.WorldToScreenPoint(unit.transform.position + new Vector3 (0, changeYPerFrame, 0));
//			}
//
//			if (Y >= maxY)
//				increasing = false;
//			else if (Y <= minY)
//				increasing = true;
		}
	}
}
