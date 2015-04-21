using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using TBTK;

using Cards;

namespace TBTK{
	
	public class Unit : MonoBehaviour {


		
		public delegate void UnitDestroyedHandler(Unit unit);
		public static event UnitDestroyedHandler onUnitDestroyedE;		//fire when the unit is destroyed
		
		public delegate void UnitSelectedHandler(Unit unit);
		public static event UnitSelectedHandler onUnitSelectedE;			//listen by UI only
		
		//public delegate void UnitMoveDepletedHandler(Unit unit);
		//public static event UnitMoveDepletedHandler onMoveDepletedE;		//fire when the unit depleted all available action
		
		
		public int prefabID;
		public int instanceID;	
		public int factionID;
		
		private int dataID=-1;//use to identiy unit in LoadData 
		public int GetDataID(){ return dataID; }
		
		public string unitName="Unit";
		public string desp="";
		public Sprite iconSprite;
		
		[HideInInspector] public bool trigger=false;
		[HideInInspector] public bool isAIUnit=false;
		[HideInInspector] public Unit lastAttacker;
		[HideInInspector] public Unit lastTarget;
		
		public int damageType=0;
		public int armorType=0;
		
		public Tile tile;	//occupied tile
		
		public float hitThreshold=0.25f;
		public Transform targetPoint;
		public Transform GetTargetT(){ return targetPoint==null ? this.transform : targetPoint; }
		
		public bool requireDirectLOSToAttack=true;
		
		public int value=5;
		
		public float moveSpeed=10;
		[HideInInspector] private float rotateSpeed=7;
		
		public DurationCounter stunCounter=new DurationCounter();
		public int stunned=0;			//if >0, unit is stunned
		private int disableAbilities=0;	//if >0, unit cannot used ability
		public bool IsStunned(){ return stunned>0 ? true : false; }
		public bool DisableAbilities(){ return disableAbilities>0 ? true : false; }
		
		public DurationCounter silentCounter=new DurationCounter();
		public int silenced=0;
		public bool IsSilenced(){ return silenced>0 ? true : false; }
		
		
		public float defaultHP=10;
		public float defaultAP=10;
		public float HP=10;
		public float guard = 0;
		public float AP=10;
		
		public float moveAPCost=0;
		public float attackAPCost=0;
		
		public float turnPriority=1;
		
		public int moveRange=3;
		public int attackRange=3;
		public int sight=6;
		
		public int movePerTurn=1;
		public int attackPerTurn=1;
		public int counterPerTurn=1;	//counter attack
		[HideInInspector] public int moveRemain=1;
		//[HideInInspector] 
		public int attackRemain=1;
		[HideInInspector] public int counterRemain=1;
		
		public float damageMin=3;
		public float damageMax=6;

		public int HPDamage = 1;
		
		public float hitChance=.8f;
		public float dodgeChance=.15f;
		
		public float critChance=0.1f;
		public float critAvoidance=0;
		public float critMultiplier=2;
		
		public float stunChance=0;
		public float stunAvoidance=0;
		public int stunDuration=1;
		
		public float silentChance=0;
		public float silentAvoidance=0;
		public int silentDuration=1;
		
		public float flankingBonus=0;
		public float flankedModifier=0;
		
		public float HPPerTurn=0;
		public float APPerTurn=0;

		public bool RangedAttackOnly = false;
		
		public bool isStatic = false;

		public Unit target;

		public static int HerculesHP;
		public static int AtalantaHP;
		public static int AchillesHP;

		public bool usedThisTurn = false;
		
		//********************************************************************************************************************************
		//these section are functions that get active stats of unit
		
		public float GetFullHP(){ return defaultHP*(1+GetEffHPBuff()+PerkManager.GetUnitHPBuff(prefabID)); }
		public float GetFullAP(){ return defaultAP*(1+GetEffAPBuff()+PerkManager.GetUnitAPBuff(prefabID)); }

		public float GetEffectiveGuard(){	return this.guard + (getStack() != null?getStack ().getGuard () : 0);}
		
		public float GetMoveAPCost(){ return (GameControl.UseAPForMove()) ? Mathf.Max(0, moveAPCost+GetEffMoveAPCost()+PerkManager.GetUnitMoveAPCost(prefabID)) : 0 ; }
		public float GetAttackAPCost(){ return (GameControl.UseAPForAttack()) ? Mathf.Max(0, attackAPCost+GetEffAttackAPCost()+PerkManager.GetUnitAttackAPCost(prefabID)) : 0 ; }
		public float GetCounterAPCost(){ return GetAttackAPCost()*GameControl.GetCounterAPMultiplier(); }
		
		public float GetTurnPriority(){ return turnPriority+GetEffTurnPriority()+PerkManager.GetUnitTurnPriority(prefabID); }
		
		public int GetMoveRange(){ return moveRange+GetEffMoveRange()+PerkManager.GetUnitMoveRange(prefabID); }
		public int GetAttackRange(){ return attackRange+GetEffAttackRange()+tile.GetAttackRange()+PerkManager.GetUnitAttackRange(prefabID); }
		public int GetSight(){ return sight+GetEffSight()+tile.GetSight()+PerkManager.GetUnitSight(prefabID); }
		
		public int GetMovePerTurn(){ return movePerTurn+GetEffMovePerTurn()+PerkManager.GetUnitMovePerTurn(prefabID); }
		public int GetAttackPerTurn(){ return attackPerTurn+GetEffAttackPerTurn()+PerkManager.GetUnitAttackPerTurn(prefabID); }
		public int GetCounterPerTurn(){ return counterPerTurn+GetEffCounterPerTurn()+PerkManager.GetUnitCounterPerTurn(prefabID); }
		
		public float GetDamageMin(){ return damageMin*(1+GetEffDamage()+tile.GetDamage()+PerkManager.GetUnitDamage(prefabID)); }
		public float GetEffectiveDamage(){ return this.GetDamageMin () + (this.getStack() != null? this.getStack ().getDamage () : 0); }
		public float GetDamageMax(){ return damageMax*(1+GetEffDamage()+tile.GetDamage()+PerkManager.GetUnitDamage(prefabID)); }
		
		public float GetHitChance(){ return hitChance+GetEffHitChance()+tile.GetHitChance()+PerkManager.GetUnitHitChance(prefabID); }
		public float GetDodgeChance(){ return dodgeChance+GetEffDodgeChance()+tile.GetDodgeChance()+PerkManager.GetUnitDodgeChance(prefabID); }
		
