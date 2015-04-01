using UnityEngine;
using System.Collections;
using Cards;

namespace Cards{
	public class CardsStackManager : MonoBehaviour {


		private CardsList damageCards;
		private CardsList guardCards;
		private static CardsList moveCards;

		void Awake(){
			damageCards = new CardsList ();
			guardCards = new CardsList();
			moveCards = new CardsList();
		}

		public void updateDamageAndGuard(){
			damageCards.updateList ();
			guardCards.updateList ();
		}

		public void updateMove(){
			moveCards.updateList ();
		}


		//This method should only be called when adding damage cards
		public void addDamageCard(Card card){
			card.currentCount = card.turnCount;
			damageCards.addCard (card);
		}

		//This method should only be called when adding guard cards
		public void addGuardCard(Card card){
			card.currentCount = card.turnCount;
			guardCards.addCard (card);
		}

		//This method should only be called when adding move cards
		public void addMoveCard(Card card){
			card.currentCount = card.turnCount;
			moveCards.addCard (card);
		}

		public static int getMovement(){
			return moveCards.getAttribute(false, false, true);
		}

		
		public int getDamage(){
			return damageCards.getAttribute(true, false, false);
		}
		
		public int getGuard(){
			return guardCards.getAttribute(false, true, false);
		}

		public void decreaseDamage(int decrease){
			damageCards.decreaseAttribute (decrease, true);
		}

		
		public void decreaseGuard(int decrease){
			damageCards.decreaseAttribute (decrease, false, true);
		}

		public static void decreaseMovement(int decrease){
			moveCards.decreaseAttribute (decrease, false, false, true);
		}

		public void updateMovementCount(){
			moveCards.updateAttributesCount ();
		}

		public void updateAttributesForLists(){
			damageCards.updateAttributesCount ();
			guardCards.updateAttributesCount ();
		}
	}
}