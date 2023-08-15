using System.Collections.Generic;
using UnityEngine;
using BF2D.Game.Actions;
using BF2D.Enums;
using BF2D.UI;

namespace BF2D.Game.Combat
{
    public class CharacterTargeterData : MonoBehaviour
    {
        public OptionsGrid PlayerPlatforms => this.playerPlatforms;
        public OptionsGrid EnemyPlatforms => this.enemyPlatforms;
        public OptionsGrid AnyPlatforms => this.anyPlatforms;

        [Header("Character Targeter")]
        [SerializeField] private OptionsGrid playerPlatforms = null;
        [SerializeField] private List<CombatGridTile> initPlayerOptions = new();
        [Space(10)]
        [SerializeField] private OptionsGrid enemyPlatforms = null;
        [SerializeField] private List<CombatGridTile> initEnemyOptions = new();
        [Space(10)]
        [SerializeField] private OptionsGrid anyPlatforms = null;
        [SerializeField] private List<CombatGridTile> initAnyOptions = new();

        protected void Awake()
        {
            this.playerPlatforms.LoadOptions(this.initPlayerOptions);
            this.enemyPlatforms.LoadOptions(this.initEnemyOptions);
            this.anyPlatforms.LoadOptions(this.initAnyOptions);
        }
    }
}
