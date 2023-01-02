// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Freya {

	/// <summary>A shared interface between Ray2D, Line2D and LineSegment2D</summary>
	public interface ILinear2D {

		/// <summary>The origin of this linear 2D object (Ray2D, Line2D or LineSegment2D)</summary>
		Vector2 Origin { get; }

		/// <summary>The direction of this linear 2D object (Ray2D, Line2D or LineSegment2D). Note: this vector may or may not be normalized</summary>
		Vector2 Dir { get; }

		/// <summary>Returns whether or not this t-value is within this linear 2D object (Ray2D, Line2D or LineSegment2D)</summary>
		/// <param name="t">The t-value along the linear 2D object</param>
		bool IsValidTValue( float t );

		/// <summary>Clamps the value into the range of this linear 2D object (Ray2D, Line2D or LineSegment2D)</summary>
		/// <param name="t">The t-value along the linear 2D object</param>
		float ClampTValue( float t );
	}

	public static class ExtILinear2D {

		const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

		/// <summary>Gets a point along this line</summary>
		/// <param name="linear">The linear object to get a point along (Ray2D, Line2D or LineSegment2D)</param>
		/// <param name="t">The t-value along the ray to get the point of. If the ray direction is normalized, t is equivalent to distance</param>
		[MethodImpl( INLINE )] public static Vector2 GetPoint<T>( this T linear, float t ) where T : ILinear2D => linear.Origin + linear.Dir * t;

		/// <summary>Returns the t-value of a point projected onto this line</summary>
		/// <param name="linear">The linear object to project onto (Ray2D, Line2D or LineSegment2D)</param>
		/// <param name="point">The point to use when projecting</param>
		[MethodImpl( INLINE )] public static float ProjectPointTValue<T>( this T linear, Vector2 point ) where T : ILinear2D => linear.ClampTValue( Line2D.ProjectPointToLineTValue( linear.Origin, linear.Dir, point ) );

		/// <summary>Projects a point onto this line</summary>
		/// <param name="linear">The linear object to project onto (Ray2D, Line2D or LineSegment2D)</param>
		/// <param name="point">The point to project</param>
		[MethodImpl( INLINE )] public static Vector2 ProjectPoint<T>( this T linear, Vector2 point ) where T : ILinear2D => linear.GetPoint( linear.ProjectPointTValue( point ) );

		/// <summary>The shortest distance from this line to a point</summary>
		/// <param name="linear">The linear object to check distance from (Ray2D, Line2D or LineSegment2D)</param>
		/// <param name="point">The point to check the distance to</param>
		[MethodImpl( INLINE )] public static float Distance<T>( this T linear, Vector2 point ) where T : ILinear2D => MathF.Sqrt( DistanceSqr( linear, point ) );

		/// <summary>The shortest squared distance from this line to a point</summary>
		/// <param name="linear">The linear object to check distance from (Ray2D, Line2D or LineSegment2D)</param>
		/// <param name="point">The point to check the distance to</param>
		[MethodImpl( INLINE )] public static float DistanceSqr<T>( this T linear, Vector2 point ) where T : ILinear2D => ( point - linear.ProjectPoint( point ) ).sqrMagnitude;

		#region Intersection Tests

		/// <summary>Returns whether or not this intersects another linear object (Ray2D, Line2D or LineSegment2D)</summary>
		/// <param name="linear">The linear object to test intersection with (Ray2D, Line2D or LineSegment2D)</param>
		/// <param name="other">The other linear object to test intersection against (Ray2D, Line2D or LineSegment2D)</param>
		[MethodImpl( INLINE )] public static bool Intersects<T, U>( this T linear, U other ) where T : ILinear2D where U : ILinear2D => IntersectionTest.Linear( linear, other );

		/// <summary>Returns whether or not this line intersects a circle</summary>
		/// <param name="linear">The linear object to test intersection with (Ray2D, Line2D or LineSegment2D)</param>
		/// <param name="circle">The circle to test intersection against</param>
		[MethodImpl( INLINE )] public static bool Intersects<T>( this T linear, Circle2D circle ) where T : ILinear2D => IntersectionTest.LinearCircleIntersects( linear, circle );

		#endregion

		#region Intersection Points

		/// <summary>Returns whether or not this line intersects another linear object (Ray2D, Line2D or LineSegment2D), and returns the point (if any)</summary>
		/// <param name="linear">The linear object to test intersection with (Ray2D, Line2D or LineSegment2D)</param>
		/// <param name="other">The other linear object to test intersection against (Ray2D, Line2D or LineSegment2D)</param>
		/// <param name="intersectionPoint">The point at which they intersect</param>
		[MethodImpl( INLINE )] public static bool Intersect<T, U>( this T linear, U other, out Vector2 intersectionPoint ) where T : ILinear2D where U : ILinear2D => IntersectionTest.LinearIntersectionPoint( linear, other, out intersectionPoint );

		/// <summary>Returns the intersections this line has with a circle (if any)</summary>
		/// <param name="linear">The linear object to test intersection with (Ray2D, Line2D or LineSegment2D)</param>
		/// <param name="circle">The circle to test intersection against</param>
		[MethodImpl( INLINE )] public static ResultsMax2<Vector2> Intersect<T>( this T linear, Circle2D circle ) where T : ILinear2D => IntersectionTest.LinearCircleIntersectionPoints( linear, circle );

		#endregion


	}

}