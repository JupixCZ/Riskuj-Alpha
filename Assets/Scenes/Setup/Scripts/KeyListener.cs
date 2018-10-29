using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class KeyListener : MonoBehaviour
{
    public InputField infFirstPlayer;
    public InputField infSecondPlayer;
    public InputField infThirdPlayer;

    private int selectedInfIndex;

    public void Start()
    {
        selectedInfIndex = 1;
        SelectInputField();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            string player1Name = infFirstPlayer.text;
            string player2Name = infSecondPlayer.text;
            string player3Name = null;

            if (System.String.IsNullOrEmpty(player1Name) || System.String.IsNullOrEmpty(player2Name))
            {
                return;
            }

            GlobalHandler.InitPlayers(player1Name, player2Name, player3Name);

            SceneManager.LoadScene("Intro");
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (selectedInfIndex != 1)
            {
                selectedInfIndex--;
                SelectInputField();
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (selectedInfIndex != 3)
            {
                selectedInfIndex++;
                SelectInputField();
            }
        }
        else if (Input.GetKeyDown(KeyCode.F1)) {
            GlobalHandler.ActivateDebugMode();
        }
    }

    private void SelectInputField()
    {
        InputField selectedInputField = null;

        switch (selectedInfIndex)
        {
            case 1: selectedInputField = infFirstPlayer; break;
            case 2: selectedInputField = infSecondPlayer; break;
            case 3: selectedInputField = infThirdPlayer; break;
            default: Debug.Log("Invalid input field index"); break;
        }

        if (selectedInputField != null) {
            selectedInputField.ActivateInputField();
        }
    }
}
