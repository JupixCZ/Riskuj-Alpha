using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneHandler : MonoBehaviour
{

    private enum Phase { CHOOSING, READING, WAITING, ANSWERING, RESOLUTING };
    private Phase activePhase;
    private List<Question> questions;
    private Question activeQuestion;
    private List<GameObject> timerBtns;
    private GameObject questionPanel;
    private GameObject activeQuestionButton;
    private Animator anim;

    private string firstKey;
    private string secondKey;

    private bool answeringTimerRunning;
    private float startAnsweringTime;
    private float answeringTimerSec;

    // Use this for initialization
    void Start()
    {
        InitPlayers();
        InitQuestions();
        InitTimer();
        activePhase = Phase.CHOOSING;
        questionPanel = GameObject.Find("QuestionPanel");
        anim = questionPanel.GetComponent<Animator>();
        anim.enabled = false;
        answeringTimerRunning = false;
    }

    void Update()
    {
        if (answeringTimerRunning) {
            UpdateTimer();
        }

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

    private void InitTimer() {
        timerBtns = new List<GameObject>();

        string timerBtnString = "BtnTime";

        for (int i = 1; i < 34; i++) {
            GameObject timerBtn = GameObject.Find(timerBtnString + i);
            timerBtn.SetActive(false);
            timerBtns.Add(timerBtn);
        }
    }

    private void UpdateTimer() {
        float currentAnsweringTime = Time.time - startAnsweringTime;
        float timePercentileExpired = currentAnsweringTime / (answeringTimerSec / 100);

        if (timePercentileExpired > 100) {
            SetupResolutingPhase();
            return;
        }
        Debug.Log(timePercentileExpired);

        int btnCountToShow = (int) timePercentileExpired / 3;
        int currentBtn = 1;

        foreach (GameObject timerBtn in timerBtns) {
            if (currentBtn > btnCountToShow) {
                break;
            }

            timerBtn.SetActive(true);
            currentBtn++;
        }
    }

    private void StopTimer() {
        answeringTimerRunning = false;

        foreach (GameObject timerBtn in timerBtns) {
            timerBtn.SetActive(false);
        }
    }

    private void StartTimer() {
        startAnsweringTime = Time.time;
        answeringTimerRunning = true;
    }

    private void ResolveKeyInput(string keyCode)
    {
        switch (activePhase)
        {
            case Phase.CHOOSING: ResolveChoosingPhaseKeyInput(keyCode); break;
            case Phase.READING: ResolveReadingPhaseKeyInput(keyCode); break;
            case Phase.WAITING: ResolveWaitingPhaseKeyInput(keyCode); break;
            case Phase.ANSWERING: ResolveAnsweringPhaseKeyInput(keyCode); break;
            case Phase.RESOLUTING: ResolveResolutingPhaseKeyInput(keyCode); break;
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

        SetupReadingPhase();
    }

    private void SetupReadingPhase()
    {
        GlobalHandler.RefreshPlayers();

        questionPanel.GetComponentInChildren<Text>().text = activeQuestion.GetText();

        anim.enabled = true;
        anim.Play("QuestionSlideIn");

        activePhase = Phase.READING;

        answeringTimerSec = 5;
    }

    private void ResolveReadingPhaseKeyInput(string keyCode)
    {
        if (KeyUtil.IsSpacebar(keyCode))
        {
            SetupWaitingPhase();
        }
    }

    private void SetupWaitingPhase()
    {
        activePhase = Phase.WAITING;
        answeringTimerSec--;
        StartTimer();
    }

    private void ResolveWaitingPhaseKeyInput(string keyCode)
    {
        if (KeyUtil.IsPlayerArrow(keyCode))
        {
            SetupAnsweringPhase(keyCode);
        }
    }

    private void SetupAnsweringPhase(string keyCode)
    {
        Player player = GlobalHandler.ActivatePlayerByArrowCode(keyCode, true);
        if (player != null)
        {
            activePhase = Phase.ANSWERING;
            StopTimer();
        }
    }

    private void ResolveAnsweringPhaseKeyInput(string keyCode)
    {
        if (KeyUtil.IsPlayerArrow(keyCode))
        {
            SetupAnsweringPhase(keyCode);
        }
        else if (KeyUtil.IsSpacebar(keyCode))
        {
            if (questions.Count == 1)
            {
                // END OF ROUND
                return;
            }

            SetupNewChoosingPhase(true);
        }
        else if (KeyUtil.IsCtrl(keyCode))
        {
            GlobalHandler.ActivePlayerFailed(activeQuestion);
            GlobalHandler.DeactivatePlayers();

            if (GlobalHandler.AnyAnsweringPlayerLeft()) {
                SetupWaitingPhase();
            }
            else
            {
                SetupResolutingPhase();
            }

        }
    }

    private void SetupNewChoosingPhase(bool claimPrize)
    {
        if (claimPrize) {
            GlobalHandler.ActivePlayerClaimPrize(activeQuestion);
        }

        GlobalHandler.DeactivatePlayers();

        questions.Remove(activeQuestion);
        activeQuestionButton.SetActive(false);

        activeQuestionButton = null;
        activeQuestion = null;
        activePhase = Phase.CHOOSING;

        anim.Play("QuestionSlideOut");
    }

    private void SetupResolutingPhase() {
        StopTimer();
        activePhase = Phase.RESOLUTING;
    }

    private void ResolveResolutingPhaseKeyInput(string keyCode)
    {
        if (KeyUtil.IsSpacebar(keyCode))
        {
            if (questions.Count == 1)
            {
                // END OF ROUND
                return;
            }

            SetupNewChoosingPhase(false);
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
