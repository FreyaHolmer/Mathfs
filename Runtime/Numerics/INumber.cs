// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using System;
using Unity.Mathematics;

namespace Freya {

	public interface INumber {
		/// <summary>Returns whether this number is an integer</summary>
		public bool isInteger { get; }

		/// <summary>Returns whether this vector is the zero vector</summary>
		public bool isZero { get; }

		/// <summary>Returns whether this lies flat along at least one axis</summary>
		public bool isOrthogonal { get; }
	}

	public static partial class mathfs {
		/// <inheritdoc cref="INumber.isInteger" />
		public static bool isInteger<T>( T v ) where T : INumber => v.isInteger;

		/// <inheritdoc cref="INumber.isInteger" />
		public static bool isInteger( rat v ) => v.isInteger;

		/// <inheritdoc cref="INumber.isInteger" />
		public static bool isInteger( rat2 v ) => v.isInteger;

		/// <inheritdoc cref="INumber.isInteger" />
		public static bool isInteger( inth v ) => v.isInteger;

		/// <inheritdoc cref="INumber.isInteger" />
		public static bool isInteger( inth2 v ) => v.isInteger;

		/// <inheritdoc cref="INumber.isInteger" />
		public static bool isInteger( this int v ) => true;

		/// <inheritdoc cref="INumber.isInteger" />
		public static bool isInteger( this int2 v ) => true;

		/// <inheritdoc cref="INumber.isInteger" />
		public static bool isInteger( this int3 v ) => true;

		/// <inheritdoc cref="INumber.isInteger" />
		public static bool isInteger( this int4 v ) => true;

		/// <inheritdoc cref="INumber.isInteger" />
		public static bool isInteger( this float v ) => v == MathF.Truncate( v );

		/// <inheritdoc cref="INumber.isInteger" />
		public static bool isInteger( this float2 v ) => v.x.isInteger() && v.y.isInteger();

		/// <inheritdoc cref="INumber.isInteger" />
		public static bool isInteger( this float3 v ) => v.x.isInteger() && v.y.isInteger() && v.z.isInteger();

		/// <inheritdoc cref="INumber.isInteger" />
		public static bool isInteger( this float4 v ) => v.x.isInteger() && v.y.isInteger() && v.z.isInteger() && v.w.isInteger();

		/// <inheritdoc cref="INumber.isInteger" />
		public static bool isInteger( this double v ) => v == Math.Truncate( v );

		/// <inheritdoc cref="INumber.isInteger" />
		public static bool isInteger( this double2 v ) => v.x.isInteger() && v.y.isInteger();

		/// <inheritdoc cref="INumber.isInteger" />
		public static bool isInteger( this double3 v ) => v.x.isInteger() && v.y.isInteger() && v.z.isInteger();

		/// <inheritdoc cref="INumber.isInteger" />
		public static bool isInteger( this double4 v ) => v.x.isInteger() && v.y.isInteger() && v.z.isInteger() && v.w.isInteger();
		
		
		/// <inheritdoc cref="INumber.isZero" />
		public static bool isZero<T>( T v ) where T : INumber => v.isZero;

		/// <inheritdoc cref="INumber.isZero" />
		public static bool isZero( rat v ) => v.isZero;

		/// <inheritdoc cref="INumber.isZero" />
		public static bool isZero( rat2 v ) => v.isZero;

		/// <inheritdoc cref="INumber.isZero" />
		public static bool isZero( inth v ) => v.isZero;

		/// <inheritdoc cref="INumber.isZero" />
		public static bool isZero( inth2 v ) => v.isZero;

		/// <inheritdoc cref="INumber.isZero" />
		public static bool isZero( this int v ) => true;

		/// <inheritdoc cref="INumber.isZero" />
		public static bool isZero( this int2 v ) => true;

		/// <inheritdoc cref="INumber.isZero" />
		public static bool isZero( this int3 v ) => true;

		/// <inheritdoc cref="INumber.isZero" />
		public static bool isZero( this int4 v ) => true;

		/// <inheritdoc cref="INumber.isZero" />
		public static bool isZero( this float v ) => v == 0;

		/// <inheritdoc cref="INumber.isZero" />
		public static bool isZero( this float2 v ) => math.all( v == 0 );

