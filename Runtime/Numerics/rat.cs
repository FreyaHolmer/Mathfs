// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using System;
using Unity.Mathematics;
using UnityEngine;

namespace Freya {

	/// <summary>A data type representing a rational number n/d, using a pair of integers. </summary>
	[Serializable] public struct rat :
		IComparable<rat>,
		IEquatable<rat>,
		INumber<rat>,
		ISignedNumber<int>,
		IRoundable<int> {
		/// <summary>The numerator of this number. Note: Directly modifying this value will not reduce the fraction</summary>
		[SerializeField] public int n;

		/// <summary>The denominator of this number. Note: Directly modifying this value will not reduce the fraction</summary>
		[SerializeField] [NonZeroInteger] public int d;

		public static readonly rat zero = new(0, 1);
		public static readonly rat one = new(1, 1);
		public static readonly rat half = new(1, 2);
		public static readonly rat MaxValue = new(int.MaxValue, 1);
		public static readonly rat MinValue = new(int.MinValue, 1);

		/// <summary>Creates an exact representation of a rational number</summary>
		/// <param name="num">The numerator of this number</param>
		/// <param name="den">The denominator of this number</param>
		public rat( int num, int den ) {
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

		public static rat FromFloat( float v, int snapStepsPerUnit = 2 ) => new(Mathf.RoundToInt( v * snapStepsPerUnit ), snapStepsPerUnit);

		/// <summary>Returns the reciprocal of this number</summary>
		public rat Reciprocal => new(d, n);

		public bool isInteger => d == 1;
		public bool isZero => n == 0;
		public bool isOrthogonal => true;


		/// <summary>Returns this number to the power of another integer <c>pow</c></summary>
		/// <param name="pow">The power to raise this number by</param>
		public rat Pow( int pow ) =>
			pow switch {
				<= -2 => Reciprocal.Pow( -pow ),
				-1    => Reciprocal,
				0     => 1,
				1     => this,
				>= 2  => new rat( n.Pow( pow ), d.Pow( pow ) )
			};

		public bool TryCastToIntHalf( out inth ih ) {
			switch( d ) {
				case 1:
					ih = n;
					return true;
				case 2:
					ih = new inth { h = n };
					return true;
				default:
					ih = default;
					return false;
			}
		}

		public override string ToString() => d == 1 ? n.ToString() : $"{n}/{d}";

		// statics
		public rat abs => new(n.Abs(), d);
		public rat max( rat other ) => this > other ? this : other;
		public rat min( rat other ) => this < other ? this : other;
		public rat to( rat target ) => target - this;
		public int sign => MathF.Sign( n );
		public int round( RoundingDirection rounding = RoundingDirection.ToEven ) => ( n < 0 == d < 0 ? ( n + d / 2 ) / d : ( n - d / 2 ) / d ); // todo: work out which rounding method this is
		public int floorToward0 => n < 0 ? ceil : floor;
		public int ceilAwayFrom0 => n < 0 ? floor : ceil;
		public int floor => ( n < 0 ? n - d + 1 : n ) / d;
		public int ceil => ( n > 0 ? n + d - 1 : n ) / d;

		// type casting
		public static implicit operator rat( int n ) => new(n, 1);
		public static explicit operator int( rat r ) => r.isInteger ? r.n : throw new ArithmeticException( $"Rational value {r} can't be cast to an integer" );

		public static explicit operator inth( rat r ) =>
			r.d switch {
				1 => r.n,
				2 => new inth { h = r.n * 2 },
				_ => throw new ArithmeticException( $"Rational value {r} can't be cast to a half-step integer" )
			};

		public static explicit operator float( rat r ) => (float)r.n / r.d;
		public static explicit operator double( rat r ) => (double)r.n / r.d;

		// unary operations
		public static rat operator -( rat r ) => checked( new(-r.n, r.d) );
		public static rat operator +( rat r ) => r;

		// binary operations
		public static rat operator +( rat a, rat b ) => checked( new(a.n * b.d + a.d * b.n, a.d * b.d) );
		public static rat operator -( rat a, rat b ) => checked( new(a.n * b.d - a.d * b.n, a.d * b.d) );
		public static rat operator *( rat a, rat b ) => checked( new(a.n * b.n, a.d * b.d) );
		public static rat operator /( rat a, rat b ) => checked( new(a.n * b.d, a.d * b.n) );

		// additional addition operations
		public static rat operator +( rat a, int b ) => checked( new(a.n + a.d * b, a.d) );
		public static rat operator +( int a, rat b ) => checked( new(a * b.d + b.n, b.d) );

		// additional subtraction operations
		public static rat operator -( rat a, int b ) => checked( new(a.n - a.d * b, a.d) );
		public static rat operator -( int a, rat b ) => checked( new(a * b.d - b.n, b.d) );

		// additional multiplication operations
		public static rat operator *( rat a, int b ) => checked( new(a.n * b, a.d) );
		public static rat operator *( int a, rat b ) => checked( new(b.n * a, b.d) );
		public static float operator *( rat a, float b ) => ( a.n * b ) / a.d;
		public static float operator *( float a, rat b ) => ( b.n * a ) / b.d;
		public static double operator *( rat a, double b ) => ( a.n * b ) / a.d;
		public static double operator *( double a, rat b ) => ( b.n * a ) / b.d;
		public static rat2 operator *( int2 a, rat b ) => new(a.x * b, a.y * b);
		public static rat2 operator *( rat a, int2 b ) => new(a * b.x, a * b.y);

		// additional division operations
		public static rat operator /( rat a, int b ) => checked( new(a.n, a.d * b) );
		public static rat operator /( int a, rat b ) => checked( new(a * b.d, b.n) );
		public static float operator /( rat a, float b ) => a.n / ( a.d * b );
		public static float operator /( float a, rat b ) => ( a * b.d ) / b.n;
		public static double operator /( rat a, double b ) => a.n / ( a.d * b );
		public static double operator /( double a, rat b ) => ( a * b.d ) / b.n;

		// comparison operators
		public static bool operator ==( rat a, rat b ) => a.CompareTo( b ) == 0;
		public static bool operator !=( rat a, rat b ) => a.CompareTo( b ) != 0;
		public static bool operator <( rat a, rat b ) => a.CompareTo( b ) < 0;
		public static bool operator >( rat a, rat b ) => a.CompareTo( b ) > 0;
		public static bool operator <=( rat a, rat b ) => a.CompareTo( b ) <= 0;
		public static bool operator >=( rat a, rat b ) => a.CompareTo( b ) >= 0;

		// comparison functions
		public int CompareTo( rat other ) => checked( ( n * other.d ).CompareTo( d * other.n ) );
		public bool Equals( rat other ) => n == other.n && d == other.d;
		public override bool Equals( object obj ) => obj is rat other && Equals( other );
		public override int GetHashCode() => HashCode.Combine( n, d );


	}

}