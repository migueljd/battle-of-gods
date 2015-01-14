using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;

using TBTK;

namespace TBTK{
	
	public class UIUnitInfoTooltip : MonoBehaviour {
		
		public Text lbName;
		public Text lbLevel;
		
		public Text lbBasic;
		public Text lbBasicAlt;
		
		public Text lbOffense;
		public Text lbOffenseAlt;
		
		public Text lbDesp;
		
		public List<UnityButton> abilityButtonList=new List<UnityButton>();
		
		private Unit currentUnit;
		
		public GameObject thisObj;
		private Transform thisT;
		
		
		public static UIUnitInfoTooltip instance;
		
		void Start(){
			instance=this;
			
			thisObj=gameObject;
			thisT=transform;
			
			for(int i=0; i<5; i++){
				if(i==0) abilityButtonList[0].Init();
				else if(i>0){
					abilityButtonList.Add(abilityButtonList[0].Clone("ItemButton"+(i+1), new Vector3(i*35, 0, 0)));
				}
				
				abilityButtonList[i].rootObj.SetActive(false);
			}
			
			thisObj.SetActive(false);
		}
		
		
		void OnEnable(){
			
		}
		void OnDisable(){
			
		}
		
		
		
		public static void Show(Unit unit){ instance._Show(unit); }
		public void _Show(Unit unit){
			
			currentUnit=unit;
			
			lbName.text=unit.unitName;
			lbLevel.text="lvl "+unit.GetLevel();
			
			lbBasic.text=unit.HP.ToString("f0")+"/"+unit.GetFullHP()+"\n\n\n";
			lbBasicAlt.text="\n"+unit.AP.ToString("f0")+"/"+unit.GetFullAP()+"\n\n\n";
			lbBasic.text+=unit.GetMoveRange().ToString("f0");
			lbBasicAlt.text+=unit.GetAttackRange().ToString("f0");
			
			lbOffense.text=unit.GetDamageMin().ToString("f0")+"-"+unit.GetDamageMax().ToString("f0")+"\n\n\n";
			lbOffenseAlt.text="\n\n"+(unit.GetHitChance()*100).ToString("f0")+"%\n\n";
			lbOffense.text+=(unit.GetDodgeChance()*100).ToString("f0")+"%";
			lbOffenseAlt.text+=(unit.GetCritChance()*100).ToString("f0")+"%";
			
			lbDesp.text=unit.desp;
			
			
			for(int i=0; i<abilityButtonList.Count; i++){
				if(i<unit.abilityList.Count){
					abilityButtonList[i].imageIcon.sprite=unit.abilityList[i].icon;
					abilityButtonList[i].rootObj.SetActive(true);
				}
				else abilityButtonList[i].rootObj.SetActive(false);
			}
			
			UpdatePos();
			
			thisObj.SetActive(true);
		}
		public static void Hide(){ instance._Hide(); }
		public void _Hide(){
			currentUnit=null;
			thisObj.SetActive(false);
		}
		
		
		void UpdatePos(){
			if(currentUnit==null) return;
			
			Vector3 screenPos = Camera.main.WorldToScreenPoint(currentUnit.thisT.position)/UI.GetScaleFactor();
			
			float posX=0;
			if(screenPos.x>(Screen.width/UI.GetScaleFactor())/2) posX=screenPos.x-230/2-60;
			else posX=screenPos.x+230/2+60;
			
			float posY=screenPos.y;
			if(screenPos.y<(Screen.height/UI.GetScaleFactor())/2) posY=screenPos.y+190;
			
			thisT.localPosition=new Vector3(posX, posY, 0);
		}
		
		
		//for showing the unit's ability info when the cursor hover over the ability ion
		public void OnHoverAbilityButton(GameObject butObj){
			/*
			for(int i=0; i<abilityButtonList.Count; i++){
				if(butObj==abilityButtonList[i].rootObj){
					int index=currentAbilityIndexList[i];
					lbDesp.text=abilityList[index].name+" - "+abilityList[index].desp;
					break;
				}
			}
			*/
		}
		public void OnExitHoverAbilityButton(){
			//lbDesp.text=currentunit.unit.desp;
		}
		
		
	}

}