// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using System;
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

		/// <summary>The inverse characteristic matrix of a quadratic bézier curve</summary>
		public static readonly RationalMatrix3x3 quadraticBezierInverse = quadraticBezier.Inverse;

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

		/// <summary>Returns the basis function (weight) for the given spline points by index <c>i</c>,
		/// equal to the t-matrix multiplied by the characteristic matrix</summary>
		/// <param name="c">The characteristic matrix to get the basis functions of</param>
		/// <param name="i">The point index to get the basis function of</param>
		public static Polynomial GetBasisFunction( RationalMatrix4x4 c, int i ) {
			return i switch {
				0 => new Polynomial( (float)c.m00, (float)c.m10, (float)c.m20, (float)c.m30 ),
				1 => new Polynomial( (float)c.m01, (float)c.m11, (float)c.m21, (float)c.m31 ),
				2 => new Polynomial( (float)c.m02, (float)c.m12, (float)c.m22, (float)c.m32 ),
				3 => new Polynomial( (float)c.m03, (float)c.m13, (float)c.m23, (float)c.m33 ),
				_ => throw new IndexOutOfRangeException( "Basis index needs to be between 0 and 3" )
			};
		}
		
		/// <inheritdoc cref="GetBasisFunction(RationalMatrix4x4,int)"/>
		public static Polynomial GetBasisFunction( Matrix4x4 c, int i ) {
			return i switch {
				0 => new Polynomial( c.m00, c.m10, c.m20, c.m30 ),
				1 => new Polynomial( c.m01, c.m11, c.m21, c.m31 ),
				2 => new Polynomial( c.m02, c.m12, c.m22, c.m32 ),
				3 => new Polynomial( c.m03, c.m13, c.m23, c.m33 ),
				_ => throw new IndexOutOfRangeException( "Basis index needs to be between 0 and 3" )
			};
		}


	}

}