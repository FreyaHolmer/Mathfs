// collected and expended upon by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using UnityEngine;

namespace Freya {

	public static class MathfsExtensions {

		public static float Angle( this Vector2 v ) => Mathf.Atan2( v.y, v.x );
		public static Vector2 Rotate90CW( this Vector2 v ) => new Vector2( v.y, -v.x );
		public static Vector2 Rotate90CCW( this Vector2 v ) => new Vector2( -v.y, v.x );

		public static Vector2 RotateAround( this Vector2 v, Vector2 pivot, float angRad ) => Rotate( v - pivot, angRad ) + pivot;

		public static Vector2 Rotate( this Vector2 v, float angRad ) {
			float ca = Mathf.Cos( angRad );
			float sa = Mathf.Sin( angRad );
			return new Vector2( ca * v.x - sa * v.y, sa * v.x + ca * v.y );
		}

		public static Vector2 XZ( this Vector3 v ) => new Vector2( v.x, v.z );
		public static Vector3 XZtoXYZ( this Vector2 v, float y = 0 ) => new Vector3( v.x, y, v.y );

		public static float Frac( this float v ) => v - Mathfs.Floor( v );
		public static Vector2 Frac( this Vector2 v ) => v - v.Floor();
		public static Vector3 Frac( this Vector3 v ) => v - v.Floor();
		public static Vector4 Frac( this Vector4 v ) => v - v.Floor();

		public static float Clamp01( this float v ) => Mathfs.Clamp01( v );
		public static float ClampNeg1to1( this float v ) => Mathfs.ClampNeg1to1( v );
		public static Vector2 Clamp01( this Vector2 v ) => new Vector2( Mathfs.Clamp01( v.x ), Mathfs.Clamp01( v.y ) );
		public static Vector3 Clamp01( this Vector3 v ) => new Vector3( Mathfs.Clamp01( v.x ), Mathfs.Clamp01( v.y ), Mathfs.Clamp01( v.z ) );
		public static Vector4 Clamp01( this Vector4 v ) => new Vector4( Mathfs.Clamp01( v.x ), Mathfs.Clamp01( v.y ), Mathfs.Clamp01( v.z ), Mathfs.Clamp01( v.w ) );

		public static float Min( this Vector2 v ) => Mathfs.Min( v.x, v.y );
		public static float Min( this Vector3 v ) => Mathfs.Min( v.x, v.y, v.z );
		public static float Min( this Vector4 v ) => Mathfs.Min( v.x, v.y, v.z, v.w );
		public static float Max( this Vector2 v ) => Mathfs.Max( v.x, v.y );
		public static float Max( this Vector3 v ) => Mathfs.Max( v.x, v.y, v.z );
		public static float Max( this Vector4 v ) => Mathfs.Max( v.x, v.y, v.z, v.w );

		public static Vector2 WithMagnitude( this Vector2 v, float mag ) => v.normalized * mag;
		public static Vector3 WithMagnitude( this Vector3 v, float mag ) => v.normalized * mag;

		public static Vector2 ClampMagnitude( this Vector2 v, float min, float max ) {
			float mag = v.magnitude;
			if( mag < min ) {
				Vector2 dir = v / mag;
				return dir * min;
			}

			if( mag > max ) {
				Vector2 dir = v / mag;
				return dir * max;
			}

			return v;
		}

		public static Vector3 ClampMagnitude( this Vector3 v, float min, float max ) {
			float mag = v.magnitude;
			if( mag < min ) {
				Vector3 dir = v / mag;
				return dir * min;
			}

			if( mag > max ) {
				Vector3 dir = v / mag;
				return dir * max;
			}

			return v;
		}

		public static Vector3 FlattenY( this Vector3 v ) => new Vector3( v.x, 0f, v.z );

		public static Vector2 To( this Vector2 v, Vector2 target ) => target - v;
		public static Vector3 To( this Vector3 v, Vector3 target ) => target - v;
		public static Vector2 DirTo( this Vector2 v, Vector2 target ) => ( target - v ).normalized;
		public static Vector3 DirTo( this Vector3 v, Vector3 target ) => ( target - v ).normalized;

