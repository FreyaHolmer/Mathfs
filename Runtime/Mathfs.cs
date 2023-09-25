// Some of this code is similar to Unity's original Mathf source to match functionality.
// The original Mathf.cs source https://github.com/Unity-Technologies/UnityCsReference/blob/master/Runtime/Export/Math/Mathf.cs
// ...and the trace amounts of it left in here is copyright (c) Unity Technologies with license: https://unity3d.com/legal/licenses/Unity_Reference_Only_License
// 
// Collected and expanded upon to by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using System;
using UnityEngine;
using Uei = UnityEngine.Internal;
using System.Linq; // used for arbitrary count min/max functions, so it's safe and won't allocate garbage don't worry~
using System.Runtime.CompilerServices;

namespace Freya {

	/// <summary>The core math helper class. It has functions mostly for single values, but also vector helpers</summary>
	public static class Mathfs {

		const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

		public static readonly bool[] bools = { false, true };

		#region Constants

		/// <summary>The circle constant. Defined as the circumference of a circle divided by its radius. Equivalent to 2*pi</summary>
		public const float TAU = 6.28318530717959f;

		/// <summary>An obscure circle constant. Defined as the circumference of a circle divided by its diameter. Equivalent to 0.5*tau</summary>
		public const float PI = 3.14159265359f;

		/// <summary>Euler's number. The base of the natural logarithm. f(x)=e^x is equal to its own derivative</summary>
		public const float E = 2.71828182846f;

		/// <summary>The golden ratio. It is the value of a/b where a/b = (a+b)/a. It's the positive root of x^2-x-1</summary>
		public const float GOLDEN_RATIO = 1.61803398875f;

		/// <summary>The square root of two. The length of the vector (1,1)</summary>
		public const float SQRT2 = 1.41421356237f;

		/// <summary>The reciprocal of the square root of two. The components of a normalized (1,1) vector</summary>
		public const float RSQRT2 = 1f / SQRT2;

		/// <summary>Multiply an angle in degrees by this, to convert it to radians</summary>
		public const float Deg2Rad = TAU / 360f;

		/// <summary>Multiply an angle in radians by this, to convert it to degrees</summary>
		public const float Rad2Deg = 360f / TAU;

		#endregion

		#region Math operations

		/// <summary>Returns the square root of the given value</summary>
		[MethodImpl( INLINE )] public static float Sqrt( float value ) => MathF.Sqrt( value );

		/// <summary>Returns the square root of each component</summary>
		[MethodImpl( INLINE )] public static Vector2 Sqrt( Vector2 v ) => new Vector2( Sqrt( v.x ), Sqrt( v.y ) );

		/// <inheritdoc cref="Mathfs.Sqrt(Vector2)"/>
		[MethodImpl( INLINE )] public static Vector3 Sqrt( Vector3 v ) => new Vector3( Sqrt( v.x ), Sqrt( v.y ), Sqrt( v.z ) );

		/// <inheritdoc cref="Mathfs.Sqrt(Vector2)"/>
		[MethodImpl( INLINE )] public static Vector4 Sqrt( Vector4 v ) => new Vector4( Sqrt( v.x ), Sqrt( v.y ), Sqrt( v.z ), Sqrt( v.w ) );

		/// <summary>Returns the cube root of the given value, properly handling negative values unlike Pow(v,1/3)</summary>
		[MethodImpl( INLINE )] public static float Cbrt( float value ) => MathF.Cbrt( value );

		/// <summary>Returns <c>value</c> raised to the power of <c>exponent</c></summary>
		[MethodImpl( INLINE )] public static float Pow( float value, float exponent ) => MathF.Pow( value, exponent );

		/// <summary>Returns e to the power of the given value</summary>
		[MethodImpl( INLINE )] public static float Exp( float power ) => MathF.Exp( power );

		/// <summary>Returns the logarithm of a value, with the given base</summary>
		[MethodImpl( INLINE )] public static float Log( float value, float @base ) => MathF.Log( value, @base );

		/// <summary>Returns the natural logarithm of the given value</summary>
		[MethodImpl( INLINE )] public static float Log( float value ) => MathF.Log( value );

		/// <summary>Returns the base 10 logarithm of the given value</summary>
		[MethodImpl( INLINE )] public static float Log10( float value ) => MathF.Log10( value );

		/// <summary>Returns the binomial coefficient n over k</summary>
		public static ulong BinomialCoef( uint n, uint k ) {
			// source: https://blog.plover.com/math/choose.html
			ulong r = 1;
			if( k > n ) return 0;
			for( ulong d = 1; d <= k; d++ ) {
				r *= n--;
				r /= d;
			}

			return r;
			// mathematically clean but extremely prone to overflow
			//return Factorial( n ) / ( Factorial( k ) * Factorial( n - k ) );
		}

		/// <summary>Returns the Factorial of a given value from 0 to 12</summary>
		/// <param name="value">A value between 0 and 12 (integers can't store the factorial of 13 or above)</param>
		[MethodImpl( INLINE )] public static int Factorial( uint value ) {
			if( value <= 12 )
				return factorialInt[value];
			if( value <= 20 )
				throw new OverflowException( $"The Factorial of {value} is too big for integer representation, please use {nameof(FactorialLong)} instead" );
			throw new OverflowException( $"The Factorial of {value} is too big for integer representation" );
		}

		/// <summary>Returns the Factorial of a given value from 0 to 20</summary>
		/// <param name="value">A value between 0 and 20 (neither long nor ulong can store values large enough for the factorial of 21)</param>
		[MethodImpl( INLINE )] public static long FactorialLong( uint value ) {
			if( value <= 20 )
				return factorialLong[value];
			throw new OverflowException( $"The Factorial of {value} is too big for integer representation, even unsigned longs, soooo, rip" );
		}

		static readonly long[] factorialLong = {
			/*0*/ 1,
			/*1*/ 1,
			/*2*/ 2,
			/*3*/ 6,
			/*4*/ 24,
			/*5*/ 120,
			/*6*/ 720,
			/*7*/ 5040,
			/*8*/ 40320,
			/*9*/ 362880,
			/*10*/ 3628800,
			/*11*/ 39916800,
			/*12*/ 479001600,
			/*13*/ 6227020800,
			/*14*/ 87178291200,
			/*15*/ 1307674368000,
			/*16*/ 20922789888000,
			/*17*/ 355687428096000,
			/*18*/ 6402373705728000,
			/*19*/ 121645100408832000,
			/*20*/ 2432902008176640000
		};

		static readonly int[] factorialInt = {
			/*0*/ 1,
			/*1*/ 1,
			/*2*/ 2,
			/*3*/ 6,
			/*4*/ 24,
			/*5*/ 120,
			/*6*/ 720,
			/*7*/ 5040,
			/*8*/ 40320,
			/*9*/ 362880,
			/*10*/ 3628800,
			/*11*/ 39916800,
			/*12*/ 479001600
		};

		#endregion

		#region Floating point shenanigans

		/// <summary>A very small value, used for various floating point inaccuracy thresholds</summary>
		public static readonly float Epsilon = UnityEngineInternal.MathfInternal.IsFlushToZeroEnabled ? UnityEngineInternal.MathfInternal.FloatMinNormal : UnityEngineInternal.MathfInternal.FloatMinDenormal;

		/// <summary>float.PositiveInfinity</summary>
		public const float Infinity = float.PositiveInfinity;

		/// <summary>float.NegativeInfinity</summary>
		public const float NegativeInfinity = float.NegativeInfinity;

		/// <summary>Returns whether or not two values are approximately equal.
		/// They are considered equal if they are within a <c>Mathfs.Epsilon*8</c> or <c>max(a,b)*0.000001f</c> range of each other</summary>
		/// <param name="a">The first value to compare</param>
		/// <param name="b">The second value to compare</param>
		[MethodImpl( INLINE )] public static bool Approximately( float a, float b ) => Abs( b - a ) < Max( 0.000001f * Max( Abs( a ), Abs( b ) ), Epsilon * 8 );

		/// <inheritdoc cref="Approximately(float,float)"/>
		[MethodImpl( INLINE )] public static bool Approximately( Vector2 a, Vector2 b ) => Approximately( a.x, b.x ) && Approximately( a.y, b.y );

		/// <inheritdoc cref="Approximately(float,float)"/>
		[MethodImpl( INLINE )] public static bool Approximately( Vector3 a, Vector3 b ) => Approximately( a.x, b.x ) && Approximately( a.y, b.y ) && Approximately( a.z, b.z );

		/// <inheritdoc cref="Approximately(float,float)"/>
		[MethodImpl( INLINE )] public static bool Approximately( Vector4 a, Vector4 b ) => Approximately( a.x, b.x ) && Approximately( a.y, b.y ) && Approximately( a.z, b.z ) && Approximately( a.w, b.w );

		/// <inheritdoc cref="Approximately(float,float)"/>
		[MethodImpl( INLINE )] public static bool Approximately( Color a, Color b ) => Approximately( a.r, b.r ) && Approximately( a.g, b.g ) && Approximately( a.b, b.b ) && Approximately( a.a, b.a );

		#endregion

		#region Trigonometry

		/// <summary>Returns the cosine of the given angle. Equivalent to the x-component of a unit vector with the same angle</summary>
		/// <param name="angRad">Angle in radians</param>
		[MethodImpl( INLINE )] public static float Cos( float angRad ) => MathF.Cos( angRad );

		/// <summary>Returns the sine of the given angle. Equivalent to the y-component of a unit vector with the same angle</summary>
		/// <param name="angRad">Angle in radians</param>
		[MethodImpl( INLINE )] public static float Sin( float angRad ) => MathF.Sin( angRad );

		/// <summary>Returns the tangent of the given angle</summary>
		/// <param name="angRad">Angle in radians</param>
		[MethodImpl( INLINE )] public static float Tan( float angRad ) => MathF.Tan( angRad );

		/// <summary>Returns the arc cosine of the given value, in radians</summary>
		/// <param name="value">A value between -1 and 1</param>
		[MethodImpl( INLINE )] public static float Acos( float value ) => MathF.Acos( value );

		/// <summary>Returns the arc sine of the given value, in radians</summary>
		/// <param name="value">A value between -1 and 1</param>
		[MethodImpl( INLINE )] public static float Asin( float value ) => MathF.Asin( value );

		/// <summary>Returns the arc tangent of the given value, in radians</summary>
		/// <param name="value">A value between -1 and 1</param>
		[MethodImpl( INLINE )] public static float Atan( float value ) => MathF.Atan( value );

		/// <summary>Returns the angle of a vector. I don't recommend using this function, it's confusing~ Use Mathfs.DirToAng instead</summary>
		/// <param name="y">The y component of the vector. They're flipped yeah I know but this is how everyone implements if for some godforsaken reason</param>
		/// <param name="x">The x component of the vector. They're flipped yeah I know but this is how everyone implements if for some godforsaken reason</param>
		[MethodImpl( INLINE )] public static float Atan2( float y, float x ) => MathF.Atan2( y, x );

		/// <summary>Returns the cosecant of the given angle</summary>
		/// <param name="angRad">Angle in radians</param>
		[MethodImpl( INLINE )] public static float Csc( float angRad ) => 1f / MathF.Sin( angRad );

