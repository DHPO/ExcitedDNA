using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnpackTrigger : MonoBehaviour {

	public void OnTriggerEnter(Collider other){
		if (other.tag == "Zip") {
			Zip zip = other.gameObject.GetComponent<Zip> ();
			string sequence = zip.sequence;
			if (sequence [0] == '1') {
				NucleotideDirector.getInstance ().String2SingleChain (sequence, transform.position);
			} else if (sequence [0] == '2') {
				NucleotideDirector.getInstance ().String2CoupleChain (sequence, transform.position);
			}
		}
	}
}
