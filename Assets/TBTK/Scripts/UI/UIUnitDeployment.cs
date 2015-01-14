using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;

using TBTK;

namespace TBTK{

	public class UIUnitDeployment : MonoBehaviour {
		
		public Transform anchor;

		public List<UnityButton> unitIconList=new List<UnityButton>();
		
		public UnityButton buttonAutoDoneObject;
		
		public Text lbUndeployedCount;
		
		private GameObject thisObj;
		private static UIUnitDeployment instance;
		
		public void Awake(){
			instance=this;
			thisObj=gameObject;
			
			for(int i=0; i<unitIconList.Count; i++){
				unitIconList[i].Init();
			}
			
			buttonAutoDoneObject.Init();
		}
		
		void Start () {
			Hide();
			
			anchor.position=new Vector3(Screen.width/2, 0, 0);
		}
		
		
		public int currentUnitID=0;
		
		
		
		void OnEnable(){
			FactionManager.onUnitDeployedE += OnUnitDeployed;
		}
		void OnDisable(){
			FactionManager.onUnitDeployedE -= OnUnitDeployed;
		}
		
		void OnUnitDeployed(Unit unit){
			UpdateView();
		}
		
		public void OnNextButton(){
			FactionManager.PrevDeployingUnitID();
			UpdateView();
		}
		public void OnPrevButton(){
			FactionManager.NextDeployingUnitID();
			UpdateView();
		}
		public void OnCompleteButton(){
			if(FactionManager.IsDeploymentComplete()) FactionManager.CompleteDeployment();
			else{
				FactionManager.AutoDeployCurrentFaction();
				UpdateView();
			}
		}
		
		
		public void UpdateView(){
			List<Unit> unitList=FactionManager.GetDeployingUnitList();
			int unitID=FactionManager.GetDeployingUnitID();
			
			lbUndeployedCount.text=unitList.Count.ToString();
			
			List<Unit> newList=new List<Unit>();
			for(int i=0; i<unitList.Count; i++){
				newList.Add(unitList[unitID]);
				unitID+=1;
				if(unitID>=unitList.Count) unitID=0;
			}
			unitList=newList;
			
			//~ string text="("+unitID+")   ";
			//~ for(int i=0; i<unitList.Count; i++){
				//~ text+=unitList[i].prefabID+"   ";
			//~ }
			//~ Debug.Log(text);
			
			int point=Mathf.Min(3, unitList.Count/2+1);
			
			newList=new List<Unit>();
			int count=unitList.Count>=5 ? 2 : 1;
			for(int i=0; i<Mathf.Min(5, unitList.Count); i++){
				if(i<point){
					newList.Add(unitList[i]);
				}
				else{
					newList.Add(unitList[unitList.Count-count]);
					count-=1;
				}
			}
			unitList=newList;
			
			//~ text="("+unitID+")   ";
			//~ for(int i=0; i<unitList.Count; i++){
				//~ text+=unitList[i].prefabID+"   ";
			//~ }
			//~ Debug.Log(text);
			
			if(unitList.Count<=2){
				for(int i=0; i<unitList.Count; i++){
					unitIconList[i].imageIcon.sprite=unitList[i].iconSprite;
					unitIconList[i].rootObj.SetActive(true);
				}
				for(int i=unitList.Count; i<5; i++) unitIconList[i].rootObj.SetActive(false);
			}
			else if(unitList.Count==3){
				for(int i=0; i<2; i++){
					unitIconList[i].imageIcon.sprite=unitList[i].iconSprite;
					unitIconList[i].rootObj.SetActive(true);
				}
				
				unitIconList[4].imageIcon.sprite=unitList[2].iconSprite;
				unitIconList[4].rootObj.SetActive(true);
				
				unitIconList[2].rootObj.SetActive(false);
				unitIconList[3].rootObj.SetActive(false);
			}
			else if(unitList.Count==4){
				for(int i=0; i<3; i++){
					unitIconList[i].imageIcon.sprite=unitList[i].iconSprite;
					unitIconList[i].rootObj.SetActive(true);
				}
				
				unitIconList[4].imageIcon.sprite=unitList[3].iconSprite;
				unitIconList[4].rootObj.SetActive(true);
				
				unitIconList[3].rootObj.SetActive(false);
			}
			else if(unitList.Count>=5){
				for(int i=0; i<5; i++){
					if(i<unitList.Count){
						unitIconList[i].imageIcon.sprite=unitList[i].iconSprite;
						unitIconList[i].rootObj.SetActive(true);
					}
					else{
						unitIconList[i].rootObj.SetActive(false);
					}
				}
			}
			
			if(FactionManager.IsDeploymentComplete())
				buttonAutoDoneObject.label.text="Done";
			else
				buttonAutoDoneObject.label.text="Auto";
			
			
			
			//buttonDoneObject.SetActive(FactionManager.DeploymentComplete());
		}
		
		
		public static bool isOn=true;
		public static void Show(){ instance._Show(); }
		public void _Show(){
			isOn=true;
			thisObj.SetActive(isOn);
			
			UpdateView();
		}
		public static void Hide(){ instance._Hide(); }
		public void _Hide(){
			isOn=false;
			thisObj.SetActive(isOn);
		}
		
	}

}