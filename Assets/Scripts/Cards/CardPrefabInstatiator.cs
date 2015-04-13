using UnityEngine;
using System.Collections.Generic;
using Cards;

public class CardPrefabInstatiator{


	public Stack<GameObject> cardsToInstantiate;
	public Dictionary<GameObject, int> cardTypeCount;  
	public static int maxPerType = 5;


	public CardPrefabInstatiator(Stack<GameObject> cardsPrefabs){
		cardsToInstantiate = cardsPrefabs;
	}

	public CardPrefabInstatiator(){
		cardsToInstantiate = new Stack<GameObject>();
		cardTypeCount = new Dictionary<GameObject, int> ();
	}


	public void addPrefab(GameObject prefab){
		int count = 0;
		if (cardTypeCount.TryGetValue (prefab, out count) && count <maxPerType) {
			count ++;
			cardTypeCount[prefab] =  count;
			cardsToInstantiate.Push(prefab);
		} else if(!cardTypeCount.ContainsKey(prefab)){
			count = 1;
			cardTypeCount.Add (prefab, count);
			cardsToInstantiate.Push(prefab);
		}else{
			Debug.Log("There are too many of this card already");
		}
	}

	public GameObject popPrefab(){
		return cardsToInstantiate.Pop ();
	}
}
