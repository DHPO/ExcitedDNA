using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zip : MonoBehaviour {

	public string sequence;

	public void setSequence (string sequence) {
		this.sequence = sequence;
		this.setText (sequence);
	}

	private void setText(string s) {
		this.transform.GetChild(0).GetComponent<TextMesh>().text = s;
	}
}
