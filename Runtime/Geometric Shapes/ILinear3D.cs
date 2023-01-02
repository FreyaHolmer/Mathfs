// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Freya {

	/// <summary>A shared interface between Ray3D, Line3D and LineSegment3D</summary>
	public interface ILinear3D {

		/// <summary>The origin of this linear 3D object (Ray3D, Line3D or LineSegment3D)</summary>
		Vector3 Origin { get; }

		/// <summary>The direction of this linear 3D object (Ray3D, Line3D or LineSegment3D). Note: this vector may or may not be normalized</summary>
		Vector3 Dir { get; }

		/// <summary>Returns whether or not this t-value is within this linear 3D object (Ray3D, Line3D or LineSegment3D)</summary>
		/// <param name="t">The t-value along the linear 3D object</param>
		bool IsValidTValue( float t );

		/// <summary>Clamps the value into the range of this linear 3D object (Ray3D, Line3D or LineSegment3D)</summary>
		/// <param name="t">The t-value along the linear 3D object</param>
		float ClampTValue( float t );
	}

	public static class ExtILinear3D {

		const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

		/// <summary>Gets a point along this line</summary>
		/// <param name="linear">The linear object to get a point along (Ray3D, Line3D or LineSegment3D)</param>
		/// <param name="t">The t-value along the ray to get the point of. If the ray direction is normalized, t is equivalent to distance</param>
		[MethodImpl( INLINE )] public static Vector3 GetPoint<T>( this T linear, float t ) where T : ILinear3D => linear.Origin + linear.Dir * t;

		/// <summary>Returns the t-value of a point projected onto this line</summary>
		/// <param name="linear">The linear object to project onto (Ray3D, Line3D or LineSegment3D)</param>
		/// <param name="point">The point to use when projecting</param>
		[MethodImpl( INLINE )] public static float ProjectPointTValue<T>( this T linear, Vector3 point ) where T : ILinear3D => linear.ClampTValue( Line3D.ProjectPointToLineTValue( linear.Origin, linear.Dir, point ) );
		
		/// <summary>The t-values at the shortest squared distance between two linear objects</summary>
		/// <param name="linear">The linear object to check distance from (Ray3D, Line3D or LineSegment3D)</param>
		/// <param name="other">The other linear object to check the distance to</param>
		[MethodImpl( INLINE )] public static (float,float) LinearTValues<A, B>( this A linear, B other ) where A : ILinear3D where B : ILinear3D {
			( float tA, float tB ) = Line3D.ClosestPointBetweenLinesTValues( linear.Origin, linear.Dir, other.Origin, other.Dir );
			tA = linear.ClampTValue( tA );
			tB = other.ClampTValue( tB );
			return ( tA, tB );
		}

		/// <summary>Projects a point onto this line</summary>
		/// <param name="linear">The linear object to project onto (Ray3D, Line3D or LineSegment3D)</param>
		/// <param name="point">The point to project</param>
		[MethodImpl( INLINE )] public static Vector3 ProjectPoint<T>( this T linear, Vector3 point ) where T : ILinear3D => linear.GetPoint( linear.ProjectPointTValue( point ) );

		/// <summary>The shortest distance from this line to a point</summary>
		/// <param name="linear">The linear object to check distance from (Ray3D, Line3D or LineSegment3D)</param>
		/// <param name="point">The point to check the distance to</param>
		[MethodImpl( INLINE )] public static float Distance<T>( this T linear, Vector3 point ) where T : ILinear3D => MathF.Sqrt( DistanceSqr( linear, point ) );

		/// <summary>The shortest squared distance from this line to a point</summary>
		/// <param name="linear">The linear object to check distance from (Ray3D, Line3D or LineSegment3D)</param>
		/// <param name="point">The point to check the distance to</param>
		[MethodImpl( INLINE )] public static float DistanceSqr<T>( this T linear, Vector3 point ) where T : ILinear3D => ( point - linear.ProjectPoint( point ) ).sqrMagnitude;

		/// <summary>The shortest squared distance from this line to another line</summary>
		/// <param name="linear">The linear object to check distance from (Ray3D, Line3D or LineSegment3D)</param>
		/// <param name="other">The other linear object to check the distance to</param>
		[MethodImpl( INLINE )] public static float DistanceSqr<A, B>( this A linear, B other ) where A : ILinear3D where B : ILinear3D {
			( float tA, float tB ) = LinearTValues( linear, other );
			return ( linear.GetPoint( tA ) - other.GetPoint( tB ) ).sqrMagnitude;
		}

	}

}