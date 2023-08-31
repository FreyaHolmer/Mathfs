// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using System;
using UnityEngine;

namespace Freya {

	/// <summary>A struct representing exact rational numbers</summary>
	[Serializable] public struct Rational : IComparable<Rational> {

		public static readonly Rational Zero = new(0, 1);
		public static readonly Rational One = new(1, 1);
		public static readonly Rational MaxValue = new(int.MaxValue, 1);
		public static readonly Rational MinValue = new(int.MinValue, 1);

		/// <summary>The numerator of this number</summary>
		[SerializeField] public int n;

		/// <summary>The denominator of this number</summary>
		[SerializeField] [NonZeroInteger] public int d;

		/// <summary>Creates an exact representation of a rational number</summary>
		/// <param name="num">The numerator of this number</param>
		/// <param name="den">The denominator of this number</param>
		public Rational( int num, int den ) {
			switch( den ) {
				case -1:
					( n, d ) = ( -num, -den );
					break;
				case 0: throw new DivideByZeroException( "The denominator can't be 0" );
				case 1:
					( n, d ) = ( num, den );
					break;
				default:
					if( num == 0 ) {
						( n, d ) = ( 0, 1 );
						break;
					}

					// ensure only the numerator carries the sign
					int sign = Mathfs.Sign( den );
					n = sign * num;
					d = sign * den;

					if( n is -1 or 1 )
						break; // no reduction needed

					// in this case, we have to try simplifying the expression
					int gcd = Mathfs.Gcd( num, den );
					n /= gcd;
					d /= gcd;
					break;
			}
		}

		/// <summary>Returns the reciprocal of this number</summary>
		public Rational Reciprocal => new(d, n);

		public bool IsInteger => d == 1;

		/// <summary>Returns the absolute value of this number</summary>
		public Rational Abs() => new(n.Abs(), d);

		/// <summary>Returns this number to the power of another integer <c>pow</c></summary>
		/// <param name="pow">The power to raise this number by</param>
		public Rational Pow( int pow ) =>
			pow switch {
				<= -2 => Reciprocal.Pow( -pow ),
				-1    => Reciprocal,
				0     => 1,
				1     => this,
				>= 2  => new Rational( n.Pow( pow ), d.Pow( pow ) )
			};

		public override string ToString() => d == 1 ? n.ToString() : $"{n}/{d}";

		// statics
		public static Rational Min( Rational a, Rational b ) => a < b ? a : b;
		public static Rational Max( Rational a, Rational b ) => a > b ? a : b;
		public static Rational Lerp( Rational a, Rational b, Rational t ) => a + t * ( b - a );
		public static Rational InverseLerp( Rational a, Rational b, Rational v ) => ( v - a ) / ( b - a );

		// type casting
		public static implicit operator Rational( int n ) => new(n, 1);
		public static explicit operator int( Rational r ) => r.IsInteger ? r.n : throw new ArithmeticException( $"Rational value {r} can't be cast to an integer" );
		public static explicit operator float( Rational r ) => (float)r.n / r.d;
		public static explicit operator double( Rational r ) => (double)r.n / r.d;

		// unary operations
		public static Rational operator -( Rational r ) => checked( new(-r.n, r.d) );
		public static Rational operator +( Rational r ) => r;

		// addition
		public static Rational operator +( Rational a, Rational b ) => checked( new(a.n * b.d + a.d * b.n, a.d * b.d) );
		public static Rational operator +( Rational a, int b ) => checked( new(a.n + a.d * b, a.d) );
		public static Rational operator +( int a, Rational b ) => checked( new(a * b.d + b.n, b.d) );

		// subtraction
		public static Rational operator -( Rational a, Rational b ) => checked( new(a.n * b.d - a.d * b.n, a.d * b.d) );
		public static Rational operator -( Rational a, int b ) => checked( new(a.n - a.d * b, a.d) );
		public static Rational operator -( int a, Rational b ) => checked( new(a * b.d - b.n, b.d) );

		// multiplication
		public static Rational operator *( Rational a, Rational b ) => checked( new(a.n * b.n, a.d * b.d) );
		public static Rational operator *( Rational a, int b ) => checked( new(a.n * b, a.d) );
		public static Rational operator *( int a, Rational b ) => checked( new(b.n * a, b.d) );
		public static float operator *( Rational a, float b ) => ( a.n * b ) / a.d;
		public static float operator *( float a, Rational b ) => ( b.n * a ) / b.d;

		// division
		public static Rational operator /( Rational a, Rational b ) => checked( new(a.n * b.d, a.d * b.n) );
		public static Rational operator /( Rational a, int b ) => checked( new(a.n, a.d * b) );
		public static Rational operator /( int a, Rational b ) => checked( new(a * b.d, b.n) );
		public static float operator /( Rational a, float b ) => a.n / ( a.d * b );
		public static float operator /( float a, Rational b ) => ( a * b.d ) / b.n;

		// comparison operators
		public static bool operator ==( Rational a, Rational b ) => a.CompareTo( b ) == 0;
		public static bool operator !=( Rational a, Rational b ) => a.CompareTo( b ) != 0;
		public static bool operator <( Rational a, Rational b ) => a.CompareTo( b ) < 0;
		public static bool operator >( Rational a, Rational b ) => a.CompareTo( b ) > 0;
		public static bool operator <=( Rational a, Rational b ) => a.CompareTo( b ) <= 0;
		public static bool operator >=( Rational a, Rational b ) => a.CompareTo( b ) >= 0;

		// comparison functions
		public int CompareTo( Rational other ) => checked( ( n * other.d ).CompareTo( d * other.n ) );
		public bool Equals( Rational other ) => n == other.n && d == other.d;
		public override bool Equals( object obj ) => obj is Rational other && Equals( other );
		public override int GetHashCode() => HashCode.Combine( n, d );

	}

}