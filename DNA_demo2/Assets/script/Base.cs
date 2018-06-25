using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour {
	public GameObject text;

	public void setType(Nucleotide.Type t) {
		switch (t)
		{
			case Nucleotide.Type.A:
				setColor(new Color(1, 0, 0, 0));
				setText("A");
				break;
			case Nucleotide.Type.T:
				setColor(new Color(1, 1, 0, 0));
				setText("T");
				break;
			case Nucleotide.Type.C:
				setColor(new Color(0, 1, 0, 0));
				setText("C");
				break;
			case Nucleotide.Type.G:
				setColor(new Color(0, 0, 1, 0));
				setText("G");
				break;
		}
	}

	private void setColor(Color c) {
		gameObject.GetComponent<Renderer> ().material.color = c;
	}

	private void setText(string s) {
		text.GetComponent<TextMesh>().text = s;
	}
}
