using UnityEngine;
using System.Collections;

using TBTK;
using UnityEngine.UI;

public class PlayerHP : MonoBehaviour {

	public Unit playerUnit;
	private Faction fac;
	private Text text;


	void Awake(){
		text = this.transform.GetComponent<Text> ();
	}

	void Start(){
		fac = FactionManager.GetFaction(playerUnit.factionID);

	}

	
	// Update is called once per frame
	void Update () {
		if(text != null && fac != null) this.text.text = "Player HP: " + fac.factionHp;
	}
}
