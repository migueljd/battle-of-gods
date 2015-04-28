using UnityEngine;
using System.Collections;

using UnityEngine.UI;

public class WikiManager : MonoBehaviour {

	private static WikiManager instance;


	public Transform buttonInUse;

	public Font normalFont;
	public Font selectedFont;
	public Image wikiInfo;

	void Awake(){
		if (instance == null) {
			instance = this;
		} else {
			Destroy(this.gameObject);
		}
	
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public static void ButtonClicked(Transform button){
		Debug.Log (instance);
		instance._ButtonClicked (button);
	}

	private void _ButtonClicked(Transform button){
		if (buttonInUse != null) {
			buttonInUse.GetComponent<Image>().enabled = false;
			buttonInUse.GetChild(0).GetComponent<Text>().font = normalFont;
		}
		
		buttonInUse = button;
		button.GetComponent<Image> ().enabled = true;
		buttonInUse.GetChild(0).GetComponent<Text>().font = selectedFont;
		wikiInfo.sprite = button.GetComponent<WikiButton> ().textImage;


	}


}
