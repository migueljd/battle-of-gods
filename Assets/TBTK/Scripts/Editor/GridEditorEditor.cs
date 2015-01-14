using UnityEngine;
using UnityEditor;

using System.Collections;
using System.Collections.Generic;

using TBTK;

namespace TBTK{

	[CustomEditor(typeof(GridEditor))]
	public class GridEditorEditor : Editor {

		public enum _EditMode{ Tile, Unit, }
		private static _EditMode editMode;
		
		private Color colorOn=new Color(.25f, 1f, 1f, 1f);
		
		private bool rotatingView=false;
		
		private GridManager gridManager;
		private FactionManager factionManager;
		
		private static bool enabled=true;
		
		void Awake(){
			//GridEditor instance = (GridEditor)target;
			
			EditorDBManager.Init();
			
			factionManager=FactionManager.GetInstance();
			gridManager=GridManager.GetInstance();
		}
		
		
		public override void OnInspectorGUI(){
			
			EditorGUILayout.Space();
			
			//~ EditorGUILayout.BeginHorizontal();
				//~ if(enabled) GUI.color=new Color(0, 1f, 1f, 1f);
				//if(GUILayout.Button("Enable", GUILayout.ExpandWidth(true), GUILayout.Height(25))){
				//~ if(GUILayout.Button("Enable", GUILayout.Width(125), GUILayout.Height(20))){
					//~ enabled=true;
				//~ }
				//~ if(!enabled) GUI.color=Color.red;
				//~ else GUI.color=Color.white;
				//if(GUILayout.Button("Disable", GUILayout.ExpandWidth(true), GUILayout.Height(25))){
				//~ if(GUILayout.Button("Disable", GUILayout.Width(125), GUILayout.Height(20))){
					//~ enabled=false;
				//~ }
				//~ GUI.color=Color.white;
			//~ EditorGUILayout.EndHorizontal();
			//~ EditorGUILayout.Space();
				
			string text=enabled ? "Disable" : "Enable";
			GUI.color=enabled ? colorOn : Color.white ;
			//~ if(GUILayout.Button(text, GUILayout.Width(125), GUILayout.Height(20))){
			if(GUILayout.Button(text)) enabled=!enabled;
			GUI.color=Color.white;
			
			EditorGUILayout.Space();
			
			if(enabled){
				GUILayout.Label("Edit Type:");
				
				EditorGUILayout.BeginHorizontal();
				
				GUI.color=editMode==_EditMode.Tile ? colorOn : Color.white ;
				if(GUILayout.Button("Tile")) editMode=_EditMode.Tile;
				
				GUI.color=editMode==_EditMode.Unit ? colorOn : Color.white ;
				if(GUILayout.Button("Unit")) editMode=_EditMode.Unit;
				
				GUI.color=Color.white;
				
				EditorGUILayout.EndHorizontal();
				
				
				
				if(editMode==_EditMode.Tile) DrawEditModeTileUI();
				if(editMode==_EditMode.Unit) DrawEditModeUnitUI();
				
			}
		}
		
		
		void DrawEditModeTileUI(){
			
			GUILayout.Label("________________________________________________________________________________________________________");
			
			//~ EditorGUILayout.Space();
			
			GUILayout.Label("Tile State:");
			
			GUI.color=tileState==_TileState.Unwalkable ? colorOn : Color.white ;
			if(GUILayout.Button("Unwalkable")) tileState=_TileState.Unwalkable;
			
			GUI.color=tileState==_TileState.Default ? colorOn : Color.white ;
			if(GUILayout.Button("Default")) tileState=_TileState.Default;
			
			EditorGUILayout.BeginHorizontal();
				GUI.color=tileState==_TileState.WallH ? colorOn : Color.white ;
				if(GUILayout.Button("Wall (Half)")) tileState=_TileState.WallH;
			
				GUI.color=tileState==_TileState.WallF ? colorOn : Color.white ;
				if(GUILayout.Button("Wall (Full)")) tileState=_TileState.WallF;
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
				GUI.color=tileState==_TileState.ObstacleH ? colorOn : Color.white ;
				if(GUILayout.Button("Obstacle (Half)")) tileState=_TileState.ObstacleH;
			
				GUI.color=tileState==_TileState.ObstacleF ? colorOn : Color.white ;
				if(GUILayout.Button("Obstacle (Full)")) tileState=_TileState.ObstacleF;
			EditorGUILayout.EndHorizontal();
			
			GUI.color=tileState==_TileState.SpawnArea ? colorOn : Color.white ;
			if(GUILayout.Button("SpawnArea")) tileState=_TileState.SpawnArea;
				
			GUI.color=tileState==_TileState.Deployment ? colorOn : Color.white ;
			if(GUILayout.Button("Deployment")) tileState=_TileState.Deployment;
			
			
			if(tileState==_TileState.SpawnArea){
				
				GUILayout.Label("________________________________________________________________________________________________________");
			
				for(int i=0; i<factionManager.factionList.Count; i++){
					EditorGUILayout.Space();
					
					GUI.color=spawnAreaFactionID==i ? colorOn : Color.white ;
					if(GUILayout.Button(factionManager.factionList[i].name.ToString())) spawnAreaFactionID=i;
					
					if(spawnAreaFactionID==i){
						
						Faction fac=factionManager.factionList[i];
						
						spawnAreaInfoID=Mathf.Clamp(spawnAreaInfoID, 0, fac.spawnInfoList.Count);
						
						for(int n=0; n<fac.spawnInfoList.Count; n++){
							EditorGUILayout.BeginHorizontal();
							
								GUILayout.Label("   - ", GUILayout.Width(30));
								
								GUI.color=spawnAreaInfoID==n ? colorOn : Color.white ;
								if(GUILayout.Button("SpawnArea "+(n+1))) spawnAreaInfoID=n;
							
								GUI.color=Color.white;
								if(GUILayout.Button("Clear All ", GUILayout.Width(70))){
									for(int m=0; m<fac.spawnInfoList[n].startingTileList.Count; m++){
										fac.spawnInfoList[n].startingTileList[m].spawnAreaID=-1;
									}
									fac.spawnInfoList[n].startingTileList=new List<Tile>();
									SceneView.RepaintAll();
								}
							
							EditorGUILayout.EndHorizontal();
						}
					}
				}
			}
			if(tileState==_TileState.Deployment){
				GUILayout.Label("________________________________________________________________________________________________________");
			
				for(int i=0; i<factionManager.factionList.Count; i++){
					EditorGUILayout.Space();
					EditorGUILayout.BeginHorizontal();
						GUI.color=deployAreaFactionID==i ? colorOn : Color.white ;
						if(GUILayout.Button(factionManager.factionList[i].name.ToString())) deployAreaFactionID=i;
					
						GUI.color=Color.white;
						if(GUILayout.Button("Clear All ", GUILayout.Width(70))){
							Faction facAlt=factionManager.factionList[i];
							for(int n=0; n<facAlt.deployableTileList.Count; n++){
								facAlt.deployableTileList[n].deployAreaID=-1;
							}
							facAlt.deployableTileList=new List<Tile>();
							SceneView.RepaintAll();
						}
					EditorGUILayout.EndHorizontal();
				}
			}
			//factionManager
			
			GUI.color=Color.white;
			
			EditorGUILayout.Space();
		}
		
		
		
		
		
		
		void DrawEditModeUnitUI(){
			
			
			GUILayout.Label("________________________________________________________________________________________________________");
			
			GUILayout.Label("Unit's Faction:");
			
				for(int i=0; i<factionManager.factionList.Count; i++){
					
					GUI.color=unitFactionID==i ? colorOn : Color.white ;
					if(GUILayout.Button(factionManager.factionList[i].name.ToString())) unitFactionID=i;

				}
			
			
			GUILayout.Label("________________________________________________________________________________________________________");
			
			GUILayout.Label("Unit To Deploy:");
			
			List<Unit> unitList=EditorDBManager.GetUnitList();
			
			if(Event.current.type == EventType.Layout)
				unitNumInRow=(int)Mathf.Max(1, Mathf.Floor(editorWidth/50));
			
			
			
			EditorGUILayout.BeginHorizontal();
			
			Rect rect=new Rect(0, 0, 0, 0);
			
			for(int i=0; i<unitList.Count; i++){
				if(i%unitNumInRow==0){
					EditorGUILayout.EndHorizontal();
					EditorGUILayout.BeginHorizontal();
				}
				
				GUI.color=selectedUnit==unitList[i] ? colorOn : Color.white ;
				if(unitList[i].iconSprite==null) cont=new GUIContent((Texture)null, unitList[i].unitName);
				else cont=new GUIContent(unitList[i].iconSprite.texture, unitList[i].unitName);
				
				if(GUILayout.Button(cont, GUILayout.Width(45), GUILayout.Height(45))) selectedUnit=unitList[i];
				
				if(selectedUnit==unitList[i]) rect=GUILayoutUtility.GetLastRect();
			}
			
			if(selectedUnit!=null){
				rect.x+=3; rect.y+=3; rect.width-=6; rect.height-=6;
				EditorUtilities.DrawSprite(rect, selectedUnit.iconSprite, false, false);
			}
			
			EditorGUILayout.EndHorizontal();
			
			GUI.color=Color.white;
			
			EditorGUILayout.Space();
			
			editorWidth=Screen.width-8;
		}
		
