using UnityEngine;
using System;
namespace TMPro
{
    /// <summary>
    /// EXample of a Custom Character Input Validator to only allow digits from 0 to 9.
    /// </summary>
    [Serializable]
    [CreateAssetMenu(fileName = "Shell Validator", menuName = "TextMeshPro/Input Validators/Shell", order = 100)]
    public class TMP_ShellValidator : TMP_InputValidator
    {
        [SerializeField] private TMP_FontAsset font;

        // Custom text input validation function
        public override char Validate(ref string text, ref int pos, char ch)
        {
            if (this.font.HasCharacter(ch))
            {
                text = text.Insert(pos, ch.ToString());
                pos += 1;
                return ch;
            }
            return (char)0;
        }
    }
}