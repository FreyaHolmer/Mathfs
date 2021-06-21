// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)
// a lot of stuff here made possible by this excellent writeup on bezier curves: https://pomax.github.io/bezierinfo/

using System;
using UnityEngine;
using static Freya.Mathfs;

namespace Freya {

	// Bezier math
	// A lot of the following code is unrolled into floats and components for performance reasons.
	// It's much faster than keeping the more readable function calls and vector types unfortunately
	[Serializable] public partial struct BezierCubic2D {
		public Vector2 p0, p1, p2, p3;
		public BezierCubic2D( Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3 ) => ( this.p0, this.p1, this.p2, this.p3 ) = ( p0, p1, p2, p3 );
		public Vector2 this[ int i ] {
			get {
				switch( i ) {
					case 0:  return p0;
					case 1:  return p1;
					case 2:  return p2;
					case 3:  return p3;
					default: throw new ArgumentOutOfRangeException( nameof(i), $"Index has to be in the 0 to 3 range, and I think {i} is outside that range you know" );
				}
			}
			set {
				switch( i ) {
					case 0:
						p0 = value;
						break;
					case 1:
						p1 = value;
						break;
					case 2:
						p2 = value;
						break;
					case 3:
						p3 = value;
						break;
					default: throw new ArgumentOutOfRangeException( nameof(i), $"Index has to be in the 0 to 3 range, and I think {i} is outside that range you know" );
				}
			}

		}
	}

	[Serializable] public partial struct BezierCubic3D {
		public Vector3 p0, p1, p2, p3;
		public BezierCubic3D( Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3 ) => ( this.p0, this.p1, this.p2, this.p3 ) = ( p0, p1, p2, p3 );
		public Vector3 this[ int i ] {
			get {
				switch( i ) {
					case 0:  return p0;
					case 1:  return p1;
					case 2:  return p2;
					case 3:  return p3;
					default: throw new IndexOutOfRangeException();
				}
			}
			set {
				switch( i ) {
					case 0:
						p0 = value;
						break;
					case 1:
						p1 = value;
						break;
					case 2:
						p2 = value;
						break;
					case 3:
						p3 = value;
						break;
					default: throw new IndexOutOfRangeException();
				}
			}

		}
	}


	// Base properties - Points, Derivatives & Tangents

	#region Point

	public partial struct BezierCubic2D {
		/// <summary>Returns the point at the given t-value on the curve</summary>
		public Vector2 GetPoint( float t ) {
			float ax = p0.x + ( p1.x - p0.x ) * t; // a = lerp( p0, p1, t );
			float ay = p0.y + ( p1.y - p0.y ) * t;
			float bx = p1.x + ( p2.x - p1.x ) * t; // b = lerp( p1, p2, t );
			float by = p1.y + ( p2.y - p1.y ) * t;
			float cx = p2.x + ( p3.x - p2.x ) * t; // c = lerp( p2, p3, t );
			float cy = p2.y + ( p3.y - p2.y ) * t;
			float dx = ax + ( bx - ax ) * t; // d = lerp( a, b, t );
			float dy = ay + ( by - ay ) * t;
			float ex = bx + ( cx - bx ) * t; // e = lerp( b, c, t );
			float ey = by + ( cy - by ) * t;
			return new Vector2( // ret lerp( d, e, t );
				dx + ( ex - dx ) * t,
				dy + ( ey - dy ) * t
			);
		}
	}

