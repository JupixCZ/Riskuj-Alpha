using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class KeyUtil
{

    //TODO redo to KEYCODES! maybe not needed at all!
    private const string SHIFT = "shift";
    private const string SPACEBAR = "space";
    private const string CTRL = "ctrl";
    private const string ARROW_LEFT = "left";
    private const string ARROW_RIGHT = "right";
    private const string ARROW_DOWN = "down";

    private static string[] numKeyCodes = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
    private static string[] playerArrowCodes = new string[] { ARROW_LEFT, ARROW_RIGHT, ARROW_DOWN };

    public static bool IsNumber(string keyCode)
    {
        return System.Array.IndexOf(numKeyCodes, keyCode) != -1;
    }

    public static bool IsSpacebar(string keyCode)
    {
        return keyCode.Equals(SPACEBAR);
    }

    public static bool IsPlayerArrow(string keyCode)
    {
        return System.Array.IndexOf(playerArrowCodes, keyCode) != -1;
    }

    public static bool IsCtrl(string keyCode)
    {
        return keyCode.Equals(CTRL);
    }

    public static bool IsShift(string keyCode)
    {
        return keyCode.Equals(SHIFT);
    }

    public static string ConvertKeyInput()
    {
        if (Input.GetKeyDown(SPACEBAR))
        {
            return SPACEBAR;
        }

        if (Input.GetKeyDown(ARROW_LEFT))
        {
            return ARROW_LEFT;
        }

        if (Input.GetKeyDown(ARROW_RIGHT))
        {
            return ARROW_RIGHT;
        }

        if (Input.GetKeyDown(ARROW_DOWN))
        {
            return ARROW_DOWN;
        }

        if (Input.GetKeyDown(KeyCode.RightControl))
        {
            return CTRL;
        }

        if (Input.GetKeyDown(KeyCode.RightShift))
        {
            return SHIFT;
        }

        return Input.inputString;
    }

    public static int GetPlayerIndexByArrowCode(int playersCount, string keyCode)
    {
        //TODO LOGGING

        if (playersCount == 2)
        {
            switch (keyCode)
            {
                case ARROW_LEFT: return 0;
                case ARROW_RIGHT: return 1;
            }
        }
        else if (playersCount == 3)
        {
            switch (keyCode)
            {
                case ARROW_LEFT: return 0;
                case ARROW_DOWN: return 1;
                case ARROW_RIGHT: return 2;
            }
        }

        return 0;
    }
}
