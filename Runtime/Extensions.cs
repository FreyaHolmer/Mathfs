// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using System.Runtime.CompilerServices;
using UnityEngine;


namespace Freya {

	/// <summary>Various extensions for floats, vectors and colors</summary>
	public static class MathfsExtensions {

		const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

		#region Vector rotation and angles

		/// <summary>Returns the angle of this vector, in radians</summary>
		/// <param name="v">The vector to get the angle of. It does not have to be normalized</param>
		/// <seealso cref="Mathfs.DirToAng"/>
		[MethodImpl( INLINE )] public static float Angle( this Vector2 v ) => Mathf.Atan2( v.y, v.x );

		/// <summary>Rotates the vector 90 degrees clockwise (negative Z axis rotation)</summary>
		[MethodImpl( INLINE )] public static Vector2 Rotate90CW( this Vector2 v ) => new Vector2( v.y, -v.x );

		/// <summary>Rotates the vector 90 degrees counter-clockwise (positive Z axis rotation)</summary>
		[MethodImpl( INLINE )] public static Vector2 Rotate90CCW( this Vector2 v ) => new Vector2( -v.y, v.x );

		/// <summary>Rotates the vector around <c>pivot</c> with the given angle (in radians)</summary>
		/// <param name="v">The vector to rotate</param>
		/// <param name="pivot">The point to rotate around</param>
		/// <param name="angRad">The angle to rotate by, in radians</param>
		[MethodImpl( INLINE )] public static Vector2 RotateAround( this Vector2 v, Vector2 pivot, float angRad ) => Rotate( v - pivot, angRad ) + pivot;

		/// <summary>Rotates the vector around <c>(0,0)</c> with the given angle (in radians)</summary>
		/// <param name="v">The vector to rotate</param>
		/// <param name="angRad">The angle to rotate by, in radians</param>
		public static Vector2 Rotate( this Vector2 v, float angRad ) {
			float ca = Mathf.Cos( angRad );
			float sa = Mathf.Sin( angRad );
			return new Vector2( ca * v.x - sa * v.y, sa * v.x + ca * v.y );
		}

		/// <summary>Converts an angle in degrees to radians</summary>
		/// <param name="angDegrees">The angle, in degrees, to convert to radians</param>
		[MethodImpl( INLINE )] public static float DegToRad( this float angDegrees ) => angDegrees * Mathfs.Deg2Rad;

		/// <summary>Converts an angle in radians to degrees</summary>
		/// <param name="angRadians">The angle, in radians, to convert to degrees</param>
		[MethodImpl( INLINE )] public static float RadToDeg( this float angRadians ) => angRadians * Mathfs.Rad2Deg;

		/// <summary>Extracts the quaternion components into a Vector4</summary>
		/// <param name="q">The quaternion to get the components of</param>
		[MethodImpl( INLINE )] public static Vector4 ToVector4( this Quaternion q ) => new Vector4( q.x, q.y, q.z, q.w );

		#endregion

		#region Swizzling

		/// <summary>Returns X and Y as a Vector2, equivalent to <c>new Vector2(v.x,v.y)</c></summary>
		[MethodImpl( INLINE )] public static Vector2 XY( this Vector2 v ) => new Vector2( v.y, v.x );

		/// <summary>Returns Y and X as a Vector2, equivalent to <c>new Vector2(v.y,v.x)</c></summary>
		[MethodImpl( INLINE )] public static Vector2 YX( this Vector2 v ) => new Vector2( v.y, v.x );

		/// <summary>Returns X and Z as a Vector2, equivalent to <c>new Vector2(v.x,v.z)</c></summary>
		[MethodImpl( INLINE )] public static Vector2 XZ( this Vector3 v ) => new Vector2( v.x, v.z );

		/// <summary>Returns this vector as a Vector3, slotting X into X, and Y into Z, and the input value y into Y.
		/// Equivalent to <c>new Vector3(v.x,y,v.y)</c></summary>
		[MethodImpl( INLINE )] public static Vector3 XZtoXYZ( this Vector2 v, float y = 0 ) => new Vector3( v.x, y, v.y );

