using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranscriptionTrigger : MonoBehaviour {
    public string StartPoint;
    public string EndPoint;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "NucleotideCouple")
        {
            Debug.Log("Transcript");
            NucleotideCouple chain = other.gameObject.GetComponent<NucleotideCouple>();
            NucleotideDirector.getInstance().transcriptFromCoupleChain(chain, StartPoint, EndPoint);
        }
    }
}
