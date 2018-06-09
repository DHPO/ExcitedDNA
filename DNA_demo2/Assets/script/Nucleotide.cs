using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nucleotide : MonoBehaviour {
	public Nucleotide prev = null;
	public Nucleotide next = null;

	public EndpointTrigger headTrigger;
	public EndpointTrigger tailTrigger;

	public GameObject prevBond;
	public GameObject nextBond;
	public Base base_; /* keyword confliction :( */

	public bool broadcast = false;

	public static float gap = 1.5F;

	public enum Type{Empty, A, T, C, G};
	public Type type;

	public bool isPaired = false;

	// Use this for initialization
	void Start () {
		if (headTrigger) {
			headTrigger.transform.rotation = this.transform.rotation;
			headTrigger.transform.position = this.transform.position + this.transform.rotation * Vector3.up * gap * 0.5F;
		}
		if (tailTrigger) {
			tailTrigger.transform.rotation = this.transform.rotation;
			tailTrigger.transform.position = this.transform.position + this.transform.rotation * Vector3.down * gap * 0.5F;
		}
		prevBond.SetActive(false);
		nextBond.SetActive(false);
	}
	
	public void updateTransform(Vector3 position, Quaternion rotation, Nucleotide from) {
		this.transform.position = position;
		this.transform.rotation = rotation;

		if (prev && from != prev) {
			Quaternion prevRotation = rotation;
			Vector3 prevPosition = position + rotation * Vector3.up * gap;
			prev.updateTransform(prevPosition, prevRotation, this);
		}
		if (next && from != next) {
			Quaternion nextRotation = rotation;
			Vector3 nextPosition = position + rotation * Vector3.down * gap;
			next.updateTransform(nextPosition, nextRotation, this);
		}
	}

	public void broadcastUpdateTransform() {
		if (prev) {
			Quaternion prevRotation = this.transform.rotation;
			Vector3 prevPosition = this.transform.position + this.transform.rotation * Vector3.up * gap;
			prev.updateTransform(prevPosition, prevRotation, this);
		}
		if (next) {
			Quaternion nextRotation = this.transform.rotation;
			Vector3 nextPosition = this.transform.position + this.transform.rotation * Vector3.down * gap;
			next.updateTransform(nextPosition, nextRotation, this);
		}
	}

	// Update is called once per frame
	void FixedUpdate () {
		try{
			if (gameObject.GetComponent<clickmove>().isDraging())
				broadcastUpdateTransform();
		}
		catch(System.Exception) {}

		/*if (broadcast)
			broadcastUpdateTransform();*/
	}

	public bool addPrev(Nucleotide n) {
		if (n.next != null)
			return false;
			
		n.nextBond.SetActive(true);
		n.next = this;
		this.prevBond.SetActive(true);
		this.prev = n;
		broadcastUpdateTransform();
		return true;
	}

	public void cutPrev() {
		if (this.prev) {
			this.prev.nextBond.SetActive(false);
			this.prev.next = null;
		}
		this.prevBond.SetActive(false);
		this.prev = null;
	}

	public void setType(Type t) {
		this.type = t;
		base_.setType(t);
		if (t == Type.Empty) {
			enableDisplay(false);
		}
		else {
			enableDisplay(true);
		}
	}

	private void enableDisplay(bool isEnabled) {
		MeshRenderer render = gameObject.GetComponentInChildren<MeshRenderer>();
		if (isEnabled) {
			foreach ( Transform child in transform ) {
				child.gameObject.SetActive( true );
			}
			render.enabled = true;
		}
		else {
			foreach ( Transform child in transform ) {
				child.gameObject.SetActive( false );
			}
			render.enabled = false;
		}
	}

	public void enableDrag(bool isEnabled) {
		clickmove c = gameObject.GetComponent<clickmove>();
		c.enabled = isEnabled;
	}
}
