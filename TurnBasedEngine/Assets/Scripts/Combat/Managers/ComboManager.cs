using UnityEngine;
using System;
using System.Collections.Generic;
using BF2D;
using BF2D.Enums;

namespace BF2D.Combat
{
    public class ComboManager : MonoBehaviour
    {
        /*
        [Serializable]
        private class Combo
        {
            [Serializable]
            private class Keystroke
            {
                private List<InputButton> inputKeys = new List<InputButton>();
                public float Timer { get { return this.timer; } }
                private float timer = 0f;

                public bool Check()
                {
                    bool value = true;
                    foreach (InputButton key in this.inputKeys)
                    {
                        value = value && InputManager.GetButtonPress(key);
                    }
                    return value;
                }
            }

            public string Name { get { return this.name; } }
            private string name = string.Empty;

            public string actionType;
            public object action;

            private List<Keystroke> keystrokes = new List<Keystroke>();
            private int keystrokeIndex = 0;
            private int frameIndex = 0;

            public string RunKeystroke()
            {
                Keystroke current = this.keystrokes[this.keystrokeIndex];
                if (current.Timer - 1 < this.frameIndex)
                {
                    this.frameIndex = 0;
                    return null;
                }

                if (this.keystrokeIndex > this.keystrokes.Count - 1)
                {
                    string a = this.action.ToString();
                    return a;
                }

                if (current.Check())
                {
                    ++this.keystrokeIndex;
                }

                this.frameIndex++;
                return null;
            }
        }

        private List<Combo> combos = new List<Combo>();
        private Dictionary<string, Func<string, int>> comboActions = new Dictionary<string, Func<string, int>>
    {
        {
            "foo",
            (x) =>
            {
                Debug.Log("foo" + x);
                return 0;
            }
        },
        {
            "bar",
            (x) =>
            {
                Debug.Log("bar" + x);
                return 1;
            }
        }
    };

        public void LoadCombos(string json)
        {
            this.combos.Clear();
            try
            {
                this.combos = JsonUtility.FromJson<List<Combo>>(json);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                return;
            }
        }

        public void Start()
        {
            int x = this.comboActions["foo"]("test");
            Debug.Log(x);
        }
    */
    }
}