		/// <summary>Returns the secant of the given angle</summary>
		/// <param name="angRad">Angle in radians</param>
		[MethodImpl( INLINE )] public static float Sec( float angRad ) => 1f / MathF.Cos( angRad );

		/// <summary>Returns the cotangent of the given angle</summary>
		/// <param name="angRad">Angle in radians</param>
		[MethodImpl( INLINE )] public static float Cot( float angRad ) => 1f / MathF.Tan( angRad );

		/// <summary>Returns the versine of the given angle</summary>
		/// <param name="angRad">Angle in radians</param>
		[MethodImpl( INLINE )] public static float Ver( float angRad ) => 1 - MathF.Cos( angRad );

		/// <summary>Returns the coversine of the given angle</summary>
		/// <param name="angRad">Angle in radians</param>
		[MethodImpl( INLINE )] public static float Cvs( float angRad ) => 1 - MathF.Sin( angRad );

		/// <summary>Returns the chord of the given angle</summary>
		/// <param name="angRad">Angle in radians</param>
		[MethodImpl( INLINE )] public static float Crd( float angRad ) => 2 * MathF.Sin( angRad / 2 );

		const double SINC_W = 0.01;
		const double SINC_P_C2 = -1 / 6.0;
		const double SINC_P_C4 = 1 / 120.0;
		const double SINCRCP_P_C2 = 1 / 6.0;
		const double SINCRCP_P_C4 = 7 / 360.0;

		/// <summary>The unnormalized sinc function sin(x)/x, properly handling the removable singularity around x = 0</summary>
		/// <param name="x">The input value for the Sinc function</param>
		public static float Sinc( float x ) => (float)Sinc( (double)x );

		/// <inheritdoc cref="Sinc(float)"/>
		public static double Sinc( double x ) {
			x = Math.Abs( x ); // sinc is symmetric
			if( x < SINC_W ) {
				// approximate the singularity w. a polynomial
				double x2 = x * x;
				double x4 = x2 * x2;
				return 1 + SINC_P_C2 * x2 + SINC_P_C4 * x4;
			}

			return Math.Sin( x ) / x;
		}

		/// <summary>The unnormalized cosinc or cosc function (1-cos(x))/x, properly handling the removable singularity around x = 0</summary>
		/// <param name="x">The input value for the Cosinc function</param>
		public static float Cosinc( float x ) => (float)Cosinc( (double)x );

		/// <inheritdoc cref="Cosinc(float)"/>
		public static double Cosinc( double x ) {
			if( Math.Abs( x ) < 0.01 )
				return x / 2 - ( x * x * x ) / 24; // approximate the singularity w. a polynomial, based on the taylor series expansion
			return ( 1 - Math.Cos( x ) ) / x;
		}

		/// <summary>The unnormalized reciprocal sinc function x/sin(x), properly handling the removable singularity around x = 0</summary>
		/// <param name="x">The input value for the reciprocal Sinc function</param>
		public static float SincRcp( float x ) => (float)SincRcp( (double)x );

		/// <inheritdoc cref="SincRcp(float)"/>
		public static double SincRcp( double x ) {
			x = Math.Abs( x ); // sinc is symmetric
			if( x < SINC_W ) {
				// approximate the singularity w. a polynomial
				double x2 = x * x;
				double x4 = x2 * x2;
				return 1 + SINCRCP_P_C2 * x2 + SINCRCP_P_C4 * x4;
			}

			return x / Math.Sin( x );
		}

		#endregion

		#region Hyperbolic Trigonometry

		/// <summary>Returns the hyperbolic cosine of the given hyperbolic angle</summary>
		[MethodImpl( INLINE )] public static float Cosh( float x ) => MathF.Cosh( x );

		/// <summary>Returns the hyperbolic sine of the given hyperbolic angle</summary>
		[MethodImpl( INLINE )] public static float Sinh( float x ) => MathF.Sinh( x );

		/// <summary>Returns the hyperbolic tangent of the given hyperbolic angle</summary>
		[MethodImpl( INLINE )] public static float Tanh( float x ) => MathF.Tanh( x );

		/// <summary>Returns the hyperbolic arc cosine of the given value</summary>
		[MethodImpl( INLINE )] public static float Acosh( float x ) => MathF.Acosh( x );

		/// <summary>Returns the hyperbolic arc sine of the given value</summary>
		[MethodImpl( INLINE )] public static float Asinh( float x ) => MathF.Asinh( x );

		/// <summary>Returns the hyperbolic arc tangent of the given value</summary>
		[MethodImpl( INLINE )] public static float Atanh( float x ) => MathF.Atanh( x );

		#endregion

		#region Geometric Algebra

		/// <summary>Returns the wedge product between two vectors</summary>
		public static float Wedge( Vector2 a, Vector2 b ) => a.x * b.y - a.y * b.x;

		/// <summary>Returns the wedge product between two vectors</summary>
		public static Bivector3 Wedge( Vector3 a, Vector3 b ) =>
			new Bivector3(
				a.y * b.z - a.z * b.y,
				a.z * b.x - a.x * b.z,
				a.x * b.y - a.y * b.x
			);

		/// <summary>Returns the geometric product between two vectors</summary>
		public static Rotor3 GeometricProduct( Vector3 a, Vector3 b ) => new Rotor3( Vector3.Dot( a, b ), Wedge( a, b ) );

		#endregion

		#region Absolute Values

		/// <summary>Returns the absolute value. Basically makes negative numbers positive</summary>
		[MethodImpl( INLINE )] public static float Abs( float value ) => Math.Abs( value );

		/// <inheritdoc cref="Mathfs.Abs(float)"/>
		[MethodImpl( INLINE )] public static int Abs( int value ) => Math.Abs( value );

		/// <summary>Returns the absolute value, per component. Basically makes negative numbers positive</summary>
		[MethodImpl( INLINE )] public static Vector2 Abs( Vector2 v ) => new Vector2( Abs( v.x ), Abs( v.y ) );

		/// <inheritdoc cref="Mathfs.Abs(Vector2)"/>
		[MethodImpl( INLINE )] public static Vector3 Abs( Vector3 v ) => new Vector3( Abs( v.x ), Abs( v.y ), Abs( v.z ) );

		/// <inheritdoc cref="Mathfs.Abs(Vector2)"/>
		[MethodImpl( INLINE )] public static Vector4 Abs( Vector4 v ) => new Vector4( Abs( v.x ), Abs( v.y ), Abs( v.z ), Abs( v.w ) );

		#endregion

		#region Clamping

		/// <summary>Returns the value clamped between <c>min</c> and <c>max</c></summary>
		/// <param name="value">The value to clamp</param>
		/// <param name="min">The minimum value</param>
		/// <param name="max">The maximum value</param>
		public static float Clamp( float value, float min, float max ) => value < min ? min : value > max ? max : value;

		/// <summary>Clamps each component between <c>min</c> and <c>max</c></summary>
		public static Vector2 Clamp( Vector2 v, Vector2 min, Vector2 max ) =>
			new Vector2(
				v.x < min.x ? min.x : v.x > max.x ? max.x : v.x,
				v.y < min.y ? min.y : v.y > max.y ? max.y : v.y
			);

		/// <inheritdoc cref="Mathfs.Clamp(Vector2,Vector2,Vector2)"/>
		public static Vector3 Clamp( Vector3 v, Vector3 min, Vector3 max ) =>
			new Vector3(
				v.x < min.x ? min.x : v.x > max.x ? max.x : v.x,
				v.y < min.y ? min.y : v.y > max.y ? max.y : v.y,
				v.z < min.z ? min.z : v.z > max.z ? max.z : v.z
			);

		/// <inheritdoc cref="Mathfs.Clamp(Vector2,Vector2,Vector2)"/>
		public static Vector4 Clamp( Vector4 v, Vector4 min, Vector4 max ) =>
			new Vector4(
				v.x < min.x ? min.x : v.x > max.x ? max.x : v.x,
				v.y < min.y ? min.y : v.y > max.y ? max.y : v.y,
				v.z < min.z ? min.z : v.z > max.z ? max.z : v.z,
				v.w < min.w ? min.w : v.w > max.w ? max.w : v.w
			);

		/// <inheritdoc cref="Mathfs.Clamp(float,float,float)"/>
		public static int Clamp( int value, int min, int max ) => value < min ? min : value > max ? max : value;

		/// <summary>Returns the value clamped between 0 and 1</summary>
		public static float Clamp01( float value ) => value < 0f ? 0f : value > 1f ? 1f : value;

		/// <summary>Clamps each component between 0 and 1</summary>
		public static Vector2 Clamp01( Vector2 v ) =>
			new Vector2(
				v.x < 0f ? 0f : v.x > 1f ? 1f : v.x,
				v.y < 0f ? 0f : v.y > 1f ? 1f : v.y
			);

		/// <inheritdoc cref="Mathfs.Clamp01(Vector2)"/>
		public static Vector3 Clamp01( Vector3 v ) =>
			new Vector3(
				v.x < 0f ? 0f : v.x > 1f ? 1f : v.x,
				v.y < 0f ? 0f : v.y > 1f ? 1f : v.y,
				v.z < 0f ? 0f : v.z > 1f ? 1f : v.z
			);

		/// <inheritdoc cref="Mathfs.Clamp01(Vector2)"/>
		public static Vector4 Clamp01( Vector4 v ) =>
			new Vector4(
				v.x < 0f ? 0f : v.x > 1f ? 1f : v.x,
				v.y < 0f ? 0f : v.y > 1f ? 1f : v.y,
				v.z < 0f ? 0f : v.z > 1f ? 1f : v.z,
				v.w < 0f ? 0f : v.w > 1f ? 1f : v.w
			);

		/// <summary>Clamps the value between -1 and 1</summary>
		public static double ClampNeg1to1( double value ) => value < -1.0 ? -1.0 : value > 1.0 ? 1.0 : value;

		/// <summary>Clamps the value between -1 and 1</summary>
		public static float ClampNeg1to1( float value ) => value < -1f ? -1f : value > 1f ? 1f : value;

		/// <summary>Clamps each component between -1 and 1</summary>
		public static Vector2 ClampNeg1to1( Vector2 v ) =>
			new Vector2(
				v.x < -1f ? -1f : v.x > 1f ? 1f : v.x,
				v.y < -1f ? -1f : v.y > 1f ? 1f : v.y
			);

		/// <summary>Clamps each component between -1 and 1</summary>
		public static Vector3 ClampNeg1to1( Vector3 v ) =>
			new Vector3(
				v.x < -1f ? -1f : v.x > 1f ? 1f : v.x,
				v.y < -1f ? -1f : v.y > 1f ? 1f : v.y,
				v.z < -1f ? -1f : v.z > 1f ? 1f : v.z
			);

		/// <summary>Clamps each component between -1 and 1</summary>
		public static Vector4 ClampNeg1to1( Vector4 v ) =>
			new Vector4(
				v.x < -1f ? -1f : v.x > 1f ? 1f : v.x,
				v.y < -1f ? -1f : v.y > 1f ? 1f : v.y,
				v.z < -1f ? -1f : v.z > 1f ? 1f : v.z,
				v.w < -1f ? -1f : v.w > 1f ? 1f : v.w
			);

