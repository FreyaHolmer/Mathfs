// Some of this code is similar to Unity's original Mathf source to match functionality.
// The original Mathf.cs source https://github.com/Unity-Technologies/UnityCsReference/blob/master/Runtime/Export/Math/Mathf.cs
// ...and the trace amounts of it left in here is copyright (c) Unity Technologies with license: https://unity3d.com/legal/licenses/Unity_Reference_Only_License
// 
// Collected and expanded upon to by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using System;
using System.ComponentModel;
using UnityEngine;
using Uei = UnityEngine.Internal;
using System.Linq; // used for arbitrary count min/max functions, so it's safe and won't allocate garbage don't worry~
using System.Runtime.CompilerServices;

namespace Freya {

	public static class Mathfs {

		const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

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

		/// <summary>Multiply an angle in degrees by this, to convert it to radians</summary>
		public const float Deg2Rad = TAU / 360f;

		/// <summary>Multiply an angle in radians by this, to convert it to degrees</summary>
		public const float Rad2Deg = 360f / TAU;

		#endregion

		#region Math operations

		/// <summary>Returns the square root of the given value</summary>
		[MethodImpl( INLINE )] public static float Sqrt( float value ) => (float)Math.Sqrt( value );

		/// <summary>Returns the square root of each component</summary>
		[MethodImpl( INLINE )] public static Vector2 Sqrt( Vector2 v ) => new Vector2( Sqrt( v.x ), Sqrt( v.y ) );

		/// <inheritdoc cref="Mathfs.Sqrt(Vector2)"/>
		[MethodImpl( INLINE )] public static Vector3 Sqrt( Vector3 v ) => new Vector3( Sqrt( v.x ), Sqrt( v.y ), Sqrt( v.z ) );

		/// <inheritdoc cref="Mathfs.Sqrt(Vector2)"/>
		[MethodImpl( INLINE )] public static Vector4 Sqrt( Vector4 v ) => new Vector4( Sqrt( v.x ), Sqrt( v.y ), Sqrt( v.z ), Sqrt( v.w ) );

		/// <summary>Returns the cube root of the given value, properly handling negative values unlike Pow(v,1/3)</summary>
		[MethodImpl( INLINE )] public static float Cbrt( float value ) => value < 0 ? -Pow( -value, 1f / 3f ) : Pow( value, 1f / 3f );

		/// <summary>Returns <c>value</c> raised to the power of <c>exponent</c></summary>
		[MethodImpl( INLINE )] public static float Pow( float value, float exponent ) => (float)Math.Pow( value, exponent );

		/// <summary>Returns e to the power of the given value</summary>
		[MethodImpl( INLINE )] public static float Exp( float power ) => (float)Math.Exp( power );

		/// <summary>Returns the logarithm of a value, with the given base</summary>
		[MethodImpl( INLINE )] public static float Log( float value, float @base ) => (float)Math.Log( value, @base );

		/// <summary>Returns the natural logarithm of the given value</summary>
		[MethodImpl( INLINE )] public static float Log( float value ) => (float)Math.Log( value );

		/// <summary>Returns the base 10 logarithm of the given value</summary>
		[MethodImpl( INLINE )] public static float Log10( float value ) => (float)Math.Log10( value );

		#endregion

		#region Floating point shenanigans

		public static readonly float Epsilon = UnityEngineInternal.MathfInternal.IsFlushToZeroEnabled ? UnityEngineInternal.MathfInternal.FloatMinNormal : UnityEngineInternal.MathfInternal.FloatMinDenormal;
		public const float Infinity = float.PositiveInfinity;
		public const float NegativeInfinity = float.NegativeInfinity;
		[MethodImpl( INLINE )] public static bool Approximately( float a, float b ) => Abs( b - a ) < Max( 0.000001f * Max( Abs( a ), Abs( b ) ), Epsilon * 8 );

		#endregion

		#region Trigonometry

		/// <summary>Returns the cosine of the given angle. Equivalent to the x-component of a unit vector with the same angle</summary>
		/// <param name="angRad">Angle in radians</param>
		[MethodImpl( INLINE )] public static float Cos( float angRad ) => (float)Math.Cos( angRad );

