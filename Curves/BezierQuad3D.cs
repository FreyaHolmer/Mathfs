// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)
// a lot of stuff here made possible by this excellent writeup on bezier curves: https://pomax.github.io/bezierinfo/

using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Freya {

	/// <summary>An optimized 3D quadratic bezier curve, with 3 control points</summary>
	[Serializable] public struct BezierQuad3D : IParamCurve2Diff<Vector3> {

		const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

		/// <summary>Creates a quadratic bezier curve, from 3 control points</summary>
		/// <param name="p0">The starting point of the curve</param>
		/// <param name="p1">The second control point of the curve, sometimes called the start tangent point</param>
		/// <param name="p2">The end point of the curve, sometimes called the end tangent point</param>
		public BezierQuad3D( Vector3 p0, Vector3 p1, Vector3 p2 ) {
			( this.p0, this.p1, this.p2 ) = ( p0, p1, p2 );
			validCoefficients = false;
			c2 = c1 = default;
		}

		#region Control Points

		[SerializeField] Vector3 p0, p1, p2; // the points of the curve

		/// <summary>The starting point of the curve</summary>
		public Vector3 P0 {
			[MethodImpl( INLINE )] get => p0;
			[MethodImpl( INLINE )] set => _ = ( p0 = value, validCoefficients = false );
		}

		/// <summary>The middle control point of the curve</summary>
		public Vector3 P1 {
			[MethodImpl( INLINE )] get => p1;
			[MethodImpl( INLINE )] set => _ = ( p1 = value, validCoefficients = false );
		}

		/// <summary>The end point of the curve</summary>
		public Vector3 P2 {
			[MethodImpl( INLINE )] get => p2;
			[MethodImpl( INLINE )] set => _ = ( p2 = value, validCoefficients = false );
		}

		/// <summary>Get or set a control point position by index. Valid indices: 0, 1, 2 or 3</summary>
		public Vector3 this[ int i ] {
			get {
				switch( i ) {
					case 0:  return P0;
					case 1:  return P1;
					case 2:  return P2;
					default: throw new ArgumentOutOfRangeException( nameof(i), $"Index has to be in the 0 to 2 range, and I think {i} is outside that range you know" );
				}
			}
			set {
				switch( i ) {
					case 0:
						P0 = value;
						break;
					case 1:
						P1 = value;
						break;
					case 2:
						P2 = value;
						break;
					default: throw new ArgumentOutOfRangeException( nameof(i), $"Index has to be in the 0 to 2 range, and I think {i} is outside that range you know" );
				}
			}
		}

		#endregion

		#region Coefficients

		[NonSerialized] bool validCoefficients; // inverted isDirty flag (can't default to true in structs)
		[NonSerialized] Vector3 c2, c1; // cached coefficients for fast evaluation. c0 = p0

		// Coefficient Calculation
		[MethodImpl( INLINE )] void ReadyCoefficients() {
			if( validCoefficients )
				return; // no need to update
			validCoefficients = true;
			c2.x = p0.x - 2 * p1.x + p2.x;
			c2.y = p0.y - 2 * p1.y + p2.y;
			c2.z = p0.z - 2 * p1.z + p2.z;
			c1.x = 2 * ( p1.x - p0.x );
			c1.y = 2 * ( p1.y - p0.y );
			c1.z = 2 * ( p1.z - p0.z );
		}

		/// <summary>The constant coefficient when evaluating this curve in the form C2*t² + C1*t + C0</summary>
		public Vector3 C0 {
			[MethodImpl( INLINE )] get => p0;
		}

		/// <summary>The linear coefficient when evaluating this curve in the form C2*t² + C1*t + C0</summary>
		public Vector3 C1 {
			[MethodImpl( INLINE )] get {
				ReadyCoefficients();
				return c1;
			}
		}

		/// <summary>The quadratic coefficient when evaluating this curve in the form C2*t² + C1*t + C0</summary>
		public Vector3 C2 {
			[MethodImpl( INLINE )] get {
				ReadyCoefficients();
				return c2;
			}
		}

		/// <summary>The polynomial coefficients in the form c2*t² + c1*t + c0</summary>
		[MethodImpl( INLINE )] public (Vector3 c2, Vector3 c1, Vector3 c0) GetCoefficients() {
			ReadyCoefficients();
			return ( c2, c1, p0 );
		}

		#endregion

		// todo: Object Comparison & ToString

		// todo: Type Casting

		#region Core IParamCurve Implementations

		public int Degree {
			[MethodImpl( INLINE )] get => 2;
		}
		public int Count {
			[MethodImpl( INLINE )] get => 3;
		}

		[MethodImpl( INLINE )] public Vector3 GetStartPoint() => p0;
		[MethodImpl( INLINE )] public Vector3 GetEndPoint() => p2;

		public Vector3 GetPoint( float t ) {
			ReadyCoefficients();
			float tt = t * t;
			return new Vector3( c2.x * tt + c1.x * t + p0.x, c2.y * tt + c1.y * t + p0.y, c2.z * tt + c1.z * t + p0.z );
		}

		public Vector3 GetDerivative( float t ) {
			ReadyCoefficients();
			float tx2 = 2 * t;
			return new Vector3( tx2 * c2.x + c1.x, tx2 * c2.y + c1.y, tx2 * c2.z + c1.z );
		}

		public Vector3 GetSecondDerivative( float t = 0 ) {
			ReadyCoefficients();
			return new Vector3( 2 * c2.x, 2 * c2.y, 2 * c2.z );
		}

		#endregion

		// todo: Point Components

		/// <inheritdoc cref="BezierCubic2D.Split(float)"/>
		public BezierQuad3D Split( float t ) {
			Vector3 mid = Vector3.LerpUnclamped( p0, p1, t );
			Vector3 b = Vector3.LerpUnclamped( p1, p2, t );
			Vector3 end = Vector3.LerpUnclamped( mid, b, t );
			return new BezierQuad3D( p0, mid, end );
		}

		// todo: Bounds

		// todo: exact length

		// todo: Project Point

		// todo: Intersection Tests

		// todo: Polynomial Factors

		// todo: Local Extrema


	}

}