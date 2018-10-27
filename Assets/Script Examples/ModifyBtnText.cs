using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class modifyBtnText : MonoBehaviour {

    public Button zkus;

	// Use this for initialization
	void Start () {
        Debug.Log(zkus);
        Debug.Log(zkus.GetComponent<Text>());
        zkus.GetComponentInChildren<Text>().text = "juj";
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown("space"))
        {
            zkus.GetComponentInChildren<Text>().text = "la di da";
        }
    }
}