		/// <summary>Returns the sine of the given angle. Equivalent to the y-component of a unit vector with the same angle</summary>
		/// <param name="angRad">Angle in radians</param>
		[MethodImpl( INLINE )] public static float Sin( float angRad ) => (float)Math.Sin( angRad );

		/// <summary>Returns the tangent of the given angle</summary>
		/// <param name="angRad">Angle in radians</param>
		[MethodImpl( INLINE )] public static float Tan( float angRad ) => (float)Math.Tan( angRad );

		/// <summary>Returns the arc cosine of the given value, in radians</summary>
		/// <param name="value">A value between -1 and 1</param>
		[MethodImpl( INLINE )] public static float Acos( float value ) => (float)Math.Acos( value );

		/// <summary>Returns the arc sine of the given value, in radians</summary>
		/// <param name="value">A value between -1 and 1</param>
		[MethodImpl( INLINE )] public static float Asin( float value ) => (float)Math.Asin( value );

		/// <summary>Returns the arc tangent of the given value, in radians</summary>
		/// <param name="value">A value between -1 and 1</param>
		[MethodImpl( INLINE )] public static float Atan( float value ) => (float)Math.Atan( value );

		/// <summary>Returns the angle of a vector. I don't recommend using this function, it's confusing~ Use Mathfs.DirToAng instead</summary>
		/// <param name="y">The y component of the vector. They're flipped yeah I know but this is how everyone implements if for some godforsaken reason</param>
		/// <param name="x">The x component of the vector. They're flipped yeah I know but this is how everyone implements if for some godforsaken reason</param>
		[MethodImpl( INLINE )] public static float Atan2( float y, float x ) => (float)Math.Atan2( y, x );

		/// <summary>Returns the cosecant of the given angle</summary>
		/// <param name="angRad">Angle in radians</param>
		[MethodImpl( INLINE )] public static float Csc( float angRad ) => 1f / (float)Math.Sin( angRad );

		/// <summary>Returns the secant of the given angle</summary>
		/// <param name="angRad">Angle in radians</param>
		[MethodImpl( INLINE )] public static float Sec( float angRad ) => 1f / (float)Math.Cos( angRad );

		/// <summary>Returns the cotangent of the given angle</summary>
		/// <param name="angRad">Angle in radians</param>
		[MethodImpl( INLINE )] public static float Cot( float angRad ) => 1f / (float)Math.Tan( angRad );

		/// <summary>Returns the versine of the given angle</summary>
		/// <param name="angRad">Angle in radians</param>
		[MethodImpl( INLINE )] public static float Ver( float angRad ) => 1 - (float)Math.Cos( angRad );

		/// <summary>Returns the coversine of the given angle</summary>
		/// <param name="angRad">Angle in radians</param>
		[MethodImpl( INLINE )] public static float Cvs( float angRad ) => 1 - (float)Math.Sin( angRad );

		/// <summary>Returns the chord of the given angle</summary>
		/// <param name="angRad">Angle in radians</param>
		[MethodImpl( INLINE )] public static float Crd( float angRad ) => 2 * (float)Math.Sin( angRad / 2 );

		#endregion

		#region Hyperbolic Trigonometry

		/// <summary>Returns the hyperbolic cosine of the given hyperbolic angle</summary>
		[MethodImpl( INLINE )] public static float Cosh( float x ) => (float)Math.Cosh( x );

		/// <summary>Returns the hyperbolic sine of the given hyperbolic angle</summary>
		[MethodImpl( INLINE )] public static float Sinh( float x ) => (float)Math.Sinh( x );

		/// <summary>Returns the hyperbolic tangent of the given hyperbolic angle</summary>
		[MethodImpl( INLINE )] public static float Tanh( float x ) => (float)Math.Tanh( x );

		/// <summary>Returns the hyperbolic arc cosine of the given value</summary>
		[MethodImpl( INLINE )] public static float Acosh( float x ) => (float)Math.Log( x + Mathf.Sqrt( x * x - 1 ) );

