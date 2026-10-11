using System;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions;

namespace Freya {

	public partial struct quat {
		const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

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

		/// <summary>Angle in radians. Assumes axis is already normalized</summary>
		public static quatUnit AxisAngle( float3 axis, float angle ) => exp( axis.x * ( angle / 2 ), axis.y * ( angle / 2 ), axis.z * ( angle / 2 ) );

		/// <summary>The quaternion rotation representation of a rotation vector, where the magnitude is the rotation angle in radians, and the direction is the axis</summary>
		public static quatUnit FromRotationVector( float3 v ) => exp( v.x / 2, v.y / 2, v.z / 2 );

		/// <summary>A 180 degree rotation around the specified axis. Assumes axis is already normalized</summary>
		public static quatUnit RotationBy180Around( float3 axis ) => new(axis.x, axis.y, axis.z, 0);

		/// <summary>A rotation around Y followed by an intrinsic rotation around X</summary>
		public static quatUnit FromYawPitch( float yaw, float pitch ) {
			math.sincos( yaw / 2, out float sy, out float cy );
			math.sincos( pitch / 2, out float sp, out float cp );
			return new quatUnit( cy * sp, sy * cp, -sy * sp, cy * cp );
		}

		/// <summary>A rotation that flips a normalized vector to its opposite direction</summary>
		public static quatUnit AxisFlipRotation( float3 v ) => RotationBy180Around( v.perpendicularDirection() );

		/// <summary>The rotation from a normalized vector a to another normalized vector b</summary>
		public static quatUnit FromToRotation( float3 a, float3 b ) {
			double dot = mathfs.snapUnitProjection( math.dot( (double3)a, b ), snapNear0: false );
			switch( dot ) {
				case -1:
					return AxisFlipRotation( a ); // 180 degree rotation around *any* axis perpendicular to both
				case 1:
					return identity;
				default:
					float w = (float)math.sqrt( ( 1 + dot ) / 2 );
					return new quatUnit( math.cross( a, b ) / ( 2 * w ), w );
			}
		}

		/// <summary>Rotates a vector <c>v</c> by the quaternion <c>q</c>. Equivalent to (q * (quatPure)v * q.Conj).Im</summary>
		public static float3 rotate( quatUnit q, float3 v ) {
			float3 u = q.xyz;
			float3 t = 2 * math.cross( u, v );
			return math.mad( q.w, t, v ) + math.cross( u, t );
		}

		public static float dot( quat a, quat b ) => a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
		public static float dot( quatUnit a, quatUnit b ) => a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
		public static float dot( quatPure a, quatPure b ) => a.x * b.x + a.y * b.y + a.z * b.z;
		public static quatPure cross( quatPure a, quatPure b ) => (quatPure)math.cross( (float3)a, (float3)b );

		// exp/log
		[MethodImpl( INLINE )] public static quat exp( quat q ) => exp( q.x, q.y, q.z, q.w );
		[MethodImpl( INLINE )] public static quat pow( quat q, float t ) => pow( q.x, q.y, q.z, q.w, t );
		[MethodImpl( INLINE )] public static quat ln( quat q ) => ln( q.x, q.y, q.z, q.w, out _ );
		[MethodImpl( INLINE )] public static quat ln( quat q, out float angleS3 ) => ln( q.x, q.y, q.z, q.w, out angleS3 );

		/// <inheritdoc cref="exp(float,float,float)" />
		[MethodImpl( INLINE )] public static quatUnit exp( float3 v ) => exp( v.x, v.y, v.z );

		/// <inheritdoc cref="exp(float,float,float)" />
		[MethodImpl( INLINE )] public static quatUnit exp( quatPure q ) => exp( q.x, q.y, q.z );

		/// <inheritdoc cref="lnUnit(float,float,float,float,out float)" />
		[MethodImpl( INLINE )] public static quatPure ln( quatUnit q ) => lnUnit( q.x, q.y, q.z, q.w, out _ );

		/// <inheritdoc cref="lnUnit(float,float,float,float,out float)" />
		[MethodImpl( INLINE )] public static quatPure ln( quatUnit q, out float angleS3 ) => lnUnit( q.x, q.y, q.z, q.w, out angleS3 );

		/// <inheritdoc cref="powUnit(float,float,float,float,float)" />
		[MethodImpl( INLINE )] public static quatUnit pow( quatUnit q, float t ) => powUnit( q.x, q.y, q.z, q.w, t );

		public static quat exp( float x, float y, float z, float w ) { // exp( x, y, z ) * math.exp( w )
			float angleS3 = mathfs.mag( x, y, z );
			math.sincos( angleS3, out float sin, out float cos );
			float r = math.exp( w );
			float scale = r * mathfs.sincFromSin( sin, angleS3 );
			return new quat( x * scale, y * scale, z * scale, cos * r );
		}