		/// <summary>Returns this vector as a Vector3, slotting X into X, and Y into Y, and the input value z into Z.
		/// Equivalent to <c>new Vector3(v.x,v.y,z)</c></summary>
		[MethodImpl( INLINE )] public static Vector3 XYtoXYZ( this Vector2 v, float z = 0 ) => new Vector3( v.x, v.y, z );

		/// <summary>Sets X to 0</summary>
		[MethodImpl( INLINE )] public static Vector2 FlattenX( this Vector2 v ) => new Vector2( 0f, v.y );

		/// <summary>Sets Y to 0</summary>
		[MethodImpl( INLINE )] public static Vector2 FlattenY( this Vector2 v ) => new Vector2( v.x, 0f );

		/// <summary>Sets X to 0</summary>
		[MethodImpl( INLINE )] public static Vector3 FlattenX( this Vector3 v ) => new Vector3( 0f, v.y, v.z );

		/// <summary>Sets Y to 0</summary>
		[MethodImpl( INLINE )] public static Vector3 FlattenY( this Vector3 v ) => new Vector3( v.x, 0f, v.z );

		/// <summary>Sets Z to 0</summary>
		[MethodImpl( INLINE )] public static Vector3 FlattenZ( this Vector3 v ) => new Vector3( v.x, v.y, 0f );

		#endregion

		#region Vector directions & magnitudes

		/// <summary>Returns a vector with the same direction, but with the given magnitude.
		/// Equivalent to <c>v.normalized*mag</c></summary>
		[MethodImpl( INLINE )] public static Vector2 WithMagnitude( this Vector2 v, float mag ) => v.normalized * mag;

		/// <summary>Returns a vector with the same direction, but with the given magnitude.
		/// Equivalent to <c>v.normalized*mag</c></summary>
		[MethodImpl( INLINE )] public static Vector3 WithMagnitude( this Vector3 v, float mag ) => v.normalized * mag;

		/// <summary>Returns the vector going from one position to another, also known as the displacement.
		/// Equivalent to <c>target-v</c></summary>
		[MethodImpl( INLINE )] public static Vector2 To( this Vector2 v, Vector2 target ) => target - v;

		/// <summary>Returns the vector going from one position to another, also known as the displacement.
		/// Equivalent to <c>target-v</c></summary>
		[MethodImpl( INLINE )] public static Vector3 To( this Vector3 v, Vector3 target ) => target - v;

		/// <summary>Returns the normalized direction from this vector to the target.
		/// Equivalent to <c>(target-v).normalized</c> or <c>v.To(target).normalized</c></summary>
		[MethodImpl( INLINE )] public static Vector2 DirTo( this Vector2 v, Vector2 target ) => ( target - v ).normalized;

		/// <summary>Returns the normalized direction from this vector to the target.
		/// Equivalent to <c>(target-v).normalized</c> or <c>v.To(target).normalized</c></summary>
		[MethodImpl( INLINE )] public static Vector3 DirTo( this Vector3 v, Vector3 target ) => ( target - v ).normalized;

		#endregion

		#region Color manipulation

		/// <summary>Returns the same color, but with the specified alpha value</summary>
		/// <param name="c">The source color</param>
		/// <param name="a">The new alpha value</param>
		[MethodImpl( INLINE )] public static Color WithAlpha( this Color c, float a ) => new Color( c.r, c.g, c.b, a );

		/// <summary>Returns the same color and alpha, but with RGB multiplied by the given value</summary>
		/// <param name="c">The source color</param>
		/// <param name="m">The multiplier for the RGB channels</param>
		[MethodImpl( INLINE )] public static Color MultiplyRGB( this Color c, float m ) => new Color( c.r * m, c.g * m, c.b * m, c.a );

		/// <summary>Returns the same color and alpha, but with the RGB values multiplief by another color</summary>
		/// <param name="c">The source color</param>
		/// <param name="m">The color to multiply RGB by</param>
		[MethodImpl( INLINE )] public static Color MultiplyRGB( this Color c, Color m ) => new Color( c.r * m.r, c.g * m.g, c.b * m.b, c.a );