		public float GetCritChance(){ return critChance+GetEffCritChance()+tile.GetCritChance()+PerkManager.GetUnitCritChance(prefabID); }
		public float GetCritAvoidance(){ return critAvoidance+GetEffCritAvoidance()+tile.GetCritAvoidance()+PerkManager.GetUnitCritChance(prefabID); }
		public float GetCritMultiplier(){ return critMultiplier+GetEffCritMultiplier()+tile.GetCritMultiplier()+PerkManager.GetUnitCritChance(prefabID); }
		
		public float GetStunChance(){ return stunChance+GetEffStunChance()+tile.GetStunChance()+PerkManager.GetUnitStunChance(prefabID); }
		public float GetStunAvoidance(){ return stunAvoidance+GetEffStunAvoidance()+tile.GetStunAvoidance()+PerkManager.GetUnitStunAvoidance(prefabID); }
		public int GetStunDuration(){ return stunDuration+GetEffStunDuration()+tile.GetStunDuration()+PerkManager.GetUnitStunDuration(prefabID); }
		
		public float GetSilentChance(){ return silentChance+GetEffSilentChance()+tile.GetSilentChance()+PerkManager.GetUnitSilentChance(prefabID); }
		public float GetSilentAvoidance(){ return silentAvoidance+GetEffSilentAvoidance()+tile.GetSilentAvoidance()+PerkManager.GetUnitSilentAvoidance(prefabID); }
		public int GetSilentDuration(){ return silentDuration+GetEffSilentDuration()+tile.GetSilentDuration()+PerkManager.GetUnitSilentDuration(prefabID); }
		
		public float GetFlankingBonus(){ return flankingBonus+GetEffFlankingBonus()+PerkManager.GetUnitFlankingBonus(prefabID); }
		public float GetFlankedModifier(){ return flankedModifier+GetEffFlankedModifier()+PerkManager.GetUnitFlankedModifier(prefabID); }
		
		public float GetHPPerTurn(){ return HPPerTurn*(1+GetEffHPPerTurn()+tile.GetHPPerTurn()+PerkManager.GetUnitHPPerTurn(prefabID)); }
		public float GetAPPerTurn(){ return APPerTurn*(1+GetEffAPPerTurn()+tile.GetAPPerTurn()+PerkManager.GetUnitAPPerTurn(prefabID)); }
		
		
		
		public float GetHPRatio(){ return HP/GetFullHP(); }
		public float GetAPRatio(){ return AP/GetFullAP(); }
		
		public int GetEffectiveMoveRange(){ 
			if(movePerTurn==0) return 0;
			float apCost=GetMoveAPCost();

			int apAllowance = 0;

//			if (FactionManager.IsPlayerTurn ()) {
//				AP = CardsStackManager.getMovement ();
//				apAllowance = (int)Mathf.Abs (AP / apCost);
//			} else {
			apAllowance = apCost == 0 ? 999999 : (int)Mathf.Abs (AP / apCost);
//			}
			return apAllowance;
			//return Mathf.Min(GetMoveRange(), apAllowance); 
		}
		
		public bool CanAttack(){ 
			bool apFlag=GameControl.UseAPForAttack() ? AP>GetAttackAPCost() : true ;
			return attackRemain>0 & GetAttackRange()>0 & stunned<=0 &  apFlag; 
		}
		public bool CanMove(){ return moveRemain>0 & GetEffectiveMoveRange()>0 & stunned<=0; }
		public bool CanUseAbilities(){ return disableAbilities<0 & stunned<=0 & silenced<=0; }
		
		//end get stats section
		//********************************************************************************************************************************
		
		
		
		//********************************************************************************************************************************
		//these section are related to ability effects
		public List<Effect> effectList=new List<Effect>();
		public List<Effect> GetEffectList(){ return effectList; }
		public void ApplyEffect(Effect eff){ 
			//new TextOverlay(GetTargetT().position, eff.name, Color.white);
			
			if(eff.stun>0 || eff.duration>=0) AddUnitToEffectTracker();
			
			if(eff.duration>=0){
				effectList.Add(eff);
				eff.StartDurationCount();
			}
			
			float HPVal=Random.Range(eff.HPMin, eff.HPMax);
			if(HPVal>0) RestoreHP(HPVal);
			else if(HPVal<0) ApplyDamage(-HPVal);
			AP=Mathf.Clamp(AP+Mathf.Min(eff.APMin, eff.APMax), 0, GetFullAP());
			
			if(eff.stun>0 && stunned<eff.stun){
				stunned=eff.stun;
				stunCounter.Count(eff.stun);
				VisualEffectManager.UnitStunned(this, stunned);
			}
			
			if(eff.unitStat.sight!=0 && !isAIUnit) SetupFogOfWar();
			
			if(RequireReselect(eff)){
				moveRemain+=eff.unitStat.movePerTurn;
				attackRemain+=eff.unitStat.attackPerTurn;
				if(GameControl.selectedUnit==this) GameControl.SelectUnit(tile);
			}
		}
		public bool ProcessEffectList(){
			if(stunned>0){
				stunCounter.Iterate();
				stunned=(int)stunCounter.duration;
			}
			
			if(silenced>0){
				silentCounter.Iterate();
				silenced=(int)silentCounter.duration;
			}
			
			bool turnPriorityChanged=false;
			
			for(int i=0; i<effectList.Count; i++){
				effectList[i].IterateDuration();
				
				if(effectList[i].duration<=0){
					if(effectList[i].unitStat.turnPriority!=0) turnPriorityChanged=true;
					effectList.RemoveAt(i);
					i-=1;
				}
			}
			
			if(stunned==0 && silenced==0 && effectList.Count==0) EffectTracker.RemoveUnitWithEffect(this);
			
			return turnPriorityChanged;
		}
		
		public bool RequireReselect(Effect eff){
			if(eff.unitStat.movePerTurn!=0) return true;
			if(eff.unitStat.attackPerTurn!=0) return true;
			if(eff.unitStat.moveRange!=0) return true;
			if(eff.unitStat.attackRange!=0) return true;
			if(eff.unitStat.sight!=0) return true;
			return false;
		}
		
