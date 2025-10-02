using System;

namespace ITCafe
{
    public struct InputEntry : IEquatable<InputEntry>
    {
        public readonly Action Sub;
        public readonly Action Unsub;
        public readonly int Order;

        public InputEntry(Action sub, Action unsub, int order = int.MaxValue)
        {
            Sub = sub;
            Unsub = unsub;
            Order = order;
        }

        public bool Equals(InputEntry other)
        {
            return Equals(Sub, other.Sub) && Equals(Unsub, other.Unsub) && Order == other.Order;
        }

        public override bool Equals(object obj)
        {
            return obj is InputEntry other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Sub, Unsub, Order);
        }
    }
}