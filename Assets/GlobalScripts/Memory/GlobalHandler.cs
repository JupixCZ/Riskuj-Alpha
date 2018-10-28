using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalHandler
{

    private static List<Player> players;

    public static void initPlayers(string name1, string name2, string name3)
    {
        players = new List<Player>();

        AddNewPlayer(name1);
        AddNewPlayer(name2);

        if (name3 != null)
        {
            AddNewPlayer(name3);
        }
    }

    private static void AddNewPlayer(string name)
    {
        players.Add(new Player(name));
    }

    public static Player getPlayer(int num) {
        if (num > players.Count) {
            return null;
        }

        return players[num - 1];
    }

    public static void ActivatePlayerByArrowCode(string keyCode) {
        Player activePlayer = GetPlayerByArrowCode(keyCode);
        foreach (Player player in players) {
            player.SetActive(player.Equals(activePlayer));
        }
    }

    private static Player GetPlayerByArrowCode(string keyCode) {
        int playersCount = players.Count;

        int playerIndex = KeyUtil.GetPlayerIndexByArrowCode(playersCount, keyCode);
        return players[playerIndex];
    }
}
