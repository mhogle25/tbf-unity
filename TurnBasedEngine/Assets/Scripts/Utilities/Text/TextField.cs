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
                if (ValidText(value, out invalidCharacters))
                {
                    base.text = value;
                    return;
                }

                string newText = value;
                newText = newText.Trim(invalidCharacters.ToArray());
                base.text = newText;
            }
        }

        private bool ValidText(string text, out List<char> invalidCharacters)
        {
            return this.font.HasCharacters(text, out invalidCharacters);
        }
    }
}