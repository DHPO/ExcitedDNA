using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorMarker : MonoBehaviour {
	public Color color;

	void OnTriggerEnter(Collider other) {
        if (other.tag == "NucleotideCouple") {
        	Debug.Log("mark");
            NucleotideCouple chain = other.gameObject.GetComponent<NucleotideCouple>();
            NucleotideDirector.getInstance().markCoupleChain(chain, this.color);
        }
        else if (other.tag == "Nucleotide") {
        	Debug.Log("mark");
            Nucleotide chain = other.gameObject.GetComponent<Nucleotide>();
            NucleotideDirector.getInstance().markSingleChain(chain, this.color);
        }
    }
}
