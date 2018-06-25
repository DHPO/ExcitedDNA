using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NucleotideDirector : MonoBehaviour {
	private static NucleotideDirector instance;
	public GameObject couplePrefab;
	public GameObject singlePrefab;

	private bool duplicating = false;

	void Awake() {
		instance = this;
	}

	public static NucleotideDirector getInstance() {
		return instance;
	}

	public NucleotideCouple buildCoupleFromOneSingle(Nucleotide n, bool reverse = false) {
		if (n.isPaired)
			return null;
		GameObject couple = Instantiate(couplePrefab) as GameObject;
		couple.transform.position = n.transform.position;
		couple.transform.rotation = n.transform.rotation;
        
        if (!reverse) {
        	couple.gameObject.GetComponent<NucleotideCouple>().setType(n.type, getPairType(n.type));
			Color c = n.getColor();
			couple.gameObject.GetComponent<NucleotideCouple>().setLeftColor(c);
			couple.gameObject.GetComponent<NucleotideCouple>().setRightColor(Color.white);
        }
        else {
        	couple.gameObject.GetComponent<NucleotideCouple>().setType(getPairType(n.type), n.type);
			Color c = n.getColor();
			couple.gameObject.GetComponent<NucleotideCouple>().setRightColor(c);
			couple.gameObject.GetComponent<NucleotideCouple>().setLeftColor(Color.white);
        }
		
        couple.gameObject.GetComponent<NucleotideCouple>().tag= "NucleotideCouple";
        
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
   

	public NucleotideCouple buildCoupleChainFromOneSingle(Nucleotide n, bool reverse=false) {
		if (n.isPaired)
			return null;

		Nucleotide head = getHeadOfSingleChain(n);
		Nucleotide next = head.next;
		if (next == null) {
			return buildCoupleFromOneSingle(head);
		}
		else {
			NucleotideCouple coupleHead = buildCoupleFromOneSingle(head);
			NucleotideCouple couple = coupleHead;

			while(next.next) {
				next = next.next;
				couple.next = buildCoupleFromOneSingle(next.prev, reverse);
				couple.next.prev = couple;
                
                couple = couple.next;
			}
			couple.next = buildCoupleFromOneSingle(next);
           
            couple.next.prev = couple;
            if (!reverse) {
            	coupleHead.broadcastUpdateTransform();
            }
			else {
				couple.next.broadcastUpdateTransform();
			}
			return coupleHead;
		}
	}

	public NucleotideCouple buildCoupleChainFromTwoSingles(Nucleotide c1, Nucleotide c2, Vector3 position = default(Vector3)) {
		if (getLengthOfSingleChain(c1) != getLengthOfSingleChain(c2))
			return null;

		c1 = getHeadOfSingleChain(c1);
		if (c1 == getHeadOfSingleChain(c2))
			return null;

		c2 = getTailOfSingleChain(c2);

		NucleotideCouple n = (Instantiate(couplePrefab) as GameObject).GetComponent<NucleotideCouple>();
		NucleotideCouple head = n;
		n.setLeftColor(c1.getColor()); 
		n.setRightColor(c2.getColor());
		n.setType(c1.type, c2.type);
		while (c1.next) {
			c1 = c1.next;
			c2 = c2.prev;
			n.next = (Instantiate(couplePrefab) as GameObject).GetComponent<NucleotideCouple>();
			n.next.setLeftColor(c1.getColor()); 
			n.next.setRightColor(c2.getColor());
			n.next.setType(c1.type, c2.type);
			n.next.prev = n;
			n = n.next;
		}
		head.transform.position = position;
		head.broadcastUpdateTransform();
		destroySingleChain(c1);
		destroySingleChain(c2);
		return n;
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

	public Nucleotide getTailOfSingleChain(Nucleotide n) {
		Nucleotide tail = n;

		while (tail.next)
			tail = tail.next;

		return tail;
	}

	public int getLengthOfSingleChain(Nucleotide n) {
		Nucleotide head = getHeadOfSingleChain(n);
		int cnt = 1;
		while (n.next) {
			cnt += 1;
			n = n.next;
		}
		return cnt;
	}

	public NucleotideCouple getHeadOfCoupleChain(NucleotideCouple n) {
		NucleotideCouple head = n;

		while (head.prev)
			head = head.prev;

		return head;
	}

	public void duplicateCoupleChain (NucleotideCouple chain) {
		if (duplicating)
			return;
		duplicating = true;
		NucleotideCouple head = getHeadOfCoupleChain(chain);
		StartCoroutine(duplicateCoupleChainRoutine(head));
	}

	IEnumerator duplicateCoupleChainRoutine (NucleotideCouple head) {
		Debug.Log("DeHelix");
		deHelixCoupleChain(head);
		yield return new WaitForSeconds(5);
		Debug.Log("Build Single Chain");
		List<Nucleotide> singles = buildSingleChainsFromCouple(head);
		yield return new WaitForSeconds(5);
		NucleotideCouple c1 = buildCoupleChainFromOneSingle(singles[0]);
		NucleotideCouple c2 = buildCoupleChainFromOneSingle(singles[1]);
		deHelixCoupleChain(c1);
		deHelixCoupleChain(c2);
		yield return new WaitForSeconds(2);
		helixCoupleChain(c1);
		helixCoupleChain(c2);
		duplicating = false;
	}

	public List<Nucleotide> buildSingleChainsFromCouple (NucleotideCouple head) {
		Nucleotide leftHead, left, rightTail, right;
		NucleotideCouple curr = head;

		leftHead = left = (Instantiate(singlePrefab) as GameObject).GetComponent<Nucleotide>();
		rightTail = right = (Instantiate(singlePrefab) as GameObject).GetComponent<Nucleotide>();
		left.setType(curr.getLeftType());
		left.setColor(curr.getLeftColor());
		right.setType(curr.getRightType());
		right.setColor(curr.getRightColor());

		leftHead.transform.rotation = curr.nucleotide1.transform.rotation;
		leftHead.transform.position = curr.nucleotide1.transform.position + leftHead.transform.rotation * Vector3.left * 2.0F;
		rightTail.transform.rotation = curr.nucleotide2.transform.rotation;
		rightTail.transform.position = curr.nucleotide2.transform.position + rightTail.transform.rotation * Vector3.left * 2.0F;
		
		curr = curr.next;
		while (curr) {
			Nucleotide leftPrev, rightPrev;
			leftPrev = left;
			left = (Instantiate(singlePrefab) as GameObject).GetComponent<Nucleotide>();
			leftPrev.next = left;
			left.prev = leftPrev;
			left.setType(curr.getLeftType());
			left.setColor(curr.getLeftColor());

			rightPrev = right;
			right = (Instantiate(singlePrefab) as GameObject).GetComponent<Nucleotide>();
			right.next = rightPrev;
			rightPrev.prev = right;
			right.setType(curr.getRightType());
			right.setColor(curr.getRightColor());

			curr = curr.next;
			/*if (head)
				Destroy(head.prev);*/
		}
		leftHead.broadcastUpdateTransform();
		rightTail.broadcastUpdateTransform();

		destroyCoupleChain(head);

		List<Nucleotide> result = new List<Nucleotide>();
		result.Add(leftHead);
		result.Add(rightTail);
		return result;
	}

	public void deHelixCoupleChain (NucleotideCouple chain) {
		NucleotideCouple n = getHeadOfCoupleChain(chain);
		while (n) {
			n.needHelix = false;
			n = n.next;
		}
	}

	public void destroyCoupleChain (NucleotideCouple chain) {
		NucleotideCouple prev, head = getHeadOfCoupleChain(chain);
		while (head) {
			prev = head;
			head = head.next;
			Destroy(prev.gameObject);
		}
		Debug.Log("Destroyed");
	}

	public void destroySingleChain (Nucleotide chain) {
		Nucleotide prev, head = getHeadOfSingleChain(chain);
		while (head) {
			prev = head;
			head = head.next;
			Destroy(prev.gameObject);
		}
		Debug.Log("Destroyed");
	}

	public void helixCoupleChain (NucleotideCouple chain) {
		NucleotideCouple n = getHeadOfCoupleChain(chain);
		while (n) {
			n.needHelix = true;
			n = n.next;
		}
	}

	public string CoupleChain2String(NucleotideCouple chain) {
		NucleotideCouple n = getHeadOfCoupleChain(chain);
		string result = "2";
		while (n) {
			result += Type2Char(n.getLeftType()) + Type2Char(n.getRightType());
			n = n.next;
		}
		return result;
	}

	public string SingleChain2String(Nucleotide chain) {
		Nucleotide n = getHeadOfSingleChain(chain);
		string result = "1";
		while (n) {
			result += Type2Char(n.type);
			n = n.next;
		}
		return result;
	}

	public Nucleotide String2SingleChain(string s, Vector3 position = default(Vector3)) {
		if (s[0] != '1')
			return null;

		Nucleotide n = (Instantiate(singlePrefab) as GameObject).GetComponent<Nucleotide>();
		Nucleotide head = n;
		n.setType(Char2Type(s[1]));
		for (int i = 2; i < s.Length; i++) {
			n.next = (Instantiate(singlePrefab) as GameObject).GetComponent<Nucleotide>();
			n.next.setType(Char2Type(s[i]));
			n.next.prev = n;
			n = n.next;
		}
		head.transform.position = position;
		head.broadcastUpdateTransform();
		return head;
	}

	public NucleotideCouple String2CoupleChain(string s, Vector3 position = default(Vector3)) {
		if (s[0] != '2')
			return null;

		NucleotideCouple n = (Instantiate(couplePrefab) as GameObject).GetComponent<NucleotideCouple>();
		NucleotideCouple head = n;
		n.setType(Char2Type(s[1]), Char2Type(s[2]));
		for (int i = 3; i < s.Length; i+= 2) {
			n.next = (Instantiate(couplePrefab) as GameObject).GetComponent<NucleotideCouple>();
			n.next.setType(Char2Type(s[i]), Char2Type(s[i+1]));
			n.next.prev = n;
			n = n.next;
		}
		head.transform.position = position;
		head.broadcastUpdateTransform();
		return n;
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

    public char Type2Char(Nucleotide.Type t) {
    	switch (t) {
    		case Nucleotide.Type.A:
    			return 'A';
    		case Nucleotide.Type.T:
    			return 'T';
    		case Nucleotide.Type.C:
    			return 'C';
    		case Nucleotide.Type.G:
    			return 'G';
    		default:
    			return '-';
    	}
    }
}
