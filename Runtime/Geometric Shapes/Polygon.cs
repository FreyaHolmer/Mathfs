// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using System;
using System.Collections.Generic;
using UnityEngine;
using static Freya.Mathfs;

namespace Freya {

	/// <summary>Polygon with various math functions to test if a point is inside, calculate area, etc.</summary>
	public class Polygon {

		/// <summary>The points in this polygon</summary>
		public IReadOnlyList<Vector2> points;

		/// <summary>Creates a new 2D polygon</summary>
		/// <param name="points">The points in the polygon</param>
		public Polygon( IReadOnlyList<Vector2> points ) => this.points = points;

		/// <summary>Get a point by index. Indices cannot be out of range, as they will wrap/cycle in the polygon</summary>
		/// <param name="i">The index of the point</param>
		public Vector2 this[ int i ] => points[i.Mod( Count )];

		/// <summary>The number of points in this polygon</summary>
		public int Count => points.Count;

		/// <summary>Returns whether or not this polygon is defined clockwise</summary>
		public bool IsClockwise => SignedArea > 0;

		/// <summary>Returns the area of this polygon</summary>
		public float Area => MathF.Abs( SignedArea );

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
					totalDist += MathF.Sqrt( dx * dx + dy * dy ); // unrolled for speed
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
					xMin = MathF.Min( xMin, p.x );
					xMax = MathF.Max( xMax, p.x );
					yMin = MathF.Min( yMin, p.y );
					yMax = MathF.Max( yMax, p.y );
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
		/// <summary>Returns the winding number for this polygon, around a given point</summary>
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

		/// <summary>Returns the resulting polygons when clipping this polygon by a line</summary>
		/// <param name="line">The line/plane to clip by. Points on its left side will be kept</param>
		/// <param name="clippedPolygons">The resulting array of clipped polygons (if any)</param>
		public PolygonClipper.ResultState Clip( Line2D line, out List<Polygon> clippedPolygons ) => PolygonClipper.Clip( this, line, out clippedPolygons );

		public Polygon GetMiterPolygon( float offset ) {
			List<Vector2> miterPts = new List<Vector2>();

			Line2D GetMiterLine( int i ) {
				Vector2 tangent = ( this[i + 1] - this[i] ).normalized;
				Vector2 normal = tangent.Rotate90CCW();
				return new Line2D( this[i] + normal * offset, tangent );
			}

			// Line2D prev = GetMiterLine( -1 );
			for( int i = 0; i < Count; i++ ) {
				Line2D line = GetMiterLine( i );
				Line2D line2 = GetMiterLine( i + 1 );
				if( line.Intersect( line2, out Vector2 pt ) )
					miterPts.Add( pt );
				else {
					Debug.LogError( $"{line.origin},{line.dir}\n{line2.origin},{line2.dir}\nPoints:{string.Join( '\n', points )}" );
					throw new Exception( "Line intersection failed" );
				}
				// prev = line;
			}

			return new Polygon( miterPts );
		}

	}

}