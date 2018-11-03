using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroTimer : MonoBehaviour {

	void Start () {
        StartCoroutine(LoadSetup());
    }

    public IEnumerator LoadSetup()
    {
        //float delayTime = GlobalHandler.IsDebugMode() ? 1f : 14f;
        float delayTime = 1f;

        yield return new WaitForSecondsRealtime(delayTime);

        if (GlobalHandler.IsEndGame())
        {
            Application.Quit();
        }
        else {
            SceneManager.LoadScene("Setup");
        }
    }
}
