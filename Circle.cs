// Collected and expanded upon to by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using UnityEngine;
using static Freya.Mathfs;

namespace Freya {

	// Circle math
	public struct Circle {

		public Vector2 center;
		public float radius;

		public Circle( Vector2 center, float radius ) {
			this.center = center;
			this.radius = radius;
		}

		float Area => RadiusToArea( radius );
		float Circumference => RadiusToCircumference( radius );

		public static float RadiusToArea( float r ) => r * r * ( 0.5f * TAU );
		public static float AreaToRadius( float area ) => Sqrt( 2 * area / TAU );
		public static float AreaToCircumference( float area ) => Sqrt( 2 * area / TAU ) * TAU;
		public static float CircumferenceToArea( float c ) => c * c / ( 2 * TAU );
		public static float RadiusToCircumference( float r ) => r * TAU;
		public static float CircumferenceToRadius( float c ) => c / TAU;

		public static Circle FromTwoPoints( Vector2 a, Vector2 b ) {
			return new Circle( ( a + b ) / 2f, Vector2.Distance( a, b ) / 2f );
		}

		public static bool FromThreePoints( Vector2 a, Vector2 b, Vector2 c, out Circle circle ) {
			Line2D lineA = LineSegment2D.GetBisectorFast( a, b );
			Line2D lineB = LineSegment2D.GetBisectorFast( b, c );
			if( Intersect.Lines( lineA, lineB, out circle.center ) ) {
				circle.radius = Vector2.Distance( circle.center, a );
				return true;
			}

			circle = default;
			return false;
		}


	}

}