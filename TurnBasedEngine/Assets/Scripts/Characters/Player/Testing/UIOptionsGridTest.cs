﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BF2D.UI;
using BF2D;

public class UIOptionsGridTest : MonoBehaviour
{
    [SerializeField] private UIOptionsGrid optionsGrid;
    [SerializeField] private Sprite iconSprite;
    [SerializeField] private Sprite cursorSprite;

    private void Start()
    {
        /*
        optionsGrid.Setup(10, 4);
        UIOptionData optionData = new UIOptionData
        {
            icon = iconSprite,
            cursor = cursorSprite
        };


        for (int i = 0; i < 20; i++)
        {
            optionsGrid.Add(optionData);
        }
        */
    }

    private void Update()
    {
        if (InputManager.AttackPress)
        {
            this.optionsGrid.Remove();
        }
    }

    public void HellYeah()
    {
        Debug.Log("hell fuckin yeah bruther");
    }
}