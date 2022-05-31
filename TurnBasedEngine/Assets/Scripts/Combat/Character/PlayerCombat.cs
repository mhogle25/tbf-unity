using UnityEngine;
using BF2D.Game;

namespace BF2D.Combat
{

    public class PlayerCombat : CharacterCombat
    {
        public override Stats GetStats { get { return this.playerStats; } }
        public PlayerStats GetPlayerStats { get { return this.playerStats; } }
        private PlayerStats playerStats = null;
    }
}
