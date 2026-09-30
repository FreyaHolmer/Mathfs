using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions;

namespace Freya {

	public partial struct quat {

		public static readonly quatUnit i = new(1, 0, 0, 0);
		public static readonly quatUnit j = new(0, 1, 0, 0);
		public static readonly quatUnit k = new(0, 0, 1, 0);
		public static readonly quatUnit identity = new(0, 0, 0, 1);

		public static readonly quatUnit[] basis = {
			new(1, 0, 0, 0),
			new(0, 1, 0, 0),
			new(0, 0, 1, 0),
			new(0, 0, 0, 1)
		};



		}

		/// <summary>Rotates a vector <c>v</c> by the quaternion <c>q</c></summary>
		public static float3 rotate( quatUnit q, float3 v ) => (float3)( q * new quatPure( v ) * q.Conj ).Im; // todo: simplify

		public static float dot( quat a, quat b ) => a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
		public static float dot( quatUnit a, quatUnit b ) => a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
		public static float dot( quatPure a, quatPure b ) => a.x * b.x + a.y * b.y + a.z * b.z;
		public static quatPure cross( quatPure a, quatPure b ) => (quatPure)math.cross( (float3)a, (float3)b );

		// exp/log
		public static quatUnit pow( quatUnit q, float t ) => exp( ln( q ) * t );
		public static quat pow( quat q, float t ) => exp( ln( q ) * t );
		public static quat exp( quat q ) => exp( q.x, q.y, q.z, q.w );
		public static quat exp( float x, float y, float z, float w ) => exp( x, y, z ) * math.exp( w );
		public static quatUnit exp( float3 v ) => exp( v.x, v.y, v.z );
		public static quatUnit exp( quatPure q ) => exp( q.x, q.y, q.z );

		public static quatUnit exp( float x, float y, float z ) {
			float angleS3 = math.sqrt( x * x + y * y + z * z );
			float s = Mathfs.Sinc( angleS3 );
			return new quatUnit( x * s, y * s, z * s, math.cos( angleS3 ) );
		}

		public static quatPure ln( quatUnit q ) => ln( q, out _ );
		public static quat ln( quat q ) => ln( q, out _ );

		// ln(unit) = pure
		public static quatPure ln( quatUnit q, out float theta ) {
			float vMagSq = q.Im.SqMag;
			float vMag = math.sqrt( vMagSq );
			theta = math.atan2( vMag, q.w );
			float scV = ( vMag >= 0.001f ) ? theta / vMag : Mathfs.SincRcp( theta );
			return q.Im * scV;
		}

		public static quat ln( quat q, out float theta ) {
			float vMagSq = q.Im.SqMag;
			float vMag = math.sqrt( vMagSq );
			float qMagSq = vMagSq + q.w * q.w;

			theta = math.atan2( vMag, q.w );
			float scV;
			if( vMag >= 0.001f ) {
				scV = theta / vMag;
			} else {
				scV = Mathfs.SincRcp( theta );
			}
			float vW = 0;
			if( Mathfs.Approximately( qMagSq, 1 ) == false ) {
				// non-unit quaternion
				float qMag = math.sqrt( qMagSq );
				scV /= qMag;
				vW = math.log( qMag );
			}

			return new quat(
				scV * q.x,
				scV * q.y,
				scV * q.z,
				vW
			);
		}
		public enum DeltaPath {
			Natural,
			Shortest,
			Longest
		}

		static quatUnit DeltaPathAdjust( quatUnit a, quatUnit b, DeltaPath path ) {
			return path switch {
				DeltaPath.Shortest => dot( a, b ) < 0 ? -b : b, // flip b to the closer side
				DeltaPath.Longest  => dot( a, b ) > 0 ? -b : b, // flip b to the farther side
				_                  => b
			};
		}

		static quat DeltaPathAdjust( quat a, quat b, DeltaPath path ) {
			return path switch {
				DeltaPath.Shortest => dot( a, b ) < 0 ? -b : b, // flip b to the closer side
				DeltaPath.Longest  => dot( a, b ) > 0 ? -b : b, // flip b to the farther side
				_                  => b
			};
		}

		public static quatUnit Delta( quatUnit from, quatUnit to, DeltaPath path = DeltaPath.Natural ) => from.Conj * DeltaPathAdjust( from, to, path );
		public static quat Delta( quatUnit from, quat to, DeltaPath path = DeltaPath.Natural ) => from.Conj * DeltaPathAdjust( from, to, path );
		public static quat Delta( quat from, quat to, DeltaPath path = DeltaPath.Natural ) => from.Inverse * DeltaPathAdjust( from, to, path );
		public static quatUnit slerp( quatUnit a, quatUnit b, float t, DeltaPath path = DeltaPath.Natural ) => a * pow( Delta( a, b, path ), t );
		public static quat slerp( quat a, quat b, float t, DeltaPath path = DeltaPath.Natural ) => a * pow( Delta( a, b, path ), t );

		// bilinear spherical interpolation
		public static quatUnit Bislerp( quatUnit q00, quatUnit q01, quatUnit q10, quatUnit q11, float2 uv, DeltaPath path = DeltaPath.Natural ) {
			return slerp( slerp( q00, q01, uv.x, path ), slerp( q10, q11, uv.x, path ), uv.y );
		}
	}

}