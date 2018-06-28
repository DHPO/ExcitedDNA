using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class deHelix : MonoBehaviour {

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "NucleotideCouple")
        {
            //other.gameObject.GetComponent<NucleotideCouple>().deHelix();
            NucleotideDirector.getInstance().deHelixCoupleChain(other.gameObject.GetComponent<NucleotideCouple>());
            Debug.Log("dehelix");
        }
    }
}
