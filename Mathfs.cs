// Lots of this code is similar to Unity's original Mathf source to match functionality.
// The original Mathf.cs source https://github.com/Unity-Technologies/UnityCsReference/blob/master/Runtime/Export/Math/Mathf.cs
// ...and the trace amounts of it left in here is copyright (c) Unity Technologies with license: https://unity3d.com/legal/licenses/Unity_Reference_Only_License
// 
// Collected and expanded upon to by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using System;
using System.Collections.Generic;
using UnityEngine;
using Uei = UnityEngine.Internal;
using System.Linq; // used for arbitrary count min/max functions, so it's safe and won't allocate garbage don't worry~

namespace Freya {

	public static class Mathfs {

		// Constants
		public const float TAU = 6.28318530717959f;
		public const float PI = 3.14159265359f;
		public const float E = 2.71828182846f;
		public const float GOLDEN_RATIO = 1.61803398875f;
		public const float SQRT2 = 1.41421356237f;
		public const float Infinity = Single.PositiveInfinity;
		public const float NegativeInfinity = Single.NegativeInfinity;
		public const float Deg2Rad = TAU / 360f;
		public const float Rad2Deg = 360f / TAU;
		public static readonly float Epsilon = UnityEngineInternal.MathfInternal.IsFlushToZeroEnabled ? UnityEngineInternal.MathfInternal.FloatMinNormal : UnityEngineInternal.MathfInternal.FloatMinDenormal;

		// Math operations
		public static float Sqrt( float value ) => (float)Math.Sqrt( value );
		public static float Pow( float @base, float exponent ) => (float)Math.Pow( @base, exponent );
		public static float Exp( float power ) => (float)Math.Exp( power );
		public static float Log( float value, float @base ) => (float)Math.Log( value, @base );
		public static float Log( float value ) => (float)Math.Log( value );
		public static float Log10( float value ) => (float)Math.Log10( value );
		public static bool Approximately( float a, float b ) => Abs( b - a ) < Max( 0.000001f * Max( Abs( a ), Abs( b ) ), Epsilon * 8 );

		// Trig
		public static float Sin( float angRad ) => (float)Math.Sin( angRad );
		public static float Cos( float angRad ) => (float)Math.Cos( angRad );
		public static float Tan( float angRad ) => (float)Math.Tan( angRad );
		public static float Asin( float value ) => (float)Math.Asin( value );
		public static float Acos( float value ) => (float)Math.Acos( value );
		public static float Atan( float value ) => (float)Math.Atan( value );
		public static float Atan2( float y, float x ) => (float)Math.Atan2( y, x );
		public static float Csc( float x ) => 1f / (float)Math.Sin( x );
		public static float Sec( float x ) => 1f / (float)Math.Cos( x );
		public static float Cot( float x ) => 1f / (float)Math.Tan( x );
		public static float Ver( float x ) => 1 - (float)Math.Cos( x );
		public static float Cvs( float x ) => 1 - (float)Math.Sin( x );
		public static float Crd( float x ) => 2 * (float)Math.Sin( x / 2 );

		// Absolute values
		public static float Abs( float value ) => Math.Abs( value );
		public static int Abs( int value ) => Math.Abs( value );
		public static Vector2 Abs( Vector2 v ) => new Vector2( Abs( v.x ), Abs( v.y ) );
		public static Vector3 Abs( Vector3 v ) => new Vector3( Abs( v.x ), Abs( v.y ), Abs( v.z ) );
		public static Vector4 Abs( Vector4 v ) => new Vector4( Abs( v.x ), Abs( v.y ), Abs( v.z ), Abs( v.w ) );

		// Clamping
		public static float Clamp( float value, float min, float max ) {
			if( value < min ) value = min;
			if( value > max ) value = max;
			return value;
		}

		public static int Clamp( int value, int min, int max ) {
			if( value < min ) value = min;
			if( value > max ) value = max;
			return value;
		}

		public static float Clamp01( float value ) {
			if( value < 0f ) value = 0f;
			if( value > 1f ) value = 1f;
			return value;
		}

		public static float ClampNeg1to1( float value ) {
			if( value < -1f ) value = -1f;
			if( value > 1f ) value = 1f;
			return value;
		}

		// Min & Max
		public static float Min( float a, float b ) => a < b ? a : b;
		public static float Min( float a, float b, float c ) => Min( Min( a, b ), c );
		public static float Min( float a, float b, float c, float d ) => Min( Min( a, b ), Min( c, d ) );
		public static float Max( float a, float b ) => a > b ? a : b;
		public static float Max( float a, float b, float c ) => Max( Max( a, b ), c );
		public static float Max( float a, float b, float c, float d ) => Max( Max( a, b ), Max( c, d ) );
		public static int Min( int a, int b ) => a < b ? a : b;
		public static int Min( int a, int b, int c ) => Min( Min( a, b ), c );
		public static int Max( int a, int b ) => a > b ? a : b;
		public static int Max( int a, int b, int c ) => Max( Max( a, b ), c );

