using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackTrigger : MonoBehaviour {
	public GameObject zipPrefab;

	public void OnTriggerEnter(Collider other){
		if (other.tag == "Nucleotide") {
			string sequence = NucleotideDirector.getInstance ().SingleChain2String (other.gameObject.GetComponent<Nucleotide> ());
			Zip zip = (Instantiate (zipPrefab) as GameObject).GetComponent<Zip> ();
			zip.setSequence(sequence);
			zip.transform.position = transform.position;
			NucleotideDirector.getInstance ().destroySingleChain(other.gameObject.GetComponent<Nucleotide> ());
		} else if (other.tag == "NucleotideCouple") {
			string sequence = NucleotideDirector.getInstance ().CoupleChain2String (other.gameObject.GetComponent<NucleotideCouple> ());
			Zip zip = (Instantiate (zipPrefab) as GameObject).GetComponent<Zip> ();
			zip.setSequence(sequence);
			zip.transform.position = transform.position;
			NucleotideDirector.getInstance ().destroyCoupleChain(other.gameObject.GetComponent<NucleotideCouple> ());
		}
	}
}
