// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using System.Runtime.CompilerServices;
using UnityEngine;
using static Freya.Mathfs;

namespace Freya {

	/// <summary>A 2D circle with a centerpoint and a radius</summary>
	public struct Circle2D {

		const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

		/// <summary>The center of the circle</summary>
		public Vector2 center;

		/// <summary>The radius of the circle</summary>
		public float radius;

		/// <summary>Creates a 2D Circle</summary>
		/// <param name="center">The center of the circle</param>
		/// <param name="radius">The radius of the circle</param>
		public Circle2D( Vector2 center, float radius ) {
			this.center = center;
			this.radius = radius;
		}

		/// <summary>Get or set the area of this circle</summary>
		public float Area {
			[MethodImpl( INLINE )] get => RadiusToArea( radius );
			[MethodImpl( INLINE )] set => radius = AreaToRadius( value );
		}

		/// <summary>Get or set the circumference of this circle</summary>
		public float Circumference {
			[MethodImpl( INLINE )] get => RadiusToCircumference( radius );
			[MethodImpl( INLINE )] set => radius = CircumferenceToRadius( value );
		}

		/// <summary>Projects a point onto this circle. Points inside the circle will be pushed out to the boundary</summary>
		/// <param name="point">The point to project</param>
		public Vector2 ProjectPoint( Vector2 point ) {
			Vector2 v = point - center;
			float mag = v.magnitude;
			return center + v * ( radius / mag );
		}

		/// <summary>Returns whether or not the point is inside the circle</summary>
		/// <param name="point">The point to check if it's inside or outside</param>
		[MethodImpl( INLINE )] public bool Contains( Vector2 point ) {
			float dx = point.x - center.x;
			float dy = point.y - center.y;
			return dx * dx + dy * dy <= radius * radius;
		}

		/// <summary>Calculates the area of a circle, given its radius</summary>
		/// <param name="r">The radius</param>
		[MethodImpl( INLINE )] public static float RadiusToArea( float r ) => r * r * ( 0.5f * TAU );

		/// <summary>Calculates the radius of a circle, given its area</summary>
		/// <param name="area">The area</param>
		[MethodImpl( INLINE )] public static float AreaToRadius( float area ) => Sqrt( 2 * area / TAU );

		/// <summary>Calculates the circumference of a circle, given its area</summary>
		/// <param name="area">The area</param>
		[MethodImpl( INLINE )] public static float AreaToCircumference( float area ) => Sqrt( 2 * area / TAU ) * TAU;

		/// <summary>Calculates the area of a circle, given its circumference</summary>
		/// <param name="c">The circumference</param>
		[MethodImpl( INLINE )] public static float CircumferenceToArea( float c ) => c * c / ( 2 * TAU );

		/// <summary>Calculates the circumference of a circle, given its radius</summary>
		/// <param name="r">The radius</param>
		[MethodImpl( INLINE )] public static float RadiusToCircumference( float r ) => r * TAU;

		/// <summary>Calculates the radius of a circle, given its circumference</summary>
		/// <param name="c">The circumference</param>
		[MethodImpl( INLINE )] public static float CircumferenceToRadius( float c ) => c / TAU;

		/// <summary>Returns the smallest possible circle passing through two points</summary>
		/// <param name="a">The first point in the circle</param>
		/// <param name="b">The second point in the circle</param>
		[MethodImpl( INLINE )] public static Circle2D FromTwoPoints( Vector2 a, Vector2 b ) => new Circle2D( ( a + b ) / 2f, Vector2.Distance( a, b ) / 2f );

