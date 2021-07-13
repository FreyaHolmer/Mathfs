// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using System.Runtime.CompilerServices;
using UnityEngine;

namespace Freya {

	/// <summary>Represents a line segment, similar to a line but with a defined start and end</summary>
	public struct LineSegment2D : ILinear2D {

		const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

		/// <summary>The start point of the line segment</summary>
		public Vector2 start;

		/// <summary>The end point of the line segment</summary>
		public Vector2 end;

		/// <summary>Creates a line segment with a defined start and end point</summary>
		/// <param name="start">The start point of the line segment</param>
		/// <param name="end">The end point of the line segment</param>
		public LineSegment2D( Vector2 start, Vector2 end ) {
			this.start = start;
			this.end = end;
		}

		/// <summary>Gets a point along the line segment</summary>
		/// <param name="t">The t-value along the line segment to get the point of, where 0 = start, 1 = end</param>
		[MethodImpl( INLINE )] public Vector2 GetPoint( float t ) => Vector2.LerpUnclamped( start, end, t );

		/// <summary>Calculates the length of the line segment</summary>
		public float Length {
			[MethodImpl( INLINE )] get => Vector2.Distance( start, end );
		}

		/// <summary>Calculates the length squared (faster than calculating the actual length)</summary>
		public float LengthSquared {
			[MethodImpl( INLINE )] get {
				float dx = end.x - start.x;
				float dy = end.y - start.y;
				return dx * dx + dy * dy;
			}
		}
		
		/// <summary>Returns the t-value of a point projected onto this line segment</summary>
		/// <param name="point">The point to use when projecting</param>
		[MethodImpl( INLINE )] public float ProjectPointTValue( Vector2 point ) => Mathfs.Clamp01( Line2D.ProjectPointToLineTValue( start, end - start, point ) );
		
		/// <summary>Projects a point onto this line segment</summary>
		/// <param name="point">The point to project</param>
		[MethodImpl( INLINE )] public Vector2 ProjectPoint( Vector2 point ) => GetPoint( ProjectPointTValue( point ) );



		/// <summary>Returns the perpendicular bisector. Note: the returned normal is not normalized to save performance. Use <c>Bisector.Normalized</c> if you want to make sure it is normalized</summary>
		public Line2D Bisector {
			[MethodImpl( INLINE )] get => GetBisector( start, end );
		}

		/// <summary>Returns the perpendicular bisector of the input line segment. Note: the returned line is not normalized to save performance. Use <c>GetBisector().Normalized</c> if you want to make sure it is normalized</summary>
		/// <param name="startPoint">Starting point of the line segment</param>
		/// <param name="endPoint">Endpoint of the line segment</param>

		#region Intersection tests

		/// <summary>Returns whether or not this line segment intersects a ray</summary>
		/// <param name="ray">The ray to test intersection against</param>
		[MethodImpl( INLINE )] public bool Intersects( Ray2D ray ) => IntersectionTest.Linear( this, ray );

		/// <summary>Returns whether or not this line segment intersects another line segment</summary>
		/// <param name="lineSeg">The line segment to test intersection against</param>
		[MethodImpl( INLINE )] public bool Intersects( LineSegment2D lineSeg ) => IntersectionTest.Linear( this, lineSeg );

		/// <summary>Returns whether or not this line segment intersects a line</summary>
		/// <param name="line">The line to test intersection against</param>
		[MethodImpl( INLINE )] public bool Intersects( Line2D line ) => IntersectionTest.Linear( this, line );

		/// <summary>Returns whether or not this line segment intersects the input circle</summary>
		/// <param name="circle">The circle to test intersection against</param>
		[MethodImpl( INLINE )] public bool Intersects( Circle2D circle ) => IntersectionTest.LinearCircleIntersects( this, circle );

		/// <summary>Returns whether or not this line segment intersects a ray, and returns the point (if any)</summary>
		/// <param name="ray">The ray to test intersection against</param>
		/// <param name="intersectionPoint">The point at which they intersect</param>
		[MethodImpl( INLINE )] public bool Intersect( Ray2D ray, out Vector2 intersectionPoint ) => IntersectionTest.LinearIntersectionPoint( this, ray, out intersectionPoint );

		/// <summary>Returns whether or not this line segment intersects another line segment, and returns the point (if any)</summary>
		/// <param name="lineSeg">The line segment to test intersection against</param>
		/// <param name="intersectionPoint">The point at which they intersect</param>
		[MethodImpl( INLINE )] public bool Intersect( LineSegment2D lineSeg, out Vector2 intersectionPoint ) => IntersectionTest.LinearIntersectionPoint( this, lineSeg, out intersectionPoint );

		/// <summary>Returns whether or not this line segment intersects a line, and returns the point (if any)</summary>
		/// <param name="line">The line to test intersection against</param>
		/// <param name="intersectionPoint">The point at which they intersect</param>
		[MethodImpl( INLINE )] public bool Intersect( Line2D line, out Vector2 intersectionPoint ) => IntersectionTest.LinearIntersectionPoint( this, line, out intersectionPoint );

		/// <summary>Returns the intersections this line segment has with a circle (if any)</summary>
		/// <param name="circle">The circle to test intersection against</param>
		[MethodImpl( INLINE )] public ResultsMax2<Vector2> Intersect( Circle2D circle ) => IntersectionTest.LinearCircleIntersectionPoints( this, circle );

		#endregion
		[MethodImpl( INLINE )] public static Line2D GetBisector( Vector2 startPoint, Vector2 endPoint ) => new Line2D( ( startPoint + endPoint ) * 0.5f, ( startPoint - endPoint ).Rotate90CCW() );

		#region Interface stuff for generic line tests

		Vector2 ILinear2D.Origin {
			[MethodImpl( INLINE )] get => start;
		}
		Vector2 ILinear2D.Dir {
			[MethodImpl( INLINE )] get => end - start;
		}

		[MethodImpl( INLINE )]
		bool ILinear2D.IsValidTValue( float t ) => t >= 0 && t <= 1;

		#endregion

	}

}