using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuplicateChainTrigger : MonoBehaviour {
	void OnTriggerEnter(Collider other) {
		if (other.tag == "NucleotideCouple") {
			//Debug.Log("duplicate");
			NucleotideCouple chain = other.gameObject.GetComponent<NucleotideCouple>();
			NucleotideDirector.getInstance().duplicateCoupleChain(chain);
		}
	}
}
