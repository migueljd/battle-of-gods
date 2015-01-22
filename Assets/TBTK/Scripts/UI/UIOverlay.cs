using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;

using TBTK;

namespace TBTK {
	
	public class UIOverlay : MonoBehaviour {
		
		[HideInInspector] public Camera mainCam;
		public void OnRefreshMainCamera(){ mainCam=Camera.main; }
		
		public List<UnitOverlay> unitOverlayList=new List<UnitOverlay>();
		public List<Text> textOverlayList=new List<Text>();
		
		
		
		public static UIOverlay instance;
		
		void Awake(){
			instance=this;
			
			for(int i=0; i<10; i++){
				if(i>0){
					GameObject obj=(GameObject)Instantiate(textOverlayList[0].gameObject);
					textOverlayList.Add(obj.GetComponent<Text>());
					textOverlayList[i].transform.SetParent(textOverlayList[0].transform.parent);
				}
				textOverlayList[i].text="";
				textOverlayList[i].gameObject.SetActive(false);
			}
			
			for(int i=0; i<20; i++){
				if(i==0) unitOverlayList[i].Init();
				else unitOverlayList.Add(unitOverlayList[0].Clone());
				unitOverlayList[i].rootObj.SetActive(false);
			}
			
			mainCam=Camera.main;
		}
		
		
		void OnEnable(){
			GameControl.onGameStartE += OnGameStart;
			GameControl.onGameOverE += OnGameOver;
			
			FactionManager.onInsertUnitE += OnAddUnit;
			
			TextOverlay.onTextOverlayE += OnTextOverlay;
		}
		void OnDisable(){
			GameControl.onGameStartE -= OnGameStart;
			GameControl.onGameOverE -= OnGameOver;
			
			FactionManager.onInsertUnitE -= OnAddUnit;
			
			TextOverlay.onTextOverlayE -= OnTextOverlay;
		}
		
		void OnGameStart(){
			int totalUnitCount=FactionManager.GetTotalUnitCount();
			if(unitOverlayList.Count<totalUnitCount){
				while(unitOverlayList.Count<totalUnitCount){
					unitOverlayList.Add(unitOverlayList[0].Clone());
					unitOverlayList[unitOverlayList.Count-1].rootObj.SetActive(false);
				}
			}
			
			List<Unit> unitList=FactionManager.GetAllUnit();
			List<Faction> factionList=FactionManager.GetFactionList();
			for(int i=0; i<unitList.Count; i++){
				unitOverlayList[i].unit=unitList[i];
				
				for(int n=0; n<factionList.Count; n++){
					if(unitList[i].factionID==factionList[n].ID){
						unitOverlayList[i].icon.color=factionList[n].color;
					}
				}
			}
			
			for(int i=0; i<unitOverlayList.Count; i++){
				//if(unitOverlayList[i].unit==null) 
					unitOverlayList[i].rootObj.SetActive(unitOverlayList[i].unit!=null);
			}
		}
		
		void OnAddUnit(Unit unit){ 
			int totalUnitCount=FactionManager.GetTotalUnitCount();
			if(unitOverlayList.Count<totalUnitCount){
				while(unitOverlayList.Count<totalUnitCount){
					unitOverlayList.Add(unitOverlayList[0].Clone());
					unitOverlayList[unitOverlayList.Count-1].rootObj.SetActive(false);
				}
			}
			
			for(int i=0; i<unitOverlayList.Count; i++){
				if(unitOverlayList[i].unit==null){
					Faction fac=FactionManager.GetFaction(unit.factionID);
					unitOverlayList[i].icon.color=fac.color;
					unitOverlayList[i].unit=unit;
					break;
				}
			}
		}
		
		
		void OnGameOver(int factionID){
			for(int i=0; i<unitOverlayList.Count; i++){
				if(unitOverlayList[i].rootObj.activeInHierarchy) unitOverlayList[i].rootObj.SetActive(false);
			}
		}
		
