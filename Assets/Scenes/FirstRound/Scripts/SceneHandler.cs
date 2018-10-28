﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneHandler : MonoBehaviour
{

    private enum Phase { CHOOSING, READING, WAITING, ANSWERING, RESOLUTING };
    private Phase activePhase;
    private List<Question> questions;
    private Question activeQuestion;
    private GameObject questionPanel;
    private GameObject activeQuestionButton;
    private Animator anim;

    private string firstKey;
    private string secondKey;
    private bool resolvingKey = false;

    private bool answeringAwaitingSpacebar;
    private bool answeringTimerRunning;
    private bool answeringPlayerAnswering;
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
        activeQuestion = FindByCode(finalKeyCode);

        if (activeQuestion == null)
        {
            Debug.Log("Question for code " + finalKeyCode + " not found");
            return;
        }

        activeQuestionButton = GameObject.Find(finalKeyCode);

        if (activeQuestionButton == null)
        {
            Debug.Log("Button for code " + finalKeyCode + " not found");
            return;
        }

        EnterAnsweringPhase();
    }

    private void EnterAnsweringPhase()
    {
        questionPanel.GetComponentInChildren<Text>().text = activeQuestion.GetText();

        anim.enabled = true;
        anim.Play("QuestionSlideIn");

        activePhase = Phase.ANSWERING;

        answeringPlayerAnswering = false;
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
                answeringTimerRunning = true;
                //TODO TIMER NOW
            }
        }
        else if (answeringTimerRunning)
        {
            if (KeyUtil.IsPlayerArrow(keyCode))
            {
                answeringPlayerAnswering = true;
                GlobalHandler.ActivatePlayerByArrowCode(keyCode);
            }
            else if (answeringPlayerAnswering)
            {
                if (KeyUtil.IsSpacebar(keyCode))
                {
                    GlobalHandler.ActivePlayerClaimPrize(activeQuestion);
                    GlobalHandler.DeactivatePlayers();

                    questions.Remove(activeQuestion);
                    activeQuestionButton.SetActive(false);

                    activeQuestionButton = null;
                    activeQuestion = null;
                    activePhase = Phase.CHOOSING;

                    anim.Play("QuestionSlideOut");
                }
                else if (KeyUtil.IsCtrl(keyCode))
                {
                    answeringAwaitingSpacebar = false;
                    answeringTimerRunning = true;
                    // less time
                    //TODO TIMER NOW
                }
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
