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

namespace Freya {

	public static class Mathfs {

		// Constants
		public const float TAU = 6.28318530717959f;
		public const float PI = 3.14159265359f;
		public const float E = 2.71828182846f;
		public const float GOLDEN_RATIO = 1.61803398875f;
		public const float SQRT2 = 1.41421356237f;
		public const float Deg2Rad = TAU / 360f;
		public const float Rad2Deg = 360f / TAU;

		#region Math operations

		public static float Sqrt( float value ) => (float)Math.Sqrt( value );
		public static float Cbrt( float value ) => value < 0 ? -Pow( -value, 1f / 3f ) : Pow( value, 1f / 3f );
		public static float Pow( float @base, float exponent ) => (float)Math.Pow( @base, exponent );
		public static float Exp( float power ) => (float)Math.Exp( power );
		public static float Log( float value, float @base ) => (float)Math.Log( value, @base );
		public static float Log( float value ) => (float)Math.Log( value );
		public static float Log10( float value ) => (float)Math.Log10( value );

		#endregion

		#region Floating point shenanigans

		public static readonly float Epsilon = UnityEngineInternal.MathfInternal.IsFlushToZeroEnabled ? UnityEngineInternal.MathfInternal.FloatMinNormal : UnityEngineInternal.MathfInternal.FloatMinDenormal;
		public const float Infinity = float.PositiveInfinity;
		public const float NegativeInfinity = float.NegativeInfinity;
		public static bool Approximately( float a, float b ) => Abs( b - a ) < Max( 0.000001f * Max( Abs( a ), Abs( b ) ), Epsilon * 8 );

		#endregion

		#region Trigonometry

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

		// Hyperbolic trig
		public static float Cosh( float x ) => (float)Math.Cosh( x );
		public static float Sinh( float x ) => (float)Math.Sinh( x );
		public static float Tanh( float x ) => (float)Math.Tanh( x );
		public static float Acosh( float x ) => (float)Math.Log( x + Mathf.Sqrt( x * x - 1 ) );
		public static float Asinh( float x ) => (float)Math.Log( x + Mathf.Sqrt( x * x + 1 ) );
		public static float Atanh( float x ) => (float)( 0.5 * Math.Log( ( 1 + x ) / ( 1 - x ) ) );

		#endregion

		#region Value clamping

		public static float Abs( float value ) => Math.Abs( value );
		public static int Abs( int value ) => Math.Abs( value );
		public static Vector2 Abs( Vector2 v ) => new Vector2( Abs( v.x ), Abs( v.y ) );
		public static Vector3 Abs( Vector3 v ) => new Vector3( Abs( v.x ), Abs( v.y ), Abs( v.z ) );
		public static Vector4 Abs( Vector4 v ) => new Vector4( Abs( v.x ), Abs( v.y ), Abs( v.z ), Abs( v.w ) );

		// Clamping
		public static float Clamp( float value, float min, float max ) => value < min ? min : value > max ? max : value;
		public static int Clamp( int value, int min, int max ) => value < min ? min : value > max ? max : value;
		public static float Clamp01( float value ) => value < 0f ? 0f : value > 1f ? 1f : value;
		public static float ClampNeg1to1( float value ) => value < -1f ? -1f : value > 1f ? 1f : value;

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

		#endregion

		#region Rounding & Repeating

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

		#endregion

		#region Smoothing & Easing Curves

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

		#endregion

		#region Value & Vector interpolation

		public static float Lerp( float a, float b, float t ) => ( 1f - t ) * a + t * b;
		public static float LerpClamped( float a, float b, float t ) => Lerp( a, b, Clamp01( t ) );
		public static float LerpSmooth( float a, float b, float t ) => Lerp( a, b, Smooth01( Clamp01( t ) ) );

		public static float Eerp( float a, float b, float t ) => Mathf.Pow( a, 1 - t ) * Mathf.Pow( b, t );
		public static float InverseEerp( float a, float b, float v ) => Mathf.Log( a / v ) / Mathf.Log( a / b );

		public static Vector2 Lerp( Vector2 a, Vector2 b, Vector2 t ) => new Vector2( Lerp( a.x, b.x, t.x ), Lerp( a.y, b.y, t.y ) );

