using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using TBTK;

namespace TBTK{

	//a counter class used specifically to track the cooldown/effect-duration 
	[System.Serializable]
	public class DurationCounter{
		public float duration=0;
		public float durationAlt=0;
		
		public DurationCounter(float dur=0, float durAlt=0){
			duration=dur;
			durationAlt=durAlt;
		}
		
		public void Count(float dur=0){	//initiate the counter with a value to count
			duration=dur;
			Reset();
		}
		
		public void Iterate(){			//called at each turn ends to move the counter
			if(duration<=0) return;
			
			durationAlt-=1;
			if(durationAlt<=0){
				Reset();
				duration-=1;
			}
		}
		
		//reset the durationAlt value, use internally only
		private void Reset(){
			if(TurnControl.GetTurnMode()==_TurnMode.FactionPerTurn){
				if(TurnControl.GetMoveOrder()==_MoveOrder.Free) durationAlt=FactionManager.GetTotalFactionCount();
				else durationAlt=FactionManager.GetTotalUnitCount();
			}
			else if(TurnControl.GetTurnMode()==_TurnMode.FactionUnitPerTurn){
				if(TurnControl.GetMoveOrder()==_MoveOrder.Free) durationAlt=FactionManager.GetTotalFactionCount();
				else durationAlt=FactionManager.GetTotalUnitCount();
			}
			else durationAlt=FactionManager.GetTotalUnitCount();
		}
		
		public DurationCounter Clone(){
			return new DurationCounter(duration, durationAlt);
		}
	}
	
	
	
	public static class FogOfWar{
		
		public static void InitGrid(List<Tile> tileList){
			if(!GameControl.EnableFogOfWar()) return;
			
			for(int i=0; i<tileList.Count; i++) tileList[i].SetVisible(false);
			
			List<Unit> unitList=FactionManager.GetAllPlayerUnits();
			for(int i=0; i<unitList.Count; i++){
				unitList[i].SetupFogOfWar(true);
			}
		}
		
		//check a tile visiblility to player's faction
		public static bool CheckTileVisibility(Tile tile){
			List<Unit> unitList=FactionManager.GetAllPlayerUnits();
			for(int i=0; i<unitList.Count; i++){
				if(GridManager.GetDistance(tile, unitList[i].tile)<=unitList[i].GetSight()){ //return true;
					//if(InLOS(tile, unitList[i].tile, true)) return true;		//for showing LOS cast
					if(InLOS(tile, unitList[i].tile)) return true;
				}
			}
			return false;
		}
		
		//used to check if AI faction can see a given tile
		public static bool IsTileVisibleToFaction(Tile tile, int factionID){
			List<Unit> unitList=FactionManager.GetAllUnitsOfFaction(factionID);
			for(int i=0; i<unitList.Count; i++){
				if(GridManager.GetDistance(tile, unitList[i].tile)<=unitList[i].GetSight()){ //return true;
					if(InLOS(tile, unitList[i].tile)) return true;
				}
			}
			return false;
		}
		
		
		public static bool LOSRaycast(Vector3 pos, Vector3 dir, float dist, LayerMask mask, bool debugging=false){
			float debugDuration=1.5f;
			RaycastHit[] hits=Physics.RaycastAll(pos, dir, dist, mask);
			if(hits.Length!=0){
				if(debugging) Debug.DrawLine(pos, pos+dir*dist, Color.red, debugDuration);
				return true;
			}
			else{
				if(debugging) Debug.DrawLine(pos, pos+dir*dist, Color.white, debugDuration);
				return false;
			}
		}
		
		public static bool InLOS(Tile tile1, Tile tile2, bool debugging=false){ return InLOS(tile1,tile2, -1, debugging); }
		public static bool InLOS(Tile tile1, Tile tile2, float peekFactor, bool debugging=false){
			Vector3 pos1=tile1.GetPos();
			Vector3 pos2=tile2.GetPos();
			
			if(peekFactor<0) peekFactor=GameControl.GetPeekFactor();
			
			float dist=Vector3.Distance(pos2, pos1);
			Vector3 dir=(pos2-pos1).normalized;
			Vector3 dirO=new Vector3(-dir.z, 0, dir.x).normalized;
			float posOffset=GridManager.GetTileSize()*GridManager.GetGridToTileSizeRatio()*peekFactor;
			
			LayerMask mask=1<<LayerManager.GetLayerObstacleFullCover();// | 1<<LayerManager.GetLayerObstacleHalfCover();
			
			bool flag=false;
			
			if(!LOSRaycast(pos1, dir, dist, mask, debugging)){
				if(debugging) flag=true;
				else return true;
			}
			
			if(posOffset==0) return flag;
			
			if(!LOSRaycast(pos1+dirO*posOffset, dir, dist, mask, debugging)){
				if(debugging) flag=true;
				else return true;
			}
			if(!LOSRaycast(pos1-dirO*posOffset, dir, dist, mask, debugging)){
				if(debugging) flag=true;
				else return true;
			}
			
			return flag;
		}
		
	}
	
	
	
