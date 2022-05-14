using System;
using UnityEngine;

namespace Freya {

	/// <summary>Data structure representing a characteristic up to a cubic. Used for spline evaluation</summary>
	public readonly struct CharMatrix {

		public readonly float m00, m01, m02, m03;
		public readonly float m10, m11, m12, m13;
		public readonly float m20, m21, m22, m23;
		public readonly float m30, m31, m32, m33;

		public CharMatrix( float m00, float m01, float m02, float m03, float m10, float m11, float m12, float m13, float m20, float m21, float m22, float m23, float m30, float m31, float m32, float m33 ) {
			( this.m00, this.m01, this.m02, this.m03 ) = ( m00, m01, m02, m03 );
			( this.m10, this.m11, this.m12, this.m13 ) = ( m10, m11, m12, m13 );
			( this.m20, this.m21, this.m22, this.m23 ) = ( m20, m21, m22, m23 );
			( this.m30, this.m31, this.m32, this.m33 ) = ( m30, m31, m32, m33 );
		}

		public static readonly CharMatrix cubicBezier = new(
			1, 0, 0, 0,
			-3, 3, 0, 0,
			3, -6, 3, 0,
			-1, 3, -3, 1
		);

		public Polynomial2D GetCurve( Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3 ) =>
			new(
				GetEvalPolynomial( p0.x, p1.x, p2.x, p3.x ),
				GetEvalPolynomial( p0.y, p1.y, p2.y, p3.y )
			);
		public Polynomial3D GetCurve( Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3 ) =>
			new(
				GetEvalPolynomial( p0.x, p1.x, p2.x, p3.x ),
				GetEvalPolynomial( p0.y, p1.y, p2.y, p3.y ),
				GetEvalPolynomial( p0.z, p1.z, p2.z, p3.z )
			);

		/// <summary>Returns the basis function (weight) for the given point by index <c>i</c>,
		/// equal to the t-matrix multiplied by the characteristic matrix</summary>
		/// <param name="i">The point index to get the basis function of</param>
		public Polynomial GetBasisFunction( int i ) {
			return i switch {
				0 => new Polynomial( m30, m20, m10, m00 ),
				1 => new Polynomial( m31, m21, m11, m01 ),
				2 => new Polynomial( m32, m22, m12, m02 ),
				3 => new Polynomial( m33, m23, m13, m03 ),
				_ => throw new IndexOutOfRangeException( "Bézier basis index needs to be between 0 and 3" )
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

	}

}