	public partial struct BezierCubic3D {
		/// <summary>Returns the point at the given t-value on the curve</summary>
		public Vector3 GetPoint( float t ) {
			float ax = p0.x + ( p1.x - p0.x ) * t; // a = lerp( p0, p1, t );
			float ay = p0.y + ( p1.y - p0.y ) * t;
			float az = p0.z + ( p1.z - p0.z ) * t;
			float bx = p1.x + ( p2.x - p1.x ) * t; // b = lerp( p1, p2, t );
			float by = p1.y + ( p2.y - p1.y ) * t;
			float bz = p1.z + ( p2.z - p1.z ) * t;
			float cx = p2.x + ( p3.x - p2.x ) * t; // c = lerp( p2, p3, t );
			float cy = p2.y + ( p3.y - p2.y ) * t;
			float cz = p2.z + ( p3.z - p2.z ) * t;
			float dx = ax + ( bx - ax ) * t; // d = lerp( a, b, t );
			float dy = ay + ( by - ay ) * t;
			float dz = az + ( bz - az ) * t;
			float ex = bx + ( cx - bx ) * t; // e = lerp( b, c, t );
			float ey = by + ( cy - by ) * t;
			float ez = bz + ( cz - bz ) * t;
			return new Vector3( // ret Vector3.LerpUnclamped( d, e, t );
				dx + ( ex - dx ) * t,
				dy + ( ey - dy ) * t,
				dz + ( ez - dz ) * t
			);
		}
	}

	#endregion

	#region Point Components

	public partial struct BezierCubic2D {
		/// <summary>Returns the X coordinate at the given t-value on the curve</summary>
		public float GetPointX( float t ) {
			float a = p0.x + ( p1.x - p0.x ) * t; // a = lerp( p0, p1, t );
			float b = p1.x + ( p2.x - p1.x ) * t; // b = lerp( p1, p2, t );
			float c = p2.x + ( p3.x - p2.x ) * t; // c = lerp( p2, p3, t );
			float d = a + ( b - a ) * t; // d = lerp( a, b, t );
			float e = b + ( c - b ) * t; // e = lerp( b, c, t );
			return d + ( e - d ) * t; // ret lerp( d, e, t );
		}

		/// <summary>Returns the Y coordinate at the given t-value on the curve</summary>
		public float GetPointY( float t ) {
			float a = p0.y + ( p1.y - p0.y ) * t; // a = lerp( p0, p1, t );
			float b = p1.y + ( p2.y - p1.y ) * t; // b = lerp( p1, p2, t );
			float c = p2.y + ( p3.y - p2.y ) * t; // c = lerp( p2, p3, t );
			float d = a + ( b - a ) * t; // d = lerp( a, b, t );
			float e = b + ( c - b ) * t; // e = lerp( b, c, t );
			return d + ( e - d ) * t; // ret lerp( d, e, t );
		}

		//// <summary>Returns a component of the coordinate at the given t-value on the curve</summary>
		/// <param name="component">Which component of the coordinate to return. 0 is X, 1 is Y</param>
		/// <param name="t">A value from 0 to 1 representing a normalized coordinate along the curve</param>
		public float GetPointComponent( int component, float t ) {
			switch( component ) {
				case 0:  return GetPointX( t );
				case 1:  return GetPointY( t );
				default: throw new ArgumentOutOfRangeException( nameof(component), "component has to be either 0 or 1" );
			}
		}
	}

	public partial struct BezierCubic3D {
		/// <summary>Returns the X coordinate at the given t-value on the curve</summary>
		public float GetPointX( float t ) {
			float a = p0.x + ( p1.x - p0.x ) * t; // a = lerp( p0, p1, t );
			float b = p1.x + ( p2.x - p1.x ) * t; // b = lerp( p1, p2, t );
			float c = p2.x + ( p3.x - p2.x ) * t; // c = lerp( p2, p3, t );
			float d = a + ( b - a ) * t; // d = lerp( a, b, t );
			float e = b + ( c - b ) * t; // e = lerp( b, c, t );
			return d + ( e - d ) * t; // ret lerp( d, e, t );
		}

