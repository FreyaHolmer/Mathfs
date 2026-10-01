using System;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

namespace Freya {


	/// <summary>Static functions appropriate for <c>using static</c></summary>
	public static partial class mathfs {
		// todo: this file is under construction

		const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

		// global functions?
		public static int2 quadrantToBasisX( int i ) =>
			i switch {
				1 => new int2( 00, +1 ),
				2 => new int2( -1, 00 ),
				3 => new int2( 00, -1 ),
				_ => new int2( +1, 00 )
			};

		public static int quadrantToSignedQuadrant( int i ) => i switch { 1 => 1, 2 => -2, 3 => -1, _ => 0 };

		public static (int2 x, int2 y) quadrantToBasis( int i ) =>
			i switch {
				1 => ( new int2( 00, +1 ), new int2( -1, 00 ) ),
				2 => ( new int2( -1, 00 ), new int2( 00, -1 ) ),
				3 => ( new int2( 00, -1 ), new int2( +1, 00 ) ),
				_ => ( new int2( +1, 00 ), new int2( 00, +1 ) )
			};

		// todo: sort these:
		public static rat round( rat r, rat interval, RoundingDirection rounding = RoundingDirection.ToEven ) => ( r / interval ).round( rounding ) * interval;
		public static rat2 round( rat2 r, rat interval, RoundingDirection rounding = RoundingDirection.ToEven ) => new(round( r.x, interval ), round( r.y, interval ));
		public static rat2 round( rat2 r, rat2 intervals, RoundingDirection rounding = RoundingDirection.ToEven ) => new(round( r.x, intervals.x ), round( r.y, intervals.y ));

		// todo: these should probably move to codegen
		public static int csum( this bool b ) => b ? 1 : 0;
		public static int csum( this bool2 b ) => math.csum( (int2)b );
		public static int csum( this bool3 b ) => math.csum( (int3)b );
		public static int csum( this bool4 b ) => math.csum( (int4)b );

		public static int cmaxIndex( this float3 v ) {
			float3 a = math.abs( v );
			float max = a.cmax();
			if( a.x == max || float.IsNaN( max ) )
				return 0;
			if( a.y == max )
				return 1;
			return 2;
		}

		// todo: codegen freya pls
		/// <summary>Always -1 or 1. Returns 1 if x is 0 or NaN</summary>
		public static int safeSign( this float x ) => x < 0 ? -1 : 1;

		/// <inheritdoc cref="safeSign(float)" />
		public static int safeSign( this double x ) => x < 0 ? -1 : 1;

		/// <inheritdoc cref="safeSign(float)" />
		public static int safeSign( this int x ) => x < 0 ? -1 : 1;

		public static int randomSign() => UnityEngine.Random.value <= 0.5f ? -1 : 1;

		const double ANGLE_EPSILON_UNIT_PROJ = 1.0 / ( 1 << 12 );
		static readonly double UNIT_PROJ_THRESH_PARALLEL = Math.Cos( ANGLE_EPSILON_UNIT_PROJ );
		static readonly double UNIT_PROJ_THRESH_PERPENDICULAR = Math.Cos( Math.PI / 2 - ANGLE_EPSILON_UNIT_PROJ );

		/// <summary>Snaps the scalar projection of a unit vector (cosine of an angle) to the special values when near them</summary>
		public static double snapUnitProjection( double x, bool snapNear0 = false, bool snapNearEnds = true ) {
			if( snapNearEnds ) {
				if( x >= +UNIT_PROJ_THRESH_PARALLEL )
					return +1; // parallel, same direction
				if( x <= -UNIT_PROJ_THRESH_PARALLEL )
					return -1; // parallel, opposite directions
			}
			if( snapNear0 && Math.Abs( x ) <= UNIT_PROJ_THRESH_PERPENDICULAR )
				return 0; // perpendicular
			return x;
		}

		/// <inheritdoc cref="snapUnitProjection(double, bool, bool)" />
		public static float snapUnitProjection( float x, bool snapNear0 = false, bool snapNearEnds = true ) => (float)snapUnitProjection( (double)x, snapNear0 );

		/// <summary>Snaps to the nearest orthogonal axis. Note: might point along negative axes</summary>
		public static float3 nearestOrthoAxis( this float3 v ) {
			int i = math.abs( v ).cmaxIndex();
			return new float3 { [i] = v[i].safeSign() };
		}

