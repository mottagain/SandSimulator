
using System;
using System.Reflection.Metadata;

namespace SandSimulator.Voxel
{
	internal class IntVector2 : IEquatable<IntVector2>, IComparable<IntVector2>
	{
		public int X { get; set; }

		public int Y { get; set; }

		public int CompareTo(IntVector2 other)
		{
			int result = this.Y.CompareTo(other.Y);
			if (result != 0) return result;

			return this.X.CompareTo(other.X);
		}

		public bool Equals(IntVector2 other)
		{
			return this.X == other.X && this.Y == other.Y;
		}

		public static IntVector2 operator -(IntVector2 lhs, IntVector2 rhs)
		{
			return new IntVector2 { X = lhs.X - rhs.X, Y = lhs.Y - rhs.Y };
		}

		public static IntVector2 operator +(IntVector2 lhs, IntVector2 rhs)
		{
			return new IntVector2 { X = lhs.X + rhs.X, Y = lhs.Y + rhs.Y };
		}

		public static bool IsOppositeDirection(IntVector2 lhs, IntVector2 rhs)
		{
			var dotProduct = lhs.X * rhs.X + lhs.Y * rhs.Y;
			return dotProduct < 0;
		}
	}
}