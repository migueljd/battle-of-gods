using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Cards;


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
		if (lvl <= 2)
			return "Forest";
		else if (lvl <= 3)
			return "Desert";
		else
			return "Underworld";
	}

	public static int GetTileCount(int lvl){
		switch (lvl) {
		case 1:
			return 5;
		case 2:
			return 6;
		case 3:
			return 7;
		case 4:
			return 8;
		case 5:
			return 9;
		case 6:
			return 10;
		case 7:
			return 11;
		default:
			return 12;
		}
	}

	public static List<Transform> GetEnemyPrefabsForLevel(int lvl){
		List<Transform> enemyPrefabs = new List<Transform> ();
		string levelName = GetLevelName (lvl);

		if (lvl < 3) {
			enemyPrefabs.Add (Resources.Load ("Prefabs/Units/" + levelName + "/Cyclop", typeof(Transform)) as Transform);
		} else if (lvl < 6) {
			enemyPrefabs.Add (Resources.Load ("Prefabs/Units/" + levelName + "/Cyclop", typeof(Transform)) as Transform);
			enemyPrefabs.Add (Resources.Load ("Prefabs/Units/" + levelName + "/Centaur", typeof(Transform)) as Transform);
		} else if (lvl < 9) {
			enemyPrefabs.Add (Resources.Load ("Prefabs/Units/" + levelName + "/Cyclop", typeof(Transform)) as Transform);
			enemyPrefabs.Add (Resources.Load ("Prefabs/Units/" + levelName + "/Centaur", typeof(Transform)) as Transform);
			enemyPrefabs.Add (Resources.Load ("Prefabs/Units/" + levelName + "/Minotaur", typeof(Transform)) as Transform);
		} else if (lvl < 11) {
			enemyPrefabs.Add (Resources.Load ("Prefabs/Units/" + levelName + "/Cyclop_lvl2_", typeof(Transform)) as Transform);
			enemyPrefabs.Add (Resources.Load ("Prefabs/Units/Forest/Centaur", typeof(Transform)) as Transform);
			enemyPrefabs.Add (Resources.Load ("Prefabs/Units/Forest/Minotaur", typeof(Transform)) as Transform);
		} else if (lvl < 14) {
			enemyPrefabs.Add (Resources.Load ("Prefabs/Units/" + levelName + "/Cyclop_lvl2_", typeof(Transform)) as Transform);
			enemyPrefabs.Add (Resources.Load ("Prefabs/Units/" + levelName + "/Centaur_lvl2_", typeof(Transform)) as Transform);
			enemyPrefabs.Add (Resources.Load ("Prefabs/Units/Forest/Minotaur", typeof(Transform)) as Transform);
		} else if (lvl < 17) {
			enemyPrefabs.Add (Resources.Load ("Prefabs/Units/" + levelName + "/Cyclop_lvl2_", typeof(Transform)) as Transform);
			enemyPrefabs.Add (Resources.Load ("Prefabs/Units/" + levelName + "/Centaur_lvl2_", typeof(Transform)) as Transform);
			enemyPrefabs.Add (Resources.Load ("Prefabs/Units/" + levelName + "/Minotaur_lvl2_", typeof(Transform)) as Transform);
		}else if (lvl < 19) {
			enemyPrefabs.Add (Resources.Load ("Prefabs/Units/" + levelName + "/Cyclop_lvl3_", typeof(Transform)) as Transform);
			enemyPrefabs.Add (Resources.Load ("Prefabs/Units/Desert/Centaur_lvl2_", typeof(Transform)) as Transform);
			enemyPrefabs.Add (Resources.Load ("Prefabs/Units/Desert/Minotaur_lvl2_", typeof(Transform)) as Transform);
		}else if (lvl < 22) {
			enemyPrefabs.Add (Resources.Load ("Prefabs/Units/" + levelName + "/Cyclop_lvl3_", typeof(Transform)) as Transform);
			enemyPrefabs.Add (Resources.Load ("Prefabs/Units/" + levelName + "/Centaur_lvl3_", typeof(Transform)) as Transform);
			enemyPrefabs.Add (Resources.Load ("Prefabs/Units/Desert/Minotaur_lvl2_", typeof(Transform)) as Transform);
		}else if (lvl < 25) {
			enemyPrefabs.Add (Resources.Load ("Prefabs/Units/" + levelName + "/Cyclop_lvl3_", typeof(Transform)) as Transform);
			enemyPrefabs.Add (Resources.Load ("Prefabs/Units/" + levelName + "/Centaur_lvl3_", typeof(Transform)) as Transform);
			enemyPrefabs.Add (Resources.Load ("Prefabs/Units/" + levelName + "/Minotaur_lvl3_", typeof(Transform)) as Transform);
		}


		return enemyPrefabs;
	}

	public static int GetSceneLevel(int lvl){
		if (lvl < 9)
			return 0;
		else if (lvl < 17)
			return 1;
		else 
			return 2;
	}

	public static Dictionary<int, string> GetCardsForLevel(int lvl){
		Dictionary<int, string> cardList = new Dictionary<int, string>();

		if (lvl < 5) {
			cardList.Add (40, "Athena's Touch");
			cardList.Add (80, "HeroicStrike");
			cardList.Add (100, "Zeus Thunder");

		} else if (lvl < 9) {
			cardList.Add (30, "Athena's Touch");
			cardList.Add (60, "HeroicStrike");
			cardList.Add (85, "Zeus Thunder");
			cardList.Add (100, "Afrodite's Blessing");

		} else {
			cardList.Add (25, "Athena's Touch");
			cardList.Add (50, "HeroicStrike");
			cardList.Add (65, "Zeus Thunder");
			cardList.Add (80, "Hefestus Curse");
			cardList.Add (95, "Afrodite's Blessing");
			cardList.Add (100, "Ares Wrath");
		}

		return cardList;
	
	}

	public static Dictionary<string, int> GetStartingCards(){
		Dictionary<string, int> cardList = new Dictionary<string, int>();

		cardList.Add ("Athena's Touch",3 );
		cardList.Add ("HeroicStrike", 2);

		return cardList;
		
	}
}
