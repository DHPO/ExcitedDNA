using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct AttachInfo{
	public Vector3 offsetPosition;
	public Quaternion offsetRotation;
};

public class Attachable : MonoBehaviour {
	public enum Status {Free, Fixed};
	public List<Slot> slots;

	public Status status;
	public int priority;
	public Attachable lastUpdateSource;
	public int lastUpdatePriority;
	public Hashtable attachedTable;

	// Use this for initialization
	void Start () {
		attachedTable = new Hashtable();
	}
	
	// Update is called once per frame
	void Update () {
		try
		{
			bool isDraging = this.gameObject.GetComponent<clickmove>().isDraging();
			if (isDraging)
				this.status = Status.Fixed;
			else if (this.status == Status.Fixed) {
				this.status = Status.Free;
				foreach (Attachable item in attachedTable.Keys)
				{
					lastUpdatePriority = 0;
					item.handleUpdatePhysics(this, 0, this.transform);
				}
			}
		}
		catch (System.Exception)
		{
			Debug.Log("not dragable");
		}

		if (this.status == Status.Fixed) {
			foreach (Attachable item in attachedTable.Keys)
			{
				lastUpdatePriority = 2048;
				item.handleUpdatePhysics(this, 2048, this.transform);
			}
		}
	}

	public bool handleAttach(Attachable other, Slot thisSlot, Slot otherSlot) {
		if (attachedTable.Contains(other))
			return false; 

		AttachInfo info = new AttachInfo();
		info.offsetRotation = this.transform.rotation * Quaternion.Inverse(thisSlot.transform.rotation) * Quaternion.Inverse(otherSlot.transform.rotation) * /*Quaternion.Inverse*/(other.transform.rotation);
		info.offsetPosition = 2*(this.transform.position - thisSlot.transform.position); /*- Quaternion.Inverse(Quaternion.Inverse(thisSlot.transform.rotation) * /*Quaternion.Inverse*//*(otherSlot.transform.rotation)) * (otherSlot.transform.position - other.transform.position);*/

		this.attachedTable.Add(other, info);
		return true;
	}

	public bool handleDetach(Attachable other, Slot thisSlot, Slot otherSlot) {
		this.attachedTable.Remove(other);
		if (lastUpdateSource == other) {
			lastUpdateSource = null;
			lastUpdatePriority = 0;
		}
		return true;
	}

	public bool handleUpdatePhysics(Attachable source, int priority, Transform t) {
		if (!attachedTable.Contains(source))
			return false;

		if (priority <= lastUpdatePriority) {
			if (source == lastUpdateSource) 
				lastUpdatePriority = priority;
			else
				return false;
		}
		lastUpdateSource = source;
		lastUpdatePriority = priority;


		AttachInfo info = (AttachInfo)(attachedTable[source]);
		this.transform.position = t.position + info.offsetPosition;
		this.transform.rotation = t.rotation /* info.offsetRotation*/;
		
		foreach (Attachable item in attachedTable.Keys)
		{
			item.handleUpdatePhysics(this, priority - 1, this.transform);
		}

		return true;
	}
}
