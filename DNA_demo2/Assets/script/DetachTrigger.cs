using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetachTrigger : MonoBehaviour {
	public Transform t1;
	public Transform t2;

	private void OnTriggerEnter(Collider other) {
		if (other.tag == "detach") {
			Debug.Log("detach");
			if (AttachDirector.getInstance().detach(t1, t2)) {
				Destroy(this.gameObject);
			}
		}
	}

	public void setTransforms(Transform t1, Transform t2) {
		this.t1 = t1;
		this.t2 = t2;
	}
}
