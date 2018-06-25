using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cutter : MonoBehaviour {
    public string seqBeforeCutPoint;
    public string seqAfterCutPoint;
    
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void test() {
        Debug.Log("hello");
    }

    public void exchangeSeq()//切一侧之后在另一侧应该是对称的，得把判断的seq对换一下，因为我们的couple不分左右，似乎没法达到切割双条链时判断方向调换
    {
        string tmp = seqAfterCutPoint;
        seqAfterCutPoint = seqBeforeCutPoint;
        seqBeforeCutPoint = tmp;
    }



}
