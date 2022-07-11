using System.Drawing;

namespace WibboEmulator.Game.Rooms.PathFinding
{
    public struct Coord : IEquatable<Coord>
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

        public Coord(int x, int y, int z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public static bool operator ==(Coord a, Coord b)
        {
            if (a.X == b.X && a.Y == b.Y)
            {
                return a.Z == b.Z;
            }
            else
            {
                return false;
            }
        }

        public static bool operator !=(Coord a, Coord b)
        {
            return !(a == b);
        }

        public bool Equals(Coord comparedCoord)
        {
            if (this.X == comparedCoord.X && this.Y == comparedCoord.Y)
            {
                return this.Z == comparedCoord.Z;
            }
            else
            {
                return false;
            }
        }

        public bool Equals(Point comparedCoord)
        {
            if (this.X == comparedCoord.X)
            {
                return this.Y == comparedCoord.Y;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return this.X ^ this.Y ^ this.Z;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            else
            {
                return base.GetHashCode().Equals(obj.GetHashCode());
            }
        }
    }
}
