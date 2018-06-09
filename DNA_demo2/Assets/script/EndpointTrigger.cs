using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndpointTrigger : MonoBehaviour {
	private Nucleotide parent;
	public bool isHead = true;
	private bool attached = false;

	// Use this for initialization
	void Start () {
		parent = transform.parent.gameObject.GetComponent<Nucleotide>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void OnTriggerEnter(Collider other) {
		if (other.tag == "Nucleotide") {
			Nucleotide n = other.gameObject.GetComponent<Nucleotide>();
			if (isHead && !attached && !n.isPaired && !parent.isPaired) {
				if (parent.addPrev(n))
					attached = true;
			}
		}
		else if (other.tag == "cut") {
			if (attached) {
				attached = false;
				parent.cutPrev();
			}
		}
	}
}
