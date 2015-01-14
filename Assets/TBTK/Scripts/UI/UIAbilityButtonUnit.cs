using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;

using TBTK;

namespace TBTK{

	public class UIAbilityButtonUnit : MonoBehaviour {
		
		public Transform anchor;

		public List<UnityButton> buttonList=new List<UnityButton>();
		
		public GameObject tooltipObj;
		public Text lbTooltipName;
		public Text lbTooltipDesp;
		public Text lbTooltipCost;
		
		private Vector3 buttonDefaultPos;
		
		private Color colorNormal=new Color(196f/255f, 196f/255f, 196f/255f, 196f/255f);
		private Color colorSelected=new Color(1f, 150f/255f, 0, 1f);
		
		//private GameObject thisObj;
		//private static UIUnitAbility instance;
		
		public void Awake(){
			//instance=this;
			//thisObj=gameObject;
			
			for(int i=0; i<6; i++){
				if(i==0) buttonList[0].Init();
				else if(i>0){
					//buttonList.Add(buttonList[0].Clone("button"+(i+1), new Vector3(i*55, 0, 0)));
					buttonList.Add(buttonList[0].Clone("button"+(i+1), Vector3.zero));
				}
			}
			
			buttonDefaultPos=buttonList[0].rootT.localPosition;
		}
		
		void Start () {
			tooltipObj.SetActive(false);
			
			OnUnitSelected(null);
		}
		
		
		
		
		void OnEnable(){
			Unit.onUnitSelectedE += OnUnitSelected;
		}
		void OnDisable(){
			Unit.onUnitSelectedE -= OnUnitSelected;
		}
		
		
		void OnUnitSelected(Unit unit){
			if(unit==null){
				for(int i=0; i<buttonList.Count; i++) buttonList[i].rootObj.SetActive(false);
			}
			else{
				List<UnitAbility> abilityList=unit.GetAbilityList();
				
				float offset=0.5f * (abilityList.Count-1) * 55;
				for(int i=0; i<abilityList.Count; i++){
					buttonList[i].rootT.localPosition=buttonDefaultPos+new Vector3(i*55-offset, 0, 0);
				}
				
				for(int i=0; i<buttonList.Count; i++){
					if(i>=abilityList.Count) buttonList[i].rootObj.SetActive(false);
					else{
						UnitAbility ability=abilityList[i];
						
						buttonList[i].imageIcon.sprite=ability.icon;
						
						if(ability.IsAvailable()=="") buttonList[i].imageIcon.color=new Color(1, 1, 1, 1);
						else buttonList[i].imageIcon.color=new Color(.125f, .125f, .125f, 1);
						
						if(ability.useLimit>0) buttonList[i].label.text=(ability.useLimit-ability.useCount).ToString();
						else buttonList[i].label.text="";
						
						if(i==unit.GetSelectedAbilityID()){
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
			}
		}
		
		
		public void OnAbilityButton(GameObject butObj){
			int ID=GetButtonID(butObj);
			string exception=GameControl.selectedUnit.SelectAbility(ID);
			if(exception!=null) UIGameMessage.DisplayMessage(exception);
		}
		
		
		public void OnHoverAbilityButton(GameObject butObj){
			int ID=GetButtonID(butObj);
			Ability ability=GameControl.selectedUnit.GetUnitAbility(ID);
			
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