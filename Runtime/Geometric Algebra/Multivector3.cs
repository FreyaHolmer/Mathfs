using UnityEngine;

namespace Freya {

	public struct Multivector3 {

		public float r;
		public Vector3 v;
		public Bivector3 b;
		public Trivector3 t;

		public float x {
			get => v.x;
			set => v.x = value;
		}
		public float y {
			get => v.y;
			set => v.y = value;
		}
		public float z {
			get => v.z;
			set => v.z = value;
		}
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
		public float xyz {
			get => t.xyz;
			set => t.xyz = value;
		}

		public Multivector3( float r, float x, float y, float z, float yz, float zx, float xy, float xyz )
			: this(
				r,
				new Vector3( x, y, z ),
				new Bivector3( yz, zx, xy ),
				new Trivector3( xyz ) ) {}

		public Multivector3( float r, Vector3 v ) : this( r, v, Bivector3.zero, Trivector3.zero ) {}

		public Multivector3( float r, Vector3 v, Bivector3 b, Trivector3 t ) {
			this.r = r;
			this.v = v;
			this.b = b;
			this.t = t;
		}

		// multiplication
		public static Multivector3 operator *( Multivector3 a, Multivector3 b ) {
			// 64 multiplications, 56 add/sub
			// R:	a_r*b_r		+a_x*b_x	+a_y*b_y	+a_z*b_z	+a_yz*b_yz	+a_zx*b_zx	+a_xy*b_xy	+a_xyz*b_xyz
			// X:	a_r*b_x		+a_x*b_r	-a_y*b_xy	+a_z*b_zx	-a_yz*b_xyz	-a_zx*b_z	+a_xy*b_y	-a_xyz*b_yz
			// Y:	a_r*b_y		+a_x*b_xy	+a_y*b_r	-a_z*b_yz	+a_yz*b_z	-a_zx*b_xyz	-a_xy*b_x	-a_xyz*b_zx
			// Z:	a_r*b_z		-a_x*b_zx	+a_y*b_yz	+a_z*b_r	-a_yz*b_y	+a_zx*b_x	-a_xy*b_xyz	-a_xyz*b_xy
			// YZ:	a_r*b_yz	+a_x*b_xyz	+a_y*b_z	-a_z*b_y	+a_yz*b_r	-a_zx*b_xy	+a_xy*b_zx	+a_xyz*b_x
			// ZX:	a_r*b_zx	-a_x*b_z	+a_y*b_xyz	+a_z*b_x	+a_yz*b_xy	+a_zx*b_r	-a_xy*b_yz	+a_xyz*b_y
			// XY:	a_r*b_xy	+a_x*b_y	-a_y*b_x	+a_z*b_xyz	-a_yz*b_zx	+a_zx*b_yz	+a_xy*b_r	+a_xyz*b_z
			// XYZ:	a_r*b_xyz	+a_x*byz	+a_y*b_zx	+a_z*xy		+a_yz*b_x	+a_zx*b_y	+a_xy*b_z	+a_xyz*b_r
			return new Multivector3(
				a.r * b.r + a.x * b.x + a.y * b.y + a.z * b.z + a.yz * b.yz + a.zx * b.zx + a.xy * b.xy + a.xyz * b.xyz,
				a.r * b.x + a.x * b.r - a.y * b.xy + a.z * b.zx - a.yz * b.xyz - a.zx * b.z + a.xy * b.y - a.xyz * b.yz,
				a.r * b.y + a.x * b.xy + a.y * b.r - a.z * b.yz + a.yz * b.z - a.zx * b.xyz - a.xy * b.x - a.xyz * b.zx,
				a.r * b.z - a.x * b.zx + a.y * b.yz + a.z * b.r - a.yz * b.y + a.zx * b.x - a.xy * b.xyz - a.xyz * b.xy,
				a.r * b.yz + a.x * b.xyz + a.y * b.z - a.z * b.y + a.yz * b.r - a.zx * b.xy + a.xy * b.zx + a.xyz * b.x,
				a.r * b.zx - a.x * b.z + a.y * b.xyz + a.z * b.x + a.yz * b.xy + a.zx * b.r - a.xy * b.yz + a.xyz * b.y,
				a.r * b.xy + a.x * b.y - a.y * b.x + a.z * b.xyz - a.yz * b.zx + a.zx * b.yz + a.xy * b.r + a.xyz * b.z,
				a.r * b.xyz + a.x * b.yz + a.y * b.zx + a.z * b.xy + a.yz * b.x + a.zx * b.y + a.xy * b.z + a.xyz * b.r
			);
		}

		public static Multivector3 operator *( Multivector3 m, Rotor3 r ) {
			return new Multivector3(
				m.r * r.r + m.yz * r.yz + m.zx * r.zx + m.xy * r.xy,
				m.x * r.r - m.y * r.xy + m.z * r.zx - m.xyz * r.yz,
				+m.x * r.xy + m.y * r.r - m.z * r.yz - m.xyz * r.zx,
				-m.x * r.zx + m.y * r.yz + m.z * r.r - m.xyz * r.xy,
				m.r * r.yz + m.yz * r.r - m.zx * r.xy + m.xy * r.zx,
				m.r * r.zx + m.yz * r.xy + m.zx * r.r - m.xy * r.yz,
				m.r * r.xy - m.yz * r.zx + m.zx * r.yz + m.xy * r.r,
				m.x * r.yz + m.y * r.zx + m.z * r.xy + m.xyz * r.r
			);
		}

		public static Bivector3 Wedge( Multivector3 a, Multivector3 b ) =>
			new Bivector3(
				a.r * b.yz + a.x * b.xyz + a.y * b.z - a.z * b.y + a.yz * b.r - a.zx * b.xy + a.xy * b.zx + a.xyz * b.x,
				a.r * b.zx - a.x * b.z + a.y * b.xyz + a.z * b.x + a.yz * b.xy + a.zx * b.r - a.xy * b.yz + a.xyz * b.y,
				a.r * b.xy + a.x * b.y - a.y * b.x + a.z * b.xyz - a.yz * b.zx + a.zx * b.yz + a.xy * b.r + a.xyz * b.z );

		public static float Dot( Multivector3 a, Multivector3 b ) => a.r * b.r + a.x * b.x + a.y * b.y + a.z * b.z + a.yz * b.yz + a.zx * b.zx + a.xy * b.xy + a.xyz * b.xyz;

		// addition
		public static Multivector3 operator +( Multivector3 a, float b ) => new(a.r + b, a.v, a.b, a.t);
		public static Multivector3 operator +( float a, Multivector3 b ) => b + a;
		public static Multivector3 operator +( Multivector3 a, Vector3 b ) => new(a.r, a.v + b, a.b, a.t);
		public static Multivector3 operator +( Vector3 a, Multivector3 b ) => b + a;
		public static Multivector3 operator +( Multivector3 a, Bivector3 b ) => new(a.r, a.v, a.b + b, a.t);
		public static Multivector3 operator +( Bivector3 a, Multivector3 b ) => b + a;
		public static Multivector3 operator +( Multivector3 a, Trivector3 b ) => new(a.r, a.v, a.b, a.t + b);
		public static Multivector3 operator +( Trivector3 a, Multivector3 b ) => b + a;
		public static Multivector3 operator +( Multivector3 a, Multivector3 b ) => new(a.r + b.r, a.v + b.v, a.b + b.b, a.t + b.t);

		// combined
		public static Multivector3 operator +( Multivector3 a, Rotor3 b ) => new(a.r + b.r, a.v, a.b + b.b, a.t);
		public static Multivector3 operator +( Rotor3 a, Multivector3 b ) => b + a;

	}

}