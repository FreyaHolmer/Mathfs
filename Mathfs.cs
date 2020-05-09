// Lots of this code is from Unity's original Mathf source to match functionality.
// The original Mathf.cs source https://github.com/Unity-Technologies/UnityCsReference/blob/master/Runtime/Export/Math/Mathf.cs
// ...and the bits of it in here is copyright (c) Unity Technologies with license: https://unity3d.com/legal/licenses/Unity_Reference_Only_License
// 
// Collected and expanded upon to by Freya Holmér (https://github.com/FreyaHolmer) 


// Keep the below defined if you want to port a project from using Mathf to Mathfs, otherwise I recommend commenting this out!

#define MATCH_UNITYS_IMPLEMENTATION

// Changes made compared to Unity's Mathf, when MATCH_UNITYS_IMPLEMENTATION is not defined:
// • min/max( <empty array> ) throws if empty instead of returning 0
// • lerp/inverse lerp are unclamped by default (to match shader languages)
//     • LerpClamped / InverseLerpClamped are now the special case functions
//     • they all now also use the more numerically stable (1-t)a+tb instead of a+t(b-a)
//     • inverse lerps return NaN instead of 0 when dividing by zero 
// • SmoothStep replaced with SmoothLerp
//     • SmoothInverseLerp acts like smoothstep of shader languages
//     • both are clamped due to the clamped nature of smoothing
//     • I omitted "clamped" from their names since it'll make them long and unweildy sorry </3
// • Angle functions are renamed from "[...]Angle" to "[...]AngleDeg" to distinguish them from the radians versions and clarify what units they use


using System;
using UnityEngine;
using Uei = UnityEngine.Internal;

#if MATCH_UNITYS_IMPLEMENTATION == false
using System.Linq; // used for arbitrary count min/max functions, so it's safe and won't allocate garbage don't worry~
#endif

public static class Mathfs {

	// Constants
	public const float TAU = 6.28318530717959f;
	public const float PI = 3.14159265359f;
	public const float E = 2.71828182846f;
	public const float GOLDEN_RATIO = 1.61803398875f;
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
	public static float Max( float a, float b ) => a < b ? a : b;
	public static float Max( float a, float b, float c ) => Max( Max( a, b ), c );
	public static float Max( float a, float b, float c, float d ) => Max( Max( a, b ), Max( c, d ) );
	public static int Min( int a, int b ) => a < b ? a : b;
	public static int Min( int a, int b, int c ) => Min( Min( a, b ), c );
	public static int Max( int a, int b ) => a < b ? a : b;
	public static int Max( int a, int b, int c ) => Max( Max( a, b ), c );


	#if MATCH_UNITYS_IMPLEMENTATION
	public static float Min( params float[] values ) {
		int len = values.Length;
		if( len == 0 )
			return 0;
		float m = values[0];
		for( int i = 1; i < len; i++ ) {
			if( values[i] < m )
				m = values[i];
		}

		return m;
	}

	public static int Min( params int[] values ) {
		int len = values.Length;
		if( len == 0 )
			return 0;
		int m = values[0];
		for( int i = 1; i < len; i++ ) {
			if( values[i] < m )
				m = values[i];
		}

		return m;
	}

	public static float Max( params float[] values ) {
		int len = values.Length;
		if( len == 0 )
			return 0;
		float m = values[0];
		for( int i = 1; i < len; i++ ) {
			if( values[i] > m )
				m = values[i];
		}

		return m;
	}

	public static int Max( params int[] values ) {
		int len = values.Length;
		if( len == 0 )
			return 0;
		int m = values[0];
		for( int i = 1; i < len; i++ ) {
			if( values[i] > m )
				m = values[i];
		}

		return m;
	}
	#else
	public static float Min( params float[] values ) => values.Min();
	public static float Max( params float[] values ) => values.Max();
	public static int Min( params int[] values ) => values.Min();
	public static int Max( params int[] values ) => values.Max();
	#endif

