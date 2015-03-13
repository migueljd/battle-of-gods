using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Collections;
using System.Collections.Generic;

using TBTK;

namespace TBTK{
	
	
	public enum _TileType{Hex, Square}
	public enum _TileState{Default, Selected, Walkable, Hostile, Range}
	public enum _TileCost{Grass=1, DenseGrass=2}


	[System.Serializable]
	public class Wall{
		public bool init=false;
		
		public Tile neighbour;
		public Transform wallObjT;
		public float angle=90;
		
		public Wall(float ang, Transform wallT){ angle=ang; wallObjT=wallT; }
	}
	
	
	public class Tile : MonoBehaviour {
		//-------------------------------------------------These are non TBTK attributes that will be used by the map generator
		public int tileNumber;

		public Transform enemyType;


		//0 means none have been revealed
		//1 means the first, on a clockwise notion, has been revealed
		//2 means the second, on a clockwise notion, has been revealed
		//3 means all adjacent tiles have been revealed
		public int revealed = 0;

		public Tile tile0;


		//-------------------------------------------------End of non TBTK variables


		public float tileDefense;
		public float tileAttack;


		public _TileType type;
		public bool walkable=true;
		private bool visible=true;
		public bool IsVisible(){ return visible; }
		
		public _TileState state=_TileState.Default;
		public _TileState GetState(){ return state; }
		
		public List<CoverSystem.Cover> coverList=new List<CoverSystem.Cover>();
		
		public Unit unit=null;
		
		public int spawnAreaID=-1;	//factionID of units that can be generated on the tile, -1 means the tile is close
		public int deployAreaID=-1;	//factionID of units that can be deploy on the tile, -1 means the tile is close
		
		//coordiate data for hex-tile
		public float x=0;
		public float y=0;
		public float z=0;
		
		public TileAStar aStar;
		public List<Tile> GetNeighbourList(bool walkableOnly=false){ return aStar.GetNeighbourList(walkableOnly); }
		
//		[HideInInspector] 
		public int distance=0;	//for when the tile is in walkableTileList for the selected unit, indicate the distance from selected unit

		public _TileCost cost = _TileCost.Grass;

		/*	//path-smoothing, not in used
		public List<Vector3> path=new List<Vector3>();
		public void ResetPath(){ path=new List<Vector3>{ GetPos() }; }
		public List<Vector3> GetPath(){ 
			if(path.Count==0) ResetPath();
			return path;
		}
		*/
		
		
		private Transform thisT;
		//private Vector3 pos;	//no longer in use
		
		
		public void Init(){
			thisT=transform;
		}


		public void setTileAttributes(){
			if (this.tileAttack == 0){
				switch(this.cost){
					case _TileCost.Grass:
						this.tileAttack = 1f;
						this.tileDefense = 1f;
						break;
					case _TileCost.DenseGrass:
						this.tileAttack = .6f;
						this.tileDefense = 1.3f;
						break;
				}
			}
		}

		
		
		
		
		
		//disable in mobile so it wont interfere with touch input
		#if !UNITY_IPHONE && !UNITY_ANDROID
			//function called when mouse cursor enter the area of the tile, default MonoBehaviour method
			void OnMouseEnter(){ OnTouchMouseEnter();	}
			//function called when mouse cursor leave the area of the tile, default MonoBehaviour method
			void OnMouseExit(){ OnTouchMouseExit(); }
			//function called when mouse cursor enter the area of the tile, default MonoBehaviour method
			public void OnMouseDown(){ GridManager.OnTileCursorDown(this); }
			
			//onMouseDown for right click
			//function called when mouse cursor enter the area of the tile, default MonoBehaviour method
			//used to detech right mouse click on the tile
			/*
			void OnMouseOver(){
				if(Input.GetMouseButtonDown(1)) OnRightClick();
			}
			public void OnRightClick(){
				
			}
			*/

		#endif
		
		
		//for when using inividual tile collider
		public void OnTouchMouseEnter(){
			GridManager.NewHoveredTile(this);
		}
		public void OnTouchMouseExit(){
			GridManager.ClearHoveredTile();
		}
		
		
		//code execution for when a left mouse click happen on a tile
		public void OnTouchMouseDown(){
			Debug.Log (string.Format("is it clear to proceed? {0}", TurnControl.ClearToProceed()));
			if(!TurnControl.ClearToProceed()) return;
			Debug.Log (string.Format("is it clear to proceed? {0}", TurnControl.ClearToProceed()));
			if(GameControl.GetGamePhase()==_GamePhase.Over) return;
			Debug.Log (string.Format("is it clear to proceed? {0}", TurnControl.ClearToProceed()));
			if(GameControl.GetGamePhase()==_GamePhase.UnitDeployment){
				if(unit==null) FactionManager.DeployUnitOnTile(this);
				else if(unit!=null) FactionManager.UndeployUnit(unit);
				return;
			}
			Debug.Log (string.Format("is it clear to proceed? {0}", TurnControl.ClearToProceed()));
			GridManager.OnTile(this);
		}
		public void OnTouchMouseDownAlt(){
			GridManager.OnTileAlt(this);
		}
		
		
		public void Select(){ SetState(_TileState.Selected); }
		
