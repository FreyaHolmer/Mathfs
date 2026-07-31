using System;
using Unity.Mathematics;
using UnityEngine;

namespace Freya {

	public static class Solve {

		/// <summary>Solves for x in a polynomial equation c₀ + c₁x = rhs</summary>
		public static float? Eq( float c0, float c1, float equals ) => Polynomial( c0 - equals, c1 );

		/// <summary>Solves for x in a polynomial equation c₀ + c₁x + c₂x² = rhs. Roots are sorted in increasing order</summary>
		public static ResultsMax2<float> Eq( float c0, float c1, float c2, float equals ) => Polynomial( c0 - equals, c1, c2 );

		/// <summary>Solves for x in a polynomial equation c₀ + c₁x + c₂x² + c₃x³ = rhs. Roots are sorted in increasing order</summary>
		public static ResultsMax3<float> Eq( float c0, float c1, float c2, float c3, float equals ) => Polynomial( c0 - equals, c1, c2, c3 );

		/// <summary>Solves for x in a polynomial equation c₀ + c₁x + c₂x² + c₃x³ + c₄x⁴ = rhs. Roots are sorted in increasing order</summary>
		public static ResultsMax4<float> Eq( float c0, float c1, float c2, float c3, float c4, float equals ) => Polynomial( c0 - equals, c1, c2, c3, c4 );

		/// <summary>Finds roots/x values where a polynomial c₀ + c₁x = 0</summary>
		public static float? Polynomial( float c0, float c1 ) => Mathf.Approximately( c1, 0 ) ? null : -c0 / c1;

		/// <summary>Finds roots/x values where a polynomial c₀ + c₁x + c₂x² = 0. Roots are sorted in increasing order</summary>
		public static ResultsMax2<float> Polynomial( float c0, float c1, float c2 ) {
			if( Mathf.Approximately( c2, 0 ) )
				return Polynomial( c0, c1 ); // curve is lower order
			float disc = c1 * c1 - 4 * c2 * c0;
			if( Mathf.Approximately( disc, 0 ) )
				return Polynomial( c1, 2 * c2 ); // one root
			if( disc < 0 )
				return default; // no roots
			// two roots:
			float q = -( c1 + MathF.Sqrt( disc ) * c1.Sign() ) / 2;
			float r0 = q / c2;
			float r1 = c0 / q;
			return new ResultsMax2<float>( MathF.Min( r0, r1 ), MathF.Max( r0, r1 ) );
		}

		/// <summary>Finds roots/x values where a polynomial c₀ + c₁x + c₂x² + c₃x³ = 0. Roots are sorted in increasing order</summary>
		public static ResultsMax3<float> Polynomial( float c0, float c1, float c2, float c3 ) {
			if( Mathf.Approximately( c3, 0 ) )
				return Polynomial( c0, c1, c2 ); // curve is lower order
			if( c2 == 0f && c3 == 1f )
				return SolveDepressedCubicRoots( c0, c1 ); // It's a depressed cubic :( c₀ + c₁t + t³

			// first, depress the cubic to make it easier to solve
			float aa = c3 * c3;
			float ac = c3 * c1;
			float bb = c2 * c2;
			float p_c1 = ( 3 * ac - bb ) / ( 3 * aa );
			float q_c0 = ( 2 * bb * c2 - 9 * ac * c2 + 27 * aa * c0 ) / ( 27 * aa * c3 );

			ResultsMax3<float> dpr = SolveDepressedCubicRoots( q_c0, p_c1 );

			// we now have the roots of the depressed cubic, now convert back to the normal cubic
			ResultsMax3<float> results = default;
			for( int i = 0; i < dpr.count; i++ )
				results = results.InsertSorted( dpr[i] - c2 / ( 3 * c3 ) );
			return results;
		}

		/// <summary>Finds roots/x values where a polynomial c₀ + c₁x + c₂x² + c₃x³ + c₄x⁴ = 0. Roots are sorted in increasing order</summary>
		static ResultsMax4<float> Polynomial( float c0, float c1, float c2, float c3, float c4 ) {
			if( Mathf.Approximately( c4, 0 ) )
				return Polynomial( c0, c1, c2, c3 ); // curve is lower order
			if( Mathf.Approximately( c1, 0 ) && Mathf.Approximately( c3, 0 ) )
				return SolveBiquadraticRoots( c0, c2, c4 ); // curve is biquadratic -> c0 + c2x² + c4x⁴= 0

			float iA = 1f / c4;
			float BoA = c3 * iA;
			float BoA2 = BoA * BoA;
			float BoA3 = BoA * BoA * BoA;
			float BoA4 = BoA2 * BoA2;

			float a = -( 3f / 8f ) * BoA2 + c2 * iA;
			float b = BoA3 / 8f - ( c2 / 2 ) * BoA * iA + c1 * iA;
			float c = -( 3f / 256 ) * BoA4 + ( c2 / 16f ) * BoA2 * iA - ( c1 / 4 ) * BoA * iA + c0 * iA;

			if( Mathf.Approximately( b, 0 ) )
				return SolveBiquadraticRoots( c, a, 1 );

			// not biquadratic oh boy
			ResultsMax3<float> yRoots = Solve.Polynomial( ( a * c ) / 2f - ( b * b ) / 8f, -c, -a / 2f, 1 );
			if( yRoots.count == 0 )
				return default; // no roots

			// filter roots
			float y = float.NaN;
			float vBest = float.NegativeInfinity;
			for( int i = 0; i < yRoots.count; i++ ) {
				float v = 2 * yRoots[i] - a;
				if( Mathf.Approximately( v, 0 ) )
					continue;
				if( v >= 0f && v > vBest ) {
					y = yRoots[i];
					vBest = v;
				}
			}
			if( float.IsNaN( y ) )
				return default; // no roots

			float R = math.sqrt( vBest );
			float i0 = b / ( 2f * R );
			ResultsMax2<float> q0 = Solve.Polynomial( y / 2f - i0, +R / 2, 1f );
			ResultsMax2<float> q1 = Solve.Polynomial( y / 2f + i0, -R / 2, 1f );

			float offset = -c3 / ( 4 * c4 );
			ResultsMax4<float> results = default;
			for( int i = 0; i < q0.count; i++ )
				results = results.InsertSorted( q0[i] + offset );
			for( int i = 0; i < q1.count; i++ )
				results = results.InsertSorted( q1[i] + offset );
			return results;
		}

