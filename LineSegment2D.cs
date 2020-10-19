// collected and expended upon by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

#if GODOT
using Godot;
#elif UNITY_5_3_OR_NEWER
using UnityEngine;
#endif

public static partial class Mathfs {
	public struct LineSegment2D {

		public Vector2 start;
		public Vector2 end;

		public LineSegment2D( Vector2 start, Vector2 end ) {
			this.start = start;
			this.end = end;
		}

		/// <summary>Calculates the length</summary>
#if GODOT
		public float Length => start.DistanceTo(end);
#elif UNITY_5_3_OR_NEWER
		public float Length => Vector2.Distance( start, end );
#endif

		/// <summary>Returns the perpendicular bisector. Note: the returned normal is not normalized to save performance. Use Bisector() if you want to make sure it is normalized</summary>
		public Line2D BisectorFast() => GetBisectorFast( start, end );

		/// <summary>Returns the perpendicular bisector</summary>
		public Line2D Bisector() => GetBisector( start, end );

		/// <summary>Returns the perpendicular bisector of the input line segment. Note: the returned normal is not normalized to save performance. Use GetBisector() if you want to make sure it is normalized</summary>
		/// <param name="startPoint">Starting point of the line segment</param>
		/// <param name="endPoint">Endpoint of the line segment</param>
		public static Line2D GetBisectorFast( Vector2 startPoint, Vector2 endPoint ) {
			return new Line2D( ( startPoint + endPoint ) * 0.5f, ( startPoint - endPoint ).Rotate90CCW() );
		}

		/// <summary>Returns the perpendicular bisector of the input line segment</summary>
		/// <param name="startPoint">Starting point of the line segment</param>
		/// <param name="endPoint">Endpoint of the line segment</param>
		public static Line2D GetBisector( Vector2 startPoint, Vector2 endPoint ) {
			Line2D line = GetBisectorFast( startPoint, endPoint );
#if GODOT
			line.dir = line.dir.Normalized();
#elif UNITY_5_3_OR_NEWER
			line.dir = line.dir.normalized;
#endif
			return line;
		}

	}
}
