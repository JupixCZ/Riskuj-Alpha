﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreComponent : MonoBehaviour {

    public Text balanceText;

    private Player player;
	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    void Update() {
        if (player == null) {
            return;
        }

        UpdateBalance();
	}

    public void SetPlayer(Player player) {
        this.player = player;
    }

    private void UpdateBalance() {
        int balance = player.GetBalance();
        this.balanceText.text = balance.ToString();
    }
}
