using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NameComponent : MonoBehaviour {

    public Text playerNameText;
    private Player player;
    private bool currentStateActive;
	// Use this for initialization
	void Start () {
        currentStateActive = false;
	}

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            return;
        }

        UpdateActivity();
    }

    public void SetPlayer(Player player)
    {
        this.player = player;
        this.playerNameText.text = player.GetName();
    }

    private void UpdateActivity()
    {
        bool active = player.IsActive();
        if (active && !currentStateActive) {
            GetComponent<Image>().color = Color.red;
            currentStateActive = true;
        } else if (!active && currentStateActive) {
            GetComponent<Image>().color = Color.white;
            currentStateActive = false;
        }
    }
}