		private void AddUnitToEffectTracker(){
			if(stunned<=0 || silenced<=0 || effectList.Count==0){
				EffectTracker.AddUnitWithEffect(this);
			}
		}
		
		
		float GetEffHPBuff(){
			float value=0;
			for(int i=0; i<effectList.Count; i++) value+=effectList[i].unitStat.HPBuff;
			return value;
		}
		float GetEffAPBuff(){
			float value=0;
			for(int i=0; i<effectList.Count; i++) value+=effectList[i].unitStat.APBuff;
			return value;
		}
		
		float GetEffMoveAPCost(){
			float value=0;
			for(int i=0; i<effectList.Count; i++) value+=effectList[i].unitStat.moveAPCost;
			return value;
		}
		float GetEffAttackAPCost(){
			float value=0;
			for(int i=0; i<effectList.Count; i++) value+=effectList[i].unitStat.attackAPCost;
			return value;
		}
		
		float GetEffTurnPriority(){
			float value=0;
			for(int i=0; i<effectList.Count; i++) value+=effectList[i].unitStat.turnPriority;
			return value;
		}
		
		int GetEffMoveRange(){
			int value=0;
			for(int i=0; i<effectList.Count; i++) value+=effectList[i].unitStat.moveRange;
			return value;
		}
		int GetEffAttackRange(){
			int value=0;
			for(int i=0; i<effectList.Count; i++) value+=effectList[i].unitStat.attackRange;
			return value;
		}
		int GetEffSight(){
			int value=0;
			for(int i=0; i<effectList.Count; i++) value+=effectList[i].unitStat.sight;
			return value;
		}
		
		int GetEffAttackPerTurn(){
			int value=0;
			for(int i=0; i<effectList.Count; i++) value+=effectList[i].unitStat.attackPerTurn;
			return value;
		}
		int GetEffMovePerTurn(){
			int value=0;
			for(int i=0; i<effectList.Count; i++) value+=effectList[i].unitStat.movePerTurn;
			return value;
		}
		int GetEffCounterPerTurn(){
			int value=0;
			for(int i=0; i<effectList.Count; i++) value+=effectList[i].unitStat.counterPerTurn;
			return value;
		}
		
		float GetEffDamage(){
			float value=0;
			for(int i=0; i<effectList.Count; i++) value+=effectList[i].unitStat.damage;
			return value;
		}
		//float GetEffDamageMax(){
		//	float value=0;
		//	for(int i=0; i<effectList.Count; i++) value+=effectList[i].unitStat.damageMax;
		//	return value;
		//}
		
		float GetEffHitChance(){
			float value=0;
			for(int i=0; i<effectList.Count; i++) value+=effectList[i].unitStat.hitChance;
			return value;
		}
		float GetEffDodgeChance(){
			float value=0;
			for(int i=0; i<effectList.Count; i++) value+=effectList[i].unitStat.dodgeChance;
			return value;
		}
		
		float GetEffCritChance(){
			float value=0;
			for(int i=0; i<effectList.Count; i++) value+=effectList[i].unitStat.critChance;
			return value;
		}
		float GetEffCritAvoidance(){
			float value=0;
			for(int i=0; i<effectList.Count; i++) value+=effectList[i].unitStat.critAvoidance;
			return value;
		}
		float GetEffCritMultiplier(){
			float value=0;
			for(int i=0; i<effectList.Count; i++) value+=effectList[i].unitStat.critMultiplier;
			return value;
		}
		
		float GetEffStunChance(){
			float value=0;
			for(int i=0; i<effectList.Count; i++) value+=effectList[i].unitStat.stunChance;
			return value;
		}
		float GetEffStunAvoidance(){
			float value=0;
			for(int i=0; i<effectList.Count; i++) value+=effectList[i].unitStat.stunAvoidance;
			return value;
		}
		int GetEffStunDuration(){
			int value=0;
			for(int i=0; i<effectList.Count; i++) value+=effectList[i].unitStat.stunDuration;
			return value;
		}
		
		float GetEffSilentChance(){
			float value=0;
			for(int i=0; i<effectList.Count; i++) value+=effectList[i].unitStat.silentChance;
			return value;
		}
		float GetEffSilentAvoidance(){
			float value=0;
			for(int i=0; i<effectList.Count; i++) value+=effectList[i].unitStat.silentAvoidance;
			return value;
		}
		int GetEffSilentDuration(){
			int value=0;
			for(int i=0; i<effectList.Count; i++) value+=effectList[i].unitStat.silentDuration;
			return value;
		}
		
		float GetEffFlankingBonus(){
			float value=0;
			for(int i=0; i<effectList.Count; i++) value+=effectList[i].unitStat.flankingBonus;
			return value;
		}
		float GetEffFlankedModifier(){
			float value=0;
			for(int i=0; i<effectList.Count; i++) value+=effectList[i].unitStat.flankedModifier;
			return value;
		}
		
		float GetEffHPPerTurn(){
			float value=0;
			for(int i=0; i<effectList.Count; i++) value+=effectList[i].unitStat.HPPerTurn;
			return value;
		}
		float GetEffAPPerTurn(){
			float value=0;
			for(int i=0; i<effectList.Count; i++) value+=effectList[i].unitStat.APPerTurn;
			return value;
		}
		
		//end ability effect section
		//********************************************************************************************************************************
		
		
		
		
		private int level=1;	//for data only, has no effect on game
		public int GetLevel(){ return level; }
		public void SetLevel(int lvl){ level=lvl; }
		
		public DataUnit GetData(){
			DataUnit data=new DataUnit();
			//data.unit=this;
			//data.level=level;
			
			data.Setup(this);
			
			return data;
		}
		//called by FactionManager in InitFaction to setup the stats of unit loaded from data
		public void ModifyStatsToData(DataUnit data, int ID){
			dataID=ID;
			
			level=data.level;
			
			if(data.HP>0){
				defaultHP=data.HP;
				HP=data.HP;
			}
			if(data.AP>0){
				defaultAP=data.AP;
				AP=data.AP;
			}
			
			if(data.turnPriority>0) turnPriority=data.turnPriority;
			
			if(data.moveRange>0) moveRange=data.moveRange;
			if(data.attackRange>0) attackRange=data.attackRange;
			
			if(data.hitChance>0) hitChance=data.hitChance;
			if(data.dodgeChance>0) dodgeChance=data.dodgeChance;
			if(data.damageMin>0) damageMin=data.damageMin;
			if(data.damageMax>0) damageMax=data.damageMax;
			
			if(data.critChance>0) critChance=data.critChance;
			if(data.critAvoidance>0) critAvoidance=data.critAvoidance;
			if(data.critMultiplier>0) critMultiplier=data.critMultiplier;
			
			if(data.stunChance>0) stunChance=data.stunChance;
			if(data.stunAvoidance>0) stunAvoidance=data.stunAvoidance;
			if(data.stunDuration>0) stunDuration=data.stunDuration;
			
			if(data.silentChance>0) silentChance=data.silentChance;
			if(data.silentAvoidance>0) silentAvoidance=data.silentAvoidance;
			if(data.silentDuration>0) silentDuration=data.silentDuration;
			
			if(data.HPPerTurn>0) HPPerTurn=data.HPPerTurn;
			if(data.APPerTurn>0) APPerTurn=data.APPerTurn;
		}
		
