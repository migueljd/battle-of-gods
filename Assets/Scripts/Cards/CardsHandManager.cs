// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 4.0.30319.1
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


using TBTK;

namespace Cards
{
	public class CardsHandManager : MonoBehaviour
	{

		public enum modes {_DeckBuild = 1, _GameOn = 2};

		public static CardsHandManager instance;
		public int handSize = 6;
		public Transform deck;

		public static bool movingCard = false;

		public CardTransform selectedCard;

//		public CardsList cardsInDeck;

		public CardsList cardsInDiscard;

		public CardsList cardsInHand;

		private float distanceFromCenter =8f;

		//this variable is used to store in what mode the game is on, so the card activations will have different effects
		public modes mode;

		public CardPrefabInstatiator instantiator;

		//this position is used to store the cards that won't be used
		public Vector3 cardsLimbo = new Vector3(0,0,0);

		public Vector3 managerPosition;
		public Vector3 managerRotation;

		public float DropCardChance = 60;

		private static bool startedOnce = false;

		public Vector3 baseScale = new Vector3 (0.30f, 0.42f, 0.42f);


		void Awake ()
		{
			if (instance == null) {
				instance = this;
//				instance.cardsInDeck = new CardsList ();
				instance.cardsInHand = new CardsList ();
				instance.cardsInDiscard = new CardsList();
				instantiator = new CardPrefabInstatiator ();
				SetUpInstantiator();
				Debug.Log ("Instance is null");
				DontDestroyOnLoad (this.gameObject);
			} else {
				
				Debug.Log ("Instance is not null");
				Destroy (this.gameObject);
			}


		}


		void OnLevelWasLoaded(int lvl){
			if(instance == this)
				OnEnable ();
		}

		void OnEnable(){
			if (instance == this) {

				enabled = true;
				Debug.Log ("On Enable called");
				Unit.onUnitDestroyedE += OnUnitDestroyed;
				GameControl.onPassLevelE += PassLevel;
				GameControl.onGameStartE += GameStarted;
			}
		}

		void OnDisable(){
			if (instance == this) {

				Unit.onUnitDestroyedE -= OnUnitDestroyed;
				GameControl.onPassLevelE -= PassLevel;
				GameControl.onGameStartE -= GameStarted;
			}
		}

		private void SetUpInstantiator(){
			Object[] o = Resources.LoadAll ("Prefabs/Cards/");
			for(int a = 0; a < o.Length; a++){
				for(int b = 0; b < 5; b++){
					instantiator.addPrefab ((GameObject)o[a]);
				}
			}
			changeModeToGameOn ();

		}

		public void GameStarted (){
			GameControl.AddActionAtStart ();

			transform.SetParent (GameObject.FindWithTag("CardsCamera").transform);

			transform.localPosition = instance.managerPosition;
			transform.localRotation = Quaternion.Euler (instance.managerRotation);
//			Debug.Log ("Position is " + transform.position);
//			Debug.Log ("Rotation is " + transform.rotation);

			if (instance.mode == modes._GameOn && !startedOnce) {
				startedOnce = true;
//				CreateDeck();
//				updateHand();
//				_UpdateCardsPosition ();
				StartupCards();
				CreateStartingHand();
			} else if (startedOnce) {
				_UpdateCardsPosition ();
			}
			GameControl.CompleteActionAtStart ();
		}



		public static void changeModeToDeckBuild(){
			instance.mode = modes._DeckBuild;
		}

		public static void changeModeToGameOn(){
			Debug.Log ("mode changed");
			instance.mode = modes._GameOn;
		}

		public static CardsHandManager getInstance(){
			return instance;
		}

		public static void UpdateCardsPosition(){
			instance._UpdateCardsPosition();
		}
	
		private void _UpdateCardsPosition(){
			int initialAngle = -12;
			float range = 4f*handSize;
			float increment = range/handSize;

			int cardCount = 0;


			foreach(Card c in cardsInHand.list){
				movingCard = true;
				float angle = initialAngle + increment*cardCount;
//				Debug.Log (string.Format("The angle is {0}, and the increment is {1}, the cardCount is {2}", angle, increment, cardCount));

				Vector3 finalPosition = transform.position;

				finalPosition.x += distanceFromCenter*Mathf.Sin(Mathf.Deg2Rad*angle);
				finalPosition.y += 0.02f*cardCount;
				finalPosition.z += distanceFromCenter*Mathf.Cos(Mathf.Deg2Rad*angle);
		
//				Debug.Log ("Local scale before start is:  " + this.transform.localScale); 
				c.updateTransform(finalPosition, Quaternion.Euler(90,  angle, 0), c.transform.localScale);
				
				cardCount++;
			}
			StartCoroutine (WaitForCardsToUpdate());
//0.8, -10.4, 12
		}