		/// <summary>Returns the same color, but with the alpha channel multiplied by the given value</summary>
		/// <param name="c">The source color</param>
		/// <param name="m">The multiplier for the alpha</param>
		[MethodImpl( INLINE )] public static Color MultiplyA( this Color c, float m ) => new Color( c.r, c.g, c.b, c.a * m );

		#endregion

		#region Rect

		/// <summary>Expands the rectangle to encapsulate the point <c>p</c></summary>
		/// <param name="r">The rectangle to expand</param>
		/// <param name="p">The point to encapsulate</param>
		public static Rect Encapsulate( this Rect r, Vector2 p ) {
			r.xMax = Mathf.Max( r.xMax, p.x );
			r.xMin = Mathf.Min( r.xMin, p.x );
			r.yMax = Mathf.Max( r.yMax, p.y );
			r.yMin = Mathf.Min( r.yMin, p.y );
			return r;
		}

		#endregion

		#region Simple float and int operations

		/// <summary>Returns true if v is between or equal to <c>min</c> &amp; <c>max</c></summary>
		/// <seealso cref="Between(float,float,float)"/>
		[MethodImpl( INLINE )] public static bool Within( this float v, float min, float max ) => v >= min && v <= max;

		/// <summary>Returns true if v is between or equal to <c>min</c> &amp; <c>max</c></summary>
		/// <seealso cref="Between(int,int,int)"/>
		[MethodImpl( INLINE )] public static bool Within( this int v, int min, int max ) => v >= min && v <= max;

		/// <summary>Returns true if v is between, but not equal to, <c>min</c> &amp; <c>max</c></summary>
		/// <seealso cref="Within(float,float,float)"/>
		[MethodImpl( INLINE )] public static bool Between( this float v, float min, float max ) => v > min && v < max;

		/// <summary>Returns true if v is between, but not equal to, <c>min</c> &amp; <c>max</c></summary>
		/// <seealso cref="Within(int,int,int)"/>
		[MethodImpl( INLINE )] public static bool Between( this int v, int min, int max ) => v > min && v < max;

		/// <summary>Clamps the value to be at least <c>min</c></summary>
		[MethodImpl( INLINE )] public static float AtLeast( this float v, float min ) => v < min ? min : v;

		/// <summary>Clamps the value to be at least <c>min</c></summary>
		[MethodImpl( INLINE )] public static int AtLeast( this int v, int min ) => v < min ? min : v;

		/// <summary>Clamps the value to be at most <c>max</c></summary>
		[MethodImpl( INLINE )] public static float AtMost( this float v, float max ) => v > max ? max : v;

		/// <summary>Clamps the value to be at most <c>max</c></summary>
		[MethodImpl( INLINE )] public static int AtMost( this int v, int max ) => v > max ? max : v;

		/// <summary>Squares the value. Equivalent to <c>v*v</c></summary>
		[MethodImpl( INLINE )] public static float Square( this float v ) => v * v;

		/// <summary>Cubes the value. Equivalent to <c>v*v*v</c></summary>
		[MethodImpl( INLINE )] public static float Cube( this float v ) => v * v * v;

		/// <summary>Squares the value. Equivalent to <c>v*v</c></summary>
		[MethodImpl( INLINE )] public static int Square( this int v ) => v * v;

		/// <summary>The next integer, modulo <c>length</c>. Behaves the way you want with negative values for stuff like array index access etc</summary>
		[MethodImpl( INLINE )] public static int NextMod( this int value, int length ) => ( value + 1 ).Mod( length );

		/// <summary>The previous integer, modulo <c>length</c>. Behaves the way you want with negative values for stuff like array index access etc</summary>
		[MethodImpl( INLINE )] public static int PrevMod( this int value, int length ) => ( value - 1 ).Mod( length );

		#endregion

		#region Extension method counterparts of the static Mathfs functions - lots of boilerplate in here

		#region Math operations

		/// <inheritdoc cref="Mathfs.Sqrt(float)"/>
		[MethodImpl( INLINE )] public static float Sqrt( this float value ) => Mathfs.Sqrt( value );

