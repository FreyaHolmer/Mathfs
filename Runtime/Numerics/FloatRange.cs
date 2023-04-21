﻿// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using System;
using UnityEngine;

namespace Freya {

	/// <summary>A value range between two values a and b</summary>
	public readonly struct FloatRange {

		/// <summary>The unit interval of 0 to 1</summary>
		public static readonly FloatRange unit = new FloatRange( 0, 1 );

		/// <summary>The start of this range</summary>
		public readonly float a;

		/// <summary>The end of this range</summary>
		public readonly float b;

		/// <summary>Creates a new value range</summary>
		/// <param name="a">The start of the range</param>
		/// <param name="b">The end of the range</param>
		public FloatRange( float a, float b ) => ( this.a, this.b ) = ( a, b );

		/// <summary>The value at the center of this value range</summary>
		public float Center => ( a + b ) / 2;

		/// <summary>The length/span of this value range</summary>
		public float Length => Mathfs.Abs( b - a );

		/// <summary>The minimum value of this range</summary>
		public float Min => Mathfs.Min( a, b );

		/// <summary>The maximum value of this range</summary>
		public float Max => Mathfs.Max( a, b );

		/// <summary>The direction of this value range. Returns -1 if <c>b</c> is greater than <c>a</c>, otherwise returns 1</summary>
		public int Direction => b > a ? 1 : -1;

		/// <summary>Interpolates a value from <c>a</c> to <c>b</c>, based on a parameter <c>t</c></summary>
		/// <param name="t">The normalized interpolant from <c>a</c> to <c>b</c>. A value of 0 returns <c>a</c>, a value of 1 returns <c>b</c></param>
		public float Lerp( float t ) => Mathfs.Lerp( a, b, t );

		/// <summary>Returns the normalized position of the input value <c>v</c> within this range</summary>
		/// <param name="v">The value to get the normalized position of</param>
		public float InverseLerp( float v ) => Mathfs.InverseLerp( a, b, v );

		/// <summary>Returns whether or not this range contains the value <c>v</c></summary>
		/// <param name="v">The value to see if it's inside</param>
		public bool Contains( float v ) => v >= Min && v <= Max;

		/// <summary>Returns whether or not this range contains the range <c>r</c></summary>
		/// <param name="r">The range to see if it's inside</param>
		public bool Contains( FloatRange r ) => r.Min >= Min && r.Max <= Max;

		/// <summary>Remaps the input value from the <c>input</c> range to the <c>output</c> range</summary>
		/// <param name="value">The value to remap</param>
		/// <param name="input">The input range</param>
		/// <param name="output">The output range</param>
		public static float Remap( float value, FloatRange input, FloatRange output ) => output.Lerp( input.InverseLerp( value ) );

		/// <summary>Remaps a range from the <c>input</c> range to the <c>output</c> range</summary>
		/// <param name="value">The range to remap</param>
		/// <param name="input">The input range</param>
		/// <param name="output">The output range</param>
		public static FloatRange Remap( FloatRange value, FloatRange input, FloatRange output ) => new FloatRange(Remap( value.a, input, output ), Remap( value.b, input, output ));

		/// <summary>Returns whether or not this range overlaps another range</summary>
		/// <param name="other">The other range to test overlap with</param>
		public bool Overlaps( FloatRange other ) {
			float separation = Mathfs.Abs( other.Center - Center );
			float rTotal = ( other.Length + Length ) / 2;
			return separation < rTotal;
		}

		/// <summary>Wraps/repeats the input value to stay within this range</summary>
		/// <param name="value">The value to wrap/repeat in this interval</param>
		public float Wrap( float value ) => a + Mathfs.Repeat( value - a, b - a );

		/// <summary>Clamps the input value to this range</summary>
		/// <param name="value">The value to clamp to this interval</param>
		public float Clamp( float value ) => Mathfs.Clamp( value, Min, Max );

		/// <summary>Expands the minimum or maximum value to contain the given <c>value</c></summary>
		/// <param name="value">The value to include</param>
		public FloatRange Encapsulate( float value ) =>
			Direction switch {
				1 => ( Mathfs.Min( a, value ), Mathfs.Max( b, value ) ), // forward - a is min, b is max
				_ => ( Mathfs.Min( b, value ), Mathfs.Max( a, value ) ) // reversed - b is min, a is max
			};

		/// <summary>Returns a version of this range, scaled around its start value</summary>
		/// <param name="scale">The value to scale the range by</param>
		public FloatRange ScaleFromStart( float scale ) => new FloatRange( a, a + scale * ( b - a ) );

		/// <summary>Returns this range mirrored around a given value</summary>
		/// <param name="pivot">The value to mirror around</param>
		public FloatRange MirrorAround( float pivot ) => new FloatRange( 2 * pivot - a, 2 * pivot - b );

		/// <summary>Returns a reversed version of this range, where a and b is swapped</summary>
		public FloatRange Reverse() => ( b, a );

		/// <summary>Returns the rectangle encapsulating the region defined by a range per axis. Note: The direction of each range is ignored</summary>
		/// <param name="rangeX">The range of the X axis</param>
		/// <param name="rangeY">The range of the Y axis</param>
		public static Rect ToRect( FloatRange rangeX, FloatRange rangeY ) => new Rect( rangeX.Min, rangeY.Min, rangeX.Length, rangeY.Length );

		/// <summary>Returns the bounding box encapsulating the region defined by a range per axis. Note: The direction of each range is ignored</summary>
		/// <param name="rangeX">The range of the X axis</param>
		/// <param name="rangeY">The range of the Y axis</param>
		/// <param name="rangeZ">The range of the Z axis</param>
		public static Bounds ToBounds( FloatRange rangeX, FloatRange rangeY, FloatRange rangeZ ) {
			Vector3 center = new Vector3(rangeX.Center, rangeY.Center, rangeZ.Center);
			Vector3 size = new Vector3(rangeX.Length, rangeY.Length, rangeZ.Length);
			return new Bounds( center, size );
		}

		public static FloatRange operator -( FloatRange range, float v ) => new FloatRange(range.a - v, range.b - v);
		public static FloatRange operator +( FloatRange range, float v ) => new FloatRange(range.a + v, range.b + v);

		public static implicit operator FloatRange( (float a, float b) tuple ) => new FloatRange( tuple.a, tuple.b );
		public static bool operator ==( FloatRange a, FloatRange b ) => a.a == b.a && a.b == b.b;
		public static bool operator !=( FloatRange a, FloatRange b ) => a.a != b.a || a.b != b.b;
		public bool Equals( FloatRange other ) => a.Equals( other.a ) && b.Equals( other.b );
		public override bool Equals( object obj ) => obj is FloatRange other && Equals( other );
		//public override int GetHashCode() => HashCode.Combine( a, b );
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = 17;
                hashCode = hashCode * 23 + a.GetHashCode();
                hashCode = hashCode * 23 + b.GetHashCode();
                //hashCode = hashCode * 23 + m2.GetHashCode();
                //hashCode = hashCode * 23 + m3.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString() => $"[{a},{b}]";

	}

}