using UnityEngine;
using System.Collections;

using UnityEngine.UI;


public class WikiButton : MonoBehaviour {

	public Sprite textImage;


	public void ButtonPress(){
		WikiManager.ButtonClicked (this.transform);
	}
}
