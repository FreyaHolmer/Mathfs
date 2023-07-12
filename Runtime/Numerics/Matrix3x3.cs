// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using System;
using System.Globalization;
using UnityEngine;

namespace Freya {

	/// <summary>A 3x3 matrix</summary>
	public readonly struct Matrix3x3 {

		public static readonly Matrix3x3 Identity = new Matrix3x3( 1, 0, 0, 0, 1, 0, 0, 0, 1 );
		public static readonly Matrix3x3 Zero = new Matrix3x3( 0, 0, 0, 0, 0, 0, 0, 0, 0 );

		public readonly float m00, m01, m02;
		public readonly float m10, m11, m12;
		public readonly float m20, m21, m22;

		public Matrix3x3( float m00, float m01, float m02, float m10, float m11, float m12, float m20, float m21, float m22 ) {
			( this.m00, this.m01, this.m02 ) = ( m00, m01, m02 );
			( this.m10, this.m11, this.m12 ) = ( m10, m11, m12 );
			( this.m20, this.m21, this.m22 ) = ( m20, m21, m22 );
		}

		public Matrix3x3( Vector3 col0, Vector3 col1, Vector3 col2 ) {
			m00 = col0.x;
			m10 = col0.y;
			m20 = col0.z;
			m01 = col1.x;
			m11 = col1.y;
			m21 = col1.z;
			m02 = col2.x;
			m12 = col2.y;
			m22 = col2.z;
		}

		public Matrix3x3( Matrix4x4 m ) {
			( this.m00, this.m01, this.m02 ) = ( m.m00, m.m01, m.m02 );
			( this.m10, this.m11, this.m12 ) = ( m.m10, m.m11, m.m12 );
			( this.m20, this.m21, this.m22 ) = ( m.m20, m.m21, m.m22 );
		}

		public Matrix3x3( Quaternion q ) {
			Matrix4x4 m = Matrix4x4.Rotate( q );
			( this.m00, this.m01, this.m02 ) = ( m.m00, m.m01, m.m02 );
			( this.m10, this.m11, this.m12 ) = ( m.m10, m.m11, m.m12 );
			( this.m20, this.m21, this.m22 ) = ( m.m20, m.m21, m.m22 );
		}

		public static Matrix3x3 Scale( Vector3 s ) {
			return new Matrix3x3(
				s.x, 0, 0,
				0, s.y, 0,
				0, 0, s.z
			);
		}

		public Matrix3x3 NormalizeColumns() {
			double lenX = Math.Sqrt( m00 * m00 + m10 * m10 + m20 * m20 );
			double lenY = Math.Sqrt( m01 * m01 + m11 * m11 + m21 * m21 );
			double lenZ = Math.Sqrt( m02 * m02 + m12 * m12 + m22 * m22 );
			return new(
				(float)( m00 / lenX ), (float)( m01 / lenY ), (float)( m02 / lenZ ),
				(float)( m10 / lenX ), (float)( m11 / lenY ), (float)( m12 / lenZ ),
				(float)( m20 / lenX ), (float)( m21 / lenY ), (float)( m22 / lenZ ));
		}

		public float this[ int row, int column ] {
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
					_      => throw new IndexOutOfRangeException( $"Matrix row/column indices have to be from 0 to 2, got: ({row},{column})" )
				};
			}
		}

		/// <summary>Returns the inverse of this matrix. Throws a division by zero exception if it's not invertible</summary>
		public Matrix3x3 Inverse {
			get {
				float A1212 = m11 * m22 - m12 * m21;
				float A0212 = m10 * m22 - m12 * m20;
				float A0112 = m10 * m21 - m11 * m20;
				float det = m00 * A1212 - m01 * A0212 + m02 * A0112;

				if( det == 0 )
					throw new DivideByZeroException( "The matrix is not invertible - its determinant is 0" );

				return new Matrix3x3(
					A1212, m02 * m21 - m01 * m22, m01 * m12 - m02 * m11,
					-A0212, m00 * m22 - m02 * m20, m10 * m02 - m00 * m12,
					A0112, m20 * m01 - m00 * m21, m00 * m11 - m10 * m01
				) / det;
			}
		}

		/// <summary>Returns the determinant of this matrix</summary>
		public float Determinant {
			get {
				float A1212 = m11 * m22 - m12 * m21;
				float A0212 = m10 * m22 - m12 * m20;
				float A0112 = m10 * m21 - m11 * m20;
				return m00 * A1212 - m01 * A0212 + m02 * A0112;
			}
		}
		public Matrix3x3 Transpose =>
			new(m00, m10, m20,
				m01, m11, m21,
				m02, m12, m22);

		public override string ToString() => ToStringMatrix().ToValueTableString();

		public string[,] ToStringMatrix() {
			return new[,] {
				{ m00.ToString( CultureInfo.InvariantCulture ), m01.ToString( CultureInfo.InvariantCulture ), m02.ToString( CultureInfo.InvariantCulture ) },
				{ m10.ToString( CultureInfo.InvariantCulture ), m11.ToString( CultureInfo.InvariantCulture ), m12.ToString( CultureInfo.InvariantCulture ) },
				{ m20.ToString( CultureInfo.InvariantCulture ), m21.ToString( CultureInfo.InvariantCulture ), m22.ToString( CultureInfo.InvariantCulture ) }
			};
		}

		public static explicit operator Matrix3x3( Matrix4x4 m ) => new(m.m00, m.m01, m.m02, m.m10, m.m11, m.m12, m.m20, m.m21, m.m22);

		public static Matrix3x3 operator *( Matrix3x3 c, float v ) =>
			new(c.m00 * v, c.m01 * v, c.m02 * v,
				c.m10 * v, c.m11 * v, c.m12 * v,
				c.m20 * v, c.m21 * v, c.m22 * v);

		public static Matrix3x3 operator /( Matrix3x3 c, float v ) => c * ( 1f / v );

		public static Matrix3x3 operator *( Matrix3x3 a, Matrix3x3 b ) {
			float GetEntry( int r, int c ) =>
				a[r, 0] * b[0, c] +
				a[r, 1] * b[1, c] +
				a[r, 2] * b[2, c];

			return new Matrix3x3(
				GetEntry( 0, 0 ), GetEntry( 0, 1 ), GetEntry( 0, 2 ),
				GetEntry( 1, 0 ), GetEntry( 1, 1 ), GetEntry( 1, 2 ),
				GetEntry( 2, 0 ), GetEntry( 2, 1 ), GetEntry( 2, 2 )
			);
		}

		public static Matrix3x1 operator *( Matrix3x3 c, Matrix3x1 m ) =>
			new(m.m0 * c.m00 + m.m1 * c.m01 + m.m2 * c.m02,
				m.m0 * c.m10 + m.m1 * c.m11 + m.m2 * c.m12,
				m.m0 * c.m20 + m.m1 * c.m21 + m.m2 * c.m22);

		public static Vector3 operator *( Matrix3x3 c, Vector3 v ) =>
			new(v.x * c.m00 + v.y * c.m01 + v.z * c.m02,
				v.x * c.m10 + v.y * c.m11 + v.z * c.m12,
				v.x * c.m20 + v.y * c.m21 + v.z * c.m22);

		public static Vector2Matrix3x1 operator *( Matrix3x3 c, Vector2Matrix3x1 m ) => new(c * m.X, c * m.Y);

		public static Vector3Matrix3x1 operator *( Matrix3x3 c, Vector3Matrix3x1 m ) => new(c * m.X, c * m.Y, c * m.Z);

		public static Vector4Matrix3x1 operator *( Matrix3x3 c, Vector4Matrix3x1 m ) => new(c * m.X, c * m.Y, c * m.Z, c * m.W);

		public float AverageScale() {
			return (
				MathF.Sqrt( m00 * m00 + m10 * m10 + m20 * m20 ) +
				MathF.Sqrt( m01 * m01 + m11 * m11 + m21 * m21 ) +
				MathF.Sqrt( m02 * m02 + m12 * m12 + m22 * m22 )
			) / 3;
		}

	}

}