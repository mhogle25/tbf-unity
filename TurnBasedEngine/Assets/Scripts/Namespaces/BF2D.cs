namespace BF2D
{
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
}