		/// <inheritdoc cref="Mathfs.Sqrt(Vector2)"/>
		[MethodImpl( INLINE )] public static Vector2 Sqrt( this Vector2 value ) => Mathfs.Sqrt( value );

		/// <inheritdoc cref="Mathfs.Sqrt(Vector3)"/>
		[MethodImpl( INLINE )] public static Vector3 Sqrt( this Vector3 value ) => Mathfs.Sqrt( value );

		/// <inheritdoc cref="Mathfs.Sqrt(Vector4)"/>
		[MethodImpl( INLINE )] public static Vector4 Sqrt( this Vector4 value ) => Mathfs.Sqrt( value );

		/// <inheritdoc cref="Mathfs.Cbrt(float)"/>
		[MethodImpl( INLINE )] public static float Cbrt( this float value ) => Mathfs.Cbrt( value );

		/// <inheritdoc cref="Mathfs.Pow(float, float)"/>
		[MethodImpl( INLINE )] public static float Pow( this float value, float exponent ) => Mathfs.Pow( value, exponent );

		#endregion

		#region Absolute Values

		/// <inheritdoc cref="Mathfs.Abs(float)"/>
		[MethodImpl( INLINE )] public static float Abs( this float value ) => Mathfs.Abs( value );

		/// <inheritdoc cref="Mathfs.Abs(int)"/>
		[MethodImpl( INLINE )] public static int Abs( this int value ) => Mathfs.Abs( value );

		/// <inheritdoc cref="Mathfs.Abs(Vector2)"/>
		[MethodImpl( INLINE )] public static Vector2 Abs( this Vector2 v ) => Mathfs.Abs( v );

		/// <inheritdoc cref="Mathfs.Abs(Vector3)"/>
		[MethodImpl( INLINE )] public static Vector3 Abs( this Vector3 v ) => Mathfs.Abs( v );

		/// <inheritdoc cref="Mathfs.Abs(Vector4)"/>
		[MethodImpl( INLINE )] public static Vector4 Abs( this Vector4 v ) => Mathfs.Abs( v );

		#endregion

		#region Clamping

		/// <inheritdoc cref="Mathfs.Clamp(float,float,float)"/>
		[MethodImpl( INLINE )] public static float Clamp( this float value, float min, float max ) => Mathfs.Clamp( value, min, max );

		/// <inheritdoc cref="Mathfs.Clamp(Vector2,Vector2,Vector2)"/>
		[MethodImpl( INLINE )] public static Vector2 Clamp( this Vector2 v, Vector2 min, Vector2 max ) => Mathfs.Clamp( v, min, max );

		/// <inheritdoc cref="Mathfs.Clamp(Vector3,Vector3,Vector3)"/>
		[MethodImpl( INLINE )] public static Vector3 Clamp( this Vector3 v, Vector3 min, Vector3 max ) => Mathfs.Clamp( v, min, max );

		/// <inheritdoc cref="Mathfs.Clamp(Vector4,Vector4,Vector4)"/>
		[MethodImpl( INLINE )] public static Vector4 Clamp( this Vector4 v, Vector4 min, Vector4 max ) => Mathfs.Clamp( v, min, max );

		/// <inheritdoc cref="Mathfs.Clamp(int,int,int)"/>
		[MethodImpl( INLINE )] public static int Clamp( this int value, int min, int max ) => Mathfs.Clamp( value, min, max );

		/// <inheritdoc cref="Mathfs.Clamp01(float)"/>
		[MethodImpl( INLINE )] public static float Clamp01( this float value ) => Mathfs.Clamp01( value );

		/// <inheritdoc cref="Mathfs.Clamp01(Vector2)"/>
		[MethodImpl( INLINE )] public static Vector2 Clamp01( this Vector2 v ) => Mathfs.Clamp01( v );

		/// <inheritdoc cref="Mathfs.Clamp01(Vector3)"/>
		[MethodImpl( INLINE )] public static Vector3 Clamp01( this Vector3 v ) => Mathfs.Clamp01( v );

