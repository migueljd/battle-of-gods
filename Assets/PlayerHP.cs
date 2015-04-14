using UnityEngine;
using System.Collections;

using TBTK;
using UnityEngine.UI;

public class PlayerHP : MonoBehaviour {

	private static PlayerHP instance;

	public Unit playerUnit;
	private Faction fac;
	private Text text;

	public Color defaultColor;
	public Color healColor;
	public Color damageColor;

	public float pulseTime;
	public float intervalPerIncrease;

	public int maxSize;
	public int baseSize;

	void Awake(){
		instance = this;
		text = this.transform.GetComponent<Text> ();
		
	}

	void Start(){
		fac = FactionManager.GetFaction(playerUnit.factionID);
		baseSize = text.fontSize;
	}




	private IEnumerator PulseHP(bool damage){
		float interval = pulseTime / intervalPerIncrease;
		int a = 0;
		text.color = damage ? damageColor : healColor;
		Debug.Log ("Changing");
		while (text.fontSize != maxSize) {
			text.fontSize =(int) Mathf.Lerp(baseSize, maxSize, interval*a);
			a++;
			yield return new WaitForSeconds(intervalPerIncrease);
		}
		a = 0;
		while (text.fontSize != baseSize) {
			text.fontSize =(int) Mathf.Lerp(maxSize, baseSize, interval*a);
			a++;
			yield return new WaitForSeconds(intervalPerIncrease);
		}
		text.color = defaultColor;
	}

	public static void UpdatePlayerHP(bool damage){
		instance._UpdatePlayerHP (damage);
	}

	private void _UpdatePlayerHP(bool damage){
		if (text != null && fac != null) {
			this.text.text = FactionManager.playerHP.ToString();
			StartCoroutine(PulseHP (damage));
		}
	}

}
