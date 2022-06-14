// by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using System;
using UnityEngine;

namespace Freya {

	/// <summary>A 4x1 column matrix with Vector2 values</summary>
	public readonly struct Vector2Matrix4x1 {

		public readonly Vector2 m0, m1, m2, m3;

		public Vector2Matrix4x1( Vector2 m0, Vector2 m1, Vector2 m2, Vector2 m3 ) => ( this.m0, this.m1, this.m2, this.m3 ) = ( m0, m1, m2, m3 );

		public Vector2Matrix4x1( Matrix4x1 x, Matrix4x1 y ) {
			m0 = new Vector2( x.m0, y.m0 );
			m1 = new Vector2( x.m1, y.m1 );
			m2 = new Vector2( x.m2, y.m2 );
			m3 = new Vector2( x.m3, y.m3 );
		}

		public Vector2 this[ int column ] =>
			column switch {
				0 => m0, 1 => m1, 2 => m2, 3 => m3,
				_ => throw new IndexOutOfRangeException( $"Matrix column index has to be from 0 to 3, got: {column}" )
			};

		public Matrix4x1 X => new(m0.x, m1.x, m2.x, m3.x);
		public Matrix4x1 Y => new(m0.y, m1.y, m2.y, m3.y);
		
		public static bool operator ==( Vector2Matrix4x1 a, Vector2Matrix4x1 b ) => a.m0 == b.m0 && a.m1 == b.m1 && a.m2 == b.m2 && a.m3 == b.m3;
		public static bool operator !=( Vector2Matrix4x1 a, Vector2Matrix4x1 b ) => !( a == b );
		public bool Equals( Vector2Matrix4x1 other ) => m0.Equals( other.m0 ) && m1.Equals( other.m1 ) && m2.Equals( other.m2 ) && m3.Equals( other.m3 );
		public override bool Equals( object obj ) => obj is Vector2Matrix4x1 other && Equals( other );
		public override int GetHashCode() => HashCode.Combine( m0, m1, m2, m3 );

	}

}