		#endregion

		#region Min & Max

		/// <summary>Returns the smallest of the two values</summary>
		[MethodImpl( INLINE )] public static float Min( float a, float b ) => a < b ? a : b;

		/// <summary>Returns the smallest of the three values</summary>
		[MethodImpl( INLINE )] public static float Min( float a, float b, float c ) => Min( Min( a, b ), c );

		/// <summary>Returns the smallest of the four values</summary>
		[MethodImpl( INLINE )] public static float Min( float a, float b, float c, float d ) => Min( Min( a, b ), Min( c, d ) );

		/// <summary>Returns the largest of the two values</summary>
		[MethodImpl( INLINE )] public static float Max( float a, float b ) => a > b ? a : b;

		/// <summary>Returns the largest of the three values</summary>
		[MethodImpl( INLINE )] public static float Max( float a, float b, float c ) => Max( Max( a, b ), c );

		/// <summary>Returns the largest of the four values</summary>
		[MethodImpl( INLINE )] public static float Max( float a, float b, float c, float d ) => Max( Max( a, b ), Max( c, d ) );

		/// <summary>Returns the smallest of the two values</summary>
		[MethodImpl( INLINE )] public static int Min( int a, int b ) => a < b ? a : b;

		/// <summary>Returns the smallest of the three values</summary>
		[MethodImpl( INLINE )] public static int Min( int a, int b, int c ) => Min( Min( a, b ), c );

		/// <summary>Returns the smallest of the four values</summary>
		[MethodImpl( INLINE )] public static int Min( int a, int b, int c, int d ) => Min( Min( a, b ), Min( c, d ) );

		/// <summary>Returns the largest of the two values</summary>
		[MethodImpl( INLINE )] public static int Max( int a, int b ) => a > b ? a : b;

		/// <summary>Returns the largest of the three values</summary>
		[MethodImpl( INLINE )] public static int Max( int a, int b, int c ) => Max( Max( a, b ), c );

		/// <summary>Returns the largest of the four values</summary>
		[MethodImpl( INLINE )] public static int Max( int a, int b, int c, int d ) => Max( Max( a, b ), Max( c, d ) );

		/// <summary>Returns the smallest of the given values</summary>
		[MethodImpl( INLINE )] public static float Min( params float[] values ) => values.Min();

		/// <summary>Returns the largest of the given values</summary>
		[MethodImpl( INLINE )] public static float Max( params float[] values ) => values.Max();

		/// <summary>Returns the smallest of the given values</summary>
		[MethodImpl( INLINE )] public static int Min( params int[] values ) => values.Min();

		/// <summary>Returns the largest of the given values</summary>
		[MethodImpl( INLINE )] public static int Max( params int[] values ) => values.Max();

		/// <summary>Returns the minimum value of all components in the vector</summary>
		[MethodImpl( INLINE )] public static float Min( Vector2 v ) => Min( v.x, v.y );

		/// <inheritdoc cref="Mathfs.Min(Vector2)"/>
		[MethodImpl( INLINE )] public static float Min( Vector3 v ) => Min( v.x, v.y, v.z );

		/// <inheritdoc cref="Mathfs.Min(Vector2)"/>
		[MethodImpl( INLINE )] public static float Min( Vector4 v ) => Min( v.x, v.y, v.z, v.w );

		/// <summary>Returns the maximum value of all components in the vector</summary>
		[MethodImpl( INLINE )] public static float Max( Vector2 v ) => Max( v.x, v.y );

		/// <inheritdoc cref="Mathfs.Max(Vector2)"/>
		[MethodImpl( INLINE )] public static float Max( Vector3 v ) => Max( v.x, v.y, v.z );

		/// <inheritdoc cref="Mathfs.Max(Vector2)"/>
		[MethodImpl( INLINE )] public static float Max( Vector4 v ) => Max( v.x, v.y, v.z, v.w );

		#endregion

		#region Signs & Rounding

		/// <summary>The sign of the value. Returns -1 if negative, returns 1 if greater than or equal to 0</summary>
		[MethodImpl( INLINE )] public static float Sign( float value ) => value >= 0f ? 1 : -1;

		/// <summary>The sign of each component. Returns -1 if negative, returns 1 if greater than or equal to 0</summary>
		[MethodImpl( INLINE )] public static Vector2 Sign( Vector2 value ) => new Vector2( value.x >= 0f ? 1 : -1, value.y >= 0f ? 1 : -1 );

		/// <inheritdoc cref="Mathfs.Sign(Vector2)"/>
		[MethodImpl( INLINE )] public static Vector3 Sign( Vector3 value ) => new Vector3( value.x >= 0f ? 1 : -1, value.y >= 0f ? 1 : -1, value.z >= 0f ? 1 : -1 );

		/// <inheritdoc cref="Mathfs.Sign(Vector2)"/>
		[MethodImpl( INLINE )] public static Vector4 Sign( Vector4 value ) => new Vector4( value.x >= 0f ? 1 : -1, value.y >= 0f ? 1 : -1, value.z >= 0f ? 1 : -1, value.w >= 0f ? 1 : -1 );

		/// <summary>Returns the sign of the value, either -1 if negative, or 1 if positive or 0</summary>
		[MethodImpl( INLINE )] public static int Sign( int value ) => value >= 0 ? 1 : -1;

		/// <summary>The sign of the value as an integer. Returns -1 if negative, returns 1 if greater than or equal to 0</summary>
		[MethodImpl( INLINE )] public static int SignAsInt( float value ) => value >= 0f ? 1 : -1;

		/// <summary>The sign of the value. Returns -1 if negative, return 0 if zero (or within the given threshold), returns 1 if positive</summary>
		[MethodImpl( INLINE )] public static float SignWithZero( float value, float zeroThreshold = 0.000001f ) => Abs( value ) < zeroThreshold ? 0 : Sign( value );

		/// <summary>The sign of each component. Returns -1 if negative, return 0 if zero (or within the given threshold), returns 1 if positive</summary>
		[MethodImpl( INLINE )] public static Vector2 SignWithZero( Vector2 value, float zeroThreshold = 0.000001f ) =>
			new Vector2(
				Abs( value.x ) < zeroThreshold ? 0 : Sign( value.x ),
				Abs( value.y ) < zeroThreshold ? 0 : Sign( value.y )
			);

		/// <inheritdoc cref="Mathfs.SignWithZero(Vector2,float)"/>
		[MethodImpl( INLINE )] public static Vector3 SignWithZero( Vector3 value, float zeroThreshold = 0.000001f ) =>
			new Vector3(
				Abs( value.x ) < zeroThreshold ? 0 : Sign( value.x ),
				Abs( value.y ) < zeroThreshold ? 0 : Sign( value.y ),
				Abs( value.z ) < zeroThreshold ? 0 : Sign( value.z )
			);

		/// <inheritdoc cref="Mathfs.SignWithZero(Vector2,float)"/>
		[MethodImpl( INLINE )] public static Vector4 SignWithZero( Vector4 value, float zeroThreshold = 0.000001f ) =>
			new Vector4(
				Abs( value.x ) < zeroThreshold ? 0 : Sign( value.x ),
				Abs( value.y ) < zeroThreshold ? 0 : Sign( value.y ),
				Abs( value.z ) < zeroThreshold ? 0 : Sign( value.z ),
				Abs( value.w ) < zeroThreshold ? 0 : Sign( value.w )
			);

		/// <summary>Returns the sign of the value, either -1 if negative, 0 if zero, 1 if positive</summary>
		[MethodImpl( INLINE )] public static int SignWithZero( int value ) => value == 0 ? 0 : Sign( value );

		/// <summary>The sign of the value. Returns -1 if negative, return 0 if zero (or within the given threshold), returns 1 if positive</summary>
		[MethodImpl( INLINE )] public static int SignWithZeroAsInt( float value, float zeroThreshold = 0.000001f ) => Abs( value ) < zeroThreshold ? 0 : SignAsInt( value );

		/// <summary>Rounds the value down to the nearest integer</summary>
		[MethodImpl( INLINE )] public static float Floor( float value ) => MathF.Floor( value );

		/// <summary>Rounds the vector components down to the nearest integer</summary>
		[MethodImpl( INLINE )] public static Vector2 Floor( Vector2 value ) => new Vector2( MathF.Floor( value.x ), MathF.Floor( value.y ) );

		/// <inheritdoc cref="Mathfs.Floor(Vector2)"/>
		[MethodImpl( INLINE )] public static Vector3 Floor( Vector3 value ) => new Vector3( MathF.Floor( value.x ), MathF.Floor( value.y ), MathF.Floor( value.z ) );

		/// <inheritdoc cref="Mathfs.Floor(Vector2)"/>
		[MethodImpl( INLINE )] public static Vector4 Floor( Vector4 value ) => new Vector4( MathF.Floor( value.x ), MathF.Floor( value.y ), MathF.Floor( value.z ), MathF.Floor( value.w ) );

		/// <summary>Rounds the value down to the nearest integer, returning an int value</summary>
		[MethodImpl( INLINE )] public static int FloorToInt( float value ) => (int)Math.Floor( value );

		/// <summary>Rounds the vector components down to the nearest integer, returning an integer vector</summary>
		[MethodImpl( INLINE )] public static Vector2Int FloorToInt( Vector2 value ) => new Vector2Int( (int)Math.Floor( value.x ), (int)Math.Floor( value.y ) );

		/// <inheritdoc cref="Mathfs.FloorToInt(Vector2)"/>
		[MethodImpl( INLINE )] public static Vector3Int FloorToInt( Vector3 value ) => new Vector3Int( (int)Math.Floor( value.x ), (int)Math.Floor( value.y ), (int)Math.Floor( value.z ) );

		/// <summary>Rounds the value up to the nearest integer</summary>
		[MethodImpl( INLINE )] public static float Ceil( float value ) => MathF.Ceiling( value );

		/// <summary>Rounds the vector components up to the nearest integer</summary>
		[MethodImpl( INLINE )] public static Vector2 Ceil( Vector2 value ) => new Vector2( MathF.Ceiling( value.x ), MathF.Ceiling( value.y ) );

		/// <inheritdoc cref="Mathfs.Ceil(Vector2)"/>
		[MethodImpl( INLINE )] public static Vector3 Ceil( Vector3 value ) => new Vector3( MathF.Ceiling( value.x ), MathF.Ceiling( value.y ), MathF.Ceiling( value.z ) );

		/// <inheritdoc cref="Mathfs.Ceil(Vector2)"/>
		[MethodImpl( INLINE )] public static Vector4 Ceil( Vector4 value ) => new Vector4( MathF.Ceiling( value.x ), MathF.Ceiling( value.y ), MathF.Ceiling( value.z ), MathF.Ceiling( value.w ) );

		/// <summary>Rounds the value up to the nearest integer, returning an int value</summary>
		[MethodImpl( INLINE )] public static int CeilToInt( float value ) => (int)Math.Ceiling( value );