		/// <inheritdoc cref="Mathfs.Clamp01(Vector4)"/>
		[MethodImpl( INLINE )] public static Vector4 Clamp01( this Vector4 v ) => Mathfs.Clamp01( v );

		/// <inheritdoc cref="Mathfs.ClampNeg1to1(float)"/>
		[MethodImpl( INLINE )] public static float ClampNeg1to1( this float value ) => Mathfs.ClampNeg1to1( value );

		/// <inheritdoc cref="Mathfs.ClampNeg1to1(Vector2)"/>
		[MethodImpl( INLINE )] public static Vector2 ClampNeg1to1( this Vector2 v ) => Mathfs.ClampNeg1to1( v );

		/// <inheritdoc cref="Mathfs.ClampNeg1to1(Vector3)"/>
		[MethodImpl( INLINE )] public static Vector3 ClampNeg1to1( this Vector3 v ) => Mathfs.ClampNeg1to1( v );

		/// <inheritdoc cref="Mathfs.ClampNeg1to1(Vector4)"/>
		[MethodImpl( INLINE )] public static Vector4 ClampNeg1to1( this Vector4 v ) => Mathfs.ClampNeg1to1( v );

		#endregion

		#region Min & Max

		/// <inheritdoc cref="Mathfs.Min(Vector2)"/>
		[MethodImpl( INLINE )] public static float Min( this Vector2 v ) => Mathfs.Min( v );

		/// <inheritdoc cref="Mathfs.Min(Vector3)"/>
		[MethodImpl( INLINE )] public static float Min( this Vector3 v ) => Mathfs.Min( v );

		/// <inheritdoc cref="Mathfs.Min(Vector4)"/>
		[MethodImpl( INLINE )] public static float Min( this Vector4 v ) => Mathfs.Min( v );

		/// <inheritdoc cref="Mathfs.Max(Vector2)"/>
		[MethodImpl( INLINE )] public static float Max( this Vector2 v ) => Mathfs.Max( v );

		/// <inheritdoc cref="Mathfs.Max(Vector3)"/>
		[MethodImpl( INLINE )] public static float Max( this Vector3 v ) => Mathfs.Max( v );

		/// <inheritdoc cref="Mathfs.Max(Vector4)"/>
		[MethodImpl( INLINE )] public static float Max( this Vector4 v ) => Mathfs.Max( v );

		#endregion

		#region Signs & Rounding

		/// <inheritdoc cref="Mathfs.Sign(float)"/>
		[MethodImpl( INLINE )] public static float Sign( this float value ) => Mathfs.Sign( value );

		/// <inheritdoc cref="Mathfs.Sign(Vector2)"/>
		[MethodImpl( INLINE )] public static Vector2 Sign( this Vector2 value ) => Mathfs.Sign( value );

		/// <inheritdoc cref="Mathfs.Sign(Vector3)"/>
		[MethodImpl( INLINE )] public static Vector3 Sign( this Vector3 value ) => Mathfs.Sign( value );

		/// <inheritdoc cref="Mathfs.Sign(Vector4)"/>
		[MethodImpl( INLINE )] public static Vector4 Sign( this Vector4 value ) => Mathfs.Sign( value );

		/// <inheritdoc cref="Mathfs.Sign(int)"/>
		[MethodImpl( INLINE )] public static int Sign( this int value ) => Mathfs.Sign( value );

		/// <inheritdoc cref="Mathfs.SignAsInt(float)"/>
		[MethodImpl( INLINE )] public static int SignAsInt( this float value ) => Mathfs.SignAsInt( value );

		/// <inheritdoc cref="Mathfs.SignWithZero(float,float)"/>
		[MethodImpl( INLINE )] public static float SignWithZero( this float value, float zeroThreshold = 0.000001f ) => Mathfs.SignWithZero( value, zeroThreshold );

		/// <inheritdoc cref="Mathfs.SignWithZero(Vector2,float)"/>
		[MethodImpl( INLINE )] public static Vector2 SignWithZero( this Vector2 value, float zeroThreshold = 0.000001f ) => Mathfs.SignWithZero( value, zeroThreshold );

