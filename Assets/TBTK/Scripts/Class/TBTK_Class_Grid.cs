using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using TBTK;

namespace TBTK{

	[System.Serializable]
	public class Grid {

		public GameObject gridObj;
		public List<Tile> tileList=new List<Tile>();
		
		public List<Transform> tileTList=new List<Transform>();
		
		//used in editor only
		public Tile GetNeighbourInDir(Tile tile, float angle){
			int ID=tileList.IndexOf(tile);
			
			GridManager gridManager=GridManager.GetInstance();
			
			if(gridManager.tileType==_TileType.Square){
				
				float distTH=gridManager.tileSize*gridManager.gridToTileRatio*1.1f;
				
				if(angle==90){
					if(ID+1<tileList.Count && Vector3.Distance(tile.GetPos(), tileList[ID+1].GetPos())<=distTH) return tileList[ID+1];
				}
				else if(angle==180){	
					ID-=gridManager.length;
					if(ID>=0 && Vector3.Distance(tile.GetPos(), tileList[ID].GetPos())<=distTH) return tileList[ID];
				}
				else if(angle==270){	
					if(ID-1>=0 && Vector3.Distance(tile.GetPos(), tileList[ID-1].GetPos())<=distTH) return tileList[ID-1];
				}
				else if(angle==0){	
					ID+=gridManager.length;
					if(ID<tileList.Count && Vector3.Distance(tile.GetPos(), tileList[ID].GetPos())<=distTH) return tileList[ID];
				}
			}
			else if(gridManager.tileType==_TileType.Hex){
				
				float distTH=GridGenerator.spaceZHex*gridManager.tileSize*gridManager.gridToTileRatio*1.1f;
				
				if(angle==90){
					if(ID+1<tileList.Count && Vector3.Distance(tile.GetPos(), tileList[ID+1].GetPos())<=distTH) return tileList[ID+1];
				}
				else if(angle==150){
					ID-=gridManager.length-1;
					Debug.Log(ID);
					if(ID>=0 && Vector3.Distance(tile.GetPos(), tileList[ID].GetPos())<=distTH) return tileList[ID];
				}
				else if(angle==210){
					ID-=gridManager.length;
					if(ID>=0 && Vector3.Distance(tile.GetPos(), tileList[ID].GetPos())<=distTH) return tileList[ID];
				}
				else if(angle==270){
					if(ID-1>0 && Vector3.Distance(tile.GetPos(), tileList[ID-1].GetPos())<=distTH) return tileList[ID-1];
				}
				else if(angle==330){
					ID+=gridManager.length-1;
					if(ID<tileList.Count && Vector3.Distance(tile.GetPos(), tileList[ID].GetPos())<=distTH) return tileList[ID];
				}
				else if(angle==30){
					ID+=gridManager.length;
					if(ID<tileList.Count && Vector3.Distance(tile.GetPos(), tileList[ID].GetPos())<=distTH) return tileList[ID];
				}
			}
			
			return null;
		}
		