		protected void OnUnitDestroyed(Unit unit){		
			if(onUnitDestroyedE!=null) onUnitDestroyedE(this);
		}
		
		//********************************************************************************************************************************
		//these section are related to UnitAbilities
		
		public List<int> abilityIDList=new List<int>();
		public List<UnitAbility> abilityList=new List<UnitAbility>();	//only used in runtime
		public List<UnitAbility> GetAbilityList(){ return abilityList; }
		public void InitUnitAbility(){ 
			//get bonus abilityID from perk and add it to perkAbilityIDList
			List<int> perkAbilityIDList=PerkManager.GetUnitAbilityIDList(prefabID);
			for(int i=0; i<perkAbilityIDList.Count; i++) abilityIDList.Add(perkAbilityIDList[i]);
			
			abilityList=AbilityManagerUnit.GetAbilityListBasedOnIDList(abilityIDList);
			for(int i=0; i<abilityList.Count; i++) abilityList[i].SetUnit(this);
		}
		public UnitAbility GetUnitAbility(int ID){ return abilityList[ID]; }
		
		public int selectedAbilityID=-1;
		public int GetSelectedAbilityID(){ return selectedAbilityID; }
		public void ClearSelectedAbility(){
			if(selectedAbilityID<0) return;
			selectedAbilityID=-1;
			GridManager.ClearTargetMode();
			if(onUnitSelectedE!=null) onUnitSelectedE(this);
		}
		private bool requireTargetSelection=false;		//indicate if current selected Ability require target selection
		
		public string SelectAbility(int ID){	//called from UI to select an ability
			if(AbilityManagerFaction.GetSelectedAbilityID()>=0) AbilityManagerFaction.ClearSelectedAbility();
			
			if(selectedAbilityID>=0){
				if(selectedAbilityID==ID){
					if(!requireTargetSelection) AbilityTargetSelected(tile);	
					ClearSelectedAbility();
					return "";
				}
				else ClearSelectedAbility();
			}
			
			selectedAbilityID=ID;
			UnitAbility ability=abilityList[ID];
			
			string exception=ability.IsAvailable();
			if(exception!="") return exception;
			
			selectedAbilityID=ID;
			
			requireTargetSelection=ability.requireTargetSelection;
			if(requireTargetSelection) 
				GridManager.AbilityTargetMode(tile, ability.GetRange(), ability.GetAOERange(), ability.targetType, this.AbilityTargetSelected);
			
			//if(!ability.requireTargetSelection) AbilityTargetSelected(tile);	//not target selection required, just cast the ability on occupied tile
			//else GridManager.AbilityTargetMode(tile, ability.GetRange(), ability.GetAOERange(), ability.targetType, this.AbilityTargetSelected);
			
			if(onUnitSelectedE!=null) onUnitSelectedE(this);
			
			return "";
		}
		
		//callback function for when a target tile has been selected for current active ability
		private Tile abilityTargetedTile;
		public void AbilityTargetSelected(Tile tile){
			if(tile==null) return;
			
			abilityTargetedTile=tile;
			
			UnitAbility ability=abilityList[selectedAbilityID];
			AP-=ability.GetCost();
			ability.Use();
			
			AbilityManagerUnit.AbilityActivated();
			
			if(ability.shoot){
				GameObject shootObj=ability.shootObject;
				if(shootObj==null) shootObj=GameControl.GetDefaultShootObject();
				
				AttackInstance attInstance=new AttackInstance(this, selectedAbilityID);
				StartCoroutine(AttackRoutine(tile, tile.unit, shootObj, attInstance));
			}
			else{
				AbilityShootObjectHit(selectedAbilityID);
			}
			
			if(!ability.enableMoveAfterCast) moveRemain=0;
			if(!ability.enableAttackAfterCast) attackRemain=0;
			if(!ability.enableAbilityAfterCast) disableAbilities=1;
		}
		//callback function when the shootObject of an ability which require shoot hits it's target
		public void AbilityShootObjectHit(int abID){
			AbilityManagerUnit.ApplyAbilityEffect(this, abilityTargetedTile, abilityList[abID]);
			abilityTargetedTile=null;
		}
		
		//this is for ability teleport and spawnNewUnit to settle in new unit
		public void SetNewTile(Tile targetTile){	
			if(tile!=null) tile.unit=null;
			tile=targetTile;
			targetTile.unit=this;
			thisT.position=targetTile.GetPos();
			
			FactionManager.UpdateHostileUnitTriggerStatus(this);
			SetupFogOfWar();
		}
		
		//end UnitAbilities section
		//********************************************************************************************************************************
		
		
		
		public static bool gameStarted = false;
		
		[HideInInspector] public Transform thisT;
		[HideInInspector] public GameObject thisObj;
		
		void Awake(){
			thisT=this.transform;
			thisObj=gameObject;
			
			for(int i=0; i<shootPointList.Count; i++){
				if(shootPointList[i]==null){
					shootPointList.RemoveAt(i);
					i-=1;
				}
			}
			
			if(shootPointList.Count==0) shootPointList.Add(thisT);
			
			if(turretObject==null) turretObject=thisT;
		}
		
		void GameStart () {
			GameControl.AddActionAtStart ();
			if (!gameStarted) {
				HP = GetFullHP ();
			} else {
				if(this.transform.name.Equals("Achilles")){
					HP = AchillesHP;
				}else if(this.transform.name.Equals ("Archer")){
					HP = AtalantaHP;
				}else if(this.transform.name.Equals ("Hercules")){
					HP = HerculesHP;
				}
				if(HP <=0) StartCoroutine(Dead());
			}
			if(GameControl.RestoreUnitAPOnTurn()) AP=GetFullAP();
			GameControl.CompleteActionAtStart ();
		}
		
