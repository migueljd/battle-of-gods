using UnityEngine;
using System.Collections;

using Cards;

namespace Cards{
	public class Cards : MonoBehaviour{

		public Sprite cardImage;

		public bool damageCard;
		public bool guardCard;
		public bool moveCard;

		public int damage;
		public int movement;
		public int guard;

		public bool isDamageCard(){
			return damageCard;
		}

		public bool isGuardCard(){
			return guardCard;
		}

		public bool isMoveCard(){
			return moveCard;
		}

		public int damageValue(){
			return damage;
		}

		public int movementValue(){
			return movement;
		}

		public int guardValue(){
			return guard;
		}


	}

}