		public static float Min( params float[] values ) => values.Min();
		public static float Max( params float[] values ) => values.Max();
		public static int Min( params int[] values ) => values.Min();
		public static int Max( params int[] values ) => values.Max();

		// Rounding
		public static int Sign( float value ) => value >= 0f ? 1 : -1;
		public static int Sign( int value ) => value >= 0 ? 1 : -1;
		public static int SignWithZero( int value ) => value == 0 ? 0 : Sign( value );
		public static int SignWithZero( float value, float epsilon = 0.000001f ) => Abs( value ) < epsilon ? 0 : Sign( value );
		public static float Floor( float value ) => (float)Math.Floor( value );
		public static Vector2 Floor( Vector2 value ) => new Vector2( (float)Math.Floor( value.x ), (float)Math.Floor( value.y ) );
		public static Vector3 Floor( Vector3 value ) => new Vector3( (float)Math.Floor( value.x ), (float)Math.Floor( value.y ), (float)Math.Floor( value.z ) );
		public static Vector4 Floor( Vector4 value ) => new Vector4( (float)Math.Floor( value.x ), (float)Math.Floor( value.y ), (float)Math.Floor( value.z ), (float)Math.Floor( value.w ) );
		public static float Ceil( float value ) => (float)Math.Ceiling( value );
		public static float Round( float value ) => (float)Math.Round( value );
		public static float Round( float value, float snapInterval ) => Mathf.Round( value / snapInterval ) * snapInterval;
		public static int FloorToInt( float value ) => (int)Math.Floor( value );
		public static int CeilToInt( float value ) => (int)Math.Ceiling( value );
		public static int RoundToInt( float value ) => (int)Math.Round( value );

		// Repeating
		public static float Frac( float x ) => x - Floor( x );
		public static Vector2 Frac( Vector2 x ) => x - Floor( x );
		public static Vector3 Frac( Vector3 x ) => x - Floor( x );
		public static Vector4 Frac( Vector4 x ) => x - Floor( x );
		public static float Repeat( float value, float length ) => Clamp( value - Floor( value / length ) * length, 0.0f, length );
		public static int Mod( int value, int length ) => ( value % length + length ) % length; // modulo

		public static float PingPong( float t, float length ) {
			t = Repeat( t, length * 2f );
			return length - Abs( t - length );
		}

		// Smoothing & Curves
		public static float Smooth01( float x ) => x * x * ( 3 - 2 * x );
		public static float Smoother01( float x ) => x * x * x * ( x * ( x * 6 - 15 ) + 10 );
		public static float SmoothCos01( float x ) => Cos( x * PI ) * -0.5f + 0.5f;

		public static float Gamma( float value, float absmax, float gamma ) {
			bool negative = value < 0F;
			float absval = Abs( value );
			if( absval > absmax )
				return negative ? -absval : absval;

			float result = Pow( absval / absmax, gamma ) * absmax;
			return negative ? -result : result;
		}


		// Interpolation & Remapping
		public static float InverseLerp( float a, float b, float value ) => ( value - a ) / ( b - a );
		public static float InverseLerpClamped( float a, float b, float value ) => Clamp01( ( value - a ) / ( b - a ) );
		public static float Lerp( float a, float b, float t ) => ( 1f - t ) * a + t * b;

		public static float LerpClamped( float a, float b, float t ) {
			t = Clamp01( t );
			return ( 1f - t ) * a + t * b;
		}

		public static float Eerp( float a, float b, float t ) => Mathf.Pow( a, 1 - t ) * Mathf.Pow( b, t );
		public static float InverseEerp( float a, float b, float v ) => Mathf.Log( a / v ) / Mathf.Log( a / b );

		public static Vector2 Lerp( Vector2 a, Vector2 b, Vector2 t ) => new Vector2( Mathf.Lerp( a.x, b.x, t.x ), Mathf.Lerp( a.y, b.y, t.y ) );
		public static Vector2 InverseLerp( Vector2 a, Vector2 b, Vector2 v ) => ( v - a ) / ( b - a );

		public static Vector2 Remap( Rect iRect, Rect oRect, Vector2 iPos ) {
			return Remap( iRect.min, iRect.max, oRect.min, oRect.max, iPos );
		}

