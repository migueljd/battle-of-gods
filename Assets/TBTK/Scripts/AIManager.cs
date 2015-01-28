using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using TBTK;

namespace TBTK{

	public enum _AIMode{
		Passive, 	//the unit wont move unless the there are hostile within the faction's sight (using unit sight value even when Fog-Of-War is not used)
		Trigger, 		//the unit wont move unless it's being triggered, when it spotted any hostile or attacked
		Aggressive,	//the unit will be on move all the time, looking for potential target
	}
	
	public class AIManager : MonoBehaviour {

		public _AIMode mode=_AIMode.Passive;
		public static _AIMode GetAIMode(){ return instance.mode; }
		
		private static AIManager instance;
		
		void Awake(){
			instance=this;
		}
		
		//called in FactionManager to move the whole faction
		public static void MoveFaction(Faction faction){ instance._MoveFaction(faction); }
		public void _MoveFaction(Faction faction){
			StartCoroutine(FactionRoutine(faction));
		}
		
		//called in GameControl when a AI unit is selected to move that particular unit
		public static void MoveUnit(Unit unit){ instance._MoveUnit(unit); }
		public void _MoveUnit(Unit unit){
			StartCoroutine(SingleUnitRoutine(unit));
		}
		
		
		//move the whole faction, unit by unit
		IEnumerator FactionRoutine(Faction faction){
			//GameControl.DisplayMessage("AI's Turn");
			yield return new WaitForSeconds(0.5f);
			
			for(int i=0; i<faction.allUnitList.Count; i++){
				if(faction.allUnitList[i].IsStunned()) continue;
				
				StartCoroutine(MoveUnitRoutine(faction.allUnitList[i]));
				while(movingUnit) yield return null;
				yield return new WaitForSeconds(0.25f);
				
				if(GameControl.GetGamePhase()==_GamePhase.Over) yield break;
			}
			
			GameControl.EndTurn();
		}
		//move a single unit only
		IEnumerator SingleUnitRoutine(Unit unit){
			//GameControl.DisplayMessage("AI's Turn");
			yield return new WaitForSeconds(0.5f);
			
			if(!unit.IsStunned()){
				StartCoroutine(MoveUnitRoutine(unit));
				while(movingUnit) yield return null;
				yield return new WaitForSeconds(0.25f);
			}
			
			GameControl.EndTurn();
		}
		
		
		IEnumerator DelayEndTurn(){
			yield return new WaitForSeconds(1f);
			while(!TurnControl.ClearToProceed()) yield return null;
			GameControl.EndTurn();
		}
		
		
		
		
		private bool movingUnit=false;		//set to true when a unit is being moved
		IEnumerator MoveUnitRoutine(Unit unit){
			movingUnit=true;

			if(mode!=_AIMode.Aggressive && !unit.trigger){
				StartCoroutine(EndMoveUnitRoutine());
				yield break;
			}
			
			Tile targetTile=Analyse(unit);
			
			//first move to the targetTile
			if(targetTile!=unit.tile) unit.Move(targetTile);
			
			for(int i=0; i<targetTile.hostileInRangeList.Count; i++){
				if(targetTile.hostileInRangeList[i].unit==null || targetTile.hostileInRangeList[i].unit.factionID==unit.factionID){
					targetTile.hostileInRangeList.RemoveAt(i);		i-=1;
				}
			}
			
			//if there's hostile within range, attack it
			if(targetTile.hostileInRangeList.Count>0){
				if(targetTile!=unit.tile){	//wait until the unit has moved into the targetTile
					yield return new WaitForSeconds(.25f);
					while(!TurnControl.ClearToProceed()) yield return null;
				}
				
				int rand=Random.Range(0, targetTile.hostileInRangeList.Count);
				unit.Attack(targetTile.hostileInRangeList[rand].unit);
			}
			
			StartCoroutine(EndMoveUnitRoutine());
			
			yield return null;
		}
		
