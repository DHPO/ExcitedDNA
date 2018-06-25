using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NucleotideFactory : MonoBehaviour {
	public GameObject prefab;
	public Nucleotide.Type type;

	void Start() {
		buildNucleotide();
	}

	/*void OnTriggerEnter(Collider other) {
		Debug.Log("found");
		if (other.tag == "Nucleotide") {
			Destroy(other.gameObject);
		}
	}*/

	void OnTriggerExit(Collider other) {
		//Debug.Log("found exit");
		//Debug.Log(other.tag);
		if (other.tag == "Nucleotide") {
			buildNucleotide();
		}
	}

	private void buildNucleotide() {
		Nucleotide n = (Instantiate(prefab) as GameObject).GetComponent<Nucleotide>();
		n.setType(type);
		n.transform.position = this.transform.position;
		n.transform.rotation = this.transform.rotation;
	}
}