		public static Vector2 Remap( Vector2 iMin, Vector2 iMax, Vector2 oMin, Vector2 oMax, Vector2 value ) {
			Vector2 t = InverseLerp( iMin, iMax, value );
			return Lerp( oMin, oMax, t );
		}

		public static float Remap( float iMin, float iMax, float oMin, float oMax, float value ) {
			float t = InverseLerp( iMin, iMax, value );
			return Lerp( oMin, oMax, t );
		}

		public static float RemapClamped( float iMin, float iMax, float oMin, float oMax, float value ) {
			float t = InverseLerpClamped( iMin, iMax, value );
			return Lerp( oMin, oMax, t );
		}

		public static float InverseLerpSmooth( float a, float b, float value ) => Smooth01( Clamp01( ( value - a ) / ( b - a ) ) );

		public static float LerpSmooth( float a, float b, float t ) {
			t = Smooth01( Clamp01( t ) );
			return ( 1f - t ) * a + t * b;
		}

		public static float MoveTowards( float current, float target, float maxDelta ) {
			if( Mathf.Abs( target - current ) <= maxDelta )
				return target;
			return current + Mathf.Sign( target - current ) * maxDelta;
		}

		public static float SmoothDamp( float current, float target, ref float currentVelocity, float smoothTime, float maxSpeed ) {
			float deltaTime = Time.deltaTime;
			return SmoothDamp( current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime );
		}

		public static float SmoothDamp( float current, float target, ref float currentVelocity, float smoothTime ) {
			float deltaTime = Time.deltaTime;
			float maxSpeed = Mathf.Infinity;
			return SmoothDamp( current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime );
		}

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


		// Vector math
		public static float Determinant /*or Cross*/( Vector2 a, Vector2 b ) => a.x * b.y - a.y * b.x; // 2D "cross product"
		public static Vector2 Dir( Vector2 from, Vector2 to ) => ( to - from ).normalized;
		public static Vector3 Dir( Vector3 from, Vector3 to ) => ( to - from ).normalized;
		public static Vector2 FromTo( Vector2 from, Vector2 to ) => to - from;
		public static Vector3 FromTo( Vector3 from, Vector3 to ) => to - from;
		public static Vector2 CenterPos( Vector2 a, Vector2 b ) => ( a + b ) / 2f;
		public static Vector3 CenterPos( Vector3 a, Vector3 b ) => ( a + b ) / 2f;
		public static Vector2 CenterDir( Vector2 aDir, Vector2 bDir ) => ( aDir + bDir ).normalized;
		public static Vector3 CenterDir( Vector3 aDir, Vector3 bDir ) => ( aDir + bDir ).normalized;
		public static Vector2 Rotate90CW( Vector2 v ) => new Vector2( v.y, -v.x );
		public static Vector2 Rotate90CCW( Vector2 v ) => new Vector2( -v.y, v.x );
		public static float DistanceSquared( Vector2 a, Vector2 b ) => ( a.x - b.x ).Square() + ( a.y - b.y ).Square();
		public static float DistanceSquared( Vector3 a, Vector3 b ) => ( a.x - b.x ).Square() + ( a.y - b.y ).Square() + ( a.z - b.z ).Square();
		public static float DistanceSquared( Vector4 a, Vector4 b ) => ( a.x - b.x ).Square() + ( a.y - b.y ).Square() + ( a.z - b.z ).Square() + ( a.w - b.w ).Square();

		// Angles & Rotation
		public static Vector2 AngToDir( float aRad ) => new Vector2( Mathf.Cos( aRad ), Mathf.Sin( aRad ) );
		public static float DirToAng( Vector2 dir ) => Mathf.Atan2( dir.y, dir.x );
		public static float SignedAngle( Vector2 a, Vector2 b ) => AngleBetween( a, b ) * Mathf.Sign( Determinant( a, b ) ); // -tau/2 to tau/2
		public static float AngleBetween( Vector2 a, Vector2 b ) => Mathf.Acos( Vector2.Dot( a.normalized, b.normalized ).ClampNeg1to1() );
		public static float AngleFromToCW( Vector2 from, Vector2 to ) => Determinant( from, to ) < 0 ? AngleBetween( from, to ) : TAU - AngleBetween( from, to );
		public static float AngleFromToCCW( Vector2 from, Vector2 to ) => Determinant( from, to ) > 0 ? AngleBetween( from, to ) : TAU - AngleBetween( from, to );

		public static float LerpAngle( float aRad, float bRad, float t ) {
			float delta = Repeat( ( bRad - aRad ), TAU );
			if( delta > PI )
				delta -= TAU;
			return aRad + delta * Clamp01( t );
		}

		public static float DeltaAngle( float a, float b ) => ( b - a + PI ).Repeat( TAU ) - PI;

