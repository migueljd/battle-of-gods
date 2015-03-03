using UnityEngine;
using System.Collections;

public class ChooseLevel : MonoBehaviour {

	public void loadLevel(int level){
		Application.LoadLevel(level);
	}

}
