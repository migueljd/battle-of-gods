using UnityEngine;
using System.Collections;
using Cards;

public class ChooseLevel : MonoBehaviour {

	public void loadLevel(int level){
		Application.LoadLevel(level);
	}

	public void saveDeck(int level){
		CardsHandManager.changeModeToGameOn ();
		loadLevel (level);
	}

}
