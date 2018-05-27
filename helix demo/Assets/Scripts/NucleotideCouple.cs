using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NucleotideCouple : MonoBehaviour
{

    public NucleotideCouple last;
    public NucleotideCouple next;
    public float force;
    public bool needHelix;

    private float angle = 36;
    private bool hasLast, hasNext;
    private float angularX;


    // Use this for initialization
    void Start()
    {
        hasLast = (last != null);
        hasNext = (next != null);
        angularX = 0;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0, angularX));
        angularX = 0;
        if (needHelix)
        {
            if (hasLast)
            {
                float y = last.transform.localEulerAngles.y;
                y += angle;
                angularX += calcuForce(y) * force * Time.deltaTime;
            }
            if (hasNext)
            {
                float y = next.transform.localEulerAngles.y;
                y -= angle;
                angularX += calcuForce(y) * force * Time.deltaTime;
            }
        }
        else
        {
            if (hasLast && !last.isHelix())
            {
                float y = last.transform.localEulerAngles.y;
                angularX += calcuForce(y) * force * Time.deltaTime;
            }
            else if (hasNext && !next.isHelix())
            {
                float y = next.transform.localEulerAngles.y;
                angularX += calcuForce(y) * force * Time.deltaTime;
            }
        }
    }

    float calcuForce(float y)
    {
        float me = transform.localEulerAngles.y % 360;
        float des = y % 360;
        float force = des - me;
        Debug.Log(force);

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
}
