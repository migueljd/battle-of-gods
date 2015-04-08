﻿using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;

using TBTK;

namespace TBTK{

	public class UIInteractionTooltip : MonoBehaviour {

		public Transform anchorLeft;
		public Transform anchorRight;

		public Transform actionOutcome;
		
		public Text lbMoveAPCost;
		
		public GameObject attackTooltipObj;
		private Transform attackTooltipObjT;
		private Unit tgtUnit;
		public Text lbAP;
		public Text lbStats;
		public Text lbSpecial;
		public Text lbCover;

//		private Text sourceHP;
//		private Text targetHP;
//		private Image sourceImg;
//		private Image targetImg;
//		private Text sourceName;
//		private Text targetName;
		
		
		// Use this for initialization
		void Start () {
			attackTooltipObjT=attackTooltipObj.transform;
			
			attackTooltipObj.SetActive(false);
			
			anchorLeft.position=Vector3.zero;
			anchorRight.position=Vector3.zero;



//			this.sourceName = actionOutcome.Find("SourceName").GetComponent<Text>();
//			this.targetName = actionOutcome.Find("TargetName").GetComponent<Text>();
//
//			this.sourceHP = actionOutcome.Find("SourceHPResult").GetComponent<Text>();
//			this.targetHP = actionOutcome.Find("TargetHPResult").GetComponent<Text>();
//
//			this.sourceImg = actionOutcome.Find("SourceImage").GetComponent<Image>();
//			this.targetImg = actionOutcome.Find("TargetImage").GetComponent<Image>();
//
//			actionOutcome.gameObject.SetActive(false);
		}
		
		// Update is called once per frame
		void Update () {
			if(tgtUnit!=null && attackTooltipObj.activeInHierarchy){
				//UpdateAttackTooltipPos();
			}
		}
		
		void OnEnable(){
//			GridManager.onHoverAttackableTileE += OnHoverAttackableTile;
//			GridManager.onExitAttackableTileE += OnExitAttackableTile;

			GridManager.onHostileDeselectE += OnDeselectHostileUnit;

			GridManager.onHoverWalkableTileE += OnHoverWalkableTile;
			GridManager.onExitWalkableTileE += OnExitWalkableTile;
		}
		void OnDisable(){
//			GridManager.onHoverAttackableTileE -= OnHoverAttackableTile;
//			GridManager.onExitAttackableTileE -= OnExitAttackableTile;

			GridManager.onHostileDeselectE -= OnDeselectHostileUnit;

			GridManager.onHoverWalkableTileE -= OnHoverWalkableTile;
			GridManager.onExitWalkableTileE -= OnExitWalkableTile;
		}
		
		
		void OnHoverWalkableTile(Tile tile){
			if(!GameControl.UseAPForMove()) return;
			Vector3 screenPos = Camera.main.WorldToScreenPoint(tile.GetPos());
			lbMoveAPCost.transform.localPosition=(screenPos+new Vector3(0, 0, 0))/UI.GetScaleFactor();
			lbMoveAPCost.text=(GameControl.selectedUnit.GetMoveAPCost()*tile.distance)+"AP";
		}
		void OnExitWalkableTile(){
			lbMoveAPCost.text="";
		}
		
		
		void OnHoverAttackableTile(Tile tile){
			
			Unit srcUnit=GameControl.selectedUnit;
			if(srcUnit==null){
				OnExitAttackableTile();
				return;
			}
			
			tgtUnit=tile.unit;
			
			AttackInstance attInstance=new AttackInstance(srcUnit, tgtUnit);
			attInstance.Process();
			
			
			lbAP.text=srcUnit.GetAttackAPCost()+"AP";
			
			
			float dmgMin=srcUnit.GetDamageMin()*attInstance.damageTableModifier*attInstance.flankingBonus;
			float dmgMax=srcUnit.GetDamageMax()*attInstance.damageTableModifier*attInstance.flankingBonus;
			
			string damage=dmgMin.ToString("f0")+" - "+dmgMax.ToString("f0")+"\n";
			string hitChance=(attInstance.hitChance*100).ToString("f0")+"%\n";
			string critChance=(attInstance.critChance*100).ToString("f0")+"%\n";
			lbStats.text=damage+hitChance+critChance;
			
			
			lbSpecial.text="";
			if(attInstance.stunChance>0){
				lbSpecial.text+=(attInstance.stunChance*100).ToString("f0")+"% chance to stun\n";
			}
			
			bool canCounter=tgtUnit.CanCounter(srcUnit);
			if(canCounter) lbSpecial.text+="Target can counter\n";
			
			
			lbCover.text="";
			if(GameControl.EnableCover()){
				if(attInstance.coverType==CoverSystem._CoverType.None) lbCover.text="Target not in cover\n";
				else if(attInstance.coverType==CoverSystem._CoverType.Half) lbCover.text="Target in half cover\n";
				else if(attInstance.coverType==CoverSystem._CoverType.Full) lbCover.text="Target in full cover\n";
			}
			
			if(GameControl.EnableFlanking() && attInstance.flanked) lbCover.text+="Flanking Attack";
			
			
			//UpdateAttackTooltipPos();
			
			attackTooltipObj.SetActive(true);
		}
		void OnExitAttackableTile(){
			tgtUnit=null;
			attackTooltipObj.SetActive(false);
		}

