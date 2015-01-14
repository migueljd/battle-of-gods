using UnityEngine;
using UnityEditor;

using System;

using System.Collections;
using System.Collections.Generic;

using TBTK;

namespace TBTK{

	[CustomEditor(typeof(GridManager))]
	public class BuildManagerEditor : Editor {

		private static GridManager instance;
		
		private static bool showDefaultFlag=false;
		
		private GUIContent cont;
		private GUIContent[] contList;
		
		
		private string[] tileTypeLabel;
		private string[] tileTypeTooltip;
		
		private string[] gridColliderTypeLabel;
		private string[] gridColliderTypeTooltip;
		
		private bool foldIndicatorSetting=false;
		private bool foldObstacleSetting=false;
		private bool foldHexMatSetting=false;
		private bool foldSqMatSetting=false;
		
		
		void Awake(){
			instance = (GridManager)target;
			
			EditorDBManager.Init();
			
			int enumLength = Enum.GetValues(typeof(_TileType)).Length;
			tileTypeLabel=new string[enumLength];
			tileTypeTooltip=new string[enumLength];
			for(int i=0; i<enumLength; i++){
				tileTypeLabel[i]=((_TileType)i).ToString();
				if((_TileType)i==_TileType.Hex) 
					tileTypeTooltip[i]="using Hex grid";
				if((_TileType)i==_TileType.Square) 
					tileTypeTooltip[i]="using square grid";
			}
			
			enumLength = Enum.GetValues(typeof(GridManager._GridColliderType)).Length;
			gridColliderTypeLabel=new string[enumLength];
			gridColliderTypeTooltip=new string[enumLength];
			for(int i=0; i<enumLength; i++){
				gridColliderTypeLabel[i]=((GridManager._GridColliderType)i).ToString();
				if((GridManager._GridColliderType)i==GridManager._GridColliderType.Master) 
					gridColliderTypeTooltip[i]="using a single master collider for all the tile on the grid. Allow bigger grid but the tiles on the grid cannot be adjusted";
				if((GridManager._GridColliderType)i==GridManager._GridColliderType.Individual) 
					gridColliderTypeTooltip[i]="using individual collider for each tile on the grid. This allow positional adjustment of individual tile but severely limited the grid size. Not recommend for any grid beyond 35x35.";
			}
			
			
			EditorUtility.SetDirty(instance);
		}
		
		
		
