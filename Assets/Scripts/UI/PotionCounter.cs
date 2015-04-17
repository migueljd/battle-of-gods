using UnityEngine;
using System.Collections;
using UnityEngine.UI;

using TBTK;


public class PotionCounter : MonoBehaviour {

	public static PotionCounter instance;

	public int potionCount;

	public int potionPerTurn;

	public int PotionHPRecover;

	public int potionDropChance;

	private bool usedPotionThisTurn;

	private Text potionText;

	public void Awake(){
		if (instance == null) {
			DontDestroyOnLoad (this.gameObject);
			instance = this;
		}
		else
			Destroy (this.gameObject);
	}

	public void Start(){
		potionText = transform.GetChild (0).GetComponent<Text> ();
	}
	
	void OnEnable(){
		GameControl.onPassLevelE += OnPassLevel;
		Unit.onUnitDestroyedE += OnUnitDestroyed;
		GameControl.onIterateTurnE += PassTurn;
	}

	void OnDisable(){
		GameControl.onPassLevelE -= OnPassLevel;
		Unit.onUnitDestroyedE -= OnUnitDestroyed;
	}

	private void GainPotion(int potionIncrease){
		if(potionCount == 0) GetComponent<Image>().color = Color.white;
		potionCount+=potionIncrease;
		potionText.text = "Potion: " + potionCount;
	}

	public void UsePotion(){
		if (!usedPotionThisTurn && potionCount > 0 && FactionManager.IsPlayerTurn()) {
			Unit healedUnit = GameControl.selectedUnit.HP;
			healedUnit.HP = healedUnit.HP + PotionHPRecover > healedUnit.defaultHP? healedUnit.defaultHP : healedUnit.HP + PotionHPRecover;
			potionCount -= 1;
			usedPotionThisTurn = true;
			GetComponent<Image>().color = Color.black;
			potionText.text = "Potion: " + potionCount;
		}
	}

	public static void PassTurn(){
			instance.usedPotionThisTurn = false;	
			if(instance.potionCount == 0) instance.GetComponent<Image>().color = Color.black;
			else instance.GetComponent<Image>().color = Color.white;
	}

	public static void OnPassLevel(){
		instance.transform.parent = null;
	}

	public static void OnUnitDestroyed(Unit unit){
		if (unit.isAIUnit) {
			int potionDrop = Random.Range (0, 101);
			if (potionDrop <= instance.potionDropChance) {
				instance.GainPotion (1);
			}
		}
	}


}
