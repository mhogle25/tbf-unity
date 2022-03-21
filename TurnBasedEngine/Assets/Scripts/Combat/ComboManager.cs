using UnityEngine;
using System;
using System.Collections.Generic;
using BF2D;
using System.Reflection;

public struct ComboAction
{

}

public class ComboManager : MonoBehaviour
{
    [Serializable]
    private class Combo
    {
        [Serializable]
        private class Keystroke
        {
            private List<InputKey> inputKeys = new List<InputKey>();
            public float Timer { get { return this.timer; } }
            private float timer = 0f;

            public bool Check()
            {
                bool value = true;
                foreach (InputKey key in this.inputKeys)
                {
                    value = value && InputManager.KeyPress(key);
                }
                return value;
            }
        }

        public string Name { get { return this.name; } }
        private string name = string.Empty;

        private object action = null;
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
                string a = action.ToString();
                return a;
            }

            if (current.Check())
            {
                ++this.keystrokeIndex;
            }

            frameIndex++;
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
                Debug.Log("foo");
                return 1;
            }
        },
        {
            "bar",
            (x) =>
            {
                Debug.Log("bar");
                return 1;
            }
        }
    };

    public void LoadCombos(string json)
    {
        combos.Clear();
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

    public void Update()
    {
        MethodInfo mi = this.GetType().GetMethod("");
    }

    private void Foo()
}