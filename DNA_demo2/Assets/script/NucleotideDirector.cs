using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NucleotideDirector : MonoBehaviour {
	private static NucleotideDirector instance;
	public GameObject couplePrefab;
	public GameObject singlePrefab;
    public GameObject rnaPrefab;

	private bool duplicating = false;
    private bool transcripting = false;

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

	public void duplicateCoupleChain (NucleotideCouple chain) {
		if (duplicating)
			return;
		duplicating = true;
		NucleotideCouple head = getHeadOfCoupleChain(chain);
		StartCoroutine(duplicateCoupleChainRoutine(head));
    }

    public void transcriptFromCoupleChain(NucleotideCouple chain, string StartPoint, string EndPoint)
    {
        if (transcripting)
            return;
        transcripting = true;
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

    IEnumerator transcriptFromCoupleChainRoutine(NucleotideCouple n, string StartPoint, string EndPoint)
    {
        Queue<Nucleotide.Type> tempQueue = new Queue<Nucleotide.Type>();
        bool match = false;

        // 查找启动子
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
            Debug.Log("没有找到启动子！");
            transcripting = false;
            yield break;
        }

        // 转录同时检测终止子，遇到终止子停止转录

        // 当前n是启动子序列最后一个，不需要转录
        tempQueue.Clear();
        for (int i = 0; i < EndPoint.Length; i++)
        {
            tempQueue.Enqueue(Nucleotide.Type.Empty);
        }

        match = false;
        if (!n.next)
        {
            Debug.Log("没有东西可以转录！");
            transcripting = false;
            yield break;
        }

        // 转录第一个序列
        n = n.next;
        Nucleotide curRNA = (Instantiate(rnaPrefab) as GameObject).GetComponent<Nucleotide>();
        curRNA.setType(getPairType(n.getLeftType(), true)); // 使用isRNA模式，A对应U
        curRNA.transform.position = n.transform.position + n.transform.rotation * Vector3.right * 2 * Nucleotide.gap;
        curRNA.transform.rotation = n.transform.rotation;
        yield return new WaitForSeconds(1);

        // 终止子长度肯定大于1，这里就不检测了
        tempQueue.Dequeue();
        tempQueue.Enqueue(n.getLeftType());

        while (n.next && !match)
        {
            n = n.next;
            // 转录
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
            Debug.Log("没有找到终止子！");
        }
        transcripting = false;
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

	public void helixCoupleChain (NucleotideCouple chain) {
		NucleotideCouple n = getHeadOfCoupleChain(chain);
		while (n) {
			n.needHelix = true;
			n = n.next;
		}
	}

	public Nucleotide.Type getPairType(Nucleotide.Type t, bool isRNA = false) {
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
}
