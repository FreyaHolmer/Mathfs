using System;
using UnityEngine;

namespace Freya {


	public readonly struct CharMatrix {

		/// <summary>The characteristic matrix of a quadratic bézier curve</summary>
		public static readonly CharMatrix3x3 quadraticBezier = new(
			1, 0, 0,
			-2, 2, 0,
			1, -2, 1
		);

		/// <summary>The characteristic matrix of a cubic bézier curve</summary>
		public static readonly CharMatrix4x4 cubicBezier = new(
			1, 0, 0, 0,
			-3, 3, 0, 0,
			3, -6, 3, 0,
			-1, 3, -3, 1
		);

		public static readonly CharMatrix4x4 cubicBezierInverse = new CharMatrix4x4(
			3, 0, 0, 0,
			3, 1, 0, 0,
			3, 2, 1, 0,
			3, 3, 3, 3
		) / 3;

		/// <summary>The characteristic matrix of a uniform cubic hermite curve</summary>
		public static readonly CharMatrix4x4 cubicHermite = new(
			1, 0, 0, 0,
			0, 1, 0, 0,
			-3, -2, 3, -1,
			2, 1, -2, 1
		);

		public static readonly CharMatrix4x4 cubicHermiteInverse = new(
			1, 0, 0, 0,
			0, 1, 0, 0,
			1, 1, 1, 1,
			0, 1, 2, 3
		);

		/// <summary>The characteristic matrix of a uniform cubic catmull-rom curve</summary>
		public static readonly CharMatrix4x4 cubicCatmullRom = new CharMatrix4x4(
			0, 2, 0, 0,
			-1, 0, 1, 0,
			2, -5, 4, -1,
			-1, 3, -3, 1
		) / 2;

		public static readonly CharMatrix4x4 cubicCatmullRomInverse = new CharMatrix4x4(
			1, -1, 1, 1,
			1, 0, 0, 0,
			1, 1, 1, 1,
			1, 2, 4, 6
		);

		/// <summary>The characteristic matrix of a uniform cubic B-spline segment</summary>
		public static readonly CharMatrix4x4 cubicUniformBspline = new CharMatrix4x4(
			1, 4, 1, 0,
			-3, 0, 3, 0,
			3, -6, 3, 0,
			-1, 3, -3, 1
		) / 6;

		public static readonly CharMatrix4x4 cubicUniformBsplineInverse = new CharMatrix4x4(
			3, -3, 2, 0,
			3, 0, -1, 0,
			3, 3, 2, 0,
			3, 6, 11, 18
		) / 3;

	}

