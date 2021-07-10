// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using System.Runtime.CompilerServices;
using UnityEngine;

namespace Freya {

	// This file has all the helpers/wrappers/utility functions for intersection tests using the intermediate structs,
	// while IntersectionTestCore.cs has the core math functions
	public static partial class IntersectionTest {

		#region Linear-Linear

		/// <summary>Returns the t-value of each linear 2D type (Ray2D, Line2D or LineSegment2D) where they intersect, if at all</summary>
		/// <param name="a">The first Ray, Line or LineSegment</param>
		/// <param name="b">The second Ray, Line or LineSegment</param>
		/// <param name="tA">The t-value of the intersection of the first linear type</param>
		/// <param name="tB">The t-value of the intersection of the second linear type</param>
		[MethodImpl( INLINE )] public static bool LinearTValues<T, U>( T a, U b, out float tA, out float tB ) where T : ILinear2D where U : ILinear2D {
			return IntersectionTest.LinearTValues( a.Origin, a.Dir, b.Origin, b.Dir, out tA, out tB ) && a.IsValidTValue( tA ) && b.IsValidTValue( tB );
		}

		/// <summary>Returns whether or not two linear 2D types (Ray2D, Line2D or LineSegment2D) intersect</summary>
		/// <param name="a">The first Ray, Line or LineSegment</param>
		/// <param name="b">The second Ray, Line or LineSegment</param>
		[MethodImpl( INLINE )] public static bool Linear<T, U>( T a, U b ) where T : ILinear2D where U : ILinear2D => LinearTValues( a, b, out _, out _ );

		/// <summary>Returns whether or not two linear 2D types (Ray2D, Line2D or LineSegment2D) intersect, and the point where they did, if they did</summary>
		/// <param name="a">The first Ray, Line or LineSegment</param>
		/// <param name="b">The second Ray, Line or LineSegment</param>
		/// <param name="intersectionPoint">The point at which they intersect</param>
		public static bool LinearIntersectionPoint<T, U>( T a, U b, out Vector2 intersectionPoint ) where T : ILinear2D where U : ILinear2D {
			bool intersects = LinearTValues( a, b, out float tA, out _ );
			intersectionPoint = intersects ? a.GetPoint( tA ) : default;
			return intersects;
		}

		#endregion

		#region Linear-Circle

		/// <summary>Returns the t-values of a linear 2D type (Ray2D, Line2D or LineSegment2D) where it would intersect a 2D circle, if at all</summary>
		/// <param name="linear">The Ray, Line or LineSegment to test intersection with</param>
		/// <param name="circle">The circle to test intersection with</param>
		public static ResultsMax2<float> LinearCircleTValues<T>( T linear, Circle2D circle ) where T : ILinear2D {
			ResultsMax2<float> values = IntersectionTest.LinearCircleTValues( linear.Origin, linear.Dir, circle.center, circle.radius );
			if( values.count == 0 ) return default;
			bool aValid = linear.IsValidTValue( values.a );
			bool bValid = linear.IsValidTValue( values.b );
			if( aValid && bValid ) return values;
			if( aValid ) return new ResultsMax2<float>( values.a );
			if( bValid ) return new ResultsMax2<float>( values.b );
			return default;
		}

		/// <summary>Returns whether or not a linear 2D type (Ray2D, Line2D or LineSegment2D) intersects with a 2D circle</summary>
		/// <param name="linear">The Ray, Line or LineSegment to test intersection with</param>
		/// <param name="circle">The circle to test intersection with</param>
		[MethodImpl( INLINE )] public static bool LinearCircleIntersects<T>( T linear, Circle2D circle ) where T : ILinear2D => LinearCircleTValues( linear, circle ).count > 0;

		/// <summary>Returns the intersection points between a linear 2D type (Ray2D, Line2D or LineSegment2D) and a 2D circle, if any</summary>
		/// <param name="linear">The Ray, Line or LineSegment to test intersection with</param>
		/// <param name="circle">The circle to test intersection with</param>
		public static ResultsMax2<Vector2> LinearCircleIntersectionPoints<T>( T linear, Circle2D circle ) where T : ILinear2D {
			ResultsMax2<float> tVals = LinearCircleTValues( linear, circle );
			if( tVals.count == 1 ) return new ResultsMax2<Vector2>( linear.GetPoint( tVals.a ) );
			if( tVals.count == 2 ) return new ResultsMax2<Vector2>( linear.GetPoint( tVals.a ), linear.GetPoint( tVals.b ) );
			return default;
		}

		#endregion

		#region Circle-Circle

		/// <summary>Returns whether or not two discs overlap. Unlike circles, discs overlap even if one is smaller and is completely inside the other</summary>
		/// <param name="a">The circle describing the first disc</param>
		/// <param name="b">The circle describing the second disc</param>
		[MethodImpl( INLINE )] public static bool DiscsOverlap( Circle2D a, Circle2D b ) => IntersectionTest.DiscsOverlap( a.center, a.radius, b.center, b.radius );

		#endregion

	}

}