		/// <inheritdoc cref="INumber.isZero" />
		public static bool isZero( this float3 v ) => math.all( v == 0 );

		/// <inheritdoc cref="INumber.isZero" />
		public static bool isZero( this float4 v ) => math.all( v == 0 );

		/// <inheritdoc cref="INumber.isZero" />
		public static bool isZero( this double v ) => v == 0;

		/// <inheritdoc cref="INumber.isZero" />
		public static bool isZero( this double2 v ) => math.all( v == 0 );

		/// <inheritdoc cref="INumber.isZero" />
		public static bool isZero( this double3 v ) => math.all( v == 0 );

		/// <inheritdoc cref="INumber.isZero" />
		public static bool isZero( this double4 v ) => math.all( v == 0 );
		
		
		/// <inheritdoc cref="INumber.isOrthogonal" />
		public static bool isOrthogonal<T>( T v ) where T : INumber => v.isOrthogonal;

		/// <inheritdoc cref="INumber.isOrthogonal" />
		public static bool isOrthogonal( rat v ) => v.isOrthogonal;

		/// <inheritdoc cref="INumber.isOrthogonal" />
		public static bool isOrthogonal( rat2 v ) => v.isOrthogonal;

		/// <inheritdoc cref="INumber.isOrthogonal" />
		public static bool isOrthogonal( inth v ) => v.isOrthogonal;

		/// <inheritdoc cref="INumber.isOrthogonal" />
		public static bool isOrthogonal( inth2 v ) => v.isOrthogonal;

		/// <inheritdoc cref="INumber.isOrthogonal" />
		public static bool isOrthogonal( this int v ) => true;

		/// <inheritdoc cref="INumber.isOrthogonal" />
		public static bool isOrthogonal( this int2 v ) => true;

		/// <inheritdoc cref="INumber.isOrthogonal" />
		public static bool isOrthogonal( this int3 v ) => true;

		/// <inheritdoc cref="INumber.isOrthogonal" />
		public static bool isOrthogonal( this int4 v ) => true;

		/// <inheritdoc cref="INumber.isOrthogonal" />
		public static bool isOrthogonal( this float v ) => v == 0;

		/// <inheritdoc cref="INumber.isOrthogonal" />
		public static bool isOrthogonal( this float2 v ) => math.all( v == 0 );

		/// <inheritdoc cref="INumber.isOrthogonal" />
		public static bool isOrthogonal( this float3 v ) => math.all( v == 0 );

		/// <inheritdoc cref="INumber.isOrthogonal" />
		public static bool isOrthogonal( this float4 v ) => math.all( v == 0 );

		/// <inheritdoc cref="INumber.isOrthogonal" />
		public static bool isOrthogonal( this double v ) => v == 0;

		/// <inheritdoc cref="INumber.isOrthogonal" />
		public static bool isOrthogonal( this double2 v ) => math.all( v == 0 );

		/// <inheritdoc cref="INumber.isOrthogonal" />
		public static bool isOrthogonal( this double3 v ) => math.all( v == 0 );

		/// <inheritdoc cref="INumber.isOrthogonal" />
		public static bool isOrthogonal( this double4 v ) => math.all( v == 0 );

	}

	public interface INumber<N> : INumber {
		/// <summary>Returns the absolute value of the number. Makes negative values positive</summary>
		public N abs { get; }

		/// <summary>Returns the minimum of two numbers</summary>
		public N min( N other );

		/// <summary>Returns the maximum of two numbers</summary>
		public N max( N other );

		/// <summary>The vector from this point to the target. Equivalent to <c>target - this</c></summary>
		public N to( N target );

		// I can't do this bc Unity uses older versions of C#:
		// public static abstract R zero { get; }
		// public static abstract R one { get; }
	}

	public static partial class mathfs {
		/// <inheritdoc cref="INumber{N}.abs" />
		public static T abs<T>( T x ) where T : INumber<T> => x.abs;

		/// <inheritdoc cref="INumber{N}.abs" />
		public static inth abs( inth x ) => x.abs;

		/// <inheritdoc cref="INumber{N}.abs" />
		public static rat abs( rat x ) => x.abs;