		/// <inheritdoc cref="Mathfs.SignWithZero(Vector3,float)"/>
		[MethodImpl( INLINE )] public static Vector3 SignWithZero( this Vector3 value, float zeroThreshold = 0.000001f ) => Mathfs.SignWithZero( value, zeroThreshold );

		/// <inheritdoc cref="Mathfs.SignWithZero(Vector4,float)"/>
		[MethodImpl( INLINE )] public static Vector4 SignWithZero( this Vector4 value, float zeroThreshold = 0.000001f ) => Mathfs.SignWithZero( value, zeroThreshold );

		/// <inheritdoc cref="Mathfs.SignWithZero(int)"/>
		[MethodImpl( INLINE )] public static int SignWithZero( this int value ) => Mathfs.SignWithZero( value );

		/// <inheritdoc cref="Mathfs.SignWithZeroAsInt(float,float)"/>
		[MethodImpl( INLINE )] public static int SignWithZeroAsInt( this float value, float zeroThreshold = 0.000001f ) => Mathfs.SignWithZeroAsInt( value, zeroThreshold );

		/// <inheritdoc cref="Mathfs.Floor(float)"/>
		[MethodImpl( INLINE )] public static float Floor( this float value ) => Mathfs.Floor( value );

		/// <inheritdoc cref="Mathfs.Floor(Vector2)"/>
		[MethodImpl( INLINE )] public static Vector2 Floor( this Vector2 value ) => Mathfs.Floor( value );

		/// <inheritdoc cref="Mathfs.Floor(Vector2)"/>
		[MethodImpl( INLINE )] public static Vector3 Floor( this Vector3 value ) => Mathfs.Floor( value );

		/// <inheritdoc cref="Mathfs.Floor(Vector2)"/>
		[MethodImpl( INLINE )] public static Vector4 Floor( this Vector4 value ) => Mathfs.Floor( value );

		/// <inheritdoc cref="Mathfs.FloorToInt(float)"/>
		[MethodImpl( INLINE )] public static int FloorToInt( this float value ) => Mathfs.FloorToInt( value );

		/// <inheritdoc cref="Mathfs.FloorToInt(Vector2)"/>
		[MethodImpl( INLINE )] public static Vector2Int FloorToInt( this Vector2 value ) => Mathfs.FloorToInt( value );

		/// <inheritdoc cref="Mathfs.FloorToInt(Vector2)"/>
		[MethodImpl( INLINE )] public static Vector3Int FloorToInt( this Vector3 value ) => Mathfs.FloorToInt( value );

		/// <inheritdoc cref="Mathfs.Ceil(float)"/>
		[MethodImpl( INLINE )] public static float Ceil( this float value ) => Mathfs.Ceil( value );

		/// <inheritdoc cref="Mathfs.Ceil(Vector2)"/>
		[MethodImpl( INLINE )] public static Vector2 Ceil( this Vector2 value ) => Mathfs.Ceil( value );

		/// <inheritdoc cref="Mathfs.Ceil(Vector2)"/>
		[MethodImpl( INLINE )] public static Vector3 Ceil( this Vector3 value ) => Mathfs.Ceil( value );

		/// <inheritdoc cref="Mathfs.Ceil(Vector2)"/>
		[MethodImpl( INLINE )] public static Vector4 Ceil( this Vector4 value ) => Mathfs.Ceil( value );

		/// <inheritdoc cref="Mathfs.CeilToInt(float)"/>
		[MethodImpl( INLINE )] public static int CeilToInt( this float value ) => Mathfs.CeilToInt( value );

		/// <inheritdoc cref="Mathfs.CeilToInt(Vector2)"/>
		[MethodImpl( INLINE )] public static Vector2Int CeilToInt( this Vector2 value ) => Mathfs.CeilToInt( value );

		/// <inheritdoc cref="Mathfs.CeilToInt(Vector2)"/>
		[MethodImpl( INLINE )] public static Vector3Int CeilToInt( this Vector3 value ) => Mathfs.CeilToInt( value );

		/// <inheritdoc cref="Mathfs.Round(float)"/>
		[MethodImpl( INLINE )] public static float Round( this float value ) => Mathfs.Round( value );