		/// <summary>Returns an arbitrary perpendicular direction of the given unit vector</summary>
		public static float3 perpendicularDirection( this float3 dir ) {
			float3 axisOrtho = dir.nearestOrthoAxis().yzx; // swizzles to an axis guaranteed to be at least 45 degrees away
			return math.cross( axisOrtho, dir );
		}

		public static Vector3 asVec3( this int2 v, float z = 0f ) => new(v.x, v.y, z);
		public static float3 asFloat3( this int2 v, float z = 0f ) => new(v.x, v.y, z);
		public static Vector4 asVec4( this int3 v, float w = 0f ) => new(v.x, v.y, v.z, w);
		public static float4 asFloat4( this int3 v, float w = 0f ) => new(v.x, v.y, v.z, w);

		// UNSORTED:
		public static Rect expandFromCenter( this Rect r, float expansionPerSide ) {
			rat2 g = default;
			r.xMin -= expansionPerSide;
			r.yMin -= expansionPerSide;
			r.xMax += expansionPerSide;
			r.yMax += expansionPerSide;
			return r;
		}

		/// <summary>Unsigned shortest delta between <c>a</c> and <c>b</c> under modulo <c>mod</c></summary>
		public static int modDelta( int a, int b, int mod ) {
			a = a.Mod( mod );
			b = b.Mod( mod );
			int delta = math.max( a, b ) - math.min( a, b ); // delta is guaranteed to be positive here
			// find shortest direction:
			return Mathf.Min( mod - delta, delta );
		}

		/// <summary>The signed number of quadrants travered/rotated through in going from <c>a</c> and <c>b</c></summary>
		public static int quadrantDelta( int2 a, int2 b ) => a.wedge( b ).sign() * modDelta( a.quadrant(), b.quadrant(), 4 );

		/// <inheritdoc cref="quadrantDelta(int2,int2)" />
		public static int quadrantDelta( inth2 a, inth2 b ) => a.wedge( b ).sign * modDelta( a.quadrant, b.quadrant, 4 );

		/// <inheritdoc cref="quadrantDelta(int2,int2)" />
		public static int quadrantDelta( rat2 a, rat2 b ) => a.wedge( b ).sign * modDelta( a.quadrant, b.quadrant, 4 );

		/// <summary>Integer powers of floats, using repeated multiplication. Falls back to standard pow() beyond a power of 16</summary>
		public static float pow( this float x, int exp ) {
			switch( exp ) {
				case 0: return 1f;
				case 1: return x;
				case 2: return x * x;
				case 3: {
					return x * x * x;
				}
				case 4: {
					float x2 = x * x;
					return x2 * x2;
				}
				case 5: {
					float x2 = x * x;
					float x4 = x2 * x2;
					return x4 * x;
				}
				case 6: {
					float x2 = x * x;
					float x4 = x2 * x2;
					return x4 * x2;
				}
				case 7: {
					float x2 = x * x;
					float x4 = x2 * x2;
					return x4 * x2 * x;
				}
				case 8: {
					float x2 = x * x;
					float x4 = x2 * x2;
					return x4 * x4;
				}
				case 9: {
					float x2 = x * x;
					float x4 = x2 * x2;
					float x8 = x4 * x4;
					return x8 * x;
				}
				case 10: {
					float x2 = x * x;
					float x4 = x2 * x2;
					float x8 = x4 * x4;
					return x8 * x2;
				}
				case 11: {
					float x2 = x * x;
					float x4 = x2 * x2;
					float x8 = x4 * x4;
					return x8 * x2 * x;
				}
				case 12: {
					float x2 = x * x;
					float x4 = x2 * x2;
					float x8 = x4 * x4;
					return x8 * x4;
				}
				case 13: {
					float x2 = x * x;
					float x4 = x2 * x2;
					float x8 = x4 * x4;
					return x8 * x4 * x;
				}
				case 14: {
					float x2 = x * x;
					float x4 = x2 * x2;
					float x8 = x4 * x4;
					return x8 * x4 * x2;
				}
				case 15: {
					float x2 = x * x;
					float x4 = x2 * x2;
					float x8 = x4 * x4;
					return x8 * x4 * x2 * x;
				}
				case 16: {
					float x2 = x * x;
					float x4 = x2 * x2;
					float x8 = x4 * x4;
					return x8 * x8;
				}
				default: return MathF.Pow( x, exp );
			}
		}

