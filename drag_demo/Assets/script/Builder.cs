using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractBuilder : MonoBehaviour {
	public GameObject prefab;
	public int count = 0;

	void Start () {
		wrappedBuild();
	}

	/* 继承时请实现具体的建造方法 */
	protected abstract void Build();

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "dragable")
			count += 1;
	}

	void OnTriggerExit(Collider other) {
		if (other.gameObject.tag == "dragable") {
			count -= 1;
			if (count <= 0) 
				wrappedBuild();
		}
	}

	public void wrappedBuild() {
		Build();
		//count += 1;
	}
}