		/// <inheritdoc cref="Mathfs.Round(Vector2)"/>
		[MethodImpl( INLINE )] public static Vector2 Round( this Vector2 value ) => Mathfs.Round( value );

		/// <inheritdoc cref="Mathfs.Round(Vector2)"/>
		[MethodImpl( INLINE )] public static Vector3 Round( this Vector3 value ) => Mathfs.Round( value );

		/// <inheritdoc cref="Mathfs.Round(Vector2)"/>
		[MethodImpl( INLINE )] public static Vector4 Round( this Vector4 value ) => Mathfs.Round( value );

		/// <inheritdoc cref="Mathfs.Round(float)"/>
		[MethodImpl( INLINE )] public static float Round( this float value, float snapInterval ) => Mathfs.Round( value, snapInterval );

		/// <inheritdoc cref="Mathfs.Round(Vector2)"/>
		[MethodImpl( INLINE )] public static Vector2 Round( this Vector2 value, float snapInterval ) => Mathfs.Round( value, snapInterval );

		/// <inheritdoc cref="Mathfs.Round(Vector2,float)"/>
		[MethodImpl( INLINE )] public static Vector3 Round( this Vector3 value, float snapInterval ) => Mathfs.Round( value, snapInterval );

		/// <inheritdoc cref="Mathfs.Round(Vector2,float)"/>
		[MethodImpl( INLINE )] public static Vector4 Round( this Vector4 value, float snapInterval ) => Mathfs.Round( value, snapInterval );

		/// <inheritdoc cref="Mathfs.RoundToInt(float)"/>
		[MethodImpl( INLINE )] public static int RoundToInt( this float value ) => Mathfs.RoundToInt( value );

		/// <inheritdoc cref="Mathfs.RoundToInt(Vector2)"/>
		[MethodImpl( INLINE )] public static Vector2Int RoundToInt( this Vector2 value ) => Mathfs.RoundToInt( value );

		/// <inheritdoc cref="Mathfs.RoundToInt(Vector2)"/>
		[MethodImpl( INLINE )] public static Vector3Int RoundToInt( this Vector3 value ) => Mathfs.RoundToInt( value );

		#endregion

		#region Range Repeating

		/// <inheritdoc cref="Mathfs.Frac(float)"/>
		[MethodImpl( INLINE )] public static float Frac( this float x ) => Mathfs.Frac( x );

		/// <inheritdoc cref="Mathfs.Frac(Vector2)"/>
		[MethodImpl( INLINE )] public static Vector2 Frac( this Vector2 v ) => Mathfs.Frac( v );

		/// <inheritdoc cref="Mathfs.Frac(Vector3)"/>
		[MethodImpl( INLINE )] public static Vector3 Frac( this Vector3 v ) => Mathfs.Frac( v );

		/// <inheritdoc cref="Mathfs.Frac(Vector4)"/>
		[MethodImpl( INLINE )] public static Vector4 Frac( this Vector4 v ) => Mathfs.Frac( v );

		/// <inheritdoc cref="Mathfs.Repeat(float,float)"/>
		[MethodImpl( INLINE )] public static float Repeat( this float value, float length ) => Mathfs.Repeat( value, length );

		/// <inheritdoc cref="Mathfs.Mod(int,int)"/>
		[MethodImpl( INLINE )] public static int Mod( this int value, int length ) => Mathfs.Mod( value, length );

		#endregion

		#region Smoothing & Easing Curves

		/// <inheritdoc cref="Mathfs.Smooth01(float)"/>
		[MethodImpl( INLINE )] public static float Smooth01( this float x ) => Mathfs.Smooth01( x );

		/// <inheritdoc cref="Mathfs.Smoother01(float)"/>
		[MethodImpl( INLINE )] public static float Smoother01( this float x ) => Mathfs.Smoother01( x );

		/// <inheritdoc cref="Mathfs.SmoothCos01(float)"/>
		[MethodImpl( INLINE )] public static float SmoothCos01( this float x ) => Mathfs.SmoothCos01( x );

		#endregion