		/// <summary>Returns 1 when even, -1 when odd</summary>
		public static int esign( this int x ) => x % 2 == 0 ? 1 : -1;

		public static inth divideBy2( this int p ) => new() { h = p };
		public static inth2 divideBy2( this int2 p ) => new(p.x.divideBy2(), p.y.divideBy2());

		public static int2 rot45chebyshev( this int2 v ) {
			int m = v.magChebyshev();
			return math.clamp( v + v.rot90(), new int2( -m, -m ), new int2( m, m ) );
		}

		// todo: codegen pls
		public static float projectionTValue( float2 v, float2 n ) => math.dot( v, n ) / math.dot( n, n );
		public static float projectionTValue( float3 v, float3 n ) => math.dot( v, n ) / math.dot( n, n );
		public static float projectionTValue( float4 v, float4 n ) => math.dot( v, n ) / math.dot( n, n );
		public static rat projectionTValue( rat2 v, rat2 n ) => dot( v, n ) / dot( n, n );
		public static rat projectionTValue( rat2 v, int2 n ) => dot( v, n ) / dot( n, n );

		public static rat projectionTValuePerp( rat2 v, rat2 n ) => dot( v, v ) / dot( v, n );

		public static float4 projectToNormal( float4 v, float4 n ) => n * projectionTValue( v, n );
		public static float3 projectToNormal( float3 v, float3 n ) => n * projectionTValue( v, n );
		public static float2 projectToNormal( float2 v, float2 n ) => n * projectionTValue( v, n );
		public static rat2 projectToNormal( rat2 v, rat2 n ) => n * projectionTValue( v, n );
		public static rat2 projectToNormal( rat2 v, int2 n ) => n * projectionTValue( v, n );

		public static rat2 projectToNormalPerp( rat2 v, rat2 n ) => n * projectionTValuePerp( v, n );

		public static rat2 lerp( rat2 a, rat2 b, rat2 t ) => new(lerp( a.x, b.x, t.x ), lerp( a.y, b.y, t.y ));
		public static rat2 lerp( rat2 a, rat2 b, rat t ) => a + t * ( b - a );
		public static rat lerp( rat a, rat b, rat t ) => a + t * ( b - a );

		public static rat inverseLerp( rat a, rat b, rat v ) => ( v - a ) / ( b - a );

		[MethodImpl( INLINE )] public static float magSq( float x ) => x * x;
		[MethodImpl( INLINE )] public static float magSq( float x, float y ) => x * x + y * y;
		[MethodImpl( INLINE )] public static float magSq( float x, float y, float z ) => x * x + y * y + z * z;
		[MethodImpl( INLINE )] public static float magSq( float x, float y, float z, float w ) => x * x + y * y + z * z + w * w;
		[MethodImpl( INLINE )] public static float mag( float x ) => math.sqrt( magSq( x ) );
		[MethodImpl( INLINE )] public static float mag( float x, float y ) => math.sqrt( magSq( x, y ) );
		[MethodImpl( INLINE )] public static float mag( float x, float y, float z ) => math.sqrt( magSq( x, y, z ) );
		[MethodImpl( INLINE )] public static float mag( float x, float y, float z, float w ) => math.sqrt( magSq( x, y, z, w ) );

		/// <summary>Computes ln(sqrt(x)) by ln(x)/2</summary>
		public static float logSqrt( float x ) => math.log( x ) / 2;

		public static float cosFromSin( float s ) {
			// when |X.y| < 0.1, the quadratic approximation error is about 0.0003°
			// 1px = 0.0013°, hFoV of 10° and x resolution of 7680 (8K)
			const float SINE_APPROX_THRESHOLD = 0.1f;
			float s2 = s * s;
			if( s2 < SINE_APPROX_THRESHOLD * SINE_APPROX_THRESHOLD ) { // use small angle approximation when close to being flat already
				return 1 - s2 * ( 1 + s2 / 4 ) / 2; // 1 - (1/2)x^2 - (1/8)x^4
			} else {
				float csq = 1 - s2;
				return csq * math.rsqrt( csq );
			}
		}


	}

}