		void OnSelectHostileUnit(Tile sourceTile, Tile tile){
			this.tgtUnit = tile.unit;

//			sourceName.text = sourceTile.unit.name;
//			targetName.text = tile.unit.name;
//
//			sourceImg.sprite = sourceTile.unit.iconSprite;
//			targetImg.sprite = tile.unit.iconSprite;

			int srcDamage = AttackInstance.calculateDamage(sourceTile, tile);
			int tgtDamage = sourceTile.GetNeighbourList().Contains(tile)? AttackInstance.calculateDamage(tile, sourceTile, false, true) : 0;



			int sourceUnitHp = Mathf.RoundToInt(sourceTile.unit.HP);
			int targetUnitHp = Mathf.RoundToInt(tile.unit.HP);

//			sourceHP.text = string.Format("HP: {0}->{1}", sourceUnitHp, Mathf.Max(0, sourceUnitHp - tgtDamage));
//			targetHP.text = string.Format("HP: {0}->{1}", targetUnitHp, Mathf.Max(0,targetUnitHp - srcDamage));

			//UpdateActionOutcomePos();
			actionOutcome.gameObject.SetActive(true);
		}

		void OnDeselectHostileUnit(){
//			actionOutcome.gameObject.SetActive(false);
		}

		void UpdateActionOutcomePos(){
			Vector3 screenPos = Camera.main.WorldToScreenPoint(tgtUnit.thisT.position)/UI.GetScaleFactor();

			float posX=0;
			if(screenPos.x>(Screen.width/UI.GetScaleFactor())/2) posX=screenPos.x-230/2-60;
			else posX=screenPos.x+230/2+60;
			
			float posY=screenPos.y;
			if(screenPos.y<(Screen.height/UI.GetScaleFactor())/2) posY=screenPos.y+190;
			
			actionOutcome.localPosition=new Vector3(posX, posY, 0);
		}

		void UpdateAttackTooltipPos(){
			Vector3 screenPos = Camera.main.WorldToScreenPoint(tgtUnit.thisT.position)/UI.GetScaleFactor();
			
			float posX=0;
			if(screenPos.x>(Screen.width/UI.GetScaleFactor())/2) posX=screenPos.x-230/2-60;
			else posX=screenPos.x+230/2+60;
			
			float posY=screenPos.y;
			if(screenPos.y<(Screen.height/UI.GetScaleFactor())/2) posY=screenPos.y+190;
			
			attackTooltipObjT.localPosition=new Vector3(posX, posY, 0);
		}
		
	}

}
