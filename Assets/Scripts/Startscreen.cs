using UnityEngine;
using System.Collections;

public class Startscreen : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButton(0) || Input.touchCount > 0){
			Application.LoadLevel(3);
		}
	}
}
