using UnityEngine;
using System.Collections;

public class CloseWiki : MonoBehaviour {



	public void ButtonPress(){

		if (GameTimeControler.IsButtonPressed ())
			Time.timeScale = GameTimeControler.GetFastTime ();
		else
			Time.timeScale = 1;
		Debug.Log (transform.parent);
		transform.parent.gameObject.SetActive (false);
	}
}
