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

		public static float acceptableDistanceVectors = 0.2f;
		public static float acceptableAngleDifference = 10;
		public static float translationSpeed = 20f;

		private bool shouldUpdate;
		private Vector3 initialPosition;
		private Vector3 finalPosition;
		private Quaternion initialRotation;
		private Quaternion finalRotation;

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

		public void updateTransform(Vector3 position, Quaternion rotationAroundY){
			this.initialPosition = this.transform.position;
			this.finalPosition = position;
			this.initialRotation = this.transform.rotation;
			this.finalRotation = rotationAroundY;

			shouldUpdate = true;
		}

		public void updatePosition(Vector3 position){
			if (position.y < this.transform.position.y)
				this.transform.position = position;
			else if(Vector3.Distance (this.transform.position, position) > acceptableDistanceVectors)
				transform.Translate ((position - this.initialPosition).normalized * Time.deltaTime*translationSpeed, Space.World);
		}


		public void updateRotation(Quaternion rotation){
			float angleDifference = Quaternion.Angle (rotation, this.transform.rotation);

			if (angleDifference > acceptableAngleDifference)
				this.transform.rotation = Quaternion.Slerp(this.transform.rotation, rotation, Time.deltaTime);
		}

		void Update(){
			if (shouldUpdate) {
				updatePosition (finalPosition);
				updateRotation(finalRotation);

				float angleDifference = Quaternion.Angle (this.transform.rotation, finalRotation);
				float vectorDistance = Vector3.Distance(this.transform.position, finalPosition);

				Debug.Log ("Distance: " + vectorDistance);
				Debug.Log ("Angle difference: " + angleDifference);

				if(vectorDistance <= acceptableDistanceVectors && angleDifference <= acceptableAngleDifference) shouldUpdate = false;

			}
		}

	}

}