	public static class CoverSystem{
		
		public class Cover{
			public float angle=0;
			public _CoverType type;
			public Vector3 overlayPos;
			public Quaternion overlayRot;
		}
		
		public static float fullCoverBonus=0.75f;
		public static float halfCoverBonus=0.25f;
		public static void SetFullCoverDodgeBonus(float val){ fullCoverBonus=val; }
		public static void SetHalfCoverDodgeBonus(float val){ halfCoverBonus=val; }
		public static float GetFullCoverDodgeBonus(){ return fullCoverBonus; }
		public static float GetHalfCoverDodgeBonus(){ return halfCoverBonus; }
		
		public static float exposedCritChanceBonus=0.3f;
		public static void SetExposedCritChanceBonus(float val){ exposedCritChanceBonus=val; }
		public static float GetExposedCritChanceBonus(){ return exposedCritChanceBonus; }
		
		
		public enum _CoverType{None, Half, Full}
		
		
		public static void InitCoverForTile(Tile tile){
			List<Tile> neighbourList=tile.GetNeighbourList();
			List<Cover> coverList=new List<Cover>();
			for(int i=0; i<neighbourList.Count; i++){
				Vector3 dir=(neighbourList[i].GetPos()-tile.GetPos()).normalized;
				float dist=GridManager.GetTileSize()*GridManager.GetGridToTileSizeRatio()*.75f;
				
				//if(GridManager.GetTileType()==_TileType.Square){	//dont add diagonal cover, when diagonal neighbour is used
				//	if(dir.x*dir.z!=0) continue;
				//}
				
				LayerMask mask=1<<LayerManager.GetLayerObstacleFullCover() | 1<<LayerManager.GetLayerObstacleHalfCover();
				RaycastHit hit;
				if(Physics.Raycast(tile.GetPos(), dir, out hit, dist, mask)){
					Cover cover=new Cover();
					cover.angle=Mathf.Round(Utilities.Vector2ToAngle(new Vector2(dir.x, dir.z)));
					
					int layer=hit.transform.gameObject.layer;
					if(layer==LayerManager.GetLayerObstacleFullCover()){
						cover.type=_CoverType.Full;
						Debug.DrawLine(tile.GetPos(), tile.GetPos()+dir*dist, Color.red, 2);
					}
					else if(layer==LayerManager.GetLayerObstacleHalfCover()){
						cover.type=_CoverType.Half;
						Debug.DrawLine(tile.GetPos(), tile.GetPos()+dir*dist, Color.white, 2);
					}
					
					
					if(GridManager.GetTileType()==_TileType.Square) cover.overlayPos=tile.GetPos()+dir*dist*0.4f;
					else if(GridManager.GetTileType()==_TileType.Hex) cover.overlayPos=tile.GetPos()+dir*dist*0.35f;
					
					float angleY=cover.angle+90;
					if(cover.angle==30) angleY=cover.angle+30;
					else if(cover.angle==150) angleY=cover.angle-30;
					else if(cover.angle==210) angleY=cover.angle+30;
					else if(cover.angle==330) angleY=cover.angle-30;
					cover.overlayRot=Quaternion.Euler(0, angleY, 0);
					
					coverList.Add(cover);
				}
			}
			
			tile.coverList=coverList;
		}
		
		public static _CoverType GetCoverType(Tile attackingTile, Tile targetTile){
			Vector3 dir=attackingTile.GetPos()-targetTile.GetPos();
			
			float angle=0;
			if(GridManager.GetTileType()==_TileType.Square) angle=Utilities.VectorToAngle90(new Vector2(dir.x, dir.z));
			else if(GridManager.GetTileType()==_TileType.Hex) angle=Utilities.VectorToAngle60(new Vector2(dir.x, dir.z));
			
			_CoverType cover=_CoverType.None;
			for(int i=0; i<targetTile.coverList.Count; i++){
				if(Mathf.Abs(targetTile.coverList[i].angle-angle)<90){
					cover=targetTile.coverList[i].type;
					if(targetTile.coverList[i].type==_CoverType.Full) break;
				}
			}
			
			return cover;
		}
		
	}
	
	
	
	[System.Serializable]
	public class TBTKItem{
		public int ID=0;
		public string name="";
		
		public Sprite icon;
	}
	
	[System.Serializable]
	public class DAType : TBTKItem{
		public string desp="";
	}
	[System.Serializable]
	public class DamageType : DAType{
		
	}
	[System.Serializable]
	public class ArmorType : DAType{
		public List<float> modifiers=new List<float>();
	}
	
	
	
	
	//A* information for each tile
	public class TileAStar{
		public Tile tile;
		
