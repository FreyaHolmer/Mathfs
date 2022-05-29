// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)
// a lot of stuff here made possible by this excellent writeup on bezier curves: https://pomax.github.io/bezierinfo/

using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Freya {

	/// <summary>An optimized 3D quadratic bezier curve, with 3 control points</summary>
	[Serializable] public struct BezierQuad3D : IParamCubicSplineSegment3D {

		const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

		/// <inheritdoc cref="BezierQuad2D(Vector2,Vector2,Vector2)"/>
		public BezierQuad3D( Vector3 p0, Vector3 p1, Vector3 p2 ) {
			( this.p0, this.p1, this.p2 ) = ( p0, p1, p2 );
			validCoefficients = false;
			curve = default;
		}

		Polynomial3D curve;
		public Polynomial3D Curve {
			get {
				ReadyCoefficients();
				return curve;
			}
		}

		#region Control Points

		[SerializeField] Vector3 p0, p1, p2; // the points of the curve

		/// <inheritdoc cref="BezierQuad2D.P0"/>
		public Vector3 P0 {
			[MethodImpl( INLINE )] get => p0;
			[MethodImpl( INLINE )] set => _ = ( p0 = value, validCoefficients = false );
		}

		/// <inheritdoc cref="BezierQuad2D.P1"/>
		public Vector3 P1 {
			[MethodImpl( INLINE )] get => p1;
			[MethodImpl( INLINE )] set => _ = ( p1 = value, validCoefficients = false );
		}

		/// <inheritdoc cref="BezierQuad2D.P2"/>
		public Vector3 P2 {
			[MethodImpl( INLINE )] get => p2;
			[MethodImpl( INLINE )] set => _ = ( p2 = value, validCoefficients = false );
		}

		/// <inheritdoc cref="BezierQuad2D.this[int]"/>
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

		// Coefficient Calculation
		[MethodImpl( INLINE )] void ReadyCoefficients() {
			if( validCoefficients )
				return; // no need to update
			validCoefficients = true;
			curve = CharMatrix.quadraticBezier.GetCurve( p0, p1, p2 );
		}

		#endregion

		/// <inheritdoc cref="BezierCubic2D.Split(float)"/>
		public BezierQuad3D Split( float t ) {
			Vector3 mid = Vector3.LerpUnclamped( p0, p1, t );
			Vector3 b = Vector3.LerpUnclamped( p1, p2, t );
			Vector3 end = Vector3.LerpUnclamped( mid, b, t );
			return new BezierQuad3D( p0, mid, end );
		}

	}

}