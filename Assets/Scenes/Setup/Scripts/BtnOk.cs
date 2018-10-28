using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BtnOk : MonoBehaviour {

    public void onClick() {
        string player1Name = GameObject.Find("IFfirstPlayer").GetComponent<InputField>().text;
        string player2Name = GameObject.Find("IFsecondPlayer").GetComponent<InputField>().text;

        GlobalHandler.initPlayers(player1Name, player2Name, null);

        //ScenesHandler.register(mainHandler);
        //ScenesHandler.startFirstRound();

        //mainHandler.startFirstRound();

        StartCoroutine(doIt(1.5f));
        SceneManager.LoadScene("Intro");

        

    }

    void Next()
    {
        Debug.Log("calling");
        //SceneManager.LoadScene("Setup");
    }

    public IEnumerator doIt(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        Debug.Log("calling");
        Next();
    }

}
