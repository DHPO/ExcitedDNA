using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class slot : MonoBehaviour {
	private Nucleotide attach;
	public int index;
	public chain parent;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider other) {
		if (!this.attach && other.gameObject.tag == "dragable") {
			this.attach = other.gameObject.GetComponent<Nucleotide>();
			this.attach.attach();
			this.parent.attach(this);
		}
	}

	void OnTriggerExit(Collider other) {
		if (this.attach && other.gameObject.GetComponent<Nucleotide>() == this.attach) {
			this.attach.detach();
			this.attach = null;
			this.parent.detach(this);
		}
	}

	void OnTriggerStay(Collider other) {
		// Debug.Log("enter");
		if (this.attach && other.gameObject.GetComponent<Nucleotide>() == this.attach) {
			other.gameObject.transform.position = this.transform.position;
			other.gameObject.transform.rotation = this.transform.rotation;
		}
	}
}
