// Collected and expanded upon to by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using System.Collections.Generic;
using UnityEngine;
using static Freya.Mathfs;

namespace Freya {

	// Polygon math
	public struct Polygon {
		public static bool IsClockwise( IReadOnlyList<Vector2> pts ) => SignedArea( pts ) > 0;
		public static float Area( IReadOnlyList<Vector2> pts ) => Mathf.Abs( SignedArea( pts ) );

		public static float SignedArea( IReadOnlyList<Vector2> pts ) {
			int count = pts.Count;
			float sum = 0f;
			for( int i = 0; i < count; i++ ) {
				Vector2 a = pts[i];
				Vector2 b = pts[( i + 1 ) % count];
				sum += ( b.x - a.x ) * ( b.y + a.y );
			}

			return sum * 0.5f;
		}

		public static bool Contains( IReadOnlyList<Vector2> polygon, Vector2 point ) => WindingNumber( polygon, point ) != 0;

		// modified version of the code from here:
		// http://softsurfer.com/Archive/algorithm_0103/algorithm_0103.htm
		// /---/
		// Copyright 2000 softSurfer, 2012 Dan Sunday. This code may be freely used and modified for any purpose providing that this copyright notice is included with it. SoftSurfer makes no warranty for this code, and cannot be held liable for any real or imagined damage resulting from its use. Users of this code must verify correctness for their application.
		public static int WindingNumber( IReadOnlyList<Vector2> polygon, Vector2 point ) {
			int winding = 0;
			float IsLeft( Vector2 a, Vector2 b, Vector2 p ) => SignWithZero( Determinant( a.To( p ), a.To( b ) ) );

			int count = polygon.Count;
			for( int i = 0; i < count; i++ ) {
				int iNext = ( i + 1 ) % count;
				if( polygon[i].y <= point.y ) {
					if( polygon[iNext].y > point.y && IsLeft( polygon[i], polygon[iNext], point ) > 0 )
						winding--;
				} else {
					if( polygon[iNext].y <= point.y && IsLeft( polygon[i], polygon[iNext], point ) < 0 )
						winding++;
				}
			}

			return winding;
		}
	}

}