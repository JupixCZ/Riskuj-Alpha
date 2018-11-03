using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NameComponent3 : MonoBehaviour {

    public Text playerNameText;
    private Player player;
    private Sprite imgInactive;

    void Start()
    {
        GetComponent<Image>().sprite = Resources.Load<Sprite>("Image/topic");
    }
}
