using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneHandler2 : MonoBehaviour
{

    private const string PREMIUM_CMP = "PremiumP";

    private enum Phase { CHOOSING, READING, ANSWERING, INGOT, END_ROUND };
    private Phase activePhase;
    private List<Question> questions;
    private Question activeQuestion;
    private List<GameObject> timerBtns;
    private GameObject questionPanel;
    private GameObject activeQuestionButton;
    private GameObject imgIngot;
    private Sprite imgSelectedQuestion;
    private Animator anim;

    private AudioSource audioSource;
    private AudioClip aplausSound;
    private AudioClip ingotSound;
    private AudioClip questionSound;
    private AudioClip failSound;
    private AudioClip timeOutSound; //TODO

    private string firstKey;
    private string secondKey;

    private bool answeringTimerRunning;
    private float startAnsweringTime;
    private float answeringTimerSec;

    private bool roundTimerRunning;
    private float startRoundTime;
    private int roundTimerAlert;
    private int firstTimeAlertSec;
    private int secondTimeAlertSec;
    private int thirdTimeAlertSec;
    private int finalTimeAlertSec;
    private bool timeOut;

    void Start()
    {
        InitPlayers();
        InitQuestions();
        InitTimers();
        InitSounds();
        activePhase = Phase.CHOOSING;
        questionPanel = GameObject.Find("QuestionPanel");
        imgSelectedQuestion = Resources.Load<Sprite>("Image/selectedQuestion");
        imgIngot = GameObject.Find("IngotImage");
        imgIngot.SetActive(false);
        anim = questionPanel.GetComponent<Animator>();
        anim.enabled = false;
        answeringTimerRunning = false;
    }

    void Update()
    {
        if (roundTimerRunning) {
            UpdateRoundTimer();
        }

        if (answeringTimerRunning)
        {
            UpdateAnsweringTimer();
        }

        if (Input.anyKeyDown)
        {
            if (!roundTimerRunning && !timeOut) {
                StartRoundTimer();
            }

            string inputString = KeyUtil.ConvertKeyInput();
            ResolveKeyInput(inputString);
        }
    }

    public void InitPlayers()
    {
        Player firstPlayer = GlobalHandler.getPlayer(1); //TODO Bad for 3 player game
        GameObject.Find("FirstPlayerName").GetComponent<NameComponent2>().SetPlayer(firstPlayer);
        GameObject.Find("FirstPlayerScore").GetComponent<ScoreComponent2>().SetPlayer(firstPlayer);

        InitPremium(firstPlayer);

        Player secondPlayer = GlobalHandler.getPlayer(2); //TODO Bad for 3 player game
        GameObject.Find("SecondPlayerName").GetComponent<NameComponent2>().SetPlayer(secondPlayer);
        GameObject.Find("SecondPlayerScore").GetComponent<ScoreComponent2>().SetPlayer(secondPlayer);

        InitPremium(secondPlayer);

        if (secondPlayer.GetBalance() > firstPlayer.GetBalance())
        {
            secondPlayer.SetActive(true);
            firstPlayer.SetActive(false);
        }
        else {
            firstPlayer.SetActive(true);
            secondPlayer.SetActive(false);
        }
    }

    private void InitSounds()
    {
        audioSource = GameObject.Find("AudioSource").GetComponent<AudioSource>();
        aplausSound = Resources.Load<AudioClip>("Sound/aplaus");
        ingotSound = Resources.Load<AudioClip>("Sound/ingot");
        questionSound = Resources.Load<AudioClip>("Sound/question");
        timeOutSound = Resources.Load<AudioClip>("Sound/timeout");
        failSound = Resources.Load<AudioClip>("Sound/fail");
    }

    private void InitQuestions()
    {
        questions = FileReader.GetSecondRoundQuestions();
        PairQuestionsWithButtons(questions);
    }

    private void PairQuestionsWithButtons(List<Question> questions)
    {
        GameObject button = null;
        foreach (Question question in questions)
        {
            button = GameObject.Find(question.GetCode());
            question.SetButton(button);
        }
    }

    private void InitTimers()
    {
        timerBtns = new List<GameObject>();

        string timerBtnString = "BtnTime";

        for (int i = 1; i < 34; i++)
        {
            GameObject timerBtn = GameObject.Find(timerBtnString + i);
            timerBtn.SetActive(false);
            timerBtns.Add(timerBtn);
        }

        roundTimerRunning = false;
        timeOut = false;
        firstTimeAlertSec = GlobalHandler.IsDebugMode() ? 20 : 300;
        secondTimeAlertSec = GlobalHandler.IsDebugMode() ? 40 : 360;
        thirdTimeAlertSec = GlobalHandler.IsDebugMode() ? 60 : 420;
        finalTimeAlertSec = GlobalHandler.IsDebugMode() ? 80 : 480;
    }

    private void InitPremium(Player player)
    {
        int index = player.GetIndex();
        GameObject premiumCmp;
        string cmpName;
        int premium = player.GetPremium();
        bool active;

        for (int i = 1; i < 6; i++)
        {
            cmpName = PREMIUM_CMP + index + "-" + i;
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

    private void UpdateRoundTimer()
    {
        float timeSecExpired = Time.time - startRoundTime;

        if (timeSecExpired > firstTimeAlertSec && roundTimerAlert < 1)
        {
            roundTimerAlert++;
            audioSource.PlayOneShot(timeOutSound);
            return;
        }

        if (timeSecExpired > secondTimeAlertSec && roundTimerAlert < 2)
        {
            roundTimerAlert++;
            audioSource.PlayOneShot(timeOutSound);
            return;
        }

        if (timeSecExpired > thirdTimeAlertSec && roundTimerAlert < 3) {
            roundTimerAlert++;
            audioSource.PlayOneShot(timeOutSound);
            return;
        }

        if (timeSecExpired > finalTimeAlertSec) {
            audioSource.PlayOneShot(timeOutSound);
            StopRoundTimer();
            return;
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

    private void StartRoundTimer() {
        startRoundTime = Time.time;
        roundTimerRunning = true;
    }

    private void StopRoundTimer() {
        roundTimerRunning = false;
        timeOut = true;
    }

    private void ResolveKeyInput(string keyCode)
    {
        switch (activePhase)
        {
            case Phase.CHOOSING: ResolveChoosingPhaseKeyInput(keyCode); break;
            case Phase.READING: ResolveReadingPhaseKeyInput(keyCode); break;
            case Phase.ANSWERING: ResolveAnsweringPhaseKeyInput(keyCode); break;
            case Phase.INGOT: ResolveIngotPhaseKeyInput(keyCode); break;
            case Phase.END_ROUND: ResolveEndRoundPhaseKeyInput(keyCode); break;
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

        answeringTimerSec = 5;
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
            SetupNewChoosingPhase(true);
        }
        else if (KeyUtil.IsCtrl(keyCode))
        {
            SetupNewChoosingPhase(false);
        }
    }

    private void SetupNewChoosingPhase(bool claimPrize)
    {
        if (claimPrize)
        {
            GlobalHandler.ActivePlayerClaimPrize(activeQuestion);
            audioSource.PlayOneShot(aplausSound);
        }
        else {
            GlobalHandler.ActivePlayerFailed(activeQuestion);
        }

        StopTimer();
        GlobalHandler.SwitchActivePlayers();

        questions.Remove(activeQuestion);
        activeQuestionButton.SetActive(false);

        activeQuestionButton = null;
        activeQuestion = null;
        activePhase = Phase.CHOOSING;

        anim.Play("QuestionSlideOut");

        if (questions.Count == 0 || timeOut)
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
            SetupNewChoosingPhase(true);
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
           SceneManager.LoadScene("ThirdRoundSeq");
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
