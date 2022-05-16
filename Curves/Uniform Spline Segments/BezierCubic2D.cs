// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)
// a lot of stuff here made possible by this excellent writeup on bezier curves: https://pomax.github.io/bezierinfo/

using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Freya {

	/// <summary>An optimized 2D cubic bezier curve, with 4 control points</summary>
	[Serializable] public struct BezierCubic2D : IParamCubicSplineSegment2D {

		const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

		/// <summary>Creates a cubic bezier curve, from 4 control points</summary>
		/// <param name="p0">The starting point of the curve</param>
		/// <param name="p1">The second control point of the curve, sometimes called the start tangent point</param>
		/// <param name="p2">The third control point of the curve, sometimes called the end tangent point</param>
		/// <param name="p3">The end point of the curve</param>
		public BezierCubic2D( Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3 ) {
			( this.p0, this.p1, this.p2, this.p3 ) = ( p0, p1, p2, p3 );
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

		[SerializeField] Vector2 p0, p1, p2, p3; // the points of the curve

		/// <summary>The starting point of the curve</summary>
		public Vector2 P0 {
			[MethodImpl( INLINE )] get => p0;
			[MethodImpl( INLINE )] set => _ = ( p0 = value, validCoefficients = false );
		}

		/// <summary>The second control point of the curve, sometimes called the start tangent point</summary>
		public Vector2 P1 {
			[MethodImpl( INLINE )] get => p1;
			[MethodImpl( INLINE )] set => _ = ( p1 = value, validCoefficients = false );
		}

		/// <summary>The third control point of the curve, sometimes called the end tangent point</summary>
		public Vector2 P2 {
			[MethodImpl( INLINE )] get => p2;
			[MethodImpl( INLINE )] set => _ = ( p2 = value, validCoefficients = false );
		}

		/// <summary>The end point of the curve</summary>
		public Vector2 P3 {
			[MethodImpl( INLINE )] get => p3;
			[MethodImpl( INLINE )] set => _ = ( p3 = value, validCoefficients = false );
		}

		/// <summary>Get or set a control point position by index. Valid indices: 0, 1, 2 or 3</summary>
		public Vector2 this[ int i ] {
			get {
				switch( i ) {
					case 0:  return P0;
					case 1:  return P1;
					case 2:  return P2;
					case 3:  return P3;
					default: throw new ArgumentOutOfRangeException( nameof(i), $"Index has to be in the 0 to 3 range, and I think {i} is outside that range you know" );
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
					case 3:
						P3 = value;
						break;
					default: throw new ArgumentOutOfRangeException( nameof(i), $"Index has to be in the 0 to 3 range, and I think {i} is outside that range you know" );
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
			curve = CharMatrix.cubicBezier.GetCurve( p0, p1, p2, p3 );
		}

		#endregion

		#region Object Comparison & ToString

		public static bool operator ==( BezierCubic2D a, BezierCubic2D b ) => a.P0 == b.P0 && a.P1 == b.P1 && a.P2 == b.P2 && a.P3 == b.P3;
		public static bool operator !=( BezierCubic2D a, BezierCubic2D b ) => !( a == b );
		public bool Equals( BezierCubic2D other ) => P0.Equals( other.P0 ) && P1.Equals( other.P1 ) && P2.Equals( other.P2 ) && P3.Equals( other.P3 );
		public override bool Equals( object obj ) => obj is BezierCubic2D other && Equals( other );

		public override int GetHashCode() {
			unchecked {
				int hashCode = P0.GetHashCode();
				hashCode = ( hashCode * 397 ) ^ P1.GetHashCode();
				hashCode = ( hashCode * 397 ) ^ P2.GetHashCode();
				hashCode = ( hashCode * 397 ) ^ P3.GetHashCode();
				return hashCode;
			}
		}

		public override string ToString() => $"{P0}, {P1}, {P2}, {P3}";

		#endregion

		#region Type Casting

		/// <summary>Returns this bezier curve in 3D, where z = 0</summary>
		/// <param name="bezierCubic2D">The 2D curve to cast</param>
		public static explicit operator BezierCubic3D( BezierCubic2D bezierCubic2D ) {
			return new BezierCubic3D( bezierCubic2D.P0, bezierCubic2D.P1, bezierCubic2D.P2, bezierCubic2D.P3 );
		}

		#endregion

		#region Interpolation

		/// <summary>Returns linear blend between two bézier curves</summary>
		/// <param name="a">The first curve</param>
		/// <param name="b">The second curve</param>
		/// <param name="t">A value from 0 to 1 to blend between <c>a</c> and <c>b</c></param>
		public static BezierCubic2D Lerp( BezierCubic2D a, BezierCubic2D b, float t ) {
			return new BezierCubic2D(
				Vector2.LerpUnclamped( a.p0, b.p0, t ),
				Vector2.LerpUnclamped( a.p1, b.p1, t ),
				Vector2.LerpUnclamped( a.p2, b.p2, t ),
				Vector2.LerpUnclamped( a.p3, b.p3, t )
			);
		}

		/// <summary>Returns blend between two bézier curves,
		/// where the endpoints are linearly interpolated,
		/// while the tangents are spherically interpolated relative to their corresponding endpoint</summary>
		/// <param name="a">The first curve</param>
		/// <param name="b">The second curve</param>
		/// <param name="t">A value from 0 to 1 to blend between <c>a</c> and <c>b</c></param>
		public static BezierCubic2D Slerp( BezierCubic2D a, BezierCubic2D b, float t ) {
			Vector2 p0 = Vector2.LerpUnclamped( a.p0, b.p0, t );
			Vector2 p3 = Vector2.LerpUnclamped( a.p3, b.p3, t );
			return new BezierCubic2D(
				p0,
				p0 + (Vector2)Vector3.SlerpUnclamped( a.p1 - a.p0, b.p1 - b.p0, t ),
				p3 + (Vector2)Vector3.SlerpUnclamped( a.p2 - a.p3, b.p2 - b.p3, t ),
				p3
			);
		}

		#endregion

		#region Splitting

		/// <summary>Splits this curve at the given t-value, into two curves of the exact same shape</summary>
		/// <param name="t">The t-value along the curve to sample</param>
		public (BezierCubic2D pre, BezierCubic2D post) Split( float t ) {
			Vector2 a = new Vector2(
				P0.x + ( P1.x - P0.x ) * t,
				P0.y + ( P1.y - P0.y ) * t );
			float bx = P1.x + ( P2.x - P1.x ) * t;
			float by = P1.y + ( P2.y - P1.y ) * t;
			Vector2 c = new Vector2(
				P2.x + ( P3.x - P2.x ) * t,
				P2.y + ( P3.y - P2.y ) * t );
			Vector2 d = new Vector2(
				a.x + ( bx - a.x ) * t,
				a.y + ( by - a.y ) * t );
			Vector2 e = new Vector2(
				bx + ( c.x - bx ) * t,
				by + ( c.y - by ) * t );
			Vector2 p = new Vector2(
				d.x + ( e.x - d.x ) * t,
				d.y + ( e.y - d.y ) * t );
			return ( new BezierCubic2D( P0, a, d, p ), new BezierCubic2D( p, e, c, P3 ) );
		}

		#endregion


		#region Conversion

		public UBSCubic2D ToUniformCubicBSpline() {
			// todo: channel split for performance
			return new UBSCubic2D(
				6 * p0 - 7 * p1 + 2 * p2,
				2 * p1 - p2,
				-p1 + 2 * p2,
				2 * p1 - 7 * p2 + 6 * p3 );
		}

		public CatRom2D ToUniformCubicCatRom() {
			// todo: channel split for performance
			return new CatRom2D(
				6 * p0 - 6 * p1 + p3,
				p0,
				p3,
				p0 - 6 * p2 + 6 * p3 );
		}

		public HermiteCubic2D ToHermite() {
			// todo: channel split for performance
			return new HermiteCubic2D( p0, ( p1 - p0 ) * 3, p3, ( p3 - p2 ) * 3 );
		}

		#endregion

	}

}