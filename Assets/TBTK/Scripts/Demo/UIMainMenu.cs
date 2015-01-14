using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class UIMainMenu : MonoBehaviour {

	public Text lbNoteCampaign;
	public Text lbNoteSingle;
	
	public void OnCampaignButton(){
		Application.LoadLevel("TBTK_Menu_Campaign");
	}
	
	public void OnHoverEnterCampaignButton(){
		lbNoteCampaign.enabled=true;
	}
	public void OnHoverExitCampaignButton(){
		lbNoteCampaign.enabled=false;
	}
	
	
	
	public void OnStandAlonesButton(){
		Application.LoadLevel("TBTK_Menu_StandAlone");
	}
	
	public void OnHoverEnterStandAlonesButton(){
		lbNoteSingle.enabled=true;
	}
	public void OnHoverExitStandAlonesButton(){
		lbNoteSingle.enabled=false;
	}
	
	
}
