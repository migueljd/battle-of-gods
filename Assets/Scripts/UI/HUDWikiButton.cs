using UnityEngine;
using System.Collections;

using UnityEngine.UI;


public class HUDWikiButton : MonoBehaviour {

	public WikiManager wikiManager;

	public void ButtonPress(){
		Time.timeScale = 0;
		wikiManager.gameObject.SetActive (true);
	}
}