		void OnEnable(){
			AbilityManagerUnit.onIterateAbilityCooldownE += OnIterateAbilityCooldown;
			GameControl.onPassLevelE += PassLevel;
			GameControl.onGameStartE += GameStart;
		}
		void OnDisable(){
			AbilityManagerUnit.onIterateAbilityCooldownE -= OnIterateAbilityCooldown;
			GameControl.onPassLevelE -= PassLevel;
			GameControl.onGameStartE -= GameStart;

		}
		
		void OnIterateAbilityCooldown(){
			for(int i=0; i<abilityList.Count; i++) abilityList[i].IterateCooldown();
		}
		
		
		
		public void Select(){ 
			if(unitAudio!=null) unitAudio.Select();
			if(onUnitSelectedE!=null) onUnitSelectedE(this);
		}
		public static void Deselect(){ if(onUnitSelectedE!=null) onUnitSelectedE(null); }


//		void OnGUI(){
//			Vector2 targetPos;
//			targetPos = Camera.main.WorldToScreenPoint (this.transform.position);
//			
//			GUI.Box(new Rect(targetPos.x, Screen.height- targetPos.y, 60, 20), HP + "/" + defaultHP);
//		}
		
		private bool rotating=false;
		private Quaternion facingRotation;
		public void Rotate(Quaternion rotation){ 
			facingRotation=rotation;
			if(!rotating) StartCoroutine(_Rotate());
		}
		IEnumerator _Rotate(){
			rotating=true;
			while(Quaternion.Angle(facingRotation, thisT.rotation)>0.5f){
				if(!TurnControl.ClearToProceed()) yield break;
				thisT.rotation=Quaternion.Lerp(thisT.rotation, facingRotation, Time.deltaTime*moveSpeed*2);
				yield return null;
			}
			rotating=false;
		}
		
		public void Move(Tile targetTile){
			if(moveRemain<=0) return;
			if (FactionManager.IsPlayerTurn ())
				GameControl.ChooseSelectedUnit ();
			moveRemain-=1;
			Debug.Log("moving "+name+" to "+targetTile);
			GameControl.LockUnitSelect();


			StartCoroutine(MoveRoutine(targetTile));
			this.AP -= targetTile.distance;
		}
		public IEnumerator MoveRoutine(Tile targetTile){
			tile.unit=null;
			GridManager.ClearAllTile();
			
			int allowedDistance = GetEffectiveMoveRange();
			List<Tile> path=AStar.SearchWalkableTile(tile, targetTile);

			//while(path.Count>GetMoveRange()) path.RemoveAt(path.Count-1);
//			Debug.Log ("Allowed Distance: " + allowedDistance);
			for(int a =path.Count - 1; a >= 0; a--){
//				Debug.Log (path[a].name);
//				Debug.Log (path[a].distance);
				if(path[a].distance > allowedDistance || path[a].unit != null){
					Debug.Log ("Removing " + path[a] + " which have distance " + path[a].distance + " and the allowed distance is " + allowedDistance);
					path.RemoveAt(a);
				}
			}
			if(GridManager.GetTileType()==_TileType.Square){	//smooth the path so the unit wont zig-zag along a diagonal direction
				path.Insert(0, tile);
				path=PathSmoothing.SmoothDiagonal(path);
				path.RemoveAt(0);
			}


//			AP-=GetMoveAPCost()*(int)Mathf.Min(GetMoveRange(), GridManager.GetDistance(tile, targetTile));

			while(!TurnControl.ClearToProceed()) yield return null;
			
			if(GameControl.EnableFogOfWar() && isAIUnit){
				bool pathVisible=false;
				for(int i=0; i<path.Count; i++){
					if(path[i].IsVisible()){
						pathVisible=true;
						break;
					}
				}
				
				if(!pathVisible){
					thisT.position=path[path.Count-1].GetPos();
					tile=path[path.Count-1];
					tile.unit=this;
					yield break;
				}
			}
			
			//path.Insert(0, tile);
			//PathSmoothing.Smooth(path);
			//path.RemoveAt(0);
			TurnControl.ActionCommenced();

			if(unitAnim!=null) unitAnim.Move();
			if(unitAudio!=null) unitAudio.Move();
			
			
			while(path.Count>0){
				
				UpdateVisibility(path[0]);
				
				/*
				//for path smoothing with subpath witin tile
				List<Vector3> tilePath=path[0].GetPath();
				while(tilePath.Count>0){
					while(true){
						Quaternion wantedRot=Quaternion.LookRotation(tilePath[0]-thisT.position);
						thisT.rotation=Quaternion.Lerp(thisT.rotation, wantedRot, Time.deltaTime*moveSpeed*4);
						
						float dist=Vector3.Distance(thisT.position, tilePath[0]);
						if(dist<0.05f) break;
						
						Vector3 dir=(tilePath[0]-thisT.position).normalized;
						thisT.Translate(dir*Mathf.Min(moveSpeed*Time.deltaTime, dist), Space.World);
						yield return null;
					}
					
					tilePath.RemoveAt(0);
				}
				*/
				
				while(true){
					float dist=Vector3.Distance(thisT.position, path[0].GetPos());
					if(dist<0.05f){
						break;
					}
					Quaternion wantedRot=Quaternion.LookRotation(path[0].GetPos()-thisT.position);
					thisT.rotation=Quaternion.Slerp(thisT.rotation, wantedRot, Time.deltaTime*moveSpeed*3);
					
					Vector3 dir=(path[0].GetPos()-thisT.position).normalized;
					thisT.Translate(dir*Mathf.Min(moveSpeed*Time.deltaTime, dist), Space.World);
					yield return null;
				}
				
				tile=path[0];
				
				FactionManager.UpdateHostileUnitTriggerStatus(this);
				SetupFogOfWar();
				
				path.RemoveAt(0);
			}
			
			if(unitAnim!=null) unitAnim.StopMove();
			
			tile.unit=this;
			thisT.position=tile.GetPos();
			

			TurnControl.ActionCompleted(GameControl.delayPerAction);
			FinishAction();

 
		}
		
		
		
		
		public void Attack(Unit targetUnit){
			//variable used to tell particles it can get ready to show
			target = targetUnit;

			//makes this unit used in case it wasn't already
			usedThisTurn = true;
			if(attackRemain==0) return;
			if(AP<GetAttackAPCost()) return;
			attackRemain-=1;
			AP-=GetAttackAPCost();
			
			Debug.Log(this.unitName+" attacking "+targetUnit+"      "+GridManager.GetDistance(tile, targetUnit.tile, true)+"    "+GetAttackRange()+"     "+FogOfWar.InLOS(tile, targetUnit.tile)+"      attackRemain:"+attackRemain);//.unitName);
			GameControl.LockUnitSelect();
			
			AttackInstance attInstance=new AttackInstance(this, targetUnit);
			attInstance.Process();
			
			GameObject shootObj=shootObject!=null ? shootObject.gameObject : null ;
			if(shootObj==null) shootObj=GameControl.GetDefaultShootObject();
			StartCoroutine(AttackRoutine(targetUnit.tile, targetUnit, shootObj, attInstance));
			
			if(!GameControl.EnableActionAfterAttack()){
				moveRemain=0;
				disableAbilities=1;
			}
		}
		
		
		
