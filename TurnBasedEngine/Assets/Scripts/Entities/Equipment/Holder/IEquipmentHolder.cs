using System.Collections.Generic;

namespace BF2D.Game
{
    public interface IEquipmentHolder : IEnumerable<EquipmentInfo>
    {
        public EquipmentInfo AcquireEquipment(string id);

        /// <summary>
        /// Removes a single equipment from the holder. This method will delete the equipment's datafile if its count reaches zero and if it is a generated equipment. Use when customizing (transforming) or deleting an equipment.
        /// </summary>
        /// <param name="info">The equipment info to remove</param>
        public void RemoveEquipment(EquipmentInfo info);

        /// <summary>
        /// Moves an equipment from one holder to another
        /// </summary>
        /// <param name="info">The equipment to transfer</param>
        /// <param name="receiver">The receiving holder</param>
        /// <returns>The received equipment info</returns>
        public EquipmentInfo TransferEquipment(EquipmentInfo info, IEquipmentHolder receiver);

        /// <summary>
        /// Removes an equipment from the holder and returns its ID
        /// </summary>
        /// <param name="info">The equipment to extract</param>
        /// <returns>The id of the equipment</returns>
        public string ExtractEquipment(EquipmentInfo info);

        public EquipmentInfo GetEquipment(string id);
    }
}