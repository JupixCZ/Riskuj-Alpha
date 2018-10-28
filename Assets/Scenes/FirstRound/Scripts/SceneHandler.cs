using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneHandler : MonoBehaviour
{

    private enum Phase { CHOOSING, READING, WAITING, ANSWERING, RESOLUTING };
    private Phase activePhase;
    private List<Question> questions;
    private GameObject questionPanel;
    private Animator anim;

    private string firstKey;
    private string secondKey;
    private bool resolvingKey = false;
    private bool answeringAwaitingSpacebar;
    private bool answeringAwaitingPlayer;
    private int answeringTimerSec;

    // Use this for initialization
    void Start()
    {
        InitPlayers();
        InitQuestions();
        activePhase = Phase.CHOOSING;
        questionPanel = GameObject.Find("QuestionPanel");
        anim = questionPanel.GetComponent<Animator>();
        anim.enabled = false;
    }


    void Update()
    {
        if (Input.anyKeyDown)
        {
            string inputString = KeyUtil.ConvertKeyInput();
            ResolveKeyInput(inputString);
        }
    }

    public void InitPlayers()
    {
        Player firstPlayer = GlobalHandler.getPlayer(1);
        GameObject.Find("FirstPlayerName").GetComponent<NameComponent>().SetPlayer(firstPlayer);
        GameObject.Find("FirstPlayerScore").GetComponent<ScoreComponent>().SetPlayer(firstPlayer);

        Player secondPlayer = GlobalHandler.getPlayer(2);
        GameObject.Find("SecondPlayerName").GetComponent<NameComponent>().SetPlayer(secondPlayer);
        GameObject.Find("SecondPlayerScore").GetComponent<ScoreComponent>().SetPlayer(secondPlayer);

        Player thirdPlayer = GlobalHandler.getPlayer(3);
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
        switch (activePhase)
        {
            case Phase.CHOOSING: ResolveChoosingPhaseKeyInput(keyCode); break;
            case Phase.ANSWERING: ResolveAnsweringPhaseKeyInput(keyCode); break;
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

        if (question == null)
        {
            Debug.Log("Question for code " + finalKeyCode + " not found");
            return;
        }

        EnterAnsweringPhase(question);
    }

    private void EnterAnsweringPhase(Question question)
    {
        questionPanel.GetComponentInChildren<Text>().text = question.GetText();

        anim.enabled = true;
        anim.Play("QuestionSlideIn");

        activePhase = Phase.ANSWERING;

        answeringAwaitingSpacebar = true;
        answeringTimerSec = 3;
    }

    private void ResolveAnsweringPhaseKeyInput(string keyCode)
    {
        //anim.Play("QuestionSlideOut");
        if (answeringAwaitingSpacebar)
        {
            if (KeyUtil.IsSpacebar(keyCode))
            {
                answeringAwaitingSpacebar = false;
                answeringAwaitingPlayer = true;
                //TODO TIMER NOW
            }
        }
        else if (answeringAwaitingPlayer)
        {
            if (KeyUtil.IsPlayerArrow(keyCode))
            {
                GlobalHandler.ActivatePlayerByArrowCode(keyCode);
            }
        }
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
