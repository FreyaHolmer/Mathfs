using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Freya {

	public interface IVectorMath<V> {
		const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;
		[MethodImpl( INLINE )] V Add( V a, V b );
		[MethodImpl( INLINE )] V Sub( V a, V b );
		[MethodImpl( INLINE )] V Mul( V v, float c );
		[MethodImpl( INLINE )] V Div( V v, float c );
		[MethodImpl( INLINE )] float Dot( V a, V b );
		[MethodImpl( INLINE )] float Mag( V v );
		[MethodImpl( INLINE )] V Normalize( V v );

		[MethodImpl( INLINE )] V Mul( float c, V v ) => Mul( v, c );
		[MethodImpl( INLINE )] float SqMag( V v ) => Dot( v, v );
		[MethodImpl( INLINE )] float SqDist( V a, V b ) => SqMag( Sub( b, a ) );
		[MethodImpl( INLINE )] float Dist( V a, V b ) => MathF.Sqrt( SqDist( b, a ) );
		[MethodImpl( INLINE )] V VecProject( V p, V to ) => Mul( to, BasisProject( p, to ) );
		[MethodImpl( INLINE )] V VecReject( V p, V to ) => Sub( p, VecProject( p, to ) );
		[MethodImpl( INLINE )] float BasisProject( V p, V to ) => Dot( p, to ) / Dot( to, to );
		[MethodImpl( INLINE )] V VecFromLineToPoint( V o, V n, V p ) => VecReject( Sub( p, o ), n );
		[MethodImpl( INLINE )] float SqDistFromPointToLine( V o, V n, V p ) => SqMag( VecFromLineToPoint( o, n, p ) );
		[MethodImpl( INLINE )] V GetPointAlongLine( V o, V n, float t ) => Add( o, Mul( n, t ) );
		[MethodImpl( INLINE )] float ProjPointToLineSegmentTValue( V a, V b, V p ) => Mathf.Clamp01( ProjPointToLineTValue( a, Sub( b, a ), p ) );
		[MethodImpl( INLINE )] float ProjPointToLineTValue( V o, V n, V p ) => BasisProject( Sub( p, o ), n );
		[MethodImpl( INLINE )] V ProjPointToLine( V o, V n, V p ) => Add( o, VecProject( Sub( p, o ), n ) );
		[MethodImpl( INLINE )] V Lerp( V a, V b, float t ) => Add( Mul( 1f - t, a ), Mul( t, b ) );

		(float tA, float tB) ClosestPointBetweenLinesTValues( V aOrigin, V aDir, V bOrigin, V bDir ) {
			// source: https://math.stackexchange.com/questions/2213165/find-shortest-distance-between-lines-in-3d
			V e = Sub( aOrigin, bOrigin );
			float be = Dot( aDir, e );
			float de = Dot( bDir, e );
			float bd = Dot( aDir, bDir );
			float b2 = Dot( aDir, aDir );
			float d2 = Dot( bDir, bDir );
			float A = -b2 * d2 + bd * bd;
			float s = ( -b2 * de + be * bd ) / A;
			float t = ( d2 * be - de * bd ) / A;
			return ( t, s );
		}

		public bool TryIntersectSphereAtOrigin( V o, V n, float r, out (float tMin, float tMax) tValues ) {
			float nn = Dot( n, n );
			if( nn <= 0f ) { // vector has zero length, there's no direction
				tValues = default;
				return false;
			}
			float oo = Dot( o, o );
			float on = Dot( o, n );

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

	public struct VectorMath1D : IVectorMath<float> {
		const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;
		[MethodImpl( INLINE )] public float Add( float a, float b ) => a + b;
		[MethodImpl( INLINE )] public float Sub( float a, float b ) => a - b;
		[MethodImpl( INLINE )] public float Mul( float v, float c ) => v * c;
		[MethodImpl( INLINE )] public float Div( float v, float c ) => v / c;
		[MethodImpl( INLINE )] public float Dot( float a, float b ) => a * b;
		[MethodImpl( INLINE )] public float Mag( float v ) => MathF.Abs( v );
		[MethodImpl( INLINE )] public float Normalize( float v ) => v < 0 ? -1 : 1;
	}

	public struct VectorMath2D : IVectorMath<Vector2> {
		const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;
		[MethodImpl( INLINE )] public Vector2 Add( Vector2 a, Vector2 b ) => new(a.x + b.x, a.y + b.y);
		[MethodImpl( INLINE )] public Vector2 Sub( Vector2 a, Vector2 b ) => new(a.x - b.x, a.y - b.y);
		[MethodImpl( INLINE )] public Vector2 Mul( Vector2 v, float c ) => new(v.x * c, v.y * c);
		[MethodImpl( INLINE )] public Vector2 Div( Vector2 v, float c ) => new(v.x / c, v.y / c);
		[MethodImpl( INLINE )] public float Dot( Vector2 a, Vector2 b ) => a.x * b.x + a.y * b.y;
		[MethodImpl( INLINE )] public float Mag( Vector2 v ) => MathF.Sqrt( Dot( v, v ) );
		[MethodImpl( INLINE )] public Vector2 Normalize( Vector2 v ) => Div( v, Mag( v ) );
	}

	public struct VectorMath3D : IVectorMath<Vector3> {
		const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;
		[MethodImpl( INLINE )] public Vector3 Add( Vector3 a, Vector3 b ) => new(a.x + b.x, a.y + b.y, a.z + b.z);
		[MethodImpl( INLINE )] public Vector3 Sub( Vector3 a, Vector3 b ) => new(a.x - b.x, a.y - b.y, a.z - b.z);
		[MethodImpl( INLINE )] public Vector3 Mul( Vector3 v, float c ) => new(v.x * c, v.y * c, v.z * c);
		[MethodImpl( INLINE )] public Vector3 Div( Vector3 v, float c ) => new(v.x / c, v.y / c, v.z / c);
		[MethodImpl( INLINE )] public float Dot( Vector3 a, Vector3 b ) => a.x * b.x + a.y * b.y + a.z * b.z;
		[MethodImpl( INLINE )] public float Mag( Vector3 v ) => MathF.Sqrt( Dot( v, v ) );
		[MethodImpl( INLINE )] public Vector3 Normalize( Vector3 v ) => Div( v, Mag( v ) );
	}

	public struct VectorMath4D : IVectorMath<Vector4> {
		const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;
		[MethodImpl( INLINE )] public Vector4 Add( Vector4 a, Vector4 b ) => new(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
		[MethodImpl( INLINE )] public Vector4 Sub( Vector4 a, Vector4 b ) => new(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);
		[MethodImpl( INLINE )] public Vector4 Mul( Vector4 v, float c ) => new(v.x * c, v.y * c, v.z * c, v.w * c);
		[MethodImpl( INLINE )] public Vector4 Div( Vector4 v, float c ) => new(v.x / c, v.y / c, v.z / c, v.w / c);
		[MethodImpl( INLINE )] public float Dot( Vector4 a, Vector4 b ) => a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
		[MethodImpl( INLINE )] public float Mag( Vector4 v ) => MathF.Sqrt( Dot( v, v ) );
		[MethodImpl( INLINE )] public Vector4 Normalize( Vector4 v ) => Div( v, Mag( v ) );
	}

}