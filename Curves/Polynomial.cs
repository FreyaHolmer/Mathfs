// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Serialization;

namespace Freya {

	/// <summary>A polynomial in the form <c>ax³+bx²+cx+d</c>, up to a cubic, with functions like derivatives, root finding, and more</summary>
	[Serializable] public struct Polynomial {

		const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

		/// <summary>A polynomial with all 0 coefficients. f(x) = 0</summary>
		public static readonly Polynomial zero = new Polynomial( 0, 0, 0, 0 );

		/// <summary>The cubic coefficient</summary>
		[FormerlySerializedAs( "fCubic" )] public float c3;

		/// <summary>The quadratic coefficient</summary>
		[FormerlySerializedAs( "fQuadratic" )] public float c2;

		/// <summary>The linear coefficient</summary>
		[FormerlySerializedAs( "fLinear" )] public float c1;

		/// <summary>The constant coefficient</summary>
		[FormerlySerializedAs( "fConstant" )] public float c0;

		/// <summary>Get or set the coefficient of the given degree</summary>
		/// <param name="degree">The degree of the coefficient you want to get/set. For example, 0 will return the constant coefficient, 3 will return the cubic coefficient</param>
		public float this[ int degree ] {
			get =>
				degree switch {
					0 => c0,
					1 => c1,
					2 => c2,
					3 => c3,
					_ => throw new IndexOutOfRangeException( "Polynomial coefficient degree/index has to be between 0 and 3" )
				};
			set {
				_ = degree switch {
					0 => c0 = value,
					1 => c1 = value,
					2 => c2 = value,
					3 => c3 = value,
					_ => throw new IndexOutOfRangeException( "Polynomial coefficient degree/index has to be between 0 and 3" )
				};
			}
		}

		/// <summary>The degree of the polynomial</summary>
		public int Degree => GetPolynomialDegree( c0, c1, c2, c3 );

		/// <summary>Creates a polynomial up to a cubic</summary>
		/// <param name="c0">The constant coefficient</param>
		/// <param name="c1">The linear coefficient</param>
		/// <param name="c2">The quadratic coefficient</param>
		/// <param name="c3">The cubic coefficient</param>
		public Polynomial( float c0, float c1, float c2, float c3 ) => ( this.c0, this.c1, this.c2, this.c3 ) = ( c0, c1, c2, c3 );

		/// <summary>Evaluates the polynomial at the given value <c>t</c></summary>
		/// <param name="t">The value to sample at</param>
		public float Eval( float t ) => c3 * ( t * t * t ) + c2 * ( t * t ) + c1 * t + c0;

		/// <summary>Differentiates this function, returning the n-th derivative of this polynomial</summary>
		/// <param name="n">The number of times to differentiate this function. 0 returns the function itself, 1 returns the first derivative</param>
		public Polynomial Differentiate( int n = 1 ) {
			return n switch {
				0 => this,
				1 => new Polynomial( c1, 2 * c2, 3 * c3, 0 ),
				2 => new Polynomial( 2 * c2, 6 * c3, 0, 0 ),
				3 => new Polynomial( 6 * c3, 0, 0, 0 ),
				_ => n > 3 ? zero : throw new IndexOutOfRangeException( "Cannot differentiate a negative amount of times" )
			};
		}

		/// <summary>Calculates the roots (values where this polynomial = 0)</summary>
		public ResultsMax3<float> Roots => GetCubicRoots( c0, c1, c2, c3 );

		/// <summary>Calculates the local extrema of this polynomial</summary>
		public ResultsMax2<float> LocalExtrema => (ResultsMax2<float>)Differentiate().Roots;

		/// <summary>Calculates the local extrema of this polynomial in the unit interval</summary>
		public ResultsMax2<float> LocalExtrema01 {
			get {
				ResultsMax2<float> all = LocalExtrema;
				ResultsMax2<float> valids = new ResultsMax2<float>();
				for( int i = 0; i < all.count; i++ ) {
					float t = all[i];
					if( t.Within( 0, 1 ) )
						valids = valids.Add( all[i] );
				}

				return valids;
			}
		}

		/// <summary>Returns the output value range within the unit interval</summary>
		public FloatRange OutputRange01 {
			get {
				FloatRange range = ( Eval( 0 ), Eval( 1 ) );
				foreach( float t in LocalExtrema01 )
					range = range.Encapsulate( Eval( t ) );
				return range;
			}
		}

		#region Statics

		/// <summary>Creates a constant polynomial</summary>
		/// <param name="constant">The constant coefficient</param>
		public static Polynomial Constant( float constant ) => new Polynomial( constant, 0, 0, 0 );

		/// <summary>Creates a linear polynomial of the form <c>ax+b</c></summary>
		/// <param name="c0">The constant coefficient <c>b</c> in <c>ax+b</c></param>
		/// <param name="c1">The linear coefficient <c>a</c> in <c>ax+b</c></param>
		public static Polynomial Linear( float c0, float c1 ) => new Polynomial( c0, c1, 0, 0 );

		/// <summary>Creates a quadratic polynomial</summary>
		/// <param name="c0">The constant coefficient</param>
		/// <param name="c1">The linear coefficient</param>
		/// <param name="c2">The quadratic coefficient</param>
		public static Polynomial Quadratic( float c0, float c1, float c2 ) => new Polynomial( c0, c1, c2, 0 );

		/// <summary>Creates a cubic polynomial</summary>
		/// <param name="c0">The constant coefficient</param>
		/// <param name="c1">The linear coefficient</param>
		/// <param name="c2">The quadratic coefficient</param>
		/// <param name="c3">The cubic coefficient</param>
		public static Polynomial Cubic( float c0, float c1, float c2, float c3 ) => new Polynomial( c0, c1, c2, c3 );

		static bool ValueAlmost0( float v ) => Mathfs.Approximately( v, 0 );

		/// <summary>Given the coefficients for a cubic polynomial, returns the net polynomial type/degree, accounting for values very close to 0</summary>
		/// <param name="c0">The constant coefficient</param>
		/// <param name="c1">The linear coefficient</param>
		/// <param name="c2">The quadratic coefficient</param>
		/// <param name="c3">The cubic coefficient</param>
		[MethodImpl( INLINE )] public static int GetPolynomialDegree( float c0, float c1, float c2, float c3 ) => ValueAlmost0( c3 ) ? GetPolynomialDegree( c0, c1, c2 ) : 3;

		/// <summary>Given the coefficients for a quadratic polynomial, returns the net polynomial degree, accounting for values very close to 0</summary>
		/// <param name="c0">The constant coefficient</param>
		/// <param name="c1">The linear coefficient</param>
		/// <param name="c2">The quadratic coefficient</param>
		[MethodImpl( INLINE )] public static int GetPolynomialDegree( float c0, float c1, float c2 ) => ValueAlmost0( c2 ) ? GetPolynomialDegree( c0, c1 ) : 2;

		/// <summary>Given the coefficients for a linear polynomial, returns the net polynomial degree, accounting for values very close to 0</summary>
		/// <param name="c0">The constant coefficient</param>
		/// <param name="c1">The linear coefficient</param>
		[MethodImpl( INLINE )] public static int GetPolynomialDegree( float c0, float c1 ) => ValueAlmost0( c1 ) ? 0 : 1;

		/// <summary>Returns the roots/solutions/x-values where this polynomial equals 0. There's either 0, 1, 2 or 3 roots, filled in left to right among the return values</summary>
		/// <param name="c0">The constant coefficient</param>
		/// <param name="c1">The linear coefficient</param>
		/// <param name="c2">The quadratic coefficient</param>
		/// <param name="c3">The cubic coefficient</param>
		public static ResultsMax3<float> GetCubicRoots( float c0, float c1, float c2, float c3 ) =>
			GetPolynomialDegree( c0, c1, c2, c3 ) switch {
				0 => default, // either no roots or infinite roots if c == 0
				1 => new ResultsMax3<float>( SolveLinearRoot( c1, c0 ) ),
				2 => SolveQuadraticRoots( c2, c1, c0 ),
				3 => SolveCubicRoots( c3, c2, c1, c0 ),
				_ => throw new IndexOutOfRangeException()
			};

		/// <summary>Returns the roots/solutions/x-values where this polynomial equals 0. There's either 0, 1 or 2 roots, filled in left to right among the return values</summary>
		/// <param name="c0">The constant coefficient</param>
		/// <param name="c1">The linear coefficient</param>
		/// <param name="c2">The quadratic coefficient</param>
		public static ResultsMax2<float> GetQuadraticRoots( float c0, float c1, float c2 ) =>
			GetPolynomialDegree( c0, c1, c2 ) switch {
				0 => default, // either no roots or infinite roots if c == 0
				1 => new ResultsMax2<float>( SolveLinearRoot( c1, c0 ) ),
				2 => SolveQuadraticRoots( c2, c1, c0 ),
				_ => throw new IndexOutOfRangeException()
			};

		/// <summary>Returns the roots/solutions/x-values where this polynomial equals 0. Returns null if there is no root</summary>
		/// <param name="c0">The constant coefficient</param>
		/// <param name="c1">The linear coefficient</param>
		public static float? GetLinearRoots( float c0, float c1 ) {
			if( GetPolynomialDegree( c0, c1 ) == 0 )
				return null;
			return -c0 / c1;
		}

		/// <summary>Linearly interpolates between two polynomials</summary>
		/// <param name="a">The first polynomial to blend from</param>
		/// <param name="b">The second polynomial to blend to</param>
		/// <param name="t">The blend value, typically from 0 to 1</param>
		public static Polynomial Lerp( Polynomial a, Polynomial b, float t ) =>
			new(
				t.Lerp( a.c0, b.c0 ),
				t.Lerp( a.c1, b.c1 ),
				t.Lerp( a.c2, b.c2 ),
				t.Lerp( a.c3, b.c3 )
			);

		#region Internal root solvers

		// These functions lack safety checks (division by zero etc.) for lower degree equivalency - they presume "a" is always nonzero.
		// These are private to avoid people mistaking them for the more stable/safe functions you are more likely to want to use

		[MethodImpl( INLINE )] static float SolveLinearRoot( float a, float b ) => -b / a;

		static ResultsMax2<float> SolveQuadraticRoots( float a, float b, float c ) {
			float rootContent = b * b - 4 * a * c;
			if( ValueAlmost0( rootContent ) )
				return new ResultsMax2<float>( -b / ( 2 * a ) ); // two equivalent solutions at one point

			if( rootContent >= 0 ) {
				float root = Mathf.Sqrt( rootContent );
				float r0 = ( -b - root ) / ( 2 * a ); // crosses at two points
				float r1 = ( -b + root ) / ( 2 * a );
				return new ResultsMax2<float>( Mathf.Min( r0, r1 ), Mathf.Max( r0, r1 ) );
			}

			return default; // no roots
		}

		static ResultsMax3<float> SolveCubicRoots( float a, float b, float c, float d ) {
			// first, depress the cubic to make it easier to solve
			float aa = a * a;
			float ac = a * c;
			float bb = b * b;
			float p = ( 3 * ac - bb ) / ( 3 * aa );
			float q = ( 2 * bb * b - 9 * ac * b + 27 * aa * d ) / ( 27 * aa * a );

			ResultsMax3<float> dpr = SolveDepressedCubicRoots( p, q );

			// we now have the roots of the depressed cubic, now convert back to the normal cubic
			float UndepressRoot( float r ) => r - b / ( 3 * a );
			switch( dpr.count ) {
				case 1:  return new ResultsMax3<float>( UndepressRoot( dpr.a ) );
				case 2:  return new ResultsMax3<float>( UndepressRoot( dpr.a ), UndepressRoot( dpr.b ) );
				case 3:  return new ResultsMax3<float>( UndepressRoot( dpr.a ), UndepressRoot( dpr.b ), UndepressRoot( dpr.c ) );
				default: return default;
			}
		}

		// t³+pt+q = 0
		static ResultsMax3<float> SolveDepressedCubicRoots( float p, float q ) {
			if( ValueAlmost0( p ) ) // triple root - one solution. solve x³+q = 0 => x = cr(-q)
				return new ResultsMax3<float>( Mathfs.Cbrt( -q ) );
			float discriminant = 4 * p * p * p + 27 * q * q;
			if( discriminant < 0.00001 ) { // two or three roots guaranteed, use trig solution
				float pre = 2 * Mathf.Sqrt( -p / 3 );
				float acosInner = ( ( 3 * q ) / ( 2 * p ) ) * Mathf.Sqrt( -3 / p );

				float GetRoot( int k ) => pre * Mathf.Cos( ( 1f / 3f ) * Mathfs.Acos( acosInner.ClampNeg1to1() ) - ( Mathfs.TAU / 3f ) * k );
				// if acos hits 0 or TAU/2, the offsets will have the same value,
				// which means we have a double root plus one regular root on our hands
				if( acosInner >= 0.9999f )
					return new ResultsMax3<float>( GetRoot( 0 ), GetRoot( 2 ) ); // two roots - one single and one double root
				if( acosInner <= -0.9999f )
					return new ResultsMax3<float>( GetRoot( 1 ), GetRoot( 2 ) ); // two roots - one single and one double root
				return new ResultsMax3<float>( GetRoot( 0 ), GetRoot( 1 ), GetRoot( 2 ) ); // three roots
			}

			if( discriminant > 0 && p < 0 ) { // one root
				float coshInner = ( 1f / 3f ) * Mathfs.Acosh( ( -3 * q.Abs() / ( 2 * p ) ) * Mathf.Sqrt( -3 / p ) );
				float r = -2 * Mathfs.Sign( q ) * Mathf.Sqrt( -p / 3 ) * Mathfs.Cosh( coshInner );
				return new ResultsMax3<float>( r );
			}

			if( p > 0 ) { // one root
				float sinhInner = ( 1f / 3f ) * Mathfs.Asinh( ( ( 3 * q ) / ( 2 * p ) ) * Mathf.Sqrt( 3 / p ) );
				float r = ( -2 * Mathf.Sqrt( p / 3 ) ) * Mathfs.Sinh( sinhInner );
				return new ResultsMax3<float>( r );
			}

			// no roots
			return default;
		}

		#endregion

		#endregion

		public static Polynomial operator /( Polynomial p, float v ) => new(p.c0 / v, p.c1 / v, p.c2 / v, p.c3 / v);
		public static Polynomial operator /( float v, Polynomial p ) => new(v / p.c0, v / p.c1, v / p.c2, v / p.c3);
		public static Polynomial operator *( Polynomial p, float v ) => new(p.c0 * v, p.c1 * v, p.c2 * v, p.c3 * v);
		public static Polynomial operator *( float v, Polynomial p ) => p * v;

	}

}