		/// <summary>t³ + c₁t + c₀ = 0</summary>
		static ResultsMax3<float> SolveDepressedCubicRoots( float c0, float c1 ) {
			if( Mathf.Approximately( c1, 0 ) ) // triple root - one solution. solve x³+q = 0 => x = cr(-q)
				return new ResultsMax3<float>( Mathfs.Cbrt( -c0 ) );
			float discriminant = 4 * c1 * c1 * c1 + 27 * c0 * c0;
			if( discriminant < 0.00001 ) { // two or three roots guaranteed, use trig solution
				float pre = 2 * MathF.Sqrt( -c1 / 3 );
				float acosInner = ( ( 3 * c0 ) / ( 2 * c1 ) ) * MathF.Sqrt( -3 / c1 );

				float GetRoot( int k ) => pre * MathF.Cos( ( 1f / 3f ) * Mathfs.Acos( acosInner.ClampNeg1to1() ) - ( Mathfs.TAU / 3f ) * k );
				// if acos hits 0 or TAU/2, the offsets will have the same value,
				// which means we have a double root plus one regular root on our hands
				if( acosInner >= 0.9999f )
					return new ResultsMax3<float>( GetRoot( 0 ), GetRoot( 2 ) ); // two roots - one single and one double root
				if( acosInner <= -0.9999f )
					return new ResultsMax3<float>( GetRoot( 1 ), GetRoot( 2 ) ); // two roots - one single and one double root
				return new ResultsMax3<float>( GetRoot( 0 ), GetRoot( 1 ), GetRoot( 2 ) ); // three roots
			}

			if( discriminant > 0 && c1 < 0 ) { // one root
				float coshInner = ( 1f / 3f ) * Mathfs.Acosh( ( -3 * c0.Abs() / ( 2 * c1 ) ) * MathF.Sqrt( -3 / c1 ) );
				float r = -2 * Mathfs.Sign( c0 ) * MathF.Sqrt( -c1 / 3 ) * Mathfs.Cosh( coshInner );
				return new ResultsMax3<float>( r );
			}

			if( c1 > 0 ) { // one root
				float sinhInner = ( 1f / 3f ) * Mathfs.Asinh( ( ( 3 * c0 ) / ( 2 * c1 ) ) * MathF.Sqrt( 3 / c1 ) );
				float r = ( -2 * MathF.Sqrt( c1 / 3 ) ) * Mathfs.Sinh( sinhInner );
				return new ResultsMax3<float>( r );
			}

			// no roots
			return default;
		}

		/// <summary>c4x⁴ + c2x² + c0 = 0</summary>
		static ResultsMax4<float> SolveBiquadraticRoots( float c0, float c2, float c4 ) {
			ResultsMax2<float> z = Polynomial( c0, c2, c4 );

			// filter roots, keep positive only
			if( z.count == 2 ) {
				if( z.b < 0 && z.a < 0 )
					return default; // no roots
				if( z.a >= 0 && z.b < 0 )
					z = new ResultsMax2<float>( z.a );
				if( z.b >= 0 && z.a < 0 )
					z = new ResultsMax2<float>( z.b );
			} else if( z.count == 1 && z.a < 0 ) {
				return default; // no roots
			}

			if( z.count == 2 ) {
				( float small, float big ) = z.a < z.b ? ( z.a, z.b ) : ( z.b, z.a );
				big = math.sqrt( big );
				if( Mathf.Approximately( c0, 0 ) ) // three roots
					return new ResultsMax4<float>( -big, 0, big );
				// four roots
				small = math.sqrt( small );
				return new ResultsMax4<float>( -big, -small, small, big );
			} else if( z.count == 1 ) {
				if( Mathf.Approximately( c0, 0 ) )
					return 0; // one root at 0
				// two roots
				float x = math.sqrt( z.a );
				return new ResultsMax4<float>( -x, x );
			}
			return default;
		}
	}

}