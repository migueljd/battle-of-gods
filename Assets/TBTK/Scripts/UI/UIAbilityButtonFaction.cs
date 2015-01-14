using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;

using TBTK;

namespace TBTK{

	public class UIAbilityButtonFaction : MonoBehaviour {
		
		public Transform anchor;
		
		public GameObject energyObj;
		public Text lbEnergy;
		
		public List<UnityButton> buttonList=new List<UnityButton>();
		
		public GameObject tooltipObj;
		public Text lbTooltipName;
		public Text lbTooltipDesp;
		public Text lbTooltipCost;
		
		
		private Color colorNormal=new Color(196f/255f, 196f/255f, 196f/255f, 196f/255f);
		private Color colorSelected=new Color(1f, 150f/255f, 0, 1f);
		
		private List<FactionAbility> currentFacAbilityList=new List<FactionAbility>();
		
		
		void Awake(){
			for(int i=0; i<6; i++){
				if(i==0) buttonList[0].Init();
				else if(i>0){
					//~ buttonList.Add(buttonList[0].Clone("button"+(i+1), new Vector3(i*55, 0, 0)));
					buttonList.Add(buttonList[0].Clone("button"+(i+1), new Vector3(i*55, 0, 0)));
				}
			}
		}
		
		// Use this for initialization
		void Start () {
			tooltipObj.SetActive(false);
			energyObj.SetActive(false);
			for(int i=0; i<buttonList.Count; i++) buttonList[i].rootObj.SetActive(false);
			
			//anchor.position=Vector3.zero;
		}
		
		
		// Update is called once per frame
		void Update () {
			
		}
		
		
		void OnEnable(){
			Unit.onUnitSelectedE += OnRefreshButtons;
			AbilityManagerFaction.onClearSelectedAbilityE += OnRefreshButtons;
		}
		void OnDisable(){
			Unit.onUnitSelectedE -= OnRefreshButtons;
			AbilityManagerFaction.onClearSelectedAbilityE -= OnRefreshButtons;
		}
		
		
		void OnRefreshButtons(){ OnRefreshButtons(GameControl.selectedUnit); }
		void OnRefreshButtons(Unit unit){
			if(unit==null){
				energyObj.SetActive(false);
				for(int i=0; i<buttonList.Count; i++) buttonList[i].rootObj.SetActive(false);
			}
			else{
				currentFacAbilityList=AbilityManagerFaction.GetFactionAbilityList(unit.factionID);
				
				for(int i=0; i<buttonList.Count; i++){
					if(i>=currentFacAbilityList.Count) buttonList[i].rootObj.SetActive(false);
					else{
						FactionAbility ability=currentFacAbilityList[i];
						
						buttonList[i].imageIcon.sprite=ability.icon;
						
						if(ability.IsAvailable()=="") buttonList[i].imageIcon.color=new Color(1, 1, 1, 1);
						else buttonList[i].imageIcon.color=new Color(.125f, .125f, .125f, 1);
						
						if(ability.useLimit>0) buttonList[i].label.text=(ability.useLimit-ability.useCount).ToString();
						else buttonList[i].label.text="";
						
						if(i==AbilityManagerFaction.GetSelectedAbilityID()){
							ColorBlock colors=buttonList[i].button.colors;
							colors.normalColor = colorSelected;
							buttonList[i].button.colors=colors;
							buttonList[i].rootT.localScale=new Vector3(1, 1, 1)*1.15f;
							buttonList[i].rootT.localPosition=new Vector3(buttonList[i].rootT.localPosition.x, 4, 0);
						}
						else{
							ColorBlock colors=buttonList[i].button.colors;
							colors.normalColor = colorNormal;
							buttonList[i].button.colors=colors;
							buttonList[i].rootT.localScale=new Vector3(1, 1, 1);
							buttonList[i].rootT.localPosition=new Vector3(buttonList[i].rootT.localPosition.x, 0, 0);
						}
						
						buttonList[i].rootObj.SetActive(true);
						
					}
				}
				
				if(currentFacAbilityList.Count>0){
					float energy=AbilityManagerFaction.GetFactionEnergy(unit.factionID);
					float energyFull=AbilityManagerFaction.GetFactionEnergyFull(unit.factionID);
					lbEnergy.text=energy+"/"+energyFull;
					energyObj.SetActive(true);
				}
				else{
					energyObj.SetActive(false);
				}
			}
		}
		
		
		
		public void OnAbilityButton(GameObject butObj){
			int ID=GetButtonID(butObj);
			string exception=AbilityManagerFaction.SelectAbility(ID);
			if(exception!="") UIGameMessage.DisplayMessage(exception);
			else OnRefreshButtons(GameControl.selectedUnit);
		}
		public void OnHoverAbilityButton(GameObject butObj){
			int ID=GetButtonID(butObj);
			Ability ability=currentFacAbilityList[ID];
			
			lbTooltipName.text=ability.name;
			lbTooltipDesp.text=ability.desp;
			lbTooltipCost.text="Cost: "+ability.cost+"AP";
			
			tooltipObj.SetActive(true);
		}
		public void OnExitHoverAbilityButton(GameObject butObj){
			tooltipObj.SetActive(false);
		}
		
		private int GetButtonID(GameObject butObj){
			for(int i=0; i<buttonList.Count; i++){
				if(butObj==buttonList[i].rootObj) return i;
			}
			return 0;
		}
		
	}

}
