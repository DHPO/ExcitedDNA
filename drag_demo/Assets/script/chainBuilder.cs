using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chainBuilder : MonoBehaviour {
	public GameObject chainPrefab;
	public string chainCache {get; set;}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void buildChain(string sequence) {
		GameObject c = Instantiate(chainPrefab) as GameObject;
		c.GetComponent<chain>().setSequence(sequence);
	}

	public void buildFromCache() {
		buildChain(chainCache);
	}
}
