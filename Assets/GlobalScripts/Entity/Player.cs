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

    public string getName() {
        return this.name;
    }

    public int getBalance() {
        return this.balance;
    }

    public void addToBalance(int num) {
        this.balance += num;
    }

    public int getPremium() {
        return this.premium;
    }
}
