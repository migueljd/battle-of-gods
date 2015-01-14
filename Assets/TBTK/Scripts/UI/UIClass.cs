using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;


namespace TBTK {

	[System.Serializable]
	public class UIObject{
		public GameObject rootObj;
		public Transform rootT;
	}


	[System.Serializable]
	public class UIItem{
		public Text label;
		public Image image;
	}

	[System.Serializable]
	public class UnityButton : UIObject{
		public Button button;
		public Text label;
		public Image imageBG;
		public Image imageIcon;
		
		
		public void Init(){
			rootT=rootObj.transform;
			
			button=rootObj.GetComponent<Button>();
			imageBG=rootObj.GetComponent<Image>();
			
			foreach(Transform child in rootT){
				if(child.name=="Image"){
					imageIcon=child.GetComponent<Image>();
				}
				else if(child.name=="Text"){
					label=child.GetComponent<Text>();
				}
			}
		}
		
		public UnityButton Clone(string name, Vector3 posOffset){
			UnityButton newBut=new UnityButton();
			newBut.rootObj=(GameObject)MonoBehaviour.Instantiate(rootObj);
			newBut.rootObj.name=name;//=="" ? srcObj.name+"(Clone)" : name;
			newBut.Init();
			
			newBut.rootT.SetParent(rootT.parent);
			newBut.rootT.localPosition=rootT.localPosition+posOffset;
			newBut.rootT.localScale=new Vector3(1, 1, 1);
			
			return newBut;
		}
	}



	[System.Serializable]
	public class UnitOverlay : UIObject{
		public Unit unit;
		
		public Image icon;
		public Text lbText;
		public Text lbTextShadow;
		public Slider barHP;
		public Slider barAP;
		
		public Image iconCover;
		
		public void Init(){
			rootT=rootObj.transform;
			
			foreach(Transform child in rootT){
				if(child.name=="APBar"){
					barAP=child.GetComponent<Slider>();
				}
				else if(child.name=="HPBar"){
					barHP=child.GetComponent<Slider>();
				}
				else if(child.name=="Icon"){
					icon=child.GetComponent<Image>();
					
					foreach(Transform subChild in child){
						if(subChild.name=="Text"){
							lbText=subChild.GetComponent<Text>();
						}
						else if(subChild.name=="TextShadow"){
							lbTextShadow=subChild.GetComponent<Text>();
						}
					}
				}
				else if(child.name=="IconCover"){
					iconCover=child.GetComponent<Image>();
				}
			}
			
			if(iconCover!=null) iconCover.enabled=false;
			if(lbText!=null) lbText.text="";
			if(lbTextShadow!=null) lbTextShadow.text="";
		}
		
		public UnitOverlay Clone(string name=""){
			UnitOverlay newOverlay=new UnitOverlay();
			newOverlay.rootObj=(GameObject)MonoBehaviour.Instantiate(rootObj);
			newOverlay.rootObj.name=name=="" ? rootObj.name+"(Clone)" : name;
			newOverlay.Init();
			
			newOverlay.rootT.SetParent(rootT.parent);
			newOverlay.rootT.localScale=rootT.localScale;
			
			return newOverlay;
		}
	}


	
	
	public class TextOverlay{
		public delegate void TextOverlayHandler(TextOverlay textO); 
		public static event TextOverlayHandler onTextOverlayE;
	
		public Vector3 pos;
		public string msg;
		public Color color;
		public bool useColor=false;
		
		public TextOverlay(Vector3 p, string m){
			float rand=.25f;
			Vector3 posR=new Vector3(Random.Range(-rand, rand), Random.Range(-rand, rand), Random.Range(-rand, rand));
			pos=p+posR;
			msg=m;
			if(onTextOverlayE!=null) onTextOverlayE(this);
		}
		public TextOverlay(Vector3 p, string m, Color col){
			float rand=.25f;
			Vector3 posR=new Vector3(Random.Range(-rand, rand), Random.Range(-rand, rand), Random.Range(-rand, rand));
			pos=p+posR;
			msg=m;
			color=col;
			useColor=true;
			
			if(onTextOverlayE!=null) onTextOverlayE(this);
			else Debug.Log("empty");
		}
	}
	

}