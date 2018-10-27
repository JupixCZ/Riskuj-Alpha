using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player {

    private string name;
    private int balance;
    private int premium;

    public Player(string name) {
        this.name = name;
        this.balance = 0;
        this.premium = 0;
    }
}
