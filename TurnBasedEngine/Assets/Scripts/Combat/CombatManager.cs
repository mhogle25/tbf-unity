using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using BF2D.UI;
using BF2D.Game;
using DataStructures.PriorityQueue;

namespace BF2D.Combat
{
    public class CombatManager : SceneManager
    {
        public ComboManager ComboManagerComponent { get { return this.comboManager; } }
        [SerializeField] private ComboManager comboManager = null;
        //public StatsPreviewController StatsPreviewControllerComponent { get { return this.statsPreviewController; } }
        [SerializeField] private StatsPreviewController statsPreviewController = null;
        //public DialogTextboxControl DialogTextboxControl { get { return this.dialogTextboxControl; } }

        public PlayerStats CurrentPlayer { get { return this.currentPlayer.GetPlayerStats; } }
        private PlayerCombat currentPlayer = null;

        public List<Stats> Characters
        {
            get
            {
                List<Stats> statsList = new List<Stats>();

                foreach (PlayerCombat playerCombat in this.players)
                {
                    statsList.Add(playerCombat.GetStats);
                }

                foreach (EnemyCombat enemyCombat in this.enemies)
                {
                    statsList.Add(enemyCombat.GetStats);
                }

                return statsList;
            }
        }

        public List<PlayerStats> Players 
        { 
            get 
            {
                List<PlayerStats> statsList = new List<PlayerStats>();
                foreach (PlayerCombat playerCombat in this.players)
                {
                    statsList.Add(playerCombat.GetPlayerStats);
                }
                return statsList;
            } 
        }
        private List<PlayerCombat> players = new List<PlayerCombat>();

        public List<EnemyStats> Enemies
        {
            get
            {
                List<EnemyStats> statsList = new List<EnemyStats>();
                foreach (EnemyCombat enemyCombat in this.enemies)
                {
                    statsList.Add(enemyCombat.GetEnemyStats);
                }
                return statsList;
            }
        }
        private List<EnemyCombat> enemies = new List<EnemyCombat>();

        private PriorityQueue<CharacterCombat, uint> turnQueue = new PriorityQueue<CharacterCombat, uint>(0);
        private Stack<CharacterCombat> characterCombatTempStorage = new Stack<CharacterCombat>();

        /*
        public CombatState State { 
            set 
            {
                this.state?.End();
                this.state = value;
                this.state.Start();
            } 
        }

        private CombatState state = null;
        private CSPlayerSelection statePlayerSelection = null;

        public CombatManager()
        {
            this.statePlayerSelection = new CSPlayerSelection(this);
        }
        */

        private void Update()
        {
            //this.state?.Update();
        }

        private void InitializeCombat()
        {
            //this.state = this.statePlayerSelection;
        }
    }
}
