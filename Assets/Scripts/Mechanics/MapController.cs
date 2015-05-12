using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TBTK;

public class MapController : MonoBehaviour {

	public Transform tilesPrefab;

	public List<Transform> enemyPrefabs;


	public static MapController instance;

	public static int level = 1;

	public float rangeTunel = 0.3f;
	
	public string levelName = "Forest";


	private IList<Transform> generatedTileList;

	public Transform portalPrefab;


	private float height;
	private float line;

	public static float rotation =-60;


	private int numberOfTiles = 30;

	public static int enemyFactionID;

	private bool gameStarted;

	private bool hadesAlive;

	void Awake(){
		if (instance == null) {
			instance = this;
		} else if(!instance.Equals(this)){
			Debug.Log ("Destroying object with instance id: " + gameObject.GetInstanceID());
			Destroy(this.gameObject);
		}

	}

	void Start(){		
		if(!gameStarted)gameObject.SendMessage("OnLevelWasLoaded",Application.loadedLevel);

		foreach (Faction f in FactionManager.GetFactionList()) {
			if (!FactionManager.IsPlayerFaction (f.ID)) {
				enemyFactionID = f.ID;
			}
		}
	}

	void OnEnable(){
		GameControl.onPassLevelE += PassLevel;
		GameControl.onGameRestartE += RestartGame;
	}
	
	void OnDisable(){
		GameControl.onPassLevelE -= PassLevel;
		GameControl.onGameRestartE -= RestartGame;
	}

	void OnLevelWasLoaded(int levelLoaded){
		this.gameStarted = false;
		generatedTileList = new List<Transform> ();
		
		//setting all variables from the DB
		numberOfTiles = Levels_DB.GetTileCount (level);
		levelName = Levels_DB.GetLevelName (level);
		Object[] tiles = Levels_DB.GenerateLevelPrefabList(level);
		instance.enemyPrefabs = Levels_DB.GetEnemyPrefabsForLevel(level);



		//Setting class variables that will be necessary for each run
		SetNewLevel (numberOfTiles, levelName);
		populatePrefabList (levelName, tiles);

		
		line = GridGenerator.spaceXHex * GridManager.GetInstance().tileSize * GridManager.GetInstance().gridToTileRatio/1.5f;
		height = line * Mathf.Sin (Mathf.PI / 3);


		//setting up initial map
		SetupMap (levelName);



	}

	private void SetupMap(string levelName){
		Tile initialTile = GameObject.FindGameObjectsWithTag("Tile_0")[0].GetComponent<Tile>();

		List<Unit> playerUnits = FactionManager.GetAllPlayerUnits ();

		foreach(Tile t in initialTile.GetNeighbourList(true)){
			if(t.tileNumber == 1){

				_RevealArea(t);

				Unit unit = playerUnits[1];

				unit.tile.unit = null;
				unit.tile = t;
				t.unit = unit;

				unit.transform.position = t.transform.position;
	

				foreach(Tile neighbour in t.GetNeighbourList(true)){
				
					if(neighbour.tileNumber == 5){
						unit = playerUnits[0];
						
						unit.tile.unit = null;
						unit.tile = neighbour;
						neighbour.unit = unit;
						
						unit.transform.position = neighbour.transform.position;
					}
					else if(neighbour.tileNumber == 3){
						unit = playerUnits[2];
						
						unit.tile.unit = null;
						unit.tile = neighbour;
						neighbour.unit = unit;
						
						unit.transform.position = neighbour.transform.position;
					}
				
				}
//
//				
//				
//				List<Unit> playerUnits = FactionManager.GetAllPlayerUnits();
//				
//				Unit unit = playerUnits[1];
//				
//				unit.tile.unit = null;
//				unit.tile = t;
//				t.unit = unit;
//				
//				unit.transform.position = t.transform.position;
				this.gameStarted = true;
			}
		}

	}

	private void populatePrefabList(string finalPath, Object[] tiles){

		for (int a =0; a < numberOfTiles - 1; a++) {
			int b = Random.Range(0, tiles.Length);
			generatedTileList.Add(((GameObject)tiles[b]).transform);
		}
		generatedTileList.Add(((GameObject) Resources.Load ("Prefabs/Environment/Tiles/" + finalPath + "/Temple")).transform);

	}




	//this method is called when it's necessary to instantiate a new area, given a tile
	public static void RevealArea(Tile tile){
		Debug.Log ("Reveal Area called");
		instance._RevealArea (tile);
	}