		private float editorWidth=1;
		private int unitNumInRow=1;
		GUIContent cont;
		
		
		void OnSceneGUI(){
			if(Application.isPlaying) return;
			
			if(!enabled) return;
			
			
			Event current = Event.current;
			int controlID = GUIUtility.GetControlID(FocusType.Passive);
		 
			switch (current.type)
			{
				case EventType.MouseDown:
					Edit(current);
					break;
				
				case EventType.MouseDrag:
					if(current.button==0) Edit(current);
					break;
				
				case EventType.keyDown:
					if(Event.current.keyCode==(KeyCode.RightAlt) || Event.current.keyCode==(KeyCode.LeftAlt)) rotatingView=true;
					break;
					
				case EventType.keyUp:
					if(Event.current.keyCode==(KeyCode.RightAlt) || Event.current.keyCode==(KeyCode.LeftAlt)) rotatingView=false;
					break;
		 
				case EventType.layout:
					HandleUtility.AddDefaultControl(controlID);
					break;
			}

			if(GUI.changed){
				//HandleUtility.Repaint();
				EditorUtility.SetDirty(target);
			}
		}
		
		void Edit(Event current){
			if(rotatingView) return;
			
			LayerMask mask=1<<LayerManager.GetLayerTile();
			Ray ray = HandleUtility.GUIPointToWorldRay(current.mousePosition);
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit, Mathf.Infinity, mask)){
				//Tile tile=hit.transform.gameObject.GetComponent<Tile>();
				//Tile tile=gridManager._GetTileOnPos(hit.point);
				
				Tile tile=null;
				if(gridManager.gridColliderType==GridManager._GridColliderType.Individual){
					tile=hit.transform.gameObject.GetComponent<Tile>();
				}
				else if(gridManager.gridColliderType==GridManager._GridColliderType.Master){
					tile=gridManager._GetTileOnPos(hit.point);
				}
				
				if(tile==null) return;
				
				//Debug.Log(tile.transform.name);
				
				if(editMode==_EditMode.Tile) EditTileState(tile, current.button, hit.point);
				else if(editMode==_EditMode.Unit) EditTileUnit(tile, current.button, hit.point);
				
				EditorUtility.SetDirty(tile);
			}
			else Debug.Log("nothing");
		}
		
		
		private enum _TileState {Unwalkable, Default, WallH, WallF, ObstacleH, ObstacleF, SpawnArea, Deployment}
		private static _TileState tileState=_TileState.Default;
		private int spawnAreaFactionID=0;	//factionID of the spawnArea
		private int spawnAreaInfoID=0;			//spawnInfoID of the spawnArea (each faction could have multiple spawnInfo
		private int deployAreaFactionID=0;
		void EditTileState(Tile tile, int mouseClick=0, Vector3 hitPos=default(Vector3)){
			if(tileState==_TileState.Unwalkable){
				if(tile.HasObstacle()){
					Debug.Log("Cannot set tile to unwalkable. Clear obstacle on tile first");
					return;
				}
				tile.walkable=false;
				tile.SetState(TBTK._TileState.Default);
			}
			else if(tileState==_TileState.Default){
				if(tile.HasObstacle()){
					Debug.Log("Cannot set tile to walkable. Clear obstacle on tile first");
					return;
				}
				tile.walkable=true;
				tile.SetState(TBTK._TileState.Default);
			}
			else if(tileState==_TileState.WallH || tileState==_TileState.WallF){
				if(tile.HasObstacle()){
					Debug.Log("Cannot add wall. Clear obstacle on tile first");
					return;
				}
				
				Grid grid=gridManager.GetGrid();
				Vector3 dir=hitPos-tile.GetPos();
				float angle=0;
				if(gridManager.tileType==_TileType.Square) angle=Utilities.VectorToAngle90(new Vector2(dir.x, dir.z));
				else if(gridManager.tileType==_TileType.Hex) angle=Utilities.VectorToAngle60(new Vector2(dir.x, dir.z));
				
				Tile neighbourTile=grid.GetNeighbourInDir(tile, angle);
				if(neighbourTile.HasObstacle()){
					Debug.Log("Cannot add wall. Clear obstacle on neighbour tile first");
					return;
				}
				
				if(mouseClick==0) tile.AddWall(angle, neighbourTile, tileState==_TileState.WallH ? 1 : 2);
				else tile.RemoveWall(angle, neighbourTile);
			}
			else if(tileState==_TileState.ObstacleH || tileState==_TileState.ObstacleF){
				if(mouseClick==1) tile.RemoveObstacle();
				else{
					ClearSpawnTile(tile);
					ClearDeployableTile(tile);
					tile.AddObstacle(tileState==_TileState.ObstacleH ? 1 : 2);
				}
			}
			else if(tileState==_TileState.SpawnArea){
				if(tile.HasObstacle()) return;
				
				if(spawnAreaFactionID>=factionManager.factionList.Count) return;
				if(spawnAreaInfoID>=factionManager.factionList[spawnAreaFactionID].spawnInfoList.Count) return;
				
				Faction fac=null;
				
				if(mouseClick==1) ClearSpawnTile(tile);
				else{
					fac=factionManager.factionList[spawnAreaFactionID];
					if(tile.spawnAreaID!=fac.ID) ClearSpawnTile(tile);
					
					if(!fac.spawnInfoList[spawnAreaInfoID].startingTileList.Contains(tile)){
						fac.spawnInfoList[spawnAreaInfoID].startingTileList.Add(tile);
						tile.spawnAreaID=fac.ID;
					}
				}
				
			}
			else if(tileState==_TileState.Deployment){
				if(tile.HasObstacle()) return;
				
				if(deployAreaFactionID>=factionManager.factionList.Count) return;
				
				Faction fac=null;
				
				if(mouseClick==1) ClearDeployableTile(tile);
				else{
					fac=factionManager.factionList[deployAreaFactionID];
					if(tile.deployAreaID!=fac.ID) ClearDeployableTile(tile);
					
					if(!fac.deployableTileList.Contains(tile)){
						fac.deployableTileList.Add(tile);
						tile.deployAreaID=fac.ID;
					}
				}
				
			}
		}
		
		void ClearSpawnTile(Tile tile){
			if(tile.spawnAreaID!=-1){
				for(int i=0; i<factionManager.factionList.Count; i++){
					Faction facAlt=factionManager.factionList[i];
					for(int n=0; n<facAlt.spawnInfoList.Count; n++){
						if(facAlt.spawnInfoList[n].startingTileList.Contains(tile)){
							facAlt.spawnInfoList[n].startingTileList.Remove(tile);
							tile.spawnAreaID=-1;
							break;
						}
					}
				}
			}
		}
		
		void ClearDeployableTile(Tile tile){
			if(tile.deployAreaID==-1) return;
			for(int i=0; i<factionManager.factionList.Count; i++){
				Faction facAlt=factionManager.factionList[i];
				if(facAlt.deployableTileList.Contains(tile)){
					facAlt.deployableTileList.Remove(tile);
					tile.deployAreaID=-1;
					break;
				}
			}
		}
		
		
		public Unit selectedUnit;
		public int unitFactionID=0;
		void EditTileUnit(Tile tile, int mouseClick=0, Vector3 hitPos=default(Vector3)){
			if(mouseClick==0){
				if(!tile.walkable){
					Debug.Log("Cannot place unit on unwalkable tile");
					return;
				}
				
				if(tile.obstacleT!=null){
					Debug.Log("Cannot place unit on tile with obstacle");
					return;
				}
				
				if(selectedUnit==null){
					Debug.Log("No unit selected. Select a unit from the editor first");
					return;
				}
				
				if(tile.unit!=null){
					RemoveUnit(tile.unit);
					DestroyImmediate(tile.unit.gameObject);
				}
				
				Vector3 dir=hitPos-tile.GetPos();
				float angle=0;
				if(gridManager.tileType==_TileType.Square) angle=360-(Utilities.VectorToAngle90(new Vector2(dir.x, dir.z))-90);
				else if(gridManager.tileType==_TileType.Hex) angle=360-(Utilities.VectorToAngle60(new Vector2(dir.x, dir.z))-90);
				
				GameObject unitObj=(GameObject)PrefabUtility.InstantiatePrefab(selectedUnit.gameObject);
				
				unitObj.transform.position=tile.GetPos();
				unitObj.transform.rotation=Quaternion.Euler(0, angle, 0);
				unitObj.transform.parent=FactionManager.GetTransform();
				
				Unit unit=unitObj.GetComponent<Unit>();
				tile.unit=unit;
				unit.tile=tile;
				
				factionManager.factionList[unitFactionID].allUnitList.Add(unit);
				
				unit.factionID=factionManager.factionList[unitFactionID].ID;
			}
			else if(mouseClick==1){
				if(tile.unit!=null){
					RemoveUnit(tile.unit);
					DestroyImmediate(tile.unit.gameObject);
					tile.unit=null;
				}
			}
		}
		
		void RemoveUnit(Unit unit){
			for(int i=0; i<factionManager.factionList.Count; i++){
				if(factionManager.factionList[i].allUnitList.Contains(unit)){
					factionManager.factionList[i].allUnitList.Remove(unit);
					break;
				}
			}
		}
		
	}

}