		/// <summary>Returns the hyperbolic arc sine of the given value</summary>
		[MethodImpl( INLINE )] public static float Asinh( float x ) => (float)Math.Log( x + Mathf.Sqrt( x * x + 1 ) );

		/// <summary>Returns the hyperbolic arc tangent of the given value</summary>
		[MethodImpl( INLINE )] public static float Atanh( float x ) => (float)( 0.5 * Math.Log( ( 1 + x ) / ( 1 - x ) ) );

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
		[MethodImpl( INLINE )] public static float Floor( float value ) => (float)Math.Floor( value );

		/// <summary>Rounds the vector components down to the nearest integer</summary>
		[MethodImpl( INLINE )] public static Vector2 Floor( Vector2 value ) => new Vector2( (float)Math.Floor( value.x ), (float)Math.Floor( value.y ) );

		/// <inheritdoc cref="Mathfs.Floor(Vector2)"/>
		[MethodImpl( INLINE )] public static Vector3 Floor( Vector3 value ) => new Vector3( (float)Math.Floor( value.x ), (float)Math.Floor( value.y ), (float)Math.Floor( value.z ) );

		/// <inheritdoc cref="Mathfs.Floor(Vector2)"/>
		[MethodImpl( INLINE )] public static Vector4 Floor( Vector4 value ) => new Vector4( (float)Math.Floor( value.x ), (float)Math.Floor( value.y ), (float)Math.Floor( value.z ), (float)Math.Floor( value.w ) );

		/// <summary>Rounds the value down to the nearest integer, returning an int value</summary>
		[MethodImpl( INLINE )] public static int FloorToInt( float value ) => (int)Math.Floor( value );

		/// <summary>Rounds the vector components down to the nearest integer, returning an integer vector</summary>
		[MethodImpl( INLINE )] public static Vector2Int FloorToInt( Vector2 value ) => new Vector2Int( (int)Math.Floor( value.x ), (int)Math.Floor( value.y ) );

		/// <inheritdoc cref="Mathfs.FloorToInt(Vector2)"/>
		[MethodImpl( INLINE )] public static Vector3Int FloorToInt( Vector3 value ) => new Vector3Int( (int)Math.Floor( value.x ), (int)Math.Floor( value.y ), (int)Math.Floor( value.z ) );

		/// <summary>Rounds the value up to the nearest integer</summary>
		[MethodImpl( INLINE )] public static float Ceil( float value ) => (float)Math.Ceiling( value );

		/// <summary>Rounds the vector components up to the nearest integer</summary>
		[MethodImpl( INLINE )] public static Vector2 Ceil( Vector2 value ) => new Vector2( (float)Math.Ceiling( value.x ), (float)Math.Ceiling( value.y ) );

		/// <inheritdoc cref="Mathfs.Ceil(Vector2)"/>
		[MethodImpl( INLINE )] public static Vector3 Ceil( Vector3 value ) => new Vector3( (float)Math.Ceiling( value.x ), (float)Math.Ceiling( value.y ), (float)Math.Ceiling( value.z ) );

		/// <inheritdoc cref="Mathfs.Ceil(Vector2)"/>
		[MethodImpl( INLINE )] public static Vector4 Ceil( Vector4 value ) => new Vector4( (float)Math.Ceiling( value.x ), (float)Math.Ceiling( value.y ), (float)Math.Ceiling( value.z ), (float)Math.Ceiling( value.w ) );

		/// <summary>Rounds the value up to the nearest integer, returning an int value</summary>
		[MethodImpl( INLINE )] public static int CeilToInt( float value ) => (int)Math.Ceiling( value );

		/// <summary>Rounds the vector components up to the nearest integer, returning an integer vector</summary>
		[MethodImpl( INLINE )] public static Vector2Int CeilToInt( Vector2 value ) => new Vector2Int( (int)Math.Ceiling( value.x ), (int)Math.Ceiling( value.y ) );

