﻿using UnityEngine;
using UnityEngine.UI;

public class EndTurnInMapButtonScript : MonoBehaviour
{
    public Button endTurnButton;

    void Start()
    {
        endTurnButton.onClick.AddListener(ClickBoton);
    }

    private void ClickBoton()
    {
        ConnectionManager.instance.CmdEndTurn(GameManager.instance.playerBattleSide);
    }
}