// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using System;
using UnityEngine;

namespace Freya {

	/// <summary>A 4x4 matrix using exact rational number representation</summary>
	public struct RationalMatrix3x3 {

		public Rational m00, m01, m02;
		public Rational m10, m11, m12;
		public Rational m20, m21, m22;

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
					case (1, 0):
						m10 = value;
						break;
					case (1, 1):
						m11 = value;
						break;
					case (1, 2):
						m12 = value;
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
					default: throw new IndexOutOfRangeException( $"Matrix row/column indices have to be from 0 to 3, got: ({row},{column})" );
				}
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

		/// <inheritdoc cref="CharMatrix4x4.GetEvalPolynomial(float,float,float,float)"/>
		public Polynomial GetEvalPolynomial( float p0, float p1, float p2 ) =>
			Polynomial.Quadratic(
				p0 * m00 + p1 * m01 + p2 * m02,
				p0 * m10 + p1 * m11 + p2 * m12,
				p0 * m20 + p1 * m21 + p2 * m22
			);

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
		/// <summary>Multiplies this characteristic matrix C by a column matrix: C*[p0,p1,p2]^T</summary>
		/// <param name="p0">The first entry of the column matrix</param>
		/// <param name="p1">The second entry of the column matrix</param>
		/// <param name="p2">The third entry of the column matrix</param>
		public (float, float, float) MultiplyColumnVec( float p0, float p1, float p2 ) =>
		(
			p0 * m00 + p1 * m01 + p2 * m02,
			p0 * m10 + p1 * m11 + p2 * m12,
			p0 * m20 + p1 * m21 + p2 * m22
		);

		/// <inheritdoc cref="RationalMatrix4x4.GetBasisFunction(int)"/>
		public Polynomial GetBasisFunction( int i ) {
			return i switch {
				0 => Polynomial.Quadratic( (float)m00, (float)m10, (float)m20 ),
				1 => Polynomial.Quadratic( (float)m01, (float)m11, (float)m21 ),
				2 => Polynomial.Quadratic( (float)m02, (float)m12, (float)m22 ),
				_ => throw new IndexOutOfRangeException( "Basis index needs to be between 0 and 2" )
			};
		}
	}

}