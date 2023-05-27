using System.Collections.Generic;

namespace BF2D.Game
{
    public interface IEquipmentHolder : IEnumerable<EquipmentInfo>
    {
        public EquipmentInfo AcquireEquipment(string id);

        public EquipmentInfo RemoveEquipment(EquipmentInfo info);
    }
}