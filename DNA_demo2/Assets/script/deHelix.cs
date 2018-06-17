using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class deHelix : MonoBehaviour {

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "NucleotideCouple")
        {
           // NucleotideDirector.getInstance().deHelix(other.gameObject.GetComponent<NucleotideCouple>());
        }
    }
}
