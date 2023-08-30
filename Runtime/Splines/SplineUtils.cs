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

		public static float CalcCatRomKnot( float kPrev, float sqDist, float alpha, bool isSquaredDist ) {
			return kPrev + CalcCatRomKnot( sqDist, alpha, isSquaredDist ).AtLeast( 0.00001f ); // ensure there are no duplicate knots
		}

		public static float CalcCatRomKnot( float dist, float alpha, bool isSquaredDist ) =>
			alpha switch {
				0    => 1, // uniform
				0.5f => isSquaredDist ? dist.Pow( 0.25f ) : Mathf.Sqrt( dist ), // centripetal
				1    => isSquaredDist ? Mathf.Sqrt( dist ) : dist, // chordal
				_    => isSquaredDist ? dist.Pow( 0.5f * alpha ) : dist.Pow( alpha )
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
			return CalcCatRomKnots( sqMag01, sqMag12, sqMag23, alpha, unitInterval, isSquaredDist:true );
		}

		public static Matrix4x1 CalcCatRomKnots( Vector3Matrix4x1 m, float alpha, bool unitInterval ) {
			if( alpha == 0 ) // uniform catrom
				return GetUniformKnots( unitInterval );
			float sqMag01 = Vector3.SqrMagnitude( m.m0 - m.m1 );
			float sqMag12 = Vector3.SqrMagnitude( m.m1 - m.m2 );
			float sqMag23 = Vector3.SqrMagnitude( m.m2 - m.m3 );
			return CalcCatRomKnots( sqMag01, sqMag12, sqMag23, alpha, unitInterval, isSquaredDist:true );
		}

		static Matrix4x1 CalcCatRomKnots( float dist01, float dist12, float dist23, float alpha, bool unitInterval, bool isSquaredDist ) {
			float i01 = CalcCatRomKnot( dist01, alpha, isSquaredDist );
			float i12 = CalcCatRomKnot( dist12, alpha, isSquaredDist );
			float i23 = CalcCatRomKnot( dist23, alpha, isSquaredDist );
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

		public static Matrix4x4 GetNUCatRomCharMatrix( Matrix4x1 knots ) {
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
			float p0sc = ( i01 * i02 * i12 );
			float p1sc = ( i01 * i12sq * i13 );
			float p2sc = ( i02 * i12sq * i23 );
			float p3sc = ( i12 * i13 * i23 );
			return CharMatrix.Create(
				p0u0 / p0sc, p1u0 / p1sc, p2u0 / p2sc, p3u0 / p3sc,
				p0u1 / p0sc, p1u1 / p1sc, p2u1 / p2sc, p3u1 / p3sc,
				p0u2 / p0sc, p1u2 / p1sc, p2u2 / p2sc, p3u2 / p3sc,
				p0u3 / p0sc, p1u3 / p1sc, p2u3 / p2sc, p3u3 / p3sc
			);
		}

		public static Vector3 GetNUCatRomCharMatrixC2End( Matrix4x1 knots, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3 ) {
			float k0 = knots.m0;
			float k1 = knots.m1;
			float k2 = knots.m2;
			float k3 = knots.m3;

			float k1k1 = k1 * k1;
			float k2k2 = k2 * k2;
			float k0k1 = k0 * k1;
			float _2k0k1 = 2 * k0k1;
			float k0k2 = k0 * k2;
			float k1k2 = k1 * k2;
			float k1k3 = k1 * k3;
			float k2k3 = k2 * k3;
			float _2k2k3 = 2 * k2k3;

			float common = _2k0k1 + k0k2 - k1k3 - _2k2k3;

			// CHAR matrix COLUMN 0:
			// CHAR matrix COLUMN 1:
			// CHAR matrix COLUMN 2:
			// CHAR matrix COLUMN 3:

			float i01 = k0 - k1;
			float i02 = k0 - k2;
			float i12 = k1 - k2;
			float i12sq = i12 * i12;
			float i13 = k1 - k3;
			float i23 = k2 - k3;
			float p0sc = ( i01 * i02 * i12 );
			float p1sc = ( i01 * i12sq * i13 );
			float p2sc = ( i02 * i12sq * i23 );
			float p3sc = ( i12 * i13 * i23 );

			float m20 = ( -k1 - 2 * k2 ) / p0sc;
			float m21 = ( common - k1k1 + k1k2 ) / p1sc;
			float m22 = ( -common - k2k2 + k1k2 ) / p2sc;
			float m23 = ( 2 * k1 + k2 ) / p3sc;
			float m30 = 1f / p0sc;
			float m31 = ( k3 - k0 ) / p1sc;
			float m32 = ( k0 - k3 ) / p2sc;
			float m33 = -1f / p3sc;

			return p0 * ( ( m20 + 3 * m30 ) / m23 ) +
				   p1 * ( ( m21 + 3 * m31 - m20 ) / m23 ) +
				   p2 * ( ( m22 + 3 * m32 - m21 ) / m23 ) +
				   p3 * ( 1 + ( 3 * m33 - m22 ) / m23 );
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
				p0sc, -p1u1 / k0k3, p2sc * p2u1, 0,
				p0sc * -2, -p1u2 / k0k3, p2sc * p2u2, p3sc,
				p0sc, -p1u3 / k0k3, p2sc * p2u3, -p3sc
			);
		}

		internal static Polynomial2D CalculateCatRomCurve( Vector2Matrix4x1 m, Matrix4x1 knots ) {
			return new Polynomial2D( GetNUCatRomCharMatrix( knots ).MultiplyColumnVector( m ) );
		}

		internal static Polynomial3D CalculateCatRomCurve( Vector3Matrix4x1 m, Matrix4x1 knots ) {
			return new Polynomial3D( GetNUCatRomCharMatrix( knots ).MultiplyColumnVector( m ) );
		}

		internal static Polynomial3D CalculateHermiteCurve( Vector3Matrix4x1 m, float k0, float k1 ) {
			return new Polynomial3D( GetNUHermiteCharMatrix( k0, k1 ).MultiplyColumnVector( m ) );
		}

		internal static Polynomial2D CalculateHermiteCurve( Vector2Matrix4x1 m, float k0, float k1 ) {
			return new Polynomial2D( GetNUHermiteCharMatrix( k0, k1 ).MultiplyColumnVector( m ) );
		}

		static Matrix4x4 GetNUHermiteCharMatrix( float k0, float k1 ) {
			float d = k1 - k0;
			float d2 = d * d;
			float d3 = d * d * d;
			float k0_2 = k0 * k0;
			float k0_3 = k0 * k0 * k0;

			// row 0
			float m02 = ( 3 * k0_2 ) / d2 + ( 2 * k0_3 ) / d3;
			float m00 = 1 - m02;
			float _k02d = k0_2 / d;
			float _k03d2 = k0_3 / d2;
			float m03 = -_k02d - _k03d2;
			float m01 = -k0 - 2 * _k02d - _k03d2;

			// row 1
			float _2k0d = 2 * k0 / d;
			float _3k02d2 = 3 * k0_2 / d2;
			float m13 = _2k0d + _3k02d2;
			float m11 = 1 + 2 * _2k0d + _3k02d2;
			float m10 = 6 * ( k0 / d2 + k0_2 / d3 );
			float m12 = -m10;

			// row 2
			float m22 = 3 / d2 + ( 6 * k0 ) / d3;
			float m20 = -m22;
			float _dRcp = 1 / d;
			float _3k0d2 = 3 * k0 / d2;
			float m23 = -_dRcp - _3k0d2;
			float m21 = m23 - _dRcp;

			// row 3
			float m30 = 2 / d3;
			float m31 = 1 / d2;
			float m32 = -m30;
			float m33 = m31;

			return CharMatrix.Create(
				m00, m01, m02, m03,
				m10, m11, m12, m13,
				m20, m21, m22, m23,
				m30, m31, m32, m33
			);
		}

	}

}