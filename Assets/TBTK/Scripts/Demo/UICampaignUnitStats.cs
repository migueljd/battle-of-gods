using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;

using TBTK;

namespace TBTK{
	
	public class UICampaignUnitStats : MonoBehaviour {
		
		public Text lbName;
		public Text lbLevel;
		
		public Text lbBasic;
		public Text lbBasicAlt;
		
		public Text lbOffense;
		public Text lbOffenseAlt;
		
		public Text lbDesp;
		
		public List<UnityButton> abilityButtonList=new List<UnityButton>();
		
		
		private List<UnitAbility> abilityList=new List<UnitAbility>();
		private List<int> currentAbilityIndexList=new List<int>();
		
		private DataUnit currentData=null;
		
		private GameObject currentPreviewObj;
		
		void Start(){
			for(int i=0; i<6; i++){
				//~ abilityButtonList[i].Init();
				//~ abilityButtonList[i].rootObj.SetActive(false);
				
				if(i==0) abilityButtonList[0].Init();
				else if(i>0){
					abilityButtonList.Add(abilityButtonList[0].Clone("ItemAbility"+(i+1), new Vector3(i*45, 0, 0)));
				}
				
				abilityButtonList[i].rootObj.SetActive(false);
			}
			
			abilityList=UnitAbilityDB.LoadClone();
		}
		
		
		void OnEnable(){
			if(currentData!=null) CreatePreviewObj();
		}
		void OnDisable(){
			if(currentPreviewObj!=null) Destroy(currentPreviewObj);
		}
		
		
		void CreatePreviewObj(){
			if(currentPreviewObj!=null) Destroy(currentPreviewObj);
			
			currentPreviewObj=(GameObject)Instantiate(currentData.unit.gameObject, Vector3.zero, Quaternion.identity);
			currentPreviewObj.transform.localScale*=4;
		}
		
		
		public void Show(Unit unit){
			DataUnit data=new DataUnit();
			data.Setup(unit);
			Show(data);
		}
		public void Show(DataUnit data){
			currentData=data;
			
			lbName.text=data.unit.unitName;
			lbLevel.text="level "+data.level;
			
			lbBasic.text=data.HP.ToString("f0")+"\n\n\n";
			lbBasicAlt.text="\n"+data.AP.ToString("f0")+"\n\n\n";
			lbBasic.text+=data.moveRange.ToString("f0");
			lbBasicAlt.text+=data.attackRange.ToString("f0");
			
			lbOffense.text=data.damageMin.ToString("f0")+"-"+data.damageMax.ToString("f0")+"\n\n\n";
			lbOffenseAlt.text="\n\n"+(data.hitChance*100).ToString("f0")+"%\n\n";
			lbOffense.text+=(data.dodgeChance*100).ToString("f0")+"%";
			lbOffenseAlt.text+=(data.critChance*100).ToString("f0")+"%";
			
			lbDesp.text=data.unit.desp;
			
			currentAbilityIndexList=new List<int>();
			for(int i=0; i<data.unit.abilityIDList.Count; i++){
				for(int n=0; n<abilityList.Count; n++){
					if(data.unit.abilityIDList[i]==abilityList[n].prefabID){
						currentAbilityIndexList.Add(n);
					}
				}
			}
			
			for(int i=0; i<abilityButtonList.Count; i++){
				if(i<currentAbilityIndexList.Count){
					int index=currentAbilityIndexList[i];
					abilityButtonList[i].imageIcon.sprite=abilityList[index].icon;
					abilityButtonList[i].rootObj.SetActive(true);
				}
				else abilityButtonList[i].rootObj.SetActive(false);
			}
			
			CreatePreviewObj();
		}
		
		
		public void OnHoverAbilityButton(GameObject butObj){
			for(int i=0; i<abilityButtonList.Count; i++){
				if(butObj==abilityButtonList[i].rootObj){
					int index=currentAbilityIndexList[i];
					lbDesp.text=abilityList[index].name+" - "+abilityList[index].desp;
					break;
				}
			}
			
		}
		public void OnExitHoverAbilityButton(){
			lbDesp.text=currentData.unit.desp;
		}
		
		
		//~ public float HP=-1;
		//~ public float AP=-1;
		
		//~ public float turnPriority=-1;
		//~ public int moveRange=-1;
		//~ public int attackRange=-1;
		
		//~ public float hitChance=-1;
		//~ public float dodgeChance=-1;
		//~ public float damageMin=-1;
		//~ public float damageMax=-1;
		
		//~ public float critChance=-1;
		//~ public float critAvoidance=-1;
		//~ public float critMultiplier=-1;
		
		//~ public float stunChance=-1;
		//~ public float stunAvoidance=-1;
		//~ public int stunDuration=-1;
		
		//~ public float HPPerTurn=-1;
		//~ public float APPerTurn=-1;
		
	}

}