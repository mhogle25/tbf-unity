using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using BF2D.Game;
using System;

namespace BF2D.Utilities
{
    public class TextField : TextMeshProUGUI
    {
        public override string text
        {
            get
            {
                return base.text;
            }
            set
            {
                List<char> invalidCharacters;
                if (GameInfo.Instance.ValidText(value, out invalidCharacters))
                {
                    base.text = value;
                    return;
                }

                string newText = value;
                newText = newText.Trim(invalidCharacters.ToArray());
                base.text = newText;
            }
        }
    }
}