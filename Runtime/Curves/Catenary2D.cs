// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using System;
using UnityEngine;

namespace Freya {

	/// <summary>A catenary curve passing through two points with a given an arc length</summary>
	public struct Catenary2D {

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

			( Vector2 pLeft, Vector2 pRight ) = p0.x > p1.x ? ( p1, p0 ) : ( p0, p1 );
			Vector2 p = pRight - pLeft;
			float x = Mathfs.Lerp( 0, p.x, t );

			float y;
			if( evaluability == Evaluability.LinearVertical ) { // chain is almost completely vertical, use a linear approximation
				float ts = t * s;
				float seg0 = ( s - p.y ) / 2;
				y = ( ts < seg0 ) ? -ts : -2 * seg0 + ts;
			} else {
				y = EvalFrom0( x );
			}

			return new Vector2( x, y ) + pLeft;
		}

		// Passing through (0,0) and point p
		float EvalFrom0( float x ) => a * Mathfs.Cosh( ( x - delta.x ) / a ) + delta.y;

		bool IsFullyVertical( float dx ) => dx < 0.001f;
		bool IsStraightLine() => s <= Vector2.Distance( p0, p1 ) * 1.00005f;

		void ReadyForEvaluation() {
			if( evaluability != Evaluability.Unknown )
				return;

			// CASE 1:
			// first, test if it's a line segment
			if( IsStraightLine() ) {
				Debug.Log( "line seg" );
				evaluability = Evaluability.LineSegment;
				return;
			}

			// relative to origin point p
			Vector2 p = p1 - p0;
			if( p.x < 0 )
				p = -p; // swap 0 and p

			// CASE 2:
			// check if it's basically a fully vertical hanging chain
			if( IsFullyVertical( p.x ) ) {
				Debug.Log( "linear vertical" );
				evaluability = Evaluability.LinearVertical;
				return;
			}

			// Now we've got a possible catenary curve on our hands

			// set up function
			float c = Mathf.Sqrt( s * s - p.y * p.y );
			float F( float a ) => 2 * a * Mathfs.Sinh( p.x / ( 2 * a ) ) - c;

			// find bounds of the root
			float xRoot = ( p.x * p.x ) / ( 2 * s ); // intial guess based on freya's flawless heuristics
			bool rootFound = FindRootRangeExponential( F, xRoot, out FloatRange xRange, out FloatRange yRange );

			if( rootFound == false ) { // refine if it hasn't already been found
				// CASE 4:
				// it's possible we failed to find a valid root range
				if( yRange.Contains( 0 ) == false ) {
					evaluability = Evaluability.LinearVertical;
					return;
				}

				// CASE 5:
				// Catenary seems valid, with roots inside! now refine this range
				RootFindBisections( F, ref xRange, BISECT_REFINE_COUNT );
			}

			// set a to the middle of the latest range
			a = xRange.Center;

			// cached delta, for faster evaluation:
			delta.x = ( p.x - a * Mathf.Log( ( s + p.y ) / ( s - p.y ) ) ) / 2;
			delta.y = -a * Mathfs.Cosh( -delta.x / a );
			evaluability = Evaluability.Catenary;
		}

		static bool FindRootRangeExponential( Func<float, float> F, float initialGuess, out FloatRange xRange, out FloatRange yRange ) {
			float xRoot = initialGuess;
			float yTest = F( xRoot );
			xRange = new FloatRange( xRoot, xRoot );
			yRange = new FloatRange( yTest, yTest );

			// find which direction to search in
			if( Mathf.Approximately( yTest, 0 ) ) {
				// already on the root, no need to iterate
				return true;
			} else if( yTest > 0 ) { // search forwards for a negative value, set upper bound
				for( int i = 0; i < INTERVAL_SEARCH_ITERATIONS; i++ ) {
					xRoot *= 2;
					float value = F( xRoot );
					if( value < 0 ) {
						xRange.b = xRoot; // found negative value!
						yRange.b = value;
						break;
					} else {
						xRange.a = xRoot; // still positive, shift left bound
						yRange.a = value;
					}
				}
			} else { // search backwards for a positive value, set lower bound
				for( int i = 0; i < INTERVAL_SEARCH_ITERATIONS; i++ ) {
					xRoot *= 0.5f;
					float value = F( xRoot );
					if( value > 0 ) {
						xRange.a = xRoot; // found positive value!
						yRange.a = value;
						break;
					} else {
						xRange.b = xRoot; // still negative, shift right bound
						yRange.b = value;
					}
				}
			}
			return false; // not found yet
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