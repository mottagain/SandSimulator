
using System;

namespace SandSimulator.Voxel
{
	internal class Position : IEquatable<Position>, IComparable<Position>
	{
		public int X { get; set; }

		public int Y { get; set; }

		public int CompareTo(Position other)
		{
			int result = this.Y.CompareTo(other.Y);
			if (result != 0) return result;

			return this.X.CompareTo(other.X);
		}

		public bool Equals(Position other)
		{
			return this.X == other.X && this.Y == other.Y;
		}
	}
}
