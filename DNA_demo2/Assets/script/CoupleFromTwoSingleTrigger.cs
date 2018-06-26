using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoupleFromTwoSingleTrigger : MonoBehaviour {
	public Nucleotide waiting;

	void Update() {
		gameObject.GetComponent<Renderer>().material.color = waiting==null?Color.white:Color.yellow;
	}

	void OnTriggerEnter(Collider other) {
		if (other.tag == "Nucleotide") {
			Nucleotide n = other.gameObject.GetComponent<Nucleotide>();
			if (n.isPaired)
				return;
			if (waiting) {
				NucleotideDirector.getInstance().buildCoupleChainFromTwoSingles(waiting, n, transform.position);
				waiting = null;
			}
			else {
				waiting = n;
			}
		}
	}

	void OnTriggerExit(Collider other) {
		if (other.tag == "Nucleotide") {
			waiting = null;
		}
	}
}