		#region Value & Vector interpolation

		/// <inheritdoc cref="Mathfs.Remap(float,float,float,float,float)"/>
		[MethodImpl( INLINE )] public static float Remap( this float value, float iMin, float iMax, float oMin, float oMax ) => Mathfs.Remap( iMin, iMax, oMin, oMax, value );

		/// <inheritdoc cref="Mathfs.Remap(float,float,float,float,int)"/>
		[MethodImpl( INLINE )] public static float Remap( this int value, float iMin, float iMax, float oMin, float oMax ) => Mathfs.Lerp( oMin, oMax, Mathfs.InverseLerp( iMin, iMax, value ) );

		/// <inheritdoc cref="Mathfs.Remap(Vector2,Vector2,Vector2,Vector2,Vector2)"/>
		[MethodImpl( INLINE )] public static Vector2 Remap( this Vector2 v, Vector2 iMin, Vector2 iMax, Vector2 oMin, Vector2 oMax ) => Mathfs.Remap( iMin, iMax, oMin, oMax, v );

		/// <inheritdoc cref="Mathfs.Remap(Vector3,Vector3,Vector3,Vector3,Vector3)"/>
		[MethodImpl( INLINE )] public static Vector3 Remap( this Vector3 v, Vector3 iMin, Vector3 iMax, Vector3 oMin, Vector3 oMax ) => Mathfs.Remap( iMin, iMax, oMin, oMax, v );

		/// <inheritdoc cref="Mathfs.Remap(Vector4,Vector4,Vector4,Vector4,Vector4)"/>
		[MethodImpl( INLINE )] public static Vector4 Remap( this Vector4 v, Vector4 iMin, Vector4 iMax, Vector4 oMin, Vector4 oMax ) => Mathfs.Lerp( oMin, oMax, Mathfs.InverseLerp( iMin, iMax, v ) );

		/// <inheritdoc cref="Mathfs.Remap(Rect,Rect,Vector2)"/>
		[MethodImpl( INLINE )] public static Vector2 Remap( this Vector2 iPos, Rect iRect, Rect oRect ) => Mathfs.Remap( iRect.min, iRect.max, oRect.min, oRect.max, iPos );

		/// <inheritdoc cref="Mathfs.Remap(Bounds,Bounds,Vector3)"/>
		[MethodImpl( INLINE )] public static Vector3 Remap( this Vector3 iPos, Bounds iBounds, Bounds oBounds ) => Mathfs.Remap( iBounds.min, iBounds.max, oBounds.min, oBounds.max, iPos );

		/// <inheritdoc cref="Mathfs.Remap(float,float,float,float,float)"/>
		[MethodImpl( INLINE )] public static float RemapClamped( this float value, float iMin, float iMax, float oMin, float oMax ) => Mathfs.Lerp( oMin, oMax, Mathfs.InverseLerpClamped( iMin, iMax, value ) );

		#endregion

		#region Vector Math

		/// <inheritdoc cref="Mathfs.GetDirAndMagnitude(Vector2)"/>
		[MethodImpl( INLINE )] public static (Vector2 dir, float magnitude ) GetDirAndMagnitude( this Vector2 v ) => Mathfs.GetDirAndMagnitude( v );

		/// <inheritdoc cref="Mathfs.GetDirAndMagnitude(Vector3)"/>
		[MethodImpl( INLINE )] public static (Vector3 dir, float magnitude ) GetDirAndMagnitude( this Vector3 v ) => Mathfs.GetDirAndMagnitude( v );

		/// <inheritdoc cref="Mathfs.ClampMagnitude(Vector2,float,float)"/>
		[MethodImpl( INLINE )] public static Vector2 ClampMagnitude( this Vector2 v, float min, float max ) => Mathfs.ClampMagnitude( v, min, max );

		/// <inheritdoc cref="Mathfs.ClampMagnitude(Vector3,float,float)"/>
		[MethodImpl( INLINE )] public static Vector3 ClampMagnitude( this Vector3 v, float min, float max ) => Mathfs.ClampMagnitude( v, min, max );

		#endregion

		#endregion


	}

}