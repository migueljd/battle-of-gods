using UnityEngine;
using System.Collections;

public class Startscreen : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
//		if (Input.GetMouseButton (0) || Input.touchCount > 0) {
////			LoadingScreen.FadeIn ();
//			Application.LoadLevel(3);
//		}
	}

	public void StartGame(){
		StartCoroutine (StartG());
	}

	IEnumerator StartG(){
		LoadingScreen.FadeIn ();

		yield return new WaitForSeconds (4.0f);

		Application.LoadLevel(5);
		yield return null;
	}

	public void Credits(){
		Application.LoadLevel(6);
	}

	public void GoBackToMainMenu(){
		Application.LoadLevel (2);
	}
}
