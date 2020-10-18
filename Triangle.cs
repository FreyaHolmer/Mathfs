// Collected and expanded upon to by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using UnityEngine;

public static partial class Mathfs {

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

	}


}