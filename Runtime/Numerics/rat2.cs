// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions;
using static Freya.mathfs;

namespace Freya {

	/// <summary>A 2D vector with rational components (ℚ² instead of ℝ²)</summary>
	[Serializable] public struct rat2 : IEquatable<rat2>,
		IVec2<rat2, rat, rat, rat, rat2, rat>,
		ISignedNumber<int2>,
		IDotProduct<int2, rat>,
		IRoundable<int2> {
		[SerializeField] public rat x;
		[SerializeField] public rat y;
		public rat this[ int i ] => i switch { 0 => x, 1 => y, _ => throw new IndexOutOfRangeException( i.ToString() ) };
		public rat X => x;
		public rat Y => y;
		public rat2 zeroX => new(0, y);
		public rat2 zeroY => new(x, 0);
		public rat2 flipX => new(-x, y);
		public rat2 flipY => new(x, -y);
		// public Rational2 rot45chebyshev => throw new NotImplementedException();
		public static readonly rat2 zero = new(rat.zero, rat.zero);
		public static readonly rat2 half = new(rat.half, rat.half);
		public static readonly rat2 one = new(rat.one, rat.one);
		/// <summary>The numerator, as an int2 vector</summary>
		public int2 N => new(x.n, y.n);
		/// <summary>The denominator, as an int2 vector</summary>
		public int2 D => new(x.d, y.d);
		public rat2( rat x, rat y ) => ( this.x, this.y ) = ( x, y );
		public rat2( int2 v ) => ( this.x, this.y ) = ( v.x, v.y );

		public static rat2 FromVector2( Vector2 v, int snapStepsPerUnit = 2 ) {
			return new rat2( rat.FromFloat( v.x, snapStepsPerUnit ), rat.FromFloat( v.y, snapStepsPerUnit ) );
		}

		public bool isZero => math.all( N == new int2( 0, 0 ) );
		public bool isInteger => math.all( D == new int2( 1, 1 ) );
		public bool isOrthogonal => ( ceilAwayFrom0.abs() > 0 ).csum() <= 1;
		public bool IsDiagonal => x.abs == y.abs;

		// Chebyshev distances
		public rat2 to( rat2 target ) => target - this;
		public rat magSq => this.dot( this );
		public rat magChebyshev => this.abs.cmax;
		public rat magTaxicab => this.abs.csum;

		public rat2 normalizedChebyshev => this / magChebyshev;
		public static rat distChebyshev( rat2 a, rat2 b ) => ( b - a ).magChebyshev;

		public int quadrant => ceilAwayFrom0.quadrant();
		public int signedQuadrant => ceilAwayFrom0.signedQuadrant();
		public int2 quadrantBasisX => ceilAwayFrom0.quadrantBasisX();
		public (int2 x, int2 y) quadrantBasis => ceilAwayFrom0.quadrantBasis();
		public rat2 complexMul( rat2 other ) => new(x * other.x - y * other.y, x * other.y + y * other.x);
		public rat2 complexConj => new(x, -y);
		public int pointSideOfPlane( rat2 planePos, rat2 planeNormal ) => ( this - planePos ).dot( planeNormal ).sign;
		public rat projTValue( rat2 n ) => this.dot( n ) / n.dot( n );

		public rat2 normalizedTaxicab => this / magTaxicab;

		public static rat sqDist( rat2 a, rat2 b ) => ( b - a ).magSq;
		public int2 normalized {
			get {
				Assert.IsTrue( isOrthogonal, $"Non-orthogonal {nameof(rat2)} vectors can't be normalized" );
				return this.sign;
			}
		}
		/// <summary>Returns an int2 snapped to the nearest orthogonal normal.
		/// Ambiguities along diagonals are resolved in the positive rotation direction</summary>
		public int2 orthonormalized {
			get {
				Assert.IsFalse( isZero, "Can't orthonormalize a zero vector" );
				rat2 a = this.abs;
				rat2 g = this / a.cmax;
				if( IsDiagonal )
					return (int2)( ( g + g.rot90 ) / 2 );
				return a.x > a.y ? new int2( (int)g.x, 0 ) : new int2( 0, (int)g.y );
			}
		}
		/// <summary>Similar to <c>orthonormalized</c>, but also provides the "other" axis.
		/// Note that this is the other axis in the same direction, it does not strictly follow handedness</summary>
		public (int2 main, int2 secondary, rat mainLen, rat secondaryLen) decomposeOrthonormal {
			get {
				int2 m = orthonormalized;
				int2 s = m.rot90();
				if( isOrthogonal )
					s *= wedge( m ).sign;
				return ( m, s, projectionTValue( this, m ), projectionTValue( this, s ) );
			}
		}
		public rat2 rot90 => new(-y, x);
		public rat2 rotNeg90 => new(y, -x);
		public rat2 rot180 => -this;
		public rat cmin => x.min( y );
		public rat cmax => x.max( y );
		public rat csum => x + y;

		public rat dot( rat2 other ) => x * other.x + y * other.y;
		public rat dot( int2 other ) => x * other.x + y * other.y;
		public rat wedge( rat2 other ) => x * other.y - y * other.x;
		public rat wedge( int2 other ) => x * other.y - y * other.x;

		public rat2 abs => new(x.abs, y.abs);
		public rat2 max( rat2 other ) => new(x.max( other.x ), y.max( other.y ));
		public rat2 min( rat2 other ) => new(x.min( other.x ), y.min( other.y ));
		public int2 sign => new(x.sign, y.sign);
		public int2 round( RoundingDirection rounding = RoundingDirection.ToEven ) => new(x.round( rounding ), y.round( rounding ));
		public int2 floorToward0 => new(x.floorToward0, y.floorToward0);
		public int2 ceilAwayFrom0 => new(x.ceilAwayFrom0, y.ceilAwayFrom0);
		public int2 floor => new(x.floor, y.floor);
		public int2 ceil => new(x.ceil, y.ceil);

		public bool TryCastToIntHalf2( out inth2 ih ) {
			if( x.TryCastToIntHalf( out inth ihx ) && y.TryCastToIntHalf( out inth ihy ) ) {
				ih = new inth2( ihx, ihy );
				return true;
			}
			ih = default;
			return false;
		}

		public override string ToString() => $"( {x}, {y} )";

		/// <summary>Returns the signed "distance" from the plane to a point</summary>
		public static rat PointScaledDistFromPlane( rat2 pt, rat2 planePt, rat2 planeTangent ) => mathfs.wedge( pt - planePt, planeTangent );

		public static rat LineIntersectionTValueAlongA( rat2 aOrigin, rat2 aDir, rat2 bOrigin, rat2 bDir ) {
			rat2 beta = bDir.rot90;
			rat2 c = projectToNormal( bOrigin - aOrigin, beta );
			return projectionTValuePerp( c, aDir );
		}

		public static rat2 IntersectLines( rat2 aOrigin, rat2 aDir, rat2 bOrigin, rat2 bDir ) {
			if( mathfs.wedge( aDir, bDir ) == 0 ) // check if parallel
				throw new Exception( "Cannot intersect parallel lines" );
			rat t = LineIntersectionTValueAlongA( aOrigin, aDir, bOrigin, bDir );
			return aOrigin + aDir * t;
			// bOrigin -= aOrigin;
			// Rational2 beta = bDir.rot90;
			// Rational2 c = projectToNormal( bOrigin, beta );
			// Rational2 I = projectToNormalPerp( c, aDir );
			// return I + aOrigin;
		}

		// type casting
		public static implicit operator rat2( int2 n ) => new(n.x, n.y);
		public static explicit operator int2( rat2 r ) => r.isInteger ? r.N : throw new ArithmeticException( $"Rational value {r} can't be cast to an integer" );
		public static explicit operator inth2( rat2 r ) => new((inth)r.x, (inth)r.y);
		public static explicit operator float2( rat2 r ) => new((float)r.x, (float)r.y);
		public static explicit operator double2( rat2 r ) => new((double)r.x, (double)r.y);
		public static explicit operator Vector2( rat2 r ) => new((float)r.x, (float)r.y);
		public static explicit operator Vector3( rat2 r ) => new((float)r.x, (float)r.y, 0);
		public static explicit operator double3( rat2 r ) => new((double)r.x, (double)r.y, 0);

		// unary operations
		public static rat2 operator -( rat2 r ) => new(-r.x, -r.y);
		public static rat2 operator +( rat2 r ) => r;

		// addition
		public static rat2 operator +( rat2 a, rat2 b ) => new(a.x + b.x, a.y + b.y);
		public static rat2 operator +( rat2 a, int2 b ) => new(a.x + b.x, a.y + b.y);
		public static rat2 operator +( int2 a, rat2 b ) => new(a.x + b.x, a.y + b.y);

		// subtraction
		public static rat2 operator -( rat2 a, rat2 b ) => new(a.x - b.x, a.y - b.y);
		public static rat2 operator -( rat2 a, int2 b ) => new(a.x - b.x, a.y - b.y);
		public static rat2 operator -( int2 a, rat2 b ) => new(a.x - b.x, a.y - b.y);

		// multiplication
		// public static Rational2 operator *( Rational2 a, Rational2 b ) => new(a.x * b.x, a.y * b.y);
		public static rat2 operator *( rat2 a, rat b ) => new(a.x * b, a.y * b);
		public static rat2 operator *( rat a, rat2 b ) => new(a * b.x, a * b.y);
		public static rat2 operator *( rat2 a, int b ) => new(a.x * b, a.y * b);
		public static rat2 operator *( int a, rat2 b ) => new(a * b.x, a * b.y);
		public static float2 operator *( rat2 a, float b ) => new(a.x * b, a.y * b);
		public static float2 operator *( float a, rat2 b ) => new(a * b.x, a * b.y);
		public static double2 operator *( rat2 a, double b ) => new(a.x * b, a.y * b);
		public static double2 operator *( double a, rat2 b ) => new(a * b.x, a * b.y);
		// public static Rational2 operator *( Rational2 a, Rational2 b ) => checked( new(a.n * b.n, a.d * b.d) );

		// division
		public static rat2 operator /( rat2 a, rat b ) => new(a.x / b, a.y / b);
		public static rat2 operator /( rat2 a, int b ) => new(a.x / b, a.y / b);
		public static rat2 operator /( int a, rat2 b ) => new(a / b.x, a / b.y);
		public static float2 operator /( rat2 a, float b ) => new(a.x / b, a.y / b);
		public static double2 operator /( rat2 a, double b ) => new(a.x / b, a.y / b);

		// public static Rational2 operator /( Rational2 a, Rational2 b ) => checked( new(a.n * b.d, a.d * b.n) );
		// public static float operator /( float a, Rational2 b ) => ( a * b.d ) / b.n;
		// public static double operator /( double a, Rational2 b ) => ( a * b.d ) / b.n;

		// comparison operators
		public static bool2 operator ==( rat2 a, rat2 b ) => new(a.x == b.x, a.y == b.y);
		public static bool2 operator !=( rat2 a, rat2 b ) => new(a.x != b.x, a.y != b.y);
		public static bool2 operator <( rat2 a, rat2 b ) => new(a.x < b.x, a.y < b.y);
		public static bool2 operator >( rat2 a, rat2 b ) => new(a.x > b.x, a.y > b.y);
		public static bool2 operator <=( rat2 a, rat2 b ) => new(a.x <= b.x, a.y <= b.y);
		public static bool2 operator >=( rat2 a, rat2 b ) => new(a.x >= b.x, a.y >= b.y);
		public bool Equals( rat2 other ) => x.Equals( other.x ) && y.Equals( other.y );
		public override bool Equals( object obj ) => obj is rat2 other && Equals( other );
		public override int GetHashCode() => HashCode.Combine( x, y );


	}

}