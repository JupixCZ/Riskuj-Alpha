﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroTimer : MonoBehaviour {

	void Start () {
        // StartCoroutine(play(14f));
        StartCoroutine(StartFirstRound(1f));
    }

    public IEnumerator StartFirstRound(float delayTime)
    {
        yield return new WaitForSecondsRealtime(delayTime);
        SceneManager.LoadScene("FirstRound");
    }
}