		/// <summary>Rounds the vector components up to the nearest integer, returning an integer vector</summary>
		[MethodImpl( INLINE )] public static Vector2Int CeilToInt( Vector2 value ) => new Vector2Int( (int)Math.Ceiling( value.x ), (int)Math.Ceiling( value.y ) );

		/// <inheritdoc cref="Mathfs.CeilToInt(Vector2)"/>
		[MethodImpl( INLINE )] public static Vector3Int CeilToInt( Vector3 value ) => new Vector3Int( (int)Math.Ceiling( value.x ), (int)Math.Ceiling( value.y ), (int)Math.Ceiling( value.z ) );

		/// <summary>Rounds the value to the nearest integer</summary>
		[MethodImpl( INLINE )] public static float Round( float value, MidpointRounding midpointRounding = MidpointRounding.ToEven ) => (float)MathF.Round( value, midpointRounding );

		/// <summary>Rounds the vector components to the nearest integer</summary>
		[MethodImpl( INLINE )] public static Vector2 Round( Vector2 value, MidpointRounding midpointRounding = MidpointRounding.ToEven ) => new Vector2( MathF.Round( value.x, midpointRounding ), MathF.Round( value.y, midpointRounding ) );

		/// <inheritdoc cref="Mathfs.Round(Vector2,MidpointRounding)"/>
		[MethodImpl( INLINE )] public static Vector3 Round( Vector3 value, MidpointRounding midpointRounding = MidpointRounding.ToEven ) => new Vector3( MathF.Round( value.x, midpointRounding ), MathF.Round( value.y, midpointRounding ), MathF.Round( value.z, midpointRounding ) );

		/// <inheritdoc cref="Mathfs.Round(Vector2,MidpointRounding)"/>
		[MethodImpl( INLINE )] public static Vector4 Round( Vector4 value, MidpointRounding midpointRounding = MidpointRounding.ToEven ) => new Vector4( MathF.Round( value.x, midpointRounding ), MathF.Round( value.y, midpointRounding ), MathF.Round( value.z, midpointRounding ), MathF.Round( value.w, midpointRounding ) );

		/// <summary>Rounds the value to the nearest value, snapped to the given interval size</summary>
		[MethodImpl( INLINE )] public static float Round( float value, float snapInterval, MidpointRounding midpointRounding = MidpointRounding.ToEven ) => MathF.Round( value / snapInterval, midpointRounding ) * snapInterval;

		/// <summary>Rounds the vector components to the nearest value, snapped to the given interval size</summary>
		[MethodImpl( INLINE )] public static Vector2 Round( Vector2 value, float snapInterval, MidpointRounding midpointRounding = MidpointRounding.ToEven ) => new Vector2( Round( value.x, snapInterval, midpointRounding ), Round( value.y, snapInterval, midpointRounding ) );

		/// <inheritdoc cref="Mathfs.Round(Vector2,float,MidpointRounding)"/>
		[MethodImpl( INLINE )] public static Vector3 Round( Vector3 value, float snapInterval, MidpointRounding midpointRounding = MidpointRounding.ToEven ) => new Vector3( Round( value.x, snapInterval, midpointRounding ), Round( value.y, snapInterval, midpointRounding ), Round( value.z, snapInterval, midpointRounding ) );

		/// <inheritdoc cref="Mathfs.Round(Vector2,float,MidpointRounding)"/>
		[MethodImpl( INLINE )] public static Vector4 Round( Vector4 value, float snapInterval, MidpointRounding midpointRounding = MidpointRounding.ToEven ) => new Vector4( Round( value.x, snapInterval, midpointRounding ), Round( value.y, snapInterval, midpointRounding ), Round( value.z, snapInterval, midpointRounding ), Round( value.w, snapInterval, midpointRounding ) );

		/// <summary>Rounds the value to the nearest integer, returning an int value</summary>
		[MethodImpl( INLINE )] public static int RoundToInt( float value, MidpointRounding midpointRounding = MidpointRounding.ToEven ) => (int)Math.Round( value, midpointRounding );

		/// <summary>Rounds the vector components to the nearest integer, returning an integer vector</summary>
		[MethodImpl( INLINE )] public static Vector2Int RoundToInt( Vector2 value, MidpointRounding midpointRounding = MidpointRounding.ToEven ) => new Vector2Int( (int)Math.Round( value.x, midpointRounding ), (int)Math.Round( value.y, midpointRounding ) );

		/// <inheritdoc cref="Mathfs.RoundToInt(Vector2,MidpointRounding)"/>
		[MethodImpl( INLINE )] public static Vector3Int RoundToInt( Vector3 value, MidpointRounding midpointRounding = MidpointRounding.ToEven ) => new Vector3Int( (int)Math.Round( value.x, midpointRounding ), (int)Math.Round( value.y, midpointRounding ), (int)Math.Round( value.z, midpointRounding ) );

		#endregion

		#region Range Repeating

		/// <summary>Returns the fractional part of the value. Equivalent to <c>x - floor(x)</c></summary>
		[MethodImpl( INLINE )] public static float Frac( float x ) => x - Floor( x );

		/// <summary>Returns the fractional part of the value for each component. Equivalent to <c>v - floor(v)</c></summary>
		[MethodImpl( INLINE )] public static Vector2 Frac( Vector2 v ) => new Vector2( v.x - Floor( v.x ), v.y - Floor( v.y ) );

		/// <inheritdoc cref="Mathfs.Frac(Vector2)"/>
		[MethodImpl( INLINE )] public static Vector3 Frac( Vector3 v ) => new Vector3( v.x - Floor( v.x ), v.y - Floor( v.y ), v.z - Floor( v.z ) );

		/// <inheritdoc cref="Mathfs.Frac(Vector2)"/>
		[MethodImpl( INLINE )] public static Vector4 Frac( Vector4 v ) => new Vector4( v.x - Floor( v.x ), v.y - Floor( v.y ), v.z - Floor( v.z ), v.w - Floor( v.w ) );

		/// <summary>Repeats the given value in the interval specified by length</summary>
		[MethodImpl( INLINE )] public static float Repeat( float value, float length ) => Clamp( value - Floor( value / length ) * length, 0.0f, length );

		/// <summary>Modulo, but, behaves the way you want with negative values, for stuff like array[(n+1)%length] etc.</summary>
		[MethodImpl( INLINE )] public static int Mod( int value, int length ) => value >= 0 ? value % length : ( value % length + length ) % length;

		/// <summary>Repeats a value within a range, going back and forth</summary>
		[MethodImpl( INLINE )] public static float PingPong( float t, float length ) => length - Abs( Repeat( t, length * 2f ) - length );

		/// <summary>Returns the height of in a triangle wave at time <c>t</c> going from <c>0</c> to <c>1</c> and back to <c>0</c> within the the given <c>period</c></summary>
		[MethodImpl( INLINE )] public static float TriangleWave( float t, float period = 1f ) {
			float x = t / period;
			return 1f - Abs( 2 * ( x - Floor( x ) ) - 1 );
		}

		/// <summary>Returns the greatest common divisor of the two numbers</summary>
		public static int Gcd( int a, int b ) {
			// special case bc we can't negate int.MinValue
			if( a == int.MinValue || b == int.MinValue ) {
				if( a == int.MinValue && b == int.MinValue )
					return int.MinValue; // the only negative return value, bc we can't negate this number
				int v = Max( a, b ).Abs();
				return v & -v;
			}

			if( a == b )
				return a.Abs();
			( a, b ) = ( Abs( a ), Abs( b ) );
			while( a != 0 && b != 0 )
				_ = a > b ? a %= b : b %= a;
			return a | b;
		}

		#endregion

		#region Smoothing & Easing Curves

		/// <summary>Applies cubic smoothing to the 0-1 interval, also known as the smoothstep function. Similar to an EaseInOut operation</summary>
		[MethodImpl( INLINE )] public static float Smooth01( float x ) => x * x * ( 3 - 2 * x );

		/// <summary>Applies quintic smoothing to the 0-1 interval, also known as the smootherstep function. Similar to an EaseInOut operation</summary>
		[MethodImpl( INLINE )] public static float Smoother01( float x ) => x * x * x * ( x * ( x * 6 - 15 ) + 10 );

		/// <summary>Applies trigonometric smoothing to the 0-1 interval. Similar to an EaseInOut operation</summary>
		[MethodImpl( INLINE )] public static float SmoothCos01( float x ) => Cos( x * PI ) * -0.5f + 0.5f;

		/// <summary>Applies a gamma curve or something idk I've never used this function before but it was part of Unity's original Mathfs.cs and it's undocumented</summary>
		public static float Gamma( float value, float absmax, float gamma ) {
			bool negative = value < 0F;
			float absval = Abs( value );
			if( absval > absmax )
				return negative ? -absval : absval;

			float result = Pow( absval / absmax, gamma ) * absmax;
			return negative ? -result : result;
		}

		#endregion

		#region Value & Vector interpolation

		/// <summary>Blends between a and b, based on the t-value. When t = 0 it returns a, when t = 1 it returns b, and any values between are blended linearly </summary>
		/// <param name="a">The start value, when t is 0</param>
		/// <param name="b">The start value, when t is 1</param>
		/// <param name="t">The t-value from 0 to 1 representing position along the lerp</param>
		[MethodImpl( INLINE )] public static float Lerp( float a, float b, float t ) => ( 1f - t ) * a + t * b;

		/// <summary>Blends between a and b of each component, based on the t-value of each component in the t-vector. When t = 0 it returns a, when t = 1 it returns b, and any values between are blended linearly </summary>
		/// <param name="a">The start value, when t is 0</param>
		/// <param name="b">The start value, when t is 1</param>
		/// <param name="t">The t-values from 0 to 1 representing position along the lerp</param>
		[MethodImpl( INLINE )] public static Vector2 Lerp( Vector2 a, Vector2 b, Vector2 t ) => new Vector2( Lerp( a.x, b.x, t.x ), Lerp( a.y, b.y, t.y ) );

		/// <inheritdoc cref="Mathfs.Lerp(Vector2,Vector2,Vector2)"/>
		[MethodImpl( INLINE )] public static Vector3 Lerp( Vector3 a, Vector3 b, Vector3 t ) => new Vector3( Lerp( a.x, b.x, t.x ), Lerp( a.y, b.y, t.y ), Lerp( a.z, b.z, t.z ) );

		/// <inheritdoc cref="Mathfs.Lerp(Vector2,Vector2,Vector2)"/>
		[MethodImpl( INLINE )] public static Vector4 Lerp( Vector4 a, Vector4 b, Vector4 t ) => new Vector4( Lerp( a.x, b.x, t.x ), Lerp( a.y, b.y, t.y ), Lerp( a.z, b.z, t.z ), Lerp( a.w, b.w, t.w ) );

		/// <summary>Linearly blends between two rectangles, moving and resizing from the center. Note: this lerp is unclamped</summary>
		/// <param name="a">The start value, when t is 0</param>
		/// <param name="b">The start value, when t is 1</param>
		/// <param name="t">The t-values from 0 to 1 representing position along the lerp</param>
		public static Rect Lerp( Rect a, Rect b, float t ) {
			Vector2 center = Vector2.LerpUnclamped( a.center, b.center, t );
			Vector2 size = Vector2.LerpUnclamped( a.size, b.size, t );
			return new Rect( default, size ) { center = center };
		}

