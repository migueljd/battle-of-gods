﻿using UnityEngine;
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

	private Vector3 basePosition;

	public AudioClip potionAudio;

	public void Awake(){
		if (instance == null) {
			DontDestroyOnLoad (this.gameObject);
			instance = this;
			basePosition=this.transform.localPosition;
		}
		else
			Destroy (this.gameObject);
	}

	public void Start(){
		potionText = transform.GetChild (0).GetComponent<Text> ();
	}

	void OnLevelWasLoaded(int lvl){
		if(instance == this)
			OnEnable ();
	}
	
	void OnEnable(){
		if (instance == this) {
			GameControl.onPassLevelE += OnPassLevel;
			GameControl.onGameStartE += PositionPotionButton;
			GameControl.onGameRestartE += RestartGame;
			Unit.onUnitDestroyedE += OnUnitDestroyed;
			GameControl.onIterateTurnE += PassTurn;
		}
	}

	void OnDisable(){
		if (instance == this) {
			GameControl.onPassLevelE -= OnPassLevel;
			Unit.onUnitDestroyedE -= OnUnitDestroyed;
			GameControl.onGameStartE -= PositionPotionButton;
			GameControl.onGameRestartE -= RestartGame;

		}

	}

	public static void PositionPotionButton(){
		GameControl.AddActionAtStart ();
		instance.transform.SetParent(GameObject.FindGameObjectsWithTag ("AnchorBottonLeft")[0].transform);
		instance.transform.localPosition = instance.basePosition;
		GameControl.CompleteActionAtStart ();
	}

	private void GainPotion(int potionIncrease){

		StartCoroutine (GainPotion ());

		if(potionCount == 0) GetComponent<Button>().interactable = true;
		potionCount+=potionIncrease;
		potionText.text = potionCount.ToString();
	}

	public void UsePotion(){
		if (!usedPotionThisTurn && potionCount > 0 && FactionManager.IsPlayerTurn()) {
			if(potionAudio != null) AudioManager.PlaySound(potionAudio);
			Unit healedUnit = GameControl.selectedUnit;
			healedUnit.HP = healedUnit.HP + PotionHPRecover > healedUnit.defaultHP? healedUnit.defaultHP : healedUnit.HP + PotionHPRecover;
			potionCount -= 1;
			usedPotionThisTurn = true;
			GetComponent<Button>().interactable = false;
			potionText.text = potionCount.ToString();
			GameControl.ChooseSelectedUnit();
		}
	}

	IEnumerator GainPotion(){
		Text child = this.transform.GetChild (0).GetComponent<Text>();

		float interpolate = 0;

		int initialFontSize = child.fontSize;
		int finalFontSize = 200;

		child.color = Color.green;

		while (interpolate <= 1) {

			child.fontSize =(int) Mathf.Lerp(initialFontSize, finalFontSize, interpolate);
			interpolate+=0.2f;

			yield return null;
		}
		
		interpolate = 0;
		while (interpolate <= 1) {
			
			child.fontSize =(int) Mathf.Lerp(finalFontSize, initialFontSize, interpolate);
			interpolate+=0.2f;
			
			yield return null;
		}

		child.color = Color.white;


	}

	public static void PassTurn(){
		instance.usedPotionThisTurn = false;	
		if(instance.potionCount == 0) instance.GetComponent<Button>().interactable = false;
		else instance.GetComponent<Button>().interactable = true;
	}

	public static void OnPassLevel(){
		GameControl.AddActionAtPassLevel ();

		instance.OnDisable ();
		instance.transform.SetParent(null);

		GameControl.CompleteActionAtPassLevel ();
	}

	public static void OnUnitDestroyed(Unit unit){
		if (unit.isAIUnit) {
			int potionDrop = Random.Range (0, 101);
			if (potionDrop <= instance.potionDropChance) {
				instance.GainPotion (1);
			}
		}
	}

	public static void RestartGame(){
		instance.OnDisable ();
		Destroy (instance.gameObject);
	}


}
