// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using System;
using UnityEngine;

namespace Freya {

	/// <summary>A 4x4 matrix using exact rational number representation</summary>
	public struct RationalMatrix4x4 {

		public Rational m00, m01, m02, m03;
		public Rational m10, m11, m12, m13;
		public Rational m20, m21, m22, m23;
		public Rational m30, m31, m32, m33;

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
			set {
				switch( ( row, column ) ) {
					case (0, 0):
						m00 = value;
						break;
					case (0, 1):
						m01 = value;
						break;
					case (0, 2):
						m02 = value;
						break;
					case (0, 3):
						m03 = value;
						break;
					case (1, 0):
						m10 = value;
						break;
					case (1, 1):
						m11 = value;
						break;
					case (1, 2):
						m12 = value;
						break;
					case (1, 3):
						m13 = value;
						break;
					case (2, 0):
						m20 = value;
						break;
					case (2, 1):
						m21 = value;
						break;
					case (2, 2):
						m22 = value;
						break;
					case (2, 3):
						m23 = value;
						break;
					case (3, 0):
						m30 = value;
						break;
					case (3, 1):
						m31 = value;
						break;
					case (3, 2):
						m32 = value;
						break;
					case (3, 3):
						m33 = value;
						break;
					default: throw new IndexOutOfRangeException( $"Matrix row/column indices have to be from 0 to 3, got: ({row},{column})" );
				}
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

		public static RationalMatrix4x4 operator *( RationalMatrix4x4 c, Rational v ) =>
			new(c.m00 * v, c.m01 * v, c.m02 * v, c.m03 * v,
				c.m10 * v, c.m11 * v, c.m12 * v, c.m13 * v,
				c.m20 * v, c.m21 * v, c.m22 * v, c.m23 * v,
				c.m30 * v, c.m31 * v, c.m32 * v, c.m33 * v);


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

		/// <inheritdoc cref="MultiplyColumnVec(float,float,float,float)"/>
		public (Vector3, Vector3, Vector3, Vector3) MultiplyColumnVec( Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3 ) {
			( float x0, float x1, float x2, float x3 ) = MultiplyColumnVec( p0.x, p1.x, p2.x, p3.x );
			( float y0, float y1, float y2, float y3 ) = MultiplyColumnVec( p0.y, p1.y, p2.y, p3.y );
			( float z0, float z1, float z2, float z3 ) = MultiplyColumnVec( p0.z, p1.z, p2.z, p3.z );
			return (
				new Vector3( x0, y0, z0 ),
				new Vector3( x1, y1, z1 ),
				new Vector3( x2, y2, z2 ),
				new Vector3( x3, y3, z3 )
			);
		}

		/// <summary>Returns the basis function (weight) for the given point by index <c>i</c>,
		/// equal to the t-matrix multiplied by the characteristic matrix</summary>
		/// <param name="i">The point index to get the basis function of</param>
		public Polynomial GetBasisFunction( int i ) {
			return i switch {
				0 => new Polynomial( (float)m00, (float)m10, (float)m20, (float)m30 ),
				1 => new Polynomial( (float)m01, (float)m11, (float)m21, (float)m31 ),
				2 => new Polynomial( (float)m02, (float)m12, (float)m22, (float)m32 ),
				3 => new Polynomial( (float)m03, (float)m13, (float)m23, (float)m33 ),
				_ => throw new IndexOutOfRangeException( "Basis index needs to be between 0 and 3" )
			};
		}

	}

}