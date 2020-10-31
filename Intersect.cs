// collected and expended upon by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using System;
using UnityEngine;
using static Freya.Mathfs;

namespace Freya {

	public static class Intersect {

		// These are used to see if an intersection t-value is valid for rays and line segments
		// If you want make on-vertex intersections not count, you can tweak that here by changing comparisons
		// You can also add margin here if you want to technically count slightly outside/inside as an intersection or not
		// (Note that Line isn't range bound, so it's not here. It's always true)
		static bool BoundsTestRay( float t ) => t >= 0;
		static bool BoundsTestLineSegment( float t ) => t >= 0 && t <= 1;

		// Naming order: Ray, LineSegment, Line, Circle

		#region Rays

		/// <summary>Returns whether or not two rays intersect</summary>
		public static bool Rays( Ray2D a, Ray2D b ) => GetLineLineTValues( b.origin - a.origin, a.dir, b.dir, out float tA, out float tB ) && BoundsTestRay( tA ) && BoundsTestRay( tB );

		/// <summary>Returns whether or not two rays intersect</summary>
		public static bool Rays( Vector2 aOrigin, Vector2 aDir, Vector2 bOrigin, Vector2 bDir ) => GetLineLineTValues( bOrigin - aOrigin, aDir, bDir, out float tA, out float tB ) && BoundsTestRay( tA ) && BoundsTestRay( tB );

		/// <summary>Returns the intersection point of two rays (if there is one)</summary>
		/// <param name="intersectionPoint">The point at which they intersect</param>
		public static bool Rays( Ray2D a, Ray2D b, out Vector2 intersectionPoint ) => Rays( a.origin, a.dir, b.origin, b.dir, out intersectionPoint );

		/// <summary>Returns the intersection point of two rays (if there is one)</summary>
		/// <param name="intersectionPoint">The point at which they intersect</param>
		public static bool Rays( Vector2 aOrigin, Vector2 aDir, Vector2 bOrigin, Vector2 bDir, out Vector2 intersectionPoint ) {
			if( GetLineLineTValues( bOrigin - aOrigin, aDir, bDir, out float tA, out float tB ) && BoundsTestRay( tA ) && BoundsTestRay( tB ) ) {
				intersectionPoint = aOrigin + aDir * tA;
				return true;
			}

			intersectionPoint = default;
			return false;
		}

		#endregion

		#region Ray-LineSegment

		/// <summary>Returns whether or not a ray and a line segment intersect</summary>
		public static bool RayLineSegment( Ray2D ray, LineSegment2D lineSegment ) => RayLineSegment( ray.origin, ray.dir, lineSegment.start, lineSegment.end );

		/// <summary>Returns whether or not a ray and a line segment intersect</summary>
		public static bool RayLineSegment( Vector2 rayOrigin, Vector2 rayDir, Vector2 lineSegStart, Vector2 lineSegEnd ) => GetLineLineTValues( lineSegStart - rayOrigin, rayDir, lineSegEnd, out float tA, out float tB ) && BoundsTestRay( tA ) && BoundsTestLineSegment( tB );

		/// <summary>Returns the intersection point of a ray and a line segment (if there is one)</summary>
		/// <param name="intersectionPoint">The point at which they intersect</param>
		public static bool RayLineSegment( Vector2 rayOrigin, Vector2 rayDir, Vector2 lineSegStart, Vector2 lineSegEnd, out Vector2 intersectionPoint ) {
			if( GetLineLineTValues( lineSegStart - rayOrigin, rayDir, lineSegEnd - lineSegStart, out float tA, out float tB ) && BoundsTestRay( tA ) && BoundsTestLineSegment( tB ) ) {
				intersectionPoint = rayOrigin + rayDir * tA;
				return true;
			}

			intersectionPoint = default;
			return false;
		}

		/// <summary>Returns the intersection point of a ray and a line segment (if there is one)</summary>
		/// <param name="intersectionPoint">The point at which they intersect</param>
		public static bool RayLineSegment( Ray2D ray, LineSegment2D lineSegment, out Vector2 intersectionPoint ) => RayLineSegment( ray.origin, ray.dir, lineSegment.start, lineSegment.end, out intersectionPoint );

		#endregion

		#region Ray-Line

		/// <summary>Returns whether or not a ray and a line intersect</summary>
		public static bool RayLine( Ray2D ray, Line2D line ) => RayLine( ray.origin, ray.dir, line.origin, line.dir );

