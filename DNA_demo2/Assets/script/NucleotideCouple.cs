using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class NucleotideCouple : MonoBehaviour
{

    public NucleotideCouple prev;
    public NucleotideCouple next;
    public float force = 1F;
    public bool needHelix = true;
    public float gap = 1.5F;
    public Nucleotide nucleotide1;
    public Nucleotide nucleotide2;
    public GameObject HydrogenBond;
    private float angle = 36;
    private bool hasPrev, hasNext;
    private float angularX;


    // Use this for initialization
    void Start()
    {
        hasPrev = (prev != null);
        hasNext = (next != null);
        angularX = 0;

        
    }

    // Update is called once per frame
    void Update()
    {

        nucleotide1.enableDrag(false);
        nucleotide2.enableDrag(false);

        /* test */
        nucleotide1.prevBond.SetActive(prev!=null);
        nucleotide2.prevBond.SetActive(prev!=null);
        nucleotide2.nextBond.SetActive(next!=null);
        nucleotide1.nextBond.SetActive(next!=null);
        nucleotide1.isPaired = true;
        nucleotide2.isPaired = true;

        transform.Rotate(new Vector3(0, angularX));
        
        angularX = 0;
        float helixRatio = 0; /* 0: no helix; 1: totally helix */

        if (needHelix)
        {
            float helixRatioPrev = 0;
            float helixRatioNext = 0;
            if (hasPrev)
            {
                float y = prev.transform.localEulerAngles.y;
                y += angle;
                angularX += calcuForce(y) * force * Time.deltaTime;
                helixRatioPrev = 1 - Mathf.Abs(transform.localEulerAngles.y % 360 - y % 360) % angle / angle;
            }
            if (hasNext)
            {
                float y = next.transform.localEulerAngles.y;
                y -= angle;
                angularX += calcuForce(y) * force * Time.deltaTime;
                helixRatioNext = 1 - Mathf.Abs(transform.localEulerAngles.y % 360 - y % 360) % angle / angle;
            }
            helixRatio = Mathf.Max(helixRatioPrev, helixRatioNext);
        }
        else
        {
            float helixRatioPrev = 0;
            float helixRatioNext = 0;
            if (hasPrev && !prev.isHelix())
            {
                float y = prev.transform.localEulerAngles.y;
                angularX += calcuForce(y) * force * Time.deltaTime;
                helixRatioPrev = 1 - Mathf.Abs(transform.localEulerAngles.y % 360 - y % 360) % angle / angle;
            }
            else if (hasNext && !next.isHelix())
            {
                float y = next.transform.localEulerAngles.y;
                angularX += calcuForce(y) * force * Time.deltaTime;
                helixRatioNext = 1 - Mathf.Abs(transform.localEulerAngles.y % 360 - y % 360) % angle / angle;
            }
            helixRatio = Mathf.Max(helixRatioPrev, helixRatioNext);
        }

        setBondAngle(helixRatio);
    }

    void setBondAngle(float helixRatio) {
        float helixRatioPrev = needHelix || (prev && prev.needHelix) ? helixRatio : 0;
        float helixRatioNext = needHelix || (next && next.needHelix) ? helixRatio : 0;
        //nucleotide1.transform.eulerAngles = new Vector3(-45, transform.localEulerAngles.y, 0);
        nucleotide1.prevBond.transform.eulerAngles = new Vector3(-45 * helixRatioPrev, transform.localEulerAngles.y, -9 * helixRatioPrev);
        nucleotide1.nextBond.transform.eulerAngles = new Vector3(-45 * helixRatioNext, transform.localEulerAngles.y,  9 * helixRatioNext);
        //nucleotide2.transform.eulerAngles = new Vector3(45, transform.localEulerAngles.y, 180);
        nucleotide2.prevBond.transform.eulerAngles = new Vector3( 45 * helixRatioPrev, transform.localEulerAngles.y,  9 * helixRatioPrev);
        nucleotide2.nextBond.transform.eulerAngles = new Vector3( 45 * helixRatioNext, transform.localEulerAngles.y, -9 * helixRatioNext);
    }

    void FixedUpdate () {
		try{
			if (gameObject.GetComponent<clickmove>().isDraging() || gameObject.GetComponent<VRTK_InteractableObject>().IsGrabbed())
				broadcastUpdateTransform();
		}
		catch(System.Exception) {}

		/*if (broadcast)
			broadcastUpdateTransform();*/
	}

    float calcuForce(float y)
    {
        float me = transform.localEulerAngles.y % 360;
        float des = y % 360;
        float force = des - me;
        //Debug.Log(force);

        if (Mathf.Abs(force) < 1)
        {
            return 0;
        }
        if (force > 180)
        {
            return force - 360;
        }
        if (force < -180)
        {
            return force + 360;
        }
        else
        {
            return force;
        }
    }

    public bool isHelix()
    {
        return needHelix;

    }

    public void setType(Nucleotide.Type t1, Nucleotide.Type t2) {
        nucleotide1.setType(t1);
        nucleotide2.setType(t2);
        if (t1 == Nucleotide.Type.A || t1 == Nucleotide.Type.T)
        {//A T只有两条氢键，要使最中间一条不显示{
            Debug.Log("AT has two bond");
            HydrogenBond.SetActive(false);
        }
    }

    public void updateTransform(Vector3 position, Quaternion rotation, NucleotideCouple from) {
		this.transform.position = position;
		//this.transform.rotation = rotation;

		if (prev && from != prev) {
			Quaternion prevRotation = rotation;
			Vector3 prevPosition = position + rotation * Vector3.up * gap;
			prev.updateTransform(prevPosition, prevRotation, this);
		}
		if (next && from != next) {
			Quaternion nextRotation = rotation;
			Vector3 nextPosition = position + rotation * Vector3.down * gap;
			next.updateTransform(nextPosition, nextRotation, this);
		}
	}

	public void broadcastUpdateTransform() {
		if (prev) {
			Quaternion prevRotation = this.transform.rotation;
			Vector3 prevPosition = this.transform.position + this.transform.rotation * Vector3.up * gap;
			prev.updateTransform(prevPosition, prevRotation, this);
		}
		if (next) {
			Quaternion nextRotation = this.transform.rotation;
			Vector3 nextPosition = this.transform.position + this.transform.rotation * Vector3.down * gap;
			next.updateTransform(nextPosition, nextRotation, this);
		}
	}
}
