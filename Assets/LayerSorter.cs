using UnityEngine;
using System.Collections;

public class LayerSorter : MonoBehaviour {

	public int sortingLayerOrder = 1;

	// Use this for initialization
	void Start () {
		for(int a = 0; a < transform.childCount; a++)
		{
			transform.GetChild(a).GetComponent<MeshRenderer>().sortingOrder = sortingLayerOrder;

		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
