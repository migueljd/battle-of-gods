using UnityEngine;
using System.Collections;

using UnityEngine.UI;
using TBTK;

using Cards;

public class HUDWikiButton : MonoBehaviour {

	public WikiManager wikiManager;

	public AudioClip clickAudio;

	public void ButtonPress(){
		Time.timeScale = 0;
	
		CardsHandManager.movingCard = true;
		wikiManager.gameObject.SetActive (true);
		AudioManager.PlaySound (clickAudio);
		UI.HideWikiArrow ();
	}
}
