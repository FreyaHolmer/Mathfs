using System;
using UnityEngine;

namespace Freya {

	/// <summary>Various utility functions for splines</summary>
	public static class SplineUtils {

		/// <summary>Samples a bernstein polynomial bézier basis function</summary>
		/// <param name="degree">The degree of the bézier curve</param>
		/// <param name="i">The basis function index</param>
		/// <param name="t">The value to sample at</param>
		public static float SampleBernsteinBasisFunction( int degree, int i, float t ) {
			ulong bc = Mathfs.BinomialCoef( (uint)degree, (uint)i );
			double scale = Math.Pow( 1f - t, degree - i ) * Math.Pow( t, i );
			return (float)( bc * scale );
		}

		public static float[] GenerateUniformKnots( int degree, int pCount, bool open ) {
			int kCount = degree + pCount + 1;
			float[] knots = new float[kCount];
			// open:		0 0[0 1 2 3 4]4 4
			// closed:	   [0 1 2 3 4 5 6 7 8]
			for( int i = 0; i < kCount; i++ )
				knots[i] = open == false ? i : Mathf.Clamp( i - degree, 0, kCount - 2 * degree - 1 );
			return knots;
		}

		internal static int BSplineKnotCount( int pointCount, int degree ) => degree + pointCount + 1;

		public static float CalcCatRomKnot( float kPrev, float alpha, float sqDist ) {
			return kPrev + sqDist.Pow( 0.5f * alpha ).AtLeast( 0.00001f ); // ensure there are no duplicate knots
		}

		public static (float, float, float, float) CalcCatRomKnots( Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float alpha, bool unitInterval ) {
			if( alpha == 0 ) // uniform catrom
				return ( -1, 0, 1, 2 );
			float i01 = Vector2.SqrMagnitude( p0 - p1 ).Pow( 0.5f * alpha );
			float i12 = Vector2.SqrMagnitude( p1 - p2 ).Pow( 0.5f * alpha );
			float i23 = Vector2.SqrMagnitude( p2 - p3 ).Pow( 0.5f * alpha );

			float k0, k1, k2, k3;
			if( unitInterval ) {
				return ( -i01 / i12, 0, 1, 1 + i23 / i12 );
			} else {
				k0 = 0;
				k1 = k0 + i01;
				k2 = k1 + i12;
				k3 = k2 + i23;
			}

			return ( k0, k1, k2, k3 );
		}

		internal static Polynomial2D CalculateCatRomCurve( Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float k0, float k1, float k2, float k3 ) {
			float k1k1 = k1 * k1;
			float k2k2 = k2 * k2;
			float k0k1 = k0 * k1;
			float _2k0k1 = 2 * k0k1;
			float k0k2 = k0 * k2;
			float k1k2 = k1 * k2;
			float _2k1k2 = 2 * k1k2;
			float k1k3 = k1 * k3;
			float k2k3 = k2 * k3;
			float _2k2k3 = 2 * k2k3;
			float k0k1k2 = k0k1 * k2;
			float k0k1k3 = k0k1 * k3;
			float k1k1k2 = k1k1 * k2;
			float k0k2k3 = k0k2 * k3;
			float k1k1k3 = k1k1 * k3;
			float k1k2k2 = k1k2 * k2;
			float k1k2k3 = k1k2 * k3;
			float k0k1k1 = k0k1 * k1;
			float k0k2k2 = k0k2 * k2;

			// CHAR matrix COLUMN 0:
			float p0u0 = -k1k2k2;
			float p0u1 = _2k1k2 + k2k2;
			float p0u2 = -k1 - 2 * k2;
			const float p0u3 = 1;
			// CHAR matrix COLUMN 1:
			float p1u0 = k2 * ( k0k1k2 + k0k1k3 - k0k2k3 - k1k1k3 );
			float p1u1 = -3 * k0k1k2 - k0k1k3 + k0k2k3 + k1k1k3 + k1k1k2 - k1k2k2 + k1k2k3 + k2k2 * k3;
			float p1u2 = _2k0k1 + k0k2 - k1k1 + k1k2 - k1k3 - _2k2k3;
			float p1u3 = -k0 + k3;
			// CHAR matrix COLUMN 2:
			float common = k0k1k3 + k0k2k2 - k0k2k3;
			float p2u0 = -k1 * ( common - k1k2k3 );
			float p2u1 = k0k1k1 + k0k1k2 + common - k1k1k2 + k1k2k2 - 3 * k1k2k3;
			float p2u2 = -_2k0k1 - k0k2 + k1k2 + k1k3 - k2k2 + _2k2k3;
			float p2u3 = k0 - k3;
			// CHAR matrix COLUMN 3:
			float p3u0 = k1k1k2;
			float p3u1 = -k1k1 - _2k1k2;
			float p3u2 = 2 * k1 + k2;
			const float p3u3 = -1;

			float i01 = k0 - k1;
			float i02 = k0 - k2;
			float i12 = k1 - k2;
			float i12sq = i12 * i12;
			float i13 = k1 - k3;
			float i23 = k2 - k3;
			Vector2 scaledP0 = p0 / ( i01 * i02 * i12 );
			Vector2 scaledP1 = p1 / ( i01 * i12sq * i13 );
			Vector2 scaledP2 = p2 / ( i02 * i12sq * i23 );
			Vector2 scaledP3 = p3 / ( i12 * i13 * i23 );
			return new Polynomial2D(
				p0u0 * scaledP0 + p1u0 * scaledP1 + p2u0 * scaledP2 + p3u0 * scaledP3,
				p0u1 * scaledP0 + p1u1 * scaledP1 + p2u1 * scaledP2 + p3u1 * scaledP3,
				p0u2 * scaledP0 + p1u2 * scaledP1 + p2u2 * scaledP2 + p3u2 * scaledP3,
				p0u3 * scaledP0 + p1u3 * scaledP1 + p2u3 * scaledP2 + p3u3 * scaledP3
			);
		}

	}

}