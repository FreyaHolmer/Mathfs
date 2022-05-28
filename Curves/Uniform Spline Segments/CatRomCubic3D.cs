// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Freya {

	/// <summary>An optimized uniform cubic catmull-rom 3D curve, with 4 control points</summary>
	[Serializable] public struct CatRomCubic3D : IParamCubicSplineSegment3D {

		const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

		/// <inheritdoc cref="CatRomCubic2D(Vector2,Vector2,Vector2,Vector2)"/>
		public CatRomCubic3D( Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3 ) {
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

		/// <inheritdoc cref="CatRomCubic2D.P0"/>
		public Vector3 P0 {
			[MethodImpl( INLINE )] get => p0;
			[MethodImpl( INLINE )] set => _ = ( p0 = value, validCoefficients = false );
		}

		/// <inheritdoc cref="CatRomCubic2D.P1"/>
		public Vector3 P1 {
			[MethodImpl( INLINE )] get => p1;
			[MethodImpl( INLINE )] set => _ = ( p1 = value, validCoefficients = false );
		}

		/// <inheritdoc cref="CatRomCubic2D.P2"/>
		public Vector3 P2 {
			[MethodImpl( INLINE )] get => p2;
			[MethodImpl( INLINE )] set => _ = ( p2 = value, validCoefficients = false );
		}

		/// <inheritdoc cref="CatRomCubic2D.P3"/>
		public Vector3 P3 {
			[MethodImpl( INLINE )] get => p3;
			[MethodImpl( INLINE )] set => _ = ( p3 = value, validCoefficients = false );
		}

		/// <inheritdoc cref="CatRomCubic2D.this[int]"/>
		public Vector3 this[ int i ] {
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
			curve = CharMatrix.cubicCatmullRom.GetCurve( p0, p1, p2, p3 );
		}

		#endregion

		#region Object Comparison & ToString

		public static bool operator ==( CatRomCubic3D a, CatRomCubic3D b ) => a.P0 == b.P0 && a.P1 == b.P1 && a.P2 == b.P2 && a.P3 == b.P3;
		public static bool operator !=( CatRomCubic3D a, CatRomCubic3D b ) => !( a == b );
		public bool Equals( CatRomCubic3D other ) => P0.Equals( other.P0 ) && P1.Equals( other.P1 ) && P2.Equals( other.P2 ) && P3.Equals( other.P3 );
		public override bool Equals( object obj ) => obj is CatRomCubic3D other && Equals( other );

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

		#region Interpolation

		/// <inheritdoc cref="CatRomCubic2D.Lerp(CatRomCubic2D,CatRomCubic2D,float)"/>
		public static CatRomCubic3D Lerp( CatRomCubic3D a, CatRomCubic3D b, float t ) {
			return new CatRomCubic3D(
				Vector3.LerpUnclamped( a.p0, b.p0, t ),
				Vector3.LerpUnclamped( a.p1, b.p1, t ),
				Vector3.LerpUnclamped( a.p2, b.p2, t ),
				Vector3.LerpUnclamped( a.p3, b.p3, t )
			);
		}

		#endregion

		/// <inheritdoc cref="CatRomCubic2D.ToBezier()"/>
		public BezierCubic3D ToBezier() =>
			new BezierCubic3D(
				p1,
				p1 + ( p2 - p0 ) / 6f,
				p2 + ( p1 - p3 ) / 6f,
				p2
			);

		/// <inheritdoc cref="CatRomCubic2D.ToHermite()"/>
		public HermiteCubic3D ToHermite() =>
			new HermiteCubic3D(
				p1,
				( p2 - p0 ) / 2f,
				p2,
				( p3 - p1 ) / 2f
			);

		/// <inheritdoc cref="CatRomCubic2D.ToBSpline()"/>
		public UBSCubic3D ToBSpline() =>
			new UBSCubic3D(
				( 7 * p0 - 4 * p1 + 5 * p2 - 2 * p3 ) / 6,
				( -2 * p0 + 11 * p1 - 4 * p2 + p3 ) / 6,
				( p0 - 4 * p1 + 11 * p2 - 2 * p3 ) / 6,
				( -2 * p0 + 5 * p1 - 4 * p2 + 7 * p3 ) / 6
			);

	}

}