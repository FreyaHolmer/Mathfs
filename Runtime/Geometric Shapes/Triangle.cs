// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using System;
using UnityEngine;
using static Freya.Mathfs;

namespace Freya {

	/// <summary>A triangle in 2D space</summary>
	[Serializable] public partial struct Triangle2D {
		/// <summary>The first vertex of the triangle</summary>
		public Vector2 a;

		/// <summary>The second vertex of the triangle</summary>
		public Vector2 b;

		/// <summary>The third vertex of the triangle</summary>
		public Vector2 c;

		/// <summary>Creates a 2D triangle</summary>
		/// <param name="a">The first vertex in the triangle</param>
		/// <param name="b">The second vertex in the triangle</param>
		/// <param name="c">The third vertex in the triangle</param>
		public Triangle2D( Vector2 a, Vector2 b, Vector2 c ) => ( this.a, this.b, this.c ) = ( a, b, c );

		/// <summary>Returns a control point position by index. Valid indices: 0, 1, 2 or 3</summary>
		public Vector2 this[ int i ] {
			get {
				switch( i ) {
					case 0:  return a;
					case 1:  return b;
					case 2:  return c;
					default: throw new ArgumentOutOfRangeException( nameof(i), $"Index has to be in the 0 to 2 range, and I think {i} is outside that range you know" );
				}
			}
			set {
				switch( i ) {
					case 0:
						a = value;
						break;
					case 1:
						b = value;
						break;
					case 2:
						c = value;
						break;
					default: throw new ArgumentOutOfRangeException( nameof(i), $"Index has to be in the 0 to 2 range, and I think {i} is outside that range you know" );
				}
			}
		}

	}

	/// <summary>A triangle in 3D space</summary>
	[Serializable] public partial struct Triangle3D {
		/// <inheritdoc cref="Triangle2D.a"/>
		public Vector3 a;

		/// <inheritdoc cref="Triangle2D.b"/>
		public Vector3 b;

		/// <inheritdoc cref="Triangle2D.c"/>
		public Vector3 c;

		/// <summary>Creates a 3D triangle</summary>
		/// <param name="a">The first vertex in the triangle</param>
		/// <param name="b">The second vertex in the triangle</param>
		/// <param name="c">The third vertex in the triangle</param>
		public Triangle3D( Vector2 a, Vector2 b, Vector2 c ) => ( this.a, this.b, this.c ) = ( a, b, c );

		/// <summary>Returns a control point position by index. Valid indices: 0, 1, 2 or 3</summary>
		public Vector2 this[ int i ] {
			get {
				switch( i ) {
					case 0:  return a;
					case 1:  return b;
					case 2:  return c;
					default: throw new ArgumentOutOfRangeException( nameof(i), $"Index has to be in the 0 to 2 range, and I think {i} is outside that range you know" );
				}
			}
			set {
				switch( i ) {
					case 0:
						a = value;
						break;
					case 1:
						b = value;
						break;
					case 2:
						c = value;
						break;
					default: throw new ArgumentOutOfRangeException( nameof(i), $"Index has to be in the 0 to 2 range, and I think {i} is outside that range you know" );
				}
			}
		}

	}

	#region Area

	public partial struct Triangle2D {
		/// <summary>The area of the triangle</summary>
		public float Area => MathF.Abs( SignedArea );

		// todo: verify clockwise vs ccw
		/// <summary>The signed area of the triangle. When the triangle is defined clockwise, the area will be negative</summary>
		public float SignedArea => Determinant( b - a, c - a ) / 2f;
	}

	public partial struct Triangle3D {
		/// <inheritdoc cref="Freya.Triangle2D.Area"/>
		public float Area => Vector3.Cross( b - a, c - a ).magnitude / 2f;
	}

	#endregion

	#region Triangle Centers

	public partial struct Triangle2D {

		/// <summary>The centroid of the triangle, which is the point of intersection of its medians (the lines joining each vertex with the midpoint of the opposite side).
		/// Physically, this would also be its center of mass</summary>
		public Vector2 Centroid => ( a + b + c ) / 3f;

		/// <summary>The incenter of a triangle, which is the point of intersection of the angular bisectors.
		/// This is also the center of the incircle, the largest circle that can fit in the triangle.</summary>
		public Vector2 Incenter {
			get {
				float bc = Vector2.Distance( b, c );
				float ca = Vector2.Distance( c, a );
				float ab = Vector2.Distance( a, b );
				return ( bc * a + ca * b + ab * c ) / ( bc + ca + ab );
			}
		}

		/// <summary>The intersection of the perpendicular bisectors of each edge of the triangle</summary>
		public Vector2 Circumcenter {
			get {
				Line2D bsA = LineSegment2D.GetBisector( a, b );
				Line2D bsB = LineSegment2D.GetBisector( b, c );
				if( bsA.Intersect( bsB, out Vector2 intPt ) )
					return intPt;
				throw new ArithmeticException( "Cannot get the circumcenter of a triangle without area" );
			}
		}

