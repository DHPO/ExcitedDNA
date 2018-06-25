using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainDirector : MonoBehaviour {
	private static ChainDirector instance = null;

	public static ChainDirector getInstance() {return instance;}

	void Awake() {
		instance = this;
	}

	public void connectSingles(SingleChain c1, SingleChain c2) {
		c1.tail.next = c2.head;
		c2.head.prev = c1.tail;
		c1.tail = c2.tail;
		Destroy(c2.gameObject);
	}

	public void buildCoupleFromOneSingle(SingleChain c) {

	}

	public void makeCoupleFromTwoSingles(SingleChain c1, SingleChain c2) {

	}

	public void splitSingle(SingleChain c1, Nucleotide cutPoint) {
		
	}
}
