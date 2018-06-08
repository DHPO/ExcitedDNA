using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachDirector : MonoBehaviour {
	private static AttachDirector instance;
	public GameObject detachTrigger;

	private void Awake() {
		instance = this;
	}

	public static AttachDirector getInstance() {
		return instance;
	}

	public bool attach(Transform t1, Transform t2) {
		Debug.Log("attach");
		if ((t1.parent != null && t1.parent == t2.parent) || t1.parent == t2 || t2.parent == t1) {
			return false;
		}
		GameObject trigger = Instantiate(detachTrigger) as GameObject;
		trigger.GetComponent<DetachTrigger>().setTransforms(t1, t2);
		trigger.transform.position = 0.5f * (t1.position + t2.position);
		if (t1.parent == null) {
			if (t2.parent == null) {
				GameObject g = new GameObject();
				g.transform.parent = null;
				t1.parent = g.transform;
				t2.parent = g.transform;
				trigger.transform.parent = g.transform;
			}
			else {
				t1.parent = t2.parent;
				trigger.transform.parent = t2.parent;
			}
		}
		else {
			t2.parent = t1.parent;
			trigger.transform.parent = t1.parent;
		}
		return true;
	}

	public bool detach(Transform t1, Transform t2) {
		if (t1.parent == t2.parent) {
			t2.parent = null;
		}
		else if (t1.parent == t2) {
			t1.parent = null;
		}
		else if (t2.parent == t1) {
			t2.parent = null;
		}
		else {
			return false;
		}
		return true;
	}
}
