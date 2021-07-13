// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using static Freya.Mathfs;

namespace Freya {

	// 2D line math
	/// <summary>A structure representing an infinitely long 2D line</summary>
	[Serializable] public struct Line2D : ILinear2D {

		const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

		/// <summary>The origin of this line</summary>
		public Vector2 origin;

		/// <summary>The direction of the ray. Note: Line2D allows non-normalized direction vectors</summary>
		public Vector2 dir;

		/// <summary>Returns a normalized version of this line. Normalized lines ensure t-values correspond to distance</summary>
		public Line2D Normalized => new Line2D( origin, dir );

		/// <summary>Creates an infinitely long 2D line, given an origin and a direction</summary>
		/// <param name="origin">The origin of the line</param>
		/// <param name="dir">The direction of the line. It does not have to be normalized, but if it is, the t-value when sampling will correspond to distance along the ray</param>
		public Line2D( Vector2 origin, Vector2 dir ) => ( this.origin, this.dir ) = ( origin, dir );
		}

		/// <summary>Gets a point along the line</summary>
		/// <param name="t">The t-value along the ray to get the point of. If the ray direction is normalized, t is equivalent to distance</param>
		[MethodImpl( INLINE )] public Vector2 GetPoint( float t ) => origin + dir * t;
		
		/// <summary>Returns the t-value of a point projected onto this line</summary>
		/// <param name="point">The point to use when projecting</param>
		[MethodImpl( INLINE )] public float ProjectPointTValue( Vector2 point ) => ProjectPointToLineTValue( origin, dir, point );
		
		/// <summary>Projects a point onto this line</summary>
		/// <param name="point">The point to project</param>
		[MethodImpl( INLINE )] public Vector2 ProjectPoint( Vector2 point ) => GetPoint( ProjectPointTValue( point ) );

		/// <summary>Projects a point onto an infinite line, returning the t-value along the line</summary>
		/// <param name="lineOrigin">Line origin</param>
		/// <param name="lineDir">Line direction (does not have to be normalized)</param>
		/// <param name="point">The point to project onto the line</param>
		[MethodImpl( INLINE )] public static float ProjectPointToLineTValue( Vector2 lineOrigin, Vector2 lineDir, Vector2 point ) {
			return Vector2.Dot( lineDir, point - lineOrigin ) / Vector2.Dot( lineDir, lineDir );
		}

		/// <summary>Projects a point onto an infinite line</summary>
		/// <param name="lineOrigin">Line origin</param>
		/// <param name="lineDir">Line direction (does not have to be normalized)</param>
		/// <param name="point">The point to project onto the line</param>
		[MethodImpl( INLINE )] public static Vector2 ProjectPointToLine( Vector2 lineOrigin, Vector2 lineDir, Vector2 point ) {
			return lineOrigin + lineDir * ProjectPointToLineTValue( lineOrigin, lineDir, point );
		}

		/// <summary>Projects a point onto an infinite line</summary>
		/// <param name="line">Line to project onto</param>
		/// <param name="point">The point to project onto the line</param>
		[MethodImpl( INLINE )] public static Vector2 ProjectPointToLine( Line2D line, Vector2 point ) => ProjectPointToLine( line.origin, line.dir, point );

		/// <summary>Returns the signed distance to a 2D plane</summary>
		/// <param name="planeOrigin">Plane origin</param>
		/// <param name="planeNormal">Plane normal (has to be normalized for a true distance)</param>
		/// <param name="point">The point to use when checking distance to the plane</param>
		[MethodImpl( INLINE )] public static float PointToPlaneSignedDistance( Vector2 planeOrigin, Vector2 planeNormal, Vector2 point ) {
			return Vector2.Dot( point - planeOrigin, planeNormal );
		}

		/// <summary>Returns the distance to a 2D plane</summary>
		/// <param name="planeOrigin">Plane origin</param>
		/// <param name="planeNormal">Plane normal (has to be normalized for a true distance)</param>
		/// <param name="point">The point to use when checking distance to the plane</param>
		[MethodImpl( INLINE )] public static float PointToPlaneDistance( Vector2 planeOrigin, Vector2 planeNormal, Vector2 point ) {
			return Abs( PointToPlaneSignedDistance( planeOrigin, planeNormal, point ) );
		}

		#region Intersection tests

		/// <summary>Returns whether or not this line intersects a ray</summary>
		/// <param name="ray">The ray to test intersection against</param>
		[MethodImpl( INLINE )] public bool Intersects( Ray2D ray ) => IntersectionTest.Linear( this, ray );

		/// <summary>Returns whether or not this line intersects a line segment</summary>
		/// <param name="lineSeg">The line segment to test intersection against</param>
		[MethodImpl( INLINE )] public bool Intersects( LineSegment2D lineSeg ) => IntersectionTest.Linear( this, lineSeg );

		/// <summary>Returns whether or not this line intersects another line</summary>
		/// <param name="line">The line to test intersection against</param>
		[MethodImpl( INLINE )] public bool Intersects( Line2D line ) => IntersectionTest.Linear( this, line );

		/// <summary>Returns whether or not this line intersects a circle</summary>
		/// <param name="circle">The circle to test intersection against</param>
		[MethodImpl( INLINE )] public bool Intersects( Circle2D circle ) => IntersectionTest.LinearCircleIntersects( this, circle );

		/// <summary>Returns whether or not this line intersects a ray, and returns the point (if any)</summary>
		/// <param name="ray">The ray to test intersection against</param>
		/// <param name="intersectionPoint">The point at which they intersect</param>
		[MethodImpl( INLINE )] public bool Intersect( Ray2D ray, out Vector2 intersectionPoint ) => IntersectionTest.LinearIntersectionPoint( this, ray, out intersectionPoint );

		/// <summary>Returns whether or not this line intersects a line segment, and returns the point (if any)</summary>
		/// <param name="lineSeg">The line segment to test intersection against</param>
		/// <param name="intersectionPoint">The point at which they intersect</param>
		[MethodImpl( INLINE )] public bool Intersect( LineSegment2D lineSeg, out Vector2 intersectionPoint ) => IntersectionTest.LinearIntersectionPoint( this, lineSeg, out intersectionPoint );

		/// <summary>Returns whether or not this line intersects another line, and returns the point (if any)</summary>
		/// <param name="line">The line to test intersection against</param>
		/// <param name="intersectionPoint">The point at which they intersect</param>
		[MethodImpl( INLINE )] public bool Intersect( Line2D line, out Vector2 intersectionPoint ) => IntersectionTest.LinearIntersectionPoint( this, line, out intersectionPoint );

		/// <summary>Returns the intersections this line has with a circle (if any)</summary>
		/// <param name="circle">The circle to test intersection against</param>
		[MethodImpl( INLINE )] public ResultsMax2<Vector2> Intersect( Circle2D circle ) => IntersectionTest.LinearCircleIntersectionPoints( this, circle );

		#endregion

		#region Interface stuff for generic line tests

		Vector2 ILinear2D.Origin {
			[MethodImpl( INLINE )] get => origin;
		}
		Vector2 ILinear2D.Dir {
			[MethodImpl( INLINE )] get => dir;
		}

		[MethodImpl( INLINE )]
		bool ILinear2D.IsValidTValue( float t ) => true; // just always valid uwu

		#endregion


	}

}