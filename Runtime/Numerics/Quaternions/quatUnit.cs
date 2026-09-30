using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Mathematics;
using UnityEngine;

namespace Freya {

	[StructLayout( LayoutKind.Sequential )]
	public struct quatUnit : IQuat<quatUnit> {
		public float x, y, z, w;

		public float3 AxisX =>
			new(
				1 - 2 * ( y * y + z * z ),
				2 * ( x * y + w * z ),
				2 * ( x * z - w * y )
			);
		public float3 AxisY =>
			new(
				2 * ( y * x - w * z ),
				1 - 2 * ( z * z + x * x ),
				2 * ( y * z + w * x )
			);
		public float3 AxisZ =>
			new(
				2 * ( z * x + w * y ),
				2 * ( z * y - w * x ),
				1 - 2 * ( x * x + y * y )
			);

		public float3x3 RotationMatrix {
			get {
				float xx = x * x;
				float yy = y * y;
				float zz = z * z;
				float xy = x * y;
				float yz = y * z;
				float zx = z * x;
				float xw = x * w;
				float yw = y * w;
				float zw = z * w;
				return new float3x3(
					1 - 2 * ( yy + zz ), 2 * ( xy - zw ), 2 * ( zx + yw ),
					2 * ( xy + zw ), 1 - 2 * ( zz + xx ), 2 * ( yz - xw ),
					2 * ( zx - yw ), 2 * ( yz + xw ), 1 - 2 * ( xx + yy )
				);
			}
		}

		// properties
		public float2 xy => new(x, y);
		public float2 yz => new(y, z);
		public float2 zx => new(z, x);
		public float2 yx => new(y, x);
		public float2 zy => new(z, y);
		public float2 xz => new(x, z);
		public float3 xyz => new(x, y, z);
		public float4 xyzw => new(x, y, z, w);
		public float4 yxwz => new(y, x, w, z);
		public float Real => w;
		public quatPure Im => new(x, y, z);
		public bool IsUnit => true;
		public bool IsPure => Mathfs.Approximately( w, 0 );
		public float SqMag => 1;
		public float Mag => 1;
		public float Angle {
			get => AngleS3 * 2;
			set => AngleS3 = value / 2;
		}
		public float AngleS3 {
			get => math.acos( math.clamp( w, -1, 1 ) );
			set {
				quatUnit q = this.pow( value / AngleS3 );
				this.x = q.x;
				this.y = q.y;
				this.z = q.z;
				this.w = q.w;
			}
		}
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
		public quatUnit Conj => new(-x, -y, -z, w);
		public quatUnit Inverse => Conj;
		public quat Exp() => quat.exp( this );
		public quatPure Ln() => quat.ln( this, out _ );
		public quatPure Ln( out float theta ) => quat.ln( this, out theta );
		public quatUnit pow( float t ) => quat.pow( this, t );
		public float3 Rotate( float3 v ) => quat.rotate( this, v );

		/// <summary>Rotates 180° around the extrinsic pre-rotation X axis, sometimes this is interpreted as a world space rotation, as opposed to rotating around its own axes</summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining )] public quatUnit rotate180AroundExtrX() => new(w, -z, y, -x);

		/// <summary>Rotates 180° around the extrinsic pre-rotation Y axis, sometimes this is interpreted as a world space rotation, as opposed to rotating around its own axes</summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining )] public quatUnit rotate180AroundExtrY() => new(z, w, -x, -y);

		/// <summary>Rotates 180° around the extrinsic pre-rotation Z axis, sometimes this is interpreted as a world space rotation, as opposed to rotating around its own axes</summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining )] public quatUnit rotate180AroundExtrZ() => new(-y, x, w, -z);

		/// <summary>Rotates 180° around its local X axis</summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining )] public quatUnit rotate180AroundSelfX() => new(w, z, -y, -x);

		/// <summary>Rotates 180° around its local Y axis</summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining )] public quatUnit rotate180AroundSelfY() => new(-z, w, x, -y);

		/// <summary>Rotates 180° around its local Z axis</summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining )] public quatUnit rotate180AroundSelfZ() => new(y, -x, w, -z);

		public quatUnit( float x, float y, float z, float w ) => ( this.x, this.y, this.z, this.w ) = ( x, y, z, w );
		public quatUnit( float3 xyz, float w ) => ( this.x, this.y, this.z, this.w ) = ( xyz.x, xyz.y, xyz.z, w );
		public quatUnit( Vector3 xyz, float w ) => ( this.x, this.y, this.z, this.w ) = ( xyz.x, xyz.y, xyz.z, w );
		public quatUnit( quatPure q, float w ) => ( this.x, this.y, this.z, this.w ) = ( q.x, q.y, q.z, w );
		public quatUnit( float4 values ) => ( this.x, this.y, this.z, this.w ) = ( values.x, values.y, values.z, values.w );
		public quatUnit( Vector4 values ) => ( this.x, this.y, this.z, this.w ) = ( values.x, values.y, values.z, values.w );

		// casting
		public static implicit operator quat( quatUnit q ) => new(q.x, q.y, q.z, q.w);
		public static implicit operator Unity.Mathematics.quaternion( quatUnit q ) => new(q.x, q.y, q.z, q.w);
		public static implicit operator UnityEngine.Quaternion( quatUnit q ) => new(q.x, q.y, q.z, q.w);

		public static explicit operator float4( quatUnit q ) => new(q.x, q.y, q.z, q.w);
		public static explicit operator Vector4( quatUnit q ) => new(q.x, q.y, q.z, q.w);
		public static explicit operator quatUnit( float4 q ) => new(q.x, q.y, q.z, q.w);
		public static explicit operator quatUnit( Vector4 q ) => new(q.x, q.y, q.z, q.w);
		public static explicit operator quatPure( quatUnit q ) => new(q.x, q.y, q.z);
		public static explicit operator quatUnit( UnityEngine.Quaternion q ) => new(q.x, q.y, q.z, q.w);
		public static explicit operator quatUnit( Unity.Mathematics.quaternion q ) => new(q.value);

		// basic operators
		public static quat operator +( quatUnit q, quatUnit p ) => new(q.x + p.x, q.y + p.y, q.z + p.z, q.w + p.w);
		public static quatUnit operator +( quatUnit q ) => q;
		public static quat operator -( quatUnit q, quatUnit p ) => new(q.x - p.x, q.y - p.y, q.z - p.z, q.w - p.w);
		public static quatUnit operator -( quatUnit q ) => new(-q.x, -q.y, -q.z, -q.w);
		public static quat operator /( quatUnit q, float x ) => new(q.x / x, q.y / x, q.z / x, q.w / x);
		public static quat operator *( float x, quatUnit q ) => new(q.x * x, q.y * x, q.z * x, q.w * x);
		public static quat operator *( quatUnit q, float x ) => new(q.x * x, q.y * x, q.z * x, q.w * x);

		public static quatUnit operator *( quatUnit q, quatUnit p ) => new(p.Im * q.w + q.Im * p.w + quat.cross( q.Im, p.Im ), q.w * p.w - quat.dot( q.Im, p.Im ));
		public static quat operator *( quatUnit q, quatPure p ) => new(p.Im * q.w + quat.cross( q.Im, p.Im ), -quat.dot( q.Im, p.Im ));
		public static quat operator *( quatPure q, quatUnit p ) => new(q.Im * p.w + quat.cross( q.Im, p.Im ), -quat.dot( q.Im, p.Im ));



	}

}