		/// <summary>Blends between a and b, based on the t-value. When t = 0 it returns a, when t = 1 it returns b, and any values between are blended linearly</summary>
		/// <param name="a">The start value, when t is 0</param>
		/// <param name="b">The start value, when t is 1</param>
		/// <param name="t">The t-value from 0 to 1 representing position along the lerp, clamped between 0 and 1</param>
		[MethodImpl( INLINE )] public static float LerpClamped( float a, float b, float t ) => Lerp( a, b, Clamp01( t ) );

		/// <summary>Lerps between a and b, applying cubic smoothing to the t-value</summary>
		/// <param name="a">The start value, when t is 0</param>
		/// <param name="b">The start value, when t is 1</param>
		/// <param name="t">The t-value from 0 to 1 representing position along the lerp, clamped between 0 and 1</param>
		[MethodImpl( INLINE )] public static float LerpSmooth( float a, float b, float t ) => Lerp( a, b, Smooth01( Clamp01( t ) ) );

		/// <summary>Given a value between a and b, returns its normalized location in that range, as a t-value (interpolant) from 0 to 1</summary>
		/// <param name="a">The start of the range, where it would return 0</param>
		/// <param name="b">The end of the range, where it would return 1</param>
		/// <param name="value">A value between a and b. Note: values outside this range are still valid, and will be extrapolated</param>
		[MethodImpl( INLINE )] public static float InverseLerp( float a, float b, float value ) => ( value - a ) / ( b - a );

		/// <summary>Given a value between a and b, returns its normalized location in that range, as a t-value (interpolant) from 0 to 1.
		/// This safe version returns 0 if a == b, instead of a division by zero</summary>
		/// <param name="a">The start of the range, where it would return 0</param>
		/// <param name="b">The end of the range, where it would return 1</param>
		/// <param name="value">A value between a and b. Note: values outside this range are still valid, and will be extrapolated</param>
		[MethodImpl( INLINE )] public static float InverseLerpSafe( float a, float b, float value ) {
			float den = b - a;
			if( den == 0 )
				return 0;
			return ( value - a ) / den;
		}

		/// <summary>Given values between a and b in each component, returns their normalized locations in the given ranges, as t-values (interpolants) from 0 to 1</summary>
		/// <param name="a">The start of the ranges, where it would return 0</param>
		/// <param name="b">The end of the ranges, where it would return 1</param>
		/// <param name="v">A value between a and b. Note: values outside this range are still valid, and will be extrapolated</param>
		[MethodImpl( INLINE )] public static Vector2 InverseLerp( Vector2 a, Vector2 b, Vector2 v ) => new Vector2( ( v.x - a.x ) / ( b.x - a.x ), ( v.y - a.y ) / ( b.y - a.y ) );

		/// <inheritdoc cref="Mathfs.InverseLerp(Vector2,Vector2,Vector2)"/>
		[MethodImpl( INLINE )] public static Vector3 InverseLerp( Vector3 a, Vector3 b, Vector3 v ) => new Vector3( ( v.x - a.x ) / ( b.x - a.x ), ( v.y - a.y ) / ( b.y - a.y ), ( v.z - a.z ) / ( b.z - a.z ) );

		/// <inheritdoc cref="Mathfs.InverseLerp(Vector2,Vector2,Vector2)"/>
		[MethodImpl( INLINE )] public static Vector4 InverseLerp( Vector4 a, Vector4 b, Vector4 v ) => new Vector4( ( v.x - a.x ) / ( b.x - a.x ), ( v.y - a.y ) / ( b.y - a.y ), ( v.z - a.z ) / ( b.z - a.z ), ( v.w - a.w ) / ( b.w - a.w ) );

		/// <summary>Given a value between a and b, returns its normalized location in that range, as a t-value (interpolant) clamped between 0 and 1</summary>
		/// <param name="a">The start of the range, where it would return 0</param>
		/// <param name="b">The end of the range, where it would return 1</param>
		/// <param name="value">A value between a and b</param>
		[MethodImpl( INLINE )] public static float InverseLerpClamped( float a, float b, float value ) => Clamp01( ( value - a ) / ( b - a ) );

		/// <summary>Given a value between a and b, returns its normalized location in that range, as a t-value (interpolant) from 0 to 1, with cubic smoothing applied.
		/// Equivalent to "smoothstep" in shader code</summary>
		/// <param name="a">The start of the range, where it would return 0</param>
		/// <param name="b">The end of the range, where it would return 1</param>
		/// <param name="value">A value between a and b. Note: values outside this range are still valid, and will be extrapolated</param>
		[MethodImpl( INLINE )] public static float InverseLerpSmooth( float a, float b, float value ) => Smooth01( Clamp01( ( value - a ) / ( b - a ) ) );

		/// <summary>Remaps a value from the input range [iMin to iMax] into the output range [oMin to oMax].
		/// Equivalent to Lerp(oMin,oMax,InverseLerp(iMin,iMax,value))</summary>
		/// <param name="iMin">The start value of the input range</param>
		/// <param name="iMax">The end value of the input range</param>
		/// <param name="oMin">The start value of the output range</param>
		/// <param name="oMax">The end value of the output range</param>
		/// <param name="value">The value to remap</param>
		[MethodImpl( INLINE )] public static float Remap( float iMin, float iMax, float oMin, float oMax, float value ) => Lerp( oMin, oMax, InverseLerp( iMin, iMax, value ) );

		/// <inheritdoc cref="Mathfs.Remap(float,float,float,float,float)"/>
		[MethodImpl( INLINE )] public static float Remap( float iMin, float iMax, float oMin, float oMax, int value ) => Lerp( oMin, oMax, InverseLerp( iMin, iMax, value ) );

		/// <summary>Remaps values from the input range [iMin to iMax] into the output range [oMin to oMax] on a per-component basis.
		/// Equivalent to Lerp(oMin,oMax,InverseLerp(iMin,iMax,value))</summary>
		/// <param name="iMin">The start values of the input ranges</param>
		/// <param name="iMax">The end values of the input ranges</param>
		/// <param name="oMin">The start values of the output ranges</param>
		/// <param name="oMax">The end values of the output ranges</param>
		/// <param name="value">The values to remap</param>
		[MethodImpl( INLINE )] public static Vector2 Remap( Vector2 iMin, Vector2 iMax, Vector2 oMin, Vector2 oMax, Vector2 value ) => Lerp( oMin, oMax, InverseLerp( iMin, iMax, value ) );

		/// <inheritdoc cref="Mathfs.Remap(Vector2,Vector2,Vector2,Vector2,Vector2)"/>
		[MethodImpl( INLINE )] public static Vector3 Remap( Vector3 iMin, Vector3 iMax, Vector3 oMin, Vector3 oMax, Vector3 value ) => Lerp( oMin, oMax, InverseLerp( iMin, iMax, value ) );

		/// <inheritdoc cref="Mathfs.Remap(Vector2,Vector2,Vector2,Vector2,Vector2)"/>
		[MethodImpl( INLINE )] public static Vector4 Remap( Vector4 iMin, Vector4 iMax, Vector4 oMin, Vector4 oMax, Vector4 value ) => Lerp( oMin, oMax, InverseLerp( iMin, iMax, value ) );

		/// <summary>Remaps a value from the input range [iMin to iMax] into the output range [oMin to oMax], clamping to make sure it does not extrapolate.
		/// Equivalent to Lerp(oMin,oMax,InverseLerpClamped(iMin,iMax,value))</summary>
		/// <param name="iMin">The start value of the input range</param>
		/// <param name="iMax">The end value of the input range</param>
		/// <param name="oMin">The start value of the output range</param>
		/// <param name="oMax">The end value of the output range</param>
		/// <param name="value">The value to remap</param>
		[MethodImpl( INLINE )] public static float RemapClamped( float iMin, float iMax, float oMin, float oMax, float value ) => Lerp( oMin, oMax, InverseLerpClamped( iMin, iMax, value ) );

		/// <summary>Remaps a value from the input Rect to the output Rect</summary>
		/// <param name="iRect">The input Rect</param>
		/// <param name="oRect">The output Rect</param>
		/// <param name="iPos">The input position in the input Rect space</param>
		[MethodImpl( INLINE )] public static Vector2 Remap( Rect iRect, Rect oRect, Vector2 iPos ) => Remap( iRect.min, iRect.max, oRect.min, oRect.max, iPos );

		/// <summary>Remaps a value from the input Bounds to the output Bounds</summary>
		/// <param name="iBounds">The input Bounds</param>
		/// <param name="oBounds">The output Bounds</param>
		/// <param name="iPos">The input position in the input Bounds space</param>
		[MethodImpl( INLINE )] public static Vector3 Remap( Bounds iBounds, Bounds oBounds, Vector3 iPos ) => Remap( iBounds.min, iBounds.max, oBounds.min, oBounds.max, iPos );

		/// <summary>Remaps a value from the input range to the output range</summary>
		/// <param name="inRange">The input range</param>
		/// <param name="outRange">The output range</param>
		/// <param name="value">The value to remap from the input range</param>
		[MethodImpl( INLINE )] public static float Remap( FloatRange inRange, FloatRange outRange, float value ) => Remap( inRange.a, inRange.b, outRange.a, outRange.b, value );

		/// <summary>Remaps a value from the input range to the output range, clamping to make sure it does not extrapolate.</summary>
		/// <param name="inRange">The input range</param>
		/// <param name="outRange">The output range</param>
		/// <param name="value">The value to remap from the input range</param>
		[MethodImpl( INLINE )] public static float RemapClamped( FloatRange inRange, FloatRange outRange, float value ) => RemapClamped( inRange.a, inRange.b, outRange.a, outRange.b, value );

		/// <summary>Exponential interpolation, the multiplicative version of lerp, useful for values such as scaling or zooming</summary>
		/// <param name="a">The start value</param>
		/// <param name="b">The end value</param>
		/// <param name="t">The t-value from 0 to 1 representing position along the eerp</param>
		[MethodImpl( INLINE )] public static float Eerp( float a, float b, float t ) =>
			t switch {
				0f => a,
				1f => b,
				_  => MathF.Pow( a, 1 - t ) * MathF.Pow( b, t )
			};

		/// <summary>Inverse exponential interpolation, the multiplicative version of InverseLerp, useful for values such as scaling or zooming</summary>
		/// <param name="a">The start value</param>
		/// <param name="b">The end value</param>
		/// <param name="v">A value between a and b. Note: values outside this range are still valid, and will be extrapolated</param>
		[MethodImpl( INLINE )] public static float InverseEerp( float a, float b, float v ) => MathF.Log( a / v ) / MathF.Log( a / b );

		#endregion

		#region Movement helpers

		/// <summary>Moves a value <c>current</c> towards <c>target</c></summary>
		/// <param name="current">The current value</param>
		/// <param name="target">The value to move towards</param>
		/// <param name="maxDelta">The maximum change that should be applied to the value</param>
		public static float MoveTowards( float current, float target, float maxDelta ) {
			if( MathF.Abs( target - current ) <= maxDelta )
				return target;
			return current + MathF.Sign( target - current ) * maxDelta;
		}

