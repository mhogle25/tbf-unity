using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BF2D.Game;

namespace BF2D.Combat
{
    public abstract class CharacterCombat : MonoBehaviour
    {
        public abstract Stats GetStats { get; }
    }
}