		/// <summary>Returns the Y coordinate at the given t-value on the curve</summary>
		public float GetPointY( float t ) {
			float a = p0.y + ( p1.y - p0.y ) * t; // a = lerp( p0, p1, t );
			float b = p1.y + ( p2.y - p1.y ) * t; // b = lerp( p1, p2, t );
			float c = p2.y + ( p3.y - p2.y ) * t; // c = lerp( p2, p3, t );
			float d = a + ( b - a ) * t; // d = lerp( a, b, t );
			float e = b + ( c - b ) * t; // e = lerp( b, c, t );
			return d + ( e - d ) * t; // ret lerp( d, e, t );
		}

		/// <summary>Returns the Z coordinate at the given t-value on the curve</summary>
		public float GetPointZ( float t ) {
			float a = p0.z + ( p1.z - p0.z ) * t; // a = lerp( p0, p1, t );
			float b = p1.z + ( p2.z - p1.z ) * t; // b = lerp( p1, p2, t );
			float c = p2.z + ( p3.z - p2.z ) * t; // c = lerp( p2, p3, t );
			float d = a + ( b - a ) * t; // d = lerp( a, b, t );
			float e = b + ( c - b ) * t; // e = lerp( b, c, t );
			return d + ( e - d ) * t; // ret lerp( d, e, t );
		}

		//// <summary>Returns a component of the coordinate at the given t-value on the curve</summary>
		/// <param name="component">Which component of the coordinate to return. 0 is X, 1 is Y, 2 is Z</param>
		/// <param name="t">A value from 0 to 1 representing a normalized coordinate along the curve</param>
		public float GetPointComponent( int component, float t ) {
			switch( component ) {
				case 0:  return GetPointX( t );
				case 1:  return GetPointY( t );
				case 2:  return GetPointZ( t );
				default: throw new ArgumentOutOfRangeException( nameof(component), "component has to be either 0, 1 or 2" );
			}
		}
	}

	#endregion

	#region Derivative

	public partial struct BezierCubic2D {
		/// <summary>Returns the derivative at the given t-value on the curve. Loosely analogous to "velocity" of the point along the curve</summary>
		public Vector2 GetDerivative( float t ) {
			float ax = p0.x + ( p1.x - p0.x ) * t; // a = lerp( p0, p1, t );
			float ay = p0.y + ( p1.y - p0.y ) * t;
			float bx = p1.x + ( p2.x - p1.x ) * t; // b = lerp( p1, p2, t );
			float by = p1.y + ( p2.y - p1.y ) * t;
			float cx = p2.x + ( p3.x - p2.x ) * t; // c = lerp( p2, p3, t );
			float cy = p2.y + ( p3.y - p2.y ) * t;
			float dx = ax + ( bx - ax ) * t; // d = lerp( a, b, t );
			float dy = ay + ( by - ay ) * t;
			float ex = bx + ( cx - bx ) * t; // e = lerp( b, c, t );
			float ey = by + ( cy - by ) * t;
			return new Vector2( 3 * ( ex - dx ), 3 * ( ey - dy ) ); // 3*(e - d)
		}
	}

	public partial struct BezierCubic3D {
		/// <summary>Returns the derivative at the given t-value on the curve. Loosely analogous to "velocity" of the point along the curve</summary>
		public Vector3 GetDerivative( float t ) {
			float ax = p0.x + ( p1.x - p0.x ) * t; // a = lerp( p0, p1, t );
			float ay = p0.y + ( p1.y - p0.y ) * t;
			float az = p0.z + ( p1.z - p0.z ) * t;
			float bx = p1.x + ( p2.x - p1.x ) * t; // b = lerp( p1, p2, t );
			float by = p1.y + ( p2.y - p1.y ) * t;
			float bz = p1.z + ( p2.z - p1.z ) * t;
			float cx = p2.x + ( p3.x - p2.x ) * t; // c = lerp( p2, p3, t );
			float cy = p2.y + ( p3.y - p2.y ) * t;
			float cz = p2.z + ( p3.z - p2.z ) * t;
			float dx = ax + ( bx - ax ) * t; // d = lerp( a, b, t );
			float dy = ay + ( by - ay ) * t;
			float dz = az + ( bz - az ) * t;
			float ex = bx + ( cx - bx ) * t; // e = lerp( b, c, t );
			float ey = by + ( cy - by ) * t;
			float ez = bz + ( cz - bz ) * t;
			return new Vector3( 3 * ( ex - dx ), 3 * ( ey - dy ), 3 * ( ez - dz ) ); // 3*(e - d)
		}
	}

