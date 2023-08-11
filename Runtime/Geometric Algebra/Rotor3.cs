using System;
using UnityEngine;

namespace Freya {

	/// <summary>The even subalgebra of 3D VGA, isomorphic to Quaternions</summary>
	public struct Rotor3 {

		public float r;
		public Bivector3 b;
		public float yz {
			get => b.yz;
			set => b.yz = value;
		}
		public float zx {
			get => b.zx;
			set => b.zx = value;
		}
		public float xy {
			get => b.xy;
			set => b.xy = value;
		}

		public Rotor3( float r, float yz, float zx, float xy ) : this( r, new Bivector3( yz, zx, xy ) ) {}

		public Rotor3( float r, Bivector3 b ) {
			this.r = r;
			this.b = b;
		}

		/// <summary>Creates a rotation representing twice the angle from <c>a</c> to <c>b</c>.
		/// This is equivalent to multiplying the two vectors a*b.
		/// Note: Assumes both input vectors are normalized</summary>
		public static Rotor3 FromToRotationDouble( Vector3 a, Vector3 b ) => new Rotor3( Vector3.Dot( a, b ), Mathfs.Wedge( a, b ) );

		/// <summary>Creates a rotation from <c>a</c> to <c>b</c>. Note: Assumes both input vectors are normalized</summary>
		public static Rotor3 FromToRotation( Vector3 a, Vector3 b ) => new Rotor3( Vector3.Dot( a, b ) + 1, Mathfs.Wedge( a, b ) ).Normalized();

		/// <summary>Constructs a unit rotor representing a rotation</summary>
		public Rotor3( float angle, Vector3 axis ) {
			Bivector3 axisDual = new Bivector3( axis.x, axis.y, axis.z );
			float halfAngle = angle / 2;
			r = MathF.Cos( halfAngle );
			b = axisDual * MathF.Sin( halfAngle );
		}

		public float Magnitude => MathF.Sqrt( SqrMagnitude );
		public float SqrMagnitude => r * r + b.SqrMagnitude;

		public Rotor3 Normalized() => this / Magnitude;

		public Quaternion ToQuaternion() => new(yz, zx, xy, r);

		/// <summary>Negates the bivector, which is equivalent to reversing the rotation,
		/// if this is normalized an interpreted as a rotation</summary>
		public Rotor3 Conjugate => new Rotor3( r, -b );

		/// <summary>Sandwich product, equivalent to ⭐(R* ⭐v R) (where R* is the conjugate of R and ⭐v is the hodge dual of v).
		/// Commonly used to rotate vectors with unit rotors</summary>
		/// <param name="v">The vector to multiply (or rotate)</param>
		public Vector3 Rotate( Vector3 v ) {
			// hodge variant
			Bivector3 vHodge = new(v.x, v.y, v.z);
			return ( this.Conjugate * vHodge * this ).b.HodgeDual;
			// // todo: does not work - this does R ⭐v R* instead of R* ⭐v R
			// float r2 = r * r;
			// float yz2 = yz * yz;
			// float zx2 = zx * zx;
			// float xy2 = xy * xy;
			// float yzzx = yz * zx;
			// float zxxy = zx * xy;
			// float xyyz = xy * yz;
			// float rxy = r * xy;
			// float rzx = r * zx;
			// float ryz = r * yz;
			//
			// return new Vector3(
			// 	v.x * ( r2 + yz2 - zx2 - xy2 )
			// 	+ 2 * v.y * ( yzzx + rxy )
			// 	+ 2 * v.z * ( xyyz - rzx ),
			// 	v.y * ( r2 - yz2 + zx2 - xy2 )
			// 	+ 2 * v.x * ( yzzx - rxy )
			// 	+ 2 * v.z * ( ryz + zxxy ),
			// 	v.z * ( r2 - yz2 - zx2 + xy2 )
			// 	+ 2 * v.x * ( rzx + xyyz )
			// 	+ 2 * v.y * ( zxxy - ryz )
			// );
		}

		// multiplication
		public static Rotor3 operator *( Rotor3 a, Rotor3 b ) {
			return new Rotor3(
				a.r * b.r - a.yz * b.yz - a.zx * b.zx - a.xy * b.xy,
				a.r * b.yz + a.yz * b.r - a.zx * b.xy + a.xy * b.zx,
				a.r * b.zx + a.yz * b.xy + a.zx * b.r - a.xy * b.yz,
				a.r * b.xy - a.yz * b.zx + a.zx * b.yz + a.xy * b.r
			);
		}

		public static Rotor3 operator *( Bivector3 b, Rotor3 r ) {
			return new Rotor3(
				-b.yz * r.yz - b.zx * r.zx - b.xy * r.xy,
				+b.yz * r.r - b.zx * r.xy + b.xy * r.zx,
				+b.yz * r.xy + b.zx * r.r - b.xy * r.yz,
				-b.yz * r.zx + b.zx * r.yz + b.xy * r.r
			);
		}

		public static Rotor3 operator *( Rotor3 a, Bivector3 b ) {
			return new Rotor3(
				a.yz * b.yz + a.zx * b.zx + a.xy * b.xy,
				a.r * b.yz - a.zx * b.xy + a.xy * b.zx,
				a.r * b.zx + a.yz * b.xy + -a.xy * b.yz,
				a.r * b.xy - a.yz * b.zx + a.zx * b.yz
			);
		}

		public static Multivector3 operator *( Rotor3 a, Vector3 b ) {
			return new Multivector3(
				0,
				a.r * b.x - a.zx * b.z + a.xy * b.y,
				a.r * b.y + a.yz * b.z - a.xy * b.x,
				a.r * b.z - a.yz * b.y + a.zx * b.x,
				0,
				0,
				0,
				a.yz * b.x + a.zx * b.y + a.xy * b.z
			);
		}

		public static Multivector3 operator *( Vector3 b, Rotor3 a ) {
			return new Multivector3(
				0,
				a.r * b.x - a.zx * b.z + a.xy * b.y,
				a.r * b.y + a.yz * b.z - a.xy * b.x,
				a.r * b.z - a.yz * b.y + a.zx * b.x,
				0,
				0,
				0,
				a.yz * b.x + a.zx * b.y + a.xy * b.z
			);
		}


		public static Rotor3 operator /( Rotor3 a, float b ) {
			return new Rotor3( a.r / b, a.yz / b, a.zx / b, a.xy / b );
		}

		// addition
		public static Rotor3 operator +( Rotor3 a, float b ) => new Rotor3( a.r + b, a.b );
		public static Rotor3 operator +( float a, Rotor3 b ) => b + a;
		public static Rotor3 operator +( Rotor3 a, Bivector3 b ) => new Rotor3( a.r, a.b + b );
		public static Rotor3 operator +( Bivector3 a, Rotor3 b ) => b + a;
		public static Rotor3 operator +( Rotor3 a, Rotor3 b ) => new Rotor3( a.r + b.r, a.b + b.b );

	}

}