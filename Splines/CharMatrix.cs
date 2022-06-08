// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using UnityEngine;

namespace Freya {

	public static class CharMatrix {

		/// <summary>The characteristic matrix of a quadratic bézier curve</summary>
		public static readonly RationalMatrix3x3 quadraticBezier = new(
			1, 0, 0,
			-2, 2, 0,
			1, -2, 1
		);

		/// <summary>The characteristic matrix of a cubic bézier curve</summary>
		public static readonly RationalMatrix4x4 cubicBezier = new(
			1, 0, 0, 0,
			-3, 3, 0, 0,
			3, -6, 3, 0,
			-1, 3, -3, 1
		);

		/// <summary>The characteristic matrix of a uniform cubic hermite curve</summary>
		public static readonly RationalMatrix4x4 cubicHermite = new(
			1, 0, 0, 0,
			0, 1, 0, 0,
			-3, -2, 3, -1,
			2, 1, -2, 1
		);

		/// <summary>The characteristic matrix of a uniform cubic catmull-rom curve</summary>
		public static readonly RationalMatrix4x4 cubicCatmullRom = new RationalMatrix4x4(
			0, 2, 0, 0,
			-1, 0, 1, 0,
			2, -5, 4, -1,
			-1, 3, -3, 1
		) / 2;

		/// <summary>The characteristic matrix of a uniform cubic B-spline curve</summary>
		public static readonly RationalMatrix4x4 cubicUniformBspline = new RationalMatrix4x4(
			1, 4, 1, 0,
			-3, 0, 3, 0,
			3, -6, 3, 0,
			-1, 3, -3, 1
		) / 6;

		/// <summary>The inverse characteristic matrix of a cubic bézier curve</summary>
		public static readonly RationalMatrix4x4 cubicBezierInverse = cubicBezier.Inverse;

		/// <summary>The characteristic matrix of a uniform cubic hermite curve</summary>
		public static readonly RationalMatrix4x4 cubicHermiteInverse = cubicHermite.Inverse;

		/// <summary>The characteristic matrix of a uniform cubic catmull-rom curve</summary>
		public static readonly RationalMatrix4x4 cubicCatmullRomInverse = cubicCatmullRom.Inverse;

		/// <summary>The characteristic matrix of a uniform cubic B-spline curve</summary>
		public static readonly RationalMatrix4x4 cubicUniformBsplineInverse = cubicUniformBspline.Inverse;

		/// <summary>Returns the matrix to convert control points from one cubic spline to another, keeping the same curve intact</summary>
		/// <param name="from">The characteristic matrix of the spline to convert from</param>
		/// <param name="to">The characteristic matrix of the spline to convert from</param>
		public static RationalMatrix4x4 GetConversionMatrix( RationalMatrix4x4 from, RationalMatrix4x4 to ) => to.Inverse * from;

		public static Matrix4x4 Create( float m00, float m01, float m02, float m03, float m10, float m11, float m12, float m13, float m20, float m21, float m22, float m23, float m30, float m31, float m32, float m33 ) =>
			new(
				new Vector4( m00, m10, m20, m30 ),
				new Vector4( m01, m11, m21, m31 ),
				new Vector4( m02, m12, m22, m32 ),
				new Vector4( m03, m13, m23, m33 )
			);

		/// <summary>Returns the polynomial representing the curve of a given characteristic matrix of a spline, given 4 control points</summary>
		/// <param name="c">The characteristic matrix to use</param>
		/// <param name="p0">The value of the first control point</param>
		/// <param name="p1">The value of the second control point</param>
		/// <param name="p2">The value of the third control point</param>
		/// <param name="p3">The value of the fourth control point</param>
		public static Polynomial GetSplinePolynomial( RationalMatrix4x4 c, float p0, float p1, float p2, float p3 ) => new Polynomial( c.MultiplyColumnVec( p0, p1, p2, p3 ) );

		/// <inheritdoc cref="GetSplinePolynomial(RationalMatrix4x4,float,float,float,float)"/>
		public static Polynomial2D GetSplinePolynomial( RationalMatrix4x4 c, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3 ) =>
			new(
				GetSplinePolynomial( c, p0.x, p1.x, p2.x, p3.x ),
				GetSplinePolynomial( c, p0.y, p1.y, p2.y, p3.y )
			);

		/// <inheritdoc cref="GetSplinePolynomial(RationalMatrix4x4,float,float,float,float)"/>
		public static Polynomial3D GetSplinePolynomial( RationalMatrix4x4 c, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3 ) =>
			new(
				GetSplinePolynomial( c, p0.x, p1.x, p2.x, p3.x ),
				GetSplinePolynomial( c, p0.y, p1.y, p2.y, p3.y ),
				GetSplinePolynomial( c, p0.z, p1.z, p2.z, p3.z )
			);

		/// <inheritdoc cref="GetSplinePolynomial(RationalMatrix4x4,float,float,float,float)"/>
		public static Polynomial GetSplinePolynomial( RationalMatrix3x3 c, float p0, float p1, float p2 ) => new Polynomial( c.MultiplyColumnVec( p0, p1, p2 ) );

		/// <inheritdoc cref="GetSplinePolynomial(RationalMatrix4x4,float,float,float,float)"/>
		public static Polynomial2D GetSplinePolynomial( RationalMatrix3x3 c, Vector2 p0, Vector2 p1, Vector2 p2 ) =>
			new(
				GetSplinePolynomial( c, p0.x, p1.x, p2.x ),
				GetSplinePolynomial( c, p0.y, p1.y, p2.y )
			);

		/// <inheritdoc cref="GetSplinePolynomial(RationalMatrix4x4,float,float,float,float)"/>
		public static Polynomial3D GetSplinePolynomial( RationalMatrix3x3 c, Vector3 p0, Vector3 p1, Vector3 p2 ) =>
			new(
				GetSplinePolynomial( c, p0.x, p1.x, p2.x ),
				GetSplinePolynomial( c, p0.y, p1.y, p2.y ),
				GetSplinePolynomial( c, p0.z, p1.z, p2.z )
			);


	}

}