	#endregion

	#region Second Derivative

	public partial struct BezierCubic2D {
		/// <summary>Returns the second derivative at the given t-value on the curve. Loosely analogous to "acceleration" of the point along the curve</summary>
		public Vector2 GetSecondDerivative( float t ) { // unrolled code for performance reasons
			float ax = p0.x + ( p1.x - p0.x ) * t; // a = lerp( p0, p1, t );
			float ay = p0.y + ( p1.y - p0.y ) * t;
			float bx = p1.x + ( p2.x - p1.x ) * t; // b = lerp( p1, p2, t );
			float by = p1.y + ( p2.y - p1.y ) * t;
			float cx = p2.x + ( p3.x - p2.x ) * t; // c = lerp( p2, p3, t );
			float cy = p2.y + ( p3.y - p2.y ) * t;
			return new Vector2(
				6 * ( ax - 2 * bx + cx ),
				6 * ( ay - 2 * by + cy )
			);
		}
	}

	public partial struct BezierCubic3D {
		/// <summary>Returns the second derivative at the given t-value on the curve. Loosely analogous to "acceleration" of the point along the curve</summary>
		public Vector3 GetSecondDerivative( float t ) { // unrolled code for performance reasons
			float ax = p0.x + ( p1.x - p0.x ) * t; // a = lerp( p0, p1, t );
			float ay = p0.y + ( p1.y - p0.y ) * t;
			float az = p0.z + ( p1.z - p0.z ) * t;
			float bx = p1.x + ( p2.x - p1.x ) * t; // b = lerp( p1, p2, t );
			float by = p1.y + ( p2.y - p1.y ) * t;
			float bz = p1.z + ( p2.z - p1.z ) * t;
			float cx = p2.x + ( p3.x - p2.x ) * t; // c = lerp( p2, p3, t );
			float cy = p2.y + ( p3.y - p2.y ) * t;
			float cz = p2.z + ( p3.z - p2.z ) * t;
			return new Vector3(
				6 * ( ax - 2 * bx + cx ),
				6 * ( ay - 2 * by + cy ),
				6 * ( az - 2 * bz + cz )
			);
		}
	}

	#endregion

	#region Third Derivative

	public partial struct BezierCubic2D {
		/// <summary>Returns the third derivative at the given t-value on the curve. Loosely analogous to "jerk" (rate of change of acceleration) of the point along the curve</summary>
		public Vector2 GetThirdDerivative( float t ) =>
			new Vector2(
				-6 * p0.x + 18 * p1.x - 18 * p2.x + 6 * p3.x,
				-6 * p0.y + 18 * p1.y - 18 * p2.y + 6 * p3.y
			);
	}

	public partial struct BezierCubic3D {
		/// <summary>Returns the third derivative at the given t-value on the curve. Loosely analogous to "jerk" (rate of change of acceleration) of the point along the curve</summary>
		public Vector3 GetThirdDerivative( float t ) =>
			new Vector3(
				-6 * p0.x + 18 * p1.x - 18 * p2.x + 6 * p3.x,
				-6 * p0.y + 18 * p1.y - 18 * p2.y + 6 * p3.y,
				-6 * p0.z + 18 * p1.z - 18 * p2.z + 6 * p3.z
			);
	}

	#endregion
	#region Point & Derivative combo

