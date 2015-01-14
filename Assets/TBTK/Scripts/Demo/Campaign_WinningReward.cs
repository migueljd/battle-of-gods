using UnityEngine;
using System.Collections;

using TBTK;

namespace TBTK{

	public class Campaign_WinningReward : MonoBehaviour {

		void OnEnable(){
			GameControl.onGameOverE += OnGameOver;
		}
		void OnDisable(){
			GameControl.onGameOverE -= OnGameOver;
		}
		
		//add currency when finishing a battle, for the sake of the demo, perk currency is used
		void OnGameOver(int facID){
			PerkManager.SpendCurrency(-30);	//use negative value on SpendCurrency() to add to the perk currency, otherwise it would subtract
		}
		
	}

}