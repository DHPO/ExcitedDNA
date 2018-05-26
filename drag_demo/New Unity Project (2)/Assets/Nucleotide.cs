using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nucleotide : MonoBehaviour {
	public enum Type {Unknown, A, T, C, G, U};
	public Type type;
	public GameObject bonds;
	private int count = 0;

	// Use this for initialization
	void Start () {
		this.type = Type.A;
		bonds.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void attach() {
		count += 1;
		bonds.SetActive(true);
	}

	public void detach() {
		count -= 1;
		if (count == 0)
			bonds.SetActive(false);
	}
}
