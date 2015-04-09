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


namespace Cards
{
	public class CardsHandManager : MonoBehaviour
	{

		public enum modes {_DeckBuild = 1, _GameOn = 2};

		public static CardsHandManager instance;
		public static int handSize = 5;
		public Transform deck;

		public static bool movingCard = false;

		public CardTransform selectedCard;

		public CardsList cardsInDeck;

		public CardsList cardsInDiscard;

		public CardsList cardsInHand;

		private float distanceFromCenter =8f;
		private float distanceFromOtherCard;

		//this variable is used to store in what mode the game is on, so the card activations will have different effects
		public modes mode;

		public CardPrefabInstatiator instantiator;

		//this position is used to store the cards that won't be used
		public Vector3 cardsLimbo = new Vector3(9999,9999,9999);

		public Vector3 managerPosition;
		public Vector3 managerRotation;


		void Awake ()
		{
			if (instance == null) {
				instance = this;
				instance.cardsInDeck = new CardsList ();
				instance.cardsInHand = new CardsList ();
				instance.cardsInDiscard = new CardsList();
				instantiator = new CardPrefabInstatiator ();
				SetUpInstantiator();
			} else {
				Destroy (this.gameObject);
			}

			DontDestroyOnLoad (this.gameObject);

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

			transform.SetParent (Camera.allCameras [0].transform);

			transform.localPosition = instance.managerPosition;
			transform.localRotation = Quaternion.Euler (instance.managerRotation);

//			Debug.Log ("Position is " + transform.position);
//			Debug.Log ("Rotation is " + transform.rotation);

			if (instance.mode == modes._GameOn) {
				Debug.Log ("Game On");
				CreateDeck();
				updateHand();
				updateCardsPosition ();
				
			} else {
				
			}

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

	
		public void updateCardsPosition(){
			int initialAngle = -15;
			float range = 7.5f*handSize;
			float increment = range/handSize;

			int cardCount = 0;


			foreach(Card c in cardsInHand.list){

				float angle = initialAngle + increment*cardCount;
//				Debug.Log (string.Format("The angle is {0}, and the increment is {1}, the cardCount is {2}", angle, increment, cardCount));

				Vector3 finalPosition = transform.position;

				finalPosition.x += distanceFromCenter*Mathf.Sin(Mathf.Deg2Rad*angle);
				finalPosition.y += 0.05f*cardCount;
				finalPosition.z += distanceFromCenter*Mathf.Cos(Mathf.Deg2Rad*angle);
		
//				Debug.Log ("Local scale before start is:  " + this.transform.localScale); 
				c.updateTransform(finalPosition, Quaternion.Euler(90,  angle, 0), c.transform.localScale);
				
				cardCount++;
			}
//0.8, -10.4, 12
		}

		public void updateHand(){
//			Debug.Log ("Child Count: " + instance.transform.childCount);
//			Debug.Log ("Hand Size: " + handSize);
			if (handSize - cardsInHand.getCount () > cardsInDeck.getCount ())
				_ShuffleDeck ();

			Debug.Log (instance.cardsInDeck.getCount ());
			if (cardsInHand.getCount() < handSize && instance.cardsInDeck.getCount()> 0) {
				for(int a = handSize - cardsInHand.getCount(); a != 0 &&  cardsInDeck.getCount() != 0; a--){
					Card pop = instance.cardsInDeck.popFirstCard();
					pop.transform.SetParent(this.transform);
					cardsInHand.addCard(pop);
				}
			}
			updateCardsPosition ();
		}

		public static void ShuffleDeck(){
			instance._ShuffleDeck ();
		}

		private void _ShuffleDeck(){
			if (cardsInDiscard.list.Count > 0) {
				do {
					int position = Random.Range(0, cardsInDiscard.getCount());
					cardsInDeck.addCard(cardsInDiscard.removeCardAt(position));
				} while(cardsInDiscard.getCount() > 0);
			}
		}

		public static void DeselectCard(){
			instance._DeselectCard ();
		}

		private void _DeselectCard(){
			selectedCard = null;
		}


		public static void CreateDeck(){
			instance._CreateDeck ();
		}

		private void _CreateDeck(){
//			Debug.Log (instantiator.cardsToInstantiate.Count);
//			GameObject deck = GameObject.FindGameObjectWithTag ("");
			foreach (GameObject t in instantiator.cardsToInstantiate) {
				GameObject card = (GameObject) Instantiate(t, cardsLimbo, Quaternion.identity);
				Vector3 baseScale = new Vector3 (0.15f, 0.21f, 0.21f);
//				Vector3 baseScale = this.transform.localScale;
				card.transform.localScale= baseScale;
				CardTransform.baseScale = baseScale;
				instance.cardsInDiscard.addCard((Card) card.GetComponent<Card>());
			}
		}

	}
}

