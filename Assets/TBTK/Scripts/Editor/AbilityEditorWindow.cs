using UnityEngine;
using UnityEditor;

using System;

using System.Collections;
using System.Collections.Generic;

using TBTK;

namespace TBTK {

	public class AbilityEditorWindow : EditorWindow {
		
		//protected static AbilityEditorWindow window;
		
		protected static string[] targetTypeLabel;
		protected static string[] targetTypeTooltip;
		
		
		protected static void InitLabel(){
			int enumLength = Enum.GetValues(typeof(_TargetType)).Length;
			targetTypeLabel=new string[enumLength];
			targetTypeTooltip=new string[enumLength];
			for(int i=0; i<enumLength; i++){
				targetTypeLabel[i]=((_TargetType)i).ToString();
				if((_TargetType)i==_TargetType.AllUnit) 		targetTypeTooltip[i]="Target All Units";
				if((_TargetType)i==_TargetType.HostileUnit) 	targetTypeTooltip[i]="Target Hostile Units";
				if((_TargetType)i==_TargetType.FriendlyUnit) targetTypeTooltip[i]="Target Friendly Units";
				if((_TargetType)i==_TargetType.AllTile) 		targetTypeTooltip[i]="Target Any tiles";
				if((_TargetType)i==_TargetType.Tile) 			targetTypeTooltip[i]="Target empty tile only";
			}
		}
		
		
		
		protected void SelectAbility(int ID){
			selectID=ID;
			GUI.FocusControl ("");
			
			if(selectID*35<scrollPos1.y) scrollPos1.y=selectID*35;
			if(selectID*35>scrollPos1.y+listVisibleRect.height-40) scrollPos1.y=selectID*35-listVisibleRect.height+40;
		}
		
		
		protected Vector2 scrollPos1;
		protected Vector2 scrollPos2;
		
		protected static int selectID=0;
		protected float contentHeight=0;
		protected float contentWidth=0;
		
		protected GUIContent cont;
		protected GUIContent[] contL;
		
		protected static float spaceX=110;
		protected static float spaceY=20;
		protected static float width=150;
		protected static float height=18;
		
		
		protected Rect listVisibleRect;
		protected Rect listContentRect;
		
		protected int deleteID=-1;
		protected bool minimiseList=false;
		
		
		
		
		
		
		
		protected Vector2 DrawEffectStats(float startX, float startY, Effect effect){
			
			float spaceX=100;
			
			cont=new GUIContent("HPMin:", "HitPoint (HP) value to be applied to the target HP. Damage when value is negative, heal when value is positive. Doesnt apply to tile");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			effect.HPMin=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), effect.HPMin);
			
