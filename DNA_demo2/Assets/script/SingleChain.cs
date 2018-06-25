using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleChain : MonoBehaviour {
	public Nucleotide head;
	public Nucleotide tail;

	public EndpointTrigger headTrigger;
	public EndpointTrigger tailTrigger;

	public float gap = 1.5F;

	// Use this for initialization
	void Start () {
		/* set triggers' transform */
		headTrigger.transform.rotation = this.transform.rotation;
		headTrigger.transform.position = this.transform.position + this.transform.rotation * Vector3.up * gap * 0.5F;
		tailTrigger.transform.rotation = this.transform.rotation;
		tailTrigger.transform.position = this.transform.position + this.transform.rotation * Vector3.down * gap * 0.5F;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void addNucleotideToHead(Nucleotide n) {
		/* set position */
		n.transform.parent = this.transform;
		n.transform.position = headTrigger.transform.position;
		n.transform.rotation = headTrigger.transform.rotation;
		headTrigger.transform.position += headTrigger.transform.rotation * Vector3.up * gap;

		/* set data structure */
		n.next = head;
		if (head) 
			head.prev = n;
		else 
			tail = n;
		head = n;
		/* TODO: set collider */
	}

	public void addNucleotideToTail(Nucleotide n) {
		/* set position */
		n.transform.parent = this.transform;
		n.transform.position = tailTrigger.transform.position;
		n.transform.rotation = tailTrigger.transform.rotation;
		tailTrigger.transform.position += tailTrigger.transform.rotation * Vector3.down * gap;

		/* set data structure */
		n.prev = tail;
		if (tail) 
			tail.next = n;
		else
			head = n;
		tail = n;
		/* TODO: set collider */
	}


}
