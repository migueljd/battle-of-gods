using UnityEngine;
using System.Collections;

public class LayerSorter : MonoBehaviour {

	public int sortingLayerOrder = 0;

	// Use this for initialization
	void Start () {
		for(int a = 0; a < transform.childCount; a++)
		{
			if(transform.GetChild(a).GetComponent<MeshRenderer>() != null)transform.GetChild(a).GetComponent<MeshRenderer>().sortingOrder = sortingLayerOrder;
			if(transform.GetChild(a).childCount > 0) transform.GetChild(a).GetChild(0).GetComponent<MeshRenderer>().sortingOrder = sortingLayerOrder;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
