// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using System;
using UnityEngine;

namespace Freya {

	/// <summary>A catenary curve passing through two points with a given an arc length</summary>
	public struct Catenary2D {

		#region Standard catenary equations

		/// <summary>Returns the y coordinate of a catenary at the given x value</summary>
		/// <param name="x">The x coordinate to evaluate at</param>
		/// <param name="a">The a-parameter of the catenary</param>
		public static float Eval( float x, float a ) => a * Mathfs.Cosh( x / a );

		/// <summary>Evaluates the arc length from the apex of the catenary, to the given x coordinate.
		/// Note that this is negative when x is less than 0</summary>
		/// <param name="x">The x coordinate to get the length to</param>
		/// <param name="a">The a-parameter of the catenary</param>
		public static float EvalArcLen( float x, float a ) => a * Mathfs.Sinh( x / a );

		/// <summary>Evaluates the x coordinate at the given arc length relative to the apex of the catenary.
		/// Note that the input arc length can be negative, to get the negative x coordinates</summary>
		/// <param name="s">The arc length to get the x coordinate of</param>
		/// <param name="a">The a-parameter of the catenary</param>
		public static float EvalXByArcLength( float s, float a ) => a * Mathfs.Asinh( s / a );

		#endregion

		enum Evaluability {
			Unknown = 0,
			Catenary,
			LinearVertical,
			LineSegment
		}

		const int INTERVAL_SEARCH_ITERATIONS = 12;
		const int BISECT_REFINE_COUNT = 15;

		// input data
		readonly Vector2 p0, p1;
		readonly float s;

		// cached state
		float a;
		Vector2 delta;
		Evaluability evaluability;

		/// <summary>Creates a catenary curve between two points, given an arc length <c>s</c></summary>
		/// <param name="p0">The start of the curve</param>
		/// <param name="p1">The end of the curve</param>
		/// <param name="s">The length of the curve. note: has to be equal or longer than the distance between the points</param>
		public Catenary2D( Vector2 p0, Vector2 p1, float s ) {
			( this.p0, this.p1, this.s ) = ( p0, p1, s );
			a = 0;
			delta = default;
			evaluability = Evaluability.Unknown;
		}

		/// <summary>Evaluates a position on this catenary curve, given a <c>t</c>-value from 0 to 1</summary>
		/// <param name="t">A value from 0 to 1 along the whole curve</param>
		public Vector2 Eval( float t ) {
			ReadyForEvaluation();

			if( evaluability == Evaluability.LineSegment )
				return Vector3.LerpUnclamped( p0, p1, t ); // chain is almost completely linear

			Vector2 p = p1 - p0;
			float x = Mathfs.Lerp( 0, p.x, t );
			float y;
			if( evaluability == Evaluability.LinearVertical ) { // chain is almost completely vertical, use a linear approximation
				float ts = t * s;
				float seg0 = ( s - p.y ) / 2;
				y = ( ts < seg0 ) ? -ts : -2 * seg0 + ts;
			} else {
				y = EvalFrom0( x );
			}

			return new Vector2( x, y ) + p0;
		}

		// Passing through (0,0) and point p
		float EvalFrom0( float x ) => a * Mathfs.Cosh( ( x - delta.x ) / a ) + delta.y;

		bool IsFullyVertical( float dx ) => Mathf.Abs( dx ) < 0.001f;
		bool IsStraightLine() => s <= Vector2.Distance( p0, p1 ) * 1.00005f;

		void ReadyForEvaluation() {
			if( evaluability != Evaluability.Unknown )
				return;

			// CASE 1:
			// first, test if it's a line segment
			if( IsStraightLine() ) {
				evaluability = Evaluability.LineSegment;
				return;
			}

			// relative to origin point p
			Vector2 p = p1 - p0;

			// CASE 2:
			// check if it's basically a fully vertical hanging chain
			if( IsFullyVertical( p.x ) ) {
				evaluability = Evaluability.LinearVertical;
				return;
			}

			// CASE 3:
			// Now we've got a catenary on our hands unless something explodes.
			float c = Mathf.Sqrt( s * s - p.y * p.y );
			float pAbsX = p.x.Abs(); // solve only in x > 0
			float R( float a ) => 2 * a * Mathfs.Sinh( pAbsX / ( 2 * a ) ) - c; // set up root solve function

			// find bounds of the root
			float xRoot = ( p.x * p.x ) / ( 2 * s ); // intial guess based on freya's flawless heuristics
			if( TryFindRootBounds( R, xRoot, out FloatRange xRange ) ) {
				// refine range, if necessary (which is very likely)
				if( Mathf.Approximately( xRange.Length, 0 ) == false )
					RootFindBisections( R, ref xRange, BISECT_REFINE_COUNT ); // Catenary seems valid, with roots inside, refine the range
				a = xRange.Center; // set a to the middle of the latest range
				delta = CalcCatenaryDelta( a, p ); // find delta to pass through both points
				evaluability = Evaluability.Catenary;
			} else {
				// CASE 4:
				// something exploded, couldn't find a range, so let's use a straight line as a fallback
				evaluability = Evaluability.LineSegment;
			}
		}

		// Calculates the required offset to make a catenary pass through the origin and a point p
		static Vector2 CalcCatenaryDelta( float a, Vector2 p ) {
			Vector2 d;
			d.x = p.x / 2 - a * Mathfs.Asinh( p.y / ( 2 * a * Mathfs.Sinh( p.x / ( 2 * a ) ) ) );
			d.y = -a * Mathfs.Cosh( -d.x / a );
			return d;
		}

		// presumes a decreasing function with one root in x > 0
		// g = initial guess
		static bool TryFindRootBounds( Func<float, float> R, float g, out FloatRange xRange ) {
			float y = R( g );
			xRange = new FloatRange( g, g );
			if( Mathfs.Approximately( y, 0 ) ) // somehow landed *on* our root in our initial guess
				return true;

			bool findingUpper = y > 0;

			for( int n = 1; n <= INTERVAL_SEARCH_ITERATIONS; n++ ) {
				if( findingUpper ) {
					// It's positive - we found our lower bound
					// exponentially search for upper bound
					xRange.a = xRange.b;
					xRange.b = g * Mathf.Pow( 2, n );
					y = R( xRange.b );
					if( y < 0 )
						return true; // upper bound found!
				} else {
					// It's negative - we found our upper bound
					// exponentially search for lower bound
					xRange.b = xRange.a;
					xRange.a = g * Mathf.Pow( 2, -n );
					y = R( xRange.a );
					if( y > 0 )
						return true; // lower bound found!
				}
			}

			return false; // no root found
		}

		static void RootFindBisections( Func<float, float> F, ref FloatRange xRange, int iterationCount ) {
			for( int i = 0; i < iterationCount; i++ )
				RootFindBisection( F, ref xRange );
		}

		static void RootFindBisection( Func<float, float> F, ref FloatRange xRange ) {
			float xInter = xRange.Center; // bisection
			float yInter = F( xInter );
			if( yInter > 0 )
				xRange.a = xInter; // adjust left bound
			else
				xRange.b = xInter; // adjust right bound
		}

	}

}