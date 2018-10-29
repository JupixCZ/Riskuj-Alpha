using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroTimer : MonoBehaviour {

	void Start () {
        StartCoroutine(StartFirstRound());
    }

    public IEnumerator StartFirstRound()
    {
        float delayTime = GlobalHandler.IsDebugMode() ? 1f : 14f;

        yield return new WaitForSecondsRealtime(delayTime);
        SceneManager.LoadScene("FirstRound");
    }
}
