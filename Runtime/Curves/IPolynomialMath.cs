using UnityEngine;

namespace Freya {

	public interface IPolynomialMath<P, V> {
		public P NaN { get; }
		public P FitCubicFrom0( float x1, float x2, float x3, V y0, V y1, V y2, V y3 );
	}

	public struct PolynomialMath1D : IPolynomialMath<Polynomial, float> {
		public Polynomial NaN => Polynomial.NaN;

		/// <inheritdoc cref="Polynomial.FitCubicFrom0(float,float,float,float,float,float,float)"/>
		public Polynomial FitCubicFrom0( float x1, float x2, float x3, float y0, float y1, float y2, float y3 ) => Polynomial.FitCubicFrom0( x1, x2, x3, y0, y1, y2, y3 );
	}

	public struct PolynomialMath2D : IPolynomialMath<Polynomial2D, Vector2> {
		public Polynomial2D NaN => Polynomial2D.NaN;

		/// <inheritdoc cref="Polynomial2D.FitCubicFrom0(float,float,float,Vector2,Vector2,Vector2,Vector2)"/>
		public Polynomial2D FitCubicFrom0( float x1, float x2, float x3, Vector2 y0, Vector2 y1, Vector2 y2, Vector2 y3 ) => Polynomial2D.FitCubicFrom0( x1, x2, x3, y0, y1, y2, y3 );
	}

	public struct PolynomialMath3D : IPolynomialMath<Polynomial3D, Vector3> {
		public Polynomial3D NaN => Polynomial3D.NaN;

		/// <inheritdoc cref="Polynomial3D.FitCubicFrom0(float,float,float,Vector3,Vector3,Vector3,Vector3)"/>
		public Polynomial3D FitCubicFrom0( float x1, float x2, float x3, Vector3 y0, Vector3 y1, Vector3 y2, Vector3 y3 ) => Polynomial3D.FitCubicFrom0( x1, x2, x3, y0, y1, y2, y3 );
	}

	public struct PolynomialMath4D : IPolynomialMath<Polynomial4D, Vector4> {
		public Polynomial4D NaN => Polynomial4D.NaN;

		/// <inheritdoc cref="Polynomial4D.FitCubicFrom0(float,float,float,Vector4,Vector4,Vector4,Vector4)"/>
		public Polynomial4D FitCubicFrom0( float x1, float x2, float x3, Vector4 y0, Vector4 y1, Vector4 y2, Vector4 y3 ) => Polynomial4D.FitCubicFrom0( x1, x2, x3, y0, y1, y2, y3 );

	}

}