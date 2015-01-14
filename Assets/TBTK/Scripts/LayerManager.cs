using UnityEngine;
using System.Collections;

using TBTK;

namespace TBTK{

	public class LayerManager{
		
		private static int layerUnit=31;
		//private static int layerUnitAI=30;
		private static int layerUnitInvisible=29;
		
		private static int layerTile=28;
		private static int layerObstacleHalfCover=27;
		private static int layerObstacleFullCover=26;
		private static int layerTerrain=25;
		
		
		
		public static int GetLayerUnit(){ return layerUnit; }
		//public static int GetLayerUnitAI(){ return layerUnit; }
		public static int GetLayerUnitInvisible(){ return layerUnitInvisible; }
		
		public static int GetLayerTile(){ return layerTile; }
		public static int GetLayerObstacleHalfCover(){ return layerObstacleHalfCover; }
		public static int GetLayerObstacleFullCover(){ return layerObstacleFullCover; }
		public static int GetLayerTerrain(){ return layerTerrain; }
		
		public static int LayerUI(){ return 5; }	//layer5 is named UI by Unity's default
		
	}

}
