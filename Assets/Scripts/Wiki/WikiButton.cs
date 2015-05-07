using UnityEngine;
using System.Collections;

using UnityEngine.UI;


public class WikiButton : MonoBehaviour {

	public Sprite textImage;


	public void ButtonPress(){
		Time.timeScale = 0;
		WikiManager.ButtonClicked (this.transform);
	}
}
