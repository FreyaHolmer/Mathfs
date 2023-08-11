using System;
using UnityEngine;

namespace Freya {

	[Serializable]
	public struct Bivector3 {

		public static readonly Bivector3 zero = new Bivector3( 0, 0, 0 );
		public float yz, zx, xy;

		public float this[ int i ] => i switch { 0 => yz, 1 => zx, 2 => xy, _ => throw new IndexOutOfRangeException() };

		public Bivector3( float yz, float zx, float xy ) {
			this.yz = yz;
			this.zx = zx;
			this.xy = xy;
		}

		public Bivector3( Vector3 a, Vector3 b ) {
			Bivector3 bv = Mathfs.Wedge( a, b );
			this.yz = bv.yz;
			this.zx = bv.zx;
			this.xy = bv.xy;
		}

		public float Magnitude => MathF.Sqrt( SqrMagnitude );
		public Bivector3 Normalized => new Bivector3( yz, zx, xy ) / Magnitude;
		public Vector3 Normal => HodgeDual.normalized;
		public Vector3 HodgeDual => new Vector3( yz, zx, xy );
		public float SqrMagnitude => yz * yz + zx * zx + xy * xy;

		/// <inheritdoc cref="Dot(Bivector3,Bivector3)"/>
		public float Dot( Bivector3 b ) => Dot( this, b );

		/// <inheritdoc cref="Wedge(Bivector3,Bivector3)"/>
		public Bivector3 Wedge( Bivector3 b ) => Wedge( this, b );

		/// <summary>The real part when multiplying two bivectors</summary>
		public static float Dot( Bivector3 a, Bivector3 b ) => -a.yz * b.yz - a.zx * b.zx - a.xy * b.xy;

		/// <summary>The bivector part when multiplying two bivectors</summary>
		public static Bivector3 Wedge( Bivector3 a, Bivector3 b ) =>
			new Bivector3(
				yz: a.xy * b.zx - a.zx * b.xy,
				zx: a.yz * b.xy - a.xy * b.yz,
				xy: a.zx * b.yz - a.yz * b.zx );

		/// <summary>Returns the normal of this bivector plane and its area</summary>
		public (Vector3 normal, float area) GetNormalAndArea() => HodgeDual.GetDirAndMagnitude();

		// Multiplication
		public static Bivector3 operator -( Bivector3 b ) => new Bivector3( -b.yz, -b.zx, -b.xy );
		public static Bivector3 operator *( float a, Bivector3 b ) => b * a;
		public static Bivector3 operator *( Bivector3 a, float b ) => new Bivector3( a.yz * b, a.zx * b, a.xy * b );

		public static Rotor3 operator *( Bivector3 a, Bivector3 b ) =>
			new(
				r: Dot( a, b ),
				b: Wedge( a, b )
			);

		public Rotor3 Square() =>
			new(
				-yz * yz - zx * zx - xy * xy,
				yz: xy * zx - zx * xy,
				zx: yz * xy - xy * yz,
				xy: zx * yz - yz * zx
			);

		public static Multivector3 operator *( Bivector3 a, Vector3 b ) {
			return new Multivector3(
				0, // real
				a.xy * b.y - a.zx * b.z, // vector
				a.yz * b.z - a.xy * b.x,
				a.zx * b.x - a.yz * b.y,
				0, 0, 0, // bivector
				a.yz * b.x + a.zx * b.y + a.xy * b.z // trivector
			);
		}

		public static Multivector3 operator *( Vector3 a, Bivector3 b ) {
			return new Multivector3(
				0, // real
				a.z * b.zx - a.y * b.xy, // vector
				a.x * b.xy - a.z * b.yz,
				a.y * b.yz - a.x * b.zx,
				0, 0, 0, // bivector
				a.x * b.yz + a.y * b.zx + a.z * b.xy // trivector
			);
		}

		// division
		public static Bivector3 operator /( Bivector3 a, float b ) => new Bivector3( a.yz / b, a.zx / b, a.xy / b );

		// addition
		public static Bivector3 operator +( Bivector3 a, Bivector3 b ) => new Bivector3( a.yz * b.yz, a.zx * b.zx, a.xy * b.xy );
		public static Multivector3 operator +( Bivector3 a, Trivector3 b ) => new Multivector3( 0, Vector3.zero, a, b );
		public static Multivector3 operator +( Trivector3 a, Bivector3 b ) => new Multivector3( 0, Vector3.zero, b, a );

		// casting
		public static explicit operator Vector3( Bivector3 bv ) => new Vector3( bv.yz, bv.zx, bv.xy );
		public static explicit operator Bivector3( Vector3 v ) => new Bivector3( v.x, v.y, v.z );

	}

}