	public partial struct BezierCubic2D {
		/// <summary>Returns the point and the derivative at the given t-value on the curve. This is more performant than calling GetPoint and GetDerivative separately</summary>
		public (Vector2, Vector2) GetPointAndDerivative( float t ) { // GetPoint(t) and GetTangent(t) unrolled shared code
			float ax = p0.x + ( p1.x - p0.x ) * t; // a = lerp( p0, p1, t );
			float ay = p0.y + ( p1.y - p0.y ) * t;
			float bx = p1.x + ( p2.x - p1.x ) * t; // b = lerp( p1, p2, t );
			float by = p1.y + ( p2.y - p1.y ) * t;
			float cx = p2.x + ( p3.x - p2.x ) * t; // c = lerp( p2, p3, t );
			float cy = p2.y + ( p3.y - p2.y ) * t;
			float dx = ax + ( bx - ax ) * t; // d = lerp( a, b, t );
			float dy = ay + ( by - ay ) * t;
			float ex = bx + ( cx - bx ) * t; // e = lerp( b, c, t );
			float ey = by + ( cy - by ) * t;
			return (
				new Vector2( dx + ( ex - dx ) * t, dy + ( ey - dy ) * t ), // point
				new Vector2( 3 * ( ex - dx ), 3 * ( ey - dy ) ) // derivative
			);
		}
	}

	public partial struct BezierCubic3D {
		/// <summary>Returns the point and the derivative at the given t-value on the curve. This is more performant than calling GetPoint and GetDerivative separately</summary>
		public (Vector3, Vector3) GetPointAndDerivative( float t ) { // GetPoint(t) and GetTangent(t) unrolled shared code
			float ax = p0.x + ( p1.x - p0.x ) * t; // a = lerp( p0, p1, t );
			float ay = p0.y + ( p1.y - p0.y ) * t;
			float az = p0.z + ( p1.z - p0.z ) * t;
			float bx = p1.x + ( p2.x - p1.x ) * t; // b = lerp( p1, p2, t );
			float by = p1.y + ( p2.y - p1.y ) * t;
			float bz = p1.z + ( p2.z - p1.z ) * t;
			float cx = p2.x + ( p3.x - p2.x ) * t; // c = lerp( p2, p3, t );
			float cy = p2.y + ( p3.y - p2.y ) * t;
			float cz = p2.z + ( p3.z - p2.z ) * t;
			float dx = ax + ( bx - ax ) * t; // d = lerp( a, b, t );
			float dy = ay + ( by - ay ) * t;
			float dz = az + ( bz - az ) * t;
			float ex = bx + ( cx - bx ) * t; // e = lerp( b, c, t );
			float ey = by + ( cy - by ) * t;
			float ez = bz + ( cz - bz ) * t;
			return (
				new Vector3( dx + ( ex - dx ) * t, dy + ( ey - dy ) * t, dz + ( ez - dz ) * t ), // point
				new Vector3( 3 * ( ex - dx ), 3 * ( ey - dy ), 3 * ( ez - dz ) ) // derivative
			);
		}
	}

	#endregion
	
	#region Derivative & Second Derivative combo

	public partial struct BezierCubic2D {
		/// <summary>Returns the first two derivatives at the given t-value on the curve</summary>
		public (Vector2, Vector2) GetFirstTwoDerivatives( float t ) { // GetDerivative(t) and GetSecondDerivative(t) unrolled shared code
			float ax = p0.x + ( p1.x - p0.x ) * t; // a = lerp( p0, p1, t );
			float ay = p0.y + ( p1.y - p0.y ) * t;
			float bx = p1.x + ( p2.x - p1.x ) * t; // b = lerp( p1, p2, t );
			float by = p1.y + ( p2.y - p1.y ) * t;
			float cx = p2.x + ( p3.x - p2.x ) * t; // c = lerp( p2, p3, t );
			float cy = p2.y + ( p3.y - p2.y ) * t;
			float dx = ax + ( bx - ax ) * t; // d = lerp( a, b, t );
			float dy = ay + ( by - ay ) * t;
			float ex = bx + ( cx - bx ) * t; // e = lerp( b, c, t );
			float ey = by + ( cy - by ) * t;
			return (
				new Vector2( // first derivative
					3 * ( ex - dx ),
					3 * ( ey - dy )
				),
				new Vector2( // second derivative
					6 * ( ax - 2 * bx + cx ),
					6 * ( ay - 2 * by + cy )
				)
			);
		}
	}

