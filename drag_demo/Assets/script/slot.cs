using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class slot : MonoBehaviour {
	private Nucleotide attach;
	public int index;
	public chain parent;
	public GameObject nucleotidePrefab;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (this.parent.gameObject.GetComponent<clickmove>().isDraging() && this.attach) {
			this.attach.gameObject.transform.position = this.transform.position;
			this.attach.gameObject.transform.rotation = this.transform.rotation;
		}
	}

	void OnTriggerEnter(Collider other) {
		if (!this.attach && other.gameObject.tag == "dragable") {
			this.attach = other.gameObject.GetComponent<Nucleotide>();
			if (this.attach.isAttached()) {
				this.attach = null;
				return;
			}
			this.attach.attach();
			this.parent.attach(this);
		}
		if (other.gameObject.tag == "slot") {
			slot otherSlot = other.gameObject.GetComponent<slot>();
			if (parent.isLast(index) && otherSlot.parent.isFirst(otherSlot.index)) {
				parent.chainBuilder.GetComponent<chainBuilder>().joinChains(parent, otherSlot.parent);
			}
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

	public Nucleotide.Type getType() {
		if (this.attach)
			return this.attach.type;
		else
			return Nucleotide.Type.Unknown;
	}

	public void setType(Nucleotide.Type t) {
		if (!this.attach)
			this.attach = (Instantiate(nucleotidePrefab, this.transform.position, this.transform.rotation) as GameObject).GetComponent<Nucleotide>();
		this.attach.attach();
		this.attach.setType(t);
	}

	public void destroy() {
		if (this.attach) {
			Destroy(this.attach.gameObject);
		}
		Destroy(this.gameObject);
	}
}