		/// <summary>Converts a pure quaternion, often representing a half-angle rotation vector,
		/// into a unit quaternion, representing a rotation by twice the half-angle</summary>
		public static quatUnit exp( float x, float y, float z ) {
			float angleS3 = mathfs.mag( x, y, z );
			math.sincos( angleS3, out float sin, out float cos );
			float scale = mathfs.sincFromSin( sin, angleS3 );
			return new quatUnit( x * scale, y * scale, z * scale, cos );
		}

		/// <summary>Converts a unit quaternion, often representing a rotation,
		/// into a pure quaternion, usually representing a half-angle rotation vector</summary>
		/// <param name="angleS3">half of the original quaternion rotation angle</param>
		public static quatPure lnUnit( float x, float y, float z, float w, out float angleS3 ) {
			decomposePolar( x, y, z, w, out float s, out float sSq, out angleS3 );
			float scale = nearIdentity( sSq, w ) ? 1f : angleS3 / s;
			return new quatPure( x * scale, y * scale, z * scale );
		}

		public static quat ln( float x, float y, float z, float w, out float angleS3 ) {
			decomposePolar( x, y, z, w, out float s, out float sSq, out angleS3 );
			float scale = nearIdentity( sSq, w ) ? 1f / w : angleS3 / s;
			return new quat( x * scale, y * scale, z * scale, mathfs.logSqrt( sSq + w * w ) );
		}

		/// <summary>Scales a quaternion's rotation angle by <c>t</c></summary>
		public static quatUnit powUnit( float x, float y, float z, float w, float t ) { // exp( ln( q ) * t )
			switch( t ) {
				case 0: return new quatUnit( 0, 0, 0, 1 );
				case 1: return new quatUnit( x, y, z, w );
				default:
					decomposePolar( x, y, z, w, out float s, out float sSq, out float angleS3 );
					math.sincos( t * angleS3, out float newSin, out float newCos );
					float scale = nearIdentity( sSq, w ) ? t : newSin / s;
					return new quatUnit( x * scale, y * scale, z * scale, newCos );
			}
		}

		public static quat pow( float x, float y, float z, float w, float t ) { // exp( ln( q ) * t )
			switch( t ) {
				case 0: return new quat( 0, 0, 0, 1 );
				case 1: return new quat( x, y, z, w );
				default:
					decomposePolar( x, y, z, w, out float s, out float sSq, out float angleS3 );
					math.sincos( t * angleS3, out float newSin, out float newCos );
					float r = math.exp( t * mathfs.logSqrt( sSq + w * w ) ); // radial scale
					float scale = r * ( nearIdentity( sSq, w ) ? t / w : newSin / s );
					return new quat( x * scale, y * scale, z * scale, newCos * r );
			}
		}

		const float EPSILON_LN = 1e-12f;
		[MethodImpl( INLINE )] static bool nearIdentity( float sSq, float w ) => w > 0 && sSq < EPSILON_LN * w * w;

		/// <summary>Decomposes the quaternion into its sine component magnitude and angleS3</summary>
		[MethodImpl( INLINE )] static void decomposePolar( float x, float y, float z, float w, out float mag, out float magSq, out float angleS3 ) {
			mag = math.sqrt( magSq = ( x * x + y * y + z * z ) );
			angleS3 = math.atan2( mag, w );
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

		// random rotations
		public static quatUnit RandomRotation => ( mathfs.randomSign() * (quat)UnityEngine.Random.rotationUniform ).ToUnitQuat( normalize: false );
		public static quatUnit RandomRotationUpright => ( mathfs.randomSign() * (quat)quaternion.LookRotationSafe( UnityEngine.Random.onUnitSphere, new float3( 0, 1, 0 ) ) ).ToUnitQuat( normalize: false );


		/// <summary>Removes "roll" with the shortest possible rotation, by flattening the X axis to the extrinsic Y plane</summary>
		public static quatUnit FlattenXtoYPlane( quatUnit q ) {
			// optimized version of: fromToRotation( X, math.normalize( X.zeroY() ) ) * q;
			float sin = 2 * ( q.x * q.y + q.w * q.z ); // sin(x), aka X.y
			float cos = mathfs.cosFromSin( sin ); // cos(x)
			float sec = 1f / cos; // sec(x)
			float tan = sin * sec; // tan(x)
			return new quatUnit( math.normalize( ( sec + 1 ) * q.xyzw - tan * q.yxwz ) );
		}
	}

}