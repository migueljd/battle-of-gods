using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using TBTK;

namespace TBTK{

	public class GridManager : MonoBehaviour {
		
		public delegate void HoverAttackableHandler(Tile tile);
		public static event HoverAttackableHandler onHoverAttackableTileE;		//listen by UI only
		
		public delegate void ExitAttackableHandler();
		public static event ExitAttackableHandler onExitAttackableTileE;			//listen by UI only
		
		public delegate void HoverWalkableHandler(Tile tile);
		public static event HoverWalkableHandler onHoverWalkableTileE;		//listen by UI only
		
		public delegate void ExitWalkableHandler();
		public static event ExitWalkableHandler onExitWalkableTileE;			//listen by UI only
		
		
		public bool generateGridOnStart=false;
		
		public _TileType tileType=_TileType.Hex;
		public static _TileType GetTileType(){ return instance.tileType; }
		
		public enum _GridColliderType{ Master, Individual }
		public _GridColliderType gridColliderType=_GridColliderType.Master;

		public int width=5;
		public int length=5;
		public float tileSize=1;
		public static float GetTileSize(){ return instance.tileSize; }
		
		public float gridToTileRatio=1;
		public static float GetGridToTileSizeRatio(){ return instance.gridToTileRatio; }
		
		public float unwalkableRate=0;
		
		//not visible on inspector and not in use for most part, the game might break if this is enabled
		public bool enableDiagonalNeighbour=false;
		public static bool EnableDiagonalNeighbour(){ return instance.enableDiagonalNeighbour; }
		
		
		//the prefab for obstacle
		public Transform obstacleWallH;
		public Transform obstacleWallF;
		public Transform obstacleHexF;
		public Transform obstacleHexH;
		public Transform obstacleSqF;
		public Transform obstacleSqH;
		public static Transform GetWallObstacleT(int type=1){ //1-half, 2-full
			return type==1 ? instance.obstacleWallH : instance.obstacleWallF ;
		}
		public static Transform GetObstacleT(int type=1){
			if(instance.tileType==_TileType.Hex) return type==1 ? instance.obstacleHexH : instance.obstacleHexF ;
			if(instance.tileType==_TileType.Square) return type==1 ? instance.obstacleSqH : instance.obstacleSqF ;
			return null;
		}
		
		
		//the prefab for cursor and indicators
		public Transform hexCursor;
		public Transform hexSelected;
		public Transform hexHostile;
		public Transform sqCursor;
		public Transform sqSelected;
		public Transform sqHostile;
		//active cursor and indicator in used during runtime
		private Transform indicatorCursor;	
		private Transform indicatorSelected;
		private Transform indicatorSelectedConfirmation;
		private List<Transform> indicatorHostileList=new List<Transform>();
		
		
		//on grid overlays for cover
		public List<Transform> coverHOverlayList=new List<Transform>();
		public List<Transform> coverFOverlayList=new List<Transform>();
		
		
		//material for each individual tile
		public Material hexMatNormal;
		public Material hexMatSelected;
		public Material hexMatWalkable;
		public Material hexMatUnwalkable;
		public Material hexMatHostile;
		public Material hexMatRange;
		public Material hexMatAbilityAll;
		public Material hexMatAbilityHostile;
		public Material hexMatAbilityFriendly;
		public Material hexMatInvisible;
		
		public Material sqMatNormal;
		public Material sqMatSelected;
		public Material sqMatWalkable;
		public Material sqMatUnwalkable;
		public Material sqMatHostile;
		public Material sqMatRange;
		public Material sqMatAbilityAll;
		public Material sqMatAbilityHostile;
		public Material sqMatAbilityFriendly;
		public Material sqMatInvisible;
		
		public static Material GetMatNormal(){ 		return instance._GetMatNormal(); }
		public static Material GetMatSelected(){ 	return instance._GetMatSelected(); }
		public static Material GetMatWalkable(){ 	return instance._GetMatWalkable(); }
		public static Material GetMatUnwalkable(){ return instance._GetMatUnwalkable(); }
		public static Material GetMatHostile(){ 		return instance._GetMatHostile(); }
		public static Material GetMatRange(){ 		return instance._GetMatRange(); }
		public static Material GetMatAbilityAll(){ 	return instance._GetMatABAll(); }
		public static Material GetMatAbilityHostile(){ 	return instance._GetMatABHostile(); }
		public static Material GetMatAbilityFriendly(){ return instance._GetMatABFriendly(); }
		public static Material GetMatInvisible(){ 	return instance._GetMatInvisible(); }
		