		/// <inheritdoc cref="Mathfs.CeilToInt(Vector2)"/>
		[MethodImpl( INLINE )] public static Vector3Int CeilToInt( Vector3 value ) => new Vector3Int( (int)Math.Ceiling( value.x ), (int)Math.Ceiling( value.y ), (int)Math.Ceiling( value.z ) );

		/// <summary>Rounds the value to the nearest integer</summary>
		[MethodImpl( INLINE )] public static float Round( float value ) => (float)Math.Round( value );

		/// <summary>Rounds the vector components to the nearest integer</summary>
		[MethodImpl( INLINE )] public static Vector2 Round( Vector2 value ) => new Vector2( (float)Math.Round( value.x ), (float)Math.Round( value.y ) );

		/// <inheritdoc cref="Mathfs.Round(Vector2)"/>
		[MethodImpl( INLINE )] public static Vector3 Round( Vector3 value ) => new Vector3( (float)Math.Round( value.x ), (float)Math.Round( value.y ), (float)Math.Round( value.z ) );

		/// <inheritdoc cref="Mathfs.Round(Vector2)"/>
		[MethodImpl( INLINE )] public static Vector4 Round( Vector4 value ) => new Vector4( (float)Math.Round( value.x ), (float)Math.Round( value.y ), (float)Math.Round( value.z ), (float)Math.Round( value.w ) );

		/// <summary>Rounds the value to the nearest value, snapped to the given interval size</summary>
		[MethodImpl( INLINE )] public static float Round( float value, float snapInterval ) => Mathf.Round( value / snapInterval ) * snapInterval;

		/// <summary>Rounds the vector components to the nearest value, snapped to the given interval size</summary>
		[MethodImpl( INLINE )] public static Vector2 Round( Vector2 value, float snapInterval ) => new Vector2( Round( value.x, snapInterval ), Round( value.y, snapInterval ) );

		/// <inheritdoc cref="Mathfs.Round(Vector2,float)"/>
		[MethodImpl( INLINE )] public static Vector3 Round( Vector3 value, float snapInterval ) => new Vector3( Round( value.x, snapInterval ), Round( value.y, snapInterval ), Round( value.z, snapInterval ) );

		/// <inheritdoc cref="Mathfs.Round(Vector2,float)"/>
		[MethodImpl( INLINE )] public static Vector4 Round( Vector4 value, float snapInterval ) => new Vector4( Round( value.x, snapInterval ), Round( value.y, snapInterval ), Round( value.z, snapInterval ), Round( value.w, snapInterval ) );

		/// <summary>Rounds the value to the nearest integer, returning an int value</summary>
		[MethodImpl( INLINE )] public static int RoundToInt( float value ) => (int)Math.Round( value );

		/// <summary>Rounds the vector components to the nearest integer, returning an integer vector</summary>
		[MethodImpl( INLINE )] public static Vector2Int RoundToInt( Vector2 value ) => new Vector2Int( (int)Math.Round( value.x ), (int)Math.Round( value.y ) );

		/// <inheritdoc cref="Mathfs.RoundToInt(Vector2)"/>
		[MethodImpl( INLINE )] public static Vector3Int RoundToInt( Vector3 value ) => new Vector3Int( (int)Math.Round( value.x ), (int)Math.Round( value.y ), (int)Math.Round( value.z ) );

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
		[MethodImpl( INLINE )] public static int Mod( int value, int length ) => ( value % length + length ) % length;

		/// <summary>Repeats a value within a range, going back and forth</summary>
		[MethodImpl( INLINE )] public static float PingPong( float t, float length ) => length - Abs( Repeat( t, length * 2f ) - length );

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

		/// <summary>Exponential interpolation, the multiplicative version of lerp, useful for values such as scaling or zooming</summary>
		/// <param name="a">The start value</param>
		/// <param name="b">The end value</param>
		/// <param name="t">The t-value from 0 to 1 representing position along the lerp</param>
		[MethodImpl( INLINE )] public static float Eerp( float a, float b, float t ) => Mathf.Pow( a, 1 - t ) * Mathf.Pow( b, t );

