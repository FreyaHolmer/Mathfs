// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using System.Runtime.CompilerServices;
using UnityEngine;
using static Freya.Mathfs;

namespace Freya {

	/// <summary>Core intersection test functions.
	/// Note: these are pretty esoteric, generally it's easier to use the instance methods in each shape,
	/// such as <c>myLine.Intersect(otherThing)</c></summary>
	public static partial class IntersectionTest {

		// internal
		const float PARALLEL_DETERMINANT_THRESHOLD = 0.00001f;
		const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

		/// <summary>Returns whether or not these infinite lines intersect, and if they do, also returns the t-value for each infinite line</summary>
		/// <param name="aOrigin">First line origin</param>
		/// <param name="aDir">First line direction (does not have to be normalized)</param>
		/// <param name="bOrigin">Second line origin</param>
		/// <param name="bDir">Second line direction (does not have to be normalized)</param>
		/// <param name="tA">The t-value along the first line, where the intersection happened</param>
		/// <param name="tB">The t-value along the second line, where the intersection happened</param>
		public static bool LinearTValues( Vector2 aOrigin, Vector2 aDir, Vector2 bOrigin, Vector2 bDir, out float tA, out float tB ) {
			float d = Determinant( aDir, bDir );
			if( Abs( d ) < PARALLEL_DETERMINANT_THRESHOLD ) {
				tA = tB = default;
				return false;
			}

			Vector2 aToB = bOrigin - aOrigin;
			tA = Determinant( aToB, bDir ) / d;
			tB = Determinant( aToB, aDir ) / d;
			return true;
		}

		// based on https://stackoverflow.com/questions/1073336/circle-line-segment-collision-detection-algorithm
		/// <summary>Returns the intersections between an infinite line and a circle in the form of t-values along the line where the intersections lie. Or none, if there are none</summary>
		/// <param name="lineOrigin">Line origin</param>
		/// <param name="lineDir">Line direction (does not have to be normalized)</param>
		/// <param name="circleOrigin">Center or the circle</param>
		/// <param name="radius">Radius of the circle</param>
		public static ResultsMax2<float> LinearCircleTValues( Vector2 lineOrigin, Vector2 lineDir, Vector2 circleOrigin, float radius ) {
			Vector2 circleToLineOrigin = lineOrigin - circleOrigin;
			float a = Vector2.Dot( lineDir, lineDir ); // ray len sq
			float b = 2 * Vector2.Dot( circleToLineOrigin, lineDir );
			float c = Vector2.Dot( circleToLineOrigin, circleToLineOrigin ) - radius.Square();
			float discriminant = b * b - 4 * a * c;
			if( discriminant > 0 ) {
				discriminant = Sqrt( discriminant );
				if( discriminant < 0.00001f ) // line is tangent to the circle, one intersection
					return new ResultsMax2<float>( -b / ( 2 * a ) );
				float tA = ( -b + discriminant ) / ( 2 * a );
				float tB = ( -b - discriminant ) / ( 2 * a ); // line has two intersections
				return new ResultsMax2<float>( tA, tB );
			}

			return default; // line doesn't hit it at all
		}

		/// <summary>Returns whether or not two circles intersect, and the two intersection points (if they exist)</summary>
		/// <param name="aPos">The position of the first circle</param>
		/// <param name="aRadius">The radius of the first circle</param>
		/// <param name="bPos">The position of the second circle</param>
		/// <param name="bRadius">The radius of the second circle</param>
		public static ResultsMax2<Vector2> CirclesIntersectionPoints( Vector2 aPos, float aRadius, Vector2 bPos, float bRadius ) {
			float distSq = DistanceSquared( aPos, bPos );
			float dist = Sqrt( distSq );
			bool differentPosition = dist > 0.00001f;
			float maxRad = Max( aRadius, bRadius );
			float minRad = Min( aRadius, bRadius );
			bool ringsTouching = Mathf.Abs( dist - maxRad ) < minRad;

			if( ringsTouching && differentPosition ) {
				float aRadSq = aRadius * aRadius;
				float bRadSq = bRadius * bRadius;
				float lateralOffset = ( distSq - bRadSq + aRadSq ) / ( 2 * dist );
				float normalOffset = ( 0.5f / dist ) * Sqrt( 4 * distSq * aRadSq - ( distSq - bRadSq + aRadSq ).Square() );
				Vector2 tangent = ( bPos - aPos ) / dist;
				Vector2 normal = tangent.Rotate90CCW();
				Vector2 chordCenter = aPos + tangent * lateralOffset;
				if( normalOffset < 0.00001f )
					return new ResultsMax2<Vector2>( chordCenter ); // double intersection at one point
				return new ResultsMax2<Vector2>( // two intersections
					chordCenter + normal * normalOffset,
					chordCenter - normal * normalOffset
				);
			}

			return default; // no intersections
		}

		/// <summary>Returns whether or not two circles overlap</summary>
		/// <param name="aPos">The position of the first circle</param>
		/// <param name="aRadius">The radius of the first circle</param>
		/// <param name="bPos">The position of the second circle</param>
		/// <param name="bRadius">The radius of the second circle</param>
		public static bool CirclesOverlap( Vector2 aPos, float aRadius, Vector2 bPos, float bRadius ) {
			float dist = Vector2.Distance( aPos, bPos );
			float maxRad = Max( aRadius, bRadius );
			float minRad = Min( aRadius, bRadius );
			return Mathf.Abs( dist - maxRad ) < minRad;
		}

		/// <summary>Returns whether or not a line passes through a box centered at (0,0)</summary>
		/// <param name="extents">Box extents/"radius" per axis</param>
		/// <param name="pt">A point in the line</param>
		/// <param name="dir">The direction of the line</param>
		public static bool LineRectOverlap( Vector2 extents, Vector2 pt, Vector2 dir ) {
			Vector2 corner = new Vector2( extents.x, extents.y * -Sign( dir.x * dir.y ) );
			return SignAsInt( Determinant( dir, corner - pt ) ) != SignAsInt( Determinant( dir, -corner - pt ) );
		}
		/// <summary>Returns whether or not two discs overlap. Unlike circles, discs overlap even if one is smaller and is completely inside the other</summary>
		/// <param name="aPos">The position of the first disc</param>
		/// <param name="aRadius">The radius of the first disc</param>
		/// <param name="bPos">The position of the second disc</param>
		/// <param name="bRadius">The radius of the second disc</param>
		[MethodImpl( INLINE )] public static bool DiscsOverlap( Vector2 aPos, float aRadius, Vector2 bPos, float bRadius ) {
			return DistanceSquared( aPos, bPos ) <= ( aRadius + bRadius ).Square();
		}


	}

}