using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chain : MonoBehaviour {
	public GameObject slotPrefab;
	public Hashtable slots;
	private int maxIdx = 0;
	private int minIdx = 0;

	// Use this for initialization
	void Start () {
		slots = new Hashtable();
		addSlot(0);
	}
	
	// Update is called once per frame
	void Update () {
		
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
}