	private void _RevealArea(Tile tile){
//		IList<Tile> tileList = tile.GetNeighbourList ();


		//positioning tile according to it's tile number

		//distance for second int x = 2 * width
		//distance for first int x = Mathf.Sin (60f) * width * 4;

		GridManager gridInstance = GridManager.GetInstance ();

		Vector3 firstVector = instance.findFirstVector (tile);
		
		//-60*number - 90
		Vector3 secondVector = instance.findSecondVector (tile);

		Vector3 sourcePosition = tile.transform.position;

		Transform first = null;
		Transform second = null;
		int firstRotation = Random.Range(0,5);
//		Debug.Log ("First rotation: " + firstRotation*rotation);
		int secondRotation = Random.Range(0,5);
//		Debug.Log ("Second rotation: " + secondRotation);

		if (generatedTileList.Count == 0)
			return;
		tilesPrefab = generatedTileList [0];
		generatedTileList.RemoveAt (0);
		Debug.Log (tile.revealed);
		switch (tile.revealed) {
			case 0:
				first = (Transform) Instantiate(tilesPrefab, sourcePosition + firstVector, Quaternion.Euler(new Vector3(0,firstRotation*rotation,0)));
				
				//adjusting camera movement for first tile created
				if(firstVector.x > 0 ) CameraControl.IncreaseMaxCameraMovement(true,false);
				else CameraControl.DecreaseMinCameraMovement(true, false);

				if(firstVector.z > 0 ) CameraControl.IncreaseMaxCameraMovement(false,true);
				else CameraControl.DecreaseMinCameraMovement(false, true);
			

				if (generatedTileList.Count != 0){
					tilesPrefab = generatedTileList [0];
					generatedTileList.RemoveAt (0);    
					second = (Transform) Instantiate(tilesPrefab, sourcePosition + secondVector, Quaternion.Euler(new Vector3(0,secondRotation*rotation,0)));
					
					//adjusting camera movement for second created
					if(secondVector.x > 0 ) CameraControl.IncreaseMaxCameraMovement(true,false);
					else CameraControl.DecreaseMinCameraMovement(true, false);
					
					if(secondVector.z > 0 ) CameraControl.IncreaseMaxCameraMovement(false,true);
					else CameraControl.DecreaseMinCameraMovement(false, true);
				}
				break;
			case 1:
				//adjusting camera movement for second created
				if(secondVector.x > 0 ) CameraControl.IncreaseMaxCameraMovement(true,false);
				else CameraControl.DecreaseMinCameraMovement(true, false);
				
				if(secondVector.z > 0 ) CameraControl.IncreaseMaxCameraMovement(false,true);
				else CameraControl.DecreaseMinCameraMovement(false, true);
				second = (Transform) Instantiate(tilesPrefab, sourcePosition + secondVector, Quaternion.Euler(new Vector3(0,secondRotation*rotation,0)));
				
				break;
			case 2:
			    first = (Transform) Instantiate(tilesPrefab, sourcePosition + firstVector, Quaternion.Euler(new Vector3(0,firstRotation*rotation,0)));
				//adjusting camera movement for first tile created
				if(firstVector.x > 0 ) CameraControl.IncreaseMaxCameraMovement(true,false);
				else CameraControl.DecreaseMinCameraMovement(true, false);
				
				if(firstVector.z > 0 ) CameraControl.IncreaseMaxCameraMovement(false,true);
				else CameraControl.DecreaseMinCameraMovement(false, true);
				break;
		}

		//this list will be used to logically add all tiles to their respective neighbours
		List<Tile> addedTiles = new List<Tile>();


		//add all new tiles to the gameObject hierarchy as well as add all of them to the existing tiles logic
		if (first != null) {
			InitTiles (first, gridInstance, addedTiles, (line*2) + rangeTunel, firstRotation);
			addTiles (tile, addedTiles,(line*2) + rangeTunel);
			addedTiles = new List<Tile>();
		}

		if (second != null) {
			InitTiles (second, gridInstance, addedTiles, (line*2) + rangeTunel, secondRotation);
			addTiles (tile, addedTiles,(line*2) + rangeTunel);
		}

		if(first != null)	GameObject.Destroy(first.gameObject);
		if(second != null) 	GameObject.Destroy(second.gameObject);


	}