		public Material _GetMatNormal(){ 		return tileType==_TileType.Hex ? hexMatNormal : sqMatNormal; }
		public Material _GetMatSelected(){ 	return tileType==_TileType.Hex ? hexMatSelected : sqMatSelected; }
		public Material _GetMatWalkable(){ 	return tileType==_TileType.Hex ? hexMatWalkable : sqMatWalkable; }
		public Material _GetMatUnwalkable(){ return tileType==_TileType.Hex ? hexMatUnwalkable : sqMatUnwalkable; }
		public Material _GetMatHostile(){ 		return tileType==_TileType.Hex ? hexMatHostile : sqMatHostile; }
		public Material _GetMatRange(){ 		return tileType==_TileType.Hex ? hexMatRange : sqMatRange; }
		public Material _GetMatABAll(){ 		return tileType==_TileType.Hex ? hexMatAbilityAll : sqMatAbilityAll; }
		public Material _GetMatABHostile(){ 	return tileType==_TileType.Hex ? hexMatAbilityHostile : sqMatAbilityHostile; }
		public Material _GetMatABFriendly(){ 	return tileType==_TileType.Hex ? hexMatAbilityFriendly : sqMatAbilityFriendly; }
		public Material _GetMatInvisible(){ 	return tileType==_TileType.Hex ? hexMatInvisible : sqMatInvisible; }
		
		
		//the grid instance which contains the current grid in scene
		public Grid grid=null;
		public Grid GetGrid(){ return grid; }
		
		//temporarily tile list for selected unit storing attackable and walkable tiles, reset when a new unit is selected
		private List<Tile> walkableTileList=new List<Tile>();
		private List<Tile> attackableTileList=new List<Tile>();
		
		private static GridManager instance;
		public static void SetInstance(){ if(instance==null) instance=(GridManager)FindObjectOfType(typeof(GridManager)); }
		public static GridManager GetInstance(){ return instance; }
		
		void Awake(){
			if(instance==null) instance=this;
		}	
		
		// initiate all the indicators and overlay
		void Start () {
			Transform thisT=transform;
			
			if(tileType==_TileType.Hex){
				indicatorCursor=(Transform)Instantiate(hexCursor);
				indicatorSelected=(Transform)Instantiate(hexSelected);
				indicatorSelectedConfirmation = (Transform)Instantiate(hexSelected);
				for(int i=0; i<10; i++) indicatorHostileList.Add((Transform)Instantiate(hexHostile));
			}
			else if(tileType==_TileType.Square){
				indicatorCursor=(Transform)Instantiate(sqCursor);
				indicatorSelected=(Transform)Instantiate(sqSelected);
				for(int i=0; i<10; i++) indicatorHostileList.Add((Transform)Instantiate(sqHostile));
			}
			
			indicatorCursor.parent=thisT;
			indicatorSelected.parent=thisT;
			indicatorSelectedConfirmation.parent=(Transform)thisT;
			for(int i=0; i<indicatorHostileList.Count; i++) indicatorHostileList[i].parent=thisT;
			
			
			if(GameControl.EnableCover()){
				for(int i=0; i<5; i++) coverHOverlayList.Add((Transform)Instantiate(coverHOverlayList[0]));
				for(int i=0; i<5; i++) coverFOverlayList.Add((Transform)Instantiate(coverFOverlayList[0]));
				
				coverHOverlayList.RemoveAt(0);
				coverFOverlayList.RemoveAt(0);
				
				float scaleOffset=tileType==_TileType.Hex ? 0.5f : 0.8f ;
				for(int i=0; i<coverHOverlayList.Count; i++){
					coverHOverlayList[i].localScale*=tileSize*scaleOffset;
					coverHOverlayList[i].parent=thisT;
				}
				for(int i=0; i<coverFOverlayList.Count; i++){
					coverFOverlayList[i].localScale*=tileSize*scaleOffset;
					coverFOverlayList[i].parent=thisT;
				}
				
				HideCoverOverlay();
			}
			
			HideIndicator();
			
			ClearAllTile();
		}
		
		
		//called by GameControl at the start of a scene
		public void Init(){
			if(instance==null) instance=this;
			
			if(generateGridOnStart) GenerateGrid();
			
			grid.Init();
		}
		