		/// <summary>The intersection of the altitudes of the triangle</summary>
		public Vector2 Orthocenter {
			get {
				Line2D bsA = new Line2D( a, ( b - c ).Rotate90CW() );
				Line2D bsB = new Line2D( b, ( c - a ).Rotate90CW() );
				if( bsA.Intersect( bsB, out Vector2 intPt ) )
					return intPt;
				throw new ArithmeticException( "Cannot get the orthocenter of a triangle without area" );
			}
		}


	}

	public partial struct Triangle3D {

		/// <inheritdoc cref="Freya.Triangle2D.Centroid"/>
		public Vector3 Centroid => ( a + b + c ) / 3f;

		/// <inheritdoc cref="Triangle2D.Incenter"/>
		public Vector3 Incenter {
			get {
				float bc = Vector3.Distance( b, c );
				float ca = Vector3.Distance( c, a );
				float ab = Vector3.Distance( a, b );
				return ( bc * a + ca * b + ab * c ) / ( bc + ca + ab );
			}
		}

	}

	#endregion

	#region Circles

	public partial struct Triangle2D {

		/// <summary>The incircle of this triangle, which is the largest circle that can fit inside of it. Its center is the incenter</summary>
		public Circle2D Incircle {
			get {
				float eA = EdgeA;
				float eB = EdgeB;
				float eC = EdgeC;
				float sideSum = eA + eB + eC;
				Vector2 incenter = ( eA * a + eB * b + eC * c ) / sideSum;
				float s = sideSum * 0.5f;
				float r = Sqrt( ( s - eA ) * ( s - eB ) * ( s - eC ) / s );
				return new Circle2D( incenter, r );
			}
		}

		/// <summary>The circumcircle of a triangle, which is the unique circle passing through all three vertices. Note that if the triangle is degenerate, no incircle exists</summary>
		public Circle2D Circumcircle => Circle2D.FromThreePoints( a, b, c, out Circle2D circle ) ? circle : default;

	}

	public partial struct Triangle3D {
		// todo: 3D incircle
		// todo: 3D circumcircle
	}

	#endregion

	#region Contains

	public partial struct Triangle2D {

		/// <summary>Checks whether or not a point is inside this triangle</summary>
		/// <param name="point">The point to test if it's inside or not</param>
		/// <param name="aMargin">Optional floating point offset for margin testing on side A. Note: this flips direction depending on if the triangle is clockwise or counter-clockwise</param>
		/// <param name="bMargin">Optional floating point offset for margin testing on side B. Note: this flips direction depending on if the triangle is clockwise or counter-clockwise</param>
		/// <param name="cMargin">Optional floating point offset for margin testing on side C. Note: this flips direction depending on if the triangle is clockwise or counter-clockwise</param>
		public bool Contains( Vector2 point, float aMargin = 0f, float bMargin = 0f, float cMargin = 0f ) {
			float d0 = Determinant( b - a, point - a );
			float d1 = Determinant( c - b, point - b );
			float d2 = Determinant( a - c, point - c );
			bool b0 = d0 < cMargin;
			bool b1 = d1 < aMargin;
			bool b2 = d2 < bMargin;
			return b0 == b1 && b1 == b2; // on the same side of all halfspaces, this can only happen inside
		}

	}

	public partial struct Triangle3D {
		// todo: contains checks here are a little strange. 
		// todo: you'd kinda only be able to check against flattened 2D triangles / an infinite triangular prism
		// todo: or, actual contains checks with a distance threshold, but that's also a little weird? idk
	}

	#endregion

	#region Edges & Perimeter

	public partial struct Triangle2D {

		/// <summary>Returns the length of the side opposite the given vertex index</summary>
		/// <param name="index">The vertex index opposite the side to get the length of. Valid values: 0, 1 or 2</param>
		public float GetEdge( int index ) {
			switch( index ) {
				case 0:  return EdgeA;
				case 1:  return EdgeB;
				case 2:  return EdgeC;
				default: throw new ArgumentOutOfRangeException( nameof(index), "Triangle indices have to be either 0, 1 or 2" );
			}
		}

		/// <summary>The length of the edge opposite to vertex A</summary>
		public float EdgeA => Vector2.Distance( c, b );

		/// <summary>The length of the edge opposite to vertex B</summary>
		public float EdgeB => Vector2.Distance( a, c );

		/// <summary>The length of the edge opposite to vertex C</summary>
		public float EdgeC => Vector2.Distance( b, a );

		/// <summary>The perimeter of the triangle, equivalent to the sum of all edge lengths</summary>
		public float Perimeter => EdgeA + EdgeB + EdgeC;

	}

