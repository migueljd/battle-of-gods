using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour {

	public Image img;

	public Sprite sprite2;
	public Sprite sprite3;
	public Sprite sprite4;
	public Sprite sprite5;
	public Sprite sprite6;
	public Sprite sprite7;
	public Sprite sprite8;
	public Sprite sprite9;
	public Sprite sprite10;

	private int a = 1;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyUp(KeyCode.Space) || Input.GetMouseButtonDown(0)){
			switch(a){
			case 1:
				img.sprite = sprite2;
				break;
			case 2:
				img.sprite = sprite3;
				break;
			case 3:
				img.sprite = sprite4;
				break;
			case 4:
				img.sprite = sprite5;
				break;
			case 5:
				img.sprite = sprite6;
				break;
			case 6:
				img.sprite = sprite7;
				break;
			case 7:
				img.sprite = sprite8;
				break;
			case 8:
				img.sprite = sprite9;
				break;
			case 9:
				img.sprite = sprite10;
				break;
			default:
				Application.LoadLevel(0);
				break;
			}
			a++;
		}
	}
}