	/// <summary>Data structure representing a cubic characteristic matrix with 4 points. Used for spline evaluation</summary>
	public readonly struct CharMatrix4x4 {
		public readonly float m00, m01, m02, m03;
		public readonly float m10, m11, m12, m13;
		public readonly float m20, m21, m22, m23;
		public readonly float m30, m31, m32, m33;

		public CharMatrix4x4( float m00, float m01, float m02, float m03, float m10, float m11, float m12, float m13, float m20, float m21, float m22, float m23, float m30, float m31, float m32, float m33 ) {
			( this.m00, this.m01, this.m02, this.m03 ) = ( m00, m01, m02, m03 );
			( this.m10, this.m11, this.m12, this.m13 ) = ( m10, m11, m12, m13 );
			( this.m20, this.m21, this.m22, this.m23 ) = ( m20, m21, m22, m23 );
			( this.m30, this.m31, this.m32, this.m33 ) = ( m30, m31, m32, m33 );
		}

		/// <summary>Returns the basis function (weight) for the given point by index <c>i</c>,
		/// equal to the t-matrix multiplied by the characteristic matrix</summary>
		/// <param name="i">The point index to get the basis function of</param>
		public Polynomial GetBasisFunction( int i ) {
			return i switch {
				0 => new Polynomial( m30, m20, m10, m00 ),
				1 => new Polynomial( m31, m21, m11, m01 ),
				2 => new Polynomial( m32, m22, m12, m02 ),
				3 => new Polynomial( m33, m23, m13, m03 ),
				_ => throw new IndexOutOfRangeException( "Basis index needs to be between 0 and 3" )
			};
		}

		/// <summary>Returns the polynomial representing the charateristic matrix
		/// multiplied by the input points, on a single axis</summary>
		/// <param name="p0">The value of the first point</param>
		/// <param name="p1">The value of the second point</param>
		/// <param name="p2">The value of the third point</param>
		/// <param name="p3">The value of the fourth point</param>
		public Polynomial GetEvalPolynomial( float p0, float p1, float p2, float p3 ) =>
			new(
				p0 * m30 + p1 * m31 + p2 * m32 + p3 * m33,
				p0 * m20 + p1 * m21 + p2 * m22 + p3 * m23,
				p0 * m10 + p1 * m11 + p2 * m12 + p3 * m13,
				p0 * m00 + p1 * m01 + p2 * m02 + p3 * m03);

		/// <summary>Returns the curve this characteristic matrix represents, given 4 points</summary>
		/// <param name="p0">The first point</param>
		/// <param name="p1">The second point</param>
		/// <param name="p2">The third point</param>
		/// <param name="p3">The fourth point</param>
		public Polynomial2D GetCurve( Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3 ) =>
			new(
				GetEvalPolynomial( p0.x, p1.x, p2.x, p3.x ),
				GetEvalPolynomial( p0.y, p1.y, p2.y, p3.y )
			);

		/// <summary>Returns the curve this characteristic matrix represents, given 4 points</summary>
		/// <param name="p0">The first point</param>
		/// <param name="p1">The second point</param>
		/// <param name="p2">The third point</param>
		/// <param name="p3">The fourth point</param>
		public Polynomial3D GetCurve( Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3 ) =>
			new(
				GetEvalPolynomial( p0.x, p1.x, p2.x, p3.x ),
				GetEvalPolynomial( p0.y, p1.y, p2.y, p3.y ),
				GetEvalPolynomial( p0.z, p1.z, p2.z, p3.z )
			);

		/// <summary>Multiplies this characteristic matrix C by a column matrix: C*[p0,p1,p2,p3]^T</summary>
		/// <param name="p0">The first entry of the column matrix</param>
		/// <param name="p1">The second entry of the column matrix</param>
		/// <param name="p2">The third entry of the column matrix</param>
		/// <param name="p3">The fourth entry of the column matrix</param>
		public (float, float, float, float) MultiplyColumnVec( float p0, float p1, float p2, float p3 ) =>
		(
			p0 * m00 + p1 * m01 + p2 * m02 + p3 * m03,
			p0 * m10 + p1 * m11 + p2 * m12 + p3 * m13,
			p0 * m20 + p1 * m21 + p2 * m22 + p3 * m23,
			p0 * m30 + p1 * m31 + p2 * m32 + p3 * m33
		);

		/// <inheritdoc cref="MultiplyColumnVec(float,float,float,float)"/>
		public (Vector2, Vector2, Vector2, Vector2) MultiplyColumnVec( Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3 ) {
			( float x0, float x1, float x2, float x3 ) = MultiplyColumnVec( p0.x, p1.x, p2.x, p3.x );
			( float y0, float y1, float y2, float y3 ) = MultiplyColumnVec( p0.y, p1.y, p2.y, p3.y );
			return (
				new Vector2( x0, y0 ),
				new Vector2( x1, y1 ),
				new Vector2( x2, y2 ),
				new Vector2( x3, y3 )
			);
		}

		public static CharMatrix4x4 operator *( CharMatrix4x4 c, float v ) =>
			new(c.m00 * v, c.m01 * v, c.m02 * v, c.m03 * v,
				c.m10 * v, c.m11 * v, c.m12 * v, c.m13 * v,
				c.m20 * v, c.m21 * v, c.m22 * v, c.m23 * v,
				c.m30 * v, c.m31 * v, c.m32 * v, c.m33 * v);

		public static CharMatrix4x4 operator /( CharMatrix4x4 c, float v ) => c * ( 1f / v );

		public override string ToString() => $"{m00},\t{m01},\t{m02},\t{m03}\n{m10},\t{m11},\t{m12},\t{m13}\n{m20},\t{m21},\t{m22},\t{m23}\n{m30},\t{m31},\t{m32},\t{m33}\n";

	}

	/// <summary>Data structure representing a quadratic characteristic matrix with 3 points. Used for spline evaluation</summary>
	public readonly struct CharMatrix3x3 {
		public readonly float m00, m01, m02;
		public readonly float m10, m11, m12;
		public readonly float m20, m21, m22;

		public CharMatrix3x3( float m00, float m01, float m02, float m10, float m11, float m12, float m20, float m21, float m22 ) {
			( this.m00, this.m01, this.m02 ) = ( m00, m01, m02 );
			( this.m10, this.m11, this.m12 ) = ( m10, m11, m12 );
			( this.m20, this.m21, this.m22 ) = ( m20, m21, m22 );
		}

		/// <inheritdoc cref="CharMatrix4x4.GetBasisFunction(int)"/>
		public Polynomial GetBasisFunction( int i ) {
			return i switch {
				0 => Polynomial.Quadratic( m20, m10, m00 ),
				1 => Polynomial.Quadratic( m21, m11, m01 ),
				2 => Polynomial.Quadratic( m22, m12, m02 ),
				_ => throw new IndexOutOfRangeException( "Basis index needs to be between 0 and 2" )
			};
		}

		/// <inheritdoc cref="CharMatrix4x4.GetEvalPolynomial(float,float,float,float)"/>
		public Polynomial GetEvalPolynomial( float p0, float p1, float p2 ) =>
			Polynomial.Quadratic(
				p0 * m20 + p1 * m21 + p2 * m22,
				p0 * m10 + p1 * m11 + p2 * m12,
				p0 * m00 + p1 * m01 + p2 * m02 );

		/// <inheritdoc cref="CharMatrix4x4.GetCurve(Vector2,Vector2,Vector2,Vector2)"/>
		public Polynomial2D GetCurve( Vector2 p0, Vector2 p1, Vector2 p2 ) =>
			new(
				GetEvalPolynomial( p0.x, p1.x, p2.x ),
				GetEvalPolynomial( p0.y, p1.y, p2.y )
			);

		/// <inheritdoc cref="CharMatrix4x4.GetCurve(Vector2,Vector2,Vector2,Vector2)"/>
		public Polynomial3D GetCurve( Vector3 p0, Vector3 p1, Vector3 p2 ) =>
			new(
				GetEvalPolynomial( p0.x, p1.x, p2.x ),
				GetEvalPolynomial( p0.y, p1.y, p2.y ),
				GetEvalPolynomial( p0.z, p1.z, p2.z )
			);
	}

}