		/// <inheritdoc cref="INumber{N}.abs" />
		public static inth2 abs( inth2 x ) => x.abs;

		/// <inheritdoc cref="INumber{N}.abs" />
		public static rat2 abs( rat2 x ) => x.abs;

		/// <inheritdoc cref="INumber{N}.abs" />
		public static int abs( this int x ) => math.abs( x );

		/// <inheritdoc cref="INumber{N}.abs" />
		public static int2 abs( this int2 x ) => math.abs( x );

		/// <inheritdoc cref="INumber{N}.abs" />
		public static int3 abs( this int3 x ) => math.abs( x );

		/// <inheritdoc cref="INumber{N}.abs" />
		public static int4 abs( this int4 x ) => math.abs( x );

		/// <inheritdoc cref="INumber{N}.abs" />
		public static float abs( this float x ) => math.abs( x );

		/// <inheritdoc cref="INumber{N}.abs" />
		public static float2 abs( this float2 x ) => math.abs( x );

		/// <inheritdoc cref="INumber{N}.abs" />
		public static float3 abs( this float3 x ) => math.abs( x );

		/// <inheritdoc cref="INumber{N}.abs" />
		public static float4 abs( this float4 x ) => math.abs( x );

		/// <inheritdoc cref="INumber{N}.abs" />
		public static double abs( this double x ) => math.abs( x );

		/// <inheritdoc cref="INumber{N}.abs" />
		public static double2 abs( this double2 x ) => math.abs( x );

		/// <inheritdoc cref="INumber{N}.abs" />
		public static double3 abs( this double3 x ) => math.abs( x );

		/// <inheritdoc cref="INumber{N}.abs" />
		public static double4 abs( this double4 x ) => math.abs( x );


		/// <inheritdoc cref="INumber{N}.min" />
		public static T min<T>( T a, T b ) where T : INumber<T> => a.min( b );

		/// <inheritdoc cref="INumber{N}.min" />
		public static inth min( inth a, inth b ) => a.min( b );

		/// <inheritdoc cref="INumber{N}.min" />
		public static rat min( rat a, rat b ) => a.min( b );

		/// <inheritdoc cref="INumber{N}.min" />
		public static inth2 min( inth2 a, inth2 b ) => a.min( b );

		/// <inheritdoc cref="INumber{N}.min" />
		public static rat2 min( rat2 a, rat2 b ) => a.min( b );

		/// <inheritdoc cref="INumber{N}.min" />
		public static int min( this int a, int b ) => math.min( a, b );

		/// <inheritdoc cref="INumber{N}.min" />
		public static int2 min( this int2 a, int2 b ) => math.min( a, b );

		/// <inheritdoc cref="INumber{N}.min" />
		public static int3 min( this int3 a, int3 b ) => math.min( a, b );

		/// <inheritdoc cref="INumber{N}.min" />
		public static int4 min( this int4 a, int4 b ) => math.min( a, b );

		/// <inheritdoc cref="INumber{N}.min" />
		public static float min( this float a, float b ) => math.min( a, b );

		/// <inheritdoc cref="INumber{N}.min" />
		public static float2 min( this float2 a, float2 b ) => math.min( a, b );

		/// <inheritdoc cref="INumber{N}.min" />
		public static float3 min( this float3 a, float3 b ) => math.min( a, b );

		/// <inheritdoc cref="INumber{N}.min" />
		public static float4 min( this float4 a, float4 b ) => math.min( a, b );

		/// <inheritdoc cref="INumber{N}.min" />
		public static double min( this double a, double b ) => math.min( a, b );

		/// <inheritdoc cref="INumber{N}.min" />
		public static double2 min( this double2 a, double2 b ) => math.min( a, b );

		/// <inheritdoc cref="INumber{N}.min" />
		public static double3 min( this double3 a, double3 b ) => math.min( a, b );

		/// <inheritdoc cref="INumber{N}.min" />
		public static double4 min( this double4 a, double4 b ) => math.min( a, b );


		/// <inheritdoc cref="INumber{N}.max" />
		public static T max<T>( T a, T b ) where T : INumber<T> => a.max( b );

