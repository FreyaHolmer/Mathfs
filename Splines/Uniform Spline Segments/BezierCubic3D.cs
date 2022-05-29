// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)
// a lot of stuff here made possible by this excellent writeup on bezier curves: https://pomax.github.io/bezierinfo/

using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Freya {

	/// <summary>An optimized 3D cubic bezier curve, with 4 control points</summary>
	[Serializable] public struct BezierCubic3D : IParamCubicSplineSegment3D {

		const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

		/// <inheritdoc cref="BezierCubic2D(Vector2,Vector2,Vector2,Vector2)"/>
		public BezierCubic3D( Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3 ) {
			( this.p0, this.p1, this.p2, this.p3 ) = ( p0, p1, p2, p3 );
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

		[SerializeField] Vector3 p0, p1, p2, p3; // the points of the curve

		/// <inheritdoc cref="BezierCubic2D.P0"/>
		public Vector3 P0 {
			[MethodImpl( INLINE )] get => p0;
			[MethodImpl( INLINE )] set => _ = ( p0 = value, validCoefficients = false );
		}

		/// <inheritdoc cref="BezierCubic2D.P1"/>
		public Vector3 P1 {
			[MethodImpl( INLINE )] get => p1;
			[MethodImpl( INLINE )] set => _ = ( p1 = value, validCoefficients = false );
		}

		/// <inheritdoc cref="BezierCubic2D.P2"/>
		public Vector3 P2 {
			[MethodImpl( INLINE )] get => p2;
			[MethodImpl( INLINE )] set => _ = ( p2 = value, validCoefficients = false );
		}

		/// <inheritdoc cref="BezierCubic2D.P3"/>
		public Vector3 P3 {
			[MethodImpl( INLINE )] get => p3;
			[MethodImpl( INLINE )] set => _ = ( p3 = value, validCoefficients = false );
		}

		/// <inheritdoc cref="BezierCubic2D.this"/> 
		public Vector3 this[ int i ] {
			get {
				switch( i ) {
					case 0:  return P0;
					case 1:  return P1;
					case 2:  return P2;
					case 3:  return P3;
					default: throw new IndexOutOfRangeException();
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
					default: throw new IndexOutOfRangeException();
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

		public static bool operator ==( BezierCubic3D a, BezierCubic3D b ) => a.P0 == b.P0 && a.P1 == b.P1 && a.P2 == b.P2 && a.P3 == b.P3;
		public static bool operator !=( BezierCubic3D a, BezierCubic3D b ) => !( a == b );
		public bool Equals( BezierCubic3D other ) => P0.Equals( other.P0 ) && P1.Equals( other.P1 ) && P2.Equals( other.P2 ) && P3.Equals( other.P3 );
		public override bool Equals( object obj ) => obj is BezierCubic3D other && Equals( other );

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

		/// <summary>Returns this bezier curve flattened to the Z plane, effectively setting z to 0</summary>
		/// <param name="bezierCubic3D">The 3D curve to cast and flatten on the Z plane</param>
		public static explicit operator BezierCubic2D( BezierCubic3D bezierCubic3D ) {
			return new BezierCubic2D( bezierCubic3D.P0, bezierCubic3D.P1, bezierCubic3D.P2, bezierCubic3D.P3 );
		}

		#endregion

		#region Interpolation

		/// <inheritdoc cref="BezierCubic2D.Lerp(BezierCubic2D,BezierCubic2D,float)"/>
		public static BezierCubic3D Lerp( BezierCubic3D a, BezierCubic3D b, float t ) {
			return new BezierCubic3D(
				Vector3.LerpUnclamped( a.p0, b.p0, t ),
				Vector3.LerpUnclamped( a.p1, b.p1, t ),
				Vector3.LerpUnclamped( a.p2, b.p2, t ),
				Vector3.LerpUnclamped( a.p3, b.p3, t )
			);
		}

		/// <inheritdoc cref="BezierCubic2D.Slerp(BezierCubic2D,BezierCubic2D,float)"/>
		public static BezierCubic3D Slerp( BezierCubic3D a, BezierCubic3D b, float t ) {
			Vector3 p0 = Vector3.LerpUnclamped( a.p0, b.p0, t );
			Vector3 p3 = Vector3.LerpUnclamped( a.p3, b.p3, t );
			return new BezierCubic3D(
				p0,
				p0 + Vector3.SlerpUnclamped( a.p1 - a.p0, b.p1 - b.p0, t ),
				p3 + Vector3.SlerpUnclamped( a.p2 - a.p3, b.p2 - b.p3, t ),
				p3
			);
		}

		#endregion

		#region Splitting

		/// <inheritdoc cref="BezierCubic2D.Split(float)"/>
		public (BezierCubic3D pre, BezierCubic3D post) Split( float t ) {
			Vector3 a = new Vector3(
				P0.x + ( P1.x - P0.x ) * t,
				P0.y + ( P1.y - P0.y ) * t,
				P0.z + ( P1.z - P0.z ) * t );
			float bx = P1.x + ( P2.x - P1.x ) * t;
			float by = P1.y + ( P2.y - P1.y ) * t;
			float bz = P1.z + ( P2.z - P1.z ) * t;
			Vector3 c = new Vector3(
				P2.x + ( P3.x - P2.x ) * t,
				P2.y + ( P3.y - P2.y ) * t,
				P2.z + ( P3.z - P2.z ) * t );
			Vector3 d = new Vector3(
				a.x + ( bx - a.x ) * t,
				a.y + ( by - a.y ) * t,
				a.z + ( bz - a.z ) * t );
			Vector3 e = new Vector3(
				bx + ( c.x - bx ) * t,
				by + ( c.y - by ) * t,
				bz + ( c.z - bz ) * t );
			Vector3 p = new Vector3(
				d.x + ( e.x - d.x ) * t,
				d.y + ( e.y - d.y ) * t,
				d.z + ( e.z - d.z ) * t );
			return ( new BezierCubic3D( P0, a, d, p ), new BezierCubic3D( p, e, c, P3 ) );
		}

		#endregion

	}

}