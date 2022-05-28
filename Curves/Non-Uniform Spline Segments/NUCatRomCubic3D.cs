// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Freya {

	/// <summary>A non-uniform cubic catmull-rom 3D curve</summary>
	[Serializable] public struct NUCatRomCubic3D : IParamCubicSplineSegment3D {

		public enum KnotCalcMode {
			Manual,
			Auto,
			AutoUnitInterval
		}

		const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

		#region Constructors

		/// <inheritdoc cref="NUCatRomCubic2D(Vector2,Vector2,Vector2,Vector2,float,float,float,float)"/>
		public NUCatRomCubic3D( Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float k0, float k1, float k2, float k3 ) {
			( this.p0, this.p1, this.p2, this.p3 ) = ( p0, p1, p2, p3 );
			( this.k0, this.k1, this.k2, this.k3 ) = ( k0, k1, k2, k3 );
			validCoefficients = false;
			curve = default;
			knotCalcMode = KnotCalcMode.Manual;
			alpha = default; // unused when using manual knots
		}

		/// <inheritdoc cref="NUCatRomCubic2D(Vector2,Vector2,Vector2,Vector2)"/>
		public NUCatRomCubic3D( Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3 ) : this( p0, p1, p2, p3, -1, 0, 1, 2 ) {
		}

		/// <inheritdoc cref="NUCatRomCubic2D(Vector2,Vector2,Vector2,Vector2,CatRomType,bool)"/>
		public NUCatRomCubic3D( Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, CatRomType type, bool parameterizeToUnitInterval = true )
			: this( p0, p1, p2, p3, type.AlphaValue(), parameterizeToUnitInterval ) {
		}

		/// <inheritdoc cref="NUCatRomCubic2D(Vector2,Vector2,Vector2,Vector2,float,bool)"/>
		public NUCatRomCubic3D( Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float alpha, bool parameterizeToUnitInterval = true ) {
			( this.p0, this.p1, this.p2, this.p3 ) = ( p0, p1, p2, p3 );
			validCoefficients = false;
			curve = default;
			k0 = k1 = k2 = k3 = default;
			knotCalcMode = parameterizeToUnitInterval ? KnotCalcMode.AutoUnitInterval : KnotCalcMode.Auto;
			this.alpha = alpha;
		}

		#endregion

		// serialized data
		[SerializeField] Vector3 p0, p1, p2, p3;
		[SerializeField] float k0, k1, k2, k3; // knot vector

		// knot auto-calculation fields
		[SerializeField] KnotCalcMode knotCalcMode; // knot recalculation mode
		[SerializeField] float alpha; // alpha parameterization

		Polynomial3D curve;
		public Polynomial3D Curve {
			get {
				ReadyCoefficients();
				return curve;
			}
		}

		#region Properties

		/// <inheritdoc cref="NUCatRomCubic2D.P0"/>
		public Vector3 P0 {
			[MethodImpl( INLINE )] get => p0;
			set => _ = ( p0 = value, validCoefficients = false );
		}
		/// <inheritdoc cref="NUCatRomCubic2D.P1"/>
		public Vector3 P1 {
			[MethodImpl( INLINE )] get => p1;
			set => _ = ( p1 = value, validCoefficients = false );
		}
		/// <inheritdoc cref="NUCatRomCubic2D.P2"/>
		public Vector3 P2 {
			[MethodImpl( INLINE )] get => p2;
			set => _ = ( p2 = value, validCoefficients = false );
		}
		/// <inheritdoc cref="NUCatRomCubic2D.P3"/>
		public Vector3 P3 {
			[MethodImpl( INLINE )] get => p3;
			set => _ = ( p3 = value, validCoefficients = false );
		}

		/// <inheritdoc cref="NUCatRomCubic2D.Alpha"/>
		public float Alpha {
			[MethodImpl( INLINE )] get => alpha;
			set => _ = ( alpha = value, validCoefficients = false );
		}

		#endregion

		// cached data to accelerate calculations
		[NonSerialized] bool validCoefficients; // inverted isDirty flag (can't default to true in structs)

		[MethodImpl( INLINE )] void ReadyCoefficients() {
			if( validCoefficients )
				return; // no need to update
			validCoefficients = true;
			if( knotCalcMode != KnotCalcMode.Manual )
				( k0, k1, k2, k3 ) = SplineUtils.CalcCatRomKnots( p0, p1, p2, p3, alpha, knotCalcMode == KnotCalcMode.AutoUnitInterval );
			curve = SplineUtils.CalculateCatRomCurve( p0, p1, p2, p3, k0, k1, k2, k3 );
		}

		/// <inheritdoc cref="NUCatRomCubic2D.GetPointWeightAtKnotValue(int,float)"/>
		public float GetPointWeightAtKnotValue( int i, float u ) {
			float a = Mathfs.InverseLerp( k0, k1, u );
			float b = Mathfs.InverseLerp( k1, k2, u );
			float c = Mathfs.InverseLerp( k2, k3, u );
			float d = Mathfs.InverseLerp( k0, k2, u );
			float g = Mathfs.InverseLerp( k1, k3, u );
			switch( i ) {
				case 0:  return -( a - 1 ) * ( b - 1 ) * ( d - 1 );
				case 1:  return ( b - 1 ) * ( a * d - a + b * ( d + g - 1 ) - d );
				case 2:  return -b * ( b * ( d + g - 1 ) + g * ( c - 1 ) - d );
				case 3:  return b * c * g;
				default: throw new IndexOutOfRangeException( $"Catrom point has to be either 0, 1, 2 or 3. Got: {i}" );
			}
		}

	}

}