		/// <summary>Returns a circle passing through the start with a given tangent direction, and the end point, if possible.
		/// Note: if the tangent points directly toward the second point, no valid circle exists</summary>
		/// <param name="startPt">The first point on the circle</param>
		/// <param name="startTangent">The tangent direction of the circle at the first point</param>
		/// <param name="endPt">The second point on the circle</param>
		/// <param name="circle">The circle passing through the start with a given tangent direction, and the end point, if possible</param>
		public static bool FromPointTangentPoint( Vector2 startPt, Vector2 startTangent, Vector2 endPt, out Circle2D circle ) {
			Line2D lineA = new Line2D( startPt, startTangent.Rotate90CW() );
			Line2D lineB = LineSegment2D.GetBisectorFast( startPt, endPt );
			if( lineA.Intersect( lineB, out Vector2 pt ) ) {
				circle = new Circle2D( pt, Vector2.Distance( pt, startPt ) );
				return true;
			}

			circle = default;
			return false;
		}

		/// <summary>Returns a circle passing through all three points. Note: if the three points are collinear, no valid circle exists</summary>
		/// <param name="a">The first point on the circle</param>
		/// <param name="b">The second point on the circle</param>
		/// <param name="c">The third point on the circle</param>
		/// <param name="circle">The circle passing through all three points</param>
		public static bool FromThreePoints( Vector2 a, Vector2 b, Vector2 c, out Circle2D circle ) {
			Line2D lineA = LineSegment2D.GetBisectorFast( a, b );
			Line2D lineB = LineSegment2D.GetBisectorFast( b, c );
			if( lineA.Intersect( lineB, out circle.center ) ) {
				circle.radius = Vector2.Distance( circle.center, a );
				return true;
			}

			circle = default;
			return false;
		}

		#region Intersection tests

		/// <summary>Returns whether or not this circle intersects a ray</summary>
		/// <param name="ray">The ray to test intersection against</param>
		[MethodImpl( INLINE )] public bool Intersects( Ray2D ray ) => IntersectionTest.LinearCircleIntersects( ray, this );

		/// <summary>Returns whether or not this circle intersects a line segment</summary>
		/// <param name="lineSeg">The line segment to test intersection against</param>
		[MethodImpl( INLINE )] public bool Intersects( LineSegment2D lineSeg ) => IntersectionTest.LinearCircleIntersects( lineSeg, this );

		/// <summary>Returns whether or not this circle intersects a line</summary>
		/// <param name="line">The line to test intersection against</param>
		[MethodImpl( INLINE )] public bool Intersects( Line2D line ) => IntersectionTest.LinearCircleIntersects( line, this );

		/// <summary>Returns whether or not this circle intersects another circle</summary>
		/// <param name="circle">The circle to test intersection against</param>
		[MethodImpl( INLINE )] public bool Intersects( Circle2D circle ) => IntersectionTest.CirclesOverlap( this.center, this.radius, circle.center, circle.radius );

		/// <summary>Returns the intersections this circle has with a ray (if any)</summary>
		/// <param name="ray">The ray to test intersection against</param>
		[MethodImpl( INLINE )] public ResultsMax2<Vector2> Intersect( Ray2D ray ) => IntersectionTest.LinearCircleIntersectionPoints( ray, this );

		/// <summary>Returns the intersections this circle has with a line segment (if any)</summary>
		/// <param name="lineSeg">The line segment to test intersection against</param>
		[MethodImpl( INLINE )] public ResultsMax2<Vector2> Intersect( LineSegment2D lineSeg ) => IntersectionTest.LinearCircleIntersectionPoints( lineSeg, this );

		/// <summary>Returns the intersections this circle has with a line (if any)</summary>
		/// <param name="line">The line to test intersection against</param>
		[MethodImpl( INLINE )] public ResultsMax2<Vector2> Intersect( Line2D line ) => IntersectionTest.LinearCircleIntersectionPoints( line, this );

		/// <summary>Returns the intersections this circle has with another circle (if any)</summary>
		/// <param name="circle">The circle to test intersection against</param>
		[MethodImpl( INLINE )] public ResultsMax2<Vector2> Intersect( Circle2D circle ) => IntersectionTest.CirclesIntersectionPoints( this.center, this.radius, circle.center, circle.radius );

		#endregion

	}

}