		public void Init(){
			for(int i=0; i<tileList.Count; i++) tileList[i].Init();
			
			GridManager gridManager=GridManager.GetInstance();
			
			//setup the neighbour of each tile
			if(gridManager.tileType==_TileType.Square){
				float distTH=gridManager.tileSize*gridManager.gridToTileRatio*1.1f;
				for(int i=0; i<tileList.Count; i++){
					Tile tile=tileList[i];
					tile.aStar=new TileAStar(tile);
					
					List<Tile> neighbourList=new List<Tile>();
					
					int nID=i+1;
					if(nID<tileList.Count && Vector3.Distance(tile.GetPos(), tileList[nID].GetPos())<=distTH)
						neighbourList.Add(tileList[nID]);
					
					nID=i+gridManager.length;
					if(nID<tileList.Count && Vector3.Distance(tile.GetPos(), tileList[nID].GetPos())<=distTH)
						neighbourList.Add(tileList[nID]);
					
					nID=i-1;
					if(nID>=0 && Vector3.Distance(tile.GetPos(), tileList[nID].GetPos())<=distTH)
						neighbourList.Add(tileList[nID]);
					
					nID=i-gridManager.length;
					if(nID>=0 && Vector3.Distance(tile.GetPos(), tileList[nID].GetPos())<=distTH)
						neighbourList.Add(tileList[nID]);
					
					
					//diagonal neighbour, not in used  
					if(GridManager.EnableDiagonalNeighbour()){
						nID=(i+1)+gridManager.length;
						if(nID<tileList.Count && Vector3.Distance(tile.GetPos(), tileList[nID].GetPos())<=distTH*1.5f)
							neighbourList.Add(tileList[nID]);
						
						nID=(i+1)-gridManager.length;
						if(nID>=0 && Vector3.Distance(tile.GetPos(), tileList[nID].GetPos())<=distTH*1.5f)
							neighbourList.Add(tileList[nID]);
						
						nID=(i-1)+gridManager.length;
						if(nID<tileList.Count && Vector3.Distance(tile.GetPos(), tileList[nID].GetPos())<=distTH*1.5f)
							neighbourList.Add(tileList[nID]);
						
						nID=(i-1)-gridManager.length;
						if(nID>=0 && Vector3.Distance(tile.GetPos(), tileList[nID].GetPos())<=distTH*1.5f)
							neighbourList.Add(tileList[nID]);
					}
					
					
					tile.aStar.SetNeighbourList(neighbourList);
				}
			}
			else if(gridManager.tileType==_TileType.Hex){
				float distTH=GridGenerator.spaceZHex*gridManager.tileSize*gridManager.gridToTileRatio*1.1f;
				for(int i=0; i<tileList.Count; i++){
					Tile tile=tileList[i];
					tile.aStar=new TileAStar(tile);
					
					List<Tile> neighbourList=new List<Tile>();
					
					int nID=i+1;		
					if(nID<tileList.Count && Vector3.Distance(tile.GetPos(), tileList[nID].GetPos())<=distTH)
						neighbourList.Add(tileList[nID]);
					
					nID=i+gridManager.length;
					if(nID<tileList.Count && Vector3.Distance(tile.GetPos(), tileList[nID].GetPos())<=distTH)
						neighbourList.Add(tileList[nID]);
					
					nID=i+gridManager.length-1;
					if(nID<tileList.Count && Vector3.Distance(tile.GetPos(), tileList[nID].GetPos())<=distTH)
						neighbourList.Add(tileList[nID]);
					
					nID=i-1;
					if(nID>=0 && Vector3.Distance(tile.GetPos(), tileList[nID].GetPos())<=distTH)
						neighbourList.Add(tileList[nID]);
					
					nID=i-gridManager.length;
					if(nID>=0 && Vector3.Distance(tile.GetPos(), tileList[nID].GetPos())<=distTH)
						neighbourList.Add(tileList[nID]);
					
					nID=i-gridManager.length+1;
					if(nID>=0 && Vector3.Distance(tile.GetPos(), tileList[nID].GetPos())<=distTH)
						neighbourList.Add(tileList[nID]);
					
					tile.aStar.SetNeighbourList(neighbourList);
				}
			}
			
			//setup the wall
			for(int i=0; i<tileList.Count; i++){
				tileList[i].InitWall();
			}
			
			//setup the cover
			if(GameControl.EnableCover()){
				for(int i=0; i<tileList.Count; i++) CoverSystem.InitCoverForTile(tileList[i]);
			}
		}
		
		//get all the tiles within certain distance from a given tile
		public List<Tile> GetTilesWithinDistance(Tile srcTile, int dist, bool walkableOnly=false, bool setDistance=false){
			List<Tile> neighbourList=srcTile.GetNeighbourList(walkableOnly);
			
			List<Tile> closeList=new List<Tile>();
			List<Tile> openList=new List<Tile>();
			List<Tile> newOpenList=new List<Tile>();
			
			for(int m=0; m<neighbourList.Count; m++){
				Tile neighbour=neighbourList[m];
				if(!newOpenList.Contains(neighbour)) newOpenList.Add(neighbour);
			}
			
			for(int i=0; i<dist; i++){
				openList=newOpenList;
				newOpenList=new List<Tile>();
				
				for(int n=0; n<openList.Count; n++){
					neighbourList=openList[n].GetNeighbourList(walkableOnly);
					for(int m=0; m<neighbourList.Count; m++){
						Tile neighbour=neighbourList[m];
						if(!closeList.Contains(neighbour) && !openList.Contains(neighbour) && !newOpenList.Contains(neighbour)){
							newOpenList.Add(neighbour);
						}
					}
				}
				
				for(int n=0; n<openList.Count; n++){
					Tile tile=openList[n];
					if(tile!=srcTile && !closeList.Contains(tile)){
						closeList.Add(tile);
						if(setDistance) tile.distance=i+1;
					}
				}
			}
			
			return closeList;
		}
		
		//get a direct distance, regardless of the walkable state
		public int GetDistance(Tile tile1, Tile tile2){
			if(GridManager.GetTileType()==_TileType.Hex){
				float x=Mathf.Abs(tile1.x-tile2.x);
				float y=Mathf.Abs(tile1.y-tile2.y);
				float z=Mathf.Abs(tile1.z-tile2.z);
				return (int)((x + y + z)/2);
			}
			else{
				float tileSize=GridManager.GetTileSize()*GridManager.GetGridToTileSizeRatio();
				Vector3 pos1=tile1.GetPos();
				Vector3 pos2=tile2.GetPos();
				return (int)(Mathf.Abs((pos1.x-pos2.x)/tileSize)+Mathf.Abs((pos1.z-pos2.z)/tileSize));
			}
		}
		
		//get distance when restricted to walkable tile, using A*
		public int GetWalkableDistance(Tile tile1, Tile tile2){
			return AStar.GetDistance(tile1, tile2);
		}
		
		public void ClearGrid(){
			MonoBehaviour.DestroyImmediate(gridObj);
			tileList=new List<Tile>();
			//for(int i=0; i<tileList.Count; i++) MonoBehaviour.DestroyImmediate(tileList[i].gameObject);
		}
		
	}

}