		//clear movingUnit flag so the next unit can be moved
		IEnumerator EndMoveUnitRoutine(){
			while(!TurnControl.ClearToProceed()) yield return null;
			yield return null;
			movingUnit=false;
		}
		
		
		//analyse the grid to know where the unit should move to
		private Tile Analyse(Unit unit){
			//get all wakable tiles in range first
			List<Tile> walkableTilesInRange=AStar.GetTileWithinDistance(unit.tile, unit.GetEffectiveMoveRange(), true);
//			Debug.Log("Walkable tiles: " + walkableTilesInRange.Count);
			walkableTilesInRange.Add(unit.tile);
			//setup all hostile in in those walkableTiles
			List<Unit> allHostileInSight=FactionManager.GetAllHostileUnit(unit.factionID);
			if(GameControl.EnableFogOfWar()){
				for(int i=0; i<allHostileInSight.Count; i++){
					if(!FogOfWar.IsTileVisibleToFaction(allHostileInSight[i].tile, unit.factionID)){
						allHostileInSight.RemoveAt(i);	i-=1;
					}
				}
			}
			
			//if cover system is in use
			if(GameControl.EnableCover()){
				List<Tile> halfCoveredList=new List<Tile>();	//a list for all the tiles with half Cover
				List<Tile> fullCoveredList=new List<Tile>();	//a list for all the tiles with full Cover
				
				if(allHostileInSight.Count==0) fullCoveredList=walkableTilesInRange;
				else{	//if there are hostile in sight
					
					//loop through all the walkable, record their score based on
					for(int i=0; i<walkableTilesInRange.Count; i++){
						Tile tile=walkableTilesInRange[i];
						tile.hostileCount=0;
						tile.coverScore=0;
						
						//iterate through all hostile, add the count, and cover type to the tile, this will then be used in tile.GetCoverRating() when this loop is complete
						for(int n=0; n<allHostileInSight.Count; n++){
							// if the hostile is out of range, ignore it
							int hostileRange=allHostileInSight[n].GetMoveRange()+allHostileInSight[n].GetAttackRange();
							if(GridManager.GetDistance(allHostileInSight[n].tile, tile)>hostileRange) continue;
							
							tile.hostileCount+=1;
							
							CoverSystem._CoverType coverType=CoverSystem.GetCoverType(allHostileInSight[n].tile, walkableTilesInRange[i]);
							if(coverType==CoverSystem._CoverType.Half) tile.coverScore+=1;
							else if(coverType==CoverSystem._CoverType.Full) tile.coverScore+=2;
						}
						
						//get cover rating for the tile
						//if score is >=2, the tile has full cover from all hostile, so add it to fullCoveredList
						//if score is >=1 && <2, the tile has half cover from all hostile, so add it to halfCoveredList
						//if anything <1, the tile is exposed to hostile in some manner
						if(tile.GetCoverRating()>=2) fullCoveredList.Add(tile);
						else if(tile.GetCoverRating()>=1) halfCoveredList.Add(tile);
					}
				}
				
				//if either of the CoveredList is not empty, replace walkableTilesInRange with that since there's no need to consider to move into tiles without cover
				if(fullCoveredList.Count!=0) walkableTilesInRange=fullCoveredList;
				else if(halfCoveredList.Count!=0) walkableTilesInRange=halfCoveredList;
			}
			
			//if there are hostile
			if(allHostileInSight.Count>0){
				//fill up the walkableTilesInRange hostile list 
				//then filter thru walkableTilesInRange, those that have a hostile in range will be add to a tilesWithHostileInRange
				List<Tile> tilesWithHostileInRange=new List<Tile>();
				GridManager.SetupHostileInRangeforTile(unit, walkableTilesInRange);
				for(int i=0; i<walkableTilesInRange.Count; i++){
					if(walkableTilesInRange[i].GetHostileInRange().Count>0) tilesWithHostileInRange.Add(walkableTilesInRange[i]);
				}
				
				//if the tilesWithHostileInRange is not empty after the process, means there's tiles which the unit can move into and attack
				//return one of those in the tilesWithHostileInRange so the unit can attack
				if(tilesWithHostileInRange.Count>0){
					//if the unit current tile is one of those tiles with hostile, just stay put and attack
					if(tilesWithHostileInRange.Contains(unit.tile)){
						//randomize it a bit so the unit do move around but not stay in place all the time
						if(Random.Range(0f, 1f)>0.25f) return unit.tile;
					}
					return tilesWithHostileInRange[Random.Range(0, tilesWithHostileInRange.Count)];
				}
			}
			
			//if there's not potential target at all, check if the unit has any previous attacker
			//if there are, go after the last attacker
			if(unit.lastAttacker!=null) return unit.lastAttacker.tile;
			
			
			//for aggresive mode with FogOfWar disabled, try move towards the nearest unit
//			if(mode==_AIMode.Aggressive && Random.Range(0f, 1f)>0.25f){
			if(mode==_AIMode.Aggressive || mode==_AIMode.Trigger){
				List<Unit> allHostile=FactionManager.GetAllHostileUnit(unit.factionID);
				float nearest=Mathf.Infinity;	int nearestIndex=0;

				List<Tile> attackableEnemies = new List<Tile>();

				for(int i=0; i<allHostile.Count; i++){
					float dist=AStar.GetDistance(allHostile[i].tile, unit.tile);
					foreach(Tile t in allHostile[i].tile.GetNeighbourList(true)){
						Debug.Log ("Trying");
						if(walkableTilesInRange.Contains(t)){
							attackableEnemies.Add(allHostile[i].tile);
							Debug.Log("Going away");
						}
					}
					if(dist<nearest){
						nearest=dist;
						nearestIndex=i;
					}
				}

				if(attackableEnemies.Count > 0) return attackableEnemies[Random.Range(0, attackableEnemies.Count - 1)];

				return allHostile[nearestIndex].tile;
			}
			
			
			//if there's really no hostile to go after, then just move randomly in one of the walkable
			int rand=Random.Range(0, walkableTilesInRange.Count);
			
			//clear in hostileInRange list for all moveable tile so, just in case the list is not empty (hostileInRange dont clear after each move)
			//so the unit dont try to attack anything when it moves into the targetTile
			walkableTilesInRange[rand].SetHostileInRange(new List<Tile>());
			
			return walkableTilesInRange[rand];
		}
		
	}

}

