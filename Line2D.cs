// collected and expended upon by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using UnityEngine;
using static Freya.Mathfs;

namespace Freya {

	// 2D line math
	public struct Line2D {

		public Vector2 origin;
		public Vector2 dir;

		public Line2D( Vector2 origin, Vector2 dir ) {
			this.origin = origin;
			this.dir = dir;
		}

		public Vector2 GetPoint( float t ) => origin + t * dir;

		/// <summary>Projects a point onto an infinite line</summary>
		/// <param name="lineOrigin">Line origin</param>
		/// <param name="lineDir">Line direction (does not have to be normalized)</param>
		/// <param name="point">The point to project onto the line</param>
		public static Vector2 ProjectPointToLine( Vector2 lineOrigin, Vector2 lineDir, Vector2 point ) {
			Vector2 coord = point - lineOrigin;
			float t = Vector2.Dot( lineDir, coord ) / Vector2.Dot( lineDir, lineDir );
			return lineOrigin + lineDir * t;
		}

		/// <summary>Projects a point onto an infinite line</summary>
		/// <param name="line">Line to project onto</param>
		/// <param name="point">The point to project onto the line</param>
		public static Vector2 ProjectPointToLine( Line2D line, Vector2 point ) => ProjectPointToLine( line.origin, line.dir, point );

		/// <summary>Returns the signed distance to a 2D plane</summary>
		/// <param name="planeOrigin">Plane origin</param>
		/// <param name="planeNormal">Plane normal (has to be normalized for a true distance)</param>
		/// <param name="point">The point to use when checking distance to the plane</param>
		public static float PointToPlaneSignedDistance( Vector2 planeOrigin, Vector2 planeNormal, Vector2 point ) {
			return Vector2.Dot( point - planeOrigin, planeNormal );
		}

		/// <summary>Returns the distance to a 2D plane</summary>
		/// <param name="planeOrigin">Plane origin</param>
		/// <param name="planeNormal">Plane normal (has to be normalized for a true distance)</param>
		/// <param name="point">The point to use when checking distance to the plane</param>
		public static float PointToPlaneDistance( Vector2 planeOrigin, Vector2 planeNormal, Vector2 point ) {
			return Abs( PointToPlaneSignedDistance( planeOrigin, planeNormal, point ) );
		}


	}

}