		public static float InverseLerp( float a, float b, float value ) => ( value - a ) / ( b - a );
		public static float InverseLerpClamped( float a, float b, float value ) => Clamp01( ( value - a ) / ( b - a ) );
		public static float InverseLerpSmooth( float a, float b, float value ) => Smooth01( Clamp01( ( value - a ) / ( b - a ) ) );

		public static float Remap( float iMin, float iMax, float oMin, float oMax, float value ) => Lerp( oMin, oMax, InverseLerp( iMin, iMax, value ) );
		public static float RemapClamped( float iMin, float iMax, float oMin, float oMax, float value ) => Lerp( oMin, oMax, InverseLerpClamped( iMin, iMax, value ) );

		public static Vector2 Remap( Rect iRect, Rect oRect, Vector2 iPos ) => Remap( iRect.min, iRect.max, oRect.min, oRect.max, iPos );
		public static Vector2 Remap( Vector2 iMin, Vector2 iMax, Vector2 oMin, Vector2 oMax, Vector2 value ) => Lerp( oMin, oMax, InverseLerp( iMin, iMax, value ) );

		#endregion

		#region Movement helpers

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

		#endregion

		#region Weighted sums

		public static float WeightedSum( Vector2 w, float a, float b ) => a * w.x + b * w.y;
		public static float WeightedSum( Vector3 w, float a, float b, float c ) => a * w.x + b * w.y + c * w.z;
		public static float WeightedSum( Vector4 w, float a, float b, float c, float d ) => a * w.x + b * w.y + c * w.z + d * w.w;
		public static Vector2 WeightedSum( Vector2 w, Vector2 a, Vector2 b ) => a * w.x + b * w.y;
		public static Vector2 WeightedSum( Vector3 w, Vector2 a, Vector2 b, Vector2 c ) => a * w.x + b * w.y + c * w.z;
		public static Vector2 WeightedSum( Vector4 w, Vector2 a, Vector2 b, Vector2 c, Vector2 d ) => a * w.x + b * w.y + c * w.z + d * w.w;
		public static Vector3 WeightedSum( Vector3 w, Vector3 a, Vector3 b ) => a * w.x + b * w.y;
		public static Vector3 WeightedSum( Vector3 w, Vector3 a, Vector3 b, Vector3 c ) => a * w.x + b * w.y + c * w.z;
		public static Vector3 WeightedSum( Vector4 w, Vector3 a, Vector3 b, Vector3 c, Vector3 d ) => a * w.x + b * w.y + c * w.z + d * w.w;
		public static Vector4 WeightedSum( Vector4 w, Vector4 a, Vector4 b ) => a * w.x + b * w.y;
		public static Vector4 WeightedSum( Vector4 w, Vector4 a, Vector4 b, Vector4 c ) => a * w.x + b * w.y + c * w.z;
		public static Vector4 WeightedSum( Vector4 w, Vector4 a, Vector4 b, Vector4 c, Vector4 d ) => a * w.x + b * w.y + c * w.z + d * w.w;

		#endregion

		#region Vector math

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

		#endregion

		#region Angles & Rotation

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

		#endregion

		#region Angular movement helpers

		public static float MoveTowardsAngle( float current, float target, float maxDelta ) {
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
		public static PolynomialType GetPolynomialType( float a, float b, float c, float d ) => FactorAlmost0( a ) ? GetPolynomialType( b, c, d ) : PolynomialType.Cubic;

		/// <summary>Given ax²+bx+c, returns the net polynomial type/degree, accounting for values very close to 0</summary>
		public static PolynomialType GetPolynomialType( float a, float b, float c ) => FactorAlmost0( a ) ? GetPolynomialType( b, c ) : PolynomialType.Quadratic;

		/// <summary>Given ax+b, returns the net polynomial type/degree, accounting for values very close to 0</summary>
		public static PolynomialType GetPolynomialType( float a, float b ) => FactorAlmost0( a ) ? PolynomialType.Constant : PolynomialType.Linear;

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

		static float SolveLinearRoot( float a, float b ) => -b / a;

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

		public static Vector2 SquareToDisc( Vector2 c ) {
			float u = c.x * Sqrt( 1 - ( c.y * c.y ) / 2 );
			float v = c.y * Sqrt( 1 - ( c.x * c.x ) / 2 );
			return new Vector2( u, v );
		}

		public static Vector2 DiscToSquare( Vector2 c ) {
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