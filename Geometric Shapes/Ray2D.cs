// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Freya {

	/// <summary>Similar to Unity's Ray2D, except this one allows you to not normalize the direction
	/// which saves performance as well as allows you to work at different scales</summary>
	[Serializable] public struct Ray2D : ILinear2D {

		const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

		/// <summary>The origin of the ray</summary>
		public Vector2 origin;

		/// <summary>The direction of the ray. Note: Ray2D allows non-normalized direction vectors</summary>
		public Vector2 dir;

		/// <summary>Returns a normalized version of this ray. Normalized rays ensure t-values correspond to distance</summary>
		public Line2D Normalized => new Line2D( origin, dir );

		/// <summary>Creates a 2D Ray. Note: direction does not have to be normalized, but if it is, the t-value will correspond to distance along the ray</summary>
		/// <param name="origin">The origin of the ray</param>
		/// <param name="dir">The direction of the ray. It does not have to be normalized, but if it is, the t-value when sampling will correspond to distance along the ray</param>

		/// <summary>Gets a point along the ray</summary>
		/// <param name="t">The t-value along the ray to get the point of. If the ray direction is normalized, t is equivalent to distance</param>
		[MethodImpl( INLINE )] public Vector2 GetPoint( float t ) => origin + dir * t;

		/// <summary>Returns the t-value of a point projected onto this ray</summary>
		/// <param name="point">The point to use when projecting</param>
		[MethodImpl( INLINE )] public float ProjectPointTValue( Vector2 point ) => Line2D.ProjectPointToLineTValue( origin, dir, point ).AtLeast( 0 );

		/// <summary>Projects a point onto this ray</summary>
		/// <param name="point">The point to project</param>
		[MethodImpl( INLINE )] public Vector2 ProjectPoint( Vector2 point ) => GetPoint( ProjectPointTValue( point ) );
		public Ray2D( Vector2 origin, Vector2 dir ) => ( this.origin, this.dir ) = ( origin, dir );

		/// <summary>Implicitly casts a Unity ray to a Mathfs ray</summary>
		/// <param name="ray">The ray to cast to a Unity ray</param>
		public static implicit operator Ray2D( Ray ray ) => new Ray2D( ray.origin, ray.direction );

		/// <summary>Implicitly casts a Mathfs ray to a Unity ray</summary>
		/// <param name="ray">The ray to cast to a Mathfs ray</param>
		public static implicit operator UnityEngine.Ray2D( Ray2D ray ) => new UnityEngine.Ray2D( ray.origin, ray.dir );

		#region Intersection tests

		/// <summary>Returns whether or not this ray intersects another ray</summary>
		/// <param name="ray">The ray to test intersection against</param>
		[MethodImpl( INLINE )] public bool Intersects( Ray2D ray ) => IntersectionTest.Linear( this, ray );

		/// <summary>Returns whether or not this ray intersects a line segment</summary>
		/// <param name="lineSeg">The line segment to test intersection against</param>
		[MethodImpl( INLINE )] public bool Intersects( LineSegment2D lineSeg ) => IntersectionTest.Linear( this, lineSeg );

		/// <summary>Returns whether or not this ray intersects a line</summary>
		/// <param name="line">The line to test intersection against</param>
		[MethodImpl( INLINE )] public bool Intersects( Line2D line ) => IntersectionTest.Linear( this, line );

		/// <summary>Returns whether or not this ray intersects a circle</summary>
		/// <param name="circle">The circle to test intersection against</param>
		[MethodImpl( INLINE )] public bool Intersects( Circle2D circle ) => IntersectionTest.LinearCircleIntersects( this, circle );

		/// <summary>Returns whether or not this ray intersects another ray, and returns the point (if any)</summary>
		/// <param name="ray">The ray to test intersection against</param>
		/// <param name="intersectionPoint">The point at which they intersect</param>
		[MethodImpl( INLINE )] public bool Intersect( Ray2D ray, out Vector2 intersectionPoint ) => IntersectionTest.LinearIntersectionPoint( this, ray, out intersectionPoint );

		/// <summary>Returns whether or not this ray intersects a line segment, and returns the point (if any)</summary>
		/// <param name="lineSeg">The line segment to test intersection against</param>
		/// <param name="intersectionPoint">The point at which they intersect</param>
		[MethodImpl( INLINE )] public bool Intersect( LineSegment2D lineSeg, out Vector2 intersectionPoint ) => IntersectionTest.LinearIntersectionPoint( this, lineSeg, out intersectionPoint );

		/// <summary>Returns whether or not this ray intersects a line, and returns the point (if any)</summary>
		/// <param name="line">The line to test intersection against</param>
		/// <param name="intersectionPoint">The point at which they intersect</param>
		[MethodImpl( INLINE )] public bool Intersect( Line2D line, out Vector2 intersectionPoint ) => IntersectionTest.LinearIntersectionPoint( this, line, out intersectionPoint );

		/// <summary>Returns the intersections this ray has with a circle (if any)</summary>
		/// <param name="circle">The circle to test intersection against</param>
		[MethodImpl( INLINE )] public ResultsMax2<Vector2> Intersect( Circle2D circle ) => IntersectionTest.LinearCircleIntersectionPoints( this, circle );

		#endregion

		#region Internal interface stuff for generic line tests

		Vector2 ILinear2D.Origin {
			[MethodImpl( INLINE )] get => origin;
		}
		Vector2 ILinear2D.Dir {
			[MethodImpl( INLINE )] get => dir;
		}
		[MethodImpl( INLINE )] bool ILinear2D.IsValidTValue( float t ) => t >= 0;

		#endregion

	}

}