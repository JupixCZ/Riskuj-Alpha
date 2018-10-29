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

    public static Player getPlayer(int num)
    {
        if (num > players.Count)
        {
            return null;
        }

        return players[num - 1];
    }

    public static Player ActivatePlayerByArrowCode(string keyCode, bool notAnsweredOnly)
    {
        Player activePlayer = GetPlayerByArrowCode(keyCode, notAnsweredOnly);
        if (activePlayer == null)
        {
            return null;
        }

        foreach (Player player in players)
        {
            player.SetActive(player.Equals(activePlayer));
        }

        return activePlayer;
    }

    public static void DeactivatePlayers()
    {
        foreach (Player player in players)
        {
            player.SetActive(false);
        }
    }

    public static void ActivePlayerClaimPrize(Question question)
    {
        Player activePlayer = GetActivePlayer();

        activePlayer.AddToBalance(question.GetPrize());
    }

    public static void ActivePlayerFailed(Question question)
    {
        Player activePlayer = GetActivePlayer();

        activePlayer.AnsweredWrong(question.GetPrize());
    }

    private static Player GetActivePlayer()
    {
        Player activePlayer = null;
        foreach (Player player in players)
        {
            if (player.IsActive())
            {
                activePlayer = player;
                break;
            }
        }

        if (activePlayer == null)
        {
            //TODO LOGGER
            return null;
        }

        return activePlayer;
    }

    public static void RefreshPlayers()
    {
        foreach (Player player in players)
        {
            player.RefreshAnswering();
        }
    }

    public static bool AnyAnsweringPlayerLeft()
    {
        foreach (Player player in players)
        {
            if (!player.IsWrongAnswer())
            {
                return true;
            }
        }

        return false;
    }

    private static Player GetPlayerByArrowCode(string keyCode, bool notAnsweredOnly)
    {
        int playersCount = players.Count;

        int playerIndex = KeyUtil.GetPlayerIndexByArrowCode(playersCount, keyCode);
        Player playerByArrowCode = players[playerIndex];

        if (notAnsweredOnly && playerByArrowCode.IsWrongAnswer())
        {
            return null;
        }

        return playerByArrowCode;
    }
}