	public partial struct BezierCubic3D {
		/// <summary>Returns the first two derivatives at the given t-value on the curve</summary>
		public (Vector3, Vector3) GetFirstTwoDerivatives( float t ) { // GetDerivative(t) and GetSecondDerivative(t) unrolled shared code
			float ax = p0.x + ( p1.x - p0.x ) * t; // a = lerp( p0, p1, t );
			float ay = p0.y + ( p1.y - p0.y ) * t;
			float az = p0.z + ( p1.z - p0.z ) * t;
			float bx = p1.x + ( p2.x - p1.x ) * t; // b = lerp( p1, p2, t );
			float by = p1.y + ( p2.y - p1.y ) * t;
			float bz = p1.z + ( p2.z - p1.z ) * t;
			float cx = p2.x + ( p3.x - p2.x ) * t; // c = lerp( p2, p3, t );
			float cy = p2.y + ( p3.y - p2.y ) * t;
			float cz = p2.z + ( p3.z - p2.z ) * t;
			float dx = ax + ( bx - ax ) * t; // d = lerp( a, b, t );
			float dy = ay + ( by - ay ) * t;
			float dz = az + ( bz - az ) * t;
			float ex = bx + ( cx - bx ) * t; // e = lerp( b, c, t );
			float ey = by + ( cy - by ) * t;
			float ez = bz + ( cz - bz ) * t;
			return (
				new Vector3( // first derivative
					3 * ( ex - dx ),
					3 * ( ey - dy ),
					3 * ( ez - dz )
				),
				new Vector3( // second derivative
					6 * ( ax - 2 * bx + cx ),
					6 * ( ay - 2 * by + cy ),
					6 * ( az - 2 * bz + cz )
				)
			);
		}
	}

	#endregion

	#region Derivative, Second Derivative & Third derivative combo

	public partial struct BezierCubic2D {
		/// <summary>Returns all three derivatives at the given t-value on the curve</summary>
		public (Vector2, Vector2, Vector2) GetAllThreeDerivatives( float t ) { // GetDerivative(t), GetSecondDerivative(t) and GetThirdDerivative(t) unrolled shared code
			float ax = p0.x + ( p1.x - p0.x ) * t; // a = lerp( p0, p1, t );
			float ay = p0.y + ( p1.y - p0.y ) * t;
			float bx = p1.x + ( p2.x - p1.x ) * t; // b = lerp( p1, p2, t );
			float by = p1.y + ( p2.y - p1.y ) * t;
			float cx = p2.x + ( p3.x - p2.x ) * t; // c = lerp( p2, p3, t );
			float cy = p2.y + ( p3.y - p2.y ) * t;
			float dx = ax + ( bx - ax ) * t; // d = lerp( a, b, t );
			float dy = ay + ( by - ay ) * t;
			float ex = bx + ( cx - bx ) * t; // e = lerp( b, c, t );
			float ey = by + ( cy - by ) * t;
			return (
				new Vector2( // first derivative
					3 * ( ex - dx ),
					3 * ( ey - dy )
				),
				new Vector2( // second derivative
					6 * ( ax - 2 * bx + cx ),
					6 * ( ay - 2 * by + cy )
				),
				GetThirdDerivative( t )
			);
		}
	}

