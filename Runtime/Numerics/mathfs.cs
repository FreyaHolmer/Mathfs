using System;
using Unity.Mathematics;
using UnityEngine;

namespace Freya {


	/// <summary>Static functions appropriate for <c>using static</c></summary>
	public static partial class mathfs {

		// todo: this file is under construction

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

		public static inth divideBy2( this int p ) => new() { h = p };
		public static inth2 divideBy2( this int2 p ) => new(p.x.divideBy2(), p.y.divideBy2());

		public static int2 rot45chebyshev( this int2 v ) {
			int m = v.magChebyshev();
			return math.clamp( v + v.rot90(), new int2( -m, -m ), new int2( m, m ) );
		}

		public static float projectionTValue( float2 v, float2 n ) => math.dot( v, n ) / math.dot( n, n );
		public static rat projectionTValue( rat2 v, rat2 n ) => dot( v, n ) / dot( n, n );
		public static rat projectionTValue( rat2 v, int2 n ) => dot( v, n ) / dot( n, n );

		public static rat projectionTValuePerp( rat2 v, rat2 n ) => dot( v, v ) / dot( v, n );

		public static float2 projectToNormal( float2 v, float2 n ) => n * projectionTValue( v, n );
		public static rat2 projectToNormal( rat2 v, rat2 n ) => n * projectionTValue( v, n );
		public static rat2 projectToNormal( rat2 v, int2 n ) => n * projectionTValue( v, n );

		public static rat2 projectToNormalPerp( rat2 v, rat2 n ) => n * projectionTValuePerp( v, n );

		public static rat2 lerp( rat2 a, rat2 b, rat2 t ) => new(lerp( a.x, b.x, t.x ), lerp( a.y, b.y, t.y ));
		public static rat2 lerp( rat2 a, rat2 b, rat t ) => a + t * ( b - a );
		public static rat lerp( rat a, rat b, rat t ) => a + t * ( b - a );

		public static rat inverseLerp( rat a, rat b, rat v ) => ( v - a ) / ( b - a );


	}

}