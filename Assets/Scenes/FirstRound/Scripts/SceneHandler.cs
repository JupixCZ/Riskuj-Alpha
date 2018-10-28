using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneHandler : MonoBehaviour
{

    private enum Phase { CHOOSING, READING, WAITING, ANSWERING, RESOLUTING };
    private Phase activePhase;
    private List<Question> questions;
    private string firstKey;
    private string secondKey;
    private bool resolvingKey = false;

    // Use this for initialization
    void Start()
    {
        InitPlayers();
        InitQuestions();
        activePhase = Phase.CHOOSING;
    }


    void Update()
    {
        if (Input.anyKeyDown)
        {
            ResolveKeyInput(Input.inputString);
        }
    }

    public void InitPlayers()
    {
        Player firstPlayer = GlobalStats.getPlayer(1);
        GameObject.Find("FirstPlayerName").GetComponentInChildren<Text>().text = firstPlayer.getName();
        GameObject.Find("FirstPlayerScore").GetComponent<ScoreComponent>().setPlayer(firstPlayer);

        Player secondPlayer = GlobalStats.getPlayer(2);
        GameObject.Find("SecondPlayerName").GetComponentInChildren<Text>().text = secondPlayer.getName();
        GameObject.Find("SecondPlayerScore").GetComponent<ScoreComponent>().setPlayer(secondPlayer);

        Player thirdPlayer = GlobalStats.getPlayer(3);
        //TODO
    }

    private void InitQuestions()
    {
        questions = new List<Question>();
        Question question = null;
        GameObject button = null;

        string code = null;
        for (int i = 1; i < 2; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                code = (i * 10 + j).ToString();
                button = GameObject.Find(code);
                question = new Question(code, Question.QuestionMode.FIRST_ROUND, "tak co myslíš " + code, button);
                questions.Add(question);
            }
        }
    }

    private void ResolveKeyInput(string keyCode)
    {
        if (activePhase == Phase.CHOOSING)
        {
            ResolveChoosingPhaseKeyInput(keyCode);
        }
    }

    private void ResolveChoosingPhaseKeyInput(string keyCode)
    {
        if (!KeyUtil.IsNumber(keyCode))
        {
            return;
        }

        if (firstKey == null || secondKey != null)
        {
            firstKey = keyCode;
            secondKey = null;

            return;
        }
        else if (secondKey == null)
        {
            secondKey = keyCode;
        }

        string finalKeyCode = firstKey + secondKey;
        Question question = FindByCode(finalKeyCode);

        if (question == null) {
            Debug.Log("Question for code " + finalKeyCode + " not found");
            return;
        }

        Debug.Log(question.GetText());
    }

    private Question FindByCode(string code)
    {
        foreach (Question item in questions)
        {
            if (item.IsCode(code))
            {
                return item;
            }
        }

        return null;
    }


}
