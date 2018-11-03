using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneHandler3 : MonoBehaviour
{

    private const string PREMIUM_CMP = "PremiumP";

    private enum Phase { STAKING, CHOOSING, READING, ANSWERING, INGOT, END_ROUND };
    private Phase activePhase;
    private List<Question> questions;
    private Question activeQuestion;
    private List<GameObject> timerBtns;
    private GameObject questionPanel;
    private GameObject activeQuestionButton;
    private GameObject imgIngot;
    private Text stakeScore;
    private Text winScore;
    private Sprite imgSelectedQuestion;
    private Animator anim;
    private Player player;

    private int currentStake;
    private int totalWin;
    private int questionTries;

    private AudioSource audioSource;
    private AudioClip aplausSound;
    private AudioClip ingotSound;
    private AudioClip questionSound;
    private AudioClip failSound;

    private string firstKey;
    private string secondKey;

    private bool answeringTimerRunning;
    private float startAnsweringTime;
    private float answeringTimerSec;

    void Start()
    {
        InitPlayer();
        InitQuestions();
        InitTimer();
        InitSounds();
        activePhase = Phase.STAKING;
        questionPanel = GameObject.Find("QuestionPanel");
        imgSelectedQuestion = Resources.Load<Sprite>("Image/selectedfinbutton");
        imgIngot = GameObject.Find("IngotImage");
        imgIngot.SetActive(false);
        stakeScore = GameObject.Find("StakeScore").GetComponentInChildren<Text>();
        winScore = GameObject.Find("WinScore").GetComponentInChildren<Text>();
        totalWin = 0;
        currentStake = 0;
        questionTries = 0;
        anim = questionPanel.GetComponent<Animator>();
        anim.enabled = false;
        answeringTimerRunning = false;
    }

    void Update()
    {
        if (answeringTimerRunning)
        {
            UpdateAnsweringTimer();
        }

        if (Input.anyKeyDown)
        {
            string inputString = KeyUtil.ConvertKeyInput();
            ResolveKeyInput(inputString);
        }
    }

    public void InitPlayer()
    {
        player = GlobalHandler.GetLastPlayer();
        player.EnsureMinimumFinalBalance();
        GameObject.Find("PlayerName").GetComponent<NameComponent2>().SetPlayer(player);
        GameObject.Find("PlayerScore").GetComponent<ScoreComponent2>().SetPlayer(player);

        InitPremium(player);
    }

    private void InitSounds()
    {
        audioSource = GameObject.Find("AudioSource").GetComponent<AudioSource>();
        aplausSound = Resources.Load<AudioClip>("Sound/aplaus");
        ingotSound = Resources.Load<AudioClip>("Sound/ingot");
        questionSound = Resources.Load<AudioClip>("Sound/question");
        failSound = Resources.Load<AudioClip>("Sound/fail");
    }

    private void InitQuestions()
    {
        questions = FileReader.GetThirdRoundQuestions();
        PairQuestionsWithButtons(questions);
    }

    private void PairQuestionsWithButtons(List<Question> questions)
    {
        GameObject button = null;
        foreach (Question question in questions)
        {
            button = GameObject.Find("b" + question.GetCode());
            question.SetButton(button);
        }
    }

    private void InitTimer()
    {
        timerBtns = new List<GameObject>();

        string timerBtnString = "BtnTime";

        for (int i = 1; i < 34; i++)
        {
            GameObject timerBtn = GameObject.Find(timerBtnString + i);
            timerBtn.SetActive(false);
            timerBtns.Add(timerBtn);
        }
    }

    private void InitPremium(Player player)
    {
        GameObject premiumCmp;
        string cmpName;
        int premium = player.GetPremium();
        bool active;

        for (int i = 1; i < 6; i++)
        {
            cmpName = PREMIUM_CMP + "1-" + i;
            premiumCmp = GameObject.Find(cmpName);

            active = i <= premium;
            premiumCmp.SetActive(active);
        }
    }

    private void UpdateAnsweringTimer()
    {
        float currentAnsweringTime = Time.time - startAnsweringTime;
        float timePercentileExpired = currentAnsweringTime / (answeringTimerSec / 100);

        if (timePercentileExpired > 100)
        {
            StopTimer();
            audioSource.PlayOneShot(failSound);
            return;
        }

        int btnCountToShow = (int)timePercentileExpired / 3;
        int currentBtn = 1;

        foreach (GameObject timerBtn in timerBtns)
        {
            if (currentBtn > btnCountToShow)
            {
                break;
            }

            timerBtn.SetActive(true);
            currentBtn++;
        }
    }

    private void StopTimer()
    {
        answeringTimerRunning = false;

        foreach (GameObject timerBtn in timerBtns)
        {
            timerBtn.SetActive(false);
        }
    }

    private void StartTimer()
    {
        startAnsweringTime = Time.time;
        answeringTimerRunning = true;
    }

    private void ResolveKeyInput(string keyCode)
    {
        switch (activePhase)
        {
            case Phase.STAKING: ResolveStakingPhaseKeyInput(keyCode); break;
            case Phase.CHOOSING: ResolveChoosingPhaseKeyInput(keyCode); break;
            case Phase.READING: ResolveReadingPhaseKeyInput(keyCode); break;
            case Phase.ANSWERING: ResolveAnsweringPhaseKeyInput(keyCode); break;
            case Phase.INGOT: ResolveIngotPhaseKeyInput(keyCode); break;
            case Phase.END_ROUND: ResolveEndRoundPhaseKeyInput(keyCode); break;
        }
    }

    private void ResolveStakingPhaseKeyInput(string keyCode)
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
        int plannedStake = Convert.ToInt32(finalKeyCode) * 1000;
        if (player.GetBalance() < plannedStake)
        {
            return;
        }

        player.TakeFromBalance(plannedStake);
        stakeScore.text = plannedStake.ToString();
        currentStake = plannedStake;

        SetupChoosingPhase();
    }

    private void SetupChoosingPhase()
    {
        firstKey = null;
        secondKey = null;
        activePhase = Phase.CHOOSING;
    }

    private void ResolveChoosingPhaseKeyInput(string keyCode)
    {
        if (!KeyUtil.IsNumber(keyCode))
        {
            return;
        }

        activeQuestion = FindByCode(keyCode);

        if (activeQuestion == null)
        {
            Debug.Log("Question for code " + keyCode + " not found");
            return;
        }

        activeQuestionButton = GameObject.Find("b" + keyCode);

        if (activeQuestionButton == null)
        {
            Debug.Log("Button for code b" + keyCode + " not found");
            return;
        }

        activeQuestionButton.GetComponent<Image>().sprite = imgSelectedQuestion;

        if (activeQuestion.IsIngot())
        {
            SetupIngotPhase();
        }
        else
        {
            SetupReadingPhase();
        }
    }

    private void SetupReadingPhase()
    {
        questionPanel.GetComponentInChildren<Text>().text = activeQuestion.GetText();
        imgIngot.SetActive(false);

        anim.enabled = true;
        anim.Play("QuestionSlideIn");
        audioSource.PlayOneShot(questionSound);

        activePhase = Phase.READING;

        answeringTimerSec = 8;
    }

    private void ResolveReadingPhaseKeyInput(string keyCode)
    {
        if (KeyUtil.IsSpacebar(keyCode))
        {
            SetupAnsweringPhase();
        }
    }

    private void SetupAnsweringPhase()
    {
        activePhase = Phase.ANSWERING;
        StartTimer();
    }

    private void ResolveAnsweringPhaseKeyInput(string keyCode)
    {
        if (KeyUtil.IsSpacebar(keyCode))
        {
            SetupNewStakingPhase(true);
        }
        else if (KeyUtil.IsCtrl(keyCode))
        {
            SetupNewStakingPhase(false);
        }
    }

    private void SetupNewStakingPhase(bool claimPrize)
    {
        if (claimPrize)
        {
            totalWin += currentStake;
            winScore.text = totalWin.ToString();
            audioSource.PlayOneShot(aplausSound);
        }

        questionTries++;

        int zeroStake = 0;
        stakeScore.text = zeroStake.ToString();

        StopTimer();

        questions.Remove(activeQuestion);
        activeQuestionButton.SetActive(false);

        activeQuestionButton = null;
        activeQuestion = null;
        activePhase = Phase.STAKING;

        anim.Play("QuestionSlideOut");

        if (questionTries > 2)
        {
            SetupEndRoundPhase();
        }
    }

    private void SetupIngotPhase()
    {
        questionPanel.GetComponentInChildren<Text>().text = null;
        imgIngot.SetActive(true);

        audioSource.PlayOneShot(ingotSound);
        anim.enabled = true;
        anim.Play("QuestionSlideIn");

        activePhase = Phase.INGOT;
    }

    private void ResolveIngotPhaseKeyInput(string keyCode)
    {
        if (KeyUtil.IsSpacebar(keyCode))
        {
            SetupNewStakingPhase(true);
        }
    }

    private void SetupEndRoundPhase()
    {
        activePhase = Phase.END_ROUND;
    }

    private void ResolveEndRoundPhaseKeyInput(string keyCode)
    {
        if (KeyUtil.IsSpacebar(keyCode))
        {
            GlobalHandler.EndGame();
            SceneManager.LoadScene("Intro");
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
