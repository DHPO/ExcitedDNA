using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndpointTrigger : MonoBehaviour {
	private Nucleotide parent;
	public bool isHead = true;
	private bool attached = false;


	// Use this for initialization
	void Start () {
		parent = transform.parent.gameObject.GetComponent<Nucleotide>();
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}

	private void OnTriggerEnter(Collider other) {
		if (other.tag == "Nucleotide") {
			Nucleotide n = other.gameObject.GetComponent<Nucleotide>();
			if (isHead && !attached && !n.isPaired && !parent.isPaired) {
				if (parent.addPrev(n))
					attached = true;
			}
		}
		//else if (other.tag == "cut") {//判断parent上下的核苷酸类型再cutPrev
		//	if (attached) {
  //              Cutter cutter = GameObject.Find("Knife").GetComponent<Cutter>();
  //              NucleotideDirector n = NucleotideDirector.getInstance();//GameObject.Find("NucleotideDirector").GetComponent<NucleotideDirector>();
  //              Nucleotide tmp = parent;
  //              string before = cutter.seqBeforeCutPoint;
  //              string after = cutter.seqAfterCutPoint;
  //              //Debug.Log(before);
  //              //Debug.Log(after);
  //              for(int i = before.Length-1; i >= 0; i--)
  //              {
  //                  if (tmp.prev)
  //                  {
  //                      if (n.Char2Type(before[i]) == tmp.prev.type) tmp = tmp.prev;
  //                      else return;//没有对应，不能割
  //                  }
  //                  else
  //                      return;//没有足够核苷酸对应before序列，肯定不能割
  //              }
  //              tmp = parent;
  //              for(int i  =0; i < after.Length; i++)
  //              {
  //                  if (tmp)
  //                  {
  //                      if (n.Char2Type(after[i]) == tmp.type) tmp = tmp.next;
  //                      else return;
  //                  }
  //                  else
  //                      return;
  //              }

  //              //cutter.exchangeSeq();切一次成功后要对调切割点前后seq，以便切另一边
		//		attached = false;
		//		parent.cutPrev();
		//	}
		//}
	}

    
}
