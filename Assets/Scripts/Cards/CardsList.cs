// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 4.0.30319.1
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cards
{


	//this list will be used to store all the used cards and their status
	public class CardsList
	{

		public List<Card> list;

		public int damageCount;
		public int guardCount;
		public int movementCount;


		public CardsList ()
		{
			list = new List<Card> ();
		}


		public void shuffleDeck(){
			System.Random rng = new System.Random ();
			int n = list.Count;
			while (n > 1) {
				n--;
				int k = rng.Next(n + 1);
				Card c = list[k];
				list[k] = list[n];
				list[n] = c;
			}
		}

		public Card removeCardAt(int a){
			Card ret = list [a];
			list.RemoveAt (a);

			return ret;
		}

		public bool removeCard(Card card){
			return list.Remove (card);
		
		}

		//each time a card is added, it should be added in the right list, so we don't really need to check their type
		//however, we need to check if the cards turn counts is lower or higher than the ones in the existing list
		//if it is lower, it should appear before them
		public void addCard(Card card){
			list.Add (card);
			list.Sort((x,y) => {return x.currentCount - y.currentCount;});
			damageCount += card.isDamageCard () ? card.damage : 0;
			guardCount += card.isGuardCard () ? card.guard : 0;
			movementCount += card.isMoveCard() ? card.movement : 0;

		}

		public void updateAttributesCount(){

			damageCount = 0;
			guardCount = 0;
			movementCount = 0;
			foreach (Card c in list) {
				damageCount += c.isDamageCard() ? c.damage: 0;
				guardCount += c.isGuardCard() ? c.guard : 0;
				movementCount += c.isMoveCard() ? c.movement : 0;
			}
		}

		public void updateList(){

			int a = 0;

			foreach (Card c in list) {
				c.currentCount--;
				if(c.currentCount <=0)a++;
			}

			if (a > 0)
				list.RemoveRange (0, a);
		}

		public int getAttribute(bool damage = false, bool guard = false, bool move = false){
			int attribute = 0;

			if (list.Count > 0) {
				Card c = list[0];
				attribute += c.isDamageCard() && damage ? damageCount : (c.isGuardCard() && guard? guardCount : movementCount);
			}

			return attribute;
		}

		public Card popFirstCard(){

			return removeCardAt (0);
		}

		public void decreaseAttribute(int decrease, bool damage = false, bool guard = false, bool move = false){
			if (damage)
				damageCount = Mathf.Max(0, damageCount -decrease);
			if (guard)
				guardCount = Mathf.Max(0, guardCount -decrease);
			if (move)
				movementCount = Mathf.Max(0, movementCount -decrease);

		}


		public int getCount(){
			return list.Count;
		}
	}
}