	public partial struct BezierCubic3D {
		/// <summary>Returns all three derivatives at the given t-value on the curve</summary>
		public (Vector3, Vector3, Vector3) GetAllThreeDerivatives( float t ) { // GetDerivative(t), GetSecondDerivative(t) and GetThirdDerivative(t) unrolled shared code
			float ax = p0.x + ( p1.x - p0.x ) * t; // a = lerp( p0, p1, t );
			float ay = p0.y + ( p1.y - p0.y ) * t;
			float az = p0.z + ( p1.z - p0.z ) * t;
			float bx = p1.x + ( p2.x - p1.x ) * t; // b = lerp( p1, p2, t );
			float by = p1.y + ( p2.y - p1.y ) * t;
			float bz = p1.z + ( p2.z - p1.z ) * t;
			float cx = p2.x + ( p3.x - p2.x ) * t; // c = lerp( p2, p3, t );
			float cy = p2.y + ( p3.y - p2.y ) * t;
			float cz = p2.z + ( p3.z - p2.z ) * t;
			float dx = ax + ( bx - ax ) * t; // d = lerp( a, b, t );
			float dy = ay + ( by - ay ) * t;
			float dz = az + ( bz - az ) * t;
			float ex = bx + ( cx - bx ) * t; // e = lerp( b, c, t );
			float ey = by + ( cy - by ) * t;
			float ez = bz + ( cz - bz ) * t;
			return (
				new Vector3( // first derivative
					3 * ( ex - dx ),
					3 * ( ey - dy ),
					3 * ( ez - dz )
				),
				new Vector3( // second derivative
					6 * ( ax - 2 * bx + cx ),
					6 * ( ay - 2 * by + cy ),
					6 * ( az - 2 * bz + cz )
				),
				GetThirdDerivative( t )
			);
		}
	}

	#endregion

	#region Point, Derivative & Second Derivative combo

	public partial struct BezierCubic2D {
		/// <summary>Returns the point and the first two derivatives at the given t-value on the curve. This is faster than calling them separately</summary>
		public (Vector2, Vector2, Vector2) GetPointAndFirstTwoDerivatives( float t ) { // GetPoint(t), GetDerivative(t) and GetSecondDerivative(t) unrolled shared code
			float ax = p0.x + ( p1.x - p0.x ) * t; // a = lerp( p0, p1, t );
			float ay = p0.y + ( p1.y - p0.y ) * t;
			float bx = p1.x + ( p2.x - p1.x ) * t; // b = lerp( p1, p2, t );
			float by = p1.y + ( p2.y - p1.y ) * t;
			float cx = p2.x + ( p3.x - p2.x ) * t; // c = lerp( p2, p3, t );
			float cy = p2.y + ( p3.y - p2.y ) * t;
			float dx = ax + ( bx - ax ) * t; // d = lerp( a, b, t );
			float dy = ay + ( by - ay ) * t;
			float ex = bx + ( cx - bx ) * t; // e = lerp( b, c, t );
			float ey = by + ( cy - by ) * t;
			return (
				new Vector2( // point
					dx + ( ex - dx ) * t,
					dy + ( ey - dy ) * t
				),
				new Vector2( // first derivative
					3 * ( ex - dx ),
					3 * ( ey - dy )
				),
				new Vector2( // second derivative
					6 * ( ax - 2 * bx + cx ),
					6 * ( ay - 2 * by + cy )
				)
			);
		}
	}

