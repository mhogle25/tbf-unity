using System.Collections.Generic;
using UnityEngine;

namespace BF2D
{
    public abstract class TerminalRegistry : MonoBehaviour
    {
        protected Dictionary<string, TerminalCommand> commands;

        public TerminalCommand this[string key] => this.commands[key];

        public bool ContainsKey(string key) => this.commands.ContainsKey(key);
    }
}