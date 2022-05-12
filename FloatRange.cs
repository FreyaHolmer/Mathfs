// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

namespace Freya {

	/// <summary>A value range between two values a and b</summary>
	public readonly struct FloatRange {

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
		public int Sign => b > a ? 1 : -1;

		/// <summary>Interpolates a value from <c>a</c> to <c>b</c>, based on a parameter <c>t</c></summary>
		/// <param name="t">The normalized interpolant from <c>a</c> to <c>b</c>. A value of 0 returns <c>a</c>, a value of 1 returns <c>b</c></param>
		public float Lerp( float t ) => Mathfs.Lerp( a, b, t );

		/// <summary>Returns the normalized position of the input value <c>v</c> within this range</summary>
		/// <param name="v">The value to get the normalized position of</param>
		public float InverseLerp( float v ) => Mathfs.InverseLerp( a, b, v );

		/// <summary>Returns whether or not this range contains the value <c>v</c></summary>
		/// <param name="v">The value to see if it's inside</param>
		public bool Contains( float v ) => v >= Min && v <= Max;

		/// <summary>Remaps the input value from the <c>input</c> range to the <c>output</c> range</summary>
		/// <param name="value">The value to remap</param>
		/// <param name="input">The input range</param>
		/// <param name="output">The output range</param>
		public static float Remap( float value, FloatRange input, FloatRange output ) => output.Lerp( input.InverseLerp( value ) );

		/// <summary>Returns whether or not this range overlaps another range</summary>
		/// <param name="other">The other range to test overlap with</param>
		public bool Overlaps( FloatRange other ) {
			float separation = Mathfs.Abs( other.Center - Center );
			float rTotal = ( other.Length + Length ) / 2;
			return separation < rTotal;
		}

		/// <summary>Expands the minimum or maximum value to contain the given <c>value</c></summary>
		/// <param name="value">The value to include</param>
		public FloatRange Encapsulate( float value ) =>
			Sign switch {
				1 => ( Mathfs.Min( a, value ), Mathfs.Max( b, value ) ), // forward - a is min, b is max
				_ => ( Mathfs.Min( b, value ), Mathfs.Max( a, value ) ) // reversed - b is min, a is max
			};

		public static implicit operator FloatRange( (float a, float b) tuple ) => new FloatRange( tuple.a, tuple.b );

	}

}