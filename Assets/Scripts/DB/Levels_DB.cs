using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Levels_DB : MonoBehaviour {

	public static Object[] GenerateLevelPrefabList(int lvl){
		string finalPath = GetLevelName(lvl);

		Object[] temp = Resources.LoadAll ("Prefabs/Environment/Tiles/" + finalPath);
		Object[] tiles;


		if(lvl <25){
			if (lvl % 8 == 1){
				tiles = new Object[2];
				tiles = GetTilesPerLevelType(1, temp, tiles);
			}
			else if (lvl % 8 == 2){
				tiles = new Object[3];
				tiles = GetTilesPerLevelType(2, temp, tiles);
			}
			else if (lvl % 8 == 3){
				tiles = new Object[4];
				tiles = GetTilesPerLevelType(3, temp, tiles);
			}
			else{
				tiles = new Object[5];
				tiles = GetTilesPerLevelType(4, temp, tiles);
			}
		}
		else{
			tiles = new Object[5];
			tiles = GetTilesPerLevelType(4, temp, tiles);
		}
		return tiles;
	}

	private static Object[] GetTilesPerLevelType(int code, Object[] temp, Object[] tiles){
		int b = 0;
		for(int a = 0; a < temp.Length; a ++){
			if(temp[a].name.Contains("Temple")) continue;
			if(code == 1){
				if(temp[a].name.Contains("1") || temp[a].name.Contains("2")){
					tiles[b] = temp[a];
					b++;
				}
			}
			else if(code == 2){
				if( temp[a].name.Contains("1") || temp[a].name.Contains("2") || temp[a].name.Contains("3")){
					tiles[b] = temp[a];
					b++;
				}
			}
			else if(code == 3){
				if(!temp[a].name.Contains("5")){
					tiles[b] = temp[a];
					b++;
				}
			}
			else if(code == 4){
				tiles[b] = temp[a];
				b++;
			}
		}
		return tiles;
	}

	public static string GetLevelName(int lvl){
		if (lvl <= 8)
			return "Forest";
		else if (lvl <= 16)
			return "Desert";
		else
			return "Underworld";
	}

	public static int GetTileCount(int lvl){
		switch (lvl) {
		case 1:
			return 5;
		case 2:
			return 5;
		case 3:
			return 5;
		case 4:
			return 5;
		case 5:
			return 5;
		case 6:
			return 13;
		case 7:
			return 14;
		default:
			return 15;
		}
	}

	public static List<Transform> GetEnemyPrefabsForLevel(int lvl){
		List<Transform> enemyPrefabs = new List<Transform> ();
		string levelName = GetLevelName (lvl);

		enemyPrefabs.Add (Resources.Load ("Prefabs/Units/Centaur", typeof(Transform)) as Transform);
		enemyPrefabs.Add(Resources.Load("Prefabs/Units/minotaur", typeof(Transform)) as Transform);
		enemyPrefabs.Add (Resources.Load ("Prefabs/Units/Cyclop", typeof(Transform)) as Transform);

		return enemyPrefabs;
	}
}
