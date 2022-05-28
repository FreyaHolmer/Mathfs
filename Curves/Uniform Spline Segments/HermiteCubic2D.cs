// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Serialization;

namespace Freya {

	/// <summary>An optimized 2D cubic Hermite curve segment</summary>
	[Serializable] public struct HermiteCubic2D : IParamCubicSplineSegment2D {

		const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

		/// <summary>Creates a cubic Hermite curve, from two control points and two tangents</summary>
		/// <param name="p0">The starting point of the curve</param>
		/// <param name="v0">The rate of change (velocity) at the start of the curve</param>
		/// <param name="p1">The end point of the curve</param>
		/// <param name="v1">The rate of change (velocity) at the end of the curve</param>
		public HermiteCubic2D( Vector2 p0, Vector2 v0, Vector2 p1, Vector2 v1 ) {
			( this.p0, this.v0, this.p1, this.v1 ) = ( p0, v0, p1, v1 );
			validCoefficients = false;
			curve = default;
		}

		Polynomial2D curve;
		public Polynomial2D Curve {
			get {
				ReadyCoefficients();
				return curve;
			}
		}

		#region Control Points

		[SerializeField] Vector2 p0;
		[FormerlySerializedAs( "m0" )] [SerializeField] Vector2 v0;
		[SerializeField] Vector2 p1;
		[FormerlySerializedAs( "m1" )] [SerializeField] Vector2 v1;

		/// <summary>The starting point of the curve</summary>
		public Vector2 P0 {
			[MethodImpl( INLINE )] get => p0;
			[MethodImpl( INLINE )] set => _ = ( p0 = value, validCoefficients = false );
		}

		/// <summary>The rate of change (velocity) at the start of the curve</summary>
		public Vector2 V0 {
			[MethodImpl( INLINE )] get => v0;
			[MethodImpl( INLINE )] set => _ = ( v0 = value, validCoefficients = false );
		}

		/// <summary>The end point of the curve</summary>
		public Vector2 P1 {
			[MethodImpl( INLINE )] get => p1;
			[MethodImpl( INLINE )] set => _ = ( p1 = value, validCoefficients = false );
		}

		/// <summary>The rate of change (velocity) at the end of the curve</summary>
		public Vector2 V1 {
			[MethodImpl( INLINE )] get => v1;
			[MethodImpl( INLINE )] set => _ = ( v1 = value, validCoefficients = false );
		}

		#endregion

		#region Coefficients

		[NonSerialized] bool validCoefficients; // inverted isDirty flag (can't default to true in structs)

		// Coefficient Calculation
		[MethodImpl( INLINE )] void ReadyCoefficients() {
			if( validCoefficients )
				return; // no need to update
			validCoefficients = true;
			curve = CharMatrix.cubicHermite.GetCurve( p0, v0, p1, v1 );
		}

		#endregion

		public BezierCubic2D ToBezier() => new BezierCubic2D( p0, p0 + v0 / 3, p1 - v1 / 3, p1 );

	}

}