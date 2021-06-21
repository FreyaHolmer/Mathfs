using UnityEngine;

namespace Freya {

	public static class BezierUtils {

		/// <summary>Returns the cubic factors of the polynomials, of a single component, in the form at³+bt²+ct+d</summary>
		public static (float a, float b, float c, float d) GetCubicFactors( float p0, float p1, float p2, float p3 ) =>
		(
			-p0 + 3 * ( p1 - p2 ) + p3,
			3 * ( p0 - 2 * p1 + p2 ),
			3 * ( -p0 + p1 ),
			p0
		);

		/// <summary>Returns the cubic factors of the derivative polynomials, of a single component, in the form at²+bt+c</summary>
		public static (float a, float b, float c) GetCubicFactorsDerivative( float p0, float p1, float p2, float p3 ) =>
		(
			3 * ( -p0 + 3 * ( p1 - p2 ) + p3 ),
			6 * ( p0 - 2 * p1 + p2 ),
			3 * ( -p0 + p1 )
		);

		/// <summary>Returns the cubic factors of the second derivative polynomials, of a single component, in the form at+b</summary>
		public static (float a, float b) GetCubicFactorsSecondDerivative( float p0, float p1, float p2, float p3 ) =>
		(
			6 * ( -p0 + 3 * ( p1 - p2 ) + p3 ),
			6 * ( p0 - 2 * p1 + p2 )
		);

		/// <summary>Returns the bernstein polynomial weights for positions in the curve at the given point t</summary>
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
		public static Vector4 GetBernsteinPolynomialWeightsSecondDerivative( float t ) {
			return new Vector4( 6 - 6 * t, 18 * t - 12, 6 - 18 * t, 6 * t );
		}
	}

}