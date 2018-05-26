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

	public void buildChain(string sequence, Transform t=null) {
		if (t == null)
			t = this.transform;
		GameObject c = Instantiate(chainPrefab, t.position, t.rotation) as GameObject;
		c.GetComponent<chain>().setSequence(sequence);
	}

	public void buildFromCache() {
		buildChain(chainCache);
	}

	public void joinChains(chain c1, chain c2) {
		buildChain(c1.getSequence() + c2.getSequence(), c1.gameObject.transform);
		c1.destroy();
		c2.destroy();
	}
}
