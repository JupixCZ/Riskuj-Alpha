using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneHandler : MonoBehaviour
{
    private const string PREMIUM_CMP = "PremiumP";

    private enum Phase { CHOOSING, READING, WAITING, ANSWERING, RESOLUTING, INGOT };
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
        if (answeringTimerRunning)
        {
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

        InitPremium(firstPlayer);

        Player secondPlayer = GlobalHandler.getPlayer(2);
        GameObject.Find("SecondPlayerName").GetComponent<NameComponent>().SetPlayer(secondPlayer);
        GameObject.Find("SecondPlayerScore").GetComponent<ScoreComponent>().SetPlayer(secondPlayer);

        InitPremium(secondPlayer);

        Player thirdPlayer = GlobalHandler.getPlayer(3);
        //TODO
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
        questions = FileReader.GetFirstRoundQuestions();
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

    private void InitPremium(Player player) {
        int index = player.GetIndex();
        GameObject premiumCmp;
        string cmpName;

        List<GameObject> premiumImgs = new List<GameObject>();

        for (int i = 1; i < 6; i++)
        {
            cmpName = PREMIUM_CMP + index + "-" + i;
            premiumCmp = GameObject.Find(cmpName);
            premiumCmp.SetActive(false);
            premiumImgs.Add(premiumCmp);
        }

        player.SetPremiumCmps(premiumImgs);
    }

    private void UpdateTimer()
    {
        float currentAnsweringTime = Time.time - startAnsweringTime;
        float timePercentileExpired = currentAnsweringTime / (answeringTimerSec / 100);

        if (timePercentileExpired > 100)
        {
            if (activeQuestion.IsTopic()) {
                StopTimer();
                return;
            }

            audioSource.PlayOneShot(failSound);
            SetupResolutingPhase();
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
            case Phase.CHOOSING: ResolveChoosingPhaseKeyInput(keyCode); break;
            case Phase.READING: ResolveReadingPhaseKeyInput(keyCode); break;
            case Phase.WAITING: ResolveWaitingPhaseKeyInput(keyCode); break;
            case Phase.ANSWERING: ResolveAnsweringPhaseKeyInput(keyCode); break;
            case Phase.RESOLUTING: ResolveResolutingPhaseKeyInput(keyCode); break;
            case Phase.INGOT: ResolveIngotPhaseKeyInput(keyCode); break;
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
        GlobalHandler.RefreshPlayers();

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
        else if (KeyUtil.IsShift(keyCode))
        {
            if (activeQuestion.IsTopic())
            {
                StopTimer();
                SetupNewChoosingPhase(false, true);
                return;
            }
        }
        else if (KeyUtil.IsCtrl(keyCode))
        {
            if (activeQuestion.IsTopic())
            {
                StopTimer();
                SetupNewChoosingPhase(false);
                return;
            }

            GlobalHandler.ActivePlayerFailed(activeQuestion);
            GlobalHandler.DeactivatePlayers();

            if (GlobalHandler.AnyAnsweringPlayerLeft())
            {
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
        SetupNewChoosingPhase(claimPrize, false);
    }

    private void SetupNewChoosingPhase(bool claimPrize, bool premiumOnly)
    {
        if (claimPrize)
        {
            GlobalHandler.ActivePlayerClaimPrize(activeQuestion);
            audioSource.PlayOneShot(aplausSound);
        }
        else if (premiumOnly) {
            GlobalHandler.ActivePlayerClaimPremium();
            audioSource.PlayOneShot(aplausSound);
        }

        GlobalHandler.DeactivatePlayers();

        questions.Remove(activeQuestion);
        activeQuestionButton.SetActive(false);

        activeQuestionButton = null;
        activeQuestion = null;
        activePhase = Phase.CHOOSING;

        anim.Play("QuestionSlideOut");
    }

    private void SetupResolutingPhase()
    {
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

        Player player;

        if (KeyUtil.IsPlayerArrow(keyCode))
        {
            player = GlobalHandler.ActivatePlayerByArrowCode(keyCode, false);
        }
        else if (KeyUtil.IsSpacebar(keyCode))
        {

            player = GlobalHandler.GetActivePlayer();
            if (player == null)
            {
                return;
            }

            if (questions.Count == 1)
            {
                // END OF ROUND
                return;
            }

            SetupNewChoosingPhase(true);
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