		/// <summary>Gradually changes a value towards a desired goal over time.
		/// The value is smoothed by some spring-damper like function, which will never overshoot.
		/// The function can be used to smooth any kind of value, positions, colors, scalars</summary>
		/// <param name="current">The current position</param>
		/// <param name="target">The position we are trying to reach</param>
		/// <param name="currentVelocity">The current velocity, this value is modified by the function every time you call it</param>
		/// <param name="smoothTime">Approximately the time it will take to reach the target. A smaller value will reach the target faster</param>
		/// <param name="maxSpeed">	Optionally allows you to clamp the maximum speed</param>
		public static float SmoothDamp( float current, float target, ref float currentVelocity, float smoothTime, float maxSpeed = Infinity ) {
			float deltaTime = Time.deltaTime;
			return SmoothDamp( current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime );
		}

		/// <summary>Gradually changes a value towards a desired goal over time.
		/// The value is smoothed by some spring-damper like function, which will never overshoot.
		/// The function can be used to smooth any kind of value, positions, colors, scalars</summary>
		/// <param name="current">The current position</param>
		/// <param name="target">The position we are trying to reach</param>
		/// <param name="currentVelocity">The current velocity, this value is modified by the function every time you call it</param>
		/// <param name="smoothTime">Approximately the time it will take to reach the target. A smaller value will reach the target faster</param>
		/// <param name="maxSpeed">	Optionally allows you to clamp the maximum speed</param>
		/// <param name="deltaTime">The time since the last call to this function. By default Time.deltaTime</param>
		public static float SmoothDamp( float current, float target, ref float currentVelocity, float smoothTime, [Uei.DefaultValue( "Mathf.Infinity" )] float maxSpeed, [Uei.DefaultValue( "Time.deltaTime" )] float deltaTime ) {
			// Based on Game Programming Gems 4 Chapter 1.10
			smoothTime = MathF.Max( 0.0001F, smoothTime );
			float omega = 2F / smoothTime;

			float x = omega * deltaTime;
			float exp = 1F / ( 1F + x + 0.48F * x * x + 0.235F * x * x * x );
			float change = current - target;
			float originalTo = target;

			// Clamp maximum speed
			float maxChange = maxSpeed * smoothTime;
			change = Clamp( change, -maxChange, maxChange );
			target = current - change;

			float temp = ( currentVelocity + omega * change ) * deltaTime;
			currentVelocity = ( currentVelocity - omega * temp ) * exp;
			float output = target + ( change + temp ) * exp;

			// Prevent overshooting
			if( originalTo - current > 0.0F == output > originalTo ) {
				output = originalTo;
				currentVelocity = ( output - originalTo ) / deltaTime;
			}

			return output;
		}

		#endregion

		#region Weighted sums

		/// <summary>Multiplies each component of <c>w</c> by the input values, and returns their sum</summary>
		/// <param name="w">The weights (per component) to apply to the rest of the values</param>
		/// <param name="a">The first value, weighted by <c>w.x</c></param>
		/// <param name="b">The second value, weighted by <c>w.y</c></param>
		[MethodImpl( INLINE )] public static float WeightedSum( Vector2 w, float a, float b ) => a * w.x + b * w.y;

		/// <summary>Multiplies each component of <c>w</c> by the input values, and returns their sum</summary>
		/// <param name="w">The weights (per component) to apply to the rest of the values</param>
		/// <param name="a">The first value, weighted by <c>w.x</c></param>
		/// <param name="b">The second value, weighted by <c>w.y</c></param>
		/// <param name="c">The third value, weighted by <c>w.z</c></param>
		[MethodImpl( INLINE )] public static float WeightedSum( Vector3 w, float a, float b, float c ) => a * w.x + b * w.y + c * w.z;

		/// <summary>Multiplies each component of <c>w</c> by the input values, and returns their sum</summary>
		/// <param name="w">The weights (per component) to apply to the rest of the values</param>
		/// <param name="a">The first value, weighted by <c>w.x</c></param>
		/// <param name="b">The second value, weighted by <c>w.y</c></param>
		/// <param name="c">The third value, weighted by <c>w.z</c></param>
		/// <param name="d">The fourth value, weighted by <c>w.w</c></param>
		[MethodImpl( INLINE )] public static float WeightedSum( Vector4 w, float a, float b, float c, float d ) => a * w.x + b * w.y + c * w.z + d * w.w;

		/// <summary>Multiplies each component of <c>w</c> by the input values, and returns their sum</summary>
		/// <param name="w">The weights (per component) to apply to the rest of the values</param>
		/// <param name="a">The first value, weighted by <c>w.x</c></param>
		/// <param name="b">The second value, weighted by <c>w.y</c></param>		
		[MethodImpl( INLINE )] public static Vector2 WeightedSum( Vector2 w, Vector2 a, Vector2 b ) => a * w.x + b * w.y;

		/// <summary>Multiplies each component of <c>w</c> by the input values, and returns their sum</summary>
		/// <param name="w">The weights (per component) to apply to the rest of the values</param>
		/// <param name="a">The first value, weighted by <c>w.x</c></param>
		/// <param name="b">The second value, weighted by <c>w.y</c></param>
		/// <param name="c">The third value, weighted by <c>w.z</c></param>
		[MethodImpl( INLINE )] public static Vector2 WeightedSum( Vector3 w, Vector2 a, Vector2 b, Vector2 c ) => a * w.x + b * w.y + c * w.z;

		/// <summary>Multiplies each component of <c>w</c> by the input values, and returns their sum</summary>
		/// <param name="w">The weights (per component) to apply to the rest of the values</param>
		/// <param name="a">The first value, weighted by <c>w.x</c></param>
		/// <param name="b">The second value, weighted by <c>w.y</c></param>
		/// <param name="c">The third value, weighted by <c>w.z</c></param>
		/// <param name="d">The fourth value, weighted by <c>w.w</c></param>
		[MethodImpl( INLINE )] public static Vector2 WeightedSum( Vector4 w, Vector2 a, Vector2 b, Vector2 c, Vector2 d ) => a * w.x + b * w.y + c * w.z + d * w.w;

		/// <summary>Multiplies each component of <c>w</c> by the input values, and returns their sum</summary>
		/// <param name="w">The weights (per component) to apply to the rest of the values</param>
		/// <param name="a">The first value, weighted by <c>w.x</c></param>
		/// <param name="b">The second value, weighted by <c>w.y</c></param>
		[MethodImpl( INLINE )] public static Vector3 WeightedSum( Vector3 w, Vector3 a, Vector3 b ) => a * w.x + b * w.y;

		/// <summary>Multiplies each component of <c>w</c> by the input values, and returns their sum</summary>
		/// <param name="w">The weights (per component) to apply to the rest of the values</param>
		/// <param name="a">The first value, weighted by <c>w.x</c></param>
		/// <param name="b">The second value, weighted by <c>w.y</c></param>
		/// <param name="c">The third value, weighted by <c>w.z</c></param>
		[MethodImpl( INLINE )] public static Vector3 WeightedSum( Vector3 w, Vector3 a, Vector3 b, Vector3 c ) => a * w.x + b * w.y + c * w.z;

		/// <summary>Multiplies each component of <c>w</c> by the input values, and returns their sum</summary>
		/// <param name="w">The weights (per component) to apply to the rest of the values</param>
		/// <param name="a">The first value, weighted by <c>w.x</c></param>
		/// <param name="b">The second value, weighted by <c>w.y</c></param>
		/// <param name="c">The third value, weighted by <c>w.z</c></param>
		/// <param name="d">The fourth value, weighted by <c>w.w</c></param>
		[MethodImpl( INLINE )] public static Vector3 WeightedSum( Vector4 w, Vector3 a, Vector3 b, Vector3 c, Vector3 d ) => a * w.x + b * w.y + c * w.z + d * w.w;

		/// <summary>Multiplies each component of <c>w</c> by the input values, and returns their sum</summary>
		/// <param name="w">The weights (per component) to apply to the rest of the values</param>
		/// <param name="a">The first value, weighted by <c>w.x</c></param>
		/// <param name="b">The second value, weighted by <c>w.y</c></param>
		[MethodImpl( INLINE )] public static Vector4 WeightedSum( Vector4 w, Vector4 a, Vector4 b ) => a * w.x + b * w.y;

		/// <summary>Multiplies each component of <c>w</c> by the input values, and returns their sum</summary>
		/// <param name="w">The weights (per component) to apply to the rest of the values</param>
		/// <param name="a">The first value, weighted by <c>w.x</c></param>
		/// <param name="b">The second value, weighted by <c>w.y</c></param>
		/// <param name="c">The third value, weighted by <c>w.z</c></param>
		[MethodImpl( INLINE )] public static Vector4 WeightedSum( Vector4 w, Vector4 a, Vector4 b, Vector4 c ) => a * w.x + b * w.y + c * w.z;

		/// <summary>Multiplies each component of <c>w</c> by the input values, and returns their sum</summary>
		/// <param name="w">The weights (per component) to apply to the rest of the values</param>
		/// <param name="a">The first value, weighted by <c>w.x</c></param>
		/// <param name="b">The second value, weighted by <c>w.y</c></param>
		/// <param name="c">The third value, weighted by <c>w.z</c></param>
		/// <param name="d">The fourth value, weighted by <c>w.w</c></param>
		[MethodImpl( INLINE )] public static Vector4 WeightedSum( Vector4 w, Vector4 a, Vector4 b, Vector4 c, Vector4 d ) => a * w.x + b * w.y + c * w.z + d * w.w;

		#endregion

		#region Vector math

		/// <summary>The determinant is equivalent to the dot product, but with one vector rotated 90 degrees.
		/// Note that <c>det(a,b) != det(b,a)</c>. <c>It's equivalent to a.x * b.y - a.y * b.x</c>.
		/// It is also known as the 2D Cross Product, Wedge Product, Outer Product and Perpendicular Dot Product</summary>
		public static float Determinant /*or Cross*/( Vector2 a, Vector2 b ) => a.x * b.y - a.y * b.x; // 2D "cross product"

		/// <summary>Returns the direction and magnitude of the vector. Cheaper than calculating length and normalizing it separately</summary>
		public static (Vector2 dir, float magnitude ) GetDirAndMagnitude( Vector2 v ) {
			float magnitude = v.magnitude;
			return ( v / magnitude, magnitude );
		}

		/// <inheritdoc cref="Mathfs.GetDirAndMagnitude(Vector2)"/>
		public static (Vector3 dir, float magnitude ) GetDirAndMagnitude( Vector3 v ) {
			float magnitude = v.magnitude;
			return ( v / magnitude, magnitude );
		}

		/// <summary>Clamps the length of the vector between <c>min</c> and <c>max</c></summary>
		/// <param name="v">The vector to clamp</param>
		/// <param name="min">Minimum length</param>
		/// <param name="max">Maximum length</param>
		public static Vector2 ClampMagnitude( Vector2 v, float min, float max ) {
			float mag = v.magnitude;
			return mag < min ? ( v / mag ) * min : mag > max ? ( v / mag ) * max : v;
		}

