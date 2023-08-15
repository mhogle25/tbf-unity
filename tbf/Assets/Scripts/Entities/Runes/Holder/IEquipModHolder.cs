using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BF2D.Game
{
    public interface IEquipModHolder : IUtilityEntityHolder<EquipModInfo>
    {
        /// <summary>
        /// Removes a single rune from the holder. This method will delete the rune's datafile if its count reaches zero and if it is a generated rune. Use when customizing (transforming) or deleting a rune.
        /// </summary>
        /// <param name="info">The eunr info to remove</param>
        public void Destroy(EquipModInfo info);

        /// <summary>
        /// Removes a single rune from the holder. This method will delete the rune's datafile if its count reaches zero and if it is a generated rune. Use when customizing (transforming) or deleting a rune.
        /// </summary>
        /// <param name="id">The rune info id to remove</param>
        public void Destroy(string id);
    }
}
