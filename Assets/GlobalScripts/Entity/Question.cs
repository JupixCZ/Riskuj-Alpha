using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Question
{

    private string code;
    private int prize;
    private string text;
    private bool available;
    private bool ingot;
    private string topicName;
    private GameObject button;

    public Question(string code, string text, int prize, bool ingot)
    {
        this.code = code;
        this.text = text;
        this.prize = prize;
        this.ingot = ingot;
        this.available = true;
    }

    public bool IsCode(string code)
    {
        return this.code.Equals(code);
    }

    public string GetCode()
    {
        return this.code;
    }

    public int GetPrize()
    {
        return prize;
    }

    public string GetText()
    {
        return text;
    }

    public bool IsAvailable()
    {
        return available;
    }

    public bool IsIngot()
    {
        return ingot;
    }

    public void Disable()
    {
        this.available = false;
        button.SetActive(false);
    }

    public void SetTopicName(string topicName)
    {
        this.topicName = topicName;
    }

    public bool IsTopic()
    {
        return !string.IsNullOrEmpty(topicName);
    }

    public void SetButton(GameObject button)
    {
        this.button = button;
        if (IsTopic())
        {
            button.GetComponentInChildren<Text>().text = topicName;
        }
    }
}
