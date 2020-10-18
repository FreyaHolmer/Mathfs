// Lots of this code is from Unity's original Mathf source to match functionality.
// The original Mathf.cs source https://github.com/Unity-Technologies/UnityCsReference/blob/master/Runtime/Export/Math/Mathf.cs
// ...and the bits of it in here is copyright (c) Unity Technologies with license: https://unity3d.com/legal/licenses/Unity_Reference_Only_License
// 
// Collected and expanded upon to by Freya Holmér (https://github.com/FreyaHolmer/Mathfs) 

using System;
using System.Collections.Generic;
using UnityEngine;
using Uei = UnityEngine.Internal;
using System.Linq; // used for arbitrary count min/max functions, so it's safe and won't allocate garbage don't worry~

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
	public static float Sign( float value ) => value >= 0f ? 1f : -1f;
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


	// Angles & Rotation
	public static Vector2 AngToDir( float aRad ) => new Vector2( Mathf.Cos( aRad ), Mathf.Sin( aRad ) );
	public static float DirToAng( Vector2 dir ) => Mathf.Atan2( dir.y, dir.x );
	public static float SignedAngle( Vector2 a, Vector2 b ) => AngleBetween( a, b ) * Mathf.Sign( Determinant( a, b ) ); // -tau/2 to tau/2
	public static float AngleBetween( Vector2 a, Vector2 b ) => Mathf.Acos( Vector2.Dot( a.normalized, b.normalized ) );
	public static float AngleFromToCW( Vector2 from, Vector2 to ) => Determinant( from, to ) < 0 ? AngleBetween( from, to ) : TAU - AngleBetween( from, to );
	public static float AngleFromToCCW( Vector2 from, Vector2 to ) => Determinant( from, to ) > 0 ? AngleBetween( from, to ) : TAU - AngleBetween( from, to );

	public static float LerpAngleDeg( float aDeg, float bDeg, float t ) {
		float delta = Repeat( ( bDeg - aDeg ), 360f );
		if( delta > 180f )
			delta -= 360f;
		return aDeg + delta * Clamp01( t );
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

	public static float LerpAngleRad( float aRad, float bRad, float t ) {
		float delta = Repeat( ( bRad - aRad ), TAU );
		if( delta > PI )
			delta -= TAU;
		return aRad + delta * Clamp01( t );
	}


	public static float DeltaAngleRad( float a, float b ) => ( b - a + PI ).Repeat( TAU ) - PI;

	public static float InverseLerpAngleRad( float a, float b, float v ) {
		float angBetween = DeltaAngleRad( a, b );
		b = a + angBetween; // removes any a->b discontinuity
		float h = a + angBetween * 0.5f; // halfway angle
		v = h + DeltaAngleRad( h, v ); // get offset from h, and offset by h
		return InverseLerpClamped( a, b, v );
	}

	public static float DeltaAngleDeg( float a, float b ) => ( b - a + 180 ).Repeat( 360 ) - 180;

	static float InverseLerpAngleDeg( float a, float b, float v ) {
		float angBetween = DeltaAngleDeg( a, b );
		b = a + angBetween; // removes any a->b discontinuity
		float h = a + angBetween * 0.5f; // halfway angle
		v = h + DeltaAngleDeg( h, v ); // get offset from h, and offset by h
		return InverseLerpClamped( a, b, v );
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
			Vector2 center = new Vector2( noot.y, -noot.x ) / ( 2 * Determinant( CA, BA ) );
			float boop = -a.x * a.x - a.y * a.y + 2 * center.x * a.x + 2 * center.y * a.y;
			float radius = Mathf.Sqrt( center.sqrMagnitude - boop );
			return ( center, radius );
		}

		public static bool Contains( Vector2 a, Vector2 b, Vector2 c, Vector2 point, float aMargin = 0f, float bMargin = 0f, float cMargin = 0f ) {
			float d0 = Determinant( b - a, point - a );
			float d1 = Determinant( c - b, point - b );
			float d2 = Determinant( a - c, point - c );
			bool b0 = d0 < cMargin;
			bool b1 = d1 < aMargin;
			bool b2 = d2 < bMargin;
			return b0 == b1 && b1 == b2; // on the same side of all halfspaces, this can only happen inside
		}

	}

	public static class Polygon {

		public static bool IsClockwise( IReadOnlyList<Vector2> pts ) => SignedArea( pts ) > 0;
		public static float Area( IReadOnlyList<Vector2> pts ) => Mathf.Abs( SignedArea( pts ) );

		public static float SignedArea( IReadOnlyList<Vector2> pts ) {
			int count = pts.Count;
			float sum = 0f;
			for( int i = 0; i < count; i++ ) {
				Vector2 a = pts[i];
				Vector2 b = pts[( i + 1 ) % count];
				sum += ( b.x - a.x ) * ( ( b.y + a.y ) * 0.5f );
			}

			return sum;
		}

	}

	// Root Finding
	public static float GetLinearRoot( float k, float m ) => -m / k;

	public static bool TryGetLinearRoot( float k, float m, out float root ) { // kx + m
		if( Mathf.Abs( k ) > 0.00001f ) {
			root = -m / k;
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

	public static PolynomialType GetPolynomialType( float a, float b, float c ) {
		if( Mathf.Abs( a ) < 0.00001f )
			return Mathf.Abs( b ) < 0.00001f ? PolynomialType.Constant : PolynomialType.Linear;
		return PolynomialType.Quadratic;
	}

	public static List<float> GetQuadraticRoots( float a, float b, float c ) { // ax² + bx + c
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

	// Random stuff (like, actually things of the category randomization, not, "various items")
	public static class Random {
		public static Vector2 InUnitSquare => new Vector2( UnityEngine.Random.value, UnityEngine.Random.value );
	}


	// Trajectory math
	public static class Trajectory {

		/// <summary>
		/// Outputs the launch speed required to traverse a given lateral distance when launched at a given angle, if one exists
		/// </summary>
		/// <param name="gravity">Gravitational acceleration in meters per second</param>
		/// <param name="lateralDistance">Target lateral distance in meters</param>
		/// <param name="angle">Launch angle in radians (0 = flat)</param>
		/// <param name="speed">Launch speed in meters per second</param>
		/// <returns>Whether or not there is a valid launch speed</returns>
		public static bool TryGetLaunchSpeed( float gravity, float lateralDistance, float angle, out float speed ) {
			float num = lateralDistance * gravity;
			float den = Mathf.Sin( 2 * angle );
			if( Mathf.Abs( den ) < 0.00001f ) {
				speed = default;
				return false; // direction is parallel, no speed would get you there
			}

			float speedSquared = num / den;
			if( speedSquared < 0 ) {
				speed = 0;
				return false; // can't reach destination because you're going the wrong way
			}

			speed = Mathf.Sqrt( speedSquared );
			return true;
		}

		/// <summary>
		/// Outputs the two launch angles given a lateral distance and launch speed, if they exist
		/// </summary>
		/// <param name="gravity">Gravitational acceleration in meters per second</param>
		/// <param name="lateralDistance">Target lateral distance in meters</param>
		/// <param name="speed">Launch speed in meters per second</param>
		/// <param name="angleLow">The low launch angle in radians</param>
		/// <param name="angleHigh">The high launch angle in radians</param>
		/// <returns>Whether or not valid launch angles exist</returns>
		public static bool TryGetLaunchAngles( float gravity, float lateralDistance, float speed, out float angleLow, out float angleHigh ) {
			if( speed == 0 ) {
				angleLow = angleHigh = default;
				return false; // can't reach anything without speed
			}

			float asinContent = ( lateralDistance * gravity ) / ( speed * speed );
			if( asinContent.Within( -1, 1 ) == false ) {
				angleLow = angleHigh = default;
				return false; // can't reach no matter what angle is used
			}

			angleLow = Asin( asinContent ) / 2;
			angleHigh = ( -angleLow + TAU / 4 );
			return true;
		}

		/// <summary>
		/// Returns the maximum lateral range a trajectory could reach, when launched at the optimal angle of 45°
		/// </summary>
		/// <param name="gravity">Gravitational acceleration in meters per second</param>
		/// <param name="speed">Launch speed in meters per second</param>
		/// <returns>Maximum lateral range in meters per second</returns>
		public static float GetMaxRange( float gravity, float speed ) {
			return speed * speed / gravity;
		}

		/// <summary>
		/// Returns the displacement given a launch speed, launch angle and a traversal time 
		/// </summary>
		/// <param name="gravity">Gravitational acceleration in meters per second</param>
		/// <param name="speed">Launch speed in meters per second</param>
		/// <param name="angle">Launch angle in radians (0 = flat)</param>
		/// <param name="time">Traversal time in seconds</param>
		/// <returns>Displacement, where x = lateral displacement and y = vertical displacement</returns>
		public static Vector2 GetDisplacement( float gravity, float speed, float angle, float time ) {
			float xDisp = speed * time * Mathf.Cos( angle );
			float yDisp = speed * time * Mathf.Sin( angle ) - .5f * gravity * time * time;
			return new Vector2( xDisp, yDisp );
		}

		/// <summary>
		/// Returns the maximum height that can possibly be reached if speed was redirected upwards, given a current height and speed
		/// </summary>
		/// <param name="gravity">Gravitational acceleration in meters per second</param>
		/// <param name="currentHeight">Current height in meters</param>
		/// <param name="speed">Launch speed in meters per second</param>
		/// <returns>Potential height in meters</returns>
		public static float GetHeightPotential( float gravity, float currentHeight, float speed ) {
			return currentHeight + ( speed * speed ) / ( 2 * -gravity );
		}

		/// <summary>
		/// Outputs the speed of an object with a given height potential and current height, if it exists
		/// </summary>
		/// <param name="gravity">Gravitational acceleration in meters per second</param>
		/// <param name="currentHeight">Current height in meters</param>
		/// <param name="heightPotential">Potential height in meters</param>
		/// <param name="speed">Speed in meters per second</param>
		/// <returns>Whether or not there is a valid speed</returns>
		public static bool TryGetSpeedFromHeightPotential( float gravity, float currentHeight, float heightPotential, out float speed ) {
			float speedSq = ( heightPotential - currentHeight ) * -2 * gravity;
			if( speedSq <= 0 ) {
				speed = default; // Imaginary speed :sparkles:
				return false;
			}

			speed = Mathf.Sqrt( speedSq );
			return true;
		}

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