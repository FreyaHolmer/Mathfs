using System;
using System.Runtime.InteropServices;
using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;

namespace Freya {


	[StructLayout( LayoutKind.Sequential )]
	public partial struct quat : IQuat<quat> {
		public float x, y, z, w;

		public quat( float x, float y, float z, float w ) => ( this.x, this.y, this.z, this.w ) = ( x, y, z, w );
		public quat( float3 xyz, float w ) => ( this.x, this.y, this.z, this.w ) = ( xyz.x, xyz.y, xyz.z, w );
		public quat( quatPure q, float w ) => ( this.x, this.y, this.z, this.w ) = ( q.x, q.y, q.z, w );
		public quat( float4 values ) => ( this.x, this.y, this.z, this.w ) = ( values.x, values.y, values.z, values.w );

		// properties
		public float2 xy => new(x, y);
		public float2 yz => new(y, z);
		public float2 zx => new(z, x);
		public float2 yx => new(y, x);
		public float2 zy => new(z, y);
		public float2 xz => new(x, z);
		public float3 xyz => new(x, y, z);
		public float4 xyzw => new(x, y, z, w);
		public float Real => w;
		public quatPure Im => new(x, y, z);
		public bool IsUnit => Mathfs.Approximately( SqMag, 1 );
		public bool IsPure => Mathfs.Approximately( w, 1 );
		public float SqMag => x * x + y * y + z * z + w * w;
		public float Mag => sqrt( SqMag );
		public quat Conj => new(-x, -y, -z, w);
		public quat Inverse => Conj / SqMag;
		public quatUnit Normalized {
			get {
				float m = rsqrt( SqMag );
				return new quatUnit( x * m, y * m, z * m, w * m );
			}
		}
		public quat exp() => quat.exp( this );
		public quat ln() => quat.ln( this, out _ );
		public quat ln( out float theta ) => quat.ln( this, out theta );
		public float this[ int i ] {
			get { return i switch { 0 => x, 1 => y, 2 => z, 3 => w, _ => this[i.Mod( 4 )] }; }
			set {
				switch( i ) {
					case 0:  x = value; break;
					case 1:  y = value; break;
					case 2:  z = value; break;
					case 3:  w = value; break;
					default: this[i.Mod( 4 )] = value; break;
				}
			}
		}

		public quatUnit ToUnitQuat( bool normalize = true ) => normalize ? Normalized : new quatUnit( x, y, z, w );

		// casting
		public static implicit operator Unity.Mathematics.quaternion( quat q ) => new(q.x, q.y, q.z, q.w);
		public static implicit operator UnityEngine.Quaternion( quat q ) => new(q.x, q.y, q.z, q.w);
		public static implicit operator quat( UnityEngine.Quaternion q ) => new(q.x, q.y, q.z, q.w);
		public static implicit operator quat( Unity.Mathematics.quaternion q ) => new(q.value);

		public static explicit operator quatPure( quat q ) => new(q.x, q.y, q.z);
		public static explicit operator float4( quat q ) => new(q.x, q.y, q.z, q.w);
		public static explicit operator Vector4( quat q ) => new(q.x, q.y, q.z, q.w);
		public static explicit operator quat( float4 q ) => new(q.x, q.y, q.z, q.w);
		public static explicit operator quat( Vector4 q ) => new(q.x, q.y, q.z, q.w);
		// public static explicit operator quatUnit( quat q ) => new(q.x, q.y, q.z);

		// basic operators
		public static quat operator +( quat q, quat p ) => new(q.x + p.x, q.y + p.y, q.z + p.z, q.w + p.w);
		public static quat operator +( quat q ) => q;
		public static quat operator -( quat q, quat p ) => new(q.x - p.x, q.y - p.y, q.z - p.z, q.w - p.w);
		public static quat operator -( quat q ) => new(-q.x, -q.y, -q.z, -q.w);
		public static quat operator /( quat q, float x ) => new(q.x / x, q.y / x, q.z / x, q.w / x);
		public static quat operator *( float x, quat q ) => new(q.x * x, q.y * x, q.z * x, q.w * x);
		public static quat operator *( quat q, float x ) => new(q.x * x, q.y * x, q.z * x, q.w * x);

		public static quat operator *( quat q, quat p ) {
			return new quat(
				p.Im * q.w + q.Im * p.w + quat.cross( q.Im, p.Im ),
				q.w * p.w - quat.dot( q.Im, p.Im )
			);
		}
	}

}