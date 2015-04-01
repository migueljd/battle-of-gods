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
using System;
using TBTK;

namespace Cards
{
	public class CardTransform : MonoBehaviour
	{
		public Card transformCard;

		public Vector3 endScale;
		public static float holdTimeTreshold = 0.2f;
		public static float distanceToActivate = 10;

		public Vector3 finalPosition;

		public static Vector3 baseScale;

		private Vector3 initialPosition;

		private bool cardHeld = false;

		private bool zoomed;

		private float yDistance;

		void Start(){
			this.transformCard = (Card) transform.GetComponent<Card> ();
		}

		void Update(){

			if (cardHeld) {
//				Debug.Log ("Distance from parent: " + Vector3.Distance(this.transform.position, this.transform.parent.position));

				if (!zoomed && Vector3.Distance (this.transform.position, this.transform.parent.position) < distanceToActivate) {
					ZoomCard ();
					zoomed = true;
				} 
				else if(Vector3.Distance(this.transform.position,this.transform.parent.position) >= distanceToActivate && zoomed){
					scaleDown();
					zoomed = false;
				}
				Vector3 mousePos = Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, yDistance));
				this.transform.position = mousePos;


			} else if (zoomed && this.transform.parent != null) {
				DeZoom ();
			}
			else if(zoomed) {
				this.initialPosition = CardsHandManager.getInstance().cardsLimbo;
				DeZoom ();
			}
		}

		private void scaleDown(){
			transformCard.updateTransform (this.transform.position, this.transform.rotation, baseScale);
		}

		private void activateCard(CardsStackManager stack){
			Debug.Log ("Card activated");
			if (this.transformCard.damageCard) 
				stack.addDamageCard(this.transformCard);
			if (this.transformCard.guardCard)
				stack.addGuardCard (this.transformCard);
			if (this.transformCard.moveCard)
				stack.addMoveCard (this.transformCard);

			GameControl.SelectUnit (GameControl.selectedUnit);

			//Do some sort of animation then destroy this card
			this.transform.SetParent (null);
			this.transform.position = CardsHandManager.getInstance ().cardsLimbo;
			transformCard.stopUpdating ();
			CardsHandManager.getInstance ().cardsInHand.removeCard (transformCard);
			CardsHandManager.getInstance ().cardsInDiscard.addCard (transformCard);
		}
//
//		public void selectCard(){
//		
//		}
//
//		public void deselectCard(){
//			
//		}

		public void ZoomCard(){
			transformCard.updateTransform (this.transform.position, this.transform.rotation, endScale);
		}
		public void DeZoom(){
			transformCard.updateTransform (this.initialPosition, this.transform.rotation, baseScale);
			zoomed = false;
		}

		void OnMouseDown(){
			if (CardsHandManager.getInstance () != null && CardsHandManager.getInstance ().mode == CardsHandManager.modes._DeckBuild) {
				GameObject o = (GameObject)Resources.Load ("Prefabs/Cards/" + this.transform.name);
				CardsHandManager.getInstance ().instantiator.addPrefab (o);
			} 
			else {
				yDistance = Camera.main.transform.position.y - this.transform.position.y;
				cardHeld = true;
				this.initialPosition = transform.position;
			}
		}

		void OnMouseUp(){
			if(CardsHandManager.getInstance () != null && CardsHandManager.getInstance ().mode == CardsHandManager.modes._GameOn && cardHeld){
				if(Vector3.Distance(this.transform.position,this.transform.parent.position) >= distanceToActivate && GameControl.selectedUnit != null){
					activateCard(GameControl.selectedUnit.getStack());
				}
			}
			cardHeld = false;

		}

	}
}

