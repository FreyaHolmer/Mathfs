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

		public float Magnitude => MathF.Sqrt( SqrMagnitude );
		public float SqrMagnitude => r * r + b.SqrMagnitude;

		public Rotor3 Normalized() => this / Magnitude;

		/// <summary>Negates the bivector, which is equivalent to reversing the rotation,
		/// if this is normalized an interpreted as a rotation</summary>
		public Rotor3 Conjugate => new Rotor3( r, -b );

		/// <summary>Sandwich product, equivalent to RvR* (where R* is the conjugate of R).
		/// Commonly used to rotate vectors with unit rotors</summary>
		/// <param name="v">The vector to multiply (or rotate)</param>
		public Vector3 SandwichConjugate( Vector3 v ) {
			// todo: untested
			float r2 = r * r;
			float yz2 = yz * yz;
			float zx2 = zx * zx;
			float xy2 = xy * xy;
			float yzzx = yz * zx;
			float zxxy = zx * xy;
			float xyyz = xy * yz;
			float rxy = r * xy;
			float rzx = r * zx;
			float ryz = r * yz;

			return new Vector3(
				v.x * ( r2 + yz2 - zx2 - xy2 )
				+ 2 * v.y * ( yzzx + rxy )
				+ 2 * v.z * ( xyyz - rzx ),
				v.y * ( r2 - yz2 + zx2 - xy2 )
				+ 2 * v.x * ( yzzx - rxy )
				+ 2 * v.z * ( ryz + zxxy ),
				v.z * ( r2 - yz2 - zx2 + xy2 )
				+ 2 * v.x * ( rzx + xyyz )
				+ 2 * v.y * ( zxxy - ryz )
			);
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