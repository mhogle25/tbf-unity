using System.Runtime.Serialization;
using System;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace BF2D.Game.Enums
{
    [Serializable]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CombatAlignment
    {
        [EnumMember]
        Offense,
        [EnumMember]
        Defense,
        [EnumMember]
        Neutral
    }

    public class CombatAlignmentCollection<T>
    {
        private T[] collection;
        private readonly int enumSize = 0;

        public CombatAlignmentCollection()
        {
            enumSize = Enum.GetValues(typeof(CombatAlignment)).Length;
            collection = new T[enumSize];
        }

        public T this[CombatAlignment index]
        {
            get
            {
                return this.collection[(int)index];
            }

            set
            {
                this.collection[(int)index] = value;
            }
        }
    }

    public class CombatAlignmentSelector : MonoBehaviour
    {
        public CombatAlignment combatAlignment;

        public static CombatAlignment Calculate(int offense, int defense, int neutral)
        {
            if (offense > defense && offense > neutral)
                return CombatAlignment.Offense;

            if (defense > offense && defense > neutral)
                return CombatAlignment.Defense;

            return CombatAlignment.Neutral;
        }

        public static CombatAlignment CalculateCombatAlignedCollection(IEnumerable<ICombatAligned> entities)
        {
            int offense = 0;
            int defense = 0;
            int neutral = 0;

            foreach (ICombatAligned entity in entities)
            {
                switch (entity.Alignment)
                {
                    case CombatAlignment.Offense: offense++; break;
                    case CombatAlignment.Defense: defense++; break;
                    case CombatAlignment.Neutral: neutral++; break;
                }
            }

            return Calculate(offense, defense, neutral);
        }
    }
}