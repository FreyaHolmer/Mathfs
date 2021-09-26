// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)
// a lot of stuff here made possible by this excellent writeup on bezier curves: https://pomax.github.io/bezierinfo/

using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Freya {

	/// <summary>A 3D cubic catmull-rom curve</summary>
	[Serializable] public struct CatRom3D {

		const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

		// serialized data
		[SerializeField] Vector3 p0, p1, p2, p3;
		[SerializeField] [Range( 0, 1 )] float alpha;
		[SerializeField] [Range( 0, 1 )] float tension;

		// cached data to accelerate calculations
		[NonSerialized] bool validCoefficients; // inverted isDirty flag (can't default to true in structs)
		[NonSerialized] Vector3 c3, c2, c1, c0; // cached coefficients for fast evaluation

		#region Properties

		/// <inheritdoc cref="CatRom2D.P0"/>
		public Vector3 P0 {
			[MethodImpl( INLINE )] get => p0;
			set => _ = ( p0 = value, validCoefficients = false );
		}
		/// <inheritdoc cref="CatRom2D.P1"/>
		public Vector3 P1 {
			[MethodImpl( INLINE )] get => p1;
			set => _ = ( p1 = value, validCoefficients = false );
		}
		/// <inheritdoc cref="CatRom2D.P2"/>
		public Vector3 P2 {
			[MethodImpl( INLINE )] get => p2;
			set => _ = ( p2 = value, validCoefficients = false );
		}
		/// <inheritdoc cref="CatRom2D.P3"/>
		public Vector3 P3 {
			[MethodImpl( INLINE )] get => p3;
			set => _ = ( p3 = value, validCoefficients = false );
		}

		/// <inheritdoc cref="CatRom2D.Alpha"/>
		public float Alpha {
			[MethodImpl( INLINE )] get => alpha;
			set => _ = ( alpha = value, validCoefficients = false );
		}

		/// <inheritdoc cref="CatRom2D.Tension"/>
		public float Tension {
			[MethodImpl( INLINE )] get => tension;
			set => _ = ( tension = value, validCoefficients = false );
		}

		#endregion

		#region Constructors

		/// <inheritdoc cref="CatRom2D(Vector2,Vector2,Vector2,Vector2,float,float)"/>
		public CatRom3D( Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float alpha = 0.5f, float tension = 0 ) {
			_ = ( this.p0 = p0, this.p1 = p1, this.p2 = p2, this.p3 = p3 );
			this.alpha = alpha;
			this.tension = tension;
			validCoefficients = false;
			c0 = c1 = c2 = c3 = default;
		}

		/// <inheritdoc cref="CatRom2D(Vector2,Vector2,Vector2,Vector2,CatRomType,float)"/>
		public CatRom3D( Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, CatRomType type, float tension = 0 ) {
			_ = ( this.p0 = p0, this.p1 = p1, this.p2 = p2, this.p3 = p3 );
			this.alpha = type.AlphaValue();
			this.tension = tension;
			validCoefficients = false;
			c0 = c1 = c2 = c3 = default;
		}

		#endregion

		#region Internal Functions

		/// <inheritdoc cref="CatRom2D.GetKnots()"/>
		public (float, float, float, float) GetKnots() {
			if( alpha == 0 ) // uniform catrom
				return ( 0, 1, 2, 3 );
			const float k0 = 0;
			float k1 = Vector3.SqrMagnitude( p0 - p1 ).Pow( 0.5f * alpha ) + k0;
			float k2 = Vector3.SqrMagnitude( p1 - p2 ).Pow( 0.5f * alpha ) + k1;
			float k3 = Vector3.SqrMagnitude( p2 - p3 ).Pow( 0.5f * alpha ) + k2;
			return ( k0, k1, k2, k3 );
		}

		[MethodImpl( INLINE )] void ReadyCoefficients() {
			if( validCoefficients )
				return; // no need to update
			validCoefficients = true;
			if( Mathfs.Approximately( tension, 1f ) ) { // linear segment
				c3 = default;
				c2 = default;
				c1 = p2 - p1;
				c0 = p1;
			} else {
				( float k0, float k1, float k2, float k3 ) = GetKnots();
				Vector3 m1 = ( 1 - tension ) * ( k2 - k1 ) * ( ( p1 - p0 ) / ( k1 - k0 ) - ( p2 - p0 ) / ( k2 - k0 ) + ( p2 - p1 ) / ( k2 - k1 ) );
				Vector3 m2 = ( 1 - tension ) * ( k2 - k1 ) * ( ( p2 - p1 ) / ( k2 - k1 ) - ( p3 - p1 ) / ( k3 - k1 ) + ( p3 - p2 ) / ( k3 - k2 ) );
				Vector3 p2p1 = p1 - p2;
				c3 = 2 * p2p1 + m1 + m2;
				c2 = -3 * p2p1 - 2 * m1 - m2;
				c1 = m1;
				c0 = p1;
			}
		}

		#endregion

		#region Points & Derivatives

		/// <inheritdoc cref="CatRom2D.GetPoint(float)"/>
		[MethodImpl( INLINE )] public Vector3 GetPoint( float t ) {
			ReadyCoefficients();
			return c3 * t * t * t + c2 * t * t + c1 * t + c0;
		}

		/// <inheritdoc cref="CatRom2D.GetDerivative(float)"/>
		[MethodImpl( INLINE )] public Vector3 GetDerivative( float t ) {
			ReadyCoefficients();
			return 3 * c3 * t * t + 2 * c2 * t + c1;
		}

		/// <inheritdoc cref="CatRom2D.GetSecondDerivative(float)"/>
		[MethodImpl( INLINE )] public Vector3 GetSecondDerivative( float t ) {
			ReadyCoefficients();
			return 6 * c3 * t + 2 * c2;
		}

		/// <inheritdoc cref="CatRom2D.GetThirdDerivative()"/>
		[MethodImpl( INLINE )] public Vector3 GetThirdDerivative() {
			ReadyCoefficients();
			return 6 * c3;
		}

		/* Alternate method to calculate the point - this is slower but it's mathematically kinda pretty isn't it?
		public Vector3 GetPoint( float t, float alpha ) {
			( float k0, float k1, float k2, float k3 ) = GetKnots( alpha );
			float v = Mathfs.Lerp( k1, k2, t ); // remap from 0-1 to k1-k2
			Vector3 a = Remap( v, k0, k1, p0, p1 );
			Vector3 b = Remap( v, k1, k2, p1, p2 );
			Vector3 c = Remap( v, k2, k3, p2, p3 );
			Vector3 d = Remap( v, k0, k2, a, b );
			Vector3 e = Remap( v, k1, k3, b, c );
			return Remap( v, k1, k2, d, e );
		}
		Vector3 Remap( float value, float t0, float t1, Vector3 a, Vector3 b ) {
			float t = Mathfs.InverseLerp( t0, t1, value );
			return Vector3.LerpUnclamped( a, b, t );
		}*/

		#endregion

	}

}