		/// <summary>Returns whether or not a ray and a line intersect</summary>
		public static bool RayLine( Vector2 rayOrigin, Vector2 rayDir, Vector2 lineOrigin, Vector2 lineDir ) => GetLineLineTValue( lineOrigin - rayOrigin, rayDir, lineDir, out float tA ) && BoundsTestRay( tA );

		/// <summary>Returns the intersection point of a ray and an infinite line (if there is one)</summary>
		/// <param name="intersectionPoint">The point at which they intersect</param>
		public static bool RayLine( Vector2 rayOrigin, Vector2 rayDir, Vector2 lineOrigin, Vector2 lineDir, out Vector2 intersectionPoint ) {
			if( GetLineLineTValue( lineOrigin - rayOrigin, rayDir, lineDir, out float tA ) && BoundsTestRay( tA ) ) {
				intersectionPoint = rayOrigin + rayDir * tA;
				return true;
			}

			intersectionPoint = default;
			return false;
		}

		/// <summary>Returns the intersection point of a ray and an infinite line (if there is one)</summary>
		/// <param name="intersectionPoint">The point at which they intersect</param>
		public static bool RayLine( Ray2D ray, Line2D line, out Vector2 intersectionPoint ) => RayLine( ray.origin, ray.dir, line.origin, line.dir, out intersectionPoint );

		#endregion

		#region Ray-Circle

		/// <summary>Returns the number of intersections and the points at which a ray and a circle intersect (if they exist)</summary>
		public static int RayCircle( Vector2 rayOrigin, Vector2 rayDir, Vector2 circleCenter, float circleRadius, out Vector2 intsctPtA, out Vector2 intsctPtB ) => GetLineCircleIntersectionPointsFiltered( rayOrigin, rayDir, circleCenter, circleRadius, t => BoundsTestRay( t ), out intsctPtA, out intsctPtB );

		/// <summary>Returns the number of intersections and the points at which a ray and a circle intersect (if they exist)</summary>
		public static int RayCircle( Ray2D ray, Circle circle, out Vector2 intsctPtA, out Vector2 intsctPtB ) => GetLineCircleIntersectionPointsFiltered( ray.origin, ray.dir, circle.center, circle.radius, t => BoundsTestRay( t ), out intsctPtA, out intsctPtB );

		/// <summary>Returns the number of intersection points between ray and a circle</summary>
		public static int RayCircle( Vector2 rayOrigin, Vector2 rayDir, Vector2 circleCenter, float circleRadius ) => GetLineCircleIntersectionCountFiltered( rayOrigin, rayDir, circleCenter, circleRadius, t => BoundsTestRay( t ) );

		/// <summary>Returns the number of intersection points between ray and a circle</summary>
		public static int RayCircle( Ray2D ray, Circle circle ) => GetLineCircleIntersectionCountFiltered( ray.origin, ray.dir, circle.center, circle.radius, t => BoundsTestRay( t ) );

		#endregion

		#region LineSegments

		/// <summary>Returns the intersection point of two line segments (if there is one)</summary>
		/// <param name="aStart">Line segment A start</param>
		/// <param name="aEnd">Line segment A end</param>
		/// <param name="bStart">Line segment B start</param>
		/// <param name="bEnd">Line segment B end</param>
		/// <param name="intersectionPoint">The point at which they intersect</param>
		public static bool LineSegments( Vector2 aStart, Vector2 aEnd, Vector2 bStart, Vector2 bEnd, out Vector2 intersectionPoint ) {
			Vector2 aDir = aEnd - aStart;
			Vector2 bDir = bEnd - bStart;
			Vector2 aToB = bStart - aStart;
			if( GetLineLineTValues( aToB, aDir, bDir, out float tA, out float tB ) && BoundsTestLineSegment( tA ) && BoundsTestLineSegment( tB ) ) {
				intersectionPoint = aStart + tA * aDir;
				return true;
			}

			intersectionPoint = default;
			return false;
		}

		/// <summary>Returns the intersection point of two line segments (if there is one)</summary>
		/// <param name="intersectionPoint">The point at which they intersect</param>
		public static bool LineSegments( LineSegment2D a, LineSegment2D b, out Vector2 intersectionPoint ) => LineSegments( a.start, a.end, b.start, b.end, out intersectionPoint );

		/// <summary>Returns whether or not two line segments intersect</summary>
		/// <param name="aStart">Line segment A start</param>
		/// <param name="aEnd">Line segment A end</param>
		/// <param name="bStart">Line segment B start</param>
		/// <param name="bEnd">Line segment B end</param>
		public static bool LineSegments( Vector2 aStart, Vector2 aEnd, Vector2 bStart, Vector2 bEnd ) => GetLineLineTValues( bStart - aStart, aEnd - aStart, bEnd - bStart, out float tA, out float tB ) && BoundsTestLineSegment( tA ) && BoundsTestLineSegment( tB );

