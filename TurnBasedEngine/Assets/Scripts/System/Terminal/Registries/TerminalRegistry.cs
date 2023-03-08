using System.Collections.Generic;
using UnityEngine;

namespace BF2D
{
    public abstract class TerminalRegistry : MonoBehaviour
    {
        protected Dictionary<string, TerminalCommand> commands;

        public TerminalCommand this[string key]
        {
            get
            {
                return this.commands[key];
            }
        }

        public bool ContainsKey(string key)
        {
            return this.commands.ContainsKey(key);
        }
    }
}