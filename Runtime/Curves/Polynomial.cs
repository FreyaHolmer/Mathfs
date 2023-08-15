// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using System;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using UnityEngine.Serialization;

namespace Freya {

	/// <summary>A polynomial in the form <c>ax³+bx²+cx+d</c>, up to a cubic, with functions like derivatives, root finding, and more</summary>
	[Serializable] public struct Polynomial : IPolynomialCubic<Polynomial, float> {

		const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

		/// <summary>A polynomial with all 0 coefficients. f(x) = 0</summary>
		public static readonly Polynomial zero = new Polynomial( 0, 0, 0, 0 );

		/// <summary>A polynomial with all NaN coefficients</summary>
		public static readonly Polynomial NaN = new Polynomial( float.NaN, float.NaN, float.NaN, float.NaN );

		/// <summary>The constant coefficient</summary>
		[FormerlySerializedAs( "fConstant" )] public float c0;

		/// <summary>The linear coefficient</summary>
		[FormerlySerializedAs( "fLinear" )] public float c1;

		/// <summary>The quadratic coefficient</summary>
		[FormerlySerializedAs( "fQuadratic" )] public float c2;

		/// <summary>The cubic coefficient</summary>
		[FormerlySerializedAs( "fCubic" )] public float c3;

		/// <summary>Creates a polynomial up to a cubic</summary>
		/// <param name="c0">The constant coefficient</param>
		/// <param name="c1">The linear coefficient</param>
		/// <param name="c2">The quadratic coefficient</param>
		/// <param name="c3">The cubic coefficient</param>
		public Polynomial( float c0, float c1, float c2, float c3 ) => ( this.c0, this.c1, this.c2, this.c3 ) = ( c0, c1, c2, c3 );

		/// <summary>Creates a polynomial up to a quadratic</summary>
		/// <param name="c0">The constant coefficient</param>
		/// <param name="c1">The linear coefficient</param>
		/// <param name="c2">The quadratic coefficient</param>
		public Polynomial( float c0, float c1, float c2 ) => ( this.c0, this.c1, this.c2, this.c3 ) = ( c0, c1, c2, 0 );

		/// <summary>Creates a polynomial up to a linear</summary>
		/// <param name="c0">The constant coefficient</param>
		/// <param name="c1">The linear coefficient</param>
		public Polynomial( float c0, float c1 ) => ( this.c0, this.c1, this.c2, this.c3 ) = ( c0, c1, 0, 0 );

		/// <summary>Creates a polynomial</summary>
		/// <param name="coefficients">The coefficients to use</param>
		public Polynomial( Vector4 coefficients ) => ( c0, c1, c2, c3 ) = ( coefficients.x, coefficients.y, coefficients.z, coefficients.w );

		/// <inheritdoc cref="Polynomial(Vector4)"/>
		public Polynomial( Matrix4x1 coefficients ) => ( c0, c1, c2, c3 ) = ( coefficients.m0, coefficients.m1, coefficients.m2, coefficients.m3 );

		/// <inheritdoc cref="Polynomial(Vector4)"/>
		public Polynomial( Matrix3x1 coefficients ) => ( c0, c1, c2, c3 ) = ( coefficients.m0, coefficients.m1, coefficients.m2, 0 );

		/// <inheritdoc cref="Polynomial(Vector4)"/>
		public Polynomial( (float c0, float c1, float c2, float c3) coefficients ) => ( c0, c1, c2, c3 ) = coefficients;

		/// <inheritdoc cref="Polynomial(Vector4)"/>
		public Polynomial( (float c0, float c1, float c2) coefficients ) => ( c0, c1, c2, c3 ) = ( coefficients.c0, coefficients.c1, coefficients.c2, 0 );

		#region IPolynomialCubic

		public float C0 {
			[MethodImpl( INLINE )] get => c0;
			[MethodImpl( INLINE )] set => c0 = value;
		}
		public float C1 {
			[MethodImpl( INLINE )] get => c1;
			[MethodImpl( INLINE )] set => c1 = value;
		}
		public float C2 {
			[MethodImpl( INLINE )] get => c2;
			[MethodImpl( INLINE )] set => c2 = value;
		}
		public float C3 {
			[MethodImpl( INLINE )] get => c3;
			[MethodImpl( INLINE )] set => c3 = value;
		}
		public Polynomial this[ int i ] {
			[MethodImpl( INLINE )] get => i == 0 ? this : throw new IndexOutOfRangeException( "float polynomials don't have vector components" );
			[MethodImpl( INLINE )] set => this = value;
		}

		public int Degree => GetPolynomialDegree( c0, c1, c2, c3 );

		[MethodImpl( INLINE )] public float GetCoefficient( int degree ) =>
			degree switch {
				0 => c0,
				1 => c1,
				2 => c2,
				3 => c3,
				_ => throw new IndexOutOfRangeException( "Polynomial coefficient degree/index has to be between 0 and 3" )
			};

		[MethodImpl( INLINE )] public void SetCoefficient( int degree, float value ) {
			_ = degree switch {
				0 => c0 = value,
				1 => c1 = value,
				2 => c2 = value,
				3 => c3 = value,
				_ => throw new IndexOutOfRangeException( "Polynomial coefficient degree/index has to be between 0 and 3" )
			};
		}

		public float Eval( float t ) {
			float t2 = t * t;
			float t3 = t * t2;
			return c3 * t3 + c2 * t2 + c1 * t + c0;
		}

		[MethodImpl( INLINE )] public float Eval( float t, int n ) => Differentiate( n ).Eval( t );

		[MethodImpl( INLINE )] public Polynomial Differentiate( int n = 1 ) {
			return n switch {
				0 => this,
				1 => new Polynomial( c1, 2 * c2, 3 * c3, 0 ),
				2 => new Polynomial( 2 * c2, 6 * c3, 0, 0 ),
				3 => new Polynomial( 6 * c3, 0, 0, 0 ),
				_ => n > 3 ? zero : throw new IndexOutOfRangeException( "Cannot differentiate a negative amount of times" )
			};
		}

		public Polynomial ScaleParameterSpace( float factor ) {
			// ReSharper disable once CompareOfFloatsByEqualityOperator
			if( factor == 1f )
				return this;
			float factor2 = factor * factor;
			float factor3 = factor2 * factor;
			return new Polynomial(
				c0,
				c1 / factor,
				c2 / factor2,
				c3 / factor3
			);
		}

		/// <summary>Given an inner function g(x), returns f(g(x))</summary>
		/// <param name="g0">The constant coefficient of the inner function g(x)</param>
		/// <param name="g1">The linear coefficient of the inner function g(x)</param>
		public Polynomial Compose( float g0, float g1 ) {
			float g0_2 = g0 * g0;
			float g0_3 = g0 * g0_2;
			float g1_2 = g1 * g1;
			float g1_3 = g1 * g1_2;
			return new Polynomial(
				c0 + c1 * g0 + c2 * g0_2 + c3 * g0_3,
				c1 * g1 + c2 * 2 * g0 * g1 + c3 * 3 * g0_2 * g1,
				c2 * g1_2 + c3 * 3 * g0 * g1_2,
				c3 * g1_3
			);
		}

		#endregion

		/// <summary>Fits a cubic polynomial to pass through the given coordinates</summary>
		public static Polynomial FitCubic( float x0, float x1, float x2, float x3, float y0, float y1, float y2, float y3 ) {
			// precalcs
			float i01 = x1 - x0;
			float i02 = x2 - x0;
			float i03 = x3 - x0;
			float i12 = x2 - x1;
			float i13 = x3 - x1;
			float i23 = x3 - x2;
			float x0x1 = x0 * x1;
			float x0x2 = x0 * x2;
			float x0x3 = x0 * x3;
			float x1x2 = x1 * x2;
			float x1x3 = x1 * x3;
			float x2x3 = x2 * x3;
			float x1x2x3 = x1 * x2x3;
			float x0x2x3 = x0 * x2x3;
			float x0x1x3 = x0 * x1x3;
			float x0x1x2 = x0 * x1x2;
			float x0plusx1 = x0 + x1;
			float x0plusx1plusx2 = x0plusx1 + x2;
			float x0plusx1plusx3 = x0plusx1 + x3;
			float x2plusx3 = x2 + x3;
			float x0plusx2plusx3 = x0 + x2plusx3;
			float x1plusx2plusx3 = x1 + x2plusx3;
			float x1x2plusx1x3plusx2x3 = ( x1x2 + x1x3 + x2x3 );
			float x0x2plusx0x3plusx2x3 = ( x0x2 + x0x3 + x2x3 );
			float x0x1plusx0x3plusx1x3 = ( x0x1 + x0x3 + x1x3 );
			float x0x1plusx0x2plusx1x2 = ( x0x1 + x0x2 + x1x2 );

			// scale factors
			float scl0 = -( y0 / ( i01 * i02 * i03 ) );
			float scl1 = +( y1 / ( i01 * i12 * i13 ) );
			float scl2 = -( y2 / ( i02 * i12 * i23 ) );
			float scl3 = +( y3 / ( i03 * i13 * i23 ) );

			// polynomial form
			float c0 = -( scl0 * x1x2x3 + scl1 * x0x2x3 + scl2 * x0x1x3 + scl3 * x0x1x2 );
			float c1 = scl0 * x1x2plusx1x3plusx2x3 + scl1 * x0x2plusx0x3plusx2x3 + scl2 * x0x1plusx0x3plusx1x3 + scl3 * x0x1plusx0x2plusx1x2;
			float c2 = -( scl0 * x1plusx2plusx3 + scl1 * x0plusx2plusx3 + scl2 * x0plusx1plusx3 + scl3 * x0plusx1plusx2 );
			float c3 = scl0 + scl1 + scl2 + scl3;

			return new Polynomial( (float)c0, (float)c1, (float)c2, (float)c3 );
		}

		/// <summary>Fits a cubic polynomial to pass through the given coordinates, assuming x0 = 0</summary>
		public static Polynomial FitCubicFrom0( float x1, float x2, float x3, float y0, float y1, float y2, float y3 ) {
			// precalcs
			float i12 = x2 - x1;
			float i13 = x3 - x1;
			float i23 = x3 - x2;
			float x1x2 = x1 * x2;
			float x1x3 = x1 * x3;
			float x2x3 = x2 * x3;
			float x1x2x3 = x1 * x2x3;
			float x2plusx3 = x2 + x3;

			// scale factors
			float scl0 = -( y0 / ( x1 * x2 * x3 ) );
			float scl1 = +( y1 / ( x1 * i12 * i13 ) );
			float scl2 = -( y2 / ( x2 * i12 * i23 ) );
			float scl3 = +( y3 / ( x3 * i13 * i23 ) );

			// polynomial form
			float c0 = -( scl0 * x1x2x3 );
			float c1 = scl0 * ( x1x2 + x1x3 + x2x3 ) + scl1 * x2x3 + scl2 * x1x3 + scl3 * x1x2;
			float c2 = -( scl0 * ( x2plusx3 + x1 ) + scl1 * ( x2plusx3 ) + scl2 * ( x1 + x3 ) + scl3 * ( x1 + x2 ) );
			float c3 = scl0 + scl1 + scl2 + scl3;

			return new Polynomial( c0, c1, c2, c3 );
		}


		/// <summary>Splits the 0-1 range into two distinct polynomials at the given parameter value u, where both new curves cover the same total range with their individual 0-1 ranges</summary>
		/// <param name="u">The parameter value to split at</param>
		public (Polynomial pre, Polynomial post) Split01( float u ) {
			float d = 1f - u;
			float dd = d * d;
			float ddd = d * d * d;
			float uu = u * u;
			float uuu = u * u * u;

			Polynomial pre = new Polynomial( c0, c1 * u, c2 * uu, c3 * uuu );
			Polynomial post = new Polynomial(
				Eval( u ),
				d * Differentiate( 1 ).Eval( u ),
				dd / 2 * Differentiate( 2 ).Eval( u ),
				ddd / 6 * Differentiate( 3 ).Eval( u )
			);
			return ( pre, post );
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

		/// <summary>Creates a linear polynomial of the form <c>ax+b</c> from two points a and b</summary>
		/// <param name="a">The first point</param>
		/// <param name="b">The second point</param>
		public static Polynomial Linear( Vector2 a, Vector2 b ) => Linear( a.x, a.y, b.x, b.y );

		/// <summary>Creates a linear polynomial of the form <c>ax+b</c> from two points</summary>
		/// <param name="x0">The coordinate of the first point</param>
		/// <param name="y0">The value of the first point</param>
		/// <param name="x1">The coordinate of the second point</param>
		/// <param name="y1">The value of the second point</param>
		public static Polynomial Linear( float x0, float y0, float x1, float y1 ) {
			float d = ( y1 - y0 ) / ( x1 - x0 );
			return new Polynomial( y0 - d * x0, d, 0, 0 );
		}

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

			if( rootContent >= 0 ) { // crosses at two points
				float u = -b * -( b < 0 ? -1 : 1 ) * MathF.Sqrt( rootContent );
				float r0 = u / ( 2 * a );
				float r1 = ( 2 * c ) / u;
				return new ResultsMax2<float>( MathF.Min( r0, r1 ), MathF.Max( r0, r1 ) );
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
				float pre = 2 * MathF.Sqrt( -p / 3 );
				float acosInner = ( ( 3 * q ) / ( 2 * p ) ) * MathF.Sqrt( -3 / p );

				float GetRoot( int k ) => pre * MathF.Cos( ( 1f / 3f ) * Mathfs.Acos( acosInner.ClampNeg1to1() ) - ( Mathfs.TAU / 3f ) * k );
				// if acos hits 0 or TAU/2, the offsets will have the same value,
				// which means we have a double root plus one regular root on our hands
				if( acosInner >= 0.9999f )
					return new ResultsMax3<float>( GetRoot( 0 ), GetRoot( 2 ) ); // two roots - one single and one double root
				if( acosInner <= -0.9999f )
					return new ResultsMax3<float>( GetRoot( 1 ), GetRoot( 2 ) ); // two roots - one single and one double root
				return new ResultsMax3<float>( GetRoot( 0 ), GetRoot( 1 ), GetRoot( 2 ) ); // three roots
			}

			if( discriminant > 0 && p < 0 ) { // one root
				float coshInner = ( 1f / 3f ) * Mathfs.Acosh( ( -3 * q.Abs() / ( 2 * p ) ) * MathF.Sqrt( -3 / p ) );
				float r = -2 * Mathfs.Sign( q ) * MathF.Sqrt( -p / 3 ) * Mathfs.Cosh( coshInner );
				return new ResultsMax3<float>( r );
			}

			if( p > 0 ) { // one root
				float sinhInner = ( 1f / 3f ) * Mathfs.Asinh( ( ( 3 * q ) / ( 2 * p ) ) * MathF.Sqrt( 3 / p ) );
				float r = ( -2 * MathF.Sqrt( p / 3 ) ) * Mathfs.Sinh( sinhInner );
				return new ResultsMax3<float>( r );
			}

			// no roots
			return default;
		}

		#endregion

		#endregion

		#region Typecasting & Operators

		public static Polynomial operator /( Polynomial p, float v ) => new(p.c0 / v, p.c1 / v, p.c2 / v, p.c3 / v);
		public static Polynomial operator /( float v, Polynomial p ) => new(v / p.c0, v / p.c1, v / p.c2, v / p.c3);
		public static Polynomial operator *( Polynomial p, float v ) => new(p.c0 * v, p.c1 * v, p.c2 * v, p.c3 * v);
		public static Polynomial operator *( float v, Polynomial p ) => p * v;
		public static Polynomial operator +( Polynomial a, Polynomial b ) => new(a.c0 + b.c0, a.c1 + b.c1, a.c2 + b.c2, a.c3 + b.c3);
		public static Polynomial operator -( Polynomial a, Polynomial b ) => new(a.c0 - b.c0, a.c1 - b.c1, a.c2 - b.c2, a.c3 - b.c3);

		public static explicit operator Matrix3x1( Polynomial poly ) => new(poly.c0, poly.c1, poly.c2);
		public static explicit operator Matrix4x1( Polynomial poly ) => new(poly.c0, poly.c1, poly.c2, poly.c3);
		public static explicit operator BezierQuad1D( Polynomial poly ) => poly.Degree < 3 ? new BezierQuad1D( CharMatrix.quadraticBezierInverse * (Matrix3x1)poly ) : throw new InvalidCastException( "Cannot cast a cubic polynomial to a quadratic curve" );
		public static explicit operator BezierCubic1D( Polynomial poly ) => new(CharMatrix.cubicBezierInverse * (Matrix4x1)poly);
		public static explicit operator CatRomCubic1D( Polynomial poly ) => new(CharMatrix.cubicCatmullRomInverse * (Matrix4x1)poly);
		public static explicit operator HermiteCubic1D( Polynomial poly ) => new(CharMatrix.cubicHermiteInverse * (Matrix4x1)poly);
		public static explicit operator UBSCubic1D( Polynomial poly ) => new(CharMatrix.cubicUniformBsplineInverse * (Matrix4x1)poly);

		#endregion

		static StringBuilder strBuilder = new StringBuilder( 64 );
		static string[] tPowerSuffixStr = new[] { "", "x", "x²", "x³" };

		public override string ToString() {
			strBuilder.Clear();

			bool hasAddedFirstTerm = false;
			for( int c = 0; c < 4; c++ ) {
				float value = GetCoefficient( c );
				if( value != 0 ) {
					if( hasAddedFirstTerm == false ) {
						hasAddedFirstTerm = true;
						strBuilder.Append( GetCoefficient( c ) );
					} else {
						if( value > 0 )
							strBuilder.Append( "+" );
						strBuilder.Append( GetCoefficient( c ) );
						if( c > 0 )
							strBuilder.Append( tPowerSuffixStr[c] );
					}
				}
			}

			if( hasAddedFirstTerm == false )
				return "0"; // no terms. constant 0

			return strBuilder.ToString();
		}

		public string ToStringCoefficients() => $"({c0},{c1},{c2},{c3})";

	}

}