	public partial struct Triangle3D {

		/// <inheritdoc cref="Triangle2D.GetEdge(int)"/>
		public float GetEdge( int index ) {
			switch( index ) {
				case 0:  return EdgeA;
				case 1:  return EdgeB;
				case 2:  return EdgeC;
				default: throw new ArgumentOutOfRangeException( nameof(index), "Triangle indices have to be either 0, 1 or 2" );
			}
		}

		/// <inheritdoc cref="Triangle2D.EdgeA"/>
		public float EdgeA => Vector3.Distance( c, b );

		/// <inheritdoc cref="Triangle2D.EdgeB"/>
		public float EdgeB => Vector3.Distance( a, c );

		/// <inheritdoc cref="Triangle2D.EdgeC"/>
		public float EdgeC => Vector3.Distance( b, a );

		/// <inheritdoc cref="Triangle2D.Perimeter"/>
		public float Perimeter => EdgeA + EdgeB + EdgeC;

	}

	#endregion

	#region Angles

	public partial struct Triangle2D {

		/// <summary>Returns the internal angle of a given vertex</summary>
		/// <param name="index">The vertex to get the angle of. Valid values: 0, 1 or 2</param>
		public float GetAngle( int index ) {
			switch( index ) {
				case 0:  return AngleA;
				case 1:  return AngleB;
				case 2:  return AngleC;
				default: throw new ArgumentOutOfRangeException( nameof(index), "Triangle indices have to be either 0, 1 or 2" );
			}
		}

		/// <summary>The angle at vertex A</summary>
		public float AngleA => AngleBetween( b - a, c - a );

		/// <summary>The angle at vertex B</summary>
		public float AngleB => AngleBetween( c - b, a - b );

		/// <summary>The angle at vertex C</summary>
		public float AngleC => AngleBetween( a - c, b - c );

		/// <summary>The angles of each vertex. The sum of these is always half a turn = tau/2 = pi</summary>
		public (float angA, float angB, float angC) Angles {
			get {
				Vector2 abDir = ( b - a ).normalized;
				Vector2 acDir = ( c - a ).normalized;
				Vector2 bcDir = ( c - b ).normalized;
				float angA = MathF.Acos( Vector2.Dot( abDir, acDir ).ClampNeg1to1() );
				float angB = MathF.Acos( Vector2.Dot( -abDir, bcDir ).ClampNeg1to1() );
				float angC = PI - angA - angB;
				return ( angA, angB, angC );
			}
		}

		/// <summary>The smallest of the three angles</summary>
		public float SmallestAngle {
			get {
				( float angA, float angB, float angC ) = Angles;
				return Min( angA, angB, angC );
			}
		}

		/// <summary>The largest of the three angles</summary>
		public float LargestAngle {
			get {
				( float angA, float angB, float angC ) = Angles;
				return Max( angA, angB, angC );
			}
		}

	}

	public partial struct Triangle3D {

		/// <inheritdoc cref="Triangle2D.GetAngle(int)"/>
		public float GetAngle( int index ) {
			switch( index ) {
				case 0:  return AngleA;
				case 1:  return AngleB;
				case 2:  return AngleC;
				default: throw new ArgumentOutOfRangeException( nameof(index), "Triangle indices have to be either 0, 1 or 2" );
			}
		}

		/// <inheritdoc cref="Triangle2D.AngleA"/>
		public float AngleA => AngleBetween( b - a, c - a );

		/// <inheritdoc cref="Triangle2D.AngleB"/>
		public float AngleB => AngleBetween( c - b, a - b );

		/// <inheritdoc cref="Triangle2D.AngleC"/>
		public float AngleC => AngleBetween( a - c, b - c );

		/// <inheritdoc cref="Triangle2D.Angles"/>
		public (float angA, float angB, float angC) Angles {
			get {
				Vector2 abDir = ( b - a ).normalized;
				Vector2 acDir = ( c - a ).normalized;
				Vector2 bcDir = ( c - b ).normalized;
				float angA = MathF.Acos( Vector2.Dot( abDir, acDir ).ClampNeg1to1() );
				float angB = MathF.Acos( Vector2.Dot( -abDir, bcDir ).ClampNeg1to1() );
				float angC = PI - angA - angB;
				return ( angA, angB, angC );
			}
		}

		/// <inheritdoc cref="Triangle2D.SmallestAngle"/>
		public float SmallestAngle {
			get {
				( float angA, float angB, float angC ) = Angles;
				return Min( angA, angB, angC );
			}
		}

		/// <inheritdoc cref="Triangle2D.LargestAngle"/>
		public float LargestAngle {
			get {
				( float angA, float angB, float angC ) = Angles;
				return Max( angA, angB, angC );
			}
		}

	}

	#endregion

