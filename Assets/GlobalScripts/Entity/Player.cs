using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    private string name;
    private int balance;
    private int premium;
    private bool wrongAnswer;
    private bool active;

    public Player(string name)
    {
        this.name = name;
        this.balance = 0;
        this.premium = 0;
        this.active = false;
        this.wrongAnswer = false;
    }

    public string GetName()
    {
        return this.name;
    }

    public int GetBalance()
    {
        return this.balance;
    }

    public void AddToBalance(int num)
    {
        this.balance += num;
    }

    public int GetPremium()
    {
        return this.premium;
    }

    public bool IsActive()
    {
        return active;
    }

    public void SetActive(bool active)
    {
        this.active = active;
    }

    public bool IsWrongAnswer() {
        return this.wrongAnswer;
    }

    public void AnsweredWrong(int penalty) {
        this.balance -= penalty;
        this.wrongAnswer = true;
    }

    public void RefreshAnswering() {
        this.wrongAnswer = false;
    }
}
