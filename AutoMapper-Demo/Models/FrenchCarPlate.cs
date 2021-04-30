using System;

namespace AutoMapperDemo.Models
{
    public readonly struct FrenchCarPlate : IEquatable<FrenchCarPlate>
    {
        private readonly string _plate;

        public FrenchCarPlate(string plate)
        {
            _plate = plate ?? throw new ArgumentNullException(nameof(plate));

            string[] split = plate.Split("-");

            if (split.Length != 3)
            {
                throw new ArgumentException($"Plate {plate} is not valid.", nameof(plate));
            }

            First = split[0];
            Middle = split[1];
            Last = split[2];
        }

        public string First { get; }

        public string Middle { get; }

        public string Last { get; }

        /// <inheritdoc />
        public bool Equals(FrenchCarPlate other)
        {
            return First == other.First && Middle == other.Middle && Last == other.Last;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return obj is FrenchCarPlate other && Equals(other);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(First, Middle, Last);
        }

        public static bool operator ==(FrenchCarPlate left, FrenchCarPlate right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(FrenchCarPlate left, FrenchCarPlate right)
        {
            return !left.Equals(right);
        }

        /// <inheritdoc />
        public override string ToString() => _plate;
    }
}