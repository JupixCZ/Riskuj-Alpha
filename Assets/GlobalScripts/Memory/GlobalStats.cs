using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalStats
{

    private static List<Player> players;

    public static void initPlayers(string name1, string name2, string name3)
    {
        players = new List<Player>();

        addNewPlayer(name1);
        addNewPlayer(name2);

        if (name3 != null)
        {
            addNewPlayer(name3);
        }
    }

    private static void addNewPlayer(string name)
    {
        players.Add(new Player(name));
    }

    public static Player getPlayer(int num) {
        if (num > players.Count) {
            return null;
        }

        return players[num - 1];
    }
}