		/// <summary>Returns whether or not two line segments intersect</summary>
		public static bool LineSegments( LineSegment2D a, LineSegment2D b ) => LineSegments( a.start, a.end, b.start, b.end );

		#endregion

		#region LineSegment-Line

		/// <summary>Returns the intersection point of a line segment and a line (if there is one)</summary>
		/// <param name="lineSegStart">Line segment start</param>
		/// <param name="lineSegEnd">Line segment end</param>
		/// <param name="lineOrigin">Line start</param>
		/// <param name="lineDir">Line direction (does not have to be normalized)</param>
		/// <param name="intersectionPoint">The point at which they intersect</param>
		public static bool LineSegmentLine( Vector2 lineSegStart, Vector2 lineSegEnd, Vector2 lineOrigin, Vector2 lineDir, out Vector2 intersectionPoint ) {
			Vector2 aDir = lineSegEnd - lineSegStart;
			Vector2 aToB = lineOrigin - lineSegStart;

			if( GetLineLineTValue( aToB, aDir, lineDir, out float tA ) && BoundsTestLineSegment( tA ) ) {
				intersectionPoint = lineSegStart + tA * aDir;
				return true;
			}

			intersectionPoint = default;
			return false;
		}

		/// <summary>Returns the intersection point of a line segment and a line (if there is one)</summary>
		/// <param name="intersectionPoint">The point at which they intersect</param>
		public static bool LineSegmentLine( LineSegment2D lineSegment, Line2D line, out Vector2 intersectionPoint ) => LineSegmentLine( lineSegment.start, lineSegment.end, line.origin, line.dir, out intersectionPoint );

		/// <summary>Returns whether or not a line segment and a line intersect</summary>
		/// <param name="lineSegStart">Line segment start</param>
		/// <param name="lineSegEnd">Line segment end</param>
		/// <param name="lineOrigin">Line start</param>
		/// <param name="lineDir">Line direction (does not have to be normalized)</param>
		public static bool LineSegmentLine( Vector2 lineSegStart, Vector2 lineSegEnd, Vector2 lineOrigin, Vector2 lineDir ) => GetLineLineTValue( lineOrigin - lineSegStart, lineSegEnd - lineSegStart, lineDir, out float tA ) && BoundsTestLineSegment( tA );

		/// <summary>Returns whether or not a line segment and a line intersect</summary>
		public static bool LineSegmentLine( LineSegment2D lineSegment, Line2D line ) => GetLineLineTValue( line.origin - lineSegment.start, lineSegment.end - lineSegment.start, line.dir, out float tA ) && BoundsTestLineSegment( tA );

		#endregion

		#region LineSegment-Circle

		/// <summary>Returns the number of intersections and the points at which a line segment and a circle intersect (if they exist)</summary>
		public static int LineSegmentCircle( Vector2 lineStart, Vector2 lineEnd, Vector2 circleCenter, float circleRadius, out Vector2 intsctPtA, out Vector2 intsctPtB ) => GetLineCircleIntersectionPointsFiltered( lineStart, lineEnd - lineStart, circleCenter, circleRadius, t => BoundsTestLineSegment( t ), out intsctPtA, out intsctPtB );

		/// <summary>Returns the number of intersections and the points at which a line segment and a circle intersect (if they exist)</summary>
		public static int LineSegmentCircle( LineSegment2D lineSegment, Circle circle, out Vector2 intsctPtA, out Vector2 intsctPtB ) => GetLineCircleIntersectionPointsFiltered( lineSegment.start, lineSegment.end - lineSegment.start, circle.center, circle.radius, t => BoundsTestLineSegment( t ), out intsctPtA, out intsctPtB );

		/// <summary>Returns the number of intersection points between line segment and a circle</summary>
		public static int LineSegmentCircle( Vector2 lineStart, Vector2 lineEnd, Vector2 circleCenter, float circleRadius ) => GetLineCircleIntersectionCountFiltered( lineStart, lineEnd - lineStart, circleCenter, circleRadius, t => BoundsTestLineSegment( t ) );

		/// <summary>Returns the number of intersection points between line segment and a circle</summary>
		public static int LineSegmentCircle( LineSegment2D lineSegment, Circle circle ) => GetLineCircleIntersectionCountFiltered( lineSegment.start, lineSegment.end - lineSegment.start, circle.center, circle.radius, t => BoundsTestLineSegment( t ) );