		public void SetState(_TileState tileState){
			state=tileState;
			
			//if(!visible) return;
			
			if(obstacleT!=null) return;
			
			if(!walkable){
				GetComponent<Renderer>().material=GridManager.GetMatUnwalkable();
				gameObject.SetActive(false);
				return;
			}
			else{
				gameObject.SetActive(true);
			}
			
			if(state==_TileState.Default){ 
				if(visible) GetComponent<Renderer>().material=GridManager.GetMatNormal();
				else SetVisible(visible);
			}
			else if(state==_TileState.Selected) GetComponent<Renderer>().material=GridManager.GetMatSelected();
			else if(state==_TileState.Walkable) GetComponent<Renderer>().material=GridManager.GetMatWalkable();
			else if(state==_TileState.Hostile) GetComponent<Renderer>().material=GridManager.GetMatHostile();
			else if(state==_TileState.Range) GetComponent<Renderer>().material=GridManager.GetMatRange();
			
			//if(Application.isPlaying){
				//if(state==_TileState.Default) renderer.enabled=false;
				//else renderer.enabled=true;
				//renderer.enabled=true;
			//}
		}
		
		//used in ability target mode to assign a material directly without changing state
		public void SetMaterial(Material mat){
			GetComponent<Renderer>().material=mat;
			GetComponent<Renderer>().enabled=true;
		}
		
		
		
		
		//stored for the current selected unit, or for AI algorithm
		//[HideInInspector] 
		public List<Tile> hostileInRangeList=new List<Tile>();
		public void SetHostileInRange(List<Tile> list){ hostileInRangeList=list; }
		public void ClearHostileInRange(){ hostileInRangeList=new List<Tile>(); }
		public List<Tile> GetHostileInRange(){ return hostileInRangeList; }
		public List<Tile> SetupHostileInRange(Unit srcUnit=null){
			hostileInRangeList=new List<Tile>();
			
			if(srcUnit==null) srcUnit=unit;
			if(srcUnit!=null) GridManager.SetupHostileInRangeforTile(srcUnit, this);//unit.tile);
			
			return hostileInRangeList;
		}
		
		
		
		//********************************************************************************************************************************
		//these section are related to FogOfWar
		
		//to set the visibility of the tile after checking fog-of-war
		public void SetVisible(bool flag){
			if(forcedVisible.duration>0 && !flag) return;
			
			visible=flag;
			if(!flag){
				GetComponent<Renderer>().material=GridManager.GetMatInvisible();
				if(unit!=null){
					unit.gameObject.layer=LayerManager.GetLayerUnitInvisible();
					Utilities.SetLayerRecursively(unit.transform, LayerManager.GetLayerUnitInvisible());
				}
			}
			else{
				SetState(state);
				
				if(unit!=null){
					unit.gameObject.layer=LayerManager.GetLayerUnit();
					Utilities.SetLayerRecursively(unit.transform, LayerManager.GetLayerUnit());
				}
			}
		}
		
		//for ability which reveal fog-of-war for certain duration
		public DurationCounter forcedVisible=new DurationCounter();
		public void ForceVisible(int dur=1){
			if(!GameControl.EnableFogOfWar()) return;
			
			SetVisible(true);
			forcedVisible.Count(dur);
			GameControl.onIterateTurnE += IterateForcedVisible;
		}
		public void IterateForcedVisible(){
			forcedVisible.Iterate();
			if(forcedVisible.duration<=0){
				SetVisible(FogOfWar.CheckTileVisibility(this));
				GameControl.onIterateTurnE -= IterateForcedVisible;
			}
		}
		
		//end FogOfWar section
		//********************************************************************************************************************************
		
		
		
		
		
		public Vector3 GetPos(){ return thisT==null ? transform.position : thisT.position; }
		
		public Tile GetNeighbourFromAngle(float angle){
			List<Tile> neighbourList=aStar.GetNeighbourList();
			for(int n=0; n<neighbourList.Count; n++){
				Vector3 dir=neighbourList[n].GetPos()-GetPos();
				float angleN=Utilities.Vector2ToAngle(new Vector2(dir.x, dir.z));
				if(Mathf.Abs(angle-angleN)<2) return neighbourList[n];
			}
			return null;
		}
		
		
		
		
		
		
		
		
		[HideInInspector] public int hostileCount=0;
		[HideInInspector] public int coverScore=0;
		//CoverRating: 0-no cover at all, 1-halfcover, 2-fullcover
		public float GetCoverRating(){ return hostileCount>0 ? (float)coverScore/(float)hostileCount : 0 ; }
	
		
		
		
		//********************************************************************************************************************************
		//these section are related to obstacle and wall
		
