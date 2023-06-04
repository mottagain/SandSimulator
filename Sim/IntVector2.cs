
using Microsoft.Xna.Framework;
using System;

namespace SandSimulator.Sim
{
	public struct IntVector2 : IEquatable<IntVector2>, IComparable<IntVector2>
	{
		public int X { get; set; }

		public int Y { get; set; }

		public static IntVector2 FromVector2(Vector2 vector)
		{
			return new IntVector2 { X = (int)Math.Round(vector.X), Y = (int)Math.Round(vector.Y) };
		}

		public int CompareTo(IntVector2 other)
		{
			int result = this.Y.CompareTo(other.Y);
			if (result != 0) return result;

			return this.X.CompareTo(other.X);
		}

		public override bool Equals(object other)
		{
			if (other == null || !(other is IntVector2)) return false;

			return Equals((IntVector2)other);
		}

		public override int GetHashCode()
		{
			return this.X.GetHashCode() ^ this.Y.GetHashCode();
		}

		public bool Equals(IntVector2 other)
		{
			return this.X == other.X && this.Y == other.Y;
		}

		public static bool operator ==(IntVector2 left, IntVector2 right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(IntVector2 left, IntVector2 right)
		{
			return !left.Equals(right);
		}

		public static IntVector2 operator -(IntVector2 lhs, IntVector2 rhs)
		{
			return new IntVector2 { X = lhs.X - rhs.X, Y = lhs.Y - rhs.Y };
		}

		public static IntVector2 operator +(IntVector2 lhs, IntVector2 rhs)
		{
			return new IntVector2 { X = lhs.X + rhs.X, Y = lhs.Y + rhs.Y };
		}

		public Vector2 ConvertToVector2()
		{
			return new Vector2(this.X, this.Y);
		}

		public static bool IsOppositeDirection(IntVector2 lhs, IntVector2 rhs)
		{
			var dotProduct = lhs.X * rhs.X + lhs.Y * rhs.Y;
			return dotProduct < 0;
		}
	}
}