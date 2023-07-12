// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using static Freya.Mathfs;

namespace Freya {

	/// <summary>A 2D circle with a centerpoint and a radius</summary>
	public partial struct Circle2D {

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
	}

	/// <summary>A 3D circle with a centerpoint, radius, and a normal/axis</summary>
	public partial struct Circle3D {

		const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

		/// <inheritdoc cref="Circle2D.center"/>
		public Vector3 center;

		/// <inheritdoc cref="Circle2D.radius"/>
		public float radius;

		/// <summary>The normal/axis of the circle</summary>
		public Vector3 normal;

		/// <summary>Creates a 3D Circle</summary>
		/// <param name="center">The center of the circle</param>
		/// <param name="normal">The normal/axis of the circle</param>
		/// <param name="radius">The radius of the circle</param>
		public Circle3D( Vector3 center, Vector3 normal, float radius ) {
			this.center = center;
			this.normal = normal;
			this.radius = radius;
		}

	}

	#region Area

	public partial struct Circle2D {
		/// <summary>Get or set the area of this circle</summary>
		public float Area {
			[MethodImpl( INLINE )] get => RadiusToArea( radius );
			[MethodImpl( INLINE )] set => radius = AreaToRadius( value );
		}
	}

	public partial struct Circle3D {
		/// <inheritdoc cref="Circle2D.Area"/>
		public float Area {
			[MethodImpl( INLINE )] get => Circle2D.RadiusToArea( radius );
			[MethodImpl( INLINE )] set => radius = Circle2D.AreaToRadius( value );
		}
	}

	#endregion

	#region Circumference

	public partial struct Circle2D {
		/// <summary>Get or set the circumference of this circle</summary>
		public float Circumference {
			[MethodImpl( INLINE )] get => RadiusToCircumference( radius );
			[MethodImpl( INLINE )] set => radius = CircumferenceToRadius( value );
		}
	}

	public partial struct Circle3D {
		/// <inheritdoc cref="Circle2D.Circumference"/>
		public float Circumference {
			[MethodImpl( INLINE )] get => Circle2D.RadiusToCircumference( radius );
			[MethodImpl( INLINE )] set => radius = Circle2D.CircumferenceToRadius( value );
		}
	}

	#endregion

	#region Project

	public partial struct Circle2D {
		/// <summary>Projects a point to the nearest point on the circle. Points inside the circle will be pushed out to the boundary</summary>
		/// <param name="point">The point to project</param>
		public Vector2 ProjectPoint( Vector2 point ) {
			Vector2 v = point - center;
			float mag = v.magnitude;
			return center + v * ( radius / mag );
		}
	}

	public partial struct Circle3D {
		/// <inheritdoc cref="Circle2D.ProjectPoint"/>
		public Vector3 ProjectPoint( Vector3 point ) {
			Vector3 v = point - center;
			Vector3 flattened = v - Vector3.Dot( v, normal ) * normal;
			float mag = flattened.magnitude;
			return center + flattened * ( radius / mag );
		}
	}

	#endregion

	#region Contains (2D only)

	public partial struct Circle2D {
		/// <summary>Returns whether or not the point is inside the circle</summary>
		/// <param name="point">The point to check if it's inside or outside</param>
		[MethodImpl( INLINE )] public bool Contains( Vector2 point ) {
			float dx = point.x - center.x;
			float dy = point.y - center.y;
			return dx * dx + dy * dy <= radius * radius;
		}
	}

	#endregion

	#region Construct from 2 points (2D only)

	public partial struct Circle2D {
		/// <summary>Returns the smallest possible circle passing through two points</summary>
		/// <param name="a">The first point in the circle</param>
		/// <param name="b">The second point in the circle</param>
		[MethodImpl( INLINE )] public static Circle2D FromTwoPoints( Vector2 a, Vector2 b ) => new Circle2D( ( a + b ) / 2f, Vector2.Distance( a, b ) / 2f );
	}

	#endregion

	#region Construct from 3 points

	public partial struct Circle2D {
		/// <summary>Returns a circle passing through all three points. Note: if the three points are collinear, no valid circle exists</summary>
		/// <param name="a">The first point on the circle</param>
		/// <param name="b">The second point on the circle</param>
		/// <param name="c">The third point on the circle</param>
		/// <param name="circle">The circle passing through all three points</param>
		public static bool FromThreePoints( Vector2 a, Vector2 b, Vector2 c, out Circle2D circle ) {
			Line2D lineA = LineSegment2D.GetBisector( a, b );
			Line2D lineB = LineSegment2D.GetBisector( b, c );
			if( lineA.Intersect( lineB, out circle.center ) ) {
				circle.radius = Vector2.Distance( circle.center, a );
				return true;
			}

			circle = default;
			return false;
		}
	}

	public partial struct Circle3D {

		/// <inheritdoc cref="Circle2D.FromThreePoints"/>
		public static bool FromThreePoints( Vector3 a, Vector3 b, Vector3 c, out Circle3D circle ) {
			// todo: this is effectively:
			// • bisector plane-plane intersection -> Line
			// • plane-line intersection for center
			// • vector length check for radius
			// which might be cheaper than the below:

			Vector3 bRel = b - a;
			( Vector3 xAxis, float bx2D ) = bRel.GetDirAndMagnitude();
			Vector3 cRel = c - a;
			Vector3 normal = Vector3.Cross( bRel, cRel ).normalized;
			Vector3 yAxis = Vector3.Cross( normal, xAxis );

			Vector2 b2D = new Vector2( bx2D, 0 );
			Vector2 c2D = new Vector2( Vector3.Dot( xAxis, cRel ), Vector3.Dot( yAxis, cRel ) );

			if( Circle2D.FromThreePoints( default, b2D, c2D, out Circle2D circle2D ) ) {
				Vector3 origin = xAxis * circle2D.center.x + yAxis * circle2D.center.y;
				circle = new Circle3D( a + origin, normal, circle2D.radius );
				return true;
			}

			circle = default;
			return false;
		}

	}

