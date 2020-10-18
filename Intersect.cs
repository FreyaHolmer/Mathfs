// collected and expended upon by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using UnityEngine;

public static partial class Mathfs {
	public static class Intersect {

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
			if( GetLineLineTValues( aToB, aDir, bDir, out float tA, out float tB ) ) {
				if( tA.Within( 0, 1 ) && tB.Within( 0, 1 ) ) {
					intersectionPoint = aStart + tA * aDir;
					return true;
				}
			}

			intersectionPoint = default;
			return false;
		}

		/// <summary>Returns the intersection point of two line segments (if there is one)</summary>
		/// <param name="intersectionPoint">The point at which they intersect</param>
		public static bool LineSegments( LineSegment2D a, LineSegment2D b, out Vector2 intersectionPoint ) => LineSegments( a.start, a.end, b.start, b.end, out intersectionPoint );

		/// <summary>Returns the intersection point of two line segments (if there is one)</summary>
		/// <param name="aStart">Line segment A start</param>
		/// <param name="aEnd">Line segment A end</param>
		/// <param name="bOrigin">Line B start</param>
		/// <param name="bDir">Line B direction (does not have to be normalized)</param>
		/// <param name="intersectionPoint">The point at which they intersect</param>
		public static bool LineSegmentLine( Vector2 aStart, Vector2 aEnd, Vector2 bOrigin, Vector2 bDir, out Vector2 intersectionPoint ) {
			Vector2 aDir = aEnd - aStart;
			Vector2 aToB = bOrigin - aStart;

			if( GetLineLineTValue( aToB, aDir, bDir, out float tA ) ) {
				if( tA.Within( 0, 1 ) ) {
					intersectionPoint = aStart + tA * aDir;
					return true;
				}
			}

			intersectionPoint = default;
			return false;
		}

		/// <summary>Returns the intersection point of two line segments (if there is one)</summary>
		/// <param name="intersectionPoint">The point at which they intersect</param>
		public static bool LineSegmentLine( LineSegment2D lineSegment, Line2D line, out Vector2 intersectionPoint ) => LineSegmentLine( lineSegment.start, lineSegment.end, line.origin, line.dir, out intersectionPoint );


		// internal
		static bool GetLineLineTValues( Vector2 aToB, Vector2 aDir, Vector2 bDir, out float tA, out float tB ) {
			float d = Determinant( aDir, bDir );
			if( Abs( d ) < 0.00001f ) {
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
			if( Abs( d ) < 0.00001f ) {
				tA = default;
				return false;
			}

			tA = Determinant( aToB, bDir ) / d;
			return true;
		}


	}

}