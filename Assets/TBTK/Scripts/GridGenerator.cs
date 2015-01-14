using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using TBTK;

namespace TBTK{

	public class GridGenerator : MonoBehaviour {

		public static float spaceXHex=0.75f;
		public static float spaceZHex=0.86603f;
		
		private static Quaternion rot=Quaternion.Euler(-90, 0, 0);
		
		
		
		public static Grid GenerateSquareGrid(int width=5, int length=5, float gridSize=1, float GridToTileRatio=1, float unwalkableRate=0, GridManager._GridColliderType colType=GridManager._GridColliderType.Master){
			
			string loadText="";
			if(colType==GridManager._GridColliderType.Individual) loadText="ScenePrefab/SquareTile_Collider";
			else if(colType==GridManager._GridColliderType.Master) loadText="ScenePrefab/SquareTile";
			Transform tilePrefab=Resources.Load(loadText, typeof(Transform)) as Transform;
			
			List<Transform> tileTransformList=new List<Transform>();
			
			Vector3 pos=Vector3.zero;
			
			Transform parentT=new GameObject("GridTemp").transform;
			
			float spaceX=gridSize*GridToTileRatio;
			float spaceZ=gridSize*GridToTileRatio;
			
			Grid grid=new Grid();
			
			for(int i=0; i<width; i++){
				for(int n=0; n<length; n++){
					pos=new Vector3(i*spaceX, 0, n*spaceZ);
					Transform tileT=(Transform)Instantiate(tilePrefab, pos, rot);
					tileTransformList.Add(tileT);
					
					tileT.localScale*=gridSize;
					tileT.parent=parentT;
					
					#if UNITY_EDITOR
						tileT.name="Tile_"+i+"x"+n;
					#endif
					
					
					tileT.gameObject.layer=LayerManager.GetLayerTile();
					Tile tile=tileT.gameObject.AddComponent<Tile>();
					tile.type=_TileType.Square;
					
					
					tile.x=i;
					tile.y=0;
					tile.z=n;
					
					grid.tileList.Add(tile);
				}
			}
			
			if(unwalkableRate>0){
				int unwalkableCount=(int)(grid.tileList.Count*unwalkableRate);
				for(int i=0; i<unwalkableCount; i++){
					int rand=Random.Range(0, grid.tileList.Count);
					grid.tileList[rand].walkable=false;
					grid.tileList[rand].SetState(_TileState.Default);
				}
			}
			
			float x=(width-1)*spaceX; 	float z=(length-1)*spaceZ;
			parentT.position-=new Vector3(x/2, 0, z/2);
			
			Transform newParentT=new GameObject("SquareGrid").transform;
			for(int i=0; i<tileTransformList.Count; i++) tileTransformList[i].parent=newParentT;
			
			DestroyImmediate(parentT.gameObject);
			
			grid.gridObj=newParentT.gameObject;
			//newParentT.gameObject.AddComponent<CombineMesh>();
			
			newParentT.gameObject.layer=LayerManager.GetLayerTile();
			
			if(colType==GridManager._GridColliderType.Master){
				BoxCollider boxCol=newParentT.gameObject.AddComponent<BoxCollider>();
				boxCol.size=new Vector3(5000, 0, 5000);
			}
			
			return grid;
			
		}
		
		
		
		public static Grid GenerateHexGrid(int width=5, int length=5, float gridSize=1, float GridToTileRatio=1, float unwalkableRate=0, GridManager._GridColliderType colType=GridManager._GridColliderType.Master){
			
			string loadText="";
			if(colType==GridManager._GridColliderType.Individual) loadText="ScenePrefab/HexTile_Collider";
			else if(colType==GridManager._GridColliderType.Master) loadText="ScenePrefab/HexTile";
			Transform tilePrefab=Resources.Load(loadText, typeof(Transform)) as Transform;
			
			List<Transform> tileTransformList=new List<Transform>();
			
			Vector3 pos=Vector3.zero;
			
			Transform parentT=new GameObject("GridTemp").transform;
			
			float spaceX=spaceXHex*gridSize*GridToTileRatio;
			float spaceZ=spaceZHex*gridSize*GridToTileRatio;
			
			Grid grid=new Grid();
			
			for(int i=0; i<width; i++){
				
				float offsetZ=(i%2==1) ? 0 : (spaceZ/2);
				
				int limit=i%2==1 ? length : length-1;
				//int limit=length;
				
				for(int n=0; n<limit; n++){
					pos=new Vector3(i*spaceX, 0, n*spaceZ+offsetZ);
					Transform tileT=(Transform)Instantiate(tilePrefab, pos, rot);
					tileTransformList.Add(tileT);
					
					tileT.localScale*=gridSize;
					tileT.parent=parentT;
					
					#if UNITY_EDITOR
						tileT.name="Tile_"+i+"x"+n;
					#endif
					
					
					tileT.gameObject.layer=LayerManager.GetLayerTile();
					Tile tile=tileT.gameObject.AddComponent<Tile>();
					tile.type=_TileType.Hex;
					
					
					tile.x=i;
					tile.y=-((i+1)/2)+n;
					tile.z=-(i/2)-n;
					
					grid.tileList.Add(tile);
				}
			}
			
			if(unwalkableRate>0){
				int unwalkableCount=(int)(grid.tileList.Count*unwalkableRate);
				for(int i=0; i<unwalkableCount; i++){
					int rand=Random.Range(0, grid.tileList.Count);
					grid.tileList[rand].walkable=false;
					grid.tileList[rand].SetState(_TileState.Default);
				}
			}
			
			
			float x=(width-1)*spaceX; 	float z=(length-1)*spaceZ;
			parentT.position-=new Vector3(x/2, 0, z/2);
			
			Transform newParentT=new GameObject("HexGrid").transform;
			for(int i=0; i<tileTransformList.Count; i++) tileTransformList[i].parent=newParentT;
			
			DestroyImmediate(parentT.gameObject);
			
			grid.gridObj=newParentT.gameObject;
			//newParentT.gameObject.AddComponent<CombineMesh>();
			
			newParentT.gameObject.layer=LayerManager.GetLayerTile();
			
			if(colType==GridManager._GridColliderType.Master){
				BoxCollider boxCol=newParentT.gameObject.AddComponent<BoxCollider>();
				boxCol.size=new Vector3(5000, 0, 5000);
			}
			
			return grid;
			
		}
		
		
	
	}
	
}