	public partial struct BezierCubic3D {
		/// <summary>Returns the point and the first two derivatives at the given t-value on the curve. This is faster than calling them separately</summary>
		public (Vector3, Vector3, Vector3) GetPointAndFirstTwoDerivatives( float t ) { // GetPoint(t), GetDerivative(t) and GetSecondDerivative(t) unrolled shared code
			float ax = p0.x + ( p1.x - p0.x ) * t; // a = lerp( p0, p1, t );
			float ay = p0.y + ( p1.y - p0.y ) * t;
			float az = p0.z + ( p1.z - p0.z ) * t;
			float bx = p1.x + ( p2.x - p1.x ) * t; // b = lerp( p1, p2, t );
			float by = p1.y + ( p2.y - p1.y ) * t;
			float bz = p1.z + ( p2.z - p1.z ) * t;
			float cx = p2.x + ( p3.x - p2.x ) * t; // c = lerp( p2, p3, t );
			float cy = p2.y + ( p3.y - p2.y ) * t;
			float cz = p2.z + ( p3.z - p2.z ) * t;
			float dx = ax + ( bx - ax ) * t; // d = lerp( a, b, t );
			float dy = ay + ( by - ay ) * t;
			float dz = az + ( bz - az ) * t;
			float ex = bx + ( cx - bx ) * t; // e = lerp( b, c, t );
			float ey = by + ( cy - by ) * t;
			float ez = bz + ( cz - bz ) * t;
			return (
				new Vector3( // point
					dx + ( ex - dx ) * t,
					dy + ( ey - dy ) * t,
					dz + ( ez - dz ) * t
				),
				new Vector3( // first derivative
					3 * ( ex - dx ),
					3 * ( ey - dy ),
					3 * ( ez - dz )
				),
				new Vector3( // second derivative
					6 * ( ax - 2 * bx + cx ),
					6 * ( ay - 2 * by + cy ),
					6 * ( az - 2 * bz + cz )
				)
			);
		}
	}

	#endregion
	// Misc esoteric mathy stuff

	#region Polynomial Factors

	public partial struct BezierCubic2D {
		/// <summary>Returns the factors of the derivative polynomials, per-component, in the form at²+bt+c</summary>
		public (Vector2 a, Vector2 b, Vector2 c) GetDerivativeFactors() {
			Vector2 a, b, c;
			( a.x, b.x, c.x ) = BezierUtils.GetCubicDerivativeFactors( p0.x, p1.x, p2.x, p3.x );
			( a.y, b.y, c.y ) = BezierUtils.GetCubicDerivativeFactors( p0.y, p1.y, p2.y, p3.y );
			return ( a, b, c );
		}

		/// <summary>Returns the factors of the second derivative polynomials, per-component, in the form at+b</summary>
		public (Vector2 a, Vector2 b) GetSecondDerivativeFactors() {
			Vector2 a, b;
			( a.x, b.x ) = BezierUtils.GetCubicSecondDerivativeFactors( p0.x, p1.x, p2.x, p3.x );
			( a.y, b.y ) = BezierUtils.GetCubicSecondDerivativeFactors( p0.y, p1.y, p2.y, p3.y );
			return ( a, b );
		}
	}

	public partial struct BezierCubic3D {
		/// <summary>Returns the factors of the derivative polynomials, per-component, in the form at²+bt+c</summary>
		public (Vector3 a, Vector3 b, Vector3 c) GetDerivativeFactors() {
			Vector3 a, b, c;
			( a.x, b.x, c.x ) = BezierUtils.GetCubicDerivativeFactors( p0.x, p1.x, p2.x, p3.x );
			( a.y, b.y, c.y ) = BezierUtils.GetCubicDerivativeFactors( p0.y, p1.y, p2.y, p3.y );
			( a.z, b.z, c.z ) = BezierUtils.GetCubicDerivativeFactors( p0.z, p1.z, p2.z, p3.z );
			return ( a, b, c );
		}

		/// <summary>Returns the factors of the second derivative polynomials, per-component, in the form at+b</summary>
		public (Vector3 a, Vector3 b) GetSecondDerivativeFactors() {
			Vector3 a, b;
			( a.x, b.x ) = BezierUtils.GetCubicSecondDerivativeFactors( p0.x, p1.x, p2.x, p3.x );
			( a.y, b.y ) = BezierUtils.GetCubicSecondDerivativeFactors( p0.y, p1.y, p2.y, p3.y );
			( a.z, b.z ) = BezierUtils.GetCubicSecondDerivativeFactors( p0.z, p1.z, p2.z, p3.z );
			return ( a, b );
		}
	}

	#endregion
}