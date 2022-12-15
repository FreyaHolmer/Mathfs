// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using System;

namespace Freya {

	/// <summary>A struct representing a probability (as a rational number)</summary>
	[Serializable] public struct Probability : IComparable<Probability> {

		public static readonly Rational Zero = new(0, 1);
		public static readonly Rational One = new(1, 1);

		/// <summary>The value of this probability</summary>
		public Rational value;

		/// <summary>Creates a representation of probability using a rational number</summary>
		/// /// <param name="value">The probability value</param>
		public Probability( Rational value ) => this.value = value;

		/// <summary>Creates a representation of probability using a rational number</summary>
		/// <param name="num">The numerator of this probability</param>
		/// <param name="den">The denominator of this probability</param>
		public Probability( int num, int den ) : this( new Rational( num, den ) ) {
		}

		/// <summary>Randomly samples this probability, returning either true or false</summary>
		public bool Sample => Random.Range( 0, value.d ) < value.n;

		public override string ToString() => $"{value} ({(float)value * 100:#.#####}%)";

		// statics
		public static Probability operator &( Probability a, Probability b ) => new(a.value * b.value);
		public static Probability operator |( Probability a, Probability b ) => !( !a & !b );
		public static Probability operator +( Probability a, Probability b ) => new(a.value + b.value);
		public static Probability operator !( Probability p ) => new(1 - p.value);

		// comparison operators
		public static bool operator ==( Probability a, Probability b ) => a.value == b.value;
		public static bool operator !=( Probability a, Probability b ) => a.value != b.value;
		public static bool operator <( Probability a, Probability b ) => a.value < b.value;
		public static bool operator >( Probability a, Probability b ) => a.value > b.value;
		public static bool operator <=( Probability a, Probability b ) => a.value <= b.value;
		public static bool operator >=( Probability a, Probability b ) => a.value >= b.value;

		// comparison functions
		public int CompareTo( Probability other ) => value.CompareTo( other.value );
		public bool Equals( Probability other ) => value.Equals( other.value );
		public override bool Equals( object obj ) => obj is Probability other && Equals( other );
		public override int GetHashCode() => value.GetHashCode();

	}

}