	// Rounding
	public static float Sign( float value ) => value >= 0f ? 1f : -1f;
	public static float Floor( float value ) => (float)Math.Floor( value );
	public static float Ceil( float value ) => (float)Math.Ceiling( value );
	public static float Round( float value ) => (float)Math.Round( value );
	public static int FloorToInt( float value ) => (int)Math.Floor( value );
	public static int CeilToInt( float value ) => (int)Math.Ceiling( value );
	public static int RoundToInt( float value ) => (int)Math.Round( value );

	// Repeating
	public static float Frac( float x ) => x - Floor( x );
	public static float Repeat( float t, float length ) => Clamp( t - Floor( t / length ) * length, 0.0f, length );

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
	#if MATCH_UNITYS_IMPLEMENTATION
	public static float Lerp( float a, float b, float t ) => a + ( b - a ) * Clamp01( t );
	public static float LerpUnclamped( float a, float b, float t ) => a + ( b - a ) * t;

	public static float InverseLerp( float a, float b, float value ) {
		if( a != b )
			return Clamp01( ( value - a ) / ( b - a ) );
		else
			return 0f;
	}

	public static float InverseLerpUnclamped( float a, float b, float value ) {
		if( a != b )
			return ( value - a ) / ( b - a );
		else
			return 0f;
	}
	#else
	public static float InverseLerp( float a, float b, float value ) => ( value - a ) / ( b - a );
	public static float InverseLerpClamped( float a, float b, float value ) => Clamp01( ( value - a ) / ( b - a ) );
	public static float Lerp( float a, float b, float t ) => ( 1f - t ) * a + t * b;
	public static float LerpClamped( float a, float b, float t ) {
		t = Clamp01( t );
		return ( 1f - t ) * a + t * b;
	}
	#endif
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
		float t = ILERP_UNCLAMPED( iMin, iMax, value );
		return Mathf.Lerp( oMin, oMax, t );
	}

	public static float RemapClamped( float iMin, float iMax, float oMin, float oMax, float value ) {
		float t = ILERP_CLAMPED( iMin, iMax, value );
		return Mathf.LerpUnclamped( oMin, oMax, t );
	}

	#if MATCH_UNITYS_IMPLEMENTATION
	// omitted because it's confusing - it contradicts what smoothstep means in every other context
	public static float SmoothStep( float from, float to, float t ) {
		t = Mathf.Clamp01( t );
		t = -2.0f * t * t * t + 3.0f * t * t;
		return to * t + from * ( 1F - t );
	}
	#endif

	public static float InverseLerpSmooth( float a, float b, float value ) => Smooth01( Clamp01( ( value - a ) / ( b - a ) ) );

	public static float LerpSmooth( float a, float b, float t ) {
		t = Smooth01( Clamp01( t ) );
		return ( 1f - t ) * a + t * b;
	}

	static public float MoveTowards( float current, float target, float maxDelta ) {
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


	// Angles & Rotation
	public static Vector2 AngToDir( float aRad ) => new Vector2( Mathf.Cos( aRad ), Mathf.Sin( aRad ) );
	public static float DirToAng( Vector2 dir ) => Mathf.Atan2( dir.y, dir.x );

	public static float SignedAngle( Vector2 a, Vector2 b ) {
		return Mathf.Acos( Vector2.Dot( a.normalized, b.normalized ) ) * Mathf.Sign( Determinant( a, b ) ); // 0 to tau/2
	}

	#if MATCH_UNITYS_IMPLEMENTATION
	public static float LerpAngle( float a, float b, float t ) {
		float delta = Repeat( ( b - a ), 360 );
		if( delta > 180 )
			delta -= 360;
		return a + delta * Clamp01( t );
	}

	public static float DeltaAngle( float current, float target ) {
		float delta = Mathf.Repeat( ( target - current ), 360f );
		if( delta > 180f )
			delta -= 360f;
		return delta;
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
	#else
	public static float LerpAngleDeg( float aDeg, float bDeg, float t ) {
		float delta = Repeat( ( bDeg - aDeg ), 360f );
		if( delta > 180f )
			delta -= 360f;
		return aDeg + delta * Clamp01( t );
	}

	public static float DeltaAngleDeg( float current, float target ) {
		float delta = Mathf.Repeat( ( target - current ), 360f );
		if( delta > 180f )
			delta -= 360f;
		return delta;
	}

	static public float MoveTowardsAngleDeg( float current, float target, float maxDelta ) {
		float deltaAngle = DeltaAngleDeg( current, target );
		if( -maxDelta < deltaAngle && deltaAngle < maxDelta )
			return target;
		target = current + deltaAngle;
		return MoveTowards( current, target, maxDelta );
	}
	
	public static float SmoothDampAngleDeg( float current, float target, ref float currentVelocity, float smoothTime, float maxSpeed ) {
		float deltaTime = Time.deltaTime;
		return SmoothDampAngleDeg( current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime );
	}

	public static float SmoothDampAngleDeg( float current, float target, ref float currentVelocity, float smoothTime ) {
		float deltaTime = Time.deltaTime;
		float maxSpeed = Mathf.Infinity;
		return SmoothDampAngleDeg( current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime );
	}

	public static float SmoothDampAngleDeg( float current, float target, ref float currentVelocity, float smoothTime, [Uei.DefaultValue( "Mathf.Infinity" )] float maxSpeed, [Uei.DefaultValue( "Time.deltaTime" )] float deltaTime ) {
		target = current + DeltaAngleDeg( current, target );
		return SmoothDamp( current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime );
	}
	#endif
	public static float LerpAngleRad( float aRad, float bRad, float t ) {
		float delta = Repeat( ( bRad - aRad ), TAU );
		if( delta > PI )
			delta -= TAU;
		return aRad + delta * Clamp01( t );
	}

	public static float DeltaAngleRad( float current, float target ) {
		float delta = Mathf.Repeat( ( target - current ), TAU );
		if( delta > PI )
			delta -= TAU;
		return delta;
	}

	static public float MoveTowardsAngleRad( float current, float target, float maxDelta ) {
		float deltaAngle = DeltaAngleRad( current, target );
		if( -maxDelta < deltaAngle && deltaAngle < maxDelta )
			return target;
		target = current + deltaAngle;
		return MoveTowards( current, target, maxDelta );
	}

	public static float SmoothDampAngleRad( float current, float target, ref float currentVelocity, float smoothTime, float maxSpeed ) {
		float deltaTime = Time.deltaTime;
		return SmoothDampAngleRad( current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime );
	}

	public static float SmoothDampAngleRad( float current, float target, ref float currentVelocity, float smoothTime ) {
		float deltaTime = Time.deltaTime;
		float maxSpeed = Mathf.Infinity;
		return SmoothDampAngleRad( current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime );
	}

	public static float SmoothDampAngleRad( float current, float target, ref float currentVelocity, float smoothTime, [Uei.DefaultValue( "Mathf.Infinity" )] float maxSpeed, [Uei.DefaultValue( "Time.deltaTime" )] float deltaTime ) {
		target = current + DeltaAngleRad( current, target );
		return SmoothDamp( current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime );
	}


	// Triangle math
	public static class Triangle {

		public static float SignedArea( Vector3 a, Vector3 b, Vector3 c ) => Vector3.Cross( b - a, c - a ).magnitude / 2f;
		public static float Area( Vector3 a, Vector3 b, Vector3 c ) => Mathf.Abs( SignedArea( a, b, c ) );
		public static Vector3 Centroid( Vector3 a, Vector3 b, Vector3 c ) => ( a + b + c ) / 3f;

		public static Vector3 Incenter( Vector3 a, Vector3 b, Vector3 c ) {
			float bc = Vector3.Distance( b, c );
			float ca = Vector3.Distance( c, a );
			float ab = Vector3.Distance( a, b );
			return ( bc * a + ca * b + ab * c ) / ( bc + ca + ab );
		}

		public static (Vector2 center, float radius) Circumcircle( Vector2 a, Vector2 b, Vector2 c ) {
			Vector2 SqSub( Vector2 p0, Vector2 p1 ) => Vector2.Scale( p0, p0 ) - Vector2.Scale( p1, p1 );
			Vector2 BA = a - b;
			Vector2 CA = a - c;
			Vector2 s13 = SqSub( a, c ); // the heck is this
			Vector2 s21 = SqSub( b, a );
			Vector2 noot = BA * ( s13.x + s13.y ) + CA * ( s21.x + s21.y );
			Vector2 center = new Vector2( noot.y, -noot.x ) / ( 2 * Mathfs.Determinant( CA, BA ) );
			float boop = -a.x * a.x - a.y * a.y + 2 * center.x * a.x + 2 * center.y * a.y;
			float radius = Mathf.Sqrt( center.sqrMagnitude - boop );
			return ( center, radius );
		}

	}


	// Trajectory math
	public static class Trajectory {

		public static float GetLaunchSpeed( float lateralDistance, float angleRad ) => GetLaunchSpeed( lateralDistance, angleRad, -Physics.gravity.y );

		public static float GetLaunchSpeed( float lateralDistance, float angleRad, float gravity ) {
			return Mathf.Sqrt( ( lateralDistance * gravity ) / Mathf.Sin( 2 * angleRad ) );
		}

		public static bool TryGetLaunchAngles( float lateralDistance, float speed, out float angleLow, out float angleHigh ) {
			return TryGetLaunchAngles( lateralDistance, speed, -Physics.gravity.y, out angleLow, out angleHigh );
		}

		public static bool TryGetLaunchAngles( float lateralDistance, float speed, float gravity, out float angleLow, out float angleHigh ) {
			float asinContent = ( lateralDistance * gravity ) / ( speed * speed );
			if( asinContent >= -1 && asinContent <= 1 ) {
				angleLow = Mathf.Asin( asinContent ) / 2;
				angleHigh = ( -angleLow + Mathf.PI / 2 );
				return true;
			}

			angleLow = default;
			angleHigh = default;
			return false;
		}

		public static float GetMaxRange( float speed ) => GetMaxRange( speed, -Physics.gravity.y );
		public static float GetMaxRange( float speed, float gravity ) => speed * speed / gravity;

		public static Vector2 GetDisplacement( float speed, float angle, float time ) => GetDisplacement( speed, angle, time, -Physics.gravity.y );

		public static Vector2 GetDisplacement( float speed, float angle, float time, float gravity ) {
			float xDisp = speed * time * Mathf.Cos( angle );
			float yDisp = speed * time * Mathf.Sin( angle ) - .5f * gravity * time * time;
			return new Vector2( xDisp, yDisp );
		}
	}


	// internal functions to deal with branching
	#if MATCH_UNITYS_IMPLEMENTATION
	static float LERP_UNCLAMPED( float a, float b, float t ) => LerpUnclamped( a, b, t );
	static float LERP_CLAMPED( float a, float b, float t ) => Lerp( a, b, t );
	static float ILERP_UNCLAMPED( float a, float b, float value ) => InverseLerpUnclamped( a, b, value );
	static float ILERP_CLAMPED( float a, float b, float value ) => InverseLerp( a, b, value );
	#else
	static float LERP_UNCLAMPED( float a, float b, float t ) => Lerp( a, b, t );
	static float LERP_CLAMPED( float a, float b, float t ) => LerpClamped( a, b, t );
	static float ILERP_UNCLAMPED( float a, float b, float value ) => InverseLerp( a, b, value );
	static float ILERP_CLAMPED( float a, float b, float value ) => InverseLerpClamped( a, b, value );
	#endif


}