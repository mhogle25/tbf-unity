using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BF2D.UI;
using BF2D;

public class UIOptionsGridTest : MonoBehaviour
{
    [SerializeField] private OptionsGrid optionsGrid;
    [SerializeField] private Sprite iconSprite;
    [SerializeField] private Sprite cursorSprite;

    private void Start()
    {
        /*
        controlledOptionsGrid.Setup(10, 4);
        UIOptionData optionData = new UIOptionData
        {
            iconID = iconSprite,
            cursor = cursorSprite
        };


        for (int i = 0; i < 20; i++)
        {
            controlledOptionsGrid.Add(optionData);
        }
        */
    }

    public void HellYeah()
    {
        Terminal.IO.Log("hell fuckin yeah bruther");
    }
}
