// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using System;
using UnityEngine;

namespace Freya {

	/// <summary>A 4x4 matrix using exact rational number representation</summary>
	public readonly struct RationalMatrix4x4 {

		public static readonly RationalMatrix4x4 Identity = new RationalMatrix4x4( 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1 );
		public static readonly RationalMatrix4x4 Zero = new RationalMatrix4x4( 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 );

		public readonly Rational m00, m01, m02, m03;
		public readonly Rational m10, m11, m12, m13;
		public readonly Rational m20, m21, m22, m23;
		public readonly Rational m30, m31, m32, m33;

		public RationalMatrix4x4( Rational m00, Rational m01, Rational m02, Rational m03, Rational m10, Rational m11, Rational m12, Rational m13, Rational m20, Rational m21, Rational m22, Rational m23, Rational m30, Rational m31, Rational m32, Rational m33 ) {
			( this.m00, this.m01, this.m02, this.m03 ) = ( m00, m01, m02, m03 );
			( this.m10, this.m11, this.m12, this.m13 ) = ( m10, m11, m12, m13 );
			( this.m20, this.m21, this.m22, this.m23 ) = ( m20, m21, m22, m23 );
			( this.m30, this.m31, this.m32, this.m33 ) = ( m30, m31, m32, m33 );
		}

		public Rational this[ int row, int column ] {
			get {
				return ( row, column ) switch {
					(0, 0) => m00,
					(0, 1) => m01,
					(0, 2) => m02,
					(0, 3) => m03,
					(1, 0) => m10,
					(1, 1) => m11,
					(1, 2) => m12,
					(1, 3) => m13,
					(2, 0) => m20,
					(2, 1) => m21,
					(2, 2) => m22,
					(2, 3) => m23,
					(3, 0) => m30,
					(3, 1) => m31,
					(3, 2) => m32,
					(3, 3) => m33,
					_      => throw new IndexOutOfRangeException( $"Matrix row/column indices have to be from 0 to 3, got: ({row},{column})" )
				};
			}
		}

		/// <summary>Returns the inverse of this matrix. Throws a division by zero exception if it's not invertible</summary>
		public RationalMatrix4x4 Inverse {
			get {
				// source: https://stackoverflow.com/questions/1148309/inverting-a-4x4-matrix
				Rational A2323 = m22 * m33 - m23 * m32;
				Rational A1323 = m21 * m33 - m23 * m31;
				Rational A1223 = m21 * m32 - m22 * m31;
				Rational A0323 = m20 * m33 - m23 * m30;
				Rational A0223 = m20 * m32 - m22 * m30;
				Rational A0123 = m20 * m31 - m21 * m30;
				Rational det = m00 * ( m11 * A2323 - m12 * A1323 + m13 * A1223 )
							   - m01 * ( m10 * A2323 - m12 * A0323 + m13 * A0223 )
							   + m02 * ( m10 * A1323 - m11 * A0323 + m13 * A0123 )
							   - m03 * ( m10 * A1223 - m11 * A0223 + m12 * A0123 );

				if( det == Rational.Zero )
					throw new DivideByZeroException( "The matrix is not invertible - its determinant is 0" );

				Rational A2313 = m12 * m33 - m13 * m32;
				Rational A1313 = m11 * m33 - m13 * m31;
				Rational A1213 = m11 * m32 - m12 * m31;
				Rational A2312 = m12 * m23 - m13 * m22;
				Rational A1312 = m11 * m23 - m13 * m21;
				Rational A1212 = m11 * m22 - m12 * m21;
				Rational A0313 = m10 * m33 - m13 * m30;
				Rational A0213 = m10 * m32 - m12 * m30;
				Rational A0312 = m10 * m23 - m13 * m20;
				Rational A0212 = m10 * m22 - m12 * m20;
				Rational A0113 = m10 * m31 - m11 * m30;
				Rational A0112 = m10 * m21 - m11 * m20;

				return new RationalMatrix4x4(
					( m11 * A2323 - m12 * A1323 + m13 * A1223 ), -( m01 * A2323 - m02 * A1323 + m03 * A1223 ), ( m01 * A2313 - m02 * A1313 + m03 * A1213 ), -( m01 * A2312 - m02 * A1312 + m03 * A1212 ),
					-( m10 * A2323 - m12 * A0323 + m13 * A0223 ), ( m00 * A2323 - m02 * A0323 + m03 * A0223 ), -( m00 * A2313 - m02 * A0313 + m03 * A0213 ), ( m00 * A2312 - m02 * A0312 + m03 * A0212 ),
					( m10 * A1323 - m11 * A0323 + m13 * A0123 ), -( m00 * A1323 - m01 * A0323 + m03 * A0123 ), ( m00 * A1313 - m01 * A0313 + m03 * A0113 ), -( m00 * A1312 - m01 * A0312 + m03 * A0112 ),
					-( m10 * A1223 - m11 * A0223 + m12 * A0123 ), ( m00 * A1223 - m01 * A0223 + m02 * A0123 ), -( m00 * A1213 - m01 * A0213 + m02 * A0113 ), ( m00 * A1212 - m01 * A0212 + m02 * A0112 )
				) / det;
			}
		}

		/// <summary>Returns the determinant of this matrix</summary>
		public Rational Determinant {
			get {
				// source: https://stackoverflow.com/questions/1148309/inverting-a-4x4-matrix
				Rational A2323 = m22 * m33 - m23 * m32;
				Rational A1323 = m21 * m33 - m23 * m31;
				Rational A1223 = m21 * m32 - m22 * m31;
				Rational A0323 = m20 * m33 - m23 * m30;
				Rational A0223 = m20 * m32 - m22 * m30;
				Rational A0123 = m20 * m31 - m21 * m30;
				return m00 * ( m11 * A2323 - m12 * A1323 + m13 * A1223 )
					   - m01 * ( m10 * A2323 - m12 * A0323 + m13 * A0223 )
					   + m02 * ( m10 * A1323 - m11 * A0323 + m13 * A0123 )
					   - m03 * ( m10 * A1223 - m11 * A0223 + m12 * A0123 );
			}
		}

		public override string ToString() => ToStringMatrix().ToValueTableString();

		public string[,] ToStringMatrix() {
			return new[,] {
				{ m00.ToString(), m01.ToString(), m02.ToString(), m03.ToString() },
				{ m10.ToString(), m11.ToString(), m12.ToString(), m13.ToString() },
				{ m20.ToString(), m21.ToString(), m22.ToString(), m23.ToString() },
				{ m30.ToString(), m31.ToString(), m32.ToString(), m33.ToString() }
			};
		}

		public static explicit operator Matrix4x4( RationalMatrix4x4 c ) {
			return CharMatrix.Create(
				(float)c.m00, (float)c.m01, (float)c.m02, (float)c.m03,
				(float)c.m10, (float)c.m11, (float)c.m12, (float)c.m13,
				(float)c.m20, (float)c.m21, (float)c.m22, (float)c.m23,
				(float)c.m30, (float)c.m31, (float)c.m32, (float)c.m33
			);
		}

		public static RationalMatrix4x4 operator *( RationalMatrix4x4 c, Rational v ) =>
			new(c.m00 * v, c.m01 * v, c.m02 * v, c.m03 * v,
				c.m10 * v, c.m11 * v, c.m12 * v, c.m13 * v,
				c.m20 * v, c.m21 * v, c.m22 * v, c.m23 * v,
				c.m30 * v, c.m31 * v, c.m32 * v, c.m33 * v);

		public static explicit operator RationalMatrix4x4( RationalMatrix3x3 c ) =>
			new(c.m00, c.m01, c.m02, 0,
				c.m10, c.m11, c.m12, 0,
				c.m20, c.m21, c.m22, 0,
				0, 0, 0, 1);


		public static RationalMatrix4x4 operator /( RationalMatrix4x4 c, Rational v ) => c * v.Reciprocal;

		public static RationalMatrix4x4 operator *( RationalMatrix4x4 a, RationalMatrix4x4 b ) {
			Rational GetEntry( int r, int c ) => a[r, 0] * b[0, c] + a[r, 1] * b[1, c] + a[r, 2] * b[2, c] + a[r, 3] * b[3, c];

			return new RationalMatrix4x4(
				GetEntry( 0, 0 ), GetEntry( 0, 1 ), GetEntry( 0, 2 ), GetEntry( 0, 3 ),
				GetEntry( 1, 0 ), GetEntry( 1, 1 ), GetEntry( 1, 2 ), GetEntry( 1, 3 ),
				GetEntry( 2, 0 ), GetEntry( 2, 1 ), GetEntry( 2, 2 ), GetEntry( 2, 3 ),
				GetEntry( 3, 0 ), GetEntry( 3, 1 ), GetEntry( 3, 2 ), GetEntry( 3, 3 )
			);
		}

		/// <summary>Multiplies this matrix C by a column matrix M</summary>
		/// <param name="c">The left hand side 4x4 matrix</param>
		/// <param name="m">The right hand side 4x1 matrix</param>
		public static Matrix4x1 operator *( RationalMatrix4x4 c, Matrix4x1 m ) =>
			new(m.m0 * c.m00 + m.m1 * c.m01 + m.m2 * c.m02 + m.m3 * c.m03,
				m.m0 * c.m10 + m.m1 * c.m11 + m.m2 * c.m12 + m.m3 * c.m13,
				m.m0 * c.m20 + m.m1 * c.m21 + m.m2 * c.m22 + m.m3 * c.m23,
				m.m0 * c.m30 + m.m1 * c.m31 + m.m2 * c.m32 + m.m3 * c.m33);

		/// <inheritdoc cref="operator*(RationalMatrix4x4,Matrix4x1)"/>
		public static Vector2Matrix4x1 operator *( RationalMatrix4x4 c, Vector2Matrix4x1 m ) => new(c * m.X, c * m.Y);

		/// <inheritdoc cref="operator*(RationalMatrix4x4,Matrix4x1)"/>
		public static Vector3Matrix4x1 operator *( RationalMatrix4x4 c, Vector3Matrix4x1 m ) => new(c * m.X, c * m.Y, c * m.Z);

		/// <inheritdoc cref="operator*(RationalMatrix4x4,Matrix4x1)"/>
		public static Vector4Matrix4x1 operator *( RationalMatrix4x4 c, Vector4Matrix4x1 m ) => new(c * m.X, c * m.Y, c * m.Z, c * m.W);

	}

}