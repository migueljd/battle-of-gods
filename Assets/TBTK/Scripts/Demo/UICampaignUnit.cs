using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;

using TBTK;

namespace TBTK{

	public class UICampaignUnit : MonoBehaviour {
		
		public Campaign campaign;
		public UICampaignUnitStats unitStats;
		
		public List<UnityButton> unitItemList=new List<UnityButton>();
		public List<UnityButton> selectedItemList=new List<UnityButton>();
		
		private int selectedID=0;
		
		private enum _Tab{AllUnit, SelectedUnit}	//to indicate which is the current selected tab, allUnit or selectedUnit
		private _Tab tab=_Tab.AllUnit;
		public Transform selectHighlightAllUnitT;
		public Transform selectHighlightSelectedT;
		
		public Unit selectedUnit;
		
		public Button buttonAdd;
		public Button buttonRemove;
		public Image buttonAddBlock;
		public Image buttonRemoveBlock;
		

		// Use this for initialization
		void Start () {
			List<Unit> allUnitList=campaign.allUnitList;
			List<DataUnit> selectedUnitList=campaign.GetSelectedUnitList();
			
			int count=0;	float y=0;
			for(int i=0; i<allUnitList.Count; i++){
				if(i==0) unitItemList[0].Init();
				else if(i>0){
					unitItemList.Add(unitItemList[0].Clone("ItemButton"+(count+1), new Vector3(count*60, y, 0)));
				}
				
				count+=1; 
				if(count==6){ count=0; y-=60; }
				
				unitItemList[i].imageIcon.sprite=allUnitList[i].iconSprite;
				unitItemList[i].label.text=allUnitList[i].value.ToString("f0");
			}
			
			count=0;	y=0;
			for(int i=0; i<14; i++){
				if(i==0) selectedItemList[0].Init();
				else if(i>0){
					selectedItemList.Add(selectedItemList[0].Clone("ItemButton"+(count+1), new Vector3(count*60, y, 0)));
				}
				
				count+=1; 
				if(count==6){ count=0; y-=60; }
				
				
				if(i<selectedUnitList.Count){
					selectedItemList[i].imageIcon.sprite=selectedUnitList[i].unit.iconSprite;
					selectedItemList[i].label.text=selectedUnitList[i].unit.value.ToString("f0");
				}
				else{
					selectedItemList[i].rootObj.SetActive(false);
				}
			}
			
			ClearSelectedTabSelection();
			OnAllItem(0);
		}
		
		
		void UpdateUnitDisplay(){
			if(tab==_Tab.AllUnit){
				List<Unit> allUnitList=campaign.allUnitList;
				selectedUnit=allUnitList[selectedID];
				unitStats.Show(allUnitList[selectedID]);
			}
			if(tab==_Tab.SelectedUnit){
				List<DataUnit> selectedUnitList=campaign.GetSelectedUnitList();
				selectedUnit=selectedUnitList[selectedID].unit;
				unitStats.Show(selectedUnitList[selectedID]);
			}
		}
		
		
		public void OnAddButton(){
			if(tab==_Tab.SelectedUnit) return;
			
			//for the purpose of the demo, we are using PerkCurrency as the main game currency
			//you can always change this to your own custom resource
			if(PerkManager.GetPerkCurrency()<selectedUnit.value){
				UIGameMessage.DisplayMessage("Insufficient Money");
				return;
			}
			PerkManager.SpendCurrency(selectedUnit.value);
			
			campaign.AddUnit(selectedID);
			UpdateTab();
		}
		public void OnRemoveButton(){
			if(tab==_Tab.AllUnit) return;
			
			//for the purpose of the demo, we are using PerkCurrency as the main game currency
			//you can always change this to your own custom resource
			PerkManager.SpendCurrency(-selectedUnit.value);
			
			campaign.RemoveUnit(selectedID);
			UpdateTab();
			
			int selectedCount=campaign.GetSelectedUnitList().Count;
			
			if(selectedCount==0) OnAllItem(0);
			else if(selectedID>=selectedCount) OnSelectedItem(selectedCount-1);
			else UpdateUnitDisplay();
		}
		
		
		void UpdateTab(){
			List<DataUnit> selectedUnitList=campaign.GetSelectedUnitList();
			for(int i=0; i<selectedItemList.Count; i++){
				if(i<selectedUnitList.Count){
					selectedItemList[i].imageIcon.sprite=selectedUnitList[i].unit.iconSprite;
					selectedItemList[i].label.text=selectedUnitList[i].unit.value.ToString("f0");
					selectedItemList[i].rootObj.SetActive(true);
				}
				else{
					selectedItemList[i].rootObj.SetActive(false);
				}
			}
		}
		
		
		
		public void OnAllItemButton(GameObject butObj){
			int newID=0;
			for(int i=0; i<unitItemList.Count; i++){
				if(butObj==unitItemList[i].rootObj){
					newID=i; 	break;
				}
			}
			
			if(newID==selectedID && tab==_Tab.AllUnit) return;
			
			OnAllItem(newID);
		}
		public void OnAllItem(int newID){
			if(tab==_Tab.SelectedUnit) ClearSelectedTabSelection();
			else ClearAllUnitTabSelection();
			
			selectedID=newID;
			tab=_Tab.AllUnit;
			SetToSelected(unitItemList[selectedID]);
			selectHighlightAllUnitT.localPosition=unitItemList[selectedID].rootT.localPosition;
			
			buttonAdd.interactable=true;
			buttonAddBlock.enabled=false;
			buttonRemove.interactable=false;
			buttonRemoveBlock.enabled=true;
			
			UpdateUnitDisplay();
		}
		
		public void OnSelectedItemButton(GameObject butObj){
			int newID=0;
			for(int i=0; i<selectedItemList.Count; i++){
				if(butObj==selectedItemList[i].rootObj){
					newID=i; 	break;
				}
			}
			
			if(newID==selectedID && tab==_Tab.SelectedUnit) return;
			
			OnSelectedItem(newID);
		}
		void OnSelectedItem(int newID){
			if(tab==_Tab.AllUnit) ClearAllUnitTabSelection();
			else ClearSelectedTabSelection();
			
			selectedID=newID;
			tab=_Tab.SelectedUnit;
			SetToSelected(selectedItemList[selectedID]);
			selectHighlightSelectedT.localPosition=selectedItemList[selectedID].rootT.localPosition;
			
			buttonAdd.interactable=false;
			buttonAddBlock.enabled=true;
			buttonRemove.interactable=true;
			buttonRemoveBlock.enabled=false;
			
			UpdateUnitDisplay();
		}
		
		
		void ClearSelectedTabSelection(){
			selectHighlightSelectedT.localPosition=new Vector3(-999, -999, 0);
			SetToNormal(selectedItemList[selectedID]);
		}
		void ClearAllUnitTabSelection(){
			selectHighlightAllUnitT.localPosition=new Vector3(-999, -999, 0);
			SetToNormal(unitItemList[selectedID]);
		}
		
		
		
		
		private Color colorSelected=new Color(1f, 75f/255f, 75f/255f, 1f);
		private Color colorNormal=new Color(0.5f, 0.5f, 0.5f, 196f/255f);
		
		public void SetToSelected(UnityButton unitItem=null){
			ColorBlock colors=unitItem.button.colors;
			colors.normalColor = colorSelected;
			unitItem.button.colors=colors;
		}
		public void SetToNormal(UnityButton unitItem){
			ColorBlock colors=unitItem.button.colors;
			colors.normalColor = colorNormal;
			unitItem.button.colors=colors;
			
			unitItem.imageIcon.color=Color.white;
		}
		
	}

}