		public bool rotateTurretOnly=false;
		public bool rotateTurretAimInXAxis=true;
		public Transform turretObject;
		public Transform barrelObject;
		public ShootObject shootObject;
		public List<Transform> shootPointList=new List<Transform>();
		public float delayBetweenShootPoint=0;
		
		private bool aiming=false;	//to avoid aiming and rotate back to origin at the same time
		bool Aiming(Tile tile, Unit targetUnit=null){
			Quaternion wantedRotY=Quaternion.LookRotation(tile.GetPos()-thisT.position);
			Vector3 targetPos=(targetUnit==null) ? tile.GetPos() : targetUnit.GetTargetT().position;
			
			//rotate body
			if(!rotateTurretOnly){
				thisT.rotation=Quaternion.Slerp(thisT.rotation, wantedRotY, Time.deltaTime*rotateSpeed);
			}
			
			if(rotateTurretAimInXAxis){
				if(barrelObject!=null){
					turretObject.rotation=Quaternion.Slerp(turretObject.rotation, wantedRotY, Time.deltaTime*rotateSpeed);
					
					Quaternion wantedRot=Quaternion.LookRotation(targetPos-barrelObject.position);
					//~ if(targetUnit==null) wantedRot=Quaternion.LookRotation(tile.GetPos()-barrelObject.position);
					//~ else wantedRot=Quaternion.LookRotation(targetUnit.GetTargetT().position-barrelObject.position);
					
					Quaternion barrelRot=Quaternion.Euler(wantedRot.eulerAngles.x, 0, 0);
					barrelObject.localRotation=Quaternion.Slerp(barrelObject.localRotation, barrelRot, Time.deltaTime*rotateSpeed);
					
					float angle1=Quaternion.Angle(turretObject.rotation, wantedRotY);
					float angle2=Quaternion.Angle(barrelObject.localRotation, barrelRot);
					//~ float angle=Quaternion.Angle(barrelObject.rotation, wantedRot);
					if(angle1<0.5f && angle2<0.5f) return true;
				}
				else{
					Quaternion wantedRot=Quaternion.LookRotation(targetPos-turretObject.position);
					
					turretObject.rotation=Quaternion.Slerp(turretObject.rotation, wantedRot, Time.deltaTime*rotateSpeed);
					float angle=Quaternion.Angle(turretObject.rotation, wantedRot);
					if(angle<.5f) return true;
				}
			}
			else{
				turretObject.rotation=Quaternion.Slerp(turretObject.rotation, wantedRotY, Time.deltaTime*rotateSpeed);
				float angle=Quaternion.Angle(turretObject.rotation, wantedRotY);
				if(angle<1f) return true;
			}
			
			return false;
		}
		bool RotateTurretToOrigin(){
			turretObject.localRotation=Quaternion.Lerp(turretObject.localRotation, Quaternion.identity, Time.deltaTime*rotateSpeed);
			float angle=Quaternion.Angle(turretObject.localRotation, Quaternion.identity);
			
			if(barrelObject!=null){
				barrelObject.localRotation=Quaternion.Slerp(barrelObject.localRotation, Quaternion.identity, Time.deltaTime*rotateSpeed);
				float angleAlt=Quaternion.Angle(barrelObject.localRotation, Quaternion.identity);
				return (angle>1 || angleAlt>1) ? false : true ;
			}
			else return angle>1 ? false : true ;
		}
		
