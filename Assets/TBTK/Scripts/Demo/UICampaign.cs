using UnityEngine;
using UnityEngine.UI;

using System.Collections;

using TBTK;

namespace TBTK{

	public class UICampaign : MonoBehaviour {

		public Campaign campaign;
		
		public Text lbPoints;
		
		public UnityButton switchButton;
		
		private enum _Tab{Unit, Perk}
		private _Tab tab=_Tab.Unit;
		public GameObject unitMenuObj;
		public UIPerkMenu perkMenu;
		
		public GameObject mainObj;
		public GameObject SceneMenuObj;
		
		
		void Start(){
			switchButton.Init();
			
			//perkMenuObj.SetActive(false);
			SceneMenuObj.SetActive(false);
		}
		
		void Update(){
			//for the purpose of the demo, we are using PerkCurrency as the main game currency
			//you can always change this to your own custom resource
			lbPoints.text="$"+PerkManager.GetPerkCurrency().ToString("f0");
		}
		
		public void OnSwitchButton(){
			if(tab==_Tab.Unit){
				tab=_Tab.Perk;
				unitMenuObj.SetActive(false);
				//perkMenuObj.SetActive(true);
				perkMenu._Show(true);
				switchButton.label.text="Unit Menu";
			}
			else if(tab==_Tab.Perk){
				tab=_Tab.Unit;
				unitMenuObj.SetActive(true);
				//perkMenuObj.SetActive(false);
				perkMenu._Hide(true);
				switchButton.label.text="Perk Menu";
			}
		}
		
		public void OnNextButton(){
			mainObj.SetActive(false);
			unitMenuObj.SetActive(false);
			perkMenu._Hide(true);
			
			SceneMenuObj.SetActive(true);
		}
		
		public void BackFromLevel(){
			SceneMenuObj.SetActive(false);
			
			mainObj.SetActive(true);
			
			if(tab==_Tab.Unit) tab=_Tab.Perk;
			else if(tab==_Tab.Perk) tab=_Tab.Unit;
			OnSwitchButton();
		}
		
		
		
		public void OnMenuButton(){
			Application.LoadLevel("TBTK_Menu_Demo");
		}
		
	}

}