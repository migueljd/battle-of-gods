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
		if (lvl <4)
			return "Forest";
		else if (lvl <7)
			return "Desert";
		else
			return "Underworld";
	}

	public static int GetTileCount(int lvl){
		switch (lvl) {
		case 1:
			return 6;
		case 2:
			return 8;
		case 3:
			return 9;
		case 4:
			return 10;
		case 5:
			return 11;
		case 6:
			return 12;
		case 7:
			return 13;
		default:
			return 14;
		}
	}

	public static List<Transform> GetEnemyPrefabsForLevel(int lvl){
		List<Transform> enemyPrefabs = new List<Transform> ();
		string levelName = GetLevelName (lvl);

		if (lvl < 2) {
			enemyPrefabs.Add (Resources.Load ("Prefabs/Units/" + levelName + "/Cyclop", typeof(Transform)) as Transform);
		} else if (lvl < 3) {
			enemyPrefabs.Add (Resources.Load ("Prefabs/Units/" + levelName + "/Cyclop", typeof(Transform)) as Transform);
			enemyPrefabs.Add (Resources.Load ("Prefabs/Units/" + levelName + "/Centaur", typeof(Transform)) as Transform);
		} else if (lvl < 4) {
			enemyPrefabs.Add (Resources.Load ("Prefabs/Units/" + levelName + "/Cyclop", typeof(Transform)) as Transform);
			enemyPrefabs.Add (Resources.Load ("Prefabs/Units/" + levelName + "/Centaur", typeof(Transform)) as Transform);
			enemyPrefabs.Add (Resources.Load ("Prefabs/Units/" + levelName + "/Minotaur", typeof(Transform)) as Transform);
		} else if (lvl < 5) {
			enemyPrefabs.Add (Resources.Load ("Prefabs/Units/" + levelName + "/Cyclop", typeof(Transform)) as Transform);
			enemyPrefabs.Add (Resources.Load ("Prefabs/Units/Forest/Centaur", typeof(Transform)) as Transform);
			enemyPrefabs.Add (Resources.Load ("Prefabs/Units/Forest/Minotaur", typeof(Transform)) as Transform);
		} else if (lvl < 6) {
			enemyPrefabs.Add (Resources.Load ("Prefabs/Units/" + levelName + "/Cyclop", typeof(Transform)) as Transform);
			enemyPrefabs.Add (Resources.Load ("Prefabs/Units/" + levelName + "/Centaur", typeof(Transform)) as Transform);
			enemyPrefabs.Add (Resources.Load ("Prefabs/Units/Forest/Minotaur", typeof(Transform)) as Transform);
		} else if (lvl < 7) {
			enemyPrefabs.Add (Resources.Load ("Prefabs/Units/" + levelName + "/Cyclop", typeof(Transform)) as Transform);
			enemyPrefabs.Add (Resources.Load ("Prefabs/Units/" + levelName + "/Centaur", typeof(Transform)) as Transform);
			enemyPrefabs.Add (Resources.Load ("Prefabs/Units/" + levelName + "/Minotaur", typeof(Transform)) as Transform);
		} else if (lvl < 8) {
			enemyPrefabs.Add (Resources.Load ("Prefabs/Units/" + levelName + "/Cyclop", typeof(Transform)) as Transform);
			enemyPrefabs.Add (Resources.Load ("Prefabs/Units/Desert/Centaur", typeof(Transform)) as Transform);
			enemyPrefabs.Add (Resources.Load ("Prefabs/Units/Desert/Minotaur", typeof(Transform)) as Transform);
		} else if (lvl < 9) {
			enemyPrefabs.Add (Resources.Load ("Prefabs/Units/" + levelName + "/Cyclop", typeof(Transform)) as Transform);
			enemyPrefabs.Add (Resources.Load ("Prefabs/Units/" + levelName + "/Centaur", typeof(Transform)) as Transform);
			enemyPrefabs.Add (Resources.Load ("Prefabs/Units/Desert/Minotaur", typeof(Transform)) as Transform);
		} else if (lvl < 9) {
			enemyPrefabs.Add (Resources.Load ("Prefabs/Units/" + levelName + "/Cyclop", typeof(Transform)) as Transform);
			enemyPrefabs.Add (Resources.Load ("Prefabs/Units/" + levelName + "/Centaur", typeof(Transform)) as Transform);
			enemyPrefabs.Add (Resources.Load ("Prefabs/Units/" + levelName + "/Minotaur", typeof(Transform)) as Transform);
		} else {
			enemyPrefabs.Add (Resources.Load ("Prefabs/Units/" + levelName + "/Cyclop", typeof(Transform)) as Transform);
			enemyPrefabs.Add (Resources.Load ("Prefabs/Units/" + levelName + "/Centaur", typeof(Transform)) as Transform);
			enemyPrefabs.Add (Resources.Load ("Prefabs/Units/" + levelName + "/Minotaur", typeof(Transform)) as Transform);
			enemyPrefabs.Add (Resources.Load ("Prefabs/Units/" + levelName + "/Hades", typeof(Transform)) as Transform);
		}


		return enemyPrefabs;
	}

	public static int GetSceneLevel(int lvl){
		if (lvl < 4)
			return 3;
		else if (lvl < 7)
			return 4;
		else 
			return 5;
	}

	public static Dictionary<int, string> GetCardsForLevel(int lvl){
		Dictionary<int, string> cardList = new Dictionary<int, string>();

		if (lvl < 5) {
			cardList.Add (40, "Athena's Touch");
			cardList.Add (80, "HeroicStrike");
			cardList.Add (100, "Zeus Thunder");

		} else {
			cardList.Add (25, "Athena's Touch");
			cardList.Add (50, "HeroicStrike");
			cardList.Add (70, "Zeus Thunder");
			cardList.Add (85, "Hefestus Curse");
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

	public static AudioClip GetLevelMusic(int lvl){

		if (lvl < 4)
			return (AudioClip)Resources.Load ("Audio/Rushing Wind");
		else if (lvl <7)
			return (AudioClip)Resources.Load ("Audio/Scorched Sands");
		else
			return (AudioClip)Resources.Load ("Audio/Forbidden Lands");

	}
}
