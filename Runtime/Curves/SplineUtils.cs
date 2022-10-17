using System;
using UnityEngine;

namespace Freya {

	/// <summary>Various utility functions for splines</summary>
	public static class SplineUtils {

		/// <summary>Returns the cubic factors of the polynomials, of a single component, in the form at³+bt²+ct+d</summary>
		/// <param name="p0">The starting point of the curve</param>
		/// <param name="p1">The second control point of the curve, sometimes called the start tangent point</param>
		/// <param name="p2">The third control point of the curve, sometimes called the end tangent point</param>
		/// <param name="p3">The end point of the curve</param>
		public static Polynomial GetCubicPolynomial( float p0, float p1, float p2, float p3 ) =>
			new Polynomial(
				-p0 + 3 * ( p1 - p2 ) + p3,
				3 * ( p0 - 2 * p1 + p2 ),
				3 * ( -p0 + p1 ),
				p0 );


		/// <summary>Returns the cubic factors of the derivative polynomials, of a single component, in the form at²+bt+c</summary>
		/// <param name="p0">The starting point of the curve</param>
		/// <param name="p1">The second control point of the curve, sometimes called the start tangent point</param>
		/// <param name="p2">The third control point of the curve, sometimes called the end tangent point</param>
		/// <param name="p3">The end point of the curve</param>
		public static Polynomial GetCubicPolynomialDerivative( float p0, float p1, float p2, float p3 ) =>
			new Polynomial(
				3 * ( -p0 + 3 * ( p1 - p2 ) + p3 ),
				6 * ( p0 - 2 * p1 + p2 ),
				3 * ( -p0 + p1 ) );

		/// <summary>Returns the cubic factors of the second derivative polynomials, of a single component, in the form at+b</summary>
		/// <param name="p0">The starting point of the curve</param>
		/// <param name="p1">The second control point of the curve, sometimes called the start tangent point</param>
		/// <param name="p2">The third control point of the curve, sometimes called the end tangent point</param>
		/// <param name="p3">The end point of the curve</param>
		public static Polynomial GetCubicPolynomialSecondDerivative( float p0, float p1, float p2, float p3 ) =>
			new Polynomial(
				6 * ( -p0 + 3 * ( p1 - p2 ) + p3 ),
				6 * ( p0 - 2 * p1 + p2 ) );

		/// <summary>Returns the bernstein polynomial weights for positions in the curve at the given point t</summary>
		/// <param name="t">The t-value along the curve to sample</param>
		public static Vector4 GetBernsteinPolynomialWeights( float t ) {
			float omt = 1f - t;
			float omt2 = omt * omt;
			float t2 = t * t;
			return new Vector4(
				omt2 * omt, // (1-t)³
				3f * omt2 * t, // 3(1-t)²t
				3f * omt * t2, // 3(1-t)t²
				t2 * t // t³
			);
		}

		/// <summary>Returns the bernstein polynomial weights for the derivative of the curve at the given point t</summary>
		/// <param name="t">The t-value along the curve to sample</param>
		public static Vector4 GetBernsteinPolynomialWeightsDerivative( float t ) {
			float omt = 1f - t;
			float omt2 = omt * omt;
			float t2 = t * t;
			return new Vector4(
				-3 * omt2, // -3(1-t)²
				9 * t2 - 12 * t + 3, // 9t²-12t+3
				6 * t - 9 * t2, // 6t-9t²
				3 * t2 // 3t²
			);
		}

		/// <summary>Returns the bernstein polynomial weights for the second derivative of the curve at the given point t</summary>
		/// <param name="t">The t-value along the curve to sample</param>
		public static Vector4 GetBernsteinPolynomialWeightsSecondDerivative( float t ) {
			return new Vector4( 6 - 6 * t, 18 * t - 12, 6 - 18 * t, 6 * t );
		}
		
		/// <summary>Samples a bernstein polynomial bézier basis function</summary>
		/// <param name="degree">The degree of the bézier curve</param>
		/// <param name="i">The basis function index</param>
		/// <param name="t">The value to sample at</param>
		public static float SampleBasisFunction( int degree, int i, float t ) {
			ulong bc = Mathfs.BinomialCoef( (uint)degree, (uint)i );
			double scale = Math.Pow( 1f - t, degree - i ) * Math.Pow( t, i );
			return (float)(bc * scale);
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

	}

}