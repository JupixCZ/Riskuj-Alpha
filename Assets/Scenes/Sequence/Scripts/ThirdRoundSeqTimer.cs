using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ThirdRoundSeqTimer : MonoBehaviour {

    void Start()
    {
        RemoveLoserPlayer();
        StartCoroutine(StartThirdRound());
    }

    private void RemoveLoserPlayer()
    {
        Player firstPlayer = GlobalHandler.getPlayer(1);
        Player secondPlayer = GlobalHandler.getPlayer(2);

        Player playerToRemove = null;

        if (firstPlayer.GetBalance() > secondPlayer.GetBalance())
        {
            playerToRemove = secondPlayer;
        }
        else if (firstPlayer.GetBalance() < secondPlayer.GetBalance())
        {
            playerToRemove = firstPlayer;
        }
        else if (firstPlayer.GetPremium() >= secondPlayer.GetPremium())
        {
            playerToRemove = secondPlayer;
        }
        else {
            playerToRemove = firstPlayer;
        }

        GlobalHandler.RemovePlayer(playerToRemove);
    }

    public IEnumerator StartThirdRound()
    {
        float delayTime = GlobalHandler.IsDebugMode() ? 1f : 5f;

        yield return new WaitForSecondsRealtime(delayTime);
        SceneManager.LoadScene("ThirdRound");
    }
}