		//neighbour list is only setup once the application is playing
		private List<Tile> neighbourList=new List<Tile>();
		public void SetNeighbourList(List<Tile> list){ neighbourList=list; }
		
		private List<Tile> disconnectedNeighbourList=new List<Tile>();
		
		public enum _AStarListState{Unassigned, Open, Close};
		public _AStarListState listState=_AStarListState.Unassigned;
		
		public Tile parent=null;
		public float scoreG;
		public float scoreH;
		public float scoreF;
		public float tempScoreG;
		public int rangeRequired;	//the range-cost required to move to this tile, for grid with height variance
		
		
		public TileAStar(Tile t){ tile=t; }
		
		public bool IsNeighbourDisconnected(Tile tile){
			return disconnectedNeighbourList.Contains(tile) ? true : false ;
		}
		
		public void DisconnectNeighbour(Tile tile){
			if(neighbourList.Contains(tile)){
				neighbourList.Remove(tile);
				disconnectedNeighbourList.Add(tile);
			}
		}
		public void ConnectNeighbour(Tile tile){
			if(disconnectedNeighbourList.Contains(tile)){
				neighbourList.Add(tile);
				disconnectedNeighbourList.Remove(tile);
			}
		}
		
		
		
		public List<Tile> GetNeighbourList(bool walkableOnly=false){ 
			List<Tile> newList=new List<Tile>();
			if(walkableOnly){
				for(int i=0; i<neighbourList.Count; i++){
					if(neighbourList[i].walkable && neighbourList[i].unit==null) newList.Add(neighbourList[i]);
				}
			}
			else{
				for(int i=0; i<neighbourList.Count; i++) newList.Add(neighbourList[i]);
				for(int i=0; i<disconnectedNeighbourList.Count; i++) newList.Add(disconnectedNeighbourList[i]);
			}
			return newList;
		}
		
		
		//call during a serach to scan through neighbour, check their score against the position passed
		//process walkable neighbours only, used to search for a walkable path via A*
		public void ProcessWalkableNeighbour(Tile targetTile){
			for(int i=0; i<neighbourList.Count; i++){
				TileAStar neighbour=neighbourList[i].aStar;
				if(((neighbour.tile.walkable && neighbour.tile.unit==null)) || neighbour.tile==targetTile){
					//if the neightbour state is clean (never evaluated so far in the search)
					if(neighbour.listState==_AStarListState.Unassigned){
						//check the score of G and H and update F, also assign the parent to currentNode
						neighbour.scoreG=this.scoreG+(float)neighbour.tile.cost;
						neighbour.scoreH=Vector3.Distance(neighbour.tile.GetPos(), targetTile.GetPos());
						neighbour.UpdateScoreF();
						neighbour.parent=tile;

					}
					//if the neighbour state is open (it has been evaluated and added to the open list)
					else if(neighbour.listState==_AStarListState.Open){
						//calculate if the path if using this neighbour node through current node would be shorter compare to previous assigned parent node
						float tempScoreG=scoreG+(float)tile.cost;
						float tempScoreF=tempScoreG + neighbour.scoreH;
						if(neighbour.scoreF>tempScoreF){
							//if so, update the corresponding score and and reassigned parent
							neighbour.parent=tile;
							neighbour.scoreG=tempScoreG;
							neighbour.UpdateScoreF();
						}
					}
				}
			}
		}

		public void ProcessNeighbour(Tile targetTile){
			for(int i=0; i<neighbourList.Count; i++){
				TileAStar neighbour=neighbourList[i].aStar;
				//if the neightbour state is clean (never evaluated so far in the search)
				if(neighbour.listState==_AStarListState.Unassigned){
					//check the score of G and H and update F, also assign the parent to currentNode
					neighbour.scoreG=this.scoreG+1;
					neighbour.scoreH=Vector3.Distance(neighbour.tile.GetPos(), targetTile.GetPos());
					neighbour.UpdateScoreF();
					neighbour.parent=tile;

				}
				//if the neighbour state is open (it has been evaluated and added to the open list)
				else if(neighbour.listState==_AStarListState.Open){
					//calculate if the path if using this neighbour node through current node would be shorter compare to previous assigned parent node
					float tempScoreG=scoreG+1;
					float tempScoreF=tempScoreG + neighbour.scoreH;
					if(neighbour.scoreF>tempScoreF){
						//if so, update the corresponding score and and reassigned parent
						neighbour.parent=tile;
						neighbour.scoreG=tempScoreG;
						neighbour.UpdateScoreF();
					}
				}
			}
		}
		public void UpdateScoreF(){ scoreF=scoreG+scoreH; }
	}
	
}
