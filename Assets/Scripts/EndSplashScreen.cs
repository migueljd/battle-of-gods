using UnityEngine;
using System.Collections;

public class EndSplashScreen : MonoBehaviour {


	public void EndScreen () {
	
		StartCoroutine (EndSplashScreenRoutine());
	}

	IEnumerator EndSplashScreenRoutine (){
		yield return new WaitForSeconds (1);
		Application.LoadLevel (1);
	}
}
