using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Question {

    private string code;
    private int prize;
    private string text;
    private bool premium;
    private bool available;
    private GameObject button;

    public enum QuestionMode {FIRST_ROUND, SECOND_ROUND};

    public Question(string code, QuestionMode mode, string text, GameObject button) {
        string firstChar = code.Substring(0, 1);
        string secondChar = code.Substring(1, 1);
        int prize = 0;

        if (mode == QuestionMode.FIRST_ROUND)
        {
            prize = int.Parse(secondChar) * 1000;
            Init(code, prize, text, button);
        }
        else if (mode == QuestionMode.SECOND_ROUND) {
            prize = (int.Parse(firstChar) * 10) + (int.Parse(secondChar));
            prize = prize * 1000;
            Init(code, prize, text, button);
        }
    }

    private void Init(string code, int prize, string text, GameObject button) {
        this.code = code;
        this.prize = prize;
        this.text = text;
        this.premium = prize == 0;
        this.available = true;
        this.button = button;
    }

    public bool IsCode(string code) {
        return this.code.Equals(code);
    }

    public int GetPrize() {
        return prize;
    }

    public string GetText() {
        return text;
    }

    public bool IsPremium() {
        return premium;
    }

    public bool IsAvailable() {
        return available;
    }

    public void Disable() {
        this.available = false;
        button.SetActive(false);
    }
}