		public static float InverseLerpAngle( float a, float b, float v ) {
			float angBetween = DeltaAngle( a, b );
			b = a + angBetween; // removes any a->b discontinuity
			float h = a + angBetween * 0.5f; // halfway angle
			v = h + DeltaAngle( h, v ); // get offset from h, and offset by h
			return InverseLerpClamped( a, b, v );
		}

		static public float MoveTowardsAngle( float current, float target, float maxDelta ) {
			float deltaAngle = DeltaAngle( current, target );
			if( -maxDelta < deltaAngle && deltaAngle < maxDelta )
				return target;
			target = current + deltaAngle;
			return MoveTowards( current, target, maxDelta );
		}

		public static float SmoothDampAngle( float current, float target, ref float currentVelocity, float smoothTime, float maxSpeed ) {
			float deltaTime = Time.deltaTime;
			return SmoothDampAngle( current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime );
		}

		public static float SmoothDampAngle( float current, float target, ref float currentVelocity, float smoothTime ) {
			float deltaTime = Time.deltaTime;
			float maxSpeed = Mathf.Infinity;
			return SmoothDampAngle( current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime );
		}

		public static float SmoothDampAngle( float current, float target, ref float currentVelocity, float smoothTime, [Uei.DefaultValue( "Mathf.Infinity" )] float maxSpeed, [Uei.DefaultValue( "Time.deltaTime" )] float deltaTime ) {
			target = current + DeltaAngle( current, target );
			return SmoothDamp( current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime );
		}

		// Root Finding
		/// <summary>Finds the root (x-intercept) of a linear equation of the form ax+b</summary>
		public static float GetLinearRoot( float a, float b ) => -b / a;

		/// <summary>Tries to find the root (x-intercept) of a linear equation of the form ax+b</summary>
		public static bool TryGetLinearRoot( float a, float b, out float root ) {
			if( Mathf.Abs( a ) > 0.00001f ) {
				root = -b / a;
				return true;
			}

			root = default;
			return false;
		}

		public enum PolynomialType {
			Constant,
			Linear,
			Quadratic
		}

		/// <summary>Get the net polynomial type, accounting for values very close to 0, of a polynomial of the form ax²+bx+c</summary>
		public static PolynomialType GetPolynomialType( float a, float b, float c ) {
			if( Mathf.Abs( a ) < 0.00001f )
				return Mathf.Abs( b ) < 0.00001f ? PolynomialType.Constant : PolynomialType.Linear;
			return PolynomialType.Quadratic;
		}

		/// <summary>Finds the roots, if any, of a quadratic polynomial of the form ax²+bx+c</summary>
		public static List<float> GetQuadraticRoots( float a, float b, float c ) {
			List<float> roots = new List<float>();

			switch( GetPolynomialType( a, b, c ) ) {
				case PolynomialType.Constant:
					break; // either no roots or infinite roots if c == 0
				case PolynomialType.Linear:
					roots.Add( -c / b );
					break;
				case PolynomialType.Quadratic:
					float rootContent = b * b - 4 * a * c;
					if( Mathf.Abs( rootContent ) < 0.0001f ) {
						roots.Add( -b / ( 2 * a ) ); // two equivalent solutions at one point
					} else if( rootContent >= 0 ) {
						float root = Mathf.Sqrt( rootContent );
						roots.Add( ( -b + root ) / ( 2 * a ) ); // crosses at two points
						roots.Add( ( -b - root ) / ( 2 * a ) );
					} // else no roots

					break;
			}

			return roots;
		}


		// coordinate shenanigans
		public static Vector2 SquareToDisc( Vector2 c ) {
			float u = c.x * Sqrt( 1 - ( c.y * c.y ) / 2 );
			float v = c.y * Sqrt( 1 - ( c.x * c.x ) / 2 );
			return new Vector2( u, v );
		}

		public static Vector2 DiscToSquare( Vector2 c ) {
			float u = c.x;
			float v = c.y;
			float u2 = c.x * c.x;
			float v2 = c.y * c.y;

			Vector2 n = new Vector2( 1, -1 );
			Vector2 p = new Vector2( 2, 2 ) + n * ( u2 - v2 );
			Vector2 q = 2 * SQRT2 * c;
			Vector2 smolVec = Vector2.one * 0.0001f;
			Vector2 Sqrt( Vector2 noot ) => new Vector2( Mathf.Sqrt( noot.x ), Mathf.Sqrt( noot.y ) );
			return 0.5f * ( Sqrt( Vector2.Max( smolVec, p + q ) ) - Sqrt( Vector2.Max( smolVec, p - q ) ) );
		}


	}

}