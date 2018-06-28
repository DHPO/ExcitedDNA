using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class NucleotideDirector : MonoBehaviour {
	private static NucleotideDirector instance;
	public GameObject couplePrefab;
	public GameObject singlePrefab;
    public GameObject rnaPrefab;

	private bool animating = false;

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
    
    public void mergeTwo(NucleotideCouple upper,NucleotideCouple downer)
    {
        string sequence = "1",seqUp="",seqDown="";
        NucleotideCouple upperHead, downerHead;
        if(upper.nucleotide1.isActiveAndEnabled && upper.nucleotide2.isActiveAndEnabled)
        {
            upperHead = getHeadOfCoupleChain(upper);
            downerHead = getHeadOfCoupleChain(downer);
        }
        else
        {
            upperHead = getHeadOfCoupleChain(downer);
            downerHead = getHeadOfCoupleChain(upper);
        }
        while (upperHead)
        {
            if(upperHead.nucleotide1.isActiveAndEnabled && upperHead.nucleotide2.isActiveAndEnabled)//这部分是都显示的
            {
                sequence += Type2Char(upperHead.nucleotide1.type);
            }
            else if(upperHead.nucleotide1.isActiveAndEnabled || upperHead.nucleotide2.isActiveAndEnabled)//这部分是粘性末端，记录下来做比较
            {
                sequence += Type2Char(upperHead.nucleotide1.type);//这部分只能加进sequence里一次，下个while循环里不能加
                if (upperHead.nucleotide1.isActiveAndEnabled) seqUp += Type2Char(upperHead.nucleotide1.type);
                else seqUp += Type2Char(upperHead.nucleotide2.type);
            }
            upperHead = upperHead.next;
        }

        while (downerHead)
        {
            if(downerHead.nucleotide1.isActiveAndEnabled && downerHead.nucleotide2.isActiveAndEnabled)
            {
                sequence += Type2Char(downerHead.nucleotide1.type); //添加另一半
            }
            else if(downerHead.nucleotide1.isActiveAndEnabled || downerHead.nucleotide2.isActiveAndEnabled)
            {
                //Debug.Log(downerHead.nucleotide1.type);
                if (downerHead.nucleotide1.isActiveAndEnabled) seqDown += Type2Char(downerHead.nucleotide2.type);
                else seqDown += Type2Char(downerHead.nucleotide1.type);//注意这里跟上个while循环是反着来的，为了之后直接对比seqUp seqDown，一样的话就算match了
            }
            downerHead = downerHead.next;
        }

        Debug.Log(seqUp);
        Debug.Log(seqDown);
        //Debug.Log(sequence);
        if(seqDown == seqUp)
        {
            Debug.Log("match");
            Nucleotide headSingle = String2SingleChain(sequence);
            destroyCoupleChain(upper);
            destroyCoupleChain(downer);
            NucleotideCouple headCouple = buildCoupleChainFromOneSingle(headSingle);
        }
    }

    public NucleotideCouple buildCoupleFromOneSingleWithoutDestroy(Nucleotide n)
    {
        Debug.Log("start build");
        GameObject couple = Instantiate(couplePrefab) as GameObject;
        couple.transform.position = n.transform.position;
        couple.transform.rotation = n.transform.rotation;
        Debug.Log("instantiate complete");
        couple.gameObject.GetComponent<NucleotideCouple>().setType(n.type, getPairType(n.type));
        couple.gameObject.GetComponent<NucleotideCouple>().tag = "NucleotideCouple";
        couple.gameObject.GetComponent<NucleotideCouple>().needHelix = false;
        couple.gameObject.GetComponent<NucleotideCouple>().setColor(Color.white);
        Debug.Log("config complete");
        return couple.gameObject.GetComponent<NucleotideCouple>();
    }

    public NucleotideCouple buildCoupleChainFromOneDirection(NucleotideCouple nc, int direction)//1 is up, 0 is down
    {
        NucleotideCouple coupleHead = buildCoupleFromOneSingleWithoutDestroy(nc.nucleotide1);
        NucleotideCouple couple = coupleHead;
        if (direction == 1)
        {
            NucleotideCouple pre = nc.prev;

            while (pre)
            {
                
                couple.prev = buildCoupleFromOneSingleWithoutDestroy(pre.nucleotide1);
                couple.prev.next = couple;
                pre = pre.prev;
                couple = couple.prev;
            }

        }
        else
        {
            NucleotideCouple nex = nc.next;

            while (nex)
            {
                couple.next = buildCoupleFromOneSingleWithoutDestroy(nex.nucleotide1);
                couple.next.prev = couple;
                nex = nex.next;
                couple = couple.next;
            }


        }
        coupleHead.broadcastUpdateTransform();
        return coupleHead;
    }
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

	public NucleotideCouple buildCoupleChainFromOneSingleAnimation(Nucleotide chain, float timeGap = 0.8F) {
		NucleotideCouple head = buildHalfCoupleChainFromOneSingle(chain);
		if (head == null)
			return null;
		animating = true;
		StartCoroutine(fillHalfCoupleChainRoutine(head, timeGap));
		return head;
	}

	public IEnumerator fillHalfCoupleChainRoutine(NucleotideCouple head, float timeGap = 0.1F) {
		while (head) {
			head.setType(head.nucleotide1.type, getPairType(head.nucleotide1.type));
			head.setRightColor(Color.white);
			head.needHelix = true;
			head = head.next;
			yield return new WaitForSeconds(timeGap);
		}
		animating = false;
	}

	public NucleotideCouple buildHalfCoupleChainFromOneSingle(Nucleotide chain) {
		if (chain.isPaired)
			return null;

		chain = getHeadOfSingleChain(chain);
		Nucleotide nhead = chain;

		NucleotideCouple n = (Instantiate(couplePrefab) as GameObject).GetComponent<NucleotideCouple>();
		NucleotideCouple head = n;
		n.setLeftColor(chain.getColor()); 
		n.setType(chain.type, Nucleotide.Type.Empty);
		n.needHelix = false;
		while (chain.next) {
			chain = chain.next;
			n.next = (Instantiate(couplePrefab) as GameObject).GetComponent<NucleotideCouple>();
			n.next.setLeftColor(chain.getColor()); 
			n.next.setType(chain.type, Nucleotide.Type.Empty);
			n.next.prev = n;
			n.next.needHelix = false;
			n.next.transform.rotation = nhead.transform.rotation;
			n = n.next;
		}
		head.transform.rotation = nhead.transform.rotation;
		head.transform.position = nhead.transform.position;
		head.broadcastUpdateTransform();
		destroySingleChain(chain);
		return head;
	}

	public NucleotideCouple buildCoupleChainFromTwoSingles(Nucleotide c1, Nucleotide c2, Vector3 position = default(Vector3)) {
		if (c1.isPaired || c2.isPaired)
			return null;

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
		while (head.next) {
			cnt += 1;
			head = head.next;
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
		if (animating)
			return;
		animating = true;
		NucleotideCouple head = getHeadOfCoupleChain(chain);
		StartCoroutine(duplicateCoupleChainRoutine(head));
    }

    public void transcriptFromCoupleChain(NucleotideCouple chain, string StartPoint, string EndPoint)
    {
        if (animating)
            return;
        animating = true;
        NucleotideCouple head = getHeadOfCoupleChain(chain);
        StartCoroutine(transcriptFromCoupleChainRoutine(head, StartPoint, EndPoint));
    }

    IEnumerator duplicateCoupleChainRoutine (NucleotideCouple head) {
		Debug.Log("DeHelix");
		deHelixCoupleChain(head);
		yield return new WaitForSeconds(5);
		Debug.Log("Build Single Chain");
		List<Nucleotide> singles = buildSingleChainsFromCouple(head);
		yield return new WaitForSeconds(5);
		animating = false;
		NucleotideCouple c1 = buildCoupleChainFromOneSingleAnimation(singles[0]);
		animating = false;
		NucleotideCouple c2 = buildCoupleChainFromOneSingleAnimation(singles[1]);	
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

    IEnumerator transcriptFromCoupleChainRoutine(NucleotideCouple n, string StartPoint, string EndPoint)
    {
        Queue<Nucleotide.Type> tempQueue = new Queue<Nucleotide.Type>();
        bool match = false;

        // 鏌ユ壘鍚姩瀛?
        for (int i = 0; i < StartPoint.Length; i++)
        {
            tempQueue.Enqueue(Nucleotide.Type.Empty);
        }
        tempQueue.Dequeue();
        tempQueue.Enqueue(n.getLeftType());
        while (n.next && !match)
        {
            n = n.next;
            tempQueue.Dequeue();
            tempQueue.Enqueue(n.getLeftType());

            match = true;
            Queue<Nucleotide.Type> checkQueue = new Queue<Nucleotide.Type>(tempQueue);
            for (int i = 0; i< StartPoint.Length; i++)
            {
                if (checkQueue.Dequeue() != Char2Type(StartPoint[i]))
                {
                    match = false;
                    break;
                }
            }
        }
        if (!match)
        {
            Debug.Log("娌℃湁鎵惧埌鍚姩瀛愶紒");
            animating = false;
            yield break;
        }

        // 杞綍鍚屾椂妫�娴嬬粓姝㈠瓙锛岄亣鍒扮粓姝㈠瓙鍋滄杞綍

        // 褰撳墠n鏄惎鍔ㄥ瓙搴忓垪鏈�鍚庝竴涓紝涓嶉渶瑕佽浆褰?
        tempQueue.Clear();
        for (int i = 0; i < EndPoint.Length; i++)
        {
            tempQueue.Enqueue(Nucleotide.Type.Empty);
        }

        match = false;
        if (!n.next)
        {
            Debug.Log("娌℃湁涓滆タ鍙互杞綍锛?");
            animating = false;
            yield break;
        }

        // 杞綍绗竴涓簭鍒?
        n = n.next;
        Nucleotide curRNA = (Instantiate(rnaPrefab) as GameObject).GetComponent<Nucleotide>();
        curRNA.setType(getPairType(n.getLeftType(), true)); // 浣跨敤isRNA妯″紡锛孉瀵瑰簲U
        curRNA.transform.position = n.transform.position + n.transform.rotation * Vector3.right * 2 * Nucleotide.gap;
        curRNA.transform.rotation = n.transform.rotation;
        yield return new WaitForSeconds(1);

        // 缁堟瀛愰暱搴﹁偗瀹氬ぇ浜?锛岃繖閲屽氨涓嶆娴嬩簡
        tempQueue.Dequeue();
        tempQueue.Enqueue(n.getLeftType());

        while (n.next && !match)
        {
            n = n.next;
            // 杞綍
            Nucleotide rna = (Instantiate(rnaPrefab) as GameObject).GetComponent<Nucleotide>();
            rna.setType(getPairType(n.getLeftType(), true));
            rna.transform.position = n.transform.position + n.transform.rotation * Vector3.right * 2 * Nucleotide.gap;
            rna.transform.rotation = n.transform.rotation;
            rna.addPrev(curRNA);
            curRNA = rna;
            yield return new WaitForSeconds(1);

            tempQueue.Dequeue();
            tempQueue.Enqueue(n.getLeftType());

            match = true;
            Queue<Nucleotide.Type> checkQueue = new Queue<Nucleotide.Type>(tempQueue);
            for (int i = 0; i < EndPoint.Length; i++)
            {
                if (checkQueue.Dequeue() != Char2Type(EndPoint[i]))
                {
                    match = false;
                    break;
                }
            }
        }
        if (!match)
        {
            Debug.Log("娌℃湁鎵惧埌缁堟瀛愶紒");
        }
        animating = false;
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
			result += Type2Char (n.getLeftType ());
			result += Type2Char(n.getRightType());
			n = n.next;
		}
		Debug.Log (result);
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
		n.setColor (Color.white);
		n.setType(Char2Type(s[1]), Char2Type(s[2]));
		for (int i = 3; i < s.Length; i+= 2) {
			n.next = (Instantiate(couplePrefab) as GameObject).GetComponent<NucleotideCouple>();
			n.next.setColor (Color.white);
			n.next.setType(Char2Type(s[i]), Char2Type(s[i+1]));
			n.next.prev = n;
			n = n.next;
		}
		head.transform.position = position;
		head.broadcastUpdateTransform();
		return n;
	}

	public Nucleotide.Type getPairType(Nucleotide.Type t, bool isRNA=false) {
		switch (t)
		{
			case Nucleotide.Type.A:
                if (isRNA) return Nucleotide.Type.U;
                else return Nucleotide.Type.T;
			case Nucleotide.Type.T:
				return Nucleotide.Type.A;
            case Nucleotide.Type.U:
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
            case 'U':
                return Nucleotide.Type.U;
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

    public static string reverseString(string s)
    {
        char[] charArray = s.ToCharArray();
        Array.Reverse(charArray);
        return new string(charArray);
    }
}