		IEnumerator WaitForCardsToUpdate(){ 
			bool done = false;
			do {
				done = true;
				foreach (Card c in cardsInHand.list) {
					if (c.shouldUpdate)
						done = false;
				}
				yield return new WaitForSeconds (0.1f);
			} while(!done);
			movingCard = false;
		}

//		public void updateHand(){
//			Debug.Log ("Child Count: " + instance.transform.childCount);
//			Debug.Log ("Hand Size: " + handSize);
//			if (handSize - cardsInHand.getCount () > cardsInDeck.getCount ())
//				_ShuffleDeck ();
//
//			Debug.Log (instance.cardsInDeck.getCount ());
//			if (cardsInHand.getCount() < handSize && instance.cardsInDeck.getCount()> 0) {
//				for(int a = handSize - cardsInHand.getCount(); a != 0 &&  cardsInDeck.getCount() != 0; a--){
//					Card pop = instance.cardsInDeck.popFirstCard();
//					pop.transform.SetParent(this.transform);
//					cardsInHand.addCard(pop);
//				}
//			}
//			_UpdateCardsPosition ();
//			this.transform.localPosition = managerPosition;
//		}

		private void CreateStartingHand(){
			Dictionary<string, int> startingHand = Levels_DB.GetStartingCards ();

			foreach (string key in startingHand.Keys) {
				int count = 0;
				startingHand.TryGetValue(key, out count);
				foreach (Card c in cardsInDiscard.list) {
					if(c.name.Equals(key+"(Clone)") && count >0){
						count--;
						cardsInHand.addCard(c);
						c.transform.parent = this.transform;
					}
				}
			}
			foreach (Card c in cardsInHand.list) {
				cardsInDiscard.removeCard(c);
			}
			_UpdateCardsPosition ();
		}

////		public static void ShuffleDeck(){
//			instance._ShuffleDeck ();
//		}

//		private void _ShuffleDeck(){
//			if (cardsInDiscard.list.Count > 0) {
//				do {
//					int position = Random.Range(0, cardsInDiscard.getCount());
////					cardsInDeck.addCard(cardsInDiscard.removeCardAt(position));
//				} while(cardsInDiscard.getCount() > 0);
//			}
//		}

		public static void DeselectCard(){
			instance._DeselectCard ();
		}

		private void _DeselectCard(){
			selectedCard = null;
		}


		public static void CreateDeck(){
			instance.StartupCards ();
		}

		private void StartupCards(){
//			Debug.Log (instantiator.cardsToInstantiate.Count);
//			GameObject deck = GameObject.FindGameObjectWithTag ("");
			foreach (GameObject t in instantiator.cardsToInstantiate) {
				GameObject card = (GameObject) Instantiate(t, cardsLimbo, Quaternion.identity);
//				Vector3 baseScale = this.transform.localScale;
				card.transform.localScale= baseScale;
				CardTransform.baseScale = baseScale;
				instance.cardsInDiscard.addCard((Card) card.GetComponent<Card>());
			}
		}

		private void OnUnitDestroyed(Unit unit){
			Debug.Log ("Tried to get a card");
			Debug.Log (unit.factionID);
			Debug.Log (FactionManager.GetPlayerFactionID () [0]);
			Debug.Log ("Faction Man: " + !FactionManager.IsPlayerFaction (unit.factionID));
			Debug.Log ("cardsInHand: " + (cardsInHand == null));
			Debug.Log ("Unit: " + unit);
			if (!FactionManager.IsPlayerFaction (unit.factionID) && cardsInHand.getCount () < handSize) {
				float getCard = Random.Range (0, 100);
				Debug.Log ("GetCard was" + getCard);
				if (getCard <= DropCardChance) {
					Debug.Log ("Received a card");

					Dictionary<int, string> cards = Levels_DB.GetCardsForLevel (MapController.level);

					int card = Random.Range (0, 101);

					string cardName = "";
					int lowestDifference = int.MaxValue;
					int final = 0;
					foreach (int key in cards.Keys) {
						if (key >= card && lowestDifference > key - card) {
							lowestDifference = key - card;
							final = key;
						}
					}

					cards.TryGetValue (final, out cardName);

					Card won = null;

					foreach (Card c in cardsInDiscard.list) {
						if (c.name.Equals (cardName + "(Clone)")) {
							won = c;
						}
					}
					if (won != null)
						cardsInDiscard.removeCard (won);
					else
						won = (Instantiate (Resources.Load ("Prefabs/Cards/" + cardName), cardsLimbo, Quaternion.identity) as GameObject).GetComponent<Card>();

					won.transform.parent = this.transform;

					won.transform.localScale= baseScale;
					CardTransform.baseScale = baseScale;

					cardsInHand.addCard(won);
					_UpdateCardsPosition();
				}
			}
		}

		public static void Disattach(){
			Debug.Log ("Disataching");
			instance.transform.parent = null;
		}

		public static void PassLevel(){
			Disattach ();
		}


	}
}

