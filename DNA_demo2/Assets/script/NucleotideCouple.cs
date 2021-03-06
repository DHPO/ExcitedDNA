﻿using System.Collections;
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
    public GameObject HydrogenBond1;
    public GameObject HydrogenBond2;
    public GameObject HydrogenBond3;
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
        nucleotide1.prevBond.SetActive(prev!=null && nucleotide1.type != Nucleotide.Type.Empty && prev.nucleotide1.type != Nucleotide.Type.Empty);
        nucleotide2.prevBond.SetActive(prev!=null && nucleotide2.type != Nucleotide.Type.Empty && prev.nucleotide2.type != Nucleotide.Type.Empty);
        nucleotide2.nextBond.SetActive(next!=null && nucleotide2.type != Nucleotide.Type.Empty && next.nucleotide2.type != Nucleotide.Type.Empty);
        nucleotide1.nextBond.SetActive(next!=null && nucleotide1.type != Nucleotide.Type.Empty && next.nucleotide1.type != Nucleotide.Type.Empty);
        nucleotide1.isPaired = true;
        nucleotide2.isPaired = true;

        transform.Rotate(transform.rotation * new Vector3(0, angularX, 0));
        
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
        nucleotide1.prevBond.transform.eulerAngles = new Vector3(-45 * helixRatioPrev + transform.localEulerAngles.x, transform.localEulerAngles.y, -9 * helixRatioPrev + transform.localEulerAngles.z);
        nucleotide1.nextBond.transform.eulerAngles = new Vector3(-45 * helixRatioNext + transform.localEulerAngles.x, transform.localEulerAngles.y,  9 * helixRatioNext + transform.localEulerAngles.z);
        //nucleotide2.transform.eulerAngles = new Vector3(45, transform.localEulerAngles.y, 180);
        nucleotide2.prevBond.transform.eulerAngles = new Vector3( 45 * helixRatioPrev + transform.localEulerAngles.x, transform.localEulerAngles.y,  9 * helixRatioPrev + transform.localEulerAngles.z);
        nucleotide2.nextBond.transform.eulerAngles = new Vector3( 45 * helixRatioNext + transform.localEulerAngles.x, transform.localEulerAngles.y, -9 * helixRatioNext + transform.localEulerAngles.z);
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

    public void setType(Nucleotide.Type t1, Nucleotide.Type t2)
    {
        nucleotide1.setType(t1);
        nucleotide2.setType(t2);
        if (t1 == Nucleotide.Type.A || t1 == Nucleotide.Type.T)
        {//A T只有两条氢键，要使最中间一条不显示{
            //Debug.Log("A/T has two bond");
            HydrogenBond1.SetActive(true);
            HydrogenBond2.SetActive(false);
            HydrogenBond3.SetActive(true);
        }
        else if (t1 == Nucleotide.Type.C || t1 == Nucleotide.Type.G) {
            HydrogenBond1.SetActive(true);
            HydrogenBond2.SetActive(true);
            HydrogenBond3.SetActive(true);
        }
        if (NucleotideDirector.getInstance().getPairType(t1) != t2 && t1 != Nucleotide.Type.Empty && t2 != Nucleotide.Type.Empty) {
            nucleotide1.gameObject.GetComponent<Renderer>().material.color = Color.red;
            nucleotide2.gameObject.GetComponent<Renderer>().material.color = Color.red;
            HydrogenBond1.SetActive(false);
            HydrogenBond2.SetActive(false);
            HydrogenBond3.SetActive(false);
        }
        if (t1 == Nucleotide.Type.Empty || t2 == Nucleotide.Type.Empty) {
            HydrogenBond1.SetActive(false);
            HydrogenBond2.SetActive(false);
            HydrogenBond3.SetActive(false);
        }
    }

    public void hideHydrogenBond()
    {
        HydrogenBond1.SetActive(false);
        HydrogenBond2.SetActive(false);
        HydrogenBond3.SetActive(false);
    }

    public void deHelix()
    {
        NucleotideCouple pre = prev;
        NucleotideCouple nex = next;
        while (pre != null)
        {
            pre.needHelix = false;
            pre = pre.prev;
        }
        while(nex != null)
        {
            nex.needHelix = false;
            nex = nex.next;
        }
    }

    public Nucleotide.Type getLeftType() {
        return nucleotide1.type;
    }

    public Nucleotide.Type getRightType() {
        return nucleotide2.type;
    }

    public void updateTransform(Vector3 position, Quaternion rotation, NucleotideCouple from, bool updateRotation=false) {
		this.transform.position = position;
        if (updateRotation)
		  this.transform.rotation = rotation;

		if (prev && from != prev) {
			Quaternion prevRotation = rotation;
			Vector3 prevPosition = position + rotation * Vector3.up * gap;
			prev.updateTransform(prevPosition, prevRotation, this, updateRotation);
		}
		if (next && from != next) {
			Quaternion nextRotation = rotation;
			Vector3 nextPosition = position + rotation * Vector3.down * gap;
			next.updateTransform(nextPosition, nextRotation, this, updateRotation);
		}
	}

	public void broadcastUpdateTransform(bool updateRotation = false) {
		if (prev) {
			Quaternion prevRotation = this.transform.rotation;
			Vector3 prevPosition = this.transform.position + this.transform.rotation * Vector3.up * gap;
			prev.updateTransform(prevPosition, prevRotation, this, updateRotation);
		}
		if (next) {
			Quaternion nextRotation = this.transform.rotation;
			Vector3 nextPosition = this.transform.position + this.transform.rotation * Vector3.down * gap;
			next.updateTransform(nextPosition, nextRotation, this, updateRotation);
		}
	}

    public void setColor(Color c) {
        this.nucleotide1.setColor(c);
        this.nucleotide2.setColor(c);
    }

    public void setLeftColor(Color c) {
        Debug.Log(c);
        this.nucleotide1.setColor(c);
    }

    public void setRightColor(Color c) {
        this.nucleotide2.setColor(c);
    }

    public Color getLeftColor() {
        return this.nucleotide1.getColor();
    }

    public Color getRightColor() {
        return this.nucleotide2.getColor();
    }
}