		public IEnumerator AttackRoutine(Tile targetTile, Unit targetUnit, GameObject shootObject, AttackInstance attInstance){
			TurnControl.ActionCommenced();

//			i ff (!attInstance.isAbility && !attInstance.stunned && !attInstance.destroyed && targetUnit != null && targetUnit.CanCounter (this))
//				Debug.Log ("Tried to counter");
//			else {
//				Debug.Log (string.Format("Didn try to counter because is ability? {0}. Is stun? {1}. Is destroyed? {2}. Target is null? {3}", attInstance.isAbility,attInstance.stunned , attInstance.destroyed, targetUnit == null));
//			}
//			if (!targetUnit.CanCounter (this)) {
//				if(!GameControl.EnableCounter()) Debug.Log ("Game control fault");
//				if(targetUnit.stunned>0) Debug.Log ("Stunned");
//				if(targetUnit.counterRemain<=0) Debug.Log ("No more counters");
//				if(targetUnit.GetCounterAPCost()>AP) Debug.Log ("No more AP");
//				
//				float dist=GridManager.GetDistance(this.tile, targetUnit.tile);
//				if(dist>GetAttackRange()) Debug.Log ( "Too far");
//				
//				Debug.Log ( "Yes it can");
//			}

			
			aiming=true;	yield return null;
			while(!Aiming(targetTile, targetUnit)) yield return null;
			aiming=false;

			float attackDelay = 0;

			if(unitAnim!=null) attackDelay = unitAnim.Attack();
			//			if (unitParticles != null)
			//				unitParticles.Attack (targetUnit);
			if(unitAudio!=null) unitAudio.Attack();
			Debug.Log ("Attack delay is: " + attackDelay);
			yield return new WaitForSeconds (attackDelay);

			//shoot
			for(int i=0; i<shootPointList.Count; i++){
				Shoot(shootObject, targetTile, shootPointList[i], attInstance, i==shootPointList.Count-1);
				if(delayBetweenShootPoint>0) yield return new WaitForSeconds(delayBetweenShootPoint);
			}
			//play animation
			TurnControl.ActionCompleted(0);
			while (!TurnControl.ClearToProceed()) {

				yield return null;
				}
			FinishAction();

			if(!attInstance.isAbility && !attInstance.stunned && !attInstance.destroyed && targetUnit != null && targetUnit.CanCounter(this)) targetUnit.Counter(this);
			
			while(TurnControl.CounterInProgress()) yield return null;

			if(turretObject!=null && turretObject!=thisT){ while(!RotateTurretToOrigin() && !aiming) yield return null; }
			target = null;
			if (unitParticles != null)
				unitParticles.EndAttack ();
		}
		
		
		//counter attack routine
		public void Counter(Unit targetUnit){ 			
			TurnControl.CounterCommenced();
			StartCoroutine(CounterRoutine(targetUnit)); }
		public IEnumerator CounterRoutine(Unit targetUnit){

			if (FactionManager.IsPlayerTurn ()) {
				Debug.Log(this.name + " is trying to counter");
			}

			yield return null;	//wait for shot to be fired first
									//TurnControl.actionInProgress will be set to >2 when there's shootObject active
									//thus the next line wont be skipped, refer toTurnControl.ClearToCounter() 
			
			while(!TurnControl.ClearToCounter()) yield return null;

			AP-=GetCounterAPCost();
			Debug.Log(AP+"    "+GetCounterAPCost());
			counterRemain-=1;
			
			while(!Aiming(targetUnit.tile, targetUnit)) yield return null;

			float delayAttack = 0;

			if(unitAnim!=null) delayAttack = unitAnim.Attack();
			if(unitAudio!=null) unitAudio.Attack();
			
			AttackInstance attInstance=new AttackInstance(this, targetUnit, true);
			attInstance.Process();
			
			GameObject shootObj=shootObject!=null ? shootObject.gameObject : null ;
			if(shootObj==null) shootObj=GameControl.GetDefaultShootObject();
			
			for(int i=0; i<shootPointList.Count; i++){
				Shoot(shootObj, targetUnit.tile, shootPointList[i], attInstance, i==shootPointList.Count-1);
				if(delayBetweenShootPoint>0) yield return new WaitForSeconds(delayBetweenShootPoint);
			}
			
			TurnControl.ActionCompleted(delayAttack);
			while(!TurnControl.ClearToCounter()) yield return null;
			TurnControl.CounterCompleted();
			

			if(turretObject!=null && turretObject!=thisT){ while(!RotateTurretToOrigin() && !aiming) yield return null; }
		}
		
		
		private void Shoot(GameObject shootObj, Tile targetTile, Transform sp, AttackInstance attInstance, bool lastShootPoint=false){
			GameObject sObjInstance=(GameObject)Instantiate(shootObj, sp.position, sp.rotation);
			ShootObject soInstance=sObjInstance.GetComponent<ShootObject>();
			if(!attInstance.isAbility){							//for normal attack
				if(!lastShootPoint){
					Vector3 pos=targetTile.unit.GetTargetT().position;
					soInstance.Shoot(pos, null);
				}
				else soInstance.Shoot(attInstance);
			}
			else{ 													//this is for UnitAbility
				Vector3 pos=targetTile.GetPos();
				if(targetTile.unit!=null) pos=targetTile.unit.GetTargetT().position;
				if(!lastShootPoint) soInstance.Shoot(pos, null);
				else soInstance.Shoot(pos, attInstance);
			}
		}
		
		
		void FinishAction(){
			if(isAIUnit) return;
			
			if(!IsAllActionCompleted()) GameControl.SelectUnit(this);
			else{
				FactionManager.UnitMoveDepleted(this);
				TurnControl.NextUnit();
			}
		}
		
		
		public void ApplyAttack(AttackInstance attInstance){
			attInstance.srcUnit.lastTarget=this;
			lastAttacker=attInstance.srcUnit;
			if(isAIUnit && !trigger) trigger=true;
			
			if(attInstance.missed){
				new TextOverlay(GetTargetT().position, "missed", Color.white);
				return;
			}
			
			if(unitAudio!=null) unitAudio.Hit();
			if(unitAnim!=null) unitAudio.Hit();
			
			ApplyDamage(attInstance.damage, attInstance.critical, false, attInstance.srcUnit);
			
			if(attInstance.stunned && stunned<attInstance.stun){
				AddUnitToEffectTracker();
				stunned=attInstance.stun;
				stunCounter.Count(attInstance.stun);
				VisualEffectManager.UnitStunned(this, stunned);
			}
			
			if(attInstance.silenced && silenced<attInstance.silent){
				AddUnitToEffectTracker();
				silenced=attInstance.silent;
				silentCounter.Count(attInstance.silent);
			}
		}
		public virtual void ApplyDamage(float dmg, bool critical=false, bool showOverlay = false, Unit source = null){
			//Call unit damage text
			if(showOverlay){
				if(!critical) new TextOverlay(GetTargetT().position + new Vector3(0, 2,0), dmg.ToString("f0"), new Color(.713f,.188f,.188f, 1f));
				else new TextOverlay(GetTargetT().position, dmg.ToString("f0")+" Critical!", new Color(1f, .6f, 0, 1f));
			}
//			HP-=dmg;

			bool playerUnit = this.factionID == FactionManager.GetPlayerFactionID () [0];

			float g = this.GetEffectiveGuard ();

			this.HP-= dmg - g >0? dmg - g : 0;
			float totalHP = this.HP + this.tile.tileDefense;


			if (playerUnit) {
				getStack ().decreaseGuard ((int)dmg);
				totalHP += this.getStack ().getGuard ();
				if(GameControl.selectedUnit != null)UI.UpdateUnitInfo(GameControl.selectedUnit);
			} else {
				if(GameControl.selectedUnit != null)UI.UpdateUnitInfo(GameControl.selectedUnit);
				if(GameControl.selectedTile != null)UI.UpdateEnemyInfo(GameControl.selectedTile.unit);
			}
			if (totalHP <=0){
			//It's important to do some sort of animation in case the unit didn't die
//				if(playerUnit){

					//this will be used to decrease the total life of the player
//					if(source != null){
//						FactionManager.playerHP -= source.HPDamage;
//						PlayerHP.UpdatePlayerHP(true);
//						if(FactionManager.playerHP <= 0){//GameOver
//						}
//					}
//				}
					//if its an enemy unit, the enemy should just disappear and give exp/money/etc
//				else{
					HP=0;
				
					StartCoroutine(Dead());
				
					ClearVisibleTile();
					tile.unit=null;
			}
		}

