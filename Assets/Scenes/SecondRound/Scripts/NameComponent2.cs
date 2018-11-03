using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NameComponent2 : MonoBehaviour {

    public Text playerNameText;
    private Player player;
    private bool currentStateActive;
    private Sprite imgActive;
    private Sprite imgInactive;

    void Start()
    {
        currentStateActive = false;
        imgActive = Resources.Load<Sprite>("Image/selectedQuestion");
        imgInactive = Resources.Load<Sprite>("Image/topic");
    }

    void Update()
    {
        if (player == null)
        {
            return;
        }

        UpdateActivity();
    }

    public void SetPlayer(Player player)
    {
        this.player = player;
        this.playerNameText.text = player.GetName();
    }

    private void UpdateActivity()
    {
        bool active = player.IsActive();
        if (active && !currentStateActive)
        {
            GetComponent<Image>().sprite = imgActive;
            currentStateActive = true;
        }
        else if (!active && currentStateActive)
        {
            GetComponent<Image>().sprite = imgInactive;
            currentStateActive = false;
        }
    }
}