		public Transform obstacleT;
		public bool HasObstacle(){ return obstacleT==null ? false : true ; }
		
		public void AddObstacle(int obsType){
			if(obstacleT!=null){
				if(obstacleT.gameObject.layer==LayerManager.GetLayerObstacleHalfCover() && obsType==1) return;
				if(obstacleT.gameObject.layer==LayerManager.GetLayerObstacleFullCover() && obsType==2) return;
				
				DestroyImmediate(obstacleT.gameObject);
			}
			
			if(wallList.Count>0){
				if(!Application.isPlaying){
					Grid grid=GridManager.GetInstance().GetGrid();
					while(wallList.Count>0) RemoveWall(wallList[0].angle, grid.GetNeighbourInDir(this, wallList[0].angle));
				}
				else{
					while(wallList.Count>0) RemoveWall(wallList[0].angle, GetNeighbourFromAngle(wallList[0].angle));
				}
			}
			
			float gridSize=GridManager.GetTileSize();
			
			#if UNITY_EDITOR
				Transform obsT=(Transform)PrefabUtility.InstantiatePrefab(GridManager.GetObstacleT(obsType));
			#else
				Transform obsT=(Transform)Instantiate(GridManager.GetObstacleT(obsType));
			#endif
			
			float offsetY=0;
			if(type==_TileType.Square){
				if(obsType==1)offsetY=obsT.localScale.z*gridSize/4;
				if(obsType==2)offsetY=obsT.localScale.z*gridSize/2;
			}
			else if(type==_TileType.Hex) offsetY=obsT.localScale.z*gridSize/4;
			
			obsT.position=GetPos()+new Vector3(0, offsetY, 0);
			
			obsT.localScale*=gridSize;
			obsT.parent=transform;
			
			obstacleT=obsT;
			walkable=false;
			
			GetComponent<Renderer>().enabled=false;
			
			//SetState(_TileState.Default);
		}
		public void RemoveObstacle(){
			if(obstacleT!=null) DestroyImmediate(obstacleT.gameObject);
			
			walkable=true;
			SetState(_TileState.Default);
			
			GetComponent<Renderer>().enabled=true;
		}
		
		
		
		
		public List<Wall> wallList=new List<Wall>();
		
		//used in edit mode only
		public void AddWall(float angle, Tile neighbour, int wallType=0){
			if(neighbour==null) return;
			
			if(angle>360) angle-=360;
			
			if(IsWalled(angle)) return;
			
			float gridSize=GridManager.GetTileSize();
			if(type==_TileType.Square) gridSize*=2;
			
			#if UNITY_EDITOR
				Transform wallT=(Transform)PrefabUtility.InstantiatePrefab(GridManager.GetWallObstacleT(wallType));
			#else
				Transform wallT=(Transform)Instantiate(GridManager.GetWallObstacleT(wallType));
			#endif
			
			float wallTAngle=angle+90;
			
			if(type==_TileType.Square) wallTAngle=360-(angle-90);
			else if(type==_TileType.Hex) wallTAngle=360-(angle-90);
			
			wallT.rotation=Quaternion.Euler(0, wallTAngle, 0);
			wallT.position=(GetPos()+neighbour.GetPos())/2+new Vector3(0, wallT.localScale.y*gridSize/2, 0);
			wallT.localScale=new Vector3(wallT.localScale.x*gridSize, wallT.localScale.y*gridSize, wallT.localScale.z);
			wallT.parent=transform;
			
			wallList.Add(new Wall(angle, wallT));
			
			if((angle+=180)>360) angle-=360;
			neighbour.wallList.Add(new Wall(angle, wallT));
			
			if(Application.isPlaying){
				//CreateWall(neighbour, angle);
				//neighbour.CreateWall(this, angle+180);
			}
		}
		public void RemoveWall(float angle, Tile neighbour){
			if(angle>360) angle-=360;
			for(int i=0; i<wallList.Count; i++){ 
				if(wallList[i].angle==angle){
					DestroyImmediate(wallList[i].wallObjT.gameObject);
					wallList.RemoveAt(i);
					break;
				}
			}
			
			if((angle+=180)>360) angle-=360;
			for(int i=0; i<neighbour.wallList.Count; i++){ 
				if(neighbour.wallList[i].angle==angle){
					neighbour.wallList.RemoveAt(i);
					break;
				}
			}
		}
		
		
		public bool IsWalled(float angle){
			for(int i=0; i<wallList.Count; i++){ if(wallList[i].angle==angle) return true; }
			return false;
		}
		
