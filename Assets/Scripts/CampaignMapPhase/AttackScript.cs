﻿using UnityEngine;
using UnityEngine.UI;

public class AttackScript : MonoBehaviour
{
    public Button endTurnButton;

    void Start()
    {
        endTurnButton.onClick.AddListener(ClickBoton);
    }

    private void ClickBoton()
    {
        ConnectionManager.instance.CmdPlayerAttacked();
    }
}
