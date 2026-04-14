using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Freya {

	public static class VectorMathExt {
		const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

		[MethodImpl( INLINE )] public static float SqMag<V, VM>( this VM vm, V v ) where VM : struct, IVectorMath<V> => vm.Dot( v, v );
		[MethodImpl( INLINE )] public static float SqDist<V, VM>( this VM vm, V a, V b ) where VM : struct, IVectorMath<V> => vm.SqMag( vm.Sub( b, a ) );
		[MethodImpl( INLINE )] public static V VecProject<V, VM>( this VM vm, V p, V to ) where VM : struct, IVectorMath<V> => vm.Mul( to, vm.BasisProject( p, to ) );
		[MethodImpl( INLINE )] public static V VecReject<V, VM>( this VM vm, V p, V to ) where VM : struct, IVectorMath<V> => vm.Sub( p, vm.VecProject( p, to ) );
		[MethodImpl( INLINE )] public static float BasisProject<V, VM>( this VM vm, V p, V to ) where VM : struct, IVectorMath<V> => vm.Dot( p, to ) / vm.Dot( to, to );
		[MethodImpl( INLINE )] public static V VecFromLineToPoint<V, VM>( this VM vm, V o, V n, V p ) where VM : struct, IVectorMath<V> => vm.VecReject( vm.Sub( p, o ), n );
		[MethodImpl( INLINE )] public static float SqDistFromPointToLine<V, VM>( this VM vm, V o, V n, V p ) where VM : struct, IVectorMath<V> => vm.SqMag( vm.VecFromLineToPoint( o, n, p ) );
		[MethodImpl( INLINE )] public static V GetPointAlongLine<V, VM>( this VM vm, V o, V n, float t ) where VM : struct, IVectorMath<V> => vm.Add( o, vm.Mul( n, t ) );
		[MethodImpl( INLINE )] public static float ProjPointToLineSegmentTValue<V, VM>( this VM vm, V a, V b, V p ) where VM : struct, IVectorMath<V> => Mathf.Clamp01( vm.ProjPointToLineTValue( a, vm.Sub( b, a ), p ) );
		[MethodImpl( INLINE )] public static float ProjPointToLineTValue<V, VM>( this VM vm, V o, V n, V p ) where VM : struct, IVectorMath<V> => vm.BasisProject( vm.Sub( p, o ), n );
		[MethodImpl( INLINE )] public static V ProjPointToLine<V, VM>( this VM vm, V o, V n, V p ) where VM : struct, IVectorMath<V> => vm.Add( o, vm.VecProject( vm.Sub( p, o ), n ) );

		public static (float tA, float tB) ClosestPointBetweenLinesTValues<V, VM>( this VM vm, V aOrigin, V aDir, V bOrigin, V bDir ) where VM : struct, IVectorMath<V> {
			// source: https://math.stackexchange.com/questions/2213165/find-shortest-distance-between-lines-in-3d
			V e = vm.Sub( aOrigin, bOrigin );
			float be = vm.Dot( aDir, e );
			float de = vm.Dot( bDir, e );
			float bd = vm.Dot( aDir, bDir );
			float b2 = vm.Dot( aDir, aDir );
			float d2 = vm.Dot( bDir, bDir );
			float A = -b2 * d2 + bd * bd;
			float s = ( -b2 * de + be * bd ) / A;
			float t = ( d2 * be - de * bd ) / A;
			return ( t, s );
		}

		public static bool TryIntersectSphereAtOrigin<V, VM>( this VM vm, V o, V n, float r, out (float tMin, float tMax) tValues ) where VM : struct, IVectorMath<V> {
			float nn = vm.Dot( n, n );
			if( nn <= 0f ) { // vector has zero length, there's no direction
				tValues = default;
				return false;
			}
			float oo = vm.Dot( o, o );
			float on = vm.Dot( o, n );

			// quadratic terms
			double A = nn;
			double B = 2 * on;
			double C = oo - r * r;

			// try root solving
			double discriminant = B * B - 4 * A * C;
			if( discriminant < 0 ) { // no root, line is outside the circle
				tValues = default;
				return false;
			}
			int sign = B < 0 ? -1 : 1;
			double u = -B - sign * Math.Sqrt( discriminant );

			float tA = (float)( u / ( 2 * A ) );
			float tB = (float)( 2 * C / u );
			tValues = tA < tB ? ( tA, tB ) : ( tB, tA );
			return true;
		}

	}

	public interface IVectorMath<V> {
		const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;
		[MethodImpl( INLINE )] V Zero();
		[MethodImpl( INLINE )] V Add( V a, V b );
		[MethodImpl( INLINE )] V Sub( V a, V b );
		[MethodImpl( INLINE )] V Mul( V v, float c );
		[MethodImpl( INLINE )] V Mul( float c, V v );
		[MethodImpl( INLINE )] V Div( V v, float c );
		[MethodImpl( INLINE )] float Dot( V a, V b );
		[MethodImpl( INLINE )] float Mag( V v );
		[MethodImpl( INLINE )] float Dist( V a, V b );
		[MethodImpl( INLINE )] V Normalize( V v );
		[MethodImpl( INLINE )] V Lerp( V a, V b, float t );
	}

	public struct VectorMath1D : IVectorMath<float> {
		const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;
		[MethodImpl( INLINE )] public float Zero() => 0;
		[MethodImpl( INLINE )] public float Add( float a, float b ) => a + b;
		[MethodImpl( INLINE )] public float Sub( float a, float b ) => a - b;
		[MethodImpl( INLINE )] public float Mul( float v, float c ) => v * c;
		[MethodImpl( INLINE )] public float Div( float v, float c ) => v / c;
		[MethodImpl( INLINE )] public float Dot( float a, float b ) => a * b;
		[MethodImpl( INLINE )] public float Mag( float v ) => MathF.Abs( v );
		[MethodImpl( INLINE )] public float Dist( float a, float b ) => MathF.Abs( b - a );
		[MethodImpl( INLINE )] public float Normalize( float v ) => v < 0 ? -1 : 1;

		// shared implementations
		[MethodImpl( INLINE )] public float Lerp( float a, float b, float t ) => ( 1f - t ) * a + t * b;
	}

	public struct VectorMath2D : IVectorMath<Vector2> {
		const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;
		[MethodImpl( INLINE )] public Vector2 Zero() => new(0, 0);
		[MethodImpl( INLINE )] public Vector2 Add( Vector2 a, Vector2 b ) => new(a.x + b.x, a.y + b.y);
		[MethodImpl( INLINE )] public Vector2 Sub( Vector2 a, Vector2 b ) => new(a.x - b.x, a.y - b.y);
		[MethodImpl( INLINE )] public Vector2 Mul( Vector2 v, float c ) => new(v.x * c, v.y * c);
		[MethodImpl( INLINE )] public Vector2 Mul( float c, Vector2 v ) => new(v.x * c, v.y * c);
		[MethodImpl( INLINE )] public Vector2 Div( Vector2 v, float c ) => new(v.x / c, v.y / c);
		[MethodImpl( INLINE )] public float Dot( Vector2 a, Vector2 b ) => a.x * b.x + a.y * b.y;
		[MethodImpl( INLINE )] public float Mag( Vector2 v ) => MathF.Sqrt( Dot( v, v ) );
		[MethodImpl( INLINE )] public float Dist( Vector2 a, Vector2 b ) => Mag( Sub( b, a ) );
		[MethodImpl( INLINE )] public Vector2 Normalize( Vector2 v ) => Div( v, Mag( v ) );

		// shared implementations
		[MethodImpl( INLINE )] public Vector2 Lerp( Vector2 a, Vector2 b, float t ) {
			float omt = 1f - t;
			return new Vector2( omt * a.x + t * b.x, omt * a.y + t * b.y );
		}

	}

	public struct VectorMath3D : IVectorMath<Vector3> {
		const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;
		[MethodImpl( INLINE )] public Vector3 Zero() => new(0, 0, 0);
		[MethodImpl( INLINE )] public Vector3 Add( Vector3 a, Vector3 b ) => new(a.x + b.x, a.y + b.y, a.z + b.z);
		[MethodImpl( INLINE )] public Vector3 Sub( Vector3 a, Vector3 b ) => new(a.x - b.x, a.y - b.y, a.z - b.z);
		[MethodImpl( INLINE )] public Vector3 Mul( Vector3 v, float c ) => new(v.x * c, v.y * c, v.z * c);
		[MethodImpl( INLINE )] public Vector3 Mul( float c, Vector3 v ) => new(v.x * c, v.y * c, v.z * c);
		[MethodImpl( INLINE )] public Vector3 Div( Vector3 v, float c ) => new(v.x / c, v.y / c, v.z / c);
		[MethodImpl( INLINE )] public float Dot( Vector3 a, Vector3 b ) => a.x * b.x + a.y * b.y + a.z * b.z;
		[MethodImpl( INLINE )] public float Mag( Vector3 v ) => MathF.Sqrt( Dot( v, v ) );
		[MethodImpl( INLINE )] public float Dist( Vector3 a, Vector3 b ) => Mag( Sub( b, a ) );
		[MethodImpl( INLINE )] public Vector3 Normalize( Vector3 v ) => Div( v, Mag( v ) );

		// shared implementations
		[MethodImpl( INLINE )] public Vector3 Lerp( Vector3 a, Vector3 b, float t ) {
			float omt = 1f - t;
			return new Vector3( omt * a.x + t * b.x, omt * a.y + t * b.y, omt * a.z + t * b.z );
		}
	}

	public struct VectorMath4D : IVectorMath<Vector4> {
		const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;
		[MethodImpl( INLINE )] public Vector4 Zero() => new(0, 0, 0, 0);
		[MethodImpl( INLINE )] public Vector4 Add( Vector4 a, Vector4 b ) => new(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
		[MethodImpl( INLINE )] public Vector4 Sub( Vector4 a, Vector4 b ) => new(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);
		[MethodImpl( INLINE )] public Vector4 Mul( Vector4 v, float c ) => new(v.x * c, v.y * c, v.z * c, v.w * c);
		[MethodImpl( INLINE )] public Vector4 Mul( float c, Vector4 v ) => new(v.x * c, v.y * c, v.z * c, v.w * c);
		[MethodImpl( INLINE )] public Vector4 Div( Vector4 v, float c ) => new(v.x / c, v.y / c, v.z / c, v.w / c);
		[MethodImpl( INLINE )] public float Dot( Vector4 a, Vector4 b ) => a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
		[MethodImpl( INLINE )] public float Mag( Vector4 v ) => MathF.Sqrt( Dot( v, v ) );
		[MethodImpl( INLINE )] public float Dist( Vector4 a, Vector4 b ) => Mag( Sub( b, a ) );
		[MethodImpl( INLINE )] public Vector4 Normalize( Vector4 v ) => Div( v, Mag( v ) );

		// shared implementations
		[MethodImpl( INLINE )] public Vector4 Lerp( Vector4 a, Vector4 b, float t ) {
			float omt = 1f - t;
			return new Vector4( omt * a.x + t * b.x, omt * a.y + t * b.y, omt * a.z + t * b.z, omt * a.w + t * b.w );
		}
	}

	// todo: quaternions, as a treat. this is untested and unported basically lol
	public struct VectorMathQuat : IVectorMath<Quaternion> {
		const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;
		[MethodImpl( INLINE )] public Quaternion Zero() => new(0, 0, 0, 0);
		[MethodImpl( INLINE )] public Quaternion Add( Quaternion a, Quaternion b ) => new(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
		[MethodImpl( INLINE )] public Quaternion Sub( Quaternion a, Quaternion b ) => new(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);
		[MethodImpl( INLINE )] public Quaternion Mul( Quaternion v, float c ) => new(v.x * c, v.y * c, v.z * c, v.w * c);
		[MethodImpl( INLINE )] public Quaternion Mul( float c, Quaternion v ) => new(v.x * c, v.y * c, v.z * c, v.w * c);
		[MethodImpl( INLINE )] public Quaternion Div( Quaternion v, float c ) => new(v.x / c, v.y / c, v.z / c, v.w / c);
		[MethodImpl( INLINE )] public float Dot( Quaternion a, Quaternion b ) => a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
		[MethodImpl( INLINE )] public float Mag( Quaternion v ) => MathF.Sqrt( Dot( v, v ) );
		[MethodImpl( INLINE )] public float Dist( Quaternion a, Quaternion b ) => Mathfs.Angle( a, b );
		[MethodImpl( INLINE )] public Quaternion Normalize( Quaternion v ) => Div( v, Mag( v ) );

		// shared implementations
		[MethodImpl( INLINE )] public Quaternion Lerp( Quaternion a, Quaternion b, float t ) {
			float omt = 1f - t;
			return new Quaternion( omt * a.x + t * b.x, omt * a.y + t * b.y, omt * a.z + t * b.z, omt * a.w + t * b.w );
		}
	}

}