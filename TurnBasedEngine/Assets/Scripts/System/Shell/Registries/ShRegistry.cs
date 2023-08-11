using System.Collections.Generic;
using UnityEngine;

namespace BF2D
{
    public abstract class ShRegistry : MonoBehaviour
    {
        protected Dictionary<string, ShellCommand> commands;

        public ShellCommand this[string key] => this.commands[key];

        public bool ContainsKey(string key) => this.commands.ContainsKey(key);
    }
}