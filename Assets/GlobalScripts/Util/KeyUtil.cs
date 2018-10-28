using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class KeyUtil
{
    private static string[] numKeyCodes = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9"};

    public static bool IsNumber(string keyCode) {
        return System.Array.IndexOf(numKeyCodes, keyCode) != -1;
    }
}
