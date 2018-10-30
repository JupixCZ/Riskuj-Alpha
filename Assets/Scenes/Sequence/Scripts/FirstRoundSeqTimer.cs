using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FirstRoundSeqTimer : MonoBehaviour {

    void Start()
    {
        StartCoroutine(StartFirstRound());
    }

    public IEnumerator StartFirstRound()
    {
        float delayTime = GlobalHandler.IsDebugMode() ? 1f : 4f;

        yield return new WaitForSecondsRealtime(delayTime);
        SceneManager.LoadScene("FirstRound");
    }
}
