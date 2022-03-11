using System.Runtime.Serialization;
using System;

namespace BF2D
{
    [Serializable]
    public enum Axis
    {
        [EnumMember(Value = "Horizontal")]
        Horizontal, 
        [EnumMember(Value = "Vertical")]
        Vertical
    };

    [Serializable]
    struct IntVector2
    {
        public int x;
        public int y;

        public (int, int) Tuple { get { return (x, y); } }

        public IntVector2(int valueX, int valueY)
        {
            x = valueX;
            y = valueY;
        }

        public static bool operator ==(IntVector2 a, IntVector2 b)
        {
            return (a.x == b.x && a.y == b.y);
        }

        public static bool operator !=(IntVector2 a, IntVector2 b)
        {
            return !(a.x == b.x && a.y == b.y);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    [Serializable]
    public enum InputKey
    {
        [EnumMember(Value = "Up")]
        Up,
        [EnumMember(Value = "Left")]
        Left,
        [EnumMember(Value = "Down")]
        Down,
        [EnumMember(Value = "Right")]
        Right,
        [EnumMember(Value = "Confirm")]
        Confirm,
        [EnumMember(Value = "Back")]
        Back,
        [EnumMember(Value = "Attack")]
        Attack,
        [EnumMember(Value = "Menu")]
        Menu,
        [EnumMember(Value = "Pause")]
        Pause,
        [EnumMember(Value = "Select")]
        Select
    }
}