		public override void OnInspectorGUI(){
			
			GUI.changed = false;
			
			EditorGUILayout.Space();
				
				//cont=new GUIContent("Generate Grid On Start:", "Check to re-generate the grid whenever the level is loaded. Note that this will overwrite all the existing layout and unit on the grid and force FactionManager to regenerate the unit on the grid");
				//instance.generateGridOnStart=EditorGUILayout.Toggle(cont, instance.generateGridOnStart);
			
				int tileType=(int)instance.tileType;
				cont=new GUIContent("Tile Type:", "The type of grid to use (Hex or Square)");
				contList=new GUIContent[tileTypeLabel.Length];
				for(int i=0; i<contList.Length; i++) contList[i]=new GUIContent(tileTypeLabel[i], tileTypeTooltip[i]);
				tileType = EditorGUILayout.Popup(cont, tileType, contList);
				instance.tileType=(_TileType)tileType;
			
				int gridColliderType=(int)instance.gridColliderType;
				cont=new GUIContent("Grid Collider Type:", "The type of collider to use (The collider are used for cursor detection)");
				contList=new GUIContent[gridColliderTypeLabel.Length];
				for(int i=0; i<contList.Length; i++) contList[i]=new GUIContent(gridColliderTypeLabel[i], gridColliderTypeTooltip[i]);
				gridColliderType = EditorGUILayout.Popup(cont, gridColliderType, contList);
				instance.gridColliderType=(GridManager._GridColliderType)gridColliderType;
			
			
				cont=new GUIContent("Width:", "The grid size of the grid on the platform");
				instance.width=EditorGUILayout.IntField(cont, instance.width);
			
				cont=new GUIContent("Length:", "The grid size of the grid on the platform");
				instance.length=EditorGUILayout.IntField(cont, instance.length);
			
				cont=new GUIContent("Tile Size:", "The grid size of the grid on the platform");
				instance.tileSize=EditorGUILayout.FloatField(cont, instance.tileSize);
			
				cont=new GUIContent("GridToTileSizeRatio:", "The grid size of the grid on the platform");
				instance.gridToTileRatio=EditorGUILayout.FloatField(cont, instance.gridToTileRatio);
				
				cont=new GUIContent("UnwalkableRate:", "The percentage of the unwalkable tile on the grid");
				instance.unwalkableRate=EditorGUILayout.FloatField(cont, instance.unwalkableRate);
			
			EditorGUILayout.Space();
			
				if(!Application.isPlaying){
					if(GUILayout.Button("Generate Grid", GUILayout.MaxWidth(258))) instance.GenerateGrid();
					if(GUILayout.Button("Generate Unit", GUILayout.MaxWidth(258))) FactionManager.GenerateUnit();
				}
			
			EditorGUILayout.Space();
				
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("", GUILayout.MaxWidth(10));
				foldIndicatorSetting=EditorGUILayout.Foldout(foldIndicatorSetting, "Show Indicator Prefab Assignment");
				EditorGUILayout.EndHorizontal();
				if(foldIndicatorSetting){
					cont=new GUIContent("Hex Cursor:", "The prefab to be used as cursor on-grid-indicator when using hex-grid.");
					instance.hexCursor=(Transform)EditorGUILayout.ObjectField(cont, instance.hexCursor, typeof(Transform), true);
					cont=new GUIContent("Hex Selected:", "The prefab to be used as on-grid-indicator for selected unit when using hex-grid.");
					instance.hexSelected=(Transform)EditorGUILayout.ObjectField(cont, instance.hexSelected, typeof(Transform), true);
					cont=new GUIContent("Hex Hostile:", "The prefab to be used as on-grid-indicator for attackable unit when using hex-grid.");
					instance.hexHostile=(Transform)EditorGUILayout.ObjectField(cont, instance.hexHostile, typeof(Transform), true);
					
					cont=new GUIContent("Hex Cursor:", "The prefab to be used as cursor on-grid-indicator when using square-grid.");
					instance.sqCursor=(Transform)EditorGUILayout.ObjectField(cont, instance.sqCursor, typeof(Transform), true);
					cont=new GUIContent("Hex Selected:", "The prefab to be used as on-grid-indicator for selected unit when using square-grid.");
					instance.sqSelected=(Transform)EditorGUILayout.ObjectField(cont, instance.sqSelected, typeof(Transform), true);
					cont=new GUIContent("Hex Hostile:", "The prefab to be used as on-grid-indicator for attackable unit when using square-grid.");
					instance.sqHostile=(Transform)EditorGUILayout.ObjectField(cont, instance.sqHostile, typeof(Transform), true);
					
					EditorGUILayout.Space();
				}
				
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("", GUILayout.MaxWidth(10));
				foldObstacleSetting=EditorGUILayout.Foldout(foldObstacleSetting, "Show Obstacle Prefab Assignment");
				EditorGUILayout.EndHorizontal();
				if(foldObstacleSetting){
					cont=new GUIContent("Obstacle Wall H:", "Wall prefab with full-cover. This applies to both hex and square grid");
					instance.obstacleWallH=(Transform)EditorGUILayout.ObjectField(cont, instance.obstacleWallH, typeof(Transform), true);
					cont=new GUIContent("Obstacle Wall F:", "Wall prefab with half-cover. This applies to both hex and square grid");
					instance.obstacleWallF=(Transform)EditorGUILayout.ObjectField(cont, instance.obstacleWallF, typeof(Transform), true);
					
					cont=new GUIContent("Obstacle Hex H:", "Obstacle prefab with full-cover for hex-grid");
					instance.obstacleHexF=(Transform)EditorGUILayout.ObjectField(cont, instance.obstacleHexF, typeof(Transform), true);
					cont=new GUIContent("Obstacle Hex F:", "Obstacle prefab with half-cover for hex-grid");
					instance.obstacleHexH=(Transform)EditorGUILayout.ObjectField(cont, instance.obstacleHexH, typeof(Transform), true);
					
					cont=new GUIContent("Obstacle Square H:", "Obstacle prefab with full-cover for square-grid");
					instance.obstacleSqF=(Transform)EditorGUILayout.ObjectField(cont, instance.obstacleSqF, typeof(Transform), true);
					cont=new GUIContent("Obstacle Square F:", "Obstacle prefab with half-cover for square-grid");
					instance.obstacleSqH=(Transform)EditorGUILayout.ObjectField(cont, instance.obstacleSqH, typeof(Transform), true);
					
					EditorGUILayout.Space();
				}
				
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("", GUILayout.MaxWidth(10));
				foldHexMatSetting=EditorGUILayout.Foldout(foldHexMatSetting, "Show Hex-Tile Material Assignment");
				EditorGUILayout.EndHorizontal();
				if(foldHexMatSetting){
					cont=new GUIContent("Hex Mat Normal:", "Material to used for hex-tile in normal state");
					instance.hexMatNormal=(Material)EditorGUILayout.ObjectField(cont, instance.hexMatNormal, typeof(Material), true);
					cont=new GUIContent("Hex Mat Selected:", "Material to used for hex-tile in selected state");
					instance.hexMatSelected=(Material)EditorGUILayout.ObjectField(cont, instance.hexMatSelected, typeof(Material), true);
					cont=new GUIContent("Hex Mat Walkable:", "Material to used for hex-tile in walkable state");
					instance.hexMatWalkable=(Material)EditorGUILayout.ObjectField(cont, instance.hexMatWalkable, typeof(Material), true);
					cont=new GUIContent("Hex Mat Walkable:", "Material to used for hex-tile in unwalkable state");
					instance.hexMatUnwalkable=(Material)EditorGUILayout.ObjectField(cont, instance.hexMatUnwalkable, typeof(Material), true);
					cont=new GUIContent("Hex Mat Walkable:", "Material to used for hex-tile in there's an attackable hostile on the tile");
					instance.hexMatHostile=(Material)EditorGUILayout.ObjectField(cont, instance.hexMatHostile, typeof(Material), true);
					
					cont=new GUIContent("Hex Mat Walkable:", "Material to used for hex-tile in are within unit ability's range when in ability target selection");
					instance.hexMatRange=(Material)EditorGUILayout.ObjectField(cont, instance.hexMatRange, typeof(Material), true);
					cont=new GUIContent("Hex Mat Walkable:", "Material to used for hex-tile as effective target tile for ability when in ability target selection, with ability targetType set to all");
					instance.hexMatAbilityAll=(Material)EditorGUILayout.ObjectField(cont, instance.hexMatAbilityAll, typeof(Material), true);
					cont=new GUIContent("Hex Mat Walkable:", "Material to used for hex-tile as effective target tile for ability when in ability target selection, with ability targetType set to hostile");
					instance.hexMatAbilityHostile=(Material)EditorGUILayout.ObjectField(cont, instance.hexMatAbilityHostile, typeof(Material), true);
					cont=new GUIContent("Hex Mat Walkable:", "Material to used for hex-tile as effective target tile for ability when in ability target selection, with ability targetType set to friendly");
					instance.hexMatAbilityFriendly=(Material)EditorGUILayout.ObjectField(cont, instance.hexMatAbilityFriendly, typeof(Material), true);
					
					cont=new GUIContent("Hex Mat Walkable:", "Material to used for hex-tile when hidden in fog-of-war");
					instance.hexMatInvisible=(Material)EditorGUILayout.ObjectField(cont, instance.hexMatInvisible, typeof(Material), true);
					
					EditorGUILayout.Space();
				}
				
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("", GUILayout.MaxWidth(10));
				foldSqMatSetting=EditorGUILayout.Foldout(foldSqMatSetting, "Show Square-Tile Material Assignment");
				EditorGUILayout.EndHorizontal();
				if(foldSqMatSetting){
					cont=new GUIContent("Sq Mat Normal:", "Material to used for square-tile in normal state");
					instance.sqMatNormal=(Material)EditorGUILayout.ObjectField(cont, instance.sqMatNormal, typeof(Material), true);
					cont=new GUIContent("Sq Mat Selected:", "Material to used for square-tile in selected state");
					instance.sqMatSelected=(Material)EditorGUILayout.ObjectField(cont, instance.sqMatSelected, typeof(Material), true);
					cont=new GUIContent("Sq Mat Walkable:", "Material to used for square-tile in walkable state");
					instance.sqMatWalkable=(Material)EditorGUILayout.ObjectField(cont, instance.sqMatWalkable, typeof(Material), true);
					cont=new GUIContent("Sq Mat Walkable:", "Material to used for square-tile in unwalkable state");
					instance.sqMatUnwalkable=(Material)EditorGUILayout.ObjectField(cont, instance.sqMatUnwalkable, typeof(Material), true);
					cont=new GUIContent("Sq Mat Walkable:", "Material to used for square-tile in there's an attackable hostile on the tile");
					instance.sqMatHostile=(Material)EditorGUILayout.ObjectField(cont, instance.sqMatHostile, typeof(Material), true);
					
					cont=new GUIContent("Sq Mat Walkable:", "Material to used for square-tile in are within unit ability's range when in ability target selection");
					instance.sqMatRange=(Material)EditorGUILayout.ObjectField(cont, instance.sqMatRange, typeof(Material), true);
					cont=new GUIContent("Sq Mat Walkable:", "Material to used for square-tile as effective target tile for ability when in ability target selection, with ability targetType set to all");
					instance.sqMatAbilityAll=(Material)EditorGUILayout.ObjectField(cont, instance.sqMatAbilityAll, typeof(Material), true);
					cont=new GUIContent("Sq Mat Walkable:", "Material to used for square-tile as effective target tile for ability when in ability target selection, with ability targetType set to hostile");
					instance.sqMatAbilityHostile=(Material)EditorGUILayout.ObjectField(cont, instance.sqMatAbilityHostile, typeof(Material), true);
					cont=new GUIContent("Sq Mat Walkable:", "Material to used for square-tile as effective target tile for ability when in ability target selection, with ability targetType set to friendly");
					instance.sqMatAbilityFriendly=(Material)EditorGUILayout.ObjectField(cont, instance.sqMatAbilityFriendly, typeof(Material), true);
					
					cont=new GUIContent("Sq Mat Walkable:", "Material to used for square-tile when hidden in fog-of-war");
					instance.sqMatInvisible=(Material)EditorGUILayout.ObjectField(cont, instance.sqMatInvisible, typeof(Material), true);
				}
				
				
			EditorGUILayout.Space();
				
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("", GUILayout.MaxWidth(10));
			showDefaultFlag=EditorGUILayout.Foldout(showDefaultFlag, "Show default editor");
			EditorGUILayout.EndHorizontal();
			if(showDefaultFlag) DrawDefaultInspector();
			
			
			if(GUI.changed) EditorUtility.SetDirty(instance);
			
		}
		
		
		
	}

	
}