		#endregion

		#region Lines

		/// <summary>Returns the intersection point of two infinite lines (if there is one)</summary>
		/// <param name="aOrigin">Line A origin</param>
		/// <param name="aDir">Line A direction (does not have to be normalized)</param>
		/// <param name="bOrigin">Line B origin</param>
		/// <param name="bDir">Line B direction (does not have to be normalized)</param>
		/// <param name="intersectionPoint">The point at which they intersect</param>
		public static bool Lines( Vector2 aOrigin, Vector2 aDir, Vector2 bOrigin, Vector2 bDir, out Vector2 intersectionPoint ) {
			Vector2 aToB = bOrigin - aOrigin;
			if( GetLineLineTValue( aToB, aDir, bDir, out float tA ) ) {
				intersectionPoint = aOrigin + tA * aDir;
				return true;
			}

			intersectionPoint = default;
			return false;
		}

		/// <summary>Returns the intersection point of two infinite lines (if there is one)</summary>
		/// <param name="intersectionPoint">The point at which they intersect</param>
		public static bool Lines( Line2D a, Line2D b, out Vector2 intersectionPoint ) => Lines( a.origin, a.dir, b.origin, b.dir, out intersectionPoint );

		#endregion

		#region Line-Circle

		/// <summary>Returns the points at which an infinite line and a circle intersect (if they exist)</summary>
		public static bool LineCircle( Vector2 lineStart, Vector2 lineDir, Vector2 circleCenter, float circleRadius, out Vector2 intsctPtA, out Vector2 intsctPtB ) => GetLineCircleIntersectionPointsUnfiltered( lineStart, lineDir, circleCenter, circleRadius, out intsctPtA, out intsctPtB ) > 0;

		/// <summary>Returns the points at which an infinite line and a circle intersect (if they exist)</summary>
		public static bool LineCircle( Line2D line, Circle circle, out Vector2 intsctPtA, out Vector2 intsctPtB ) => GetLineCircleIntersectionPointsUnfiltered( line.origin, line.dir, circle.center, circle.radius, out intsctPtA, out intsctPtB ) > 0;

		/// <summary>Returns whether or not an infinite line and a circle intersect</summary>
		public static bool LineCircle( Vector2 lineStart, Vector2 lineDir, Vector2 circleCenter, float circleRadius ) => GetLineCircleIntersectionState( lineStart, lineDir, circleCenter, circleRadius );

		/// <summary>Returns whether or not an infinite line and a circle intersect</summary>
		public static bool LineCircle( Line2D line, Circle circle ) => GetLineCircleIntersectionState( line.origin, line.dir, circle.center, circle.radius );

		#endregion

		/// <summary>Returns the two intersection points of two circles (if they exist)</summary>
		/// <param name="intsctPtA">The first intersection point</param>
		/// <param name="intsctPtB">The second intersection point</param>
		public static bool Circles( Circle a, Circle b, out Vector2 intsctPtA, out Vector2 intsctPtB ) {
			float distSq = DistanceSquared( a.center, b.center );
			float dist = Sqrt( distSq );
			bool differentPosition = dist > 0.00001f;
			float maxRad = Max( a.radius, b.radius );
			float minRad = Min( a.radius, b.radius );
			bool ringsTouching = Mathf.Abs( dist - maxRad ) < minRad;

			if( ringsTouching && differentPosition ) {
				float aRadSq = a.radius.Square();
				float bRadSq = b.radius.Square();
				float lateralOffset = ( distSq - bRadSq + aRadSq ) / ( 2 * dist );
				float normalOffset = ( 0.5f / dist ) * Sqrt( 4 * distSq * aRadSq - ( distSq - bRadSq + aRadSq ).Square() );
				Vector2 tangent = ( b.center - a.center ) / dist;
				Vector2 normal = tangent.Rotate90CCW();
				Vector2 chordCenter = a.center + tangent * lateralOffset;
				intsctPtA = chordCenter + normal * normalOffset;
				intsctPtB = chordCenter - normal * normalOffset;
				return true;
			}

			intsctPtA = intsctPtB = default;
			return false;
		}

		/// <summary>Returns whether or not two discs overlap</summary>
		public static bool Discs( Circle a, Circle b ) => DistanceSquared( a.center, b.center ) <= ( a.radius + b.radius ).Square();

		/// <summary>Returns whether or not two circles overlap</summary>
		public static bool Circles( Circle a, Circle b ) {
			float dist = Vector2.Distance( a.center, b.center );
			float maxRad = Max( a.radius, b.radius );
			float minRad = Min( a.radius, b.radius );
			return Mathf.Abs( dist - maxRad ) < minRad;
		}


