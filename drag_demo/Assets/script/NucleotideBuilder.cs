using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NucleotideBuilder : MonoBehaviour {
	public GameObject nuleotide;
	public Nucleotide.Type type;

	// Use this for initialization
	void Start () {
		build();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerExit(Collider other) {
		if (other.gameObject.tag == "dragable") {
			build();
		}
	}

	private void build() {
		GameObject n = Instantiate(nuleotide, this.transform.position, this.transform.rotation) as GameObject;
		n.GetComponent<Nucleotide>().setType(this.type);
	}
}
