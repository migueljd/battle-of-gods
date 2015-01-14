using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using TBTK;

namespace TBTK{

	public class Campaign : MonoBehaviour {
		
		public List<Unit> allUnitList=new List<Unit>();
		
		//see the class defination in TBTK_Class_Data
		private List<DataUnit> selectedUnitList=new List<DataUnit>();
		public List<DataUnit> GetSelectedUnitList(){ return selectedUnitList; }
		
		void Awake(){
			//check if any previous data exist
			if(Data.EndDataExist()){
				//get the data with ID of 0 from previous level, for the purpose of the demo, all data uses ID of 0
				selectedUnitList=Data.GetEndData(0);
				
				//these are the surviving unit from previous battle, level up them
				for(int i=0; i<selectedUnitList.Count; i++){
					selectedUnitList[i].level+=1;
					
					selectedUnitList[i].HP+=5;
					selectedUnitList[i].AP+=5;
					
					selectedUnitList[i].hitChance+=0.04f;
					selectedUnitList[i].dodgeChance+=0.04f;
					selectedUnitList[i].damageMin+=1;
					selectedUnitList[i].damageMax+=1;
				}
				
				//this is disabled and moved to Campaign_WinningReward.cs
				//StartCoroutine(AddCurrency());
			}
		}
		
		//add currency when surviving a battle, for the sake of the demo, perk currency is used
		IEnumerator AddCurrency(){
			yield return null;							//give it a frame of delay so PerkManager can set thing up
			PerkManager.SpendCurrency(-30);	//use negative vale on SpendCurrency() to add to the perk currency, otherwise it would subtract
		}
		
		
		//add unit to the selectedUnitList, called from UI
		public void AddUnit(int ID){
			DataUnit data=new DataUnit();
			data.Setup(allUnitList[ID]);
			selectedUnitList.Add(data);
		}
		//remove unit from the selectedUnitList, called from UI
		public void RemoveUnit(int ID){
			selectedUnitList.RemoveAt(ID);
		}
		
		
		public void OnPlayButton(string lvlName){
			if(selectedUnitList.Count==0){
				//if(onMessageE!=null) onMessageE("You must select at least one unit");
				UIGameMessage.DisplayMessage("You must select at least one unit");
				return;
			}
			
			//set the selectedUnitList to data with ID of 0
			//the campaign scene are set to load data with ID of 0 as player starting unit
			Data.SetLoadData(0, selectedUnitList);
			Application.LoadLevel(lvlName);
		}
		
		
		
		
	}

}