	#endregion

	#region Construct from Point/Tangent/Point

	public partial struct Circle2D {
		/// <summary>Returns a circle passing through the start with a given tangent direction, and the end point, if possible.
		/// Note: if the tangent points directly toward the second point, no valid circle exists</summary>
		/// <param name="startPt">The first point on the circle</param>
		/// <param name="startTangent">The tangent direction of the circle at the first point</param>
		/// <param name="endPt">The second point on the circle</param>
		/// <param name="circle">The circle passing through the start with a given tangent direction, and the end point, if possible</param>
		public static bool FromPointTangentPoint( Vector2 startPt, Vector2 startTangent, Vector2 endPt, out Circle2D circle ) {
			Line2D lineA = new Line2D( startPt, startTangent.Rotate90CW() );
			Line2D lineB = LineSegment2D.GetBisector( startPt, endPt );
			if( lineA.Intersect( lineB, out Vector2 pt ) ) {
				circle = new Circle2D( pt, Vector2.Distance( pt, startPt ) );
				return true;
			}


			circle = default;
			return false;
		}
	}

	public partial struct Circle3D {
		/// <inheritdoc cref="Circle2D.FromPointTangentPoint"/>
		public static bool FromPointTangentPoint( Vector3 startPt, Vector3 startTangent, Vector3 endPt, out Circle3D circle ) {
			Vector3 delta = endPt - startPt;
			( Vector3 xAxis, float d ) = delta.GetDirAndMagnitude();
			if( Vector3.Dot( xAxis, startTangent ).Abs() < 0.9999f ) {
				float h = d / 2;
				float ang = AngleBetween( xAxis, startTangent );
				float fh = h * MathF.Tan( ang + TAU / 4 );
				float x2D = h;
				float y2D = fh;
				float r = MathF.Sqrt( h * h + fh * fh );
				Vector3 normal = Vector3.Cross( xAxis, startTangent ).normalized;
				Vector3 yAxis = Vector3.Cross( normal, xAxis );
				Vector3 center = startPt + xAxis * x2D + yAxis * y2D;
				circle = new Circle3D( center, normal, r );
				return true;
			}

			circle = default;
			return false;
		}
	}

	#endregion

	#region Intersection Tests (2D only, for now)

	public partial struct Circle2D {

		/// <summary>Returns whether or not this circle intersects a linear object</summary>
		/// <param name="linear">The linear object to test intersection against (Ray2D, Line2D or LineSegment2D)</param>
		[MethodImpl( INLINE )] public bool Intersects<T>( T linear ) where T : ILinear2D => IntersectionTest.LinearCircleIntersects( linear, this );

		/// <summary>Returns whether or not this circle intersects another circle</summary>
		/// <param name="circle">The circle to test intersection against</param>
		[MethodImpl( INLINE )] public bool Intersects( Circle2D circle ) => IntersectionTest.CirclesOverlap( this.center, this.radius, circle.center, circle.radius );

		/// <summary>Returns the intersections this circle has with a linear object (if any)</summary>
		/// <param name="linear">The linear object to test intersection against (Ray2D, Line2D or LineSegment2D)</param>
		[MethodImpl( INLINE )] public ResultsMax2<Vector2> Intersect<T>( T linear ) where T : ILinear2D => IntersectionTest.LinearCircleIntersectionPoints( linear, this );

		/// <summary>Returns the intersections this circle has with another circle (if any)</summary>
		/// <param name="circle">The circle to test intersection against</param>
		[MethodImpl( INLINE )] public ResultsMax2<Vector2> Intersect( Circle2D circle ) => IntersectionTest.CirclesIntersectionPoints( this.center, this.radius, circle.center, circle.radius );

	}

	#endregion

	#region Static general circle functions

	public partial struct Circle2D {

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

		/// <summary>Returns the osculating circle of a point in a curve. Osculating circles are defined everywhere except on inflection points, where curvature is 0</summary>
		/// <param name="point">The point of the curve</param>
		/// <param name="velocity">The first derivative of the point in the curve</param>
		/// <param name="acceleration">The second derivative of the point in the curve</param>
		[MethodImpl( INLINE )] public static Circle2D GetOsculatingCircle( Vector2 point, Vector2 velocity, Vector2 acceleration ) {
			float curvature = GetCurvature( velocity, acceleration );
			Vector2 tangent = velocity.normalized;
			Vector2 normal = tangent.Rotate90CCW();
			float signedRadius = 1f / curvature;
			return new Circle2D( point + normal * signedRadius, Abs( signedRadius ) );
		}

		public static Circle2D operator *( Circle2D circle, float value ) => new(circle.center * value, circle.radius * value);
		public static Circle2D operator *( float value, Circle2D circle ) => new(circle.center * value, circle.radius * value);

	}

	public partial struct Circle3D {

		/// <inheritdoc cref="Circle2D.GetOsculatingCircle(Vector2,Vector2,Vector2)"/>
		public static Circle3D GetOsculatingCircle( Vector3 point, Vector3 velocity, Vector3 acceleration ) {
			Bivector3 curvatureBivector = GetCurvature( velocity, acceleration );
			( Vector3 axis, float curvature ) = curvatureBivector.GetNormalAndArea();
			Vector3 normal = Vector3.Cross( velocity, Vector3.Cross( acceleration, velocity ) ).normalized;
			float signedRadius = 1f / curvature;
			return new Circle3D( point + normal * signedRadius, axis, Abs( signedRadius ) );
		}

	}

	#endregion

}