	/// <summary>Helper functions for right angle triangles</summary>
	public static class RightTriangle {

		/// <summary>The area of a right angle triangle, given a base and a height</summary>
		/// <param name="base">The length of the base</param>
		/// <param name="height">The height of the triangle</param>
		public static float Area( float @base, float height ) => ( @base * height ) * 0.5f;

		/// <summary>Returns the angle, given the length of the opposite edge and the hypotenuse</summary>
		/// <param name="opposite">The length of the edge opposite to this angle</param>
		/// <param name="hypotenuse">The length of the hypotenuse</param>
		public static float AngleFromOppositeHypotenuse( float opposite, float hypotenuse ) => Asin( ( opposite / hypotenuse ).ClampNeg1to1() );

		/// <summary>Returns the angle, given the length of the adjacent edge and the hypotenuse</summary>
		/// <param name="adjacent">The length of the edge adjacent to this angle</param>
		/// <param name="hypotenuse">The length of the hypotenuse</param>
		public static float AngleFromAdjacentHypotenuse( float adjacent, float hypotenuse ) => Acos( ( adjacent / hypotenuse ).ClampNeg1to1() );

		/// <summary>Returns the angle, given the length of the opposite edge and the adjacent edge</summary>
		/// <param name="opposite">The length of the edge opposite to this angle</param>
		/// <param name="adjacent">The length of the edge adjacent to this angle</param>
		public static float AngleFromOppositeAdjacent( float opposite, float adjacent ) => Atan( opposite / adjacent );

		/// <summary>Returns the length of the hypotenuse, given an angle and its adjacent edge length</summary>
		/// <param name="angle">The angle between the hypotenuse and its adjacent edge</param>
		/// <param name="adjacent">The length of the adjacent edge</param>
		public static float HypotenuseFromAngleAdjacent( float angle, float adjacent ) => adjacent / Cos( angle );

		/// <summary>Returns the length of the hypotenuse, given an angle and its opposite edge length</summary>
		/// <param name="angle">The angle between the hypotenuse and its adjacent edge</param>
		/// <param name="opposite">The length of the opposite edge</param>
		public static float HypotenuseFromAngleOpposite( float angle, float opposite ) => opposite / Sin( angle );

		/// <summary>Returns the length of the hypotenuse, given the length of the two other sides</summary>
		/// <param name="opposite">The length of the opposite edge</param>
		/// <param name="adjacent">The length of the adjacent edge</param>
		public static float HypotenuseFromOppositeAdjacent( float opposite, float adjacent ) => Sqrt( adjacent.Square() + opposite.Square() );

		/// <summary>Returns the length of the adjecent edge, given an angle and its opposite edge length</summary>
		/// <param name="angle">The angle between the hypotenuse and its adjacent edge</param>
		/// <param name="opposite">The length of the opposite edge</param>
		public static float AdjacentFromAngleOpposite( float angle, float opposite ) => opposite / Tan( angle );

		/// <summary>Returns the length of the adjecent edge, given an angle and the hypotenuse</summary>
		/// <param name="angle">The angle between the hypotenuse and its adjacent edge</param>
		/// <param name="hypotenuse">The length of the hypotenuse</param>
		public static float AdjacentFromAngleHypotenuse( float angle, float hypotenuse ) => Cos( angle ) * hypotenuse;

		/// <summary>Returns the length of the adjecent edge, given the opposite edge and the hypotenuse</summary>
		/// <param name="opposite">The length of the opposite edge</param>
		/// <param name="hypotenuse">The length of the hypotenuse</param>
		public static float AdjacentFromOppositeHypotenuse( float opposite, float hypotenuse ) => Sqrt( hypotenuse.Square() - opposite.Square() );

		/// <summary>Returns the length of the opposite edge, given an angle and its adjacent edge length</summary>
		/// <param name="angle">The angle between the hypotenuse and its adjacent edge</param>
		/// <param name="adjacent">The length of the adjacent edge</param>
		public static float OppositeFromAngleAdjacent( float angle, float adjacent ) => Tan( angle ) * adjacent;

		/// <summary>Returns the length of the opposite edge, given an angle and the hypotenuse</summary>
		/// <param name="angle">The angle between the hypotenuse and its adjacent edge</param>
		/// <param name="hypotenuse">The length of the hypotenuse</param>
		public static float OppositeFromAngleHypotenuse( float angle, float hypotenuse ) => Sin( angle ) * hypotenuse;

		/// <summary>Returns the length of the opposite edge, given the adjacent edge and the hypotenuse</summary>
		/// <param name="adjacent">The length of the adjacent edge</param>
		/// <param name="hypotenuse">The length of the hypotenuse</param>
		public static float OppositeFromAdjacentHypotenuse( float adjacent, float hypotenuse ) => Sqrt( hypotenuse.Square() - adjacent.Square() );

	}

}