using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Freya {

	/// <summary>An optimized 2D cubic Hermite curve segment</summary>
	[Serializable] public struct Hermite2D : IParamCurve3Diff<Vector2> {

		const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

		/// <summary>Creates a cubic Hermite curve, from two control points and two tangents</summary>
		/// <param name="p0">The starting point of the curve</param>
		/// <param name="m0">The relative tangent vector of the start point</param>
		/// <param name="p1">The end point of the curve</param>
		/// <param name="m1">The relative tangent vector of the end point</param>
		public Hermite2D( Vector2 p0, Vector2 m0, Vector2 p1, Vector2 m1 ) {
			( this.p0, this.m0, this.p1, this.m1 ) = ( p0, m0, p1, m1 );
			validCoefficients = false;
			c3 = c2 = default;
		}

		#region Control Points

		[SerializeField] Vector2 p0, m0, p1, m1; // the points & tangents of the curve

		/// <summary>The starting point of the curve</summary>
		public Vector2 P0 {
			[MethodImpl( INLINE )] get => p0;
			[MethodImpl( INLINE )] set => _ = ( p0 = value, validCoefficients = false );
		}

		/// <summary>The second control point of the curve, sometimes called the start tangent point</summary>
		public Vector2 M0 {
			[MethodImpl( INLINE )] get => m0;
			[MethodImpl( INLINE )] set => _ = ( m0 = value, validCoefficients = false );
		}

		/// <summary>The third control point of the curve, sometimes called the end tangent point</summary>
		public Vector2 P1 {
			[MethodImpl( INLINE )] get => p1;
			[MethodImpl( INLINE )] set => _ = ( p1 = value, validCoefficients = false );
		}

		/// <summary>The end point of the curve</summary>
		public Vector2 M1 {
			[MethodImpl( INLINE )] get => m1;
			[MethodImpl( INLINE )] set => _ = ( m1 = value, validCoefficients = false );
		}

		#endregion

		#region Coefficients

		[NonSerialized] bool validCoefficients; // inverted isDirty flag (can't default to true in structs)
		[NonSerialized] Vector2 c3, c2; // cached coefficients for fast evaluation. c0 = p0. c1 = m0

		// Coefficient Calculation
		[MethodImpl( INLINE )] void ReadyCoefficients() {
			if( validCoefficients )
				return; // no need to update
			validCoefficients = true;
			c3.x = ( 2 * ( p0.x - p1.x ) + m0.x + m1.x );
			c2.x = ( 3 * ( p1.x - p0.x ) - 2 * m0.x - m1.x );
			c3.y = ( 2 * ( p0.y - p1.y ) + m0.y + m1.y );
			c2.y = ( 3 * ( p1.y - p0.y ) - 2 * m0.y - m1.y );
			// c1 = m0
			// c0 = p0
		}

		/// <summary>The constant coefficient when evaluating this curve in the form C3*t³ + C2*t² + C1*t + C0</summary>
		public Vector2 C0 {
			[MethodImpl( INLINE )] get => p0;
		}

		/// <summary>The linear coefficient when evaluating this curve in the form C3*t³ + C2*t² + C1*t + C0</summary>
		public Vector2 C1 {
			[MethodImpl( INLINE )] get => m0;
		}

		/// <summary>The quadratic coefficient when evaluating this curve in the form C3*t³ + C2*t² + C1*t + C0</summary>
		public Vector2 C2 {
			[MethodImpl( INLINE )] get {
				ReadyCoefficients();
				return c2;
			}
		}

		/// <summary>The cubic coefficient when evaluating this curve in the form C3*t³ + C2*t² + C1*t + C0</summary>
		public Vector2 C3 {
			[MethodImpl( INLINE )] get {
				ReadyCoefficients();
				return c3;
			}
		}

		/// <summary>The polynomial coefficients in the form c3*t³ + c2*t² + c1*t + c0</summary>
		[MethodImpl( INLINE )] public (Vector2 c3, Vector2 c2, Vector2 c1, Vector2 c0) GetCoefficients() {
			ReadyCoefficients();
			return ( c3, c2, m0, p0 );
		}

		#endregion

		#region Core IParamCurve Implementations

		public int Degree {
			[MethodImpl( INLINE )] get => 3;
		}
		public int Count {
			[MethodImpl( INLINE )] get => 4;
		}

		[MethodImpl( INLINE )] public Vector2 GetStartPoint() => p0;
		[MethodImpl( INLINE )] public Vector2 GetEndPoint() => p1;

		[MethodImpl( INLINE )] public Vector2 GetPoint( float t ) {
			ReadyCoefficients();
			float t2 = t * t;
			float t3 = t2 * t;
			return new Vector2( t3 * c3.x + t2 * c2.x + t * m0.x + p0.x, t3 * c3.y + t2 * c2.y + t * m0.y + p0.y );
		}

		[MethodImpl( INLINE )] public Vector2 GetDerivative( float t ) {
			ReadyCoefficients();
			float t2 = t * t;
			return new Vector2( 3 * t2 * c3.x + 2 * t * c2.x + m0.x, 3 * t2 * c3.y + 2 * t * c2.y + m0.y );
		}

		[MethodImpl( INLINE )] public Vector2 GetSecondDerivative( float t ) {
			ReadyCoefficients();
			return new Vector2( 6 * t * c3.x + 2 * c2.x, 6 * t * c3.y + 2 * c2.y );
		}

		[MethodImpl( INLINE )] public Vector2 GetThirdDerivative( float t = 0 ) {
			ReadyCoefficients();
			return new Vector2( 6 * c3.x, 6 * c3.y );
		}

		#endregion

		public BezierCubic2D ToBezier() => new BezierCubic2D( p0, p0 + m0 / 3, p1 - m1 / 3, p1 );

	}

}