			cont=new GUIContent("HPMax:", "HitPoint (HP) value to be applied to the target HP. Damage when value is negative, heal when value is positive. Doesnt apply to tile");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			effect.HPMax=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), effect.HPMax);
			
			startY+=8;
			
			cont=new GUIContent("APMin:", "ActionPoint (AP) value to be applied to the target AP. Damage when value is negative, restore when value is positive. Doesnt apply to tile");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			effect.APMin=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), effect.APMin);
			
			cont=new GUIContent("APMax:", "ActionPoint (AP) value to be applied to the target AP. Damage when value is negative, restore when value is positive. Doesnt apply to tile");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			effect.APMax=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), effect.APMax);
			
			
			cont=new GUIContent("Stun:", "Duration of stun (in turn) to be applied to the target. Doesnt apply to tile");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			effect.stun=EditorGUI.IntField(new Rect(startX+spaceX, startY, 40, height), effect.stun);
			
			startY+=4;
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY/2, width-10, height), "______________________________");
			startY+=4;
			
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY/2, width-10, height), "*Buff/Debuff to target");		startY+=4;
			
			cont=new GUIContent("HPBuff:", "HitPoint (HP) multiplier to be applied to the target. ie, 0.2 being increment by 20%, -0.1 being reduction by 10%");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			effect.unitStat.HPBuff=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), effect.unitStat.HPBuff);
			
			cont=new GUIContent("APBuff:", "ActionPoint (AP) multiplier to be applied to the target. ie, 0.2 being increment by 20%, -0.1 being reduction by 10%");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			effect.unitStat.APBuff=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), effect.unitStat.APBuff);
			
			
			cont=new GUIContent("MoveAPCost:", "Move AP cost modifier to be applied to the target.");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			effect.unitStat.moveAPCost=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), effect.unitStat.moveAPCost);
			
			cont=new GUIContent("AttackAPCost:", "Attack AP cost modifier to be applied to the target.");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			effect.unitStat.attackAPCost=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), effect.unitStat.attackAPCost);
			
			startY+=8;

			cont=new GUIContent("Turn Priority:", "Turn Priority modifier to be applied to the target");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			effect.unitStat.turnPriority=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), effect.unitStat.turnPriority);
			
			cont=new GUIContent("MoveRange:", "Movement range modifier (in tile) to be applied to the target");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			effect.unitStat.moveRange=EditorGUI.IntField(new Rect(startX+spaceX, startY, 40, height), effect.unitStat.moveRange);
			
			cont=new GUIContent("AttackRange:", "Attack range modifier (in tile) to be applied to the target");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			effect.unitStat.attackRange=EditorGUI.IntField(new Rect(startX+spaceX, startY, 40, height), effect.unitStat.attackRange);
			
			cont=new GUIContent("Sight:", "modifier to be applied to the target. ");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			effect.unitStat.sight=EditorGUI.IntField(new Rect(startX+spaceX, startY, 40, height), effect.unitStat.sight);
			
			startY+=8;
			
			cont=new GUIContent("AttackPerTurn:", "Attack-per-turn modifier to be applied to the target. Doesnt apply to tile");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			effect.unitStat.attackPerTurn=EditorGUI.IntField(new Rect(startX+spaceX, startY, 40, height), effect.unitStat.attackPerTurn);
			
			cont=new GUIContent("MovePerTurn:", "Move-per-turn modifier to be applied to the target. Doesnt apply to tile");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			effect.unitStat.movePerTurn=EditorGUI.IntField(new Rect(startX+spaceX, startY, 40, height), effect.unitStat.movePerTurn);
			
			cont=new GUIContent("CounterPerTurn:", "CounterAttack-per-turn modifier to be applied to the target. Doesnt apply to tile");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			effect.unitStat.counterPerTurn=EditorGUI.IntField(new Rect(startX+spaceX, startY, 40, height), effect.unitStat.counterPerTurn);
			
			
			startY+=8;
			
			cont=new GUIContent("Damage:", "Damage multiplier to be applied to the target's both min and max damage. ie, 0.2 being increment by 20%, -0.1 being reduction by 10%");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			effect.unitStat.damage=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), effect.unitStat.damage);
			
			//cont=new GUIContent("DamageMax:", "Multiplier to be applied to the target. ie, 0.2 being increment by 20%, -0.1 being reduction by 10%");
			//EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			//effect.unitStat.damageMax=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), effect.unitStat.damageMax);
			
			
			cont=new GUIContent("HitChance:", "Hit Chance multiplier to be applied to the target. ie, 0.2 being increment by 20%, -0.1 being reduction by 10%");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			effect.unitStat.hitChance=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), effect.unitStat.hitChance);
			
			cont=new GUIContent("DodgeChance:", "Multiplier to be applied to the target. ie, 0.2 being increment by 20%, -0.1 being reduction by 10%");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			effect.unitStat.dodgeChance=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), effect.unitStat.dodgeChance);
			
			
			startY+=8;
			
			cont=new GUIContent("CritChance:", "Multiplier to be applied to the target. ie, 0.2 being increment by 20%, -0.1 being reduction by 10%");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			effect.unitStat.critChance=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), effect.unitStat.critChance);
			
			cont=new GUIContent("CritAvoidance:", "Multiplier to be applied to the target. ie, 0.2 being increment by 20%, -0.1 being reduction by 10%");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			effect.unitStat.critAvoidance=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), effect.unitStat.critAvoidance);
			
			cont=new GUIContent("CritMultiplier:", "Critical damage multiplier to be applied to the target. ie, 0.2 being increment by 20%, -0.1 being reduction by 10%");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			effect.unitStat.critMultiplier=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), effect.unitStat.critMultiplier);
			
			
			startY+=8;
			
			cont=new GUIContent("StunChance:", "Multiplier to be applied to the target. ie, 0.2 being increment by 20%, -0.1 being reduction by 10%");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			effect.unitStat.stunChance=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), effect.unitStat.stunChance);
			
			cont=new GUIContent("StunAvoidance:", "Multiplier to be applied to the target. ie, 0.2 being increment by 20%, -0.1 being reduction by 10%");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			effect.unitStat.stunAvoidance=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), effect.unitStat.stunAvoidance);
			
			cont=new GUIContent("StunDuration:", "Multiplier to be applied to the target. ie, 0.2 being increment by 20%, -0.1 being reduction by 10%");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			effect.unitStat.stunDuration=EditorGUI.IntField(new Rect(startX+spaceX, startY, 40, height), effect.unitStat.stunDuration);
			
			startY+=8;
			
			cont=new GUIContent("SilentChance:", "Multiplier to be applied to the target. ie, 0.2 being increment by 20%, -0.1 being reduction by 10%");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			effect.unitStat.silentChance=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), effect.unitStat.silentChance);
			
			cont=new GUIContent("SilentAvoidance:", "Multiplier to be applied to the target. ie, 0.2 being increment by 20%, -0.1 being reduction by 10%");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			effect.unitStat.silentAvoidance=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), effect.unitStat.silentAvoidance);
			
			cont=new GUIContent("SilentDuration:", "Multiplier to be applied to the target. ie, 0.2 being increment by 20%, -0.1 being reduction by 10%");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			effect.unitStat.silentDuration=EditorGUI.IntField(new Rect(startX+spaceX, startY, 40, height), effect.unitStat.silentDuration);
			
			startY+=8;
			
			cont=new GUIContent("Flanking Bonus:", "Damage multiplier to be applied to the unit when flanking a target. Takes value from 0 and above with 0.2 being increase the damage by 20%");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			effect.unitStat.flankingBonus=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), effect.unitStat.flankingBonus);
			cont=new GUIContent("Flanked Modifier:", "Damage multiplier to be applied to the unit when being flanked. Takes value from 0 and above with 0.2 being reduce the incoming damage by 20%");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			effect.unitStat.flankedModifier=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), effect.unitStat.flankedModifier);
			
			startY+=8;
			
			cont=new GUIContent("HPPerTurn:", "Multiplier to be applied to the target. ie, 0.2 being increment by 20%, -0.1 being reduction by 10%");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			effect.unitStat.HPPerTurn=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), effect.unitStat.HPPerTurn);
			
			cont=new GUIContent("APPerTurn:", "Multiplier to be applied to the target. ie, 0.2 being increment by 20%, -0.1 being reduction by 10%");
			EditorGUI.LabelField(new Rect(startX, startY+=spaceY, width, height), cont);
			effect.unitStat.APPerTurn=EditorGUI.FloatField(new Rect(startX+spaceX, startY, 40, height), effect.unitStat.APPerTurn);
			
			
			return new Vector2(startX, startY);
		}
		
		
		
	}

}
