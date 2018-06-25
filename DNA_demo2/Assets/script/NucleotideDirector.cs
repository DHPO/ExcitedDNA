using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NucleotideDirector : MonoBehaviour {
	private static NucleotideDirector instance;
	public GameObject couplePrefab;

	void Awake() {
		instance = this;
	}

	public static NucleotideDirector getInstance() {
		return instance;
	}

<<<<<<< HEAD
    public NucleotideCouple buildCoupleFromOneSingle(Nucleotide n) {
        if (n.isPaired)
            return null;
        GameObject couple = Instantiate(couplePrefab) as GameObject;
        couple.transform.position = n.transform.position;
        couple.transform.rotation = n.transform.rotation;

        couple.gameObject.GetComponent<NucleotideCouple>().setType(n.type, getPairType(n.type));
        couple.gameObject.GetComponent<NucleotideCouple>().tag = "NucleotideCouple";
=======
	public NucleotideCouple buildCoupleFromOneSingle(Nucleotide n) {
		if (n.isPaired)
			return null;
		GameObject couple = Instantiate(couplePrefab) as GameObject;
		couple.transform.position = n.transform.position;
		couple.transform.rotation = n.transform.rotation;
        
		couple.gameObject.GetComponent<NucleotideCouple>().setType(n.type, getPairType(n.type));
		couple.gameObject.GetComponent<NucleotideCouple>().setColor(n.getColor(), true);
        couple.gameObject.GetComponent<NucleotideCouple>().tag= "NucleotideCouple";
>>>>>>> 6352ab5f916ee99bc6d77a13188ed781fbce3e5f
        
        Destroy(n.gameObject);
		return couple.gameObject.GetComponent<NucleotideCouple>();
	}

    //核苷酸之间氢键角度不知道怎么归位，deHelix先注释掉了
    //public void deHelix(NucleotideCouple n)
    //{
    //    if(n.needHelix == true)
    //    {
    //        n.needHelix = false;
    //    }
    //}
   

	public void buildCoupleChainFromOneSingle(Nucleotide n) {
		if (n.isPaired)
			return;

		Nucleotide head = getHeadOfSingleChain(n);
		Nucleotide next = head.next;
		if (next == null) {
			buildCoupleFromOneSingle(head);
		}
		else {
			NucleotideCouple coupleHead = buildCoupleFromOneSingle(n);
			NucleotideCouple couple = coupleHead;

			while(next.next) {
				next = next.next;
				couple.next = buildCoupleFromOneSingle(next.prev);
				couple.next.prev = couple;
                
                couple = couple.next;
			}
			couple.next = buildCoupleFromOneSingle(next);
           
            couple.next.prev = couple;
			coupleHead.broadcastUpdateTransform();
		}
	}

	public void markCoupleChain( NucleotideCouple chain, Color c) {
		NucleotideCouple n = getHeadOfCoupleChain(chain);

		while (n) {
			n.setColor(c);
			n = n.next;
		}
	}

	public void markSingleChain (Nucleotide chain, Color c) {
		Nucleotide n = getHeadOfSingleChain(chain);

		while (n) {
			n.setColor(c);
			n = n.next;
		}
	}

	public Nucleotide getHeadOfSingleChain(Nucleotide n) {
		Nucleotide head = n;

		while(head.prev)
			head = head.prev;

		return head;
	}

	public NucleotideCouple getHeadOfCoupleChain(NucleotideCouple n) {
		NucleotideCouple head = n;

		while (head.prev)
			head = head.prev;

		return head;
	}

	public Nucleotide.Type getPairType(Nucleotide.Type t) {
		switch (t)
		{
			case Nucleotide.Type.A:
				return Nucleotide.Type.T;
			case Nucleotide.Type.T:
				return Nucleotide.Type.A;
			case Nucleotide.Type.C:
				return Nucleotide.Type.G;
			case Nucleotide.Type.G:
				return Nucleotide.Type.C;
			default:
				return Nucleotide.Type.Empty;
		}
	}

    public Nucleotide.Type Char2Type(char t)
    {
        switch (t)
        {
            case 'A':
                return Nucleotide.Type.A;
            case 'G':
                return Nucleotide.Type.G;
            case 'T':
                return Nucleotide.Type.T;
            case 'C':
                return Nucleotide.Type.C;
            default:
                return Nucleotide.Type.Empty;
        }
    }
}
