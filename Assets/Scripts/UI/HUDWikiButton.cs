using UnityEngine;
using System.Collections;

using UnityEngine.UI;
using TBTK;


public class HUDWikiButton : MonoBehaviour {

	public WikiManager wikiManager;

	public AudioClip clickAudio;

	public void ButtonPress(){
		Time.timeScale = 0;
		wikiManager.gameObject.SetActive (true);
		AudioManager.PlaySound (clickAudio);
	}
}
