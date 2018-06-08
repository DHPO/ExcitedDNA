using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nucleotide : MonoBehaviour {
	public enum Type {Unknown, A, T, C, G, U};
	public Type type;
	public GameObject bonds;
	public GameObject text;
	public GameObject base_;
	private int count = 0;

	// Use this for initialization
	void Start () {
		//this.type = Type.A;
		if (count == 0)
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

	public bool isAttached() {
		return count > 0;
	}

	private void setColor(Color c) {
		this.base_.GetComponent<Renderer>().material.color = c;
	}

	private void setText(string t) {
		text.GetComponent<TextMesh>().text = t;
	}

	public void setType(Type t) {
		this.type = t;
		switch (t)
		{
			case Type.A:
				setColor(new Color(255, 0, 0));
				setText("A");
				break;
			case Type.T:
				setColor(new Color(0, 255, 0));
				setText("T");
				break;
			case Type.C:
				setColor(new Color(0, 255, 255));
				setText("C");
				break;
			case Type.G:
				setColor(new Color(0, 0, 255));
				setText("G");
				break;
		}
	}
}
