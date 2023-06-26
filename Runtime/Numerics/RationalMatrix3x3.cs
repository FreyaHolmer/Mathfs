// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using System;
using UnityEngine;

namespace Freya {

	/// <summary>A 3x3 matrix using exact rational number representation</summary>
	public readonly struct RationalMatrix3x3 {

		public static readonly RationalMatrix3x3 Identity = new RationalMatrix3x3( 1, 0, 0, 0, 1, 0, 0, 0, 1 );
		public static readonly RationalMatrix3x3 Zero = new RationalMatrix3x3( 0, 0, 0, 0, 0, 0, 0, 0, 0 );

		public readonly Rational m00, m01, m02;
		public readonly Rational m10, m11, m12;
		public readonly Rational m20, m21, m22;

		public RationalMatrix3x3( Rational m00, Rational m01, Rational m02, Rational m10, Rational m11, Rational m12, Rational m20, Rational m21, Rational m22 ) {
			( this.m00, this.m01, this.m02 ) = ( m00, m01, m02 );
			( this.m10, this.m11, this.m12 ) = ( m10, m11, m12 );
			( this.m20, this.m21, this.m22 ) = ( m20, m21, m22 );
		}

		public Rational this[ int row, int column ] {
			get {
				return ( row, column ) switch {
					(0, 0) => m00,
					(0, 1) => m01,
					(0, 2) => m02,
					(1, 0) => m10,
					(1, 1) => m11,
					(1, 2) => m12,
					(2, 0) => m20,
					(2, 1) => m21,
					(2, 2) => m22,
					_      => throw new IndexOutOfRangeException( $"Matrix row/column indices have to be from 0 to 3, got: ({row},{column})" )
				};
			}
		}

		/// <summary>Returns the inverse of this matrix. Throws a division by zero exception if it's not invertible</summary>
		public RationalMatrix3x3 Inverse {
			get {
				Rational A1212 = m11 * m22 - m12 * m21;
				Rational A0212 = m10 * m22 - m12 * m20;
				Rational A0112 = m10 * m21 - m11 * m20;
				Rational det = m00 * A1212 - m01 * A0212 + m02 * A0112;

				if( det == Rational.Zero )
					throw new DivideByZeroException( "The matrix is not invertible - its determinant is 0" );

				return new RationalMatrix3x3(
					A1212, m02 * m21 - m01 * m22, m01 * m12 - m02 * m11,
					-A0212, m00 * m22 - m02 * m20, m10 * m02 - m00 * m12,
					A0112, m20 * m01 - m00 * m21, m00 * m11 - m10 * m01
				) / det;
			}
		}

		/// <summary>Returns the determinant of this matrix</summary>
		public Rational Determinant {
			get {
				Rational A1212 = m11 * m22 - m12 * m21;
				Rational A0212 = m10 * m22 - m12 * m20;
				Rational A0112 = m10 * m21 - m11 * m20;
				return m00 * A1212 - m01 * A0212 + m02 * A0112;
			}
		}

		public override string ToString() => ToStringMatrix().ToValueTableString();

		public string[,] ToStringMatrix() {
			return new[,] {
				{ m00.ToString(), m01.ToString(), m02.ToString() },
				{ m10.ToString(), m11.ToString(), m12.ToString() },
				{ m20.ToString(), m21.ToString(), m22.ToString() }
			};
		}

		public static RationalMatrix3x3 operator *( RationalMatrix3x3 c, Rational v ) =>
			new(c.m00 * v, c.m01 * v, c.m02 * v,
				c.m10 * v, c.m11 * v, c.m12 * v,
				c.m20 * v, c.m21 * v, c.m22 * v);

		public static RationalMatrix3x3 operator /( RationalMatrix3x3 c, Rational v ) => c * v.Reciprocal;

		public static RationalMatrix3x3 operator *( RationalMatrix3x3 a, RationalMatrix3x3 b ) {
			Rational GetEntry( int r, int c ) => a[r, 0] * b[0, c] + a[r, 1] * b[1, c] + a[r, 2] * b[2, c] + a[r, 3] * b[3, c];

			return new RationalMatrix3x3(
				GetEntry( 0, 0 ), GetEntry( 0, 1 ), GetEntry( 0, 2 ),
				GetEntry( 1, 0 ), GetEntry( 1, 1 ), GetEntry( 1, 2 ),
				GetEntry( 2, 0 ), GetEntry( 2, 1 ), GetEntry( 2, 2 )
			);
		}

		/// <inheritdoc cref="RationalMatrix4x4.operator*(RationalMatrix4x4,Matrix4x1)"/>
		public static Matrix3x1 operator *( RationalMatrix3x3 c, Matrix3x1 m ) =>
			new(m.m0 * c.m00 + m.m1 * c.m01 + m.m2 * c.m02,
				m.m0 * c.m10 + m.m1 * c.m11 + m.m2 * c.m12,
				m.m0 * c.m20 + m.m1 * c.m21 + m.m2 * c.m22);

		/// <inheritdoc cref="RationalMatrix4x4.operator*(RationalMatrix4x4,Matrix4x1)"/>
		public static Vector2Matrix3x1 operator *( RationalMatrix3x3 c, Vector2Matrix3x1 m ) => new(c * m.X, c * m.Y);

		/// <inheritdoc cref="RationalMatrix4x4.operator*(RationalMatrix4x4,Matrix4x1)"/>
		public static Vector3Matrix3x1 operator *( RationalMatrix3x3 c, Vector3Matrix3x1 m ) => new(c * m.X, c * m.Y, c * m.Z);
		
		/// <inheritdoc cref="RationalMatrix4x4.operator*(RationalMatrix4x4,Matrix4x1)"/>
		public static Vector4Matrix3x1 operator *( RationalMatrix3x3 c, Vector4Matrix3x1 m ) => new(c * m.X, c * m.Y, c * m.Z, c * m.W);

	}

}