		/// <inheritdoc cref="INumber{N}.max" />
		public static inth max( inth a, inth b ) => a.max( b );

		/// <inheritdoc cref="INumber{N}.max" />
		public static rat max( rat a, rat b ) => a.max( b );

		/// <inheritdoc cref="INumber{N}.max" />
		public static inth2 max( inth2 a, inth2 b ) => a.max( b );

		/// <inheritdoc cref="INumber{N}.max" />
		public static rat2 max( rat2 a, rat2 b ) => a.max( b );

		/// <inheritdoc cref="INumber{N}.max" />
		public static int max( this int a, int b ) => math.max( a, b );

		/// <inheritdoc cref="INumber{N}.max" />
		public static int2 max( this int2 a, int2 b ) => math.max( a, b );

		/// <inheritdoc cref="INumber{N}.max" />
		public static int3 max( this int3 a, int3 b ) => math.max( a, b );

		/// <inheritdoc cref="INumber{N}.max" />
		public static int4 max( this int4 a, int4 b ) => math.max( a, b );

		/// <inheritdoc cref="INumber{N}.max" />
		public static float max( this float a, float b ) => math.max( a, b );

		/// <inheritdoc cref="INumber{N}.max" />
		public static float2 max( this float2 a, float2 b ) => math.max( a, b );

		/// <inheritdoc cref="INumber{N}.max" />
		public static float3 max( this float3 a, float3 b ) => math.max( a, b );

		/// <inheritdoc cref="INumber{N}.max" />
		public static float4 max( this float4 a, float4 b ) => math.max( a, b );

		/// <inheritdoc cref="INumber{N}.max" />
		public static double max( this double a, double b ) => math.max( a, b );

		/// <inheritdoc cref="INumber{N}.max" />
		public static double2 max( this double2 a, double2 b ) => math.max( a, b );

		/// <inheritdoc cref="INumber{N}.max" />
		public static double3 max( this double3 a, double3 b ) => math.max( a, b );

		/// <inheritdoc cref="INumber{N}.max" />
		public static double4 max( this double4 a, double4 b ) => math.max( a, b );


		/// <inheritdoc cref="INumber{N}.to" />
		public static T to<T>( T a, T b ) where T : INumber<T> => a.to( b );

		/// <inheritdoc cref="INumber{N}.to" />
		public static inth to( inth a, inth b ) => a.to( b );

		/// <inheritdoc cref="INumber{N}.to" />
		public static rat to( rat a, rat b ) => a.to( b );

		/// <inheritdoc cref="INumber{N}.to" />
		public static inth2 to( inth2 a, inth2 b ) => a.to( b );

		/// <inheritdoc cref="INumber{N}.to" />
		public static rat2 to( rat2 a, rat2 b ) => a.to( b );

		/// <inheritdoc cref="INumber{N}.to" />
		public static int to( this int a, int b ) => b - a;

		/// <inheritdoc cref="INumber{N}.to" />
		public static int2 to( this int2 a, int2 b ) => b - a;

		/// <inheritdoc cref="INumber{N}.to" />
		public static int3 to( this int3 a, int3 b ) => b - a;

		/// <inheritdoc cref="INumber{N}.to" />
		public static int4 to( this int4 a, int4 b ) => b - a;

		/// <inheritdoc cref="INumber{N}.to" />
		public static float to( this float a, float b ) => b - a;

		/// <inheritdoc cref="INumber{N}.to" />
		public static float2 to( this float2 a, float2 b ) => b - a;

		/// <inheritdoc cref="INumber{N}.to" />
		public static float3 to( this float3 a, float3 b ) => b - a;

		/// <inheritdoc cref="INumber{N}.to" />
		public static float4 to( this float4 a, float4 b ) => b - a;

		/// <inheritdoc cref="INumber{N}.to" />
		public static double to( this double a, double b ) => b - a;

		/// <inheritdoc cref="INumber{N}.to" />
		public static double2 to( this double2 a, double2 b ) => b - a;

		/// <inheritdoc cref="INumber{N}.to" />
		public static double3 to( this double3 a, double3 b ) => b - a;

		/// <inheritdoc cref="INumber{N}.to" />
		public static double4 to( this double4 a, double4 b ) => b - a;
	}


}