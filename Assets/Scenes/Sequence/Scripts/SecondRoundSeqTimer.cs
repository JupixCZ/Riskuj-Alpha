using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SecondRoundSeqTimer : MonoBehaviour {

    void Start()
    {
        StartCoroutine(StartSecondRound());
    }

    public IEnumerator StartSecondRound()
    {
        float delayTime = GlobalHandler.IsDebugMode() ? 1f : 5f;

        yield return new WaitForSecondsRealtime(delayTime);
        SceneManager.LoadScene("SecondRound");
    }
}
