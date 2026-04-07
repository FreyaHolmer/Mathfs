// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using System;
using UnityEngine;

namespace Freya {}

namespace Freya {

	/// <summary>A fixed precision data type for half-integers, using a single backing interger. For numbers like: 0, 0.5, 1, 1.5, 2, etc.</summary>
	[Serializable] public struct inth :
		IComparable<inth>,
		IEquatable<inth>,
		INumber<inth, int>,
		ISignedNumber<int>,
		IHalfNumber<int>,
		IRoundable<int> {
		/// <summary>The number of halves</summary>
		[SerializeField] public int h;

		// public intHalf( int halves ) => this.h = halves;
		public inth fromInt( int intValue ) => this.h = intValue * 2;

		public bool isInteger => h % 2 == 0;
		public int sign => Math.Sign( h );
		public inth abs => new() { h = Math.Abs( h ) };
		public inth max( inth other ) => this > other ? this : other;
		public inth min( inth other ) => this < other ? this : other;
		public static int zero => 0;
		public static inth half => new() { h = 1 };
		public static int one => 1;
		public int times2 => h;

		public static inth fromFloat( float v, RoundingDirection rounding = RoundingDirection.ToEven ) => new() { h = mathfs.round( v * 2, rounding ) };

		public int round( RoundingDirection rounding = RoundingDirection.ToEven ) {
			if( isInteger )
				return h / 2;
			int rUp = ( h + 1 ) / 2;
			return rounding switch {
				RoundingDirection.AwayFromZero       => h > 0 ? rUp : rUp - 1,
				RoundingDirection.ToZero             => h < 0 ? rUp : rUp - 1,
				RoundingDirection.ToNegativeInfinity => rUp - 1, // floor(x)
				RoundingDirection.ToPositiveInfinity => rUp, // ceil(x)
				RoundingDirection.ToEven or _        => rUp % 2 == 0 ? rUp : rUp - 1,
			};
		}

		public int floorToward0 => round( RoundingDirection.ToZero );
		public int ceilAwayFrom0 => round( RoundingDirection.AwayFromZero );
		public int floor => round( RoundingDirection.ToNegativeInfinity );
		public int ceil => round( RoundingDirection.ToPositiveInfinity );

		public override string ToString() => isInteger ? $"{h / 2}" : $"{h / 2}.5";

		// public static intHalf Lerp( intHalf a, intHalf b, intHalf t ) => a + t * ( b - a );
		// public static intHalf InverseLerp( intHalf a, intHalf b, intHalf v ) => ( v - a ) / ( b - a );

		public static implicit operator rat( inth ih ) => new(ih.h, 2);
		public static implicit operator inth( int i ) => new() { h = i * 2 };
		public static explicit operator float( inth i ) => i.h / 2f; // todo: this is a little wasteful prolly

		// unary operations
		public static inth operator -( inth ih ) => new() { h = -ih.h };
		public static inth operator +( inth ih ) => ih;

		// binary operations
		public static inth operator +( inth a, inth b ) => new() { h = a.h + b.h };
		public static inth operator +( inth a, int b ) => new() { h = a.h + b * 2 };
		public static inth operator +( int a, inth b ) => new() { h = a * 2 + b.h };
		public static inth operator -( inth a, inth b ) => new() { h = a.h - b.h };
		public static inth operator -( inth a, int b ) => new() { h = a.h - b * 2 };
		public static inth operator -( int a, inth b ) => new() { h = a * 2 - b.h };
		public static inth operator *( inth a, int b ) => new() { h = a.h * b };
		public static inth operator *( int a, inth b ) => new() { h = a * b.h };

		// comparison operators
		public static bool operator ==( inth a, inth b ) => a.CompareTo( b ) == 0;
		public static bool operator !=( inth a, inth b ) => a.CompareTo( b ) != 0;
		public static bool operator <( inth a, inth b ) => a.CompareTo( b ) < 0;
		public static bool operator >( inth a, inth b ) => a.CompareTo( b ) > 0;
		public static bool operator <=( inth a, inth b ) => a.CompareTo( b ) <= 0;
		public static bool operator >=( inth a, inth b ) => a.CompareTo( b ) >= 0;

		// comparison functions
		public int CompareTo( inth other ) => h.CompareTo( other.h );
		public bool Equals( inth other ) => h == other.h;
		public override bool Equals( object obj ) => obj is inth other && Equals( other );
		public override int GetHashCode() => h.GetHashCode();
	}

}