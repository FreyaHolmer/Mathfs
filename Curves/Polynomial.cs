// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using System;
using System.ComponentModel;
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
					_ => throw new IndexOutOfRangeException( "Polynomial factor degree has to be between 0 and 3" )
				};
			set {
				_ = degree switch {
					0 => c0 = value,
					1 => c1 = value,
					2 => c2 = value,
					3 => c3 = value,
					_ => throw new IndexOutOfRangeException( "Polynomial factor degree has to be between 0 and 3" )
				};
			}
		}

		/// <summary>The degree of the polynomial</summary>
		public PolynomialDegree Degree => GetPolynomialDegree( c3, c2, c1, c0 );

		/// <inheritdoc cref="Polynomial.Cubic"/>
		public Polynomial( float a, float b, float c, float d ) => ( c3, c2, c1, c0 ) = ( a, b, c, d );

		/// <summary>Evaluates the polynomial at the given value <c>t</c></summary>
		/// <param name="t">The value to sample at</param>
		public float Eval( float t ) => c3 * ( t * t * t ) + c2 * ( t * t ) + c1 * t + c0;
		
		/// <summary>Differentiates this function, returning the n-th derivative of this polynomial</summary>
		/// <param name="n">The number of times to differentiate this function. 0 returns the function itself, 1 returns the first derivative</param>
		public Polynomial Differentiate( int n = 1 ) {
			return n switch {
				0 => this,
				1 => new Polynomial( 0, 3 * c3, 2 * c2, c1 ),
				2 => new Polynomial( 0, 0, 6 * c3, 2 * c2 ),
				3 => new Polynomial( 0, 0, 0, 6 * c3 ),
				_ => n > 3 ? zero : throw new IndexOutOfRangeException( "Cannot differentiate a negative amount of times" )
			};
		}

		/// <summary>Calculates the roots (values where this polynomial = 0)</summary>
		public ResultsMax3<float> Roots => GetCubicRoots( c3, c2, c1, c0 );

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
		/// <param name="constant">The constant factor</param>
		public static Polynomial Constant( float constant ) => new Polynomial( 0, 0, 0, constant );

		/// <summary>Creates a linear polynomial of the form <c>ax+b</c></summary>
		/// <param name="a">The linear factor <c>a</c> in <c>ax+b</c></param>
		/// <param name="b">The constant factor <c>b</c> in <c>ax+b</c></param>
		public static Polynomial Linear( float a, float b ) => new Polynomial( 0, 0, a, b );

		/// <summary>Creates a quadratic polynomial of the form <c>ax²+bx+c</c></summary>
		/// <param name="a">The quadratic factor <c>a</c> in <c>ax²+bx+c</c></param>
		/// <param name="b">The linear factor <c>b</c> in <c>ax²+bx+c</c></param>
		/// <param name="c">The constant factor <c>c</c> in <c>ax²+bx+c</c></param>
		public static Polynomial Quadratic( float a, float b, float c ) => new Polynomial( 0, a, b, c );

		/// <summary>Creates a cubic polynomial of the form <c>ax³+bx²+cx+d</c></summary>
		/// <param name="a">The cubic factor <c>a</c> in <c>ax³+bx²+cx+d</c></param>
		/// <param name="b">The quadratic factor <c>b</c> in <c>ax³+bx²+cx+d</c></param>
		/// <param name="c">The linear factor <c>c</c> in <c>ax³+bx²+cx+d</c></param>
		/// <param name="d">The constant factor <c>d</c> in <c>ax³+bx²+cx+d</c></param>
		public static Polynomial Cubic( float a, float b, float c, float d ) => new Polynomial( a, b, c, d );

		static bool FactorAlmost0( float v ) => v.Abs() < 0.00001f;

		/// <summary>Given ax³+bx²+cx+d, returns the net polynomial type/degree, accounting for values very close to 0</summary>
		/// <param name="a">The cubic factor <c>a</c> in <c>ax³+bx²+cx+d</c></param>
		/// <param name="b">The quadratic factor <c>b</c> in <c>ax³+bx²+cx+d</c></param>
		/// <param name="c">The linear factor <c>c</c> in <c>ax³+bx²+cx+d</c></param>
		/// <param name="d">The constant factor <c>d</c> in <c>ax³+bx²+cx+d</c></param>
		[MethodImpl( INLINE )] public static PolynomialDegree GetPolynomialDegree( float a, float b, float c, float d ) => FactorAlmost0( a ) ? GetPolynomialDegree( b, c, d ) : PolynomialDegree.Cubic;

		/// <summary>Given ax²+bx+c, returns the net polynomial degree, accounting for values very close to 0</summary>
		/// <param name="a">The quadratic factor <c>a</c> in <c>ax²+bx+c</c></param>
		/// <param name="b">The linear factor <c>b</c> in <c>ax²+bx+c</c></param>
		/// <param name="c">The constant factor <c>c</c> in <c>ax²+bx+c</c></param>
		[MethodImpl( INLINE )] public static PolynomialDegree GetPolynomialDegree( float a, float b, float c ) => FactorAlmost0( a ) ? GetPolynomialDegree( b, c ) : PolynomialDegree.Quadratic;

		/// <summary>Given ax+b, returns the net polynomial degree, accounting for values very close to 0</summary>
		/// <param name="a">The linear factor <c>a</c> in <c>ax+b</c></param>
		/// <param name="b">The constant factor <c>b</c> in <c>ax+b</c></param>
		[MethodImpl( INLINE )] public static PolynomialDegree GetPolynomialDegree( float a, float b ) => FactorAlmost0( a ) ? PolynomialDegree.Constant : PolynomialDegree.Linear;

		/// <summary>Returns the roots/solutions of ax³+bx²+cx+d = 0. There's either 0, 1, 2 or 3 roots, filled in left to right among the return values</summary>
		/// <param name="a">The cubic factor <c>a</c> in <c>ax³+bx²+cx+d</c></param>
		/// <param name="b">The quadratic factor <c>b</c> in <c>ax³+bx²+cx+d</c></param>
		/// <param name="c">The linear factor <c>c</c> in <c>ax³+bx²+cx+d</c></param>
		/// <param name="d">The constant factor <c>d</c> in <c>ax³+bx²+cx+d</c></param>
		public static ResultsMax3<float> GetCubicRoots( float a, float b, float c, float d ) =>
			GetPolynomialDegree( a, b, c, d ) switch {
				PolynomialDegree.Constant  => default, // either no roots or infinite roots if c == 0
				PolynomialDegree.Linear    => new ResultsMax3<float>( SolveLinearRoot( c, d ) ),
				PolynomialDegree.Quadratic => SolveQuadraticRoots( b, c, d ),
				PolynomialDegree.Cubic     => SolveCubicRoots( a, b, c, d ),
				_                          => throw new InvalidEnumArgumentException()
			};

		/// <summary>Returns the roots/solutions of ax²+bx+c = 0. There's either 0, 1 or 2 roots, filled in left to right among the return values</summary>
		/// <param name="a">The quadratic factor <c>a</c> in <c>ax²+bx+c</c></param>
		/// <param name="b">The linear factor <c>b</c> in <c>ax²+bx+c</c></param>
		/// <param name="c">The constant factor <c>c</c> in <c>ax²+bx+c</c></param>
		public static ResultsMax2<float> GetQuadraticRoots( float a, float b, float c ) =>
			GetPolynomialDegree( a, b, c ) switch {
				PolynomialDegree.Constant  => default, // either no roots or infinite roots if c == 0
				PolynomialDegree.Linear    => new ResultsMax2<float>( SolveLinearRoot( b, c ) ),
				PolynomialDegree.Quadratic => SolveQuadraticRoots( a, b, c ),
				_                          => throw new InvalidEnumArgumentException()
			};

		/// <summary>Returns the root/solution of ax+b = 0. Returns null if there is no root</summary>
		/// <param name="a">The linear factor <c>a</c> in <c>ax+b</c></param>
		/// <param name="b">The constant factor <c>b</c> in <c>ax+b</c></param>
		public static float? GetLinearRoots( float a, float b ) {
			if( GetPolynomialDegree( a, b ) == PolynomialDegree.Constant )
				return null;
			return -b / a;
		}


		/// <summary>Linearly interpolates between two polynomials</summary>
		/// <param name="a">The first polynomial to blend from</param>
		/// <param name="b">The second polynomial to blend to</param>
		/// <param name="t">The blend value, typically from 0 to 1</param>
		public static Polynomial Lerp( Polynomial a, Polynomial b, float t ) =>
			new(
				t.Lerp( a.c3, b.c3 ),
				t.Lerp( a.c2, b.c2 ),
				t.Lerp( a.c1, b.c1 ),
				t.Lerp( a.c0, b.c0 )
			);

		#region Internal root solvers

		// These functions lack safety checks (division by zero etc.) for lower degree equivalency - they presume "a" is always nonzero.
		// These are private to avoid people mistaking them for the more stable/safe functions you are more likely to want to use

		[MethodImpl( INLINE )] static float SolveLinearRoot( float a, float b ) => -b / a;

		static ResultsMax2<float> SolveQuadraticRoots( float a, float b, float c ) {
			float rootContent = b * b - 4 * a * c;
			if( FactorAlmost0( rootContent ) )
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
			float p = ( 3 * a * c - b * b ) / ( 3 * a * a );
			float q = ( 2 * b * b * b - 9 * a * b * c + 27 * a * a * d ) / ( 27 * a * a * a );

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
			if( FactorAlmost0( p ) ) // triple root - one solution. solve x³+q = 0 => x = cr(-q)
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

		public static Polynomial operator /( Polynomial p, float v ) => new(p.c3 / v, p.c2 / v, p.c1 / v, p.c0 / v);
		public static Polynomial operator /( float v, Polynomial p ) => new(v / p.c3, v / p.c2, v / p.c1, v / p.c0);
		public static Polynomial operator *( Polynomial p, float v ) => new(p.c3 * v, p.c2 * v, p.c1 * v, p.c0 * v);
		public static Polynomial operator *( float v, Polynomial p ) => p * v;

	}

}