		/// <inheritdoc cref="Mathfs.ClampMagnitude(Vector2,float,float)"/>
		public static Vector3 ClampMagnitude( Vector3 v, float min, float max ) {
			float mag = v.magnitude;
			return mag < min ? ( v / mag ) * min : mag > max ? ( v / mag ) * max : v;
		}

		/// <summary>Returns the chebyshev distance between the two vectors</summary>
		[MethodImpl( INLINE )] public static float ChebyshevDistance( Vector3 a, Vector3 b ) => Max( Abs( a.x - b.x ), Abs( a.y - b.y ), Abs( a.z - b.z ) );

		/// <summary>Returns the taxicab/rectilinear distance between the two vectors</summary>
		[MethodImpl( INLINE )] public static float TaxicabDistance( Vector3 a, Vector3 b ) => Abs( a.x - b.x ) + Abs( a.y - b.y ) + Abs( a.z - b.z );

		/// <inheritdoc cref="ChebyshevDistance(Vector2,Vector2)"/>
		[MethodImpl( INLINE )] public static float ChebyshevDistance( Vector2 a, Vector2 b ) => Max( Abs( a.x - b.x ), Abs( a.y - b.y ) );

		/// <inheritdoc cref="TaxicabDistance(Vector2,Vector2)"/>
		[MethodImpl( INLINE )] public static float TaxicabDistance( Vector2 a, Vector2 b ) => Abs( a.x - b.x ) + Abs( a.y - b.y );

		/// <summary>Returns the average/center of the two input vectors</summary>
		[MethodImpl( INLINE )] public static Vector2 Average( Vector2 a, Vector2 b ) => ( a + b ) / 2f;

		/// <inheritdoc cref="Average(Vector2,Vector2)"/>
		[MethodImpl( INLINE )] public static Vector3 Average( Vector3 a, Vector3 b ) => ( a + b ) / 2f;

		/// <summary>Returns the average/halfway direction between the two input direction vectors. Note that this presumes both <c>aDir</c> and <c>bDir</c> have the same length</summary>
		[MethodImpl( INLINE )] public static Vector2 AverageDir( Vector2 aDir, Vector2 bDir ) => ( aDir + bDir ).normalized;

		/// <inheritdoc cref="AverageDir(Vector2,Vector2)"/>
		[MethodImpl( INLINE )] public static Vector3 AverageDir( Vector3 aDir, Vector3 bDir ) => ( aDir + bDir ).normalized;

		/// <summary>Returns the squared distance between two points.
		/// This is faster than the actual distance, and is useful when comparing distances where the absolute distance doesn't matter</summary>
		[MethodImpl( INLINE )] public static float DistanceSquared( Vector2 a, Vector2 b ) => ( a.x - b.x ).Square() + ( a.y - b.y ).Square();

		/// <inheritdoc cref="DistanceSquared(Vector2,Vector2)"/>
		[MethodImpl( INLINE )] public static float DistanceSquared( Vector3 a, Vector3 b ) => ( a.x - b.x ).Square() + ( a.y - b.y ).Square() + ( a.z - b.z ).Square();

		/// <inheritdoc cref="DistanceSquared(Vector2,Vector2)"/>
		[MethodImpl( INLINE )] public static float DistanceSquared( Vector4 a, Vector4 b ) => ( a.x - b.x ).Square() + ( a.y - b.y ).Square() + ( a.z - b.z ).Square() + ( a.w - b.w ).Square();

		#endregion

		#region Angles & Rotation

		/// <summary>Returns the direction of the input angle, as a normalized vector</summary>
		/// <param name="aRad">The input angle, in radians</param>
		/// <seealso cref="MathfsExtensions.Angle"/>
		[MethodImpl( INLINE )] public static Vector2 AngToDir( float aRad ) => new Vector2( MathF.Cos( aRad ), MathF.Sin( aRad ) );

		/// <summary>Returns the angle of the input vector, in radians. You can also use <c>myVector.Angle()</c></summary>
		/// <param name="vec">The vector to get the angle of. It does not have to be normalized</param>
		/// <seealso cref="MathfsExtensions.Angle"/>
		[MethodImpl( INLINE )] public static float DirToAng( Vector2 vec ) => MathF.Atan2( vec.y, vec.x );

		/// <summary>Returns a 2D orientation from a vector, representing the X axis</summary>
		/// <param name="v">The direction to create a 2D orientation from (does not have to be normalized)</param>
		[MethodImpl( INLINE )] public static Quaternion DirToOrientation( Vector2 v ) {
			v.Normalize();
			v.x += 1;
			v.Normalize();
			return new Quaternion( 0, 0, v.y, v.x );
		}

		/// <summary>The angle between two quaternions, in radians</summary>
		public static float Angle( Quaternion a, Quaternion b ) {
			float num = Mathf.Min( Mathf.Abs( Quaternion.Dot( a, b ) ), 1f );
			return num > 0.999998986721039 ? 0.0f : (float)( MathF.Acos( num ) * 2.0 );
		}

		/// <summary>Returns a 2D Pose from a point and a vector, representing the X axis</summary>
		/// <param name="pt">The location of the pose</param>
		/// <param name="v">The direction to create a 2D orientation from (does not have to be normalized)</param>
		[MethodImpl( INLINE )] public static Pose PointDirToPose( Vector2 pt, Vector2 v ) => new Pose( pt, DirToOrientation( v ) );

		/// <summary>Linearly blends between two poses. The position will lerp, while the rotation will slerp. Note: this lerp is unclamped</summary>
		/// <param name="a">Pose at t = 0</param>
		/// <param name="b">Pose at t = 1</param>
		/// <param name="t">The t-value to blend from a to b, from 0 to 1 (values outside will extrapolate)</param>
		public static Pose Lerp( Pose a, Pose b, float t ) =>
			new Pose(
				Vector3.LerpUnclamped( a.position, b.position, t ),
				Quaternion.SlerpUnclamped( a.rotation, b.rotation, t )
			);

		/// <summary>Returns a matrix representing a 2D position and rotation</summary>
		/// <param name="point">The location of the matrix</param>
		/// <param name="tangent">The direction of the X axis (has to be normalized)</param>
		[MethodImpl( INLINE )] public static Matrix4x4 GetMatrixFrom2DPointDir( Vector2 point, Vector2 tangent ) {
			Vector2 N = tangent.Rotate90CCW();
			return new Matrix4x4(
				new Vector4( tangent.x, tangent.y, 0, 0 ),
				new Vector4( N.x, N.y, 0, 0 ),
				new Vector4( 0, 0, 1, 0 ),
				new Vector4( point.x, point.y, 0, 1 )
			);
		}

		/// <summary>Returns the signed curvature at a point in a curve, in radians per distance unit (equivalent to the reciprocal radius of the osculating circle)</summary>
		/// <param name="velocity">The first derivative of the point in the curve</param>
		/// <param name="acceleration">The second derivative of the point in the curve</param>
		[MethodImpl( INLINE )] public static float GetCurvature( Vector2 velocity, Vector2 acceleration ) {
			float dMag = velocity.magnitude;
			return Determinant( velocity, acceleration ) / ( dMag * dMag * dMag );
		}

		/// <summary>Returns the curvature of a point in a 3D curve, as a Bivector.
		/// The magnitude is the curvature in radians per distance unit,
		/// casting it to a Vector3 gives you the axis of curvature</summary>
		/// <param name="velocity">The first derivative of the point in the curve</param>
		/// <param name="acceleration">The second derivative of the point in the curve</param>
		[MethodImpl( INLINE )] public static Bivector3 GetCurvature( Vector3 velocity, Vector3 acceleration ) {
			float dMag = velocity.magnitude;
			return Wedge( velocity, acceleration ) / ( dMag * dMag * dMag );
		}

		/// <summary>Returns the torsion of a given point in a curve, in radians per distance unit</summary>
		/// <param name="velocity">The first derivative of the point in the curve</param>
		/// <param name="acceleration">The second derivative of the point in the curve</param>
		/// <param name="jerk">The third derivative of the point in the curve</param>
		[MethodImpl( INLINE )] public static float GetTorsion( Vector3 velocity, Vector3 acceleration, Vector3 jerk ) {
			Vector3 cVector = Vector3.Cross( velocity, acceleration );
			return Vector3.Dot( cVector, jerk ) / cVector.sqrMagnitude;
		}

		/// <summary>Returns the frenet-serret (curvature-based) normal direction at a given point in a curve</summary>
		/// <param name="velocity">The first derivative of the point in the curve</param>
		/// <param name="acceleration">The second derivative of the point in the curve</param>
		[MethodImpl( INLINE )] public static Vector3 GetArcNormal( Vector3 velocity, Vector3 acceleration ) => Vector3.Cross( Vector3.Cross( velocity, acceleration ).normalized, velocity.normalized );

		/// <summary>Returns the frenet-serret (curvature-based) binormal direction at a given point in a curve</summary>
		/// <param name="velocity">The first derivative of the point in the curve</param>
		/// <param name="acceleration">The second derivative of the point in the curve</param>
		[MethodImpl( INLINE )] public static Vector3 GetArcBinormal( Vector3 velocity, Vector3 acceleration ) => Vector3.Cross( velocity, acceleration ).normalized;

		/// <summary>Returns a normal direction given a reference up vector and a tangent direction</summary>
		/// <param name="tangent">The tangent direction (does not have to be normalized)</param>
		/// <param name="up">The reference up vector. The normal will be perpendicular to both the supplied up vector and the curve</param>
		[MethodImpl( INLINE )] public static Vector3 GetNormalFromLookTangent( Vector3 tangent, Vector3 up ) => Vector3.Cross( up, tangent ).normalized;

		/// <summary>Returns the binormal from a vector, given a reference up vector.
		/// The binormal will attempt to be as aligned with the reference vector as possible,
		/// while still being perpendicular to the tangent</summary>
		/// <param name="tangent">The tangent direction (does not have to be normalized)</param>
		/// <param name="up">The reference up vector. The normal will be perpendicular to both the supplied up vector and the tangent</param>
		[MethodImpl( INLINE )] public static Vector3 GetBinormalFromLookTangent( Vector3 tangent, Vector3 up ) {
			Vector3 normal = Vector3.Cross( up, tangent ).normalized;
			return Vector3.Cross( tangent.normalized, normal );
		}

		/// <summary>Returns the frenet-serret (curvature-based) orientation of a point in a curve with the given velocity and acceleration values, where the Z direction is tangent to the curve.
		/// The X axis will point to the inner arc of the current curvature, while Y is the axis of rotation</summary>
		/// <param name="velocity">The first derivative of the point in the curve</param>
		/// <param name="acceleration">The second derivative of the point in the curve</param>
		[MethodImpl( INLINE )] public static Quaternion GetArcOrientation( Vector3 velocity, Vector3 acceleration ) {
			Vector3 binormal = Vector3.Cross( velocity, acceleration );
			return Quaternion.LookRotation( velocity, binormal );
		}