	//Initialize all new tiles, so all their logic is ready to be appended to the existing tiles
	private void InitTiles(Transform tilesT, GridManager instance, List<Tile> added, float range, int rotation){
		List<Tile> tileList = new List<Tile> ();

//		int enemyCount = 0;
		int enemyCount = Random.Range(tilesT.GetChild(0).GetComponent<Tile>().tile0.minEnemyCount,tilesT.GetChild(0).GetComponent<Tile>().tile0.maxEnemyCount+1);
		for (int a = tilesT.childCount - 1; a >=0; a--) {

			if(!tilesT.GetChild(a).name.Contains("Tile")){
				tilesT.GetChild(a).transform.SetParent(GameObject.FindGameObjectWithTag ("MainGrid").transform);
				continue;
			}
			Tile tile = (Tile)tilesT.GetChild (a).GetComponent<Tile> ();
			tileList.Add(tile);
			int tileNumber = tile.tileNumber + rotation;
			if(tileNumber > 6) tileNumber = tileNumber%7 + 1;
			tile.tileNumber = tile.name != "Tile_0" ? tileNumber : 0;

			//----------------------------------- Spawn code start



			if(gameStarted){
				//it's necessary that an enemy is instantiated here
				if((a-1) - (enemyCount) == 0 && tile.walkable && enemyCount > 0){
					enemyCount --;
					createNewEnemy(tile); 
				}
				//it it's possible that this tile has no enemy, toss a coin
				else if(Random.Range(0,2) >1 && tile.walkable && enemyCount > 0){
					enemyCount --;
					createNewEnemy(tile);
					}
			}

			//----------------------------------- Spawn code end
			tile.setTileAttributes();
		}
		foreach(Tile tile in tileList){

			tile.aStar=new TileAStar(tile);
			
			List<Tile> neighbourList=new List<Tile>();

			foreach(Tile t in tileList){
				if(!t.Equals(tile) && Vector3.Distance(t.transform.position, tile.transform.position) <= range) neighbourList.Add(t);
			}
			
			tile.aStar.SetNeighbourList(neighbourList);

			instance.grid.tileList.Add(tile);

			added.Add(tile);
			if(neighbourList.Count == 6) tile.revealed = 3;
		}

	}
	
	//logically add all new tiles to the  existing tiles
	private void addTiles(Tile origin, List<Tile> tileList, float range){
		Tile currentTile = origin;

		List<Tile> openList = new List<Tile> ();
		List<Tile> closeList = new List<Tile> ();

		while (currentTile != null) {
			closeList.Add(currentTile);
			currentTile.aStar.listState = TileAStar._AStarListState.Close;
			List<Tile> newNeighbourList = currentTile.GetNeighbourList();

			Vector3 firstVector = instance.findFirstVector(currentTile);
			Vector3 secondVector = instance.findSecondVector(currentTile);

			bool first = false;
			bool second = false;



			foreach(Tile t in tileList){
//				if(t.Equals(currentTile)){Debug.Log(string.Format("T is {0} and currenTile is {1} and their respective positions are {2} and {3}", t, currentTile, t.transform.position, currentTile.transform.position));}
				if(Vector3.Distance(currentTile.transform.position, t.transform.position) <=range && !newNeighbourList.Contains(t) ){
					List<Tile> newList = t.GetNeighbourList();
					newList.Add(currentTile);
					t.aStar.SetNeighbourList(newList);
					newNeighbourList.Add(t);

					//check if this would be the first or second new tile for currentTile, the bools will be checked afterwards to know what
					//should be done

//					if(currentTile.name == "Tile_2" && Vector3.Distance(currentTile.transform.position, new Vector3(4.5f, 0, 2.6f)) <= 0.5f) Debug.Log (string.Format("Second vector is {0} and the difference is {1} and the difference between the expected and this is {2} which makes the bool {3}",secondVector, (t.transform.parent.position - currentTile.transform.position), Vector3.Distance(t.transform.parent.position - currentTile.transform.position, secondVector),t.transform.parent.position - currentTile.transform.position == secondVector));
					if(Vector3.Distance((t.transform.parent.position - currentTile.transform.position),firstVector) <= rangeTunel){
						first = true;

						//now we need to check what this guy represents for t
						Vector3 firstVectorForT = instance.findFirstVector(t);
						Vector3 secondVectorForT = instance.findSecondVector(t);

						bool distanceFromFirst = Vector3.Distance((currentTile.tile0.transform.position - t.transform.position), firstVectorForT) <= rangeTunel;
						bool distanceFromSecond = Vector3.Distance((currentTile.tile0.transform.position - t.transform.position), secondVectorForT) <= rangeTunel;
						updateTileRevealed(t, distanceFromFirst,  distanceFromSecond);
					}
					if(Vector3.Distance((t.transform.parent.position - currentTile.transform.position),secondVector) <= rangeTunel){
//						if(currentTile.name == "Tile_2" && Vector3.Distance(currentTile.transform.position, new Vector3(4.5f, 0, 2.6f)) <= 0.5f) Debug.Log ("Got here");

						second = true;
						//now we need to check what this guy represents for t
						Vector3 firstVectorForT = instance.findFirstVector(t);
						Vector3 secondVectorForT = instance.findSecondVector(t);

						bool distanceFromFirst = Vector3.Distance((currentTile.tile0.transform.position - t.transform.position), firstVectorForT) <= rangeTunel;
						bool distanceFromSecond = Vector3.Distance((currentTile.tile0.transform.position - t.transform.position), secondVectorForT) <= rangeTunel;
						updateTileRevealed(t, distanceFromFirst,  distanceFromSecond);

					}
				}
			}


//			Debug.Log (string.Format("Tile name is {0} and it's revealed number is {1} and it's position is {2} before it was updated", currentTile, currentTile.revealed, currentTile.transform.position));
			updateTileRevealed(currentTile, first, second);
//			Debug.Log (string.Format("Tile name is {0} and it's revealed number is {1} and it's position is {2} after it was updated", currentTile, currentTile.revealed, currentTile.transform.position));
//			Debug.Log ("---------------------------------------------------------------------------------------------------");
			if(first || second){
				foreach(Tile t in currentTile.GetNeighbourList()){
					if(t.aStar.listState != TileAStar._AStarListState.Close && t.revealed != 3){
						openList.Add(t);
					}
				}
			}

			currentTile.aStar.SetNeighbourList(newNeighbourList);

			currentTile = null;
			if(openList.Count != 0){
				currentTile = openList[0];
				openList.RemoveAt(0);
			}
		}

		foreach (Tile t in tileList) {
			t.transform.SetParent(GameObject.FindGameObjectWithTag ("MainGrid").transform);
		}
		
		AStar.ResetGraph (origin, openList, closeList);
	}

