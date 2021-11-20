// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)
// a lot of stuff here made possible by this excellent writeup on bezier curves: https://pomax.github.io/bezierinfo/

using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Freya {

	/// <summary>A 2D cubic catmull-rom curve</summary>
	[Serializable] public struct CatRom2D {

		const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

		// serialized data
		[SerializeField] Vector2 p0, p1, p2, p3;
		[SerializeField] float k0, k1, k2, k3; // knots
		[SerializeField] [Range( 0, 1 )] float alpha;
		[SerializeField] [Range( 0, 1 )] float tension;
		[SerializeField] bool manualKnots;

		// cached data to accelerate calculations
		[NonSerialized] bool validCoefficients; // inverted isDirty flag (can't default to true in structs)
		[NonSerialized] Vector2 c3, c2, c1, c0; // cached coefficients for fast evaluation

		#region Properties

		/// <summary>The first control point of the catrom curve. Note that this point is not included in the curve itself, and only helps to shape it</summary>
		public Vector2 P0 {
			[MethodImpl( INLINE )] get => p0;
			set => _ = ( p0 = value, validCoefficients = false );
		}
		/// <summary>The second control point, and the start of the catrom curve</summary>
		public Vector2 P1 {
			[MethodImpl( INLINE )] get => p1;
			set => _ = ( p1 = value, validCoefficients = false );
		}
		/// <summary>The third control point, and the end of the catrom curve</summary>
		public Vector2 P2 {
			[MethodImpl( INLINE )] get => p2;
			set => _ = ( p2 = value, validCoefficients = false );
		}
		/// <summary>The last control point of the catrom curve. Note that this point is not included in the curve itself, and only helps to shape it</summary>
		public Vector2 P3 {
			[MethodImpl( INLINE )] get => p3;
			set => _ = ( p3 = value, validCoefficients = false );
		}

		/// <summary>The alpha parameter, which controls how much the length of each segment should influence the shape of the curve.
		/// A value of 0 is called a uniform catrom, and is fast to evaluate but has a tendency to overshoot.
		/// A value of 0.5 is a centripetal catrom, which follows points very tightly, and prevents cusps and loops.
		/// A value of 1 is a chordal catrom, which follows the points very smoothly with wide arcs</summary>
		public float Alpha {
			[MethodImpl( INLINE )] get => alpha;
			set => _ = ( alpha = value, validCoefficients = false );
		}

		/// <summary>Controls tension of the curve, where a value of 0 is a standard smooth catrom curve, while a value of 1 flattens the curve to a straight line segment</summary>
		public float Tension {
			[MethodImpl( INLINE )] get => tension;
			set => _ = ( tension = value, validCoefficients = false );
		}

		#endregion

		#region Constructors

		/// <summary>Creates a cubic catmull-rom curve, from 4 control points with explicit alpha parameter to define its type</summary>
		/// <param name="p0">The first control point of the catrom curve. Note that this point is not included in the curve itself, and only helps to shape it</param>
		/// <param name="p1">The second control point, and the start of the catrom curve</param>
		/// <param name="p2">The third control point, and the end of the catrom curve</param>
		/// <param name="p3">The last control point of the catrom curve. Note that this point is not included in the curve itself, and only helps to shape it</param>
		/// <param name="alpha">The alpha parameter controls how much the length of each segment should influence the shape of the curve.
		/// A value of 0 is called a uniform catrom, and is fast to evaluate but has a tendency to overshoot.
		/// A value of 0.5 is a centripetal catrom, which follows points very tightly, and prevents cusps and loops.
		/// A value of 1 is a chordal catrom, which follows the points very smoothly with wide arcs</param>
		/// <param name="tension">Controls tension of the curve, where a value of 0 is a standard smooth catrom curve, while a value of 1 flattens the curve to a straight line segment</param>
		public CatRom2D( Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float alpha = 0.5f, float tension = 0 ) {
			( this.p0, this.p1, this.p2, this.p3 ) = ( p0, p1, p2, p3 );
			this.alpha = alpha;
			this.tension = tension;
			this.manualKnots = false;
			validCoefficients = false;
			c0 = c1 = c2 = c3 = default;
			k0 = k1 = k2 = k3 = default;
		}

		/// <summary>Creates a cubic catmull-rom curve, from 4 control points with explicit alpha parameter to define its type</summary>
		/// <param name="p0">The first control point of the catrom curve. Note that this point is not included in the curve itself, and only helps to shape it</param>
		/// <param name="p1">The second control point, and the start of the catrom curve</param>
		/// <param name="p2">The third control point, and the end of the catrom curve</param>
		/// <param name="p3">The last control point of the catrom curve. Note that this point is not included in the curve itself, and only helps to shape it</param>
		/// <param name="type">The type of catrom curve to use. This will internally determine the value of the <c>alpha</c> parameter</param>
		/// <param name="tension">Controls tension of the curve, where a value of 0 is a standard smooth catrom curve, while a value of 1 flattens the curve to a straight line segment</param>
		public CatRom2D( Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, CatRomType type, float tension = 0 ) {
			( this.p0, this.p1, this.p2, this.p3 ) = ( p0, p1, p2, p3 );
			this.alpha = type.AlphaValue();
			this.tension = tension;
			this.manualKnots = false;
			validCoefficients = false;
			c0 = c1 = c2 = c3 = default;
			k0 = k1 = k2 = k3 = default;
		}

		/// <summary>Creates a catmull-rom curve with 4 control points and manually assigned knot values</summary>
		/// <param name="p0">The first control point of the catrom curve. Note that this point is not included in the curve itself, and only helps to shape it</param>
		/// <param name="p1">The second control point, and the start of the catrom curve</param>
		/// <param name="p2">The third control point, and the end of the catrom curve</param>
		/// <param name="p3">The last control point of the catrom curve. Note that this point is not included in the curve itself, and only helps to shape it</param>
		/// <param name="tension">Controls tension of the curve, where a value of 0 is a standard smooth catrom curve, while a value of 1 flattens the curve to a straight line segment</param>
		/// <param name="k0">The first knot value</param>
		/// <param name="k1">The second knot value</param>
		/// <param name="k2">The third knot value</param>
		/// <param name="k3">The fourth knot value</param>
		public CatRom2D( Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float k0, float k1, float k2, float k3, float tension = 0 ) {
			( this.p0, this.p1, this.p2, this.p3 ) = ( p0, p1, p2, p3 );
			( this.k0, this.k1, this.k2, this.k3 ) = ( k0, k1, k2, k3 );
			this.alpha = 0; // unused when using manual vectors
			this.tension = tension;
			this.manualKnots = true;
			validCoefficients = false;
			c0 = c1 = c2 = c3 = default;
		}

		#endregion

		#region Internal Functions

		/// <summary>Returns the internal knot vector of this curve</summary>
		public (float, float, float, float) GetKnots() {
			if( manualKnots )
				return ( k0, k1, k2, k3 );
			if( alpha == 0 ) // uniform catrom
				return ( 0, 1, 2, 3 );
			const float ak0 = 0;
			float ak1 = Vector2.SqrMagnitude( p0 - p1 ).Pow( 0.5f * alpha ) + ak0;
			float ak2 = Vector2.SqrMagnitude( p1 - p2 ).Pow( 0.5f * alpha ) + ak1;
			float ak3 = Vector2.SqrMagnitude( p2 - p3 ).Pow( 0.5f * alpha ) + ak2;
			return ( ak0, ak1, ak2, ak3 );
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
				( float kn0, float kn1, float kn2, float kn3 ) = GetKnots();
				Vector2 m1 = ( 1 - tension ) * ( kn2 - kn1 ) * ( ( p1 - p0 ) / ( kn1 - kn0 ) - ( p2 - p0 ) / ( kn2 - kn0 ) + ( p2 - p1 ) / ( kn2 - kn1 ) );
				Vector2 m2 = ( 1 - tension ) * ( kn2 - kn1 ) * ( ( p2 - p1 ) / ( kn2 - kn1 ) - ( p3 - p1 ) / ( kn3 - kn1 ) + ( p3 - p2 ) / ( kn3 - kn2 ) );
				Vector2 p2p1 = p1 - p2;
				c3 = 2 * p2p1 + m1 + m2;
				c2 = -3 * p2p1 - 2 * m1 - m2;
				c1 = m1;
				c0 = p1;
			}
		}

		#endregion

		#region Points & Derivatives

		/// <inheritdoc cref="BezierCubic2D.GetPoint(float)"/>
		[MethodImpl( INLINE )] public Vector2 GetPoint( float t ) {
			ReadyCoefficients();
			return c3 * t * t * t + c2 * t * t + c1 * t + c0;
		}

		/// <inheritdoc cref="BezierCubic2D.GetDerivative(float)"/>
		[MethodImpl( INLINE )] public Vector2 GetDerivative( float t ) {
			ReadyCoefficients();
			return 3 * c3 * t * t + 2 * c2 * t + c1;
		}

		/// <inheritdoc cref="BezierCubic2D.GetSecondDerivative(float)"/>
		[MethodImpl( INLINE )] public Vector2 GetSecondDerivative( float t ) {
			ReadyCoefficients();
			return 6 * c3 * t + 2 * c2;
		}

		/// <inheritdoc cref="BezierCubic2D.GetThirdDerivative()"/>
		[MethodImpl( INLINE )] public Vector2 GetThirdDerivative() {
			ReadyCoefficients();
			return 6 * c3;
		}

		/* Alternate method to calculate the point - this is slower but it's mathematically kinda pretty isn't it?
		public Vector2 GetPoint( float t, float alpha ) {
			( float k0, float k1, float k2, float k3 ) = GetKnots( alpha );
			float v = Mathfs.Lerp( k1, k2, t ); // remap from 0-1 to k1-k2
			Vector2 A1 = Remap( v, k0, k1, p0, p1 );
			Vector2 A2 = Remap( v, k1, k2, p1, p2 );
			Vector2 A3 = Remap( v, k2, k3, p2, p3 );
			Vector2 B1 = Remap( v, k0, k2, A1, A2 );
			Vector2 B2 = Remap( v, k1, k3, A2, A3 );
			return   C = Remap( v, k1, k2, B1, B2 );
		}
		Vector2 Remap( float value, float t0, float t1, Vector2 a, Vector2 b ) {
			float t = Mathfs.InverseLerp( t0, t1, value );
			return Vector2.LerpUnclamped( a, b, t );
		}*/

		#endregion

		public (Vector2 m1, Vector2 m2) GetPointTangents() => ( GetDerivative( 0 ), GetDerivative( 1 ) );

		public BezierCubic2D ToBezier() {
			( Vector2 m1, Vector2 m2 ) = GetPointTangents();
			return new BezierCubic2D( p1, p1 + m1 / 3, p2 - m2 / 3, p2 );
		}

		public Hermite2D ToHermite() {
			( Vector2 m1, Vector2 m2 ) = GetPointTangents();
			return new Hermite2D( p1, m1, p2, m2 );
		}

	}

}