		/// <inheritdoc cref="GetArcOrientation(Vector3,Vector3)"/>
		[MethodImpl( INLINE )] public static Quaternion GetArcOrientation( Vector2 velocity, Vector2 acceleration ) {
			Vector3 binormal = new Vector3( 0, 0, Sign( Determinant( velocity, acceleration ) ) );
			return Quaternion.LookRotation( velocity, binormal );
		}

		/// <summary>Returns the frenet-serret (curvature-based) orientation of a point in a curve with the given velocity and acceleration values, where the X direction is tangent to the curve.
		/// The Y axis (the normal) will point to the inner arc of the current curvature, while Z is the axis of rotation</summary>
		/// <param name="velocity">The first derivative of the point in the curve</param>
		/// <param name="acceleration">The second derivative of the point in the curve</param>
		[MethodImpl( INLINE )] public static Quaternion GetFrenetSerretOrientation( Vector3 velocity, Vector3 acceleration ) {
			GetCurvatureOrientationAxes( velocity, acceleration, out _, out Vector3 N, out Vector3 B );
			return Quaternion.LookRotation( B, N );
		}

		/// <inheritdoc cref="GetFrenetSerretOrientation(Vector3,Vector3)"/>
		[MethodImpl( INLINE )] public static Quaternion GetFrenetSerretOrientation( Vector2 velocity, Vector2 acceleration ) {
			GetCurvatureOrientationAxes( velocity, acceleration, out _, out Vector3 N, out Vector3 B );
			return Quaternion.LookRotation( B, N );
		}

		/// <summary>Returns the frenet-serret (curvature-based) orientation axes of a point in a curve with the given velocity and acceleration values</summary>
		/// <param name="velocity">The first derivative of the point in the curve</param>
		/// <param name="acceleration">The second derivative of the point in the curve</param>
		/// <param name="tangent">The axis pointing along the curve</param>
		/// <param name="normal">The axis pointing to the inside of the curve</param>
		/// <param name="binormal">The axis of rotation of the curve</param>
		[MethodImpl( INLINE )] public static void GetCurvatureOrientationAxes( Vector3 velocity, Vector3 acceleration, out Vector3 tangent, out Vector3 normal, out Vector3 binormal ) {
			tangent = velocity.normalized;
			binormal = Vector3.Cross( velocity, acceleration ).normalized;
			normal = Vector3.Cross( binormal, tangent );
		}

		/// <inheritdoc cref="GetCurvatureOrientationAxes(Vector3,Vector3,out Vector3,out Vector3,out Vector3)"/>
		[MethodImpl( INLINE )] public static void GetCurvatureOrientationAxes( Vector2 velocity, Vector2 acceleration, out Vector3 tangent, out Vector3 normal, out Vector3 binormal ) {
			tangent = velocity.normalized;
			float sign = Sign( Determinant( velocity, acceleration ) );
			binormal = new Vector3( 0, 0, sign );
			normal = new Vector3( -sign * tangent.y, sign * tangent.x, 0 );
		}

		/// <summary>Returns a 2D look-orientation (X forward), ensuring the returned Y axis is upright with regards to the up vector</summary>
		/// <param name="forward">The forward direction of the rotation (X axis)</param>
		/// <param name="up">The reference up direction of the rotation to align to, usually pointing along world up</param>
		[MethodImpl( INLINE )] public static Quaternion GetLookRotation2D( Vector2 forward, Vector2 up ) {
			int sign = Determinant( forward, up ) >= 0 ? 1 : -1;
			Vector2 Y = new(-sign * forward.y, sign * forward.x);
			Vector3 Z = new(0, 0, sign);
			return Quaternion.LookRotation( Z, Y );
		}

		/// <inheritdoc cref="GetLookRotation2D(Vector2,Vector2)"/>
		[MethodImpl( INLINE )] public static Quaternion GetLookRotation2D( Vector2 forward ) => GetLookRotation2D( forward, Vector2.up );

		/// <summary>Returns the signed angle between <c>a</c> and <c>b</c>, in the range -tau/2 to tau/2 (-pi to pi)</summary>
		[MethodImpl( INLINE )] public static float SignedAngle( Vector2 a, Vector2 b ) => AngleBetween( a, b ) * MathF.Sign( Determinant( a, b ) ); // -tau/2 to tau/2

		/// <summary>Returns the shortest angle between <c>a</c> and <c>b</c>, in the range 0 to tau/2 (0 to pi)</summary>
		[MethodImpl( INLINE )] public static float AngleBetween( Vector2 a, Vector2 b ) => MathF.Acos( Vector2.Dot( a.normalized, b.normalized ).ClampNeg1to1() );

		/// <inheritdoc cref="AngleBetween(Vector2,Vector2)"/>
		[MethodImpl( INLINE )] public static float AngleBetween( Vector3 a, Vector3 b ) => MathF.Acos( Vector3.Dot( a.normalized, b.normalized ).ClampNeg1to1() );

		/// <summary>Returns the clockwise angle between <c>from</c> and <c>to</c>, in the range 0 to tau (0 to 2*pi)</summary>
		[MethodImpl( INLINE )] public static float AngleFromToCW( Vector2 from, Vector2 to ) => Determinant( from, to ) < 0 ? AngleBetween( from, to ) : TAU - AngleBetween( from, to );

		/// <summary>Returns the counterclockwise angle between <c>from</c> and <c>to</c>, in the range 0 to tau (0 to 2*pi)</summary>
		[MethodImpl( INLINE )] public static float AngleFromToCCW( Vector2 from, Vector2 to ) => Determinant( from, to ) > 0 ? AngleBetween( from, to ) : TAU - AngleBetween( from, to );

		/// <summary>Blends between the <c>aRad</c> and <c>bRad</c> angles, based on the input t-value between 0 and 1</summary>
		/// <param name="aRad">The start value, in radians</param>
		/// <param name="bRad">The end value, in radians</param>
		/// <param name="t">The t-value between 0 and 1</param>
		public static float LerpAngle( float aRad, float bRad, float t ) {
			float delta = Repeat( ( bRad - aRad ), TAU );
			if( delta > PI )
				delta -= TAU;
			return aRad + delta * Clamp01( t );
		}

		/// <summary>Returns the shortest angle between the two input angles, in radians</summary>
		[MethodImpl( INLINE )] public static float DeltaAngle( float a, float b ) => ( b - a + PI ).Repeat( TAU ) - PI;

		/// <summary>Given an angle between a and b, returns its normalized location in that range, as a t-value (interpolant) from 0 to 1</summary>
		/// <param name="a">The start angle of the range (in radians), where it would return 0</param>
		/// <param name="b">The end angle of the range (in radians), where it would return 1</param>
		/// <param name="v">An angle between a and b</param>
		public static float InverseLerpAngle( float a, float b, float v ) {
			float angBetween = DeltaAngle( a, b );
			b = a + angBetween; // removes any a->b discontinuity
			float h = a + angBetween * 0.5f; // halfway angle
			v = h + DeltaAngle( h, v ); // get offset from h, and offset by h
			return InverseLerpClamped( a, b, v );
		}

		#endregion

		#region Angular movement helpers

		/// <summary>Same as <see cref="MoveTowards">MoveTowards</see> but makes sure the angles interpolate correctly when they wrap around a full turn.
		/// Variables <c>current</c> and <c>target</c> are assumed to be in radians.
		/// For optimization reasons, negative values of maxDelta are not supported and may cause oscillation.
		/// To push current away from a target angle, add 180 to that angle instead.</summary>
		/// <param name="current">The current angle</param>
		/// <param name="target">The angle to move towards</param>
		/// <param name="maxDelta">The maximum change that should be applied to the value</param>
		public static float MoveTowardsAngle( float current, float target, float maxDelta ) {
			float deltaAngle = DeltaAngle( current, target );
			if( -maxDelta < deltaAngle && deltaAngle < maxDelta )
				return target;
			target = current + deltaAngle;
			return MoveTowards( current, target, maxDelta );
		}

		/// <summary>Gradually changes an angle given in radians towards a desired goal angle over time.
		/// The value is smoothed by some spring-damper like function.
		/// The function can be used to smooth any kind of value, positions, colors, scalars. The most common use is for smoothing a follow camera.</summary>
		/// <param name="current">The current angle</param>
		/// <param name="target">The angle we are trying to reach</param>
		/// <param name="currentVelocity">The current angular velocity, this value is modified by the function every time you call it</param>
		/// <param name="smoothTime">Approximately the time it will take to reach the target. A smaller value will reach the target faster</param>
		/// <param name="maxSpeed">Optionally allows you to clamp the maximum speed</param>
		public static float SmoothDampAngle( float current, float target, ref float currentVelocity, float smoothTime, float maxSpeed = Infinity ) {
			float deltaTime = Time.deltaTime;
			return SmoothDampAngle( current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime );
		}

		/// <summary>Gradually changes an angle given in radians towards a desired goal angle over time.
		/// The value is smoothed by some spring-damper like function.
		/// The function can be used to smooth any kind of value, positions, colors, scalars. The most common use is for smoothing a follow camera.</summary>
		/// <param name="current">The current angle</param>
		/// <param name="target">The angle we are trying to reach</param>
		/// <param name="currentVelocity">The current angular velocity, this value is modified by the function every time you call it</param>
		/// <param name="smoothTime">Approximately the time it will take to reach the target. A smaller value will reach the target faster</param>
		/// <param name="maxSpeed">Optionally allows you to clamp the maximum speed</param>
		/// <param name="deltaTime">The time since the last call to this function. By default Time.deltaTime</param>
		public static float SmoothDampAngle( float current, float target, ref float currentVelocity, float smoothTime, [Uei.DefaultValue( "Mathf.Infinity" )] float maxSpeed, [Uei.DefaultValue( "Time.deltaTime" )] float deltaTime ) {
			target = current + DeltaAngle( current, target );
			return SmoothDamp( current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime );
		}

		#endregion

		#region Shape coordinate remapping

		/// <summary>Given a position within a -1 to 1 square, remaps it to the unit circle</summary>
		/// <param name="c">The input position inside the square</param>
		public static Vector2 SquareToDisc( Vector2 c ) {
			c.x = c.x.ClampNeg1to1();
			c.y = c.y.ClampNeg1to1();
			float u = c.x * Sqrt( 1 - ( c.y * c.y ) / 2 );
			float v = c.y * Sqrt( 1 - ( c.x * c.x ) / 2 );
			return new Vector2( u, v );
		}

		/// <summary>Given a position within the unit circle, remaps it to a square in the -1 to 1 range</summary>
		/// <param name="c">The input position inside the circle</param>
		public static Vector2 DiscToSquare( Vector2 c ) {
			c = c.ClampMagnitude( 0, 1 );
			float u2 = c.x * c.x;
			float v2 = c.y * c.y;
			Vector2 n = new Vector2( 1, -1 );
			Vector2 p = new Vector2( 2, 2 ) + n * ( u2 - v2 );
			Vector2 q = 2 * SQRT2 * c;
			Vector2 smolVec = Vector2.one * 0.0001f;
			return 0.5f * ( Vector2.Max( smolVec, p + q ).Sqrt() - Vector2.Max( smolVec, p - q ).Sqrt() );
		}

		#endregion

	}

}