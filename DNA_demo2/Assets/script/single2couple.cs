using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class single2couple : MonoBehaviour {
	void OnTriggerEnter(Collider other) {
		if (other.tag == "Nucleotide") {
			// NucleotideDirector.getInstance().buildCoupleChainFromOneSingle(other.gameObject.GetComponent<Nucleotide>());
			NucleotideDirector.getInstance().buildCoupleChainFromOneSingleAnimation(other.gameObject.GetComponent<Nucleotide>(), 0.8F);
		}
        
	}
}
