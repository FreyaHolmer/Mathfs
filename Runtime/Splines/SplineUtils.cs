// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

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

		public static float CalcCatRomKnot( float kPrev, float sqDist, float alpha ) {
			return kPrev + CalcCatRomKnot( sqDist, alpha ).AtLeast( 0.00001f ); // ensure there are no duplicate knots
		}

		public static float CalcCatRomKnot( float squaredDistance, float alpha ) =>
			alpha switch {
				0 => 1, // uniform
				1 => squaredDistance.Sqrt(), // chordal
				2 => squaredDistance, // centripetal
				_ => squaredDistance.Pow( 0.5f * alpha )
			};

		static readonly Matrix4x1 knotsUniformUnit = new(-1, 0, 1, 2);
		static readonly Matrix4x1 knotsUniform = new(0, 1, 2, 3);

		static Matrix4x1 GetUniformKnots( bool unitInterval ) => unitInterval ? knotsUniformUnit : knotsUniform;

		public static Matrix4x1 CalcCatRomKnots( Vector2Matrix4x1 m, float alpha, bool unitInterval ) {
			if( alpha == 0 ) // uniform catrom
				return GetUniformKnots( unitInterval );
			float sqMag01 = Vector2.SqrMagnitude( m.m0 - m.m1 );
			float sqMag12 = Vector2.SqrMagnitude( m.m1 - m.m2 );
			float sqMag23 = Vector2.SqrMagnitude( m.m2 - m.m3 );
			return CalcCatRomKnots( sqMag01, sqMag12, sqMag23, alpha, unitInterval );
		}

		public static Matrix4x1 CalcCatRomKnots( Vector3Matrix4x1 m, float alpha, bool unitInterval ) {
			if( alpha == 0 ) // uniform catrom
				return GetUniformKnots( unitInterval );
			float sqMag01 = Vector3.SqrMagnitude( m.m0 - m.m1 );
			float sqMag12 = Vector3.SqrMagnitude( m.m1 - m.m2 );
			float sqMag23 = Vector3.SqrMagnitude( m.m2 - m.m3 );
			return CalcCatRomKnots( sqMag01, sqMag12, sqMag23, alpha, unitInterval );
		}

		static Matrix4x1 CalcCatRomKnots( float sqMag01, float sqMag12, float sqMag23, float alpha, bool unitInterval ) {
			float i01 = CalcCatRomKnot( sqMag01, alpha );
			float i12 = CalcCatRomKnot( sqMag12, alpha );
			float i23 = CalcCatRomKnot( sqMag23, alpha );
			float k0, k1, k2, k3;
			if( unitInterval ) {
				return new(-i01 / i12, 0, 1, 1 + i23 / i12);
			} else {
				k0 = 0;
				k1 = k0 + i01;
				k2 = k1 + i12;
				k3 = k2 + i23;
			}

			return new(k0, k1, k2, k3);
		}

		static Matrix4x4 GetNUCatRomCharMatrix( Matrix4x1 knots ) {
			float k0 = knots.m0;
			float k1 = knots.m1;
			float k2 = knots.m2;
			float k3 = knots.m3;
			if( k1 == 0f && k2 == 1f )
				return GetNUCatRomCharMatrixUnitInterval( k0, k3 );
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

			float common = _2k0k1 + k0k2 - k1k3 - _2k2k3;
			float common2 = k0k1k3 + k0k2k2 - k0k2k3;
			float common3 = k1k1k2 - k1k2k2;
			float common4 = k0 - k3;

			// CHAR matrix COLUMN 0:
			float p0u0 = -k1k2k2;
			float p0u1 = _2k1k2 + k2k2;
			float p0u2 = -k1 - 2 * k2;
			const float p0u3 = 1;
			// CHAR matrix COLUMN 1:
			float p1u0 = k2 * ( k0k1k2 + k0k1k3 - k0k2k3 - k1k1k3 );
			float p1u1 = -3 * k0k1k2 - k0k1k3 + k0k2k3 + k1k1k3 + common3 + k1k2k3 + k2k2 * k3;
			float p1u2 = common - k1k1 + k1k2;
			float p1u3 = -common4;
			// CHAR matrix COLUMN 2:
			float p2u0 = -k1 * ( common2 - k1k2k3 );
			float p2u1 = k0k1k1 + k0k1k2 + common2 - common3 - 3 * k1k2k3;
			float p2u2 = -common - k2k2 + k1k2;
			float p2u3 = common4;
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
			float p0sc = 1f / ( i01 * i02 * i12 );
			float p1sc = 1f / ( i01 * i12sq * i13 );
			float p2sc = 1f / ( i02 * i12sq * i23 );
			float p3sc = 1f / ( i12 * i13 * i23 );
			return CharMatrix.Create(
				p0sc * p0u0, p1sc * p1u0, p2sc * p2u0, p3sc * p3u0,
				p0sc * p0u1, p1sc * p1u1, p2sc * p2u1, p3sc * p3u1,
				p0sc * p0u2, p1sc * p1u2, p2sc * p2u2, p3sc * p3u2,
				p0sc * p0u3, p1sc * p1u3, p2sc * p2u3, p3sc * p3u3
			);
		}

		static Matrix4x4 GetNUCatRomCharMatrixUnitInterval( float k0, float k3 ) {
			float k0mk3 = k0 - k3;
			float k0m2k3 = k0mk3 - k3;
			float k0k3 = k0 * k3;

			// CHAR matrix COLUMN 1:
			float p1u1 = k0k3 + k3;
			float p1u2 = k0m2k3;
			float p1u3 = -k0mk3;
			// CHAR matrix COLUMN 2:
			float p2u1 = k0 - k0k3;
			float p2u2 = -k0m2k3 - 1;
			float p2u3 = k0mk3;

			float i02 = k0 - 1;
			float i23 = 1 - k3;
			float p0sc = 1f / ( -k0 * i02 );
			float p1sc = 1f / ( -k0k3 );
			float p2sc = 1f / ( i02 * i23 );
			float p3sc = 1f / ( k3 * i23 );

			return CharMatrix.Create(
				0, 1, 0, 0,
				p0sc, p1sc * p1u1, p2sc * p2u1, 0,
				p0sc * -2, p1sc * p1u2, p2sc * p2u2, p3sc,
				p0sc, p1sc * p1u3, p2sc * p2u3, -p3sc
			);
		}

		internal static Polynomial2D CalculateCatRomCurve( Vector2Matrix4x1 m, Matrix4x1 knots ) {
			return new Polynomial2D( GetNUCatRomCharMatrix( knots ).MultiplyColumnVector( m ) );
		}

		internal static Polynomial3D CalculateCatRomCurve( Vector3Matrix4x1 m, Matrix4x1 knots ) {
			return new Polynomial3D( GetNUCatRomCharMatrix( knots ).MultiplyColumnVector( m ) );
		}

	}

}