		void OnTextOverlay(TextOverlay overlayInstance){
			//if(UI.DisableTextOverlay()) return;
			
			Text txt=GetUnusedTextOverlay();
			
			txt.text=overlayInstance.msg;
			if(overlayInstance.useColor) txt.color=overlayInstance.color;
			else txt.color=new Color(1f, 150/255f, 0, 1f);
			
			Vector3 screenPos = mainCam.WorldToScreenPoint(overlayInstance.pos);
			txt.transform.localPosition=screenPos/UI.GetScaleFactor();
			
			txt.gameObject.SetActive(true);
			
			StartCoroutine(TextOverlayRoutine(txt));
		}
		IEnumerator TextOverlayRoutine(Text txt){
			Transform txtT=txt.transform;
			float duration=0;
			while(duration<1){
				txtT.localPosition+=new Vector3(0, 30*Time.deltaTime, 0);
				Color color=txt.color;
				color.a=1-duration;
				//~ if(duration>0.5f) color.a=1-duration*2;
				txt.color=color;
				
				duration+=Time.deltaTime*1.5f;
				yield return null;
			}
			//txt.text="";
			txt.gameObject.SetActive(false);
		}
		
	
		
		
		void Update(){
			if(GameControl.GetGamePhase()==_GamePhase.Play){
				for(int i=0; i<unitOverlayList.Count; i++){
					UnitOverlay overlay=unitOverlayList[i];
					if(overlay.unit==null){
						if(overlay.rootObj.activeInHierarchy) overlay.rootObj.SetActive(false);
						continue;
					}
					
					Vector3 screenPos = mainCam.WorldToScreenPoint(overlay.unit.thisT.position+new Vector3(0, 0, 0));
					overlay.rootT.localPosition=(screenPos+new Vector3(0, -20, 0))/UI.GetScaleFactor();
					
					overlay.barHP.value=overlay.unit.GetHPRatio();
					overlay.barAP.value=overlay.unit.GetAPRatio();
					
					//overlay.lbText.text=overlay.unit.GetEffectList().Count.ToString();
					//overlay.lbTextShadow.text=overlay.unit.GetEffectList().Count.ToString();
					
					overlay.lbText.text=overlay.unit.factionID.ToString();
					overlay.lbTextShadow.text=overlay.unit.factionID.ToString();
					
					if(overlay.unit.thisObj.layer==LayerManager.GetLayerUnitInvisible()){
						if(overlay.rootObj.activeInHierarchy) overlay.rootObj.SetActive(false);
					}
					else{
						if(!overlay.rootObj.activeInHierarchy) overlay.rootObj.SetActive(true);
					}
				}
			}
		}
		
		
		
		public void OnOverlayButton(GameObject overlayObj){
			//Debug.Log("pressed");
		}
		public void OnHoverOverlayButton(GameObject overlayObj){
			for(int i=0; i<unitOverlayList.Count; i++){
				if(unitOverlayList[i].rootObj==overlayObj){
					UIUnitInfoTooltip.Show(unitOverlayList[i].unit);
					break;
				}
			}
		}
		public void OnHoverExitOverlayButton(){
			UIUnitInfoTooltip.Hide();
		}
		
		
		
		
		//~ UnitOverlay GetUnusedOverlay(){
			//~ for(int i=0; i<overlayList.Count; i++){
				//~ if(!overlayList[i].rootObj.activeInHierarchy) return overlayList[i];
			//~ }
			//~ UnitOverlay overlay=overlayList[0].Clone();
			//~ overlayList.Add(overlay);
			//~ return overlay;
		//~ }
		
		Text GetUnusedTextOverlay(){
			for(int i=0; i<textOverlayList.Count; i++){
				if(textOverlayList[i].text=="") return textOverlayList[i];
			}
			
			GameObject obj=(GameObject)Instantiate(textOverlayList[0].gameObject);
			obj.transform.SetParent(textOverlayList[0].transform.parent);
			obj.transform.localScale=textOverlayList[0].transform.localScale;
			Text txt=obj.GetComponent<Text>();
			textOverlayList.Add(txt);
			return txt;
		}

		
		
		
	}

}