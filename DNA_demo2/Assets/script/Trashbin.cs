using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trashbin : MonoBehaviour {
	void OnTriggerEnter(Collider other) {
		if (other.tag == "Nucleotide") {
			NucleotideDirector.getInstance ().destroySingleChain (other.gameObject.GetComponent<Nucleotide> ());
		} else if (other.tag == "NucleotideCouple") {
			NucleotideDirector.getInstance ().destroyCoupleChain (other.gameObject.GetComponent<NucleotideCouple> ());
		} else if (other.tag == "Zip") {
			Destroy (other.gameObject);
		}
		else {
			//Destroy(other.gameObject);
		}
	}
}
