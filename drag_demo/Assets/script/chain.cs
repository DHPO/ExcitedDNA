using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chain : MonoBehaviour {
	public GameObject slotPrefab;
	public GameObject chainBuilder;
	public Hashtable slots;
	private int maxIdx = 0;
	private int minIdx = 0;

	// Use this for initialization
	void Start () {
		if (slots == null)
			slots = new Hashtable();
		addSlot(0);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void setSequence(string sequence) {
		foreach (char c in sequence) {
			Nucleotide.Type t = Nucleotide.Type.Unknown;
			switch (c) {
				case 'A':
					t = Nucleotide.Type.A;
					break;
				case 'T':
					t = Nucleotide.Type.T;
					break;
				case 'C':
					t = Nucleotide.Type.C;
					break;
				case 'G':
					t = Nucleotide.Type.G;
					break;
				case '-':
					t = Nucleotide.Type.Unknown;
					break;
			}
			appendFilledSlot(t);
			// Debug.Log(c);
		}
	}

	private bool addSlot(int index) {
		if (slots.ContainsKey(index))
			return false;
		GameObject slot = Instantiate(slotPrefab) as GameObject;
		slot.transform.parent = this.transform;
		slot.transform.position = this.transform.position + this.transform.rotation * Vector3.down * 1.5F * index;
		slot.transform.rotation = this.transform.rotation;
		slot.GetComponent<slot>().index = index;
		slot.GetComponent<slot>().parent = this;
		slots.Add(index, slot.GetComponent<slot>());
		return true;
	}

	private bool appendFilledSlot(Nucleotide.Type t) {
		if (slots == null)
			slots = new Hashtable();
		if (!slots.Contains(0))
			addSlot(0);
		if (t != Nucleotide.Type.Unknown)
			(slots[maxIdx] as slot).setType(t);
		attach(slots[maxIdx] as slot);
		return true;
	}

	private bool removeSlot(int index) {
		if (!slots.ContainsKey(index))
			return false;
		Destroy((slots[index] as slot).gameObject);
		slots.Remove(index);
		return true;
	}

	public bool attach(slot s) {
		if (!slots.Contains(s.index))
			return false;
		int index = s.index;
		addSlot(index + 1);
		this.maxIdx = index + 1 > this.maxIdx ? index + 1 : this.maxIdx;
		addSlot(index - 1);
		this.minIdx = index - 1 < this.minIdx ? index - 1 : this.minIdx;
		return true;
	}

	public bool detach(slot s) {
		if (slots == null)
			slots = new Hashtable();
		if (!slots.Contains(s.index))
			return false;
		int index = s.index;
		if (index + 1 == this.maxIdx) {
			removeSlot(this.maxIdx);
			this.maxIdx = index;
		}
		if (index - 1 == this.minIdx) {
			removeSlot(this.minIdx);
			this.minIdx = index;
		}
		return true;
	}

	public string getSequence() {
		string result = "";
		for (int i = minIdx + 1; i < maxIdx; i++) {
				switch ((slots[i] as slot).getType())
				{
					case Nucleotide.Type.A:
						result += "A";
						break;
					case Nucleotide.Type.T:
						result += "T";
						break;
					case Nucleotide.Type.C:
						result += "C";
						break;
					case Nucleotide.Type.G:
						result += "G";
						break;
					case Nucleotide.Type.Unknown:
						result += "-";
						break;
					default:
						result += "?";
						break;
				}
		}
		Debug.Log(result);
		return result;
	}

	public bool isFirst(int idx) {
		return idx == minIdx;
	}

	public bool isLast(int idx) {
		return idx == maxIdx;
	}

	public void destroy() {
		foreach(slot s in slots.Values) {
			s.destroy();
		}
		Destroy(this.gameObject);
	}
}