	private void updateTileRevealed(Tile tile, bool first, bool second){
		if(tile.revealed == 0 && first) tile.revealed = 1;
		if(tile.revealed == 0 && second) tile.revealed = 2;
		else if(tile.revealed == 1 && second) tile.revealed = 3;
		else if(tile.revealed == 2 && first) tile.revealed =3;
		else if(first && second) tile.revealed = 3;
	}

	private Vector3 findFirstVector(Tile tile){
		
		Quaternion firstRotation = Quaternion.Euler (0, (-60 * (tile.tileNumber - 1)) - 90, 0);
		Vector3 firstVector = (firstRotation * new Vector3 (1, 0, 0) * height * 4 ) + new Vector3(0,-0.002f,0);	

		return firstVector;
	}
	
	private Vector3 findSecondVector(Tile tile){
		Quaternion secondRotation = Quaternion.Euler (0, (-60 * (tile.tileNumber - 1)), 0);
		Vector3 secondVector = (secondRotation * new Vector3 (1, 0, 0) * line * 3) + new Vector3(0,-0.002f,0);
		return secondVector;
	}

	private void createNewEnemy(Tile tile){
		int enemyID = Random.Range (0, MapController.instance.enemyPrefabs.Count);
		if (hadesAlive && enemyID == 3)
			enemyID = Random.Range (0, MapController.instance.enemyPrefabs.Count - 1);
		else if (enemyID == 3)
			hadesAlive = true; 
		Transform newEnemy = (Transform) Instantiate ( MapController.instance.enemyPrefabs [enemyID], tile.GetPos(), Quaternion.Euler(new Vector3(0, 180,0)));

		Unit unit = newEnemy.GetComponent<Unit>();
		unit.tile = tile;
		tile.unit = unit;

		float unitDamage = Random.Range (unit.damageMin, unit.damageMax + 1);

		unit.damageMin = unitDamage;

		FactionManager.InsertUnit (unit, enemyFactionID);

	}

	public static void PassLevel(){
		GameControl.AddActionAtPassLevel ();

		level += 1;

		GameControl.CompleteActionAtPassLevel ();
	}

	public static void HadesDied(){
		instance.hadesAlive = false;
	}

	private void SetNewLevel(int numberOfTiles, string LevelName){
		this.numberOfTiles = numberOfTiles; 
		this.levelName = LevelName;
	}

	public static void RestartGame(){
		level = 1;
	}
}
		                          