		public float GetEffectiveHP (){
			return defaultHP + this.getStack ().getGuard ();
		}
		                      
		
		public GameObject destroyEffectObj;
		protected virtual IEnumerator Dead(){
			if (gameStarted && !this.isAIUnit) 
				GameControl.AddActionAtStart ();
			if(destroyEffectObj!=null) Instantiate(destroyEffectObj, GetTargetT().position, Quaternion.identity);
			
			float delay=0.5f;
			if(unitAudio!=null) delay=Mathf.Max (delay,unitAudio.Destroy());
			if(unitAnim!=null) delay=Mathf.Max(delay, unitAnim.Destroy());

			OnUnitDestroyed(this);

			yield return new WaitForSeconds(delay + 0.7f);
			if (gameStarted && !this.isAIUnit) 
				GameControl.CompleteActionAtStart ();
			Destroy(thisObj);
		}
		
		public void RestoreHP(float val){
			HP=Mathf.Min(HP+val, GetFullHP());
			new TextOverlay(GetTargetT().position, val.ToString("f0"), Color.green);
		}
		
		//called when a unit just reach it's turn
		public void ResetUnitTurnData(){
			moveRemain=GetMovePerTurn();
			attackRemain=GetAttackPerTurn();
			counterRemain=GetCounterPerTurn();
			disableAbilities=0;
			
			if(GameControl.RestoreUnitAPOnTurn()) AP=GetFullAP();
			else AP=Mathf.Min(AP+GetAPPerTurn(), GetFullAP());
			
			HP=Mathf.Min(HP+GetHPPerTurn(), GetFullHP());
		}
		
		public bool CanCounter(Unit unit){
			if(!GameControl.EnableCounter()) return false;
			if(stunned>0) return false;
			if(counterRemain<=0) return false;
			if(GetCounterAPCost()>AP) return false;
			
			float dist=GridManager.GetDistance(unit.tile, tile);
			if(dist>GetAttackRange()) return false;

			if(GameControl.EnableFogOfWar()){
				if(requireDirectLOSToAttack && !FogOfWar.InLOS(unit.tile, tile)) return false; 
				if(!FogOfWar.IsTileVisibleToFaction(unit.tile, factionID)) return false;
			}
			
			return true;
		}
		
		public bool IsAllActionCompleted(){
			if(stunned>0) return true;
//			if (FactionManager.IsPlayerTurn ()) return false;
			if(attackRemain>0 && AP>=GetAttackAPCost()) return false;
			if(moveRemain>0 && AP>=GetMoveAPCost()) return false;
			return true;
		}
		
		
		
		//********************************************************************************************************************************
		//these section are related to FogOfWar
		
		[HideInInspector] private List<Tile> visibleTileList=new List<Tile>();	//a list of tile visible to the unit
		
		//called whenever the unit moved into a new tile
		public void SetupFogOfWar(bool ignoreFaction=false){
			if(!GameControl.EnableFogOfWar()) return;
			if(!ignoreFaction && isAIUnit) return;
			StartCoroutine(FogOfWarNewTile(ignoreFaction));
		}
		
		//add new tiles within sight to visibleTileList and remove those that are out of sight
		public IEnumerator FogOfWarNewTile(bool ignoreFaction){
			List<Tile> newList=GridManager.GetTilesWithinDistance(tile, sight, false);
			newList.Add(tile);
			for(int i=0; i<newList.Count; i++){
				if(FogOfWar.CheckTileVisibility(newList[i])){
					newList[i].SetVisible(true);
				}
			}
			
			yield return new WaitForSeconds(0.1f);
			
			ClearVisibleTile();
			
			visibleTileList=newList;
		}
		//set visible tile that becomes invisible to invisible
		public void ClearVisibleTile(){
			for(int i=0; i<visibleTileList.Count; i++){
				if(!FogOfWar.CheckTileVisibility(visibleTileList[i])) visibleTileList[i].SetVisible(false);
			}
		}
		
		//called just before a unit start moving into a new tile, for AI unit to show/hide itself as it move in/out of fog-of-war
		//also called when a unit is just been placed on a grid in mid-game
		public void UpdateVisibility(Tile newTile=null){
			if(!GameControl.EnableFogOfWar()) return;
			if(!isAIUnit) return;
			
			if(newTile==null) newTile=tile;
			
			if(newTile.IsVisible()){
				thisObj.layer=LayerManager.GetLayerUnit();
				Utilities.SetLayerRecursively(thisT, LayerManager.GetLayerUnit());
			}
			else{
				thisObj.layer=LayerManager.GetLayerUnitInvisible();
				Utilities.SetLayerRecursively(thisT, LayerManager.GetLayerUnitInvisible());
			}
		}
		
		//end FogOfWar section
		//********************************************************************************************************************************

		//This method is necessary to get the stack of this unit card
		public CardsStackManager getStack(){
			return (CardsStackManager)transform.GetComponent<CardsStackManager> ();
		}

		//Method called on PassLevel of Game Control to save a few variables before passing

		public static void PassLevel(){
			List<Unit> units = FactionManager.GetAllPlayerUnits ();
			
			foreach (Unit u in units) {
				if(u.transform.name.Equals("Achilles")){
					AchillesHP = (int)u.HP ;
				}else if(u.transform.name.Equals ("Archer")){
					AtalantaHP = (int)u.HP;
				}else if(u.transform.name.Equals ("Hercules")){
					HerculesHP = (int)u.HP;
				}	
			}

		}


		
		[HideInInspector] protected UnitParticles unitParticles;
		public void setParticles(UnitParticles unitParticlesInstance){this.unitParticles = unitParticlesInstance;} 

		[HideInInspector] protected UnitAudio unitAudio;
		public void SetAudio(UnitAudio unitAudioInstance){ unitAudio=unitAudioInstance; }
		
		[HideInInspector] protected UnitAnimation unitAnim;
		public void SetAnimation(UnitAnimation unitAnimInstance){ 
			unitAnim=unitAnimInstance;
		}
		public void DisableAnimation(){ unitAnim=null; }
	}

}