		/// <summary>Inverse exponential interpolation, the multiplicative version of InverseLerp, useful for values such as scaling or zooming</summary>
		/// <param name="a">The start value</param>
		/// <param name="b">The end value</param>
		/// <param name="v">A value between a and b. Note: values outside this range are still valid, and will be extrapolated</param>
		[MethodImpl( INLINE )] public static float InverseEerp( float a, float b, float v ) => Mathf.Log( a / v ) / Mathf.Log( a / b );

		#endregion

		#region Movement helpers

		/// <summary>Moves a value <c>current</c> towards <c>target</c></summary>
		/// <param name="current">The current value</param>
		/// <param name="target">The value to move towards</param>
		/// <param name="maxDelta">The maximum change that should be applied to the value</param>
		public static float MoveTowards( float current, float target, float maxDelta ) {
			if( Mathf.Abs( target - current ) <= maxDelta )
				return target;
			return current + Mathf.Sign( target - current ) * maxDelta;
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
			smoothTime = Mathf.Max( 0.0001F, smoothTime );
			float omega = 2F / smoothTime;

			float x = omega * deltaTime;
			float exp = 1F / ( 1F + x + 0.48F * x * x + 0.235F * x * x * x );
			float change = current - target;
			float originalTo = target;

			// Clamp maximum speed
			float maxChange = maxSpeed * smoothTime;
			change = Mathf.Clamp( change, -maxChange, maxChange );
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

		/// <summary>Returns the average/center of the two input vectors</summary>
		[MethodImpl( INLINE )] public static Vector2 Average( Vector2 a, Vector2 b ) => ( a + b ) / 2f;

		/// <summary>Returns the average/center of the two input vectors</summary>
		[MethodImpl( INLINE )] public static Vector3 Average( Vector3 a, Vector3 b ) => ( a + b ) / 2f;

		/// <summary>Returns the average/halfway direction between the two input direction vectors. Note that this presumes both <c>aDir</c> and <c>bDir</c> have the same length</summary>
		[MethodImpl( INLINE )] public static Vector2 AverageDir( Vector2 aDir, Vector2 bDir ) => ( aDir + bDir ).normalized;

		/// <summary>Returns the average/halfway direction between the two input direction vectors. Note that this presumes both <c>aDir</c> and <c>bDir</c> have the same length</summary>
		[MethodImpl( INLINE )] public static Vector3 AverageDir( Vector3 aDir, Vector3 bDir ) => ( aDir + bDir ).normalized;

		/// <summary>Returns the squared distance between two points.
		/// This is faster than the actual distance, and is useful when comparing distances where the absolute distance doesn't matter</summary>
		[MethodImpl( INLINE )] public static float DistanceSquared( Vector2 a, Vector2 b ) => ( a.x - b.x ).Square() + ( a.y - b.y ).Square();

		/// <summary>Returns the squared distance between two points.
		/// This is faster than the actual distance, and is useful when comparing distances where the absolute distance doesn't matter</summary>
		[MethodImpl( INLINE )] public static float DistanceSquared( Vector3 a, Vector3 b ) => ( a.x - b.x ).Square() + ( a.y - b.y ).Square() + ( a.z - b.z ).Square();

		/// <summary>Returns the squared distance between two points.
		/// This is faster than the actual distance, and is useful when comparing distances where the absolute distance doesn't matter</summary>
		[MethodImpl( INLINE )] public static float DistanceSquared( Vector4 a, Vector4 b ) => ( a.x - b.x ).Square() + ( a.y - b.y ).Square() + ( a.z - b.z ).Square() + ( a.w - b.w ).Square();

		#endregion

		#region Angles & Rotation

		/// <summary>Returns the direction of the input angle, as a normalized vector</summary>
		/// <param name="aRad">The input angle, in radians</param>
		/// <seealso cref="MathfsExtensions.Angle"/>
		[MethodImpl( INLINE )] public static Vector2 AngToDir( float aRad ) => new Vector2( Mathf.Cos( aRad ), Mathf.Sin( aRad ) );

		/// <summary>Returns the angle of the input vector, in radians. You can also use <c>myVector.Angle()</c></summary>
		/// <param name="vec">The vector to get the angle of. It does not have to be normalized</param>
		/// <seealso cref="MathfsExtensions.Angle"/>
		[MethodImpl( INLINE )] public static float DirToAng( Vector2 vec ) => Mathf.Atan2( vec.y, vec.x );

		/// <summary>Returns the signed angle between <c>a</c> and <c>b</c>, in the range -tau/2 to tau/2 (-pi to pi)</summary>
		[MethodImpl( INLINE )] public static float SignedAngle( Vector2 a, Vector2 b ) => AngleBetween( a, b ) * Mathf.Sign( Determinant( a, b ) ); // -tau/2 to tau/2

		/// <summary>Returns the shortest angle between <c>a</c> and <c>b</c>, in the range 0 to tau/2 (0 to pi)</summary>
		[MethodImpl( INLINE )] public static float AngleBetween( Vector2 a, Vector2 b ) => Mathf.Acos( Vector2.Dot( a.normalized, b.normalized ).ClampNeg1to1() );

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

		#region Root Finding

		public enum PolynomialType {
			Constant,
			Linear,
			Quadratic,
			Cubic
		}

		static bool FactorAlmost0( float v ) => v.Abs() < 0.00001f;

		/// <summary>Given ax³+bx²+cx+d, returns the net polynomial type/degree, accounting for values very close to 0</summary>
		[MethodImpl( INLINE )] public static PolynomialType GetPolynomialType( float a, float b, float c, float d ) => FactorAlmost0( a ) ? GetPolynomialType( b, c, d ) : PolynomialType.Cubic;

		/// <summary>Given ax²+bx+c, returns the net polynomial type/degree, accounting for values very close to 0</summary>
		[MethodImpl( INLINE )] public static PolynomialType GetPolynomialType( float a, float b, float c ) => FactorAlmost0( a ) ? GetPolynomialType( b, c ) : PolynomialType.Quadratic;

		/// <summary>Given ax+b, returns the net polynomial type/degree, accounting for values very close to 0</summary>
		[MethodImpl( INLINE )] public static PolynomialType GetPolynomialType( float a, float b ) => FactorAlmost0( a ) ? PolynomialType.Constant : PolynomialType.Linear;

		/// <summary>Returns the root/solution of ax+b = 0. Returns null if there is no root</summary>
		public static float? GetLinearRoots( float a, float b ) {
			if( GetPolynomialType( a, b ) == PolynomialType.Constant )
				return null;
			return -b / a;
		}

		/// <summary>Returns the roots/solutions of ax²+bx+c = 0. There's either 0, 1 or 2 roots, filled in left to right among the return values</summary>
		public static ResultsMax2<float> GetQuadraticRoots( float a, float b, float c ) {
			switch( GetPolynomialType( a, b, c ) ) {
				case PolynomialType.Constant:  return default; // either no roots or infinite roots if c == 0
				case PolynomialType.Linear:    return new ResultsMax2<float>( SolveLinearRoot( b, c ) );
				case PolynomialType.Quadratic: return SolveQuadraticRoots( a, b, c );
				default:                       throw new InvalidEnumArgumentException();
			}
		}

		/// <summary>Returns the roots/solutions of ax³+bx²+cx+d = 0. There's either 0, 1, 2 or 3 roots, filled in left to right among the return values</summary>
		public static ResultsMax3<float> GetCubicRoots( float a, float b, float c, float d ) {
			switch( GetPolynomialType( a, b, c, d ) ) {
				case PolynomialType.Constant:  return default; // either no roots or infinite roots if c == 0
				case PolynomialType.Linear:    return new ResultsMax3<float>( SolveLinearRoot( c, d ) );
				case PolynomialType.Quadratic: return SolveQuadraticRoots( b, c, d );
				case PolynomialType.Cubic:     return SolveCubicRoots( a, b, c, d );
				default:                       throw new InvalidEnumArgumentException();
			}
		}

		#region Internal root solvers

		// These functions lack safety checks (division by zero etc.) for lower degree equivalency - they presume "a" is always nonzero.
		// These are private to avoid people mistaking them for the more stable/safe functions you are more likely to want to use

		[MethodImpl( INLINE )] static float SolveLinearRoot( float a, float b ) => -b / a;

		static ResultsMax2<float> SolveQuadraticRoots( float a, float b, float c ) {
			float rootContent = b * b - 4 * a * c;
			if( FactorAlmost0( rootContent ) )
				return new ResultsMax2<float>( -b / ( 2 * a ) ); // two equivalent solutions at one point

			if( rootContent >= 0 ) {
				float root = Sqrt( rootContent );
				float r0 = ( -b - root ) / ( 2 * a ); // crosses at two points
				float r1 = ( -b + root ) / ( 2 * a );
				return new ResultsMax2<float>( Min( r0, r1 ), Max( r0, r1 ) );
			}

			return default; // no roots
		}

		static ResultsMax3<float> SolveCubicRoots( float a, float b, float c, float d ) {
			// first, depress the cubic to make it easier to solve
			float p = ( 3 * a * c - b * b ) / ( 3 * a * a );
			float q = ( 2 * b * b * b - 9 * a * b * c + 27 * a * a * d ) / ( 27 * a * a * a );

			ResultsMax3<float> dpr = SolveDepressedCubicRoots( p, q );

			// we now have the roots of the depressed cubic, now convert back to the normal cubic
			float UndepressRoot( float r ) => r - b / ( 3 * a );
			switch( dpr.count ) {
				case 1:  return new ResultsMax3<float>( UndepressRoot( dpr.a ) );
				case 2:  return new ResultsMax3<float>( UndepressRoot( dpr.a ), UndepressRoot( dpr.b ) );
				case 3:  return new ResultsMax3<float>( UndepressRoot( dpr.a ), UndepressRoot( dpr.b ), UndepressRoot( dpr.c ) );
				default: return default;
			}
		}

		// t³+pt+q = 0
		static ResultsMax3<float> SolveDepressedCubicRoots( float p, float q ) {
			if( FactorAlmost0( p ) ) // triple root - one solution. solve x³+q = 0 => x = cr(-q)
				return new ResultsMax3<float>( Cbrt( -q ) );
			float discriminant = 4 * p * p * p + 27 * q * q;
			if( discriminant < 0.00001 ) { // two or three roots guaranteed, use trig solution
				float pre = 2 * Sqrt( -p / 3 );
				float acosInner = ( ( 3 * q ) / ( 2 * p ) ) * Sqrt( -3 / p );

				float GetRoot( int k ) => pre * Cos( ( 1f / 3f ) * Acos( acosInner.ClampNeg1to1() ) - ( TAU / 3f ) * k );
				// if acos hits 0 or TAU/2, the offsets will have the same value,
				// which means we have a double root plus one regular root on our hands
				if( acosInner >= 0.9999f )
					return new ResultsMax3<float>( GetRoot( 0 ), GetRoot( 2 ) ); // two roots - one single and one double root
				if( acosInner <= -0.9999f )
					return new ResultsMax3<float>( GetRoot( 1 ), GetRoot( 2 ) ); // two roots - one single and one double root
				return new ResultsMax3<float>( GetRoot( 0 ), GetRoot( 1 ), GetRoot( 2 ) ); // three roots
			}

			if( discriminant > 0 && p < 0 ) { // one root
				float coshInner = ( 1f / 3f ) * Acosh( ( -3 * q.Abs() / ( 2 * p ) ) * Sqrt( -3 / p ) );
				float r = -2 * Sign( q ) * Sqrt( -p / 3 ) * Cosh( coshInner );
				return new ResultsMax3<float>( r );
			}

			if( p > 0 ) { // one root
				float sinhInner = ( 1f / 3f ) * Asinh( ( ( 3 * q ) / ( 2 * p ) ) * Sqrt( 3 / p ) );
				float r = ( -2 * Sqrt( p / 3 ) ) * Sinh( sinhInner );
				return new ResultsMax3<float>( r );
			}

			// no roots
			return default;
		}

		#endregion

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