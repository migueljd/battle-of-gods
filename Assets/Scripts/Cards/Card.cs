using UnityEngine;
using System.Collections;

using Cards;

namespace Cards{
	public class Card : MonoBehaviour{

		public Sprite cardImage;

		public bool damageCard;
		public bool guardCard;
		public bool moveCard;

		public int damage;
		public int movement;
		public int guard;

		public int turnCount;
		public int currentCount;

		public static float acceptableDistanceVectors = 0.2f;
		public static float acceptableAngleDifference = 10;
		public static float translationSpeed = 20f;

		public static float timeToScale = 0.1f;

	
		private float updateTime;

		private bool shouldUpdate;
		private Vector3 initialPosition;
		private Vector3 finalPosition;
		private Quaternion initialRotation;
		private Quaternion finalRotation;
		private Vector3 initialScale;
		private Vector3 finalScale;

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

		public void updateTransform(Vector3 position, Quaternion rotationAroundY, Vector3 scale){

			this.initialPosition = this.transform.position;
			this.finalPosition = position;
			this.initialRotation = this.transform.rotation;
			this.finalRotation = rotationAroundY;
			this.initialScale = this.transform.localScale;
			this.finalScale = scale;

			updateTime = Time.time;

			shouldUpdate = true;
		}

		public void stopUpdating(){
			shouldUpdate = false;
		}

		public void updatePosition(Vector3 position){
			float interpolate = (Time.time - updateTime)/timeToScale > 1? 1 :  (Time.time - updateTime)/timeToScale;
			transform.position = Vector3.Lerp (this.initialPosition, position, interpolate);
		}


		public void updateRotation(Quaternion rotation){

			float interpolate = (Time.time - updateTime)/timeToScale > 1? 1 :  (Time.time - updateTime)/timeToScale;
			transform.rotation = Quaternion.Lerp (this.initialRotation, rotation, interpolate);

		}

		public void updateScale(Vector3 scale){
			float interpolate = (Time.time - updateTime)/timeToScale > 1? 1 :  (Time.time - updateTime)/timeToScale;
			transform.localScale = Vector3.Lerp (this.initialScale, scale, interpolate);
		}

		void Update(){
			if (shouldUpdate) {
				updatePosition (finalPosition);
				updateRotation(finalRotation);
				updateScale(finalScale);

				bool rotationDone =Quaternion.Angle (this.transform.rotation, finalRotation) <= 0.1f;
				bool positionDone = Vector3.Distance(this.transform.position, finalPosition) <= 0.001f;
				bool scaleDone = Vector3.Distance(this.transform.localScale,finalScale) <= 0.001f;

				if(rotationDone && positionDone && scaleDone){
					shouldUpdate = false;
				}
			}
		}

	}

}