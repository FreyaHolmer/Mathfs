// Collected and expanded upon to by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using UnityEngine;
using static Freya.Mathfs;

namespace Freya {

	// Triangle math
	public static class Triangle {

		public static float SignedArea( Vector3 a, Vector3 b, Vector3 c ) => Vector3.Cross( b - a, c - a ).magnitude / 2f;
		public static float Area( Vector3 a, Vector3 b, Vector3 c ) => Mathf.Abs( SignedArea( a, b, c ) );
		public static Vector3 Centroid( Vector3 a, Vector3 b, Vector3 c ) => ( a + b + c ) / 3f;

		public static Vector3 Incenter( Vector3 a, Vector3 b, Vector3 c ) {
			float bc = Vector3.Distance( b, c );
			float ca = Vector3.Distance( c, a );
			float ab = Vector3.Distance( a, b );
			return ( bc * a + ca * b + ab * c ) / ( bc + ca + ab );
		}

		public static Circle Incircle( Vector2 a, Vector2 b, Vector2 c ) {
			float bc = Vector2.Distance( b, c );
			float ca = Vector2.Distance( c, a );
			float ab = Vector2.Distance( a, b );
			float sideSum = bc + ca + ab;
			Vector2 incenter = ( bc * a + ca * b + ab * c ) / sideSum;
			float s = sideSum * 0.5f;
			float r = Sqrt( ( s - bc ) * ( s - ca ) * ( s - ab ) / s );
			return new Circle( incenter, r );
		}

		public static bool Circumcircle( Vector2 a, Vector2 b, Vector2 c, out Circle circle ) => Circle.FromThreePoints( a, b, c, out circle );

		public static bool Contains( Vector2 a, Vector2 b, Vector2 c, Vector2 point, float aMargin = 0f, float bMargin = 0f, float cMargin = 0f ) {
			float d0 = Determinant( b - a, point - a );
			float d1 = Determinant( c - b, point - b );
			float d2 = Determinant( a - c, point - c );
			bool b0 = d0 < cMargin;
			bool b1 = d1 < aMargin;
			bool b2 = d2 < bMargin;
			return b0 == b1 && b1 == b2; // on the same side of all halfspaces, this can only happen inside
		}

		public static float SmallestAngle( Vector3 a, Vector3 b, Vector3 c ) {
			float angA = AngleBetween( c - b, a - b );
			float angB = AngleBetween( b - c, a - c );
			float angC = PI - angA - angB;
			return Min( angA, angB, angC );
		}

		// Right angle triangle math
		public static float Area( float width, float height ) => ( width * height ) * 0.5f;

		public static float AngleFromOppositeHypotenuse( float opposite, float hypotenuse ) => Asin( ( opposite / hypotenuse ).ClampNeg1to1() );
		public static float AngleFromAdjacentHypotenuse( float adjacent, float hypotenuse ) => Acos( ( adjacent / hypotenuse ).ClampNeg1to1() );
		public static float AngleFromOppositeAdjacent( float opposite, float adjacent ) => Atan( opposite / adjacent );

		public static float HypotenuseFromAngleAdjacent( float angle, float adjacent ) => adjacent / Cos( angle );
		public static float HypotenuseFromAngleOpposite( float angle, float opposite ) => opposite / Sin( angle );
		public static float HypotenuseFromOppositeAdjacent( float opposite, float adjacent ) => Sqrt( adjacent.Square() + opposite.Square() );

		public static float AdjacentFromAngleOpposite( float angle, float opposite ) => opposite / Tan( angle );
		public static float AdjacentFromAngleHypotenuse( float angle, float hypotenuse ) => Cos( angle ) * hypotenuse;
		public static float AdjacentFromOppositeHypotenuse( float opposite, float hypotenuse ) => Sqrt( hypotenuse.Square() - opposite.Square() );

		public static float OppositeFromAngleAdjacent( float angle, float adjacent ) => Tan( angle ) * adjacent;
		public static float OppositeFromAngleHypotenuse( float angle, float hypotenuse ) => Sin( angle ) * hypotenuse;
		public static float OppositeFromAdjacentHypotenuse( float adjacent, float hypotenuse ) => Sqrt( hypotenuse.Square() - adjacent.Square() );

	}


}