		//called by GameControl to setup the grid for Fog-of-war
		public static void SetupGridForFogOfWar(){ FogOfWar.InitGrid(instance.grid.tileList); }
		
		
		void OnEnable(){
			Unit.onUnitDestroyedE += OnUnitDestroyed;
			
		}
		void OnDisable(){
			Unit.onUnitDestroyedE -= OnUnitDestroyed;
		}
		
		
		void OnUnitDestroyed(Unit unit){
			if(GameControl.selectedUnit==null) return;
			
			Tile tile=unit.tile;
			if(attackableTileList.Contains(tile)) attackableTileList.Remove(tile);	//remove from target tile
			
			int dist=GetDistance(tile, GameControl.selectedUnit.tile, true);
					
			if(dist>0 && dist<GameControl.selectedUnit.GetMoveRange()){	//if within walkable distance, add to walkable tile since the tile is now open
				walkableTileList.Add(tile);
				tile.SetState(_TileState.Walkable);
			}
			else unit.tile.SetState(_TileState.Default);
		}
		
		
		// Update is called once per frame
		private Tile hoveredTile;
		private Vector3 cursorPosition;
		void Update () {
			#if UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8 || UNITY_BLACKBERRY
				if(Input.touchCount==1){
					Touch touch=Input.touches[0];
					
					cursorPosition=touch.position;
					
					//if(touch.phase==TouchPhase.Ended && targetMode) ClearTargetMode();
					if(touch.phase!=TouchPhase.Began) return;
					
					if(UIUtilities.IsCursorOnUI(0)){
						if(hoveredTile!=null) _ClearHoveredTile();
						return;
					}
				}
				else return;
			#else
				cursorPosition=Input.mousePosition;
				
				if(Input.GetMouseButtonDown(1) && targetMode) _ClearTargetMode();
				
				//if the grid uses individual collider on individual tile, then this section is not requred
				if(gridColliderType==_GridColliderType.Individual) return;
				
				if(UIUtilities.IsCursorOnUI()){
					if(hoveredTile!=null) _ClearHoveredTile();
					return;
				}
			#endif
			
			
			//check if the curosr is hover over the grid and show the appropriate indicator
			LayerMask mask=1<<LayerManager.GetLayerTile();
			Ray ray = Camera.main.ScreenPointToRay(cursorPosition);
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit, Mathf.Infinity, mask)){
				Tile newTile=_GetTileOnPos(hit.point);
				if(newTile==null || !newTile.walkable){
					if(hoveredTile!=null) _ClearHoveredTile();
					return;
				}
				else{
					if(hoveredTile!=newTile) _NewHoveredTile(newTile);
					hoveredTile=newTile;
				}
				
				if(FactionManager.IsPlayerTurn() || GameControl.GetGamePhase()==_GamePhase.UnitDeployment){
					#if UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8 || UNITY_BLACKBERRY
						if(currentSelectTile==hoveredTile){
							OnTileTouchDown();
						}
						else{
							if(targetMode || hoveredTile.unit==null || attackableTileList.Contains(hoveredTile)){
								currentSelectTile=hoveredTile;
							}
							else{
								OnTileTouchDown();
							}
						}
					#else
						//command has been issue on the specific tile, either left or right mouse click on the tile
						if(Input.GetMouseButtonDown(0)){
							if(hoveredTile!=null) _OnTileCursorDown(hoveredTile);
						}
						if(Input.GetMouseButtonDown(1)){
							if(!targetMode && hoveredTile!=null) hoveredTile.OnTouchMouseDownAlt();
						}
					#endif
				}
				
				//FogOfWar.InLOS(hoveredTile, grid.tileList[0], true);	//los function test
			}
			else{
				if(hoveredTile!=null) _ClearHoveredTile();
			}
			
		}
		
		#if UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8 || UNITY_BLACKBERRY
			//this is for touch input only
			private Tile currentSelectTile;
		
			//for touch input, confirm when a tile has been selected
			private void OnTileTouchDown(){
				_OnTileCursorDown(hoveredTile);
				currentSelectTile=null;
				_ClearHoveredTile();
			}
		#endif
		
		public static void OnTileCursorDown(Tile tile){ instance._OnTileCursorDown(tile); }
		public void _OnTileCursorDown(Tile tile){
			if(targetMode) targetModeTargetSelected(tile);
			else tile.OnTouchMouseDown();
		}
		
		//call when cursor just hover over a new tile
		public static void NewHoveredTile(Tile tile){ instance._NewHoveredTile(tile); }
		void _NewHoveredTile(Tile tile){
			//for testing los cast
			//if(GameControl.selectedUnit!=null) FogOfWar.InLOS(GameControl.selectedUnit.tile, tile, true);
			
			_ClearHoveredTile();
			ShowIndicator(tile.GetPos());
			
			if(targetMode){
				SetTargetModeHoveredTile(tile);
				return;
			}
			
			bool isWalkable=walkableTileList.Contains(tile);
			
			//show cover overlay if cover-system is enabled
			if(GameControl.EnableCover() && isWalkable){
				for(int i=0; i<tile.coverList.Count; i++){
					Transform overlayT=null;
					if(tile.coverList[i].type==CoverSystem._CoverType.Full) overlayT=coverFOverlayList[i];
					if(tile.coverList[i].type==CoverSystem._CoverType.Half) overlayT=coverHOverlayList[i];
					
					overlayT.position=tile.coverList[i].overlayPos;
					overlayT.rotation=tile.coverList[i].overlayRot;
					
					overlayT.gameObject.SetActive(true);
				}
			}
			
			//highlight potential target for the unit to be moved into this tile
			if(isWalkable && GameControl.selectedUnit.CanAttack()){
				SetWalkableHostileList(tile);
			}
			
			
			if(attackableTileList.Contains(tile)){
				if(TurnControl.ClearToProceed()){	//if the some unit is in action, dont show the overlay, the unit cant attack while someone is in action anyway
					if(onHoverAttackableTileE!=null) onHoverAttackableTileE(tile);	//show attack info on UI
				}
			}
			else if(onExitAttackableTileE!=null) onExitAttackableTileE();	//hide attack info on UI
			
			if(walkableTileList.Contains(tile)){
				if(onHoverWalkableTileE!=null) onHoverWalkableTileE(tile);
			}
			else if(onExitWalkableTileE!=null) onExitWalkableTileE();
		}
		
		//cleared the tile which has just been hovered over by the cursor
		public static void ClearHoveredTile(){ instance._ClearHoveredTile(); }
		void _ClearHoveredTile(){
			if(hoveredTile!=null){
				ClearWalkableHostileList();
			}
			
			ShowHostileIndicator(attackableTileList);
			for(int i=0; i<attackableTileList.Count; i++){
				attackableTileList[i].SetMaterial(_GetMatHostile());
			}
			
			hoveredTile=null;
			HideIndicator();
			HideCoverOverlay();
			
			if(onExitAttackableTileE!=null) onExitAttackableTileE();
			if(onExitWalkableTileE!=null) onExitWalkableTileE();
			
			if(targetMode) ClearTargetModeHoveredTile();
		}
		
		
		//for when hover over a walkable tile, show the potential target if move into that tile
		void SetWalkableHostileList(Tile tile){
			ClearHostileIndicator();
			for(int i=0; i<attackableTileList.Count; i++){
				attackableTileList[i].SetMaterial(_GetMatNormal());
			}
			List<Tile> tempAttackableTileList=tile.GetHostileInRange();
			for(int i=0; i<tempAttackableTileList.Count; i++){
				if(!tempAttackableTileList[i].IsVisible()){
					tempAttackableTileList.RemoveAt(i);	i-=1;
				}
				else tempAttackableTileList[i].SetMaterial(_GetMatHostile());
			}
			ShowHostileIndicator(tempAttackableTileList);
		}
		void ClearWalkableHostileList(){
			if(hoveredTile==null) return;
			ClearHostileIndicator();
			List<Tile> tempAttackableTileList=hoveredTile.GetHostileInRange();
			for(int i=0; i<tempAttackableTileList.Count; i++){
				if(!tempAttackableTileList[i].IsVisible()) continue;
				tempAttackableTileList[i].SetMaterial(_GetMatNormal());
			}
		}
		
		
		//**********************************************************************************************************************
		//these section are related to target tile selecting when using abilities
		
		//the callback function when a target tile has been selected
		public delegate void TargetModeSelectedCallBack(Tile tile);
		
		private enum _AbilityType{ Unit, Command }
		private _AbilityType targetModeAbilityType;
		private bool targetMode=false;
		private int targetModeAOE;
		private _TargetType targetModeType;
		private List<Tile> targetModeTileList=new List<Tile>();
		private List<Tile> targetModeHoveredTileList=new List<Tile>();
		private TargetModeSelectedCallBack targetModeCallBack;
		
		//function to initialize targetSelect mode
		public static void AbilityTargetMode(int AOE, _TargetType type, TargetModeSelectedCallBack callBack){
			instance._AbilityTargetMode(null, 0, AOE, type, callBack);	//for command ability
		}
		public static void AbilityTargetMode(Tile tile, int range, int AOE, _TargetType type, TargetModeSelectedCallBack callBack){
			instance._AbilityTargetMode(tile, range, AOE, type, callBack);	//for unit ability
		}
		public void _AbilityTargetMode(Tile tile, int range, int AOE, _TargetType type, TargetModeSelectedCallBack callBack){
			
			_AbilityType newABType=(tile!=null) ? _AbilityType.Unit : _AbilityType.Command;
			if(targetMode && targetModeAbilityType!=newABType){
				_ClearTargetMode(true);
			}
			
			targetModeAOE=AOE;
			targetModeType=type;
			targetModeCallBack=callBack;
			targetMode=true;
			
			ClearAllTile();
			
			if(tile!=null){
				targetModeAbilityType=_AbilityType.Unit;
				targetModeTileList=GetTilesWithinDistance(tile, range);
				for(int i=0; i<targetModeTileList.Count; i++) targetModeTileList[i].SetState(_TileState.Range);
			}
			else targetModeAbilityType=_AbilityType.Command;
		}
		
		//function called when a tile has been clicked on in target mode
		private void targetModeTargetSelected(Tile tile){
			if(!targetModeTileList.Contains(tile)) return;
			int currentFacID=FactionManager.GetSelectedFactionID();
			if(targetModeType==_TargetType.AllUnit || targetModeType==_TargetType.AllTile) TargetModeCallBack(tile);
			else if(targetModeType==_TargetType.HostileUnit){
				if(tile.unit!=null && tile.unit.factionID!=currentFacID) TargetModeCallBack(tile);
			}
			else if(targetModeType==_TargetType.FriendlyUnit){
				if(tile.unit!=null && tile.unit.factionID==currentFacID) TargetModeCallBack(tile);
			}
			else if(targetModeType==_TargetType.Tile){
				if(tile.unit==null) TargetModeCallBack(tile);
			}
			else GameControl.DisplayMessage("Invalid Target");
		}
		private void TargetModeCallBack(Tile tile){
			targetModeCallBack(tile);
			_ClearTargetMode();
		}
		
		public static void ClearTargetMode(){ instance._ClearTargetMode(false); }
		private void _ClearTargetMode(bool clearSelectedAbility=true){
			if(!targetMode) return;
			
			targetMode=false;
			
			ClearTargetModeHoveredTile();
			
			if(targetModeAbilityType==_AbilityType.Unit){
				for(int i=0; i<targetModeTileList.Count; i++) targetModeTileList[i].SetState(_TileState.Default);
				targetModeTileList=new List<Tile>();
				if(clearSelectedAbility) GameControl.selectedUnit.ClearSelectedAbility();
			}
			else{
				if(clearSelectedAbility) AbilityManagerFaction.ClearSelectedAbility();
			}
			
			Select(GameControl.selectedUnit);
		}
		
		
		//highlight the target tile in targetMode
		private void SetTargetModeHoveredTile(Tile tile){
			ClearTargetModeHoveredTile();
			
			if(targetModeAbilityType==_AbilityType.Unit && !targetModeTileList.Contains(tile)) return;
			
			if(targetModeAOE>0) targetModeHoveredTileList=GetTilesWithinDistance(tile, targetModeAOE);
			if(!targetModeHoveredTileList.Contains(tile)) targetModeHoveredTileList.Add(tile);
			
			Material mat=null;
			if(targetModeType==_TargetType.AllUnit) 			mat=_GetMatABAll();
			if(targetModeType==_TargetType.HostileUnit) 	mat=_GetMatABHostile();
			if(targetModeType==_TargetType.FriendlyUnit) 	mat=_GetMatABFriendly();
			if(targetModeType==_TargetType.AllTile) 			mat=_GetMatABAll();
			if(targetModeType==_TargetType.Tile) 				mat=_GetMatABAll();
			
			for(int i=0; i<targetModeHoveredTileList.Count; i++) targetModeHoveredTileList[i].SetMaterial(mat);
		}
		private void ClearTargetModeHoveredTile(){
			for(int i=0; i<targetModeHoveredTileList.Count; i++){
				if(targetModeTileList.Contains(targetModeHoveredTileList[i])) targetModeHoveredTileList[i].SetState(_TileState.Range);
				else targetModeHoveredTileList[i].SetState(_TileState.Default);
			}
			targetModeHoveredTileList=new List<Tile>();
		}
		
		//end target mode related function
		//************************************************************************
		
		
		
		//calculate the tile in the grid based on a position in the world space
		public static Tile GetTileOnPos(Vector3 pos){ //the static function is only called by GridEditor
			return instance!=null ? instance._GetTileOnPos(pos) : null;
		}
		public Tile _GetTileOnPos(Vector3 point){
			Tile tile=null;
			
			int gridOffsetX=width/2;
			int gridOffsetZ=length/2;
			
			if(tileType==_TileType.Hex){
				float spaceX=GridGenerator.spaceXHex*tileSize*gridToTileRatio;
				float spaceZ=GridGenerator.spaceZHex*tileSize*gridToTileRatio;
				
				float offX=width%2==1 ? spaceX/2 : 0;			//depends on the with of the gird, set the offset of x-axis
				int column=(int)Mathf.Floor((point.x+offX)/spaceX)+gridOffsetX;
				
				float offZ=column%2==1 ? spaceZ : spaceZ/2;	//depends on the column, introduce a offset of half a tile (odd number column has more row)
				if(length%2==1) offZ-=spaceZ/2;					//depends on the length of the grid, modify the offset
				int row=(int)Mathf.Floor((point.z-offZ)/spaceZ)+gridOffsetZ;
				
				int tileID=column*length+row-column/2;
				
				if(tileID<0 || tileID>=grid.tileList.Count) return null;
				
				tile=grid.tileList[tileID];
				
				//Debug.Log(Vector3.Distance(tile.GetPos(), point)+"    "+(GridGenerator.spaceZHex*tileSize));
				if(Vector3.Distance(tile.GetPos(), point)>GridGenerator.spaceZHex*tileSize*.5f) return null;
			}
			else if(tileType==_TileType.Square){
				float spaceX=tileSize*gridToTileRatio;
				float spaceZ=tileSize*gridToTileRatio;
				
				float offX=width%2==1 ? spaceX/2 : 0;	//depends on the with of the gird, set the offset of x-axis
				float offZ=length%2==1 ? spaceZ/2 : 0;	//depends on the length of the grid, introduce a offset of half a tile
				
				int column=(int)Mathf.Floor((point.x+offX)/spaceX)+gridOffsetX;
				int row=(int)Mathf.Floor((point.z+offZ)/spaceZ)+gridOffsetZ;
				int tileID=column*length+row;
				
				if(tileID<0 || tileID>=grid.tileList.Count) return null;
				
				tile=grid.tileList[tileID];
				
				if(Vector3.Distance(tile.GetPos(), point)>tileSize*.65f) return null;
			}
			
			return tile;
		}
		
		
		
		public void GenerateGrid(){
			Debug.Log("generate grid");
			
			FactionManager factionManager = FactionManager.GetInstance();//(FactionManager)FindObjectOfType(typeof(FactionManager));
			if(Application.isPlaying && factionManager==null)
				factionManager = (FactionManager)FindObjectOfType(typeof(FactionManager));
			
			if(factionManager!=null){
				factionManager.ClearUnit();
				factionManager.RecordSpawnTilePos();	//this is to record the tile of the spawn and deploy area
			}
			
			if(grid!=null) grid.ClearGrid();
			
			if(tileType==_TileType.Hex){
				grid=GridGenerator.GenerateHexGrid(width, length, tileSize, gridToTileRatio, unwalkableRate, gridColliderType);
			}
			else if(tileType==_TileType.Square){
				grid=GridGenerator.GenerateSquareGrid(width, length, tileSize, gridToTileRatio, unwalkableRate, gridColliderType);
			}
			
			if(grid.gridObj!=null) grid.gridObj.transform.parent=transform.parent;
			
			if(factionManager!=null){
				factionManager.SetStartingTileListBaseOnPos(tileSize);	//this is to set the tiles of the spawn and deploy area bsaed on the stored info earlier
				if(factionManager.generateUnitOnStart) factionManager._GenerateUnit();
			}
		}
		
		
		//when player click on a particular tile
		public static void OnTile(Tile tile){ instance._OnTile(tile); }
		public void _OnTile(Tile tile){
			if(!FactionManager.IsPlayerTurn()) return;
			
			if(tile.unit!=null){
				//select the unit if the unit belong's to current player in turn
				if(FactionManager.GetSelectedFactionID()==tile.unit.factionID){
					if(TurnControl.GetMoveOrder()!=_MoveOrder.Free) return;
					if(TurnControl.GetTurnMode()==_TurnMode.UnitPerTurn) return;
					if(!GameControl.AllowUnitSelect()) return;
					if(GameControl.selectedUnit.tile==tile) return;
					
					GameControl.SelectUnit(tile);
				}
				//if the unit in the tile can be attack by current selected unit, attack it
				else if(attackableTileList.Contains(tile)){
					if(GameControl.selectedTile != null && GameControl.selectedTile.Equals(tile)){
						GameControl.selectedUnit.Attack(tile.unit);
					}
					else GameControl.SelectTile(tile);
				}
			}
			//if the tile is within the move range of current selected unit, try to select it, if it is already selected, move
			else if(walkableTileList.Contains(tile)){
				if(GameControl.selectedTile != null && GameControl.selectedTile.Equals(tile)){
					GameControl.selectedUnit.Move(tile);
					if(onExitWalkableTileE!=null) onExitWalkableTileE();	//for clear UI move cost overlay
					ClearWalkableHostileList();	//in case the unit move into the destination and has insufficient ap to attack
				}
				else GameControl.SelectTile(tile);
			}

			ClearHoveredTile();	//clear the hovered tile so all the UI overlay will be cleared
		}
		
		
		//when player right-click on a particular tile
		//only used to set unit, facing
		public static void OnTileAlt(Tile tile){ instance._OnTileAlt(tile); }
		public void _OnTileAlt(Tile tile){
			if(!FactionManager.IsPlayerTurn()) return;
			
			//change the unit facing
			/*
			if(GameControl.selectedUnit!=null){
				if(tile==GameControl.selectedUnit.tile) return;
				
				float x=GameControl.selectedUnit.tile.GetPos().x-tile.GetPos().x;
				float z=GameControl.selectedUnit.tile.GetPos().z-tile.GetPos().z;
				Vector2 dir=new Vector2(x, z);
				
				float angle=Utilities.Vector2ToAngle(dir);
				
				GameControl.selectedUnit.Rotate(Quaternion.Euler(0, 360-angle-90, 0));
			}
			*/
		}
		
		//select a unit, setup the walkable, attackable tiles and what not
		public static void Select(Unit unit){
			unit.tile.SetState(_TileState.Selected);
			if(unit.CanMove()) instance.SetupWalkableTileList(unit);
			if(unit.CanAttack()) instance.SetupAttackableTileList(unit);
			instance.indicatorSelected.position=unit.tile.GetPos();
		}

		//select a given tile
		public static void Select(Tile tile){
			instance.indicatorSelectedConfirmation.position=tile.GetPos();
		}

		//function to setup and clear walkable tiles in range for current selected unit
		private void ClearWalkableTileList(){
			for(int i=0; i<walkableTileList.Count; i++){
				walkableTileList[i].SetState(_TileState.Default);
				walkableTileList[i].hostileInRangeList=new List<Tile>();
				walkableTileList[i].distance=0;
			}
			walkableTileList=new List<Tile>();
		}
		private void SetupWalkableTileList(Unit unit){
			ClearWalkableTileList();
			//List<Tile> newList=GetTilesWithinDistance(unit.tile, unit.GetEffectiveMoveRange(), true, true);
			List<Tile> newList = AStar.GetTileWithinDistance(unit.tile, unit.GetEffectiveMoveRange(), true);
			for(int i=0; i<newList.Count; i++){
				if(newList[i].unit==null){
					walkableTileList.Add(newList[i]);
					newList[i].SetState(_TileState.Walkable);
				}
			}
			AStar.ResetGraph(unit.tile, new List<Tile>(), newList);
			SetupHostileInRangeforTile(unit, walkableTileList);
		}
		
		//function to setup and clear attackble tiles in range for current selected unit
		private void ClearAttackableTileList(){
			for(int i=0; i<attackableTileList.Count; i++) attackableTileList[i].SetState(_TileState.Default);
			attackableTileList=new List<Tile>();
		}
		private void SetupAttackableTileList(Unit unit){
			ClearAttackableTileList();
			attackableTileList=unit.tile.SetupHostileInRange();
			for(int i=0; i<attackableTileList.Count; i++) attackableTileList[i].SetState(_TileState.Hostile);
			
			ShowHostileIndicator(attackableTileList);
		}
		
		
		//given a unit and a list of tiles, setup the attackable tiles with that unit in each of those given tiles. the attackble tile list are stored in each corresponding tile
		public static void SetupHostileInRangeforTile(Unit unit, Tile tile){ SetupHostileInRangeforTile(unit, new List<Tile>{ tile }); }
		public static void SetupHostileInRangeforTile(Unit unit, List<Tile> tileList){
			List<Unit> allUnitList=FactionManager.GetAllUnit();
			List<Unit> allHostileUnitList=new List<Unit>();
			for(int i=0; i<allUnitList.Count; i++){
				if(allUnitList[i].factionID!=unit.factionID) allHostileUnitList.Add(allUnitList[i]);
			}
			
			List<Unit> allFriendlyUnitList=new List<Unit>();
			if(GameControl.EnableFogOfWar()) allFriendlyUnitList=FactionManager.GetAllUnitsOfFaction(unit.factionID);
			
			int range=unit.GetAttackRange();
			int sight=unit.GetSight();
			
			for(int i=0; i<tileList.Count; i++){
				Tile srcTile=tileList[i];
				List<Tile> hostileInRangeList=new List<Tile>();
				
				for(int j=0; j<allHostileUnitList.Count; j++){
					Tile targetTile=allHostileUnitList[j].tile;
					
					if(GridManager.GetDistance(srcTile, targetTile)>range) continue;
					
					if(!GameControl.EnableFogOfWar() && !GameControl.AttackThroughObstacle()){
						if(!FogOfWar.InLOS(srcTile, targetTile, 0)) continue;
					}
					
					bool inSight=GameControl.EnableFogOfWar() ? false : true;
					if(GameControl.EnableFogOfWar()){
						if(FogOfWar.InLOS(srcTile, targetTile) && GridManager.GetDistance(srcTile, targetTile)<=sight){
							inSight=true;
						}
						else if(!unit.requireDirectLOSToAttack){
							for(int n=0; n<allFriendlyUnitList.Count; n++){
								if(allFriendlyUnitList[n]==unit) continue;
								if(GridManager.GetDistance(allFriendlyUnitList[n].tile, targetTile)>allFriendlyUnitList[n].GetSight()) continue;
								if(FogOfWar.InLOS(allFriendlyUnitList[n].tile, targetTile)){
									inSight=true;
									break;
								}
							}
						}
					}
					
					if(inSight) hostileInRangeList.Add(targetTile);
					
				}
				
				tileList[i].SetHostileInRange(hostileInRangeList);
			}
		}
		
		
		//reset all selection, walkablelist and what not
		public static void ClearAllTile(){
			if(GameControl.selectedUnit!=null) GameControl.selectedUnit.tile.SetState(_TileState.Default);
			if(GameControl.selectedTile!=null) GameControl.selectedTile.SetState(_TileState.Default);
			instance.ClearWalkableTileList();
			instance.ClearAttackableTileList();
			instance.indicatorSelected.position=new Vector3(0, 99999, 0);
			instance.indicatorSelectedConfirmation.position=new Vector3(0, 99999, 0);
			instance.ClearHostileIndicator();
			instance.ClearWalkableHostileList();
		}
		
		
		
		public void ClearHostileIndicator(){
			for(int i=0; i<indicatorHostileList.Count; i++)
				indicatorHostileList[i].position=new Vector3(0, 99999, 0);
		}
		public void ShowHostileIndicator(List<Tile> list){
			return;
			while(indicatorHostileList.Count<list.Count){
				Transform indicatorHostileT=(Transform)Instantiate(indicatorHostileList[0]);
				indicatorHostileT.parent=transform;
				indicatorHostileList.Add(indicatorHostileT);
			}
			
			for(int i=0; i<list.Count; i++){
				indicatorHostileList[i].position=list[i].GetPos()+new Vector3(0, 0.1f, 0);
			}
		}
		
		
		
		
		//called to setup tile for player's faction unit deployment, they need to be highlighted
		private List<Tile> currentDeployableTileList=new List<Tile>();
		public static void DeployingFaction(int factionID){ instance._DeployingFaction(factionID); }
		public void _DeployingFaction(int factionID){
			currentDeployableTileList=_GetDeployableTileList(factionID);
			for(int i=0; i<currentDeployableTileList.Count; i++) currentDeployableTileList[i].SetState(_TileState.Range);
		}
		public static void FactionDeploymentComplete(){ instance._FactionDeploymentComplete(); }
		public void _FactionDeploymentComplete(){
			for(int i=0; i<currentDeployableTileList.Count; i++) currentDeployableTileList[i].SetState(_TileState.Default);
		}
		//get delployable tile list for certain faction
		public static List<Tile> GetDeployableTileList(int factionID){ return instance._GetDeployableTileList(factionID); }
		public List<Tile> _GetDeployableTileList(int factionID){
			List<Tile> deployableTileList=new List<Tile>();
			for(int i=0; i<grid.tileList.Count; i++){
				if(grid.tileList[i].deployAreaID==factionID) deployableTileList.Add(grid.tileList[i]);
			}
			return deployableTileList;
		}
		public static int GetDeployableTileListCount(){ return instance._GetDeployableTileListCount(); }
		public int _GetDeployableTileListCount(){
			int count=0;
			for(int i=0; i<currentDeployableTileList.Count; i++){
				if(currentDeployableTileList[i].unit==null) count+=1;
			}
			return count;
		}
		
		
		
		public static void ShowIndicator(Vector3 pos){ instance.indicatorCursor.position=pos+new Vector3(0, 0.05f, 0); }
		public static void HideIndicator(){ instance.indicatorCursor.position=new Vector3(0, 99999, 0); }
		
		
		public static void HideCoverOverlay(){ instance._HideCoverOverlay(); }
		public void _HideCoverOverlay(){
			for(int i=0; i<coverHOverlayList.Count; i++) coverHOverlayList[i].gameObject.SetActive(false);
			for(int i=0; i<coverFOverlayList.Count; i++) coverFOverlayList[i].gameObject.SetActive(false);
		}
		
		
		//to get all the tiles within certain distance from a particular tile
		//set distance is only used when called from SetupWalkableTileList (to record the distance of each tile from the source tile)
		public static List<Tile> GetTilesWithinDistance(Tile tile, int dist, bool walkableOnly=false, bool setDistance=false){
			return instance.grid.GetTilesWithinDistance(tile, dist, walkableOnly, setDistance);
		}
		
		//get the distance (in term of tile) between 2 tiles, 
		public static int GetDistance(Tile tile1, Tile tile2, bool walkable=false){
			if(!walkable) return instance.grid.GetDistance(tile1, tile2);
			else return instance.grid.GetWalkableDistance(tile1, tile2);
		}
		
		
		
	}
	
}
