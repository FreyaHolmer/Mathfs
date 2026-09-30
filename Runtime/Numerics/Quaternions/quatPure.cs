using System.Runtime.InteropServices;
using Unity.Mathematics;
using UnityEngine;

namespace Freya {

	[StructLayout( LayoutKind.Sequential )]
	public struct quatPure : IQuat<quatPure> {
		public float x, y, z;

		public float2 xy => new(x, y);
		public float2 yz => new(y, z);
		public float2 zx => new(z, x);
		public float2 yx => new(y, x);
		public float2 zy => new(z, y);
		public float2 xz => new(x, z);
		public float3 xyz => new(x, y, z);
		public float4 xyzw => new(x, y, z, w);
		public float w => 0;
		public float Real => 0;
		public quatPure Im => this;
		public bool IsUnit => Mathf.Approximately( SqMag, 1 );
		public bool IsPure => true;
		public float SqMag => x * x + y * y + z * z;
		public float Mag => math.sqrt( SqMag );
		public quatPure Conj => new(-x, -y, -z);
		public quatPure Inverse => Conj / SqMag;
		public float this[ int i ] => i switch { 0 => x, 1 => y, 2 => z, 3 => 0, _ => this[i.Mod( 4 )] };

		public quatUnit Exp() => quat.exp( this );
		public quat Ln() => quat.ln( this, out _ );
		public quat Ln( out float theta ) => quat.ln( this, out theta );
		public quat Scale( float t ) => quat.pow( this, t );

		public quatPure( float x, float y, float z ) => ( this.x, this.y, this.z ) = ( x, y, z );
		public quatPure( float3 values ) => ( this.x, this.y, this.z ) = ( values.x, values.y, values.z );
		public quatPure( Vector3 values ) => ( this.x, this.y, this.z ) = ( values.x, values.y, values.z );

		public static implicit operator quat( quatPure q ) => new(q.x, q.y, q.z, 0);
		public static implicit operator Unity.Mathematics.quaternion( quatPure q ) => new(q.x, q.y, q.z, 0);
		public static implicit operator UnityEngine.Quaternion( quatPure q ) => new(q.x, q.y, q.z, 0);

		public static explicit operator float3( quatPure q ) => new(q.x, q.y, q.z);
		public static explicit operator Vector3( quatPure q ) => new(q.x, q.y, q.z);
		public static explicit operator quatPure( float3 q ) => new(q.x, q.y, q.z);
		public static explicit operator quatPure( Vector3 q ) => new(q.x, q.y, q.z);
		public static explicit operator quatPure( UnityEngine.Quaternion q ) => new(q.x, q.y, q.z);
		public static explicit operator quatPure( Unity.Mathematics.quaternion q ) => new(q.value.xyz);

		// operators
		public static quatPure operator +( quatPure q, quatPure p ) => new(q.x + p.x, q.y + p.y, q.z + p.z);
		public static quatPure operator +( quatPure q ) => q;
		public static quatPure operator -( quatPure q, quatPure p ) => new(q.x - p.x, q.y - p.y, q.z - p.z);
		public static quatPure operator -( quatPure q ) => new(-q.x, -q.y, -q.z);
		public static quatPure operator /( quatPure q, float x ) => new(q.x / x, q.y / x, q.z / x);
		public static quatPure operator *( float x, quatPure q ) => new(q.x * x, q.y * x, q.z * x);
		public static quatPure operator *( quatPure q, float x ) => new(q.x * x, q.y * x, q.z * x);

		public static quat operator *( quatPure q, quatPure p ) {
			return new quat( // unverified
				quat.cross( q.Im, p.Im ),
				-quat.dot( q.Im, p.Im )
			);
		}

	}

}