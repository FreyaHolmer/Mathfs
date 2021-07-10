// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using System.Collections.Generic;
using UnityEngine;
using static Freya.Mathfs;

namespace Freya {

	/// <summary>Polygon with various math functions to test if a point is inside, calculate area, etc.</summary>
	public struct Polygon {

		/// <summary>The points in this polygon</summary>
		public IReadOnlyList<Vector2> points;

		/// <summary>Creates a new 2D polygon</summary>
		/// <param name="points">The points in the polygon</param>
		public Polygon( IReadOnlyList<Vector2> points ) => this.points = points;

		/// <summary>Returns whether or not this polygon is defined clockwise</summary>
		public bool IsClockwise => SignedArea > 0;

		/// <summary>Returns the area of this polygon</summary>
		public float Area => Mathf.Abs( SignedArea );

		/// <summary>Returns the signed area of this polygon</summary>
		public float SignedArea {
			get {
				int count = points.Count;
				float sum = 0f;
				for( int i = 0; i < count; i++ ) {
					Vector2 a = points[i];
					Vector2 b = points[( i + 1 ) % count];
					sum += ( b.x - a.x ) * ( b.y + a.y );
				}

				return sum * 0.5f;
			}
		}

		/// <summary>Returns the length of the perimeter of the polygon</summary>
		public float Perimeter {
			get {
				int count = points.Count;
				float totalDist = 0f;
				for( int i = 0; i < count; i++ ) {
					Vector2 a = points[i];
					Vector2 b = points[( i + 1 ) % count];
					float dx = a.x - b.x;
					float dy = a.y - b.y;
					totalDist += Mathf.Sqrt( dx * dx + dy * dy ); // unrolled for speed
				}

				return totalDist;
			}
		}

		/// <summary>Returns the axis-aligned bounding box of this polygon</summary>
		public Rect Bounds {
			get {
				int count = points.Count;
				Vector2 p = points[0];
				float xMin = p.x, xMax = p.x, yMin = p.y, yMax = p.y;
				for( int i = 1; i < count; i++ ) {
					p = points[i];
					xMin = Mathf.Min( xMin, p.x );
					xMax = Mathf.Max( xMax, p.x );
					yMin = Mathf.Min( yMin, p.y );
					yMax = Mathf.Max( yMax, p.y );
				}

				return new Rect( xMin, yMin, xMax - xMin, yMax - yMin );
			}
		}

		/// <summary>Returns whether or not a point is inside the polygon</summary>
		/// <param name="point">The point to test and see if it's inside</param>
		public bool Contains( Vector2 point ) => WindingNumber( point ) != 0;

		// modified version of the code from here:
		// http://softsurfer.com/Archive/algorithm_0103/algorithm_0103.htm
		// Copyright 2000 softSurfer, 2012 Dan Sunday. This code may be freely used and modified for any purpose providing that this copyright notice is included with it. SoftSurfer makes no warranty for this code, and cannot be held liable for any real or imagined damage resulting from its use. Users of this code must verify correctness for their application.
		/// <summary>Returns the winding number for this polygon, given a point</summary>
		/// <param name="point">The point to check winding around</param>
		public int WindingNumber( Vector2 point ) {
			int winding = 0;
			float IsLeft( Vector2 a, Vector2 b, Vector2 p ) => SignWithZero( Determinant( a.To( p ), a.To( b ) ) );

			int count = points.Count;
			for( int i = 0; i < count; i++ ) {
				int iNext = ( i + 1 ) % count;
				if( points[i].y <= point.y ) {
					if( points[iNext].y > point.y && IsLeft( points[i], points[iNext], point ) > 0 )
						winding--;
				} else {
					if( points[iNext].y <= point.y && IsLeft( points[i], points[iNext], point ) < 0 )
						winding++;
				}
			}

			return winding;
		}
	}

}