		public static Vector2 Floor( this Vector2 v ) => new Vector2( Mathfs.Floor( v.x ), Mathfs.Floor( v.y ) );
		public static Vector3 Floor( this Vector3 v ) => new Vector3( Mathfs.Floor( v.x ), Mathfs.Floor( v.y ), Mathfs.Floor( v.z ) );
		public static Vector4 Floor( this Vector4 v ) => new Vector4( Mathfs.Floor( v.x ), Mathfs.Floor( v.y ), Mathfs.Floor( v.z ), Mathfs.Floor( v.w ) );
		public static Vector2 Ceil( this Vector2 v ) => new Vector2( Mathfs.Ceil( v.x ), Mathfs.Ceil( v.y ) );
		public static Vector3 Ceil( this Vector3 v ) => new Vector3( Mathfs.Ceil( v.x ), Mathfs.Ceil( v.y ), Mathfs.Ceil( v.z ) );
		public static Vector4 Ceil( this Vector4 v ) => new Vector4( Mathfs.Ceil( v.x ), Mathfs.Ceil( v.y ), Mathfs.Ceil( v.z ), Mathfs.Ceil( v.w ) );
		public static Vector2 Round( this Vector2 v ) => new Vector2( Mathfs.Round( v.x ), Mathfs.Round( v.y ) );
		public static Vector2Int RoundToInt( this Vector2 v ) => new Vector2Int( Mathf.RoundToInt( v.x ), Mathf.RoundToInt( v.y ) );
		public static Vector3 Round( this Vector3 v ) => new Vector3( Mathfs.Round( v.x ), Mathfs.Round( v.y ), Mathfs.Round( v.z ) );
		public static Vector3Int RoundToInt( this Vector3 v ) => new Vector3Int( Mathf.RoundToInt( v.x ), Mathf.RoundToInt( v.y ), Mathf.RoundToInt( v.z ) );
		public static Vector4 Round( this Vector4 v ) => new Vector4( Mathfs.Round( v.x ), Mathfs.Round( v.y ), Mathfs.Round( v.z ), Mathfs.Round( v.w ) );

		public static Color WithAlpha( this Color c, float a ) => new Color( c.r, c.g, c.b, a );
		public static Color MultiplyRGB( this Color c, float m ) => new Color( c.r * m, c.g * m, c.b * m, c.a );
		public static Color MultiplyRGB( this Color c, Color m ) => new Color( c.r * m.r, c.g * m.g, c.b * m.b, c.a );
		public static Color MultiplyA( this Color c, float m ) => new Color( c.r, c.g, c.b, c.a * m );

		public static float Round( this float v, float snapInterval ) => Mathfs.Round( v, snapInterval );
		public static bool Within( this float v, float min, float max ) => v >= min && v <= max;
		public static bool Between( this float v, float min, float max ) => v > min && v < max;
		public static float AtLeast( this float v, float min ) => Mathfs.Max( v, min );
		public static float AtMost( this float v, float max ) => Mathfs.Min( v, max );

		public static float Square( this float v ) => v * v;
		public static float Abs( this float v ) => Mathfs.Abs( v );
		public static float Magnitude( this float v ) => Mathfs.Abs( v );
		public static float Clamp( this float v, float min, float max ) => Mathfs.Clamp( v, min, max );
		public static float Remap( this float v, float iMin, float iMax, float oMin, float oMax ) => Mathfs.Remap( iMin, iMax, oMin, oMax, v );
		public static float Repeat( this float v, float length ) => Mathfs.Repeat( v, length );
		public static int Mod( this int value, int length ) => ( value % length + length ) % length; // modulo
		public static int NextMod( this int value, int length ) => Mod( value + 1, length );
		public static int PrevMod( this int value, int length ) => Mod( value - 1, length );
		public static int RoundToInt( this float v ) => Mathfs.RoundToInt( v );
		public static int FloorToInt( this float v ) => Mathfs.FloorToInt( v );
		public static int CeilToInt( this float v ) => Mathfs.CeilToInt( v );


	}

}