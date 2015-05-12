﻿using UnityEngine;
using System.Collections;

using TBTK;
using Cards;

public class CloseWiki : MonoBehaviour {

	public AudioClip clickAudio;



	public void ButtonPress(){

		CardsHandManager.movingCard = false;

		if (GameTimeControler.IsButtonPressed ())
			Time.timeScale = GameTimeControler.GetFastTime ();
		else
			Time.timeScale = 1;
		transform.parent.gameObject.SetActive (false);
		AudioManager.PlaySound (clickAudio);

	}
}
