using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;

using TBTK;

namespace TBTK{

	public class UISceneSelect : MonoBehaviour {
		
		public Campaign campaign;
		public UICampaign uiCampaign;
		
		public bool loadStandAloneLevels=false;
		
		private List<SceneInfo> sceneInfoList=new List<SceneInfo>();
		public List<Button> buttonList=new List<Button>();
		
		public Text lbDesp;
		
		private static int selectedID=0;
		
		void Awake(){
			string text=!loadStandAloneLevels ? "_Campaign_" : "_StandAlone_";
			string despText=" The enemies are randomly generated";
			
			SceneInfo sInfo=new SceneInfo();
			sInfo.name="TBTK"+text+"Simple";
			sInfo.desp="A simple scene with no special mechanism. Inspired by table-top style turn-based game such Heroes of Might & Magic and King's Bounty."+despText;
			sceneInfoList.Add(sInfo);
			
			sInfo=new SceneInfo();
			sInfo.name="TBTK"+text+"FogOfWar";
			sInfo.desp="A rather simple grid with Fog-of-war and flanking thrown into the mix."+despText;
			sceneInfoList.Add(sInfo);
			
			sInfo=new SceneInfo();
			sInfo.name="TBTK"+text+"CoverSystem";
			sInfo.desp="A more complex grid with X-Com style cover system enabled."+despText;
			sceneInfoList.Add(sInfo);
			
			sInfo=new SceneInfo();
			sInfo.name="TBTK"+text+"JRPG";
			sInfo.desp="A typical J-RPG turn-based setting. This scene use a fixed starting unit lineup for the player and certain abilities has been disabled."+despText;
			sceneInfoList.Add(sInfo);
			
			SetToSelected(buttonList[selectedID]);
			lbDesp.text=sceneInfoList[selectedID].desp;
		}
		
		public void OnSceneButton(GameObject buttonObj){
			int buttonID=0;
			for(int i=0; i<buttonList.Count; i++){
				if(buttonList[i].gameObject==buttonObj){
					buttonID=i;
					break;
				}
			}
			
			if(buttonID>=sceneInfoList.Count) return;
			
			SetToNormal(buttonList[selectedID]);
			
			selectedID=buttonID;
			
			SetToSelected(buttonList[selectedID]);
			
			lbDesp.text=sceneInfoList[selectedID].desp;
		}
		
		public void OnPlayButton(){
			if(loadStandAloneLevels) Application.LoadLevel(sceneInfoList[selectedID].name);
			else campaign.OnPlayButton(sceneInfoList[selectedID].name);
		}
		
		public void OnBackButton(){
			uiCampaign.BackFromLevel();
		}
		
		
		
		
		
		private Color colorSelected=new Color(1f, 150f/255f, 0f, 1f);
		private Color colorNormal=Color.white;
		public void SetToSelected(Button button){
			ColorBlock colors=button.colors;
			colors.normalColor = colorSelected;
			button.colors=colors;
			
			button.transform.localScale=new Vector3(1.05f, 1.05f, 1.05f);
		}
		public void SetToNormal(Button button){
			ColorBlock colors=button.colors;
			colors.normalColor = colorNormal;
			button.colors=colors;
			
			button.transform.localScale=new Vector3(1, 1, 1);
		}
		
		
	}
	
	public class SceneInfo{
		public string name;
		public string desp;
	}

}