		//called during grid initiation
		public void InitWall(){
			if(wallList.Count==0) return;
			
			for(int i=0; i<wallList.Count; i++){
				Wall wall=wallList[i];
				
				if(wall.init) continue;
				
				Tile neighbour=GetNeighbourFromAngle(wall.angle);
				if(neighbour!=null){
					wall.init=true;
					wall.neighbour=neighbour;
					aStar.DisconnectNeighbour(neighbour);
					neighbour.CreateWall(this, wall.angle+180);
				}
			}
		}
		//call by other tile in InitWall to create a wall instance, to avoid duplication or running the same code twice
		public void CreateWall(Tile neighbour, float angle){
			if(angle>360) angle-=360;
			for(int i=0; i<wallList.Count; i++){
				Wall wall=wallList[i];
				if(wall.angle==angle){
					wall.init=true;
					wall.neighbour=neighbour;
					aStar.DisconnectNeighbour(neighbour);
					break;
				}
			}
		}
		
		//end obstacle and wall related function
		//*********************************************************************************************************************************
		
		
		
		
		
		
		
		
		//********************************************************************************************************************************
		//these section are related to effects on tile
		
		public List<Effect> effectList=new List<Effect>();
		public void ApplyEffect(Effect eff){ 
			new TextOverlay(GetPos(), eff.name, Color.white);
			
			if(eff.duration>0){
				effectList.Add(eff);
				eff.StartDurationCount();
				
				EffectTracker.AddTileWithEffect(this);
			}
		}
		public void ProcessEffectList(){
			new TextOverlay(GetPos(), effectList[0].duration.ToString(), Color.white);
			
			for(int i=0; i<effectList.Count; i++){
				effectList[i].IterateDuration();
				
				if(effectList[i].EffectExpired()){
					effectList.RemoveAt(i);
					i-=1;
				}
			}
			
			if(effectList.Count==0) EffectTracker.RemoveTileWithEffect(this);
		}
		
		
		public int GetAttackRange(){
			int value=0;
			for(int i=0; i<effectList.Count; i++) value+=effectList[i].unitStat.attackRange;
			return value;
		}
		
		public float GetDamage(){
			float value=0;
			for(int i=0; i<effectList.Count; i++) value+=effectList[i].unitStat.damage;
			return value;
		}
		
		public float GetHitChance(){
			float value=0;
			for(int i=0; i<effectList.Count; i++) value+=effectList[i].unitStat.hitChance;
			return value;
		}
		public float GetDodgeChance(){
			float value=0;
			for(int i=0; i<effectList.Count; i++) value+=effectList[i].unitStat.dodgeChance;
			return value;
		}
		
		public float GetCritChance(){
			float value=0;
			for(int i=0; i<effectList.Count; i++) value+=effectList[i].unitStat.critChance;
			return value;
		}
		public float GetCritAvoidance(){
			float value=0;
			for(int i=0; i<effectList.Count; i++) value+=effectList[i].unitStat.critAvoidance;
			return value;
		}
		public float GetCritMultiplier(){
			float value=0;
			for(int i=0; i<effectList.Count; i++) value+=effectList[i].unitStat.critMultiplier;
			return value;
		}
		
		public float GetStunChance(){
			float value=0;
			for(int i=0; i<effectList.Count; i++) value+=effectList[i].unitStat.stunChance;
			return value;
		}
		public float GetStunAvoidance(){
			float value=0;
			for(int i=0; i<effectList.Count; i++) value+=effectList[i].unitStat.stunAvoidance;
			return value;
		}
		public int GetStunDuration(){
			int value=0;
			for(int i=0; i<effectList.Count; i++) value+=effectList[i].unitStat.stunDuration;
			return value;
		}
		
		public float GetSilentChance(){
			float value=0;
			for(int i=0; i<effectList.Count; i++) value+=effectList[i].unitStat.silentChance;
			return value;
		}
		public float GetSilentAvoidance(){
			float value=0;
			for(int i=0; i<effectList.Count; i++) value+=effectList[i].unitStat.silentAvoidance;
			return value;
		}
		public int GetSilentDuration(){
			int value=0;
			for(int i=0; i<effectList.Count; i++) value+=effectList[i].unitStat.silentDuration;
			return value;
		}
		
		public float GetHPPerTurn(){
			float value=0;
			for(int i=0; i<effectList.Count; i++) value+=effectList[i].unitStat.HPPerTurn;
			return value;
		}
		public float GetAPPerTurn(){
			float value=0;
			for(int i=0; i<effectList.Count; i++) value+=effectList[i].unitStat.APPerTurn;
			return value;
		}
		
		public int GetSight(){
			int value=0;
			for(int i=0; i<effectList.Count; i++) value+=effectList[i].unitStat.sight;
			return value;
		}
		
		
		//end effect related function
		//*********************************************************************************************************************************
		
	}
	
}