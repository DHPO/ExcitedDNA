using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour {
	public string type;
	public List<string> acceptType;
	public Color color;
	public Attachable attachable;
	public Slot attachedSlot; /* debug */

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider other) {
		if (attachedSlot)
			return;

		Slot otherSlot = other.gameObject.GetComponent<Slot>();
		if (otherSlot && acceptType.Contains(otherSlot.type)) {
			this.attachable.handleAttach(otherSlot.attachable, this, otherSlot);
			this.attachedSlot = otherSlot;
		}
	}

	void OnTriggerExit(Collider other) {
		/*Slot otherSlot = other.gameObject.GetComponent<Slot>();
		if (otherSlot && this.attachedSlot == otherSlot) {
			this.attachable.handleDetach(otherSlot.attachable, this, otherSlot);
			this.attachedSlot = null;
		}*/
	}
}