		// internal
		const float PARALLEL_DETERMINANT_THRESHOLD = 0.00001f;
		static bool Parallel( Vector2 aDir, Vector2 bDir ) => Abs( Determinant( aDir, bDir ) ) < PARALLEL_DETERMINANT_THRESHOLD;

		// internal
		static bool GetLineLineTValues( Vector2 aToB, Vector2 aDir, Vector2 bDir, out float tA, out float tB ) {
			float d = Determinant( aDir, bDir );
			if( Abs( d ) < PARALLEL_DETERMINANT_THRESHOLD ) {
				tA = tB = default;
				return false;
			}

			tA = Determinant( aToB, bDir ) / d;
			tB = Determinant( aToB, aDir ) / d;
			return true;
		}

		// internal
		static bool GetLineLineTValue( Vector2 aToB, Vector2 aDir, Vector2 bDir, out float tA ) {
			float d = Determinant( aDir, bDir );
			if( Abs( d ) < PARALLEL_DETERMINANT_THRESHOLD ) {
				tA = default;
				return false;
			}

			tA = Determinant( aToB, bDir ) / d;
			return true;
		}

		// internal, based on https://stackoverflow.com/questions/1073336/circle-line-segment-collision-detection-algorithm
		static void GetLineCircleIntersectionValues( Vector2 aOrigin, Vector2 aDir, Vector2 circleCenter, float radius, out float a, out float b, out float discriminant ) {
			Vector2 circleToLineOrigin = aOrigin - circleCenter;
			a = Vector2.Dot( aDir, aDir ); // ray len sq
			b = 2 * Vector2.Dot( circleToLineOrigin, aDir );
			float c = Vector2.Dot( circleToLineOrigin, circleToLineOrigin ) - radius.Square();
			discriminant = b * b - 4 * a * c;
		}

		// internal
		static bool GetLineCircleTValues( Vector2 aOrigin, Vector2 aDir, Vector2 circleCenter, float radius, out float tA, out float tB ) {
			GetLineCircleIntersectionValues( aOrigin, aDir, circleCenter, radius, out float a, out float b, out float discriminant );
			if( discriminant > 0 ) {
				discriminant = Mathf.Sqrt( discriminant );
				tA = ( -b + discriminant ) / ( 2 * a );
				tB = ( -b - discriminant ) / ( 2 * a );
				return true;
			} else {
				tA = tB = 0f;
				return false;
			}
		}

		// internal, only useful for the Line-Circle since it doesn't check T values
		static bool GetLineCircleIntersectionState( Vector2 aOrigin, Vector2 aDir, Vector2 circleCenter, float radius ) {
			GetLineCircleIntersectionValues( aOrigin, aDir, circleCenter, radius, out float _, out float _, out float discriminant );
			return discriminant > 0;
		}

		// internal
		static int GetLineCircleIntersectionCountFiltered( Vector2 aOrigin, Vector2 aDir, Vector2 cCenter, float r, Func<float, bool> InValidRange ) {
			int pCount = 0;

			if( GetLineCircleTValues( aOrigin, aDir, cCenter, r, out float tA, out float tB ) ) {
				if( InValidRange( tA ) ) pCount++;
				if( InValidRange( tB ) ) pCount++;
			}

			return pCount;
		}

		// internal
		static int GetLineCircleIntersectionPointsFiltered( Vector2 aOrigin, Vector2 aDir, Vector2 cCenter, float r, Func<float, bool> InValidRange, out Vector2 pA, out Vector2 pB ) {
			int pCount = 0;

			void Check( float t, out Vector2 p ) {
				if( InValidRange( t ) ) {
					p = aOrigin + aDir * t;
					pCount++;
				} else p = default;
			}

			if( GetLineCircleTValues( aOrigin, aDir, cCenter, r, out float tA, out float tB ) ) {
				Check( tA, out pA );
				Check( tB, out pB );
			} else {
				pA = pB = default;
			}

			return pCount;
		}

		// internal
		static int GetLineCircleIntersectionPointsUnfiltered( Vector2 aOrigin, Vector2 aDir, Vector2 cCenter, float r, out Vector2 pA, out Vector2 pB ) {
			if( GetLineCircleTValues( aOrigin, aDir, cCenter, r, out float tA, out float tB ) ) {
				pA = aOrigin + aDir * tA;
				pB = aOrigin + aDir * tB;